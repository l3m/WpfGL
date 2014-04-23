using System;
using OpenTK.Graphics.OpenGL4;
using WpfGL.Interop;
using WpfGL.OpenGL;

namespace WpfGL.Renderer
{
    public class Example : IRenderToTexture
    {
        private readonly int _fbo_id;

        private readonly IShaderProgram _sh;
        private readonly int _texture_id;
        private readonly int _vbo_id;
        private int _height;
        private int _width;

        public Example(IShaderProgram sh,
            ExampleShaderSources src)
        {
            _vbo_id = GL.GenBuffer();

            _fbo_id = GL.GenFramebuffer();
            _texture_id = GL.GenTexture();

            _sh = sh;
            _sh.SetSources(src);
            _sh.Setup();

            SetupVbo();
        }

        public int Render()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_id);

            GL.Viewport(0, 0, _width, _height);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_id);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.UseProgram(_sh.Id);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.UseProgram(0);

            GL.DisableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return _texture_id;
        }

        public void Resize(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_id);
            GL.BindTexture(TextureTarget.Texture2D, _texture_id);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba, width, height,
                0, PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed,
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int) TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int) TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                _texture_id,
                0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _width = width;
            _height = height;
        }

        private void SetupVbo()
        {
            float x = 0.8f;
            var pxyz = new[]
            {
                -x, -x,
                x, -x, 
                0, x, 
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_id);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(pxyz.Length * sizeof (float)),
                pxyz,
                BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
