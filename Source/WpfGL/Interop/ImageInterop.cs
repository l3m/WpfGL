using System;
using System.Windows.Media;
using WpfGL.OpenGL;
using WpfGL.Renderer;

namespace WpfGL.Interop
{
    public class ImageInterop : IImageInterop
    {
        private readonly IWpfOpenGLContext _context;
        private readonly IAsyncReadbackWpfRenderTarget _render_target;
        private readonly IRenderTimes _render_timings;
        private readonly WriteableBitmapSource _source;
        private bool _disposed;

        public ImageInterop()
            : this(new WpfOpenGLContext(),
                new AsyncReadbackWpfRenderTarget(
                    new PremultiplyAlphaWpfRenderTarget(
                        new ShaderProgram(), new WpfPostProcessShaderSources()),
                    new PremultiplyAlphaWpfRenderTarget(
                        new ShaderProgram(), new WpfPostProcessShaderSources())),
                new RenderTimes()
                )
        {
        }

        public ImageInterop(IWpfOpenGLContext context,
            IAsyncReadbackWpfRenderTarget render_target,
            IRenderTimes render_timings)
        {
            _render_target = render_target;
            _render_timings = render_timings;
            _source = new WriteableBitmapSource();
            _context = context;
            _context.MakeCurrent();
        }

        public IRenderTimes RenderTimings
        {
            get { return _render_timings; }
        }
        public ImageSource ImageSource
        {
            get { return _source.Source; }
        }

        public void Render(IRenderToTexture rtt)
        {
            _context.MakeCurrent();

            int tex = rtt.Render();

            _render_target.Render(tex);
            _render_target.Readback(_source);
        }


        public void Resize(int width, int height)
        {
            _source.Resize(width, height);
            _render_target.Resize(width, height);
        }

        public void MarkImageSourceAsDirty()
        {
            _source.AddDirtyRect();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void RenderAndSample(IRenderToTexture rtt)
        {
            _context.MakeCurrent();

            _render_timings.Restart();

            int tex = rtt.Render();

            _render_timings.Stop(RenderStep.Render);

            _render_timings.Restart();
            _render_target.Render(tex);
            _render_timings.Stop(RenderStep.PostProcess);

            _render_timings.Restart();
            _render_target.Readback(_source);
            _render_timings.Stop(RenderStep.Readback);
        }

        ~ImageInterop()
        {
            Dispose(false);
        }

        public void Dispose(bool dispose_managed)
        {
            if (_disposed)
            {
                return;
            }

            if (dispose_managed)
            {
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
