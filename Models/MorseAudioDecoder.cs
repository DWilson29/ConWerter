using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Dsp;
using NAudio.Wave;
using NLayer;
using NLayer.NAudioSupport;

namespace ConWerter.Models
{
    /// <summary>
    /// Decodes an mp3/wav audio recording of morse code into its morse and plain text
    /// representations. Audio "strength" over time is derived from a Fast Fourier Transform
    /// of successive windows of the signal; windows whose energy is above a threshold are
    /// treated as "signal present" (tone on), the rest as "signal absent" (tone off).
    /// The relative durations of the on/off runs are then classified as short/long to
    /// reconstruct dots, dashes, and the gaps between letters and words.
    /// </summary>
    public static class MorseAudioDecoder
    {
        // Must be a power of two for the FFT implementation.
        public const int DefaultFftLength = 64;

        public sealed class DecodeResult
        {
            public string Morse { get; init; } = "";
            public string Text { get; init; } = "";
        }

        public static DecodeResult Decode(string filePath, int fftLength = DefaultFftLength)
        {
            if (fftLength <= 0 || (fftLength & (fftLength - 1)) != 0)
            {
                throw new ArgumentException("FFT length must be a power of two.", nameof(fftLength));
            }

            int fftPow = (int)Math.Log2(fftLength);
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // ManagedMpegStream (NLayer) is a pure-managed, cross-platform mp3 decoder.
            // NAudio's own Mp3FileReader relies on Windows ACM and isn't available outside
            // of NAudio's Windows-targeted build, so it can't be used here.
            using WaveStream reader = extension switch
            {
                ".mp3" => new ManagedMpegStream(filePath, StereoMode.DownmixToMono),
                ".wav" => new WaveFileReader(filePath),
                _ => throw new NotSupportedException($"Unsupported audio format '{extension}'. Only .mp3 and .wav files are supported."),
            };

            ISampleProvider sampleProvider = reader.ToSampleProvider();
            int channels = sampleProvider.WaveFormat.Channels;
            int sampleRate = sampleProvider.WaveFormat.SampleRate;

            if (channels <= 0 || sampleRate <= 0)
            {
                throw new InvalidDataException("The selected audio file does not contain readable audio data.");
            }

            double frameDurationSeconds = (double)fftLength / sampleRate;

            List<double> frameEnergies = ComputeFrameEnergies(sampleProvider, channels, fftLength, fftPow);

            if (frameEnergies.Count == 0)
            {
                return new DecodeResult();
            }

            bool[] active = ClassifyActiveFrames(frameEnergies);
            return BuildMorseAndText(active, frameDurationSeconds);
        }

        private static List<double> ComputeFrameEnergies(ISampleProvider sampleProvider, int channels, int fftLength, int fftPow)
        {
            var energies = new List<double>();
            float[] interleaved = new float[fftLength * channels];
            Complex[] fftBuffer = new Complex[fftLength];

            int read;
            while ((read = ReadFully(sampleProvider, interleaved)) > 0)
            {
                int framesRead = read / channels;

                for (int i = 0; i < fftLength; i++)
                {
                    float sample = 0f;
                    if (i < framesRead)
                    {
                        float sum = 0f;
                        for (int c = 0; c < channels; c++)
                        {
                            sum += interleaved[i * channels + c];
                        }
                        sample = sum / channels;
                    }

                    // Hamming window to reduce spectral leakage at the edges of the frame.
                    double window = FastFourierTransform.HammingWindow(i, fftLength);
                    fftBuffer[i].X = (float)(sample * window);
                    fftBuffer[i].Y = 0f;
                }

                FastFourierTransform.FFT(true, fftPow, fftBuffer);

                double energy = 0;
                for (int i = 0; i < fftLength / 2; i++)
                {
                    double real = fftBuffer[i].X;
                    double imaginary = fftBuffer[i].Y;
                    energy += Math.Sqrt(real * real + imaginary * imaginary);
                }

                energies.Add(energy);
            }

            return energies;
        }

