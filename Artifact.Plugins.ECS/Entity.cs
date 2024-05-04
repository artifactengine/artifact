using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.ECS
{
    public class Entity
    {
        private List<Component> Components { get; set; } = new List<Component>();

        public Transform Transform { get; set; } = new Transform();

        public void AddComponent(Component component)
        {
            Components.Add(component);
            component.Entity = this;
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)Components.Where(c => c.GetType() == typeof(T)).First();
        }

        public void OnLoad()
        {
            foreach (var component in Components)
            {
                component.OnLoad();
            }
        }

        public void OnExit()
        {
            foreach (var component in Components)
            {
                component.OnExit();
            }
        }

        public void OnUpdate(float dt)
        {
            foreach (var component in Components)
            {
                component.OnUpdate(dt);
            }
        }

        public void Invoke(string evnt, object[] args)
        {
            foreach (var component in Components)
            {
                component.InvokeArray(evnt, args);
            }
        }
    }
}
