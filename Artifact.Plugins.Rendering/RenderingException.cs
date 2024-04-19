using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.Rendering
{
    public class RenderingException : Exception
    {
        public RenderingException() { }
        public RenderingException(string message) : base(message) { }
    }
}
