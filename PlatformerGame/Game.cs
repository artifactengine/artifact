using Artifact;
using Artifact.Plugins.Rendering;
using Artifact.Plugins.Rendering.DirectXBackend;
using Artifact.Plugins.Windowing;
using Artifact.Plugins.Windowing.GLFWBackend;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifact.Plugins.Input;
using Artifact.Plugins.Input.PollBackend;
using System.Numerics;
using Artifact.Plugin.Audio;
using Artifact.Plugins.Audio.NAudioBackend;
using Artifact.Plugins.SplashScreen;
using Artifact.Plugins.Rendering.OpenGLBackend;
using System.Drawing;
using Artifact.Plugins.Rendering.VeldridBackend;

namespace Platformer
{
    public class Game : Application, IRenderingGameLoopExtention
    {
        private RenderingPlugin renderer;
        private InputPlugin input;
        private AudioPlugin audio;
        private SplashScreenPlugin splashScreen;
        private bool first = true;

        private float playerVelY = 0f;
        private const float GRAVITY = 0.0002f;

        private float pipeX = 1f;

        private float pipeY = -0.25f;

        private IVisual player;
        private IVisual background1;
        private IVisual background2;
        private IVisual pipeBottom;
        private IVisual pipeTop;

        private bool gavePoint = false;
        private bool showPlayer = true;
        private bool canGetPoint = true;

        private ColorRGB color = new ColorRGB(255, 0, 0, 255);

        public Game()
        {
            Name = "Flappy Bird";

            AddPlugin(new WindowingPlugin(this, "Flappy Bird", 1280, 720, typeof(GLFWWindowingBackend)));
            AddPlugin(new RenderingPlugin(this, typeof(DirectXRenderingBackend)));
            AddPlugin(new InputPlugin(this, typeof(PollingInputBackend)));
            AddPlugin(new AudioPlugin(this, typeof(NAudioBackend)));
            AddPlugin(new SplashScreenPlugin(this, ["Assets/FullLogo.png"], 0.75f));
        }

