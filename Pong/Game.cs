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
using Pong.Components;

namespace Pong
{
    public class Game : Application, IRenderingGameLoopExtention
    {
        private RenderingPlugin renderer;
        private InputPlugin input;
        private AudioPlugin audio;
        private SplashScreenPlugin splashScreen;

        private FontRenderer fontRenderer;

        private Entity playerPaddle;
        private Entity botPaddle;
        private Entity ball;

        public Game()
        {
            Name = "Pong";

            AddPlugin(new WindowingPlugin(this, "Pong", 1280, 720, typeof(GLFWWindowingBackend)));
            AddPlugin(new RenderingPlugin(this, typeof(DirectXRenderingBackend)));
            AddPlugin(new InputPlugin(this));
            AddPlugin(new AudioPlugin(this, typeof(NAudioBackend)));
            AddPlugin(new SplashScreenPlugin(this, ["Assets/FullLogo.png"], 0.75f));
            AddPlugin(new DebugMenuPlugin(this));
            AddPlugin(new ECSPlugin(this));

            CreateScenes();
        }

        public void CreateScenes()
        {
            Scene mainScene = new Scene();

            mainScene.Background = new ColorRGB(155, 188, 15, 255);

            Ball ballComponent = new Ball();

            ball = new Entity();
            ball.Transform.Scale = new Vector3(32, 32, 1);
            ball.AddComponent(ballComponent);
            ball.AddComponent(new SpriteRenderer("Assets/Sprites/paddle.png"));

            mainScene.Entities.Add(ball);

            playerPaddle = new Entity();
            playerPaddle.Transform.Position = new Vector3(2100, 0, 0);
            playerPaddle.Transform.Scale = new Vector3(32, 128, 1);
            playerPaddle.AddComponent(new PlayerBat(ballComponent));
            playerPaddle.AddComponent(new SpriteRenderer("Assets/Sprites/paddle.png"));

            mainScene.Entities.Add(playerPaddle);

            botPaddle = new Entity();
            botPaddle.Transform.Position = new Vector3(-2100, 0, 0);
            botPaddle.Transform.Scale = new Vector3(32, 128, 1);
            botPaddle.AddComponent(new BotBat(ballComponent));
            botPaddle.AddComponent(new SpriteRenderer("Assets/Sprites/paddle.png"));

            mainScene.Entities.Add(botPaddle);

            SceneManager.CurrentScene = mainScene;
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
        }

        public void OnDraw()
        {
            splashScreen.Draw();
        }
    }
}
