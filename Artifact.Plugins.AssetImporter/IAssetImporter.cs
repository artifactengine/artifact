using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetImporter
{
    public interface IAssetImporter
    {
        public string Extention { get; set; }

        public object Import(string path);
    }
}