        public override void OnLoad()
        {
            renderer = GetPlugin<RenderingPlugin>();
            input = GetPlugin<InputPlugin>();
            audio = GetPlugin<AudioPlugin>();
            splashScreen = GetPlugin<SplashScreenPlugin>();

            Vertex[] vertices = {
                new Vertex(new Vector4(-0.025f, 0.02f, 0.5f, 1.0f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector4(0.025f, -0.02f, 0.5f, 1.0f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector4(-0.025f, -0.02f, 0.5f, 1.0f), new Vector2(0.0f, 1.0f)),
                new Vertex(new Vector4(0.025f, 0.02f, 0.5f, 1.0f), new Vector2(1.0f, 0.0f)),
            };

            ushort[] indices = [
                0, 1, 2,
                0, 3, 1
            ];

            player = renderer.CreateVisual(new Mesh(vertices, indices, "Assets/Sprites/Player/yellowbird-midflap.png"));

            player.Position = new Vector3(-0.75f, 0, 0);

            Vertex[] bgVertices = {
                new Vertex(new Vector4(-0.5f, 1.0f, 0.5f, 1.0f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector4(0.5f, -1.0f, 0.5f, 1.0f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector4(-0.5f, -1.0f, 0.5f, 1.0f), new Vector2(0.0f, 1.0f)),
                new Vertex(new Vector4(0.5f, 1.0f, 0.5f, 1.0f), new Vector2(1.0f, 0.0f)),
            };

            background1 = renderer.CreateVisual(new Mesh(bgVertices, indices, "Assets/Sprites/background-day.png"));
            background2 = renderer.CreateVisual(new Mesh(bgVertices, indices, "Assets/Sprites/background-day.png"));

            background1.Position = new Vector3(0, 0, -5);
            background2.Position = new Vector3(0, 0, -5);

            Vertex[] pipeVertices = {
                new Vertex(new Vector4(-0.040625f, 0.25f, 0.5f, 1.0f), new Vector2(0.0f, 0.0f)),
                new Vertex(new Vector4(0.040625f, -0.25f, 0.5f, 1.0f), new Vector2(1.0f, 1.0f)),
                new Vertex(new Vector4(-0.040625f, -0.25f, 0.5f, 1.0f), new Vector2(0.0f, 1.0f)),
                new Vertex(new Vector4(0.040625f, 0.25f, 0.5f, 1.0f), new Vector2(1.0f, 0.0f)),
            };

            pipeBottom = renderer.CreateVisual(new Mesh(pipeVertices, indices, "Assets/Sprites/pipe-green-bottom.png"));

            pipeBottom.Position = new Vector3(0, 0, -5);

            pipeTop = renderer.CreateVisual(new Mesh(pipeVertices, indices, "Assets/Sprites/pipe-green-top.png"));

            pipeTop.Position = new Vector3(0, 0, -5);

            base.OnLoad();
        }

        public override void OnExit()
        {
            logger.Info($"Exiting...");
            base.OnExit();
        }

        public float GetRandomNumber(float minimum, float maximum)
        {
            Random random = new Random();
            return random.NextSingle() * (maximum - minimum) + minimum;
        }

        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);

            if (splashScreen.show)
            {
                return;
            }

            if (input.WasKeyPressedThisFrame(Key.Space))
            {
                audio.PlayWav("Assets/SFX/wing.wav");

                playerVelY = -0.006f;
            }

            playerVelY += GRAVITY;
            player.Position -= new Vector3(0, playerVelY, 0);

            player.Position = new Vector3(player.Position.X, player.Position.Y, -5f);

            if (showPlayer)
            {
                pipeX -= 0.006f;
            }

            RectangleF bottomPipeRect = new RectangleF(pipeBottom.Position.X - 0.040625f, pipeBottom.Position.Y - 0.25f, 0.040625f * 2, 0.25f * 2);
            RectangleF topPipeRect = new RectangleF(pipeTop.Position.X - 0.040625f, pipeTop.Position.Y - 0.25f, 0.040625f * 2, 0.25f * 2);
            RectangleF playerRect = new RectangleF(player.Position.X - 0.025f, player.Position.Y - 0.02f, 0.025f * 2, 0.02f * 2);

            if ((bottomPipeRect.IntersectsWith(playerRect) || topPipeRect.IntersectsWith(playerRect)) && showPlayer)
            {
                showPlayer = false;

                audio.PlayWav("Assets/SFX/hit.wav");

                new Thread(() =>
                {
                    Thread.Sleep(3000);
                    
                    showPlayer = true;
                    canGetPoint = true;

                    player.Position = new Vector3(-0.75f, 0, -5f);
                    playerVelY = 0;
                    pipeX = 0;
                    pipeY = -0.25f;
                }).Start();

                canGetPoint = false;
            }

            if (player.Position.Y < -1 && showPlayer)
            {
                showPlayer = false;

                audio.PlayWav("Assets/SFX/die.wav");

                new Thread(() =>
                {
                    Thread.Sleep(3000);
                    showPlayer = true;
                    canGetPoint = true;

                    player.Position = new Vector3(-0.75f, 0, -5f);
                    playerVelY = 0;
                    pipeX = 0;
                    pipeY = -0.25f;
                }).Start();

                canGetPoint = false;
            }

            if (pipeX < -0.75f && !gavePoint && canGetPoint)
            {
                audio.PlayWav("Assets/SFX/point.wav");
                gavePoint = true;
            }

            if (pipeX < -1.2f)
            {
                pipeX = 1.2f;

                pipeY = GetRandomNumber(-0.75f, -0.2f);

                Console.WriteLine(pipeY);

                gavePoint = false;
            }

            
        }

        public void OnDraw()
        {
            renderer.Clear(color, 1.0f);



            background1.Position = new Vector3(-0.4375f, 0, -5);
            background1.Draw();
            background2.Position = new Vector3(0.4375f, 0, -5);
            background2.Draw();
            if (showPlayer)
            {
                player.Draw();
            }
            

            pipeBottom.Position = new Vector3(pipeX, pipeY, -5f);

            pipeBottom.Draw();

            pipeTop.Position = new Vector3(pipeX, pipeY + 0.75f, -5f);

            pipeTop.Draw();

            splashScreen.Draw();

            renderer.SwapBuffers();
        }
    }
}
