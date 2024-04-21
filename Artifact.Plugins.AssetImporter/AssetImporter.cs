using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.AssetImporter
{
    public static class AssetImporter
    {
        private static Dictionary<string, IAssetImporter> importers = new Dictionary<string, IAssetImporter>();
        
        public static object Import(string path)
        {
            string extention = Path.GetExtension(path);

            if (importers.ContainsKey(extention))
            {
                IAssetImporter importer = importers[extention];

                return importer.Import(path);
            } else if (importers.Count == 0)
            {
                var i = typeof(IAssetImporter);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => i.IsAssignableFrom(p));

                foreach (Type type in types)
                {
                    Type importer = type;

                    IAssetImporter imp = (Activator.CreateInstance(importer) as IAssetImporter)!;

                    importers.Add(imp.Extention, imp);
                }

                if (importers.ContainsKey(extention))
                {
                    IAssetImporter importer = importers[extention];

                    return importer.Import(path);
                }
                else
                {
                    throw new Exception("Missing importer for extention " + extention);
                }
            }
            else
            {
                throw new Exception("Missing importer for extention " + extention);
            }

        }
    }
}
