using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Vector3 = System.Numerics.Vector3;
using Matrix = SharpDX.Matrix;
using SharpDX.WIC;
using Artifact.Plugins.Rendering.DirectXBackend.CommonDX;
using StbImageSharp;

namespace Artifact.Plugins.Rendering.DirectXBackend
{

    public class DirectXVisual : ArtifactDisposable, IVisual
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;

        private InputLayout layout;

        private Buffer vertexBuffer;
        private Buffer indexBuffer;
        private Buffer contantBuffer;
        private VertexBufferBinding vertexBinding;
        private ShaderResourceView texture;
        private SamplerState samplerState;

        private DeviceContext context;

        private Mesh _mesh;

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Vector3 Rotation { get; set; }

        public DirectXVisual(Mesh mesh) : base()
        {
            _mesh = mesh;
            context = DirectXRenderingBackend.deviceContext;


            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("Assets/Shaders/dx/default.fx", "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None);
            vertexShader = new VertexShader(DirectXRenderingBackend.device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Assets/Shaders/dx/default.fx", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None);
            pixelShader = new PixelShader(DirectXRenderingBackend.device, pixelShaderByteCode);

            layout = new InputLayout(
                DirectXRenderingBackend.device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
                    });

            vertexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.VertexBuffer, mesh.Vertices);
            indexBuffer = Buffer.Create(DirectXRenderingBackend.device, BindFlags.IndexBuffer, mesh.Indices);
            contantBuffer = new Buffer(DirectXRenderingBackend.device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            vertexBinding = new VertexBufferBinding(vertexBuffer, 24, 0);

            DTexture dtexture = new DTexture();
            dtexture.Initialize(DirectXRenderingBackend.device, mesh.TexturePath);

            texture = dtexture.TextureResource;

            samplerState = new SamplerState(DirectXRenderingBackend.device, new SamplerStateDescription
            {
                Filter = Application.current.GetPlugin<RenderingPlugin>().SamplerMode == SamplerMode.PixelArt ? Filter.ComparisonMinLinearMagMipPoint : Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            });
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Matrices
        {
            public Matrix model;
            public Matrix view;
            public Matrix proj;
        }

        public void Draw()
        {
            /*
            Matrix view = Camera.ViewMatrix.ToDXMatrix();
            Matrix proj = Camera.ProjectionMatrix.ToDXMatrix();

            Matrix translation = Matrix.Translation(Position.ToDXVec());
            Matrix rotation = Matrix.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            Matrix scale = Matrix.Scaling(Scale.ToDXVec());

            Matrix model = (rotation * translation * scale);

            view.Transpose();
            proj.Transpose();
            model.Transpose();
            */

            Matrix proj = Camera.ViewMatrix.ToDXMatrix();
            Matrix view = Camera.ProjectionMatrix.ToDXMatrix();

            Matrix translation = Matrix.Translation(Position.ToDXVec());
            translation.Transpose();
            Matrix rotation = Matrix.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            Matrix scale = Matrix.Scaling(Scale.ToDXVec());

            Matrix model = (scale * rotation * translation);

            Matrix mvp = (proj * view * model);
            mvp.Transpose();
            //mvp.Transpose();

            context.UpdateSubresource(ref mvp, contantBuffer);

            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, vertexBinding);
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
            context.VertexShader.SetConstantBuffer(0, contantBuffer);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.PixelShader.SetSampler(0, samplerState);
            context.PixelShader.SetShaderResource(0, texture);
            context.OutputMerger.SetTargets(DirectXRenderingBackend.renderView);

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
