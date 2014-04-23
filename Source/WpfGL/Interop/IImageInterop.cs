using System;
using System.Windows.Media;
using WpfGL.Renderer;

namespace WpfGL.Interop
{
    public interface IImageInterop : IDisposable
    {
        ImageSource ImageSource { get; }

        IRenderTimes RenderTimings { get; }

        void Render(IRenderToTexture rtt);
        void Resize(int width, int height);

        // collects rendering times
        void RenderAndSample(IRenderToTexture rtt);

        // this is required to avoid flicking in Wpf; even if we do not
        // change the render image, we need to mark the image source 
        // as dirty.
        void MarkImageSourceAsDirty();
    }
}
