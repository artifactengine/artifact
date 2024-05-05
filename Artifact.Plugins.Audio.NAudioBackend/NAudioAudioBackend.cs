using Artifact.Plugin.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Audio.NAudioBackend
{
    public class NAudioBackend : ArtifactDisposable, IAudioBackend
    {
        private Dictionary<string, WaveStream> waveStreamCache = new Dictionary<string, WaveStream>();

        private void PlayWavInternal(string path)
        {
            try
            {
                if (!waveStreamCache.ContainsKey(path))
                {
                    FileStream wavStream = File.OpenRead(path);
                    WaveStream ws1 = new WaveFileReader(wavStream);
                    ws1 = WaveFormatConversionStream.CreatePcmStream(ws1);
                    waveStreamCache[path] = ws1;
                }

                WaveStream ws = waveStreamCache[path];
                ws.Position = 0; // Reset position to start

                WaveOutEvent output = new WaveOutEvent();
                output.Init(ws);
                output.Play();
            } catch (NullReferenceException)
            {

            }
            
        }

        public void PlayWav(string path)
        {
            PlayWavInternal(path);
        }

        public override void Dispose()
        {
            foreach ((string _, WaveStream stream) in waveStreamCache)
            {
                stream.Close();
            }
        }

        public void PlayWavAndWait(string path)
        {
            try
            {
                if (!waveStreamCache.ContainsKey(path))
                {
                    FileStream wavStream = File.OpenRead(path);
                    WaveStream ws1 = new WaveFileReader(wavStream);
                    ws1 = WaveFormatConversionStream.CreatePcmStream(ws1);
                    waveStreamCache[path] = ws1;
                }

                WaveStream ws = waveStreamCache[path];
                if (ws == null || !Application.current.IsOpen)
                {
                    Console.WriteLine("Close from audio");
                    ws.Dispose();
                    return;
                }
                ws.Seek(0, SeekOrigin.Begin);

                WaveOutEvent output = new WaveOutEvent();
                output.Init(ws);
                output.Play();

                while (output.PlaybackState == PlaybackState.Playing) ;
            }
            catch (Exception _)
            {

            }
        }
    }
}
