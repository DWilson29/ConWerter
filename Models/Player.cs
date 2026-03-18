using CommunityToolkit.Mvvm.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Ownaudio.Core;
using OwnaudioNET.Core;
using OwnaudioNET.Mixing;
using OwnaudioNET.Sources;

namespace ConWerter.Models
{
    internal static class Player
    {
      private static string ShortBeep = "shortbeep.wav";
      private static string LongBeep = "longbeep.wav";

      private static FileSource fileSource0;
      private static FileSource fileSource1;

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

        var Engine = OwnaudioNet.Engine!.UnderlyingEngine;

        AudioMixer mixer = new AudioMixer(Engine, bufferSizeInFrames: 512);

        mixer.MasterVolume = 0.8f;

        mixer.SourceError += (sender, e) =>
        {
            Console.WriteLine($"Source error: {e.Message}");
        };

        int targetSampleRate = OwnaudioNet.Engine!.Config.SampleRate;
        int targetChannels = OwnaudioNet.Engine!.Config.Channels;

        fileSource0 = new FileSource(ShortBeep, 8192, targetSampleRate, targetChannels);
        fileSource1 = new FileSource(LongBeep, 8192, targetSampleRate, targetChannels);

        mixer.AddSource(fileSource0);
        mixer.AddSource(fileSource1);

        mixer.Start();
      }

      static public void Beep(bool isLong, float volume)
      {
        if(isLong)
        {
          fileSource1.Play();

          while(fileSource1.State == AudioState.Playing){
            Thread.Sleep(100);
          }}
        else{
          fileSource0.Play();

          while(fileSource1.State == AudioState.Playing){
            Thread.Sleep(100);
          }
        }
      }
    }
}
