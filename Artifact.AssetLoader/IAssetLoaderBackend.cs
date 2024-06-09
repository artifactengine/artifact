using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetLoading
{
    public interface IAssetLoaderBackend
    {
        public void Create(string path);
        public string ReadAllText(string path);
        public byte[] ReadAllBytes(string path);
        public void WriteAllText(string path, string content);
        public bool Exists(string path);
    }
}
