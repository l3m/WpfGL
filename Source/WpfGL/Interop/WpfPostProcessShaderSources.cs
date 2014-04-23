using WpfGL.OpenGL;

namespace WpfGL.Interop
{
    public class WpfPostProcessShaderSources : IWpfPostProcessShaderSources
    {
        public string VertexShaderSource
        {
            get
            {
                return @"#version 420
                layout(location = 0) in vec2 position; 
                layout(location = 1) in vec2 in_tex_coord; 
                varying out vec2 tex_coord;

                void main()
                {
                    tex_coord = in_tex_coord;
                    gl_Position = vec4(position, 0, 1);
                }";
            }
        }

        public string FragmentShaderSource
        {
            get
            {
                return @"#version 420
                    in vec2 tex_coord;
                    layout(binding = 0) uniform sampler2D my_texture;
                    varying out vec4 frag_color;

                    void main()
                    {
                        vec2 wpf_tc = vec2(tex_coord.x, 1-tex_coord.y);
                        vec4 texel = texture2D(my_texture, wpf_tc);
                        float a = texel.a;
                        frag_color = vec4(texel.b * a, texel.y * a, texel.r * a, a); 
                    }";
            }
        }

    }
}
