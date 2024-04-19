using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact
{
    public class PluginBase : GameLoop
    {
        public Application Application { get; set; }

        public string BundleID { get; set; }

        public PluginBase(Application app)
        {
            Application = app;
        }
    }
}
