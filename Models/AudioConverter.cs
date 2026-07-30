using Ownaudio.Core;
using OwnAudio;
using OwnaudioNET.Recording;

namespace ConWerter.Models
{
    public static class AudioConverter
    {
        private static AudioRecorder? _recorder;
        public static void StartRecording(string outputPath)
        {
            StopRecording();

            AudioConfig config = new()
            {
                SampleRate = 48000,
                Channels = 2,
                BufferSize = 512
            };
            _recorder = new(config);
            _recorder.StartRecording(outputPath);
        }

        public static void StopRecording()
        {
            if (_recorder != null)
            {
                _recorder.StopRecording();
                _recorder.Dispose();
                _recorder = null;
            }
        }
    }
}