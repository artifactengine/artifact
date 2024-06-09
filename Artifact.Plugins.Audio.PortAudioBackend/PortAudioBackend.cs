using Artifact.Plugin.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortAudioSharp;
using NLog;
using System.Runtime.ExceptionServices;

namespace Artifact.Plugins.Audio.PortAudioBackend
{
    public class PortAudioBackend : IAudioBackend
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        int deviceIndex;
        DeviceInfo devInfo;

        public PortAudioBackend()
        {
            PortAudio.Initialize();

            logger.Info(PortAudio.VersionInfo.versionText);
            logger.Info("Device Count: " + PortAudio.DeviceCount);

            deviceIndex = PortAudio.DefaultOutputDevice;
            if (deviceIndex == PortAudio.NoDevice)
            {
                logger.Error("No default output device");
                Environment.Exit(-1);
            }

            devInfo = PortAudio.GetDeviceInfo(deviceIndex);

            logger.Info("Using device " + devInfo.name);
        }

        public void CreateAudioStream(string wavPath)
        {
            StreamParameters param = new StreamParameters();
            param.device = deviceIndex;
            param.channelCount = 1;
            param.sampleFormat = SampleFormat.Int24;
        }

        public void PlayWav(string path)
        {
            
        }

        public void PlayWavAndWait(string path)
        {
            
        }
    }
}
