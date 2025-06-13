using AVFoundation;
using Foundation;
using Supermodel.Mobile.Runtime.Common.Services;

namespace Supermodel.Mobile.Runtime.iOS.Services;

public class AudioService : IAudioService
{
    public void Play(byte[] wavSound)
    {
        if (_player == null || !_player.Playing)
        {
            _player = AVAudioPlayer.FromData(NSData.FromArray(wavSound));
            _player!.Play();
        }
    }

    //public void StartRecording()
    //{
    //    throw new System.NotImplementedException();
    //    //var recorder = new AVAudioRecorder();
    //    //recorder.
    //}

    //public byte[] StopRecording()
    //{
    //    throw new System.NotImplementedException();
    //}

    private static AVAudioPlayer _player;
}