        private static int ReadFully(ISampleProvider sampleProvider, float[] buffer)
        {
            int totalRead = 0;
            while (totalRead < buffer.Length)
            {
                int read = sampleProvider.Read(buffer.AsSpan(totalRead));
                if (read == 0)
                {
                    break;
                }
                totalRead += read;
            }
            return totalRead;
        }

        private static bool[] ClassifyActiveFrames(List<double> energies)
        {
            double max = energies.Max();
            if (max <= 0)
            {
                return new bool[energies.Count];
            }

            // Frames whose energy is above a quarter of the peak energy are considered
            // "signal present" (the tone is on).
            double threshold = max * 0.25;
            return energies.Select(e => e >= threshold).ToArray();
        }

        private readonly struct Run
        {
            public Run(bool active, int length)
            {
                Active = active;
                Length = length;
            }

            public bool Active { get; }
            public int Length { get; }
        }

        private static DecodeResult BuildMorseAndText(bool[] active, double frameDurationSeconds)
        {
            List<Run> runs = GroupIntoRuns(active);

            List<Run> onRuns = runs.Where(r => r.Active).ToList();
            if (onRuns.Count == 0)
            {
                return new DecodeResult();
            }

            // The unit length is the duration of a "dot" and is derived from the shortest
            // tone-on run detected in the recording.
            int unit = Math.Max(1, onRuns.Min(r => r.Length));

            // null entries mark a word boundary between the surrounding letter tokens.
            var tokens = new List<string?>();
            var currentLetter = new StringBuilder();
            bool sawSignal = false;

            void FlushLetter()
            {
                if (currentLetter.Length > 0)
                {
                    tokens.Add(currentLetter.ToString());
                    currentLetter.Clear();
                }
            }

            foreach (Run run in runs)
            {
                if (run.Active)
                {
                    sawSignal = true;
                    bool isDash = run.Length >= unit * 2;
                    currentLetter.Append(isDash ? '-' : '.');
                }
                else
                {
                    if (!sawSignal)
                    {
                        // Ignore leading silence before the first tone.
                        continue;
                    }

                    if (run.Length >= unit * 6)
                    {
                        // Word gap.
                        FlushLetter();
                        if (tokens.Count > 0 && tokens[^1] != null)
                        {
                            tokens.Add(null);
                        }
                    }
                    else if (run.Length >= unit * 2)
                    {
                        // Letter gap.
                        FlushLetter();
                    }
                    // Otherwise this is a short intra-letter gap and requires no separator.
                }
            }
            FlushLetter();

            // Trailing silence should not leave a dangling word boundary marker.
            while (tokens.Count > 0 && tokens[^1] == null)
            {
                tokens.RemoveAt(tokens.Count - 1);
            }

            var morse = new StringBuilder();
            var text = new StringBuilder();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == null)
                {
                    morse.Append(" / ");
                    text.Append(' ');
                }
                else
                {
                    if (morse.Length > 0 && tokens[i - 1] != null)
                    {
                        morse.Append(' ');
                    }
                    morse.Append(tokens[i]);
                    text.Append(Converter.MorseToChar(tokens[i]!) ?? '?');
                }
            }

            return new DecodeResult
            {
                Morse = morse.ToString(),
                Text = text.ToString(),
            };
        }

        private static List<Run> GroupIntoRuns(bool[] active)
        {
            var runs = new List<Run>();
            if (active.Length == 0)
            {
                return runs;
            }

            bool current = active[0];
            int length = 1;

            for (int i = 1; i < active.Length; i++)
            {
                if (active[i] == current)
                {
                    length++;
                }
                else
                {
                    runs.Add(new Run(current, length));
                    current = active[i];
                    length = 1;
                }
            }
            runs.Add(new Run(current, length));

            return runs;
        }
    }
}
