using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Windowing
{
    public class WindowingPluginException : Exception
    {
        public WindowingPluginException() { } 
        public WindowingPluginException(string message) : base(message) { }
    }
}
