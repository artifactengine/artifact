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
using Artifact.Plugins.DebugMenu;
using Artifact.Plugins.Rendering.Font;
using Artifact.Plugins.ECS;
using Artifact.Utils.RenderingComponents;
using Artifact.Physics.Box2D;

namespace Platformer
{
    public class Game : Application, IRenderingGameLoopExtention
    {
        private RenderingPlugin renderer;
        private InputPlugin input;
        private AudioPlugin audio;
        private SplashScreenPlugin splashScreen;

        private FontRenderer fontRenderer;

        private Entity player;
        private Entity floor;

        private Rigidbody playerRb;

        public Game()
        {
            Name = "Pong";

            AddPlugin(new WindowingPlugin(this, "Pong", 1280, 720, typeof(GLFWWindowingBackend)));
            AddPlugin(new RenderingPlugin(this, typeof(OpenGLRenderingBackend)));
            AddPlugin(new InputPlugin(this, typeof(PollingInputBackend)));
            AddPlugin(new AudioPlugin(this, typeof(NAudioBackend)));
            AddPlugin(new SplashScreenPlugin(this, ["Assets/FullLogo.png"], 0.75f));
            AddPlugin(new DebugMenuPlugin(this));
            AddPlugin(new ECSPlugin(this));
            AddPlugin(new PhysicsPlugin(this));

            CreateScenes();
        }

        public void CreateScenes()
        {
            Scene mainScene = new Scene();

            mainScene.Background = new ColorRGB(155, 188, 15, 255);


            floor = new Entity();
            floor.Transform.Position = new Vector3(0, -10.0f, 0);
            floor.Transform.Scale = new Vector3(5.0f, 0.5f, 0);
            floor.AddComponent(new SpriteRenderer("Assets/Sprites/floor.png"));
            floor.AddComponent(new Rigidbody(RigidbodyType.Static));

            mainScene.Entities.Add(floor);

            player = new Entity();
            player.Transform.Position = new Vector3(0, 10.0f, 0);
            player.Transform.Scale = new Vector3(0.5f, 0.5f, 0);
            player.AddComponent(new SpriteRenderer("Assets/Sprites/floor.png"));
            player.AddComponent(new Rigidbody(RigidbodyType.Dynamic));

            mainScene.Entities.Add(player);


            SceneManager.CurrentScene = mainScene;


            playerRb = player.GetComponent<Rigidbody>();
        }

        public override void OnLoad()
        {
            renderer = GetPlugin<RenderingPlugin>();
            input = GetPlugin<InputPlugin>();
            audio = GetPlugin<AudioPlugin>();
            splashScreen = GetPlugin<SplashScreenPlugin>();

            fontRenderer = new FontRenderer("C:\\Windows\\Fonts\\ARLRDBD.TTF", 128);

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

            Console.WriteLine(player.Transform.Position);

            playerRb.Velocity = new Vector2(2.5f, playerRb.Velocity.Y);
        }

        public void OnDraw()
        {
            splashScreen.Draw();
        }
    }
}
