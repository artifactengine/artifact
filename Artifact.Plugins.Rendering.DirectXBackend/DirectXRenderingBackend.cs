using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Artifact.Plugins.Rendering.DirectXBackend
{
    public class DirectXRenderingBackend : IRenderingBackend
    {
        public string FancyName => "Direct3D 11";

        public Type VisualImplementation => typeof(DirectXVisual);

        internal static Device device;
        internal static SwapChain swapchain;
        internal static DeviceContext deviceContext;
        internal static Factory factory;
        internal static Texture2D backBuffer;
        internal static RenderTargetView renderView;

        private Logger logger = LogManager.GetCurrentClassLogger();

        public void Clear(ColorRGB color, float alpha)
        {
            deviceContext.ClearRenderTargetView(renderView, new RawColor4(color.UnitR, color.UnitG, color.UnitB, color.UnitA));
        }

        public void CreateContext(nint windowHandle, int width, int height)
        {
            SwapChainDescription swapchainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                                   new ModeDescription(width, height,
                                                       new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = windowHandle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapchainDesc, out device, out swapchain);
            deviceContext = device.ImmediateContext;

            factory = swapchain.GetParent<Factory>();
            factory.MakeWindowAssociation(windowHandle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapchain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            deviceContext.Rasterizer.SetViewport(0, 0, width, height);

            logger.Info("Created D3D11 Context");
        }

        public void DisposeContext()
        {
            renderView.Dispose();
            backBuffer.Dispose();
            deviceContext.ClearState();
            deviceContext.Flush();
            device.Dispose();
            deviceContext.Dispose();
            swapchain.Dispose();
            factory.Dispose();

            logger.Info("Disposed D3D11 Context");
        }

        public void SwapBuffers()
        {
            swapchain.Present(0, PresentFlags.None);
        }
    }
}
