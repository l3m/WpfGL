using OpenTK;

namespace WpfGL.Interop
{
    public class WpfOpenGLContext :  IWpfOpenGLContext
    {
        private static GLControl _control;
        private object _lock = new object();

        public WpfOpenGLContext()
        {
            lock (_lock)
            {
                if (_control == null)
                    _control = new GLControl();
            }

            _control.MakeCurrent();
        }

        public void MakeCurrent()
        {
            _control.MakeCurrent();
        }
    }
}
