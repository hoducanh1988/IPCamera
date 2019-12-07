using NAudio.Wave;

namespace IPCameraIndoorControlLibrary.Common.IO {
    public class Speaker {
        IWavePlayer waveOutDevice = null;
        AudioFileReader audioFileReader = null;

        public Speaker(string sound_filefullname) {
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(sound_filefullname);
            waveOutDevice.Init(audioFileReader);
        }

        public bool Play() {
            try {
                waveOutDevice.Play();
            }
            catch { return false; }
            return true;
        }

        public bool IsPlaying() {
            try {
                return waveOutDevice.PlaybackState == PlaybackState.Playing;
            }
            catch { return false; }
        }

        public bool Stop() {
            try {
                waveOutDevice.Stop();
                audioFileReader.Dispose();
                waveOutDevice.Dispose();
            }
            catch { return false; }
            return true;
        }

    }
}
