using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfGL.Interop
{
    public class WriteableBitmapSource
    {
        private WriteableBitmap _bitmap;

        private int _height;
        private int _line_bytes;
        private object _lock = new object();
        private int _pixels_bytes;
        private int _width;

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public ImageSource Source
        {
            get { return _bitmap; }
        }

        public void Copy(IntPtr src, int bytes)
        {
            _bitmap.Lock();
            {
                if (bytes > _pixels_bytes)
                {
                    throw new WpfOpenGLInteropException();
                }

                if (_line_bytes == _bitmap.BackBufferStride)
                {
                    CopyMemory(_bitmap.BackBuffer, src, (uint) bytes);
                }
                else
                {
                    var dst = _bitmap.BackBuffer;
                    for (int i = 0, index = 0; i < _height; ++i, index += _line_bytes)
                    {
                        CopyMemory(dst, src, (uint) bytes);

                        src += _line_bytes;
                        dst += _bitmap.BackBufferStride;
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }

        public void AddDirtyRect()
        {
            _bitmap.Lock();
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;

            _line_bytes = 4 * _width;
            _pixels_bytes = _line_bytes * _height;

            _bitmap = new WriteableBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32,
                null
                );
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        public void Copy(byte[] pixels)
        {
            _bitmap.Lock();
            {
                if (pixels.Length > _pixels_bytes)
                {
                    throw new Exception();
                }

                if (_line_bytes == _bitmap.BackBufferStride)
                {
                    Marshal.Copy(pixels, 0, _bitmap.BackBuffer, pixels.Length);
                }
                else
                {
                    var dst = _bitmap.BackBuffer;
                    for (int i = 0, index = 0; i < _height; ++i, index += _line_bytes)
                    {
                        Marshal.Copy(pixels, index, dst, _line_bytes);
                        dst += _bitmap.BackBufferStride;
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
            _bitmap.Unlock();
        }
    }
}
