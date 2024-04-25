using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Artifact
{
    public class Application : GameLoop
    {
        public static Application current;

        public string Name { get; set; } = "UNSET";
        public int TargetFPS { get; set; } = 60;
        public float FPS { get; set; } = 60;
        public bool IsOpen { get; set; } = true;

        public static List<ArtifactDisposable> Disposables { get; set; } = new List<ArtifactDisposable>();

        public override void OnLoad() { }
        public override void OnUpdate(float dt)
        {
            foreach (PluginBase plugin in plugins)
            {
                plugin.OnUpdate(dt);
            }
        }
        public override void OnExit()
        {
            foreach (PluginBase plugin in plugins)
            {
                plugin.OnExit();
            }
        }

        public Logger logger = LogManager.GetCurrentClassLogger();

        public List<PluginBase> plugins = new List<PluginBase>();

        public void AddPlugin(PluginBase plugin)
        {
            plugins.Add(plugin);
            //plugin.OnLoad();

            logger.Info($"Loaded plugin {plugin.BundleID}");
        }

        public T GetPlugin<T>() where T : PluginBase
        {
            return plugins.First(p => p.GetType() == typeof(T)) as T;
        }

        public static void Run<T>() where T : Application
        {
            Application app = Activator.CreateInstance<T>();

            app.logger.Info($"Starting application {app.Name}...");
            app.logger.Info($"Target FPS: {app.TargetFPS}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long targetElapsedTime = Stopwatch.Frequency / app.TargetFPS; // Target time per frame for 60 FPS
            long lastTime = stopwatch.ElapsedTicks;

            foreach (PluginBase plugin in app.plugins)
            {
                plugin.OnLoad();
            }

            app.OnLoad();

            while (app.IsOpen)
            {
                long currentTime = stopwatch.ElapsedTicks;
                long elapsedTime = currentTime - lastTime;

                if (elapsedTime < targetElapsedTime)
                {
                    continue;
                }

                // Calculate delta time
                float dt = (float)elapsedTime / Stopwatch.Frequency;

                app.FPS = 1 / dt;

                app.OnUpdate(dt);

                lastTime = currentTime;
            }

            app.OnExit();

            app.logger.Info("Disposing...");

            GarbageCollect();
        }

        public static void GarbageCollect()
        {
            foreach (ArtifactDisposable disposable in Disposables)
            {
                disposable.Dispose();
            }
        }

        public Application()
        {
            current = this;
        }
    }
}
