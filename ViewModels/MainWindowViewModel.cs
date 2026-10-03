using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConWerter.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConWerter.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _phrase;

        [ObservableProperty]
        private string? _cw;

        [ObservableProperty]
        private string? _cwOutput;

        [ObservableProperty]
        private string? _phraseOutput;

        [ObservableProperty]
        private double _volume = 50;

        [ObservableProperty]
        private double _speed = 50;

        [ObservableProperty]
        private string? _selectedAudioFilePath;

        [ObservableProperty]
        private string? _audioStatus;

        [ObservableProperty]
        private string? _audioMorseOutput;

        [ObservableProperty]
        private string? _audioTextOutput;

        [ObservableProperty]
        private bool _isProcessingAudio;

        // Valid FFT lengths for the Audio to CW transform, in slider-index order.
        public static readonly int[] FftLengthOptions = { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        [ObservableProperty]
        private int _fftLengthIndex = Array.IndexOf(FftLengthOptions, MorseAudioDecoder.DefaultFftLength);

        public int FftLength => FftLengthOptions[FftLengthIndex];

        partial void OnFftLengthIndexChanged(int value)
        {
            OnPropertyChanged(nameof(FftLength));
        }

        public bool IsNotProcessingAudio => !IsProcessingAudio;

        partial void OnIsProcessingAudioChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotProcessingAudio));
        }

        public async Task LoadAndConvertAudioFileAsync(string filePath)
        {
            if (IsProcessingAudio) return;

            SelectedAudioFilePath = filePath;
            AudioMorseOutput = "";
            AudioTextOutput = "";
            IsProcessingAudio = true;
            AudioStatus = $"Processing {Path.GetFileName(filePath)}...";

            try
            {
                int fftLength = FftLength;
                MorseAudioDecoder.DecodeResult result = await Task.Run(() => MorseAudioDecoder.Decode(filePath, fftLength));

                AudioMorseOutput = string.IsNullOrWhiteSpace(result.Morse) ? "(no signal detected)" : result.Morse;
                AudioTextOutput = string.IsNullOrWhiteSpace(result.Text) ? "(no signal detected)" : result.Text;
                AudioStatus = $"Converted {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                AudioStatus = $"Error: {ex.Message}";
                AudioMorseOutput = "";
                AudioTextOutput = "";
            }
            finally
            {
                IsProcessingAudio = false;
            }
        }

        [RelayCommand]
        private async Task ConvertPhrase()
        {
            if (Phrase == null) return;
            await Task.Run(() => PlaySound(Phrase));
        }

        private float GetVolume()
        {
            return (float)(Volume / 100);
        }

        private float GetSpeed()
        {
            return (float)(100 - Speed) / 100;
        }

        public void PlaySound(string text)
        {
            if (Converter.isPlaying)
            {
                return;
            }

            Converter.isPlaying = true;
            CwOutput = "";

            foreach (char c in text.ToLower())
            {
                string code = Converter.CharToMorseCode(c);
                foreach (char symbol in code.ToCharArray())
                {
                    CwOutput += symbol;

                    if (symbol == '.')
                    {
                        Player.Beep(false, GetVolume(), GetSpeed());
                    }
                    else if (symbol == '-')
                    {
                        Player.Beep(true, GetVolume(), GetSpeed());
                    }
                    System.Threading.Thread.Sleep(200); // Pause between letters
                }
                CwOutput += ' ';
            }
            Converter.isPlaying = false;
        }

        [RelayCommand]
        private async Task ConvertCw()
        {
            if (Cw == null) return;
            string value = Converter.InvertMorse(Cw);
            PhraseOutput = value;
        }
    }
}
