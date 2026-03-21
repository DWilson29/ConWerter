using System;
using Ownaudio.Core;
using OwnaudioNET.Core;
using OwnaudioNET.Mixing;
using OwnaudioNET.Sources;
using OwnaudioNET;
using System.Threading;
using System.Diagnostics;
using Logger;

namespace ConWerter.Models
{
  internal static class Player
  {
    private static string BeepPath = "Assets/beep.wav";
    private static float BeepMaxLength = 2;

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

      mixer.SourceError += (sender, e) =>
      {
        Console.WriteLine($"Source error: {e.Message}");
      };
      mixer.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speed">Normalized value between 0 and 1 controlling speed</param>
    /// <param name="isLong">Is it long beep?</param>
    /// <returns></returns>
    static private float GetBeepStartPosition(double speed, bool isLong, out TimeSpan length)
    {
      float secLength = (float)((isLong ? BeepMaxLength : BeepMaxLength / 2) * speed);
      length = TimeSpan.FromSeconds(secLength);
      return BeepMaxLength - secLength;
    }

    static public void Beep(bool isLong, float volume, float speed)
    {
      int targetSampleRate = OwnaudioNet.Engine!.Config.SampleRate;
      int targetChannels = OwnaudioNet.Engine!.Config.Channels;

      FileSource fs = new(BeepPath, 8192, targetSampleRate, targetChannels)
      {
          Volume = volume
      };
      mixer.AddSource(fs);
      fs.StartOffset = GetBeepStartPosition(speed, isLong, out TimeSpan length);

      Log.Info($"{length.TotalSeconds} {fs.StartOffset}");

      fs.Play();
      Thread.Sleep(length);

      mixer.RemoveSource(fs);
      fs.Dispose();
    }
  }
}
