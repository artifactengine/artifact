using Artifact.Plugins.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Artifact.Plugins.SplashScreen
{
    public class SplashScreenPlugin : PluginBase
    {
        public string[] Images { get; set; }
        public float ImageWidth { get; set; }

        public bool show = true;

        private Stopwatch stopwatch;

        private List<IVisual> imageVisuals = new List<IVisual>();

        private RenderingPlugin renderer;

        public SplashScreenPlugin(Application app, string[] images, float imageSize) : base(app)
        {
            Images = images;
            ImageWidth = imageSize;

            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            renderer = Application.GetPlugin<RenderingPlugin>();

            float startingPosition = 0f - ((ImageWidth + 0.05f) * (Images.Length / 2)) - (Images.Length % 2 == 0 ? 0f : (ImageWidth + (0.05f + (0.05f * Images.Length))) / 2);

            if (Images.Length == 1)
            {
                startingPosition = 0;
            }

            foreach (string imagePath in Images)
            {
                Vertex[] vertices = {
                    new Vertex(new Vector4(-(ImageWidth / 2), (ImageWidth / 2), 0.5f, 1.0f), new Vector2(0.0f, 0.0f)),
                    new Vertex(new Vector4((ImageWidth / 2), -(ImageWidth / 2), 0.5f, 1.0f), new Vector2(1.0f, 1.0f)),
                    new Vertex(new Vector4(-(ImageWidth / 2), -(ImageWidth / 2), 0.5f, 1.0f), new Vector2(0.0f, 1.0f)),
                    new Vertex(new Vector4((ImageWidth / 2), (ImageWidth / 2), 0.5f, 1.0f), new Vector2(1.0f, 0.0f)),
                };

                ushort[] indices = [
                    0, 1, 2,
                    0, 3, 1
                ];

                IVisual visual = renderer.CreateVisual(new Mesh(vertices, indices, imagePath));

                visual.Position = new Vector3(startingPosition, 0, -5f);

                startingPosition += (ImageWidth * 2) + 0.05f;

                imageVisuals.Add(visual);
            }
        }

        public override void OnUpdate(float dt)
        {
            if (stopwatch.Elapsed.Seconds > 3)
            {
                stopwatch.Stop();

                show = false;
            }
        }

        public void Draw()
        {
            if (show)
            {
                renderer.Clear(new ColorRGB(45, 52, 64, 255), 1.0f);
                foreach (IVisual visual in imageVisuals)
                {
                    visual.Draw();
                }
            }
            

        }
    }
}
