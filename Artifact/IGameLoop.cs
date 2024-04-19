using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Artifact
{
    public class GameLoop
    {
        public virtual void OnLoad() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate(float dt) { }
    
        public void Invoke(string evnt, params object[] args)
        {
            string id = evnt.Split(':')[0];
            string name = evnt.Split(':')[1];

            // Get all interfaces implemented by the current instance
            var interfaces = this.GetType().GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                // Get the GameLoopExtention attribute from the interface
                var attribute = interfaceType.GetCustomAttribute<GameLoopExtentionAttribute>();

                // Check if the attribute exists and matches the id
                if (attribute != null && attribute.Id == id)
                {
                    // Check if the method name exists in the attribute's Events list
                    if (attribute.Events.Contains(name))
                    {
                        // Invoke the method
                        var methodInfo = interfaceType.GetMethod(name);
                        if (methodInfo != null)
                        {
                            try
                            {
                                methodInfo.Invoke(this, args);
                            } catch (TargetInvocationException exception)
                            {
                                throw exception.InnerException!;
                            }
                            
                        }
                    }
                }
            }

        }
    }
}
