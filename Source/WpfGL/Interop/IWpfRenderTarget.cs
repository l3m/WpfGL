namespace WpfGL.Interop
{
    public interface IWpfRenderTarget
    {
        void Render(int source_texture_id);
        void Readback(WriteableBitmapSource target);
        void Resize(int width, int height);
    }
}
