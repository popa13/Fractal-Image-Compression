using ImageCompressionTool.Views.Controls;
using ImageTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressionTool.Views.Compression
{
    public class BaseCompressionForm : Form
    {
        protected virtual ImageComparisonControl ImageComparisonControl
        {
            get { throw new NotImplementedException("Subclass must implement ImageComparisonControl property!"); }
        }

        /// <summary>
        /// Affiche la fenêtre de compression pour compresser une image en particulier.
        /// </summary>
        /// <param name="img">Image à compresser</param>
        public void ShowDialog(CustomImage img)
        {
            ImageComparisonControl.OriginalImage = img;
            this.ShowDialog();
        }
    }
}
