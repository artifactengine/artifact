using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact
{
    public class GameLoopExtentionAttribute : Attribute
    {
        public string Id { get; set; }
        public List<string> Events { get; set; }

        public GameLoopExtentionAttribute(string id, List<string> events)
        {
            Id = id;
            Events = events;
        }

        public GameLoopExtentionAttribute(string id, string[] events)
        {
            Id = id;
            Events = events.ToList();
        }
    }
}
