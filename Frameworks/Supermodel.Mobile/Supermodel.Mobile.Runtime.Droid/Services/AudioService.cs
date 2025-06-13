using Android.Media;
using Java.IO;
using Supermodel.Mobile.Runtime.Common.Services;
using Supermodel.Mobile.Runtime.Droid.App;

namespace Supermodel.Mobile.Runtime.Droid.Services;

public class AudioService : IAudioService
{
    public void Play(byte[] wavSound)
    {
        if (_player == null || !_player.IsPlaying)
        {
            if (_player != null) _player.Release();
            var tempWav = File.CreateTempFile("temp", "wav", DroidFormsApplication.MainActivity.CacheDir);
            // ReSharper disable once PossibleNullReferenceException
            tempWav.DeleteOnExit();
            var fos = new FileOutputStream(tempWav);
            fos.Write(wavSound);
            fos.Close();
		                
            _player = new MediaPlayer();
            var fis = new FileInputStream(tempWav);
            _player.SetDataSource(fis.FD);
		
            _player.Prepare();
            _player.Start();
        }        
    }
		
    private static MediaPlayer _player; //AudioTrack
}