using WpfGL.Renderer;

namespace WpfGL.Interop
{
    public interface IWpfGLImage
    {
        IRenderToTexture RenderToTexture { get; set; }
        IRenderTimes RenderTimes { get; }
    }
}
