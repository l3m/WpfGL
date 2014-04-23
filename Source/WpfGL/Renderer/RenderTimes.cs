using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WpfGL.Renderer
{
    public class RenderTimes : IRenderTimes
    {
        private readonly IDictionary<RenderStep, IList<TimeSpan>> _samples
            = new Dictionary<RenderStep, IList<TimeSpan>>();

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly StringBuilder _sb = new StringBuilder();

        public RenderTimes()
        {
            SetupSampleStorage( new []
            {
                RenderStep.Render,
                RenderStep.PostProcess,
                RenderStep.Readback,
            });
        }

        private void SetupSampleStorage(IEnumerable<RenderStep> rss)
        {
            foreach (var rs in rss)
            {
                _samples[rs] = new List<TimeSpan>();
            }
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }

        public void Stop(RenderStep rs)
        {
            _samples[rs].Add(_stopwatch.Elapsed);
        }

        public void Clear()
        {
            foreach (var samples in _samples.Values)
            {
                samples.Clear();
            }
        }

        public string Average()
        {
            try
            {
                _sb.Clear();

                _sb.Append(_samples[RenderStep.Render].Count +
                           "frames, ");

                foreach (var kv in _samples)
                {
                    if (kv.Value.Count == 0)
                    {
                        return "Rendering failed.";
                    }
                    _sb.Append(kv.Key);
                    _sb.Append(" ");
                    _sb.Append(MakeDisplayString(kv.Value.Select(x => x.TotalMilliseconds).Average()));
                    _sb.Append(", ");
                    kv.Value.Clear();
                }

                return _sb.ToString();
            }
            catch (Exception e)
            {
                return "Error during render timing.";
            }
        }

        private string MakeDisplayString(double d)
        {
            var s = d.ToString(CultureInfo.InvariantCulture);
            return s.Length < 5 ? s : s.Substring(0, 5);
        }

        public string Latest()
        {
            try
            {

                int latest = _samples[RenderStep.Render].Count - 1;

                _sb.Clear();

                foreach (var kv in _samples)
                {
                    if (kv.Value.Count == 0)
                    {
                        return "Rendering failed.";
                    }

                    _sb.Append(kv.Key);
                    _sb.Append(" ");
                    _sb.Append(MakeDisplayString(kv.Value[latest].TotalMilliseconds));
                    _sb.Append(", ");
                }

                return _sb.ToString();
            }
            catch (Exception e)
            {
                return "Error during render timing.";
            }
        }
    }
}