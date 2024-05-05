using System;
using System.Collections.Generic;
using Artifact.Plugins.ECS;
using System.Text;
using System.Threading.Tasks;
using Artifact.Plugins.Rendering;
using System.Numerics;
using System.Runtime.CompilerServices;
using Artifact.Plugins.Windowing;

namespace Artifact.Utils.RenderingComponents
{
    public class SpriteRenderer : Component, IRenderingGameLoopExtention
    {
        public string SpritePath { get; set; }

        private IVisual visual;

        public SpriteRenderer(string spritePath)
        {
            SpritePath = spritePath;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            if (!File.Exists(SpritePath))
            {
                throw new Exception("Missing file");
            }

            Vertex[] vertices = {
                    new Vertex(new Vector4(-0.5f, 0.5f, 0.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                    new Vertex(new Vector4(0.5f, -0.5f, 0.0f, 1.0f), new Vector2(1.0f, 1.0f)),
                    new Vertex(new Vector4(-0.5f, -0.5f, 0.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                    new Vertex(new Vector4(0.5f, 0.5f, 0.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                };

            ushort[] indices = [
                0, 1, 2,
                    0, 3, 1
            ];

            visual = Application.current.GetPlugin<RenderingPlugin>().CreateVisual(new Mesh(vertices, indices, SpritePath));
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
        }

        public void OnDraw()
        {
            visual.Position = Transform.Position / 4.0f;
            visual.Position = new Vector3(visual.Position.X, visual.Position.Y, -5);
            visual.Scale = Transform.Scale / 4.0f;
            visual.Scale = new Vector3(visual.Scale.X, visual.Scale.Y, 1);
            visual.Draw();
        }
    }
}
