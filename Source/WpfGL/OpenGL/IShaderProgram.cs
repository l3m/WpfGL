using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace WpfGL.OpenGL
{
    public interface IShaderProgram
    {
        string VertexShaderSource { get; set; }
        string FragmentShaderSource { get; set; }

        void SetSources(IShaderSources src);

        void Setup();

        int Id { get; }
    }
}
