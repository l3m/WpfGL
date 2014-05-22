using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfGL.OpenGL;
using WpfGL.Renderer;

namespace WpfGL.Interop
{
    /// <summary>
    ///     A Wpf control that can be used to render OpenGL content to.
    /// </summary>
    public class WpfGLImage : Image, IWpfGLImage
    {
        private static bool? _is_in_design_mode;
        private IImageInterop _image;

        private TimeSpan _last_rendering_time;

        private bool _rendering_enabled;
        private bool _reset_back_buffer;
        private IRenderTimes _render_timings;

        public WpfGLImage()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            _rendering_enabled = false;
        }

        public static bool IsInDesignMode
        {
            get
            {
                if (!_is_in_design_mode.HasValue)
                {
                    _is_in_design_mode =
                        (bool)
                            DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                                typeof (FrameworkElement)).Metadata.DefaultValue;
                }

                return _is_in_design_mode.Value;
            }
        }

        private void CreateRenderTarget()
        {
            int w = ActualWidth > 0 ? (int) ActualWidth : 32;
            int h = ActualHeight > 0 ? (int) ActualHeight : 32;

            _image.Resize(w, h);
            _reset_back_buffer = true;

            Source = _image.ImageSource;
        }

        private void DestroyRenderTarget()
        {
            Source = null;

            _image.Dispose();
            _image = null;
        }


        private void OnLoaded(object sender, RoutedEventArgs event_args)
        {
            if (IsInDesignMode)
            {
                return;
            }

            if (_image == null)
                _image = new ImageInterop();

            CreateRenderTarget();
            StartRendering();
        }

        private void OnUnloaded(object sender, RoutedEventArgs event_args)
        {
            if (IsInDesignMode)
            {
                return;
            }

            StopRendering();
            DestroyRenderTarget();
        }


        private void StartRendering()
        {
            if (_rendering_enabled)
            {
                return;
            }

            CompositionTarget.Rendering += OnRendering;
            _rendering_enabled = true;
        }


        private void StopRendering()
        {
            if (!_rendering_enabled)
            {
                return;
            }

            CompositionTarget.Rendering -= OnRendering;
            _rendering_enabled = false;
        }


        private void OnRendering(object sender, EventArgs event_args)
        {
            if (!_rendering_enabled)
            {
                return;
            }

            if (_reset_back_buffer)
            {
                var w = (int) ActualWidth;
                var h = (int) ActualHeight;

                _image.Resize(w, h);

                Source = _image.ImageSource;

                _reset_back_buffer = false;
            }

            _image.RenderAndSample(RenderToTexture);
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo size_info)
        {
            _reset_back_buffer = true;

            var ns = size_info.NewSize;
            var width = (int) ns.Width;
            var height = (int) ns.Height;

            RenderToTexture.Resize(width, height);

            _image.Resize(width, height);
            Source = _image.ImageSource;

            base.OnRenderSizeChanged(size_info);
        }

        public IRenderToTexture RenderToTexture { get; set; }

        public IRenderTimes RenderTimes
        {
            get { return _image.RenderTimings; }
        }
    }
}
