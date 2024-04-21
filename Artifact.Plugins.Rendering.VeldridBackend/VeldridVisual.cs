using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
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

        private Texture texture;
        private TextureView textureView;

        public (Texture, TextureView) LoadTextureAndView(GraphicsDevice gd, string texturePath)
        {
            // Load the image using ImageSharp
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(texturePath))
            {
                // Create a Texture from the image
                var texture = gd.ResourceFactory.CreateTexture(new TextureDescription(
                    (uint)image.Width, (uint)image.Height, 1, 1, 1,
                    PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled, TextureType.Texture2D));

                // Upload the image data to the texture using TryGetPixelSpan
                List<Rgba32> pixels = new List<Rgba32>();

                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        pixels.Add(image[y, x]);
                    }
                }

                gd.UpdateTexture(texture, pixels.ToArray(), 0, 0, 0, (uint)image.Width, (uint)image.Height, 1, 0, 0);

                // Create a TextureView for the texture
                var textureView = gd.ResourceFactory.CreateTextureView(texture);

                return (texture, textureView);
            }
        }

        public unsafe VeldridVisual(Mesh mesh) : base()
        {
            _mesh = mesh;

            ResourceFactory factory = VeldridRenderingBackend.factory;
            GraphicsDevice device = VeldridRenderingBackend.device;


            vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)(mesh.Vertices.Length * Vertex.SizeInBytes), BufferUsage.VertexBuffer));
            indexBuffer = factory.CreateBuffer(new BufferDescription((uint)(mesh.Indices.Length * sizeof(ushort)), BufferUsage.IndexBuffer));


            using FileStream stream = File.OpenRead(mesh.TexturePath);
            ImageResult image = ImageResult.FromStream(stream);

            //Console.WriteLine(string.Join(", ", image.Data));

            (Texture tex, TextureView texView) = LoadTextureAndView(device, mesh.TexturePath);

            texture = tex;
            textureView = texView;

            BufferDescription mvpBufferDescription = new BufferDescription(
                sizeInBytes: 64,
                BufferUsage.UniformBuffer
                
            );

            mvpBuffer = factory.CreateBuffer(mvpBufferDescription);

            Matrix4x4 mvp = Matrix4x4.Identity;
            device.UpdateBuffer(mvpBuffer, 0, ref mvp);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
        new ResourceLayoutElementDescription("mvp", ResourceKind.UniformBuffer, ShaderStages.Vertex),
        new ResourceLayoutElementDescription("tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
        new ResourceLayoutElementDescription("texSampler", ResourceKind.Sampler, ShaderStages.Fragment));

            layout = factory.CreateResourceLayout(layoutDescription);


            device.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);
            device.UpdateBuffer(indexBuffer, 0, mesh.Indices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float4),
                new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));


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

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(layout, mvpBuffer, textureView, device.Aniso4xSampler);

            resourceSet = factory.CreateResourceSet(resourceSetDescription);
        }

        public override void Dispose()
        {
            if (!vertexBuffer.IsDisposed) { vertexBuffer.Dispose(); }
            if (!indexBuffer.IsDisposed) { indexBuffer.Dispose(); }
            if (!mvpBuffer.IsDisposed) { mvpBuffer.Dispose(); }

            if (!pipeline.IsDisposed) { pipeline.Dispose(); }

            foreach (Shader shader in shaders)
            {
                if (!shader.IsDisposed) { pipeline.Dispose(); }
            }
            
            if (!textureView.IsDisposed) { textureView.Dispose(); }
            if (!texture.IsDisposed) { texture.Dispose(); }
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
