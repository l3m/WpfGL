using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace WpfGL.OpenGL
{
    public class ShaderProgram : IShaderProgram
    {
        private int _id = -1;

        public int Id
        {
            get { return _id; }
        }

        public string VertexShaderSource { get; set; }
        public string FragmentShaderSource { get; set; }

        public void SetSources(IShaderSources src)
        {
            VertexShaderSource = src.VertexShaderSource;
            FragmentShaderSource = src.FragmentShaderSource;
        }

        public void Setup()
        {
            Clear();
            _id = GL.CreateProgram();

            Debug.WriteLine("Setting up shader program " + _id + ".");

            int vs_sh = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs_sh, VertexShaderSource);
            GL.CompileShader(vs_sh);

            Debug.WriteLine("Compiling vertex shader.");
            CheckCompileStatus(vs_sh);

            int fs_sh = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs_sh, FragmentShaderSource);
            GL.CompileShader(fs_sh);

            Debug.WriteLine("Compiling fragment shader.");
            CheckCompileStatus(fs_sh);

            GL.AttachShader(_id, vs_sh);
            GL.AttachShader(_id, fs_sh);
            GL.LinkProgram(_id);

            Debug.WriteLine("Linking shader program.");
            CheckLinkStatus(_id);
        }

        private void CheckCompileStatus(int shader_id)
        {
            int compile_status;
            GL.GetShader(shader_id, ShaderParameter.CompileStatus, out compile_status);
            if (compile_status != 1)
            {
                string error_msg = GL.GetShaderInfoLog(shader_id);
                Debug.WriteLine(error_msg);
                throw new OpenGLException(error_msg);
            }
        }

        private void CheckLinkStatus(int program_id)
        {
            int compile_status;
            GL.GetProgram(program_id, GetProgramParameterName.LinkStatus, out compile_status);
            if (compile_status != 1)
            {
                string error_msg = GL.GetProgramInfoLog(program_id);
                Debug.WriteLine(error_msg);
                throw new OpenGLException(error_msg);
            }
        }

        public void Clear()
        {
            if (_id != -1)
            {
                GL.DeleteProgram(_id);
            }
        }
    }
}
