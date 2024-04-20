using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.MetalBindings;
using Veldrid.SPIRV;
using Vulkan;

namespace Artifact.Plugins.Rendering.VeldridBackend
{
    public class VeldridVisual : ArtifactDisposable, IVisual
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
        public Vector3 Rotation { get; set; }

        private Mesh _mesh;

        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private DeviceBuffer mvpBuffer;
        private Shader[] shaders;
        private Pipeline pipeline;
        private ResourceLayout layout;
        private ResourceSet resourceSet;

        public unsafe VeldridVisual(Mesh mesh) : base()
        {
            _mesh = mesh;

            ResourceFactory factory = VeldridRenderingBackend.factory;
            GraphicsDevice device = VeldridRenderingBackend.device;

            vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)(mesh.Vertices.Length * Vertex.SizeInBytes), BufferUsage.VertexBuffer));
            indexBuffer = factory.CreateBuffer(new BufferDescription((uint)(mesh.Indices.Length * sizeof(ushort)), BufferUsage.IndexBuffer));

            BufferDescription mvpBufferDescription = new BufferDescription(
                sizeInBytes: 64,
                BufferUsage.UniformBuffer
                
            );

            mvpBuffer = factory.CreateBuffer(mvpBufferDescription);

            Matrix4x4 mvp = Matrix4x4.Identity;
            device.UpdateBuffer(mvpBuffer, 0, ref mvp);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
    new ResourceLayoutElementDescription("mvp", ResourceKind.UniformBuffer, ShaderStages.Vertex));

            layout = factory.CreateResourceLayout(layoutDescription);


            device.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);
            device.UpdateBuffer(indexBuffer, 0, mesh.Indices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float4),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));


            ShaderDescription vertexShaderDesc = new ShaderDescription(
    ShaderStages.Vertex,
    Encoding.UTF8.GetBytes(File.ReadAllText("Assets/Shaders/veldrid/default.vert")),
    "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(File.ReadAllText("Assets/Shaders/veldrid/default.frag")),
                "main");

            shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
            pipelineDescription.ResourceLayouts = [layout];
            
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: shaders);

            pipelineDescription.Outputs = device.SwapchainFramebuffer.OutputDescription;

            pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(layout, mvpBuffer);

            resourceSet = factory.CreateResourceSet(resourceSetDescription);
        }

        public override void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            mvpBuffer.Dispose();

            pipeline.Dispose();

            foreach (Shader shader in shaders)
            {
                shader.Dispose();
            }
        }

        public void Draw()
        {
            Matrix4x4 model = Matrix4x4.Identity;

            Matrix4x4 translation = Matrix4x4.Transpose(Matrix4x4.CreateTranslation(Position));
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale);
            Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);

            model = translation;

            Matrix4x4 view = Camera.ViewMatrix;
            Matrix4x4 proj = Camera.ProjectionMatrix;

            Matrix4x4 mvp = Matrix4x4.Transpose(proj * view * model);
            
            VeldridRenderingBackend.device.UpdateBuffer(mvpBuffer, 0, ref mvp);


            CommandList commandList = VeldridRenderingBackend.commandList;
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetPipeline(pipeline);
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed(
                indexCount: (uint)_mesh.Indices.Length,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }
    }
}
