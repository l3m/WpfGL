using System.Windows.Media;

namespace WpfGL.Interop
{
    public class AsyncReadbackWpfRenderTarget : IAsyncReadbackWpfRenderTarget
    {
        private readonly IWpfRenderTarget[] _render_targets;
        private int _next;
        private bool _first;

        public AsyncReadbackWpfRenderTarget(IWpfRenderTarget first, IWpfRenderTarget second)
        {
            _render_targets = new IWpfRenderTarget[2];
            _render_targets[0] = first;
            _render_targets[1] = second;
        }

        public void Readback(WriteableBitmapSource target)
        {
            _render_targets[_next].Readback(target);
        }

        public void Resize(int width, int height)
        {
            foreach (var rt in _render_targets)
            {
                rt.Resize(width, height);
            }

            _first = true;
        }

        public void Render(int source_texture_id)
        {
            _render_targets[_next].Render(source_texture_id);
            _next = _next == 0 ? 1 : 0;

            if (_first)
            {
                _first = false;
                Render(source_texture_id);
            }
        }
    }
}
