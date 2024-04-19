using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Artifact.Plugins.Rendering.DirectXBackend
{
    public class DirectXVisual : Visual
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private InputLayout layout;

        private Buffer vertexBuffer;
        private Buffer indexBuffer;
        private VertexBufferBinding vertexBinding;

        private DeviceContext context;

        private Mesh _mesh;

        public DirectXVisual(Mesh mesh) : base(mesh)
        {
            _mesh = mesh;
            context = DirectXRenderingBackend.deviceContext;

            
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("Assets/default.fx", "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            vertexShader = new VertexShader(DirectXRenderingBackend.device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Assets/default.fx", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            pixelShader = new PixelShader(DirectXRenderingBackend.device, pixelShaderByteCode);
            /*
            layout = new InputLayout(
                DirectXRenderingBackend.device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });


            vertexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.VertexBuffer, mesh.Vertices);

            /*
            var indexBufferDescription = new BufferDescription(
                mesh.Indices.Length * sizeof(short), // Size in bytes
                ResourceUsage.Dynamic, // Usage
                BindFlags.IndexBuffer, // Bind flags
                CpuAccessFlags.Write, // CPU access flags
                ResourceOptionFlags.None, // Option flags
                0 // Structure byte stride
            );

            using (var dataStream = new DataStream(mesh.Indices.Length * sizeof(short), true, true))
            {
                dataStream.WriteRange(mesh.Indices);
                dataStream.Position = 0;

                indexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.IndexBuffer, ref indexBufferDescription);
            }
            */
            /*
            vertexBinding = new VertexBufferBinding(vertexBuffer, 32, 0);
        
            */

            layout = new InputLayout(
                DirectXRenderingBackend.device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            vertexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.VertexBuffer, mesh.Vertices);
            indexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.IndexBuffer, mesh.Indices);

            vertexBinding = new VertexBufferBinding(vertexBuffer, 32, 0);

        }

        public override void Draw()
        {
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, vertexBinding);
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.OutputMerger.SetTargets(DirectXRenderingBackend.renderView);

            Console.WriteLine(_mesh.Indices.Length);

            context.DrawIndexed(_mesh.Indices.Length, 0, 0);
        }

        public override void Dispose()
        {
            vertexShader.Dispose();
            pixelShader.Dispose();

            vertexBuffer.Dispose();
            //indexBuffer.Dispose();

            layout.Dispose();
        }
    }
}
