using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetLoading
{
    public class FSAssetLoader : IAssetLoaderBackend
    {
        public void Create(string path)
        {
            File.Create(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);   
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
