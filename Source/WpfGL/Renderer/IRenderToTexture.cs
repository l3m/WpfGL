namespace WpfGL.Renderer
{
    public interface IRenderToTexture
    {
        int Render();
        void Resize(int width, int height);
    }
}
