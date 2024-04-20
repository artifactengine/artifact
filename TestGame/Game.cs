using Artifact;
using Artifact.Plugins.Rendering;
using Artifact.Plugins.Rendering.DirectXBackend;
using Artifact.Plugins.Rendering.OpenGLBackend;
using Artifact.Plugins.Windowing;
using Artifact.Plugins.Windowing.SDL2Backend;
using Artifact.Plugins.Windowing.GLFWBackend;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifact.Plugins.Windowing.WinFormsBackend;
using Artifact.Plugins.Input;
using Artifact.Plugins.Input.PollBackend;
using System.Numerics;
using Artifact.Plugins.Rendering.VeldridBackend;

namespace TestGame
{
    public class Game : Application, IRenderingGameLoopExtention
    {
        private RenderingPlugin renderer;
        private InputPlugin input;
        private bool first = true;

        private IVisual quad1;
        private IVisual quad2;

        private ColorHSB color = new ColorHSB(0, 100, 50, 255);

        public Game()
        {
            Name = "Test Game";

            AddPlugin(new WindowingPlugin(this, "Test Game", 1280, 720, typeof(SDL2WindowingBackend)));
            AddPlugin(new RenderingPlugin(this, typeof(VeldridRenderingBackend)));
            AddPlugin(new InputPlugin(this, typeof(PollingInputBackend)));

            Console.WriteLine(Camera.orthoWidth);
            Console.WriteLine(Camera.orthoHeight);
        }

        public override void OnLoad()
        {
            renderer = GetPlugin<RenderingPlugin>();
            input = GetPlugin<InputPlugin>();

            Vertex[] vertices = {
                new Vertex(new Vector4(-0.1f, 0.1f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                new Vertex(new Vector4(0.1f, -0.1f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                new Vertex(new Vector4(-0.1f, -0.1f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                new Vertex(new Vector4(0.1f, 0.1f, 0.5f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)),
            };

            ushort[] indices = [
                0, 1, 2,
                0, 3, 1
            ];

            quad1 = renderer.CreateVisual(new Mesh(vertices, indices));
            quad2 = renderer.CreateVisual(new Mesh(vertices, indices));
            base.OnLoad();
        }

        public override void OnExit()
        {
            logger.Info($"Exiting...");
            base.OnExit();
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            color.H = (color.H + 1) % 360;

            if (input.IsKeyDown(Key.W))
            {
                quad1.Position += new Vector3(0, 0.01f, 0);
            }
            if (input.IsKeyDown(Key.S))
            {
                quad1.Position += new Vector3(0, -0.01f, 0);
            }
            if (input.IsKeyDown(Key.A))
            {
                quad1.Position += new Vector3(-0.01f, 0, 0);
            }
            if (input.IsKeyDown(Key.D))
            {
                quad1.Position += new Vector3(0.01f, 0, 0);
            }

            if (input.IsKeyDown(Key.Up))
            {
                quad2.Position += new Vector3(0, 0.01f, 0);
            }
            if (input.IsKeyDown(Key.Down))
            {
                quad2.Position += new Vector3(0, -0.01f, 0);
            }
            if (input.IsKeyDown(Key.Left))
            {
                quad2.Position += new Vector3(-0.01f, 0, 0);
            }
            if (input.IsKeyDown(Key.Right))
            {
                quad2.Position += new Vector3(0.01f, 0, 0);
            }
            quad1.Position = new Vector3(quad1.Position.X, quad1.Position.Y, -5f);
            Console.WriteLine(quad1.Position);
        }

        public void OnDraw()
        {
            renderer.Clear(color.ToRgb(), 1.0f);

            quad1.Draw();
            quad2.Draw();

            renderer.SwapBuffers();
        }
    }
}
