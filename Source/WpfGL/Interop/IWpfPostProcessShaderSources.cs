using WpfGL.OpenGL;

namespace WpfGL.Interop
{
    /// <summary>
    /// Wpf prefers textures with premultiplied alpha, and with row0 
    /// begin the top row. The Wpf post processing pass generates a
    /// suitable texture from the render result texture.
    /// </summary>
    public interface IWpfPostProcessShaderSources : IShaderSources
    {
    }
}
