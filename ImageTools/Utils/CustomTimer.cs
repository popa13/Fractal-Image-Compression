using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageTools.Utils
{
    /// <summary>
    /// Utils class for calculating execution time of code.
    /// Implements IDisposable so it can be use by simply calling: using (new CustomTimer("Method")) 
    /// and wrapping timed code in it.
    /// </summary>
    public class CustomTimer : IDisposable
    {
        //Stopwatch to calculate time.
        private Stopwatch _stopwatch = new Stopwatch();
        //Label of the timer (for debugging purpose)
        private string _label;

        /// <summary>
        /// Constructor of a timer.
        /// </summary>
        /// <param name="label">Label shown in the Debug Console Output.</param>
        public CustomTimer(string label)
        {
            _label = label;
            _stopwatch.Start();
        }

        /// <summary>
        /// Dispose the timer (stops it and log time in Debug Console).
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
            int elapsedMinutes = Convert.ToInt32(_stopwatch.ElapsedMilliseconds / (1000d * 60d));
            MessageBox.Show(_label + ": "+_stopwatch.ElapsedMilliseconds + " ms (" + elapsedMinutes + " min " + ((_stopwatch.ElapsedMilliseconds - elapsedMinutes * 1000d * 60d) / 1000d).ToString("0.00") + " sec)", "Execution Time", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
