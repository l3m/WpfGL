using System;
using System.Windows;
using WpfGL.Interop;
using System.Windows.Threading;
using WpfGL.OpenGL;
using WpfGL.Renderer;

namespace WpfGLDemo
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();

            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();

            var ctx = new WpfOpenGLContext();
            var rtt = new ExampleWithMultisampling(new ShaderProgram(), 
                new ExampleShaderSources());
            GLImage.RenderToTexture = rtt;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Timing.Text = GLImage.RenderTimes.Average();
        }
    }
}
