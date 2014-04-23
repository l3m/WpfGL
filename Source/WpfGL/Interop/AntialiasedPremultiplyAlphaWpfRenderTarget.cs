using System;
using OpenTK.Graphics.OpenGL4;
using WpfGL.OpenGL;
using WpfGL.Renderer;

namespace WpfGL.Interop
{
    public class AntialiasedPremultiplyAlphaWpfRenderTarget : IWpfRenderTarget
    {
        private readonly int _color_rb_id;
        private readonly int _color_rb_ms_id;
        private readonly int _fbo_id;
        private readonly int _fbo_ms_id;
        private readonly int _pbo_id;
        private readonly int _samples;
        private readonly IShaderProgram _sh;
        private readonly int _vbo_id;

        public AntialiasedPremultiplyAlphaWpfRenderTarget(IShaderProgram sh, IWpfPostProcessShaderSources src)
        {
            _sh = sh;

            _fbo_id = GL.GenFramebuffer();
            _fbo_ms_id = GL.GenFramebuffer();
            _color_rb_id = GL.GenRenderbuffer();
            _color_rb_ms_id = GL.GenRenderbuffer();
            _vbo_id = GL.GenBuffer();
            _pbo_id = GL.GenBuffer();

            _samples = DetermineSamples(8);

            _sh.SetSources(src);
            _sh.Setup();

            SetupVbo();
        }

        public int Width { get; set; }
        public int Height { get; set; }

        private void SetupVbo()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_id);

            float vmin = -1;
            float vmax = 1;
            float tmin = 0;
            float tmax = 1;

            float[] pos_texcoords =
            {
                vmin, vmin,
                tmin, tmin,
                vmin, vmax,
                tmin, tmax,
                vmax, vmin,
                tmax, tmin,
                vmax, vmax,
                tmax, tmax
            };

            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(sizeof (float) * pos_texcoords.Length),
                pos_texcoords,
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void RenderTextureToMultisamplingFramebuffer(int source_texture_id)
        {
            // render the supplied texture to a quad using the
            // premultiply alpha shader

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_ms_id);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.Viewport(0, 0, Width, Height);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo_id);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);
            GL.EnableVertexAttribArray(1);

            GL.BindTexture(TextureTarget.Texture2D, source_texture_id);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.UseProgram(_sh.Id);

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.UseProgram(0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Disable(EnableCap.Texture2D);
        }

        private void ResolveMultisampling()
        {
            // resolve multisampling 
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fbo_ms_id);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo_id);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0 + 0);

            GL.BlitFramebuffer(
                0, 0, Width, Height,
                0, 0, Width, Height,
                ClearBufferMask.ColorBufferBit,
                BlitFramebufferFilter.Nearest);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        }

        private void BeginReadback()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_id);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);

            GL.BindBuffer(BufferTarget.PixelPackBuffer, _pbo_id);
            GL.ReadPixels(0, 0, Width, Height, 
                PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed, 
                IntPtr.Zero);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render(int source_texture_id)
        {
            RenderTextureToMultisamplingFramebuffer(source_texture_id);
            ResolveMultisampling();
            BeginReadback();
        }

        public void Readback(WriteableBitmapSource target)
        {
            GL.BindBuffer(BufferTarget.PixelPackBuffer, _pbo_id);
            var ptr = GL.MapBuffer(BufferTarget.PixelPackBuffer, BufferAccess.ReadOnly);

            if (ptr == IntPtr.Zero)
                throw new WpfOpenGLInteropException();

            target.Copy(ptr, Width * Height * 4);

            GL.UnmapBuffer(BufferTarget.PixelPackBuffer);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);
        }

        public void Resize(int width, int height)
        {
            // resize multisampling framebuffer object 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_ms_id);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _color_rb_ms_id);

            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, _samples,
                RenderbufferStorage.Rgba8, width, height);
            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                RenderbufferTarget.Renderbuffer,
                _color_rb_ms_id);

            CheckFramebufferStatus();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // resize antialiased framebuffer object 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo_id);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _color_rb_id);

            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.Rgba8, width, height);
            GL.FramebufferRenderbuffer(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                RenderbufferTarget.Renderbuffer,
                _color_rb_id);

            CheckFramebufferStatus();

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            // resize pixel buffer object
            int buffer_size = width * height * 4;
            GL.BindBuffer(BufferTarget.PixelPackBuffer, _pbo_id);
            GL.BufferData(BufferTarget.PixelPackBuffer, new IntPtr(buffer_size), IntPtr.Zero, BufferUsageHint.StreamRead);
            GL.BindBuffer(BufferTarget.PixelPackBuffer, 0);

            Width = width;
            Height = height;
        }

        private int DetermineSamples(int max_samples)
        {
            int samples = GL.GetInteger(GetPName.MaxSamples);
            if (samples > max_samples)
            {
                samples = max_samples;
            }
            return samples;
        }

        public void CheckFramebufferStatus()
        {
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception();
            }
        }
    }
}
