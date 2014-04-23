namespace WpfGL.Renderer
{
    public enum RenderStep
    {
        Render,
        PostProcess,
        Readback,
    }

    public interface IRenderTimes
    {
        void Restart();
        void Stop(RenderStep rs);
        void Clear();

        string Average();
        string Latest();
    }
}
