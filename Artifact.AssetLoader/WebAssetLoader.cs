using Artifact.WASM.Common;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetLoading
{
    public class WebAssetLoader : IAssetLoaderBackend
    {
        private ZipArchive archive;

        public WebAssetLoader()
        {
            // Load url as zip file
            byte[] assets = WASMCommon.current.assets;

            MemoryStream stream = new MemoryStream(assets);

            archive = new ZipArchive(stream, ZipArchiveMode.Read);
        }

        public void Create(string path)
        {
            // Do nothing
        }

        public bool Exists(string path)
        {
            // See if path exists in zip file
            return archive.Entries.Any(e => e.FullName == path);
        }

        public byte[] ReadAllBytes(string path)
        {
            // Read file from path within zip file
            var entry = archive.GetEntry(path);
            if (entry != null)
            {
                using (var streamReader = entry.Open())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        streamReader.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            return null;
        }

        public string ReadAllText(string path)
        {
            // Read file from path within zip file
            var entry = archive.GetEntry(path);
            if (entry != null)
            {
                using (var streamReader = new StreamReader(entry.Open()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            return null;

        }

        public void WriteAllText(string path, string content)
        {
            // Write content to file from path within zip file
            var entry = archive.CreateEntry(path);
            using (var streamWriter = new StreamWriter(entry.Open()))
            {
                streamWriter.Write(content);
            }
        }
    }
}
