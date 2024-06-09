using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Vk;
using Vulkan;

namespace Artifact.Plugins.Rendering.VeldridBackend
{
    public class VeldridRenderingBackend : ArtifactDisposable, IRenderingBackend
    {
        public Type VisualImplementation => typeof(VeldridVisual);

        public string FancyName { get; set; }

        public static GraphicsDevice device;
        public static CommandList commandList;
        public static ResourceFactory factory;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        public void Predraw()
        {
            commandList.Begin();
            commandList.SetFramebuffer(device.SwapchainFramebuffer);
        }

        public void Clear(ColorRGB color, float alpha)
        {
            commandList.ClearColorTarget(0, new RgbaFloat(color.UnitR, color.UnitG, color.UnitB, color.UnitA));
        }

        public void CreateContext(object windowHandle, int width, int height)
        {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };
            SwapchainSource source;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                source = SwapchainSource.CreateWin32((nint)windowHandle, GetModuleHandle(null));
            } else
            {
                var data = ((nint, nint))windowHandle;

                source = SwapchainSource.CreateXlib(data.Item1, data.Item2);
            }

            SwapchainDescription desc = new SwapchainDescription(
                source,
                (uint)width, (uint)height,
                options.SwapchainDepthFormat,
                options.SyncToVerticalBlank,
                options.SwapchainSrgbFormat
            );
            device = GraphicsDevice.CreateVulkan(options, desc);
            FancyName = "Vulkan";

            factory = device.ResourceFactory;

            commandList = factory.CreateCommandList();
        }

        public override void Dispose()
        {
            commandList.Dispose();
            device.Dispose();
        }

        public void SwapBuffers()
        {
            try
            {
                commandList.End();
                device.SubmitCommands(commandList);
                device.SwapBuffers();
            } catch (VeldridException)
            {
                Environment.Exit(0);
            }
            
        }
    }
}
