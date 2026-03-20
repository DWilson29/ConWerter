using System;
using Ownaudio.Core;
using OwnaudioNET.Core;
using OwnaudioNET.Mixing;
using OwnaudioNET.Sources;
using OwnaudioNET;
using System.Threading;

namespace ConWerter.Models
{
  internal static class Player
  {
    private static string ShortBeep = "Assets/shortbeep.wav";
    private static string LongBeep = "Assets/longbeep.wav";

    private static AudioMixer mixer;

    static Player()
    {
      AudioConfig config = new AudioConfig()
      {
        SampleRate = 48000,
        Channels = 2,
        BufferSize = 512
      };

      OwnaudioNet.Initialize(config);
      OwnaudioNet.Start();

      IAudioEngine Engine = OwnaudioNet.Engine!.UnderlyingEngine;

      mixer = new(Engine, bufferSizeInFrames: 512);

      // pull from UI
      mixer.MasterVolume = 0.8f;

      mixer.SourceError += (sender, e) =>
      {
        Console.WriteLine($"Source error: {e.Message}");
      };
      mixer.Start();
    }

    static public void Beep(bool isLong, float volume)
    {
      int targetSampleRate = OwnaudioNet.Engine!.Config.SampleRate;
      int targetChannels = OwnaudioNet.Engine!.Config.Channels;

      FileSource fs;
      if (isLong)
      {
        fs = new(ShortBeep, 8192, targetSampleRate, targetChannels);
        mixer.AddSource(fs);
        fs.Play();

        while (fs.State == AudioState.Playing)
        {
          Thread.Sleep(100);
        }
      }
      else
      {
        fs = new(LongBeep, 8192, targetSampleRate, targetChannels);
        mixer.AddSource(fs);
        fs.Play();

        while (fs.State == AudioState.Playing)
        {
          Thread.Sleep(100);
        }
      }
      mixer.RemoveSource(fs);
      fs.Dispose();
    }
  }
}
