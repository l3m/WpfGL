using System;

namespace WpfGL.Interop
{
    public class WpfOpenGLInteropException : Exception
    {
        public WpfOpenGLInteropException()
        {
        }

        public WpfOpenGLInteropException(string message)
            : base(message)
        {
        }

        public WpfOpenGLInteropException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
