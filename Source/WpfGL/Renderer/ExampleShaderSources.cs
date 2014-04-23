using WpfGL.OpenGL;

namespace WpfGL.Renderer
{
    public class ExampleShaderSources : IPositionToColorShaderSources
    {
        public string VertexShaderSource
        {
            get
            {
                return @"#version 420
                layout(location = 0) in vec2 position; 
                out vec2 color;

                void main()
                {
                    color = position;
                    gl_Position = vec4(position, 0, 1);
                }";
            }
        }

        public string FragmentShaderSource
        {
            get
            {
                return @"#version 420
                    in vec2 color;
                    out vec4 frag_color;

                    void main()
                    {
                        vec2 h = vec2(0.5);
                        vec2 c = h + color * h;
                        frag_color = vec4(c, 1-c.x, 1);
                    }";
            }
        }

    }
}
