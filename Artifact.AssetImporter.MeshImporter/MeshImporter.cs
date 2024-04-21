using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetImporter.MeshImporter
{
    public class MeshImporter : IAssetImporter
    {
        public string Extention { get => ""; set => throw new NotImplementedException(); }

        public object Import(string path)
        {
            return null;
        }
    }
}
