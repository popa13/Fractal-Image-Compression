using ImageTools;
using ImageTools.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressionTool.Views.Compression
{
    /// <summary>
    /// Form used to display information on comparison between 2 images.
    /// </summary>
    public partial class ImageDifferenceForm : Form
    {
        /// <summary>
        /// Constructor of the Form.
        /// </summary>
        public ImageDifferenceForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show the form using the infos and images specified.
        /// </summary>
        /// <param name="infos">Informations to display</param>
        /// <param name="imgA">Image A</param>
        /// <param name="imgB">Image B</param>
        public void ShowDialog(string infos, CustomImage imgA, CustomImage imgB)
        {
            pcbDifference.Image = ImageUtils.CalculateDifferencesImage(imgA, imgB).GetBitmap();
            lblInfos.Text = infos;
            this.ShowDialog();
        }

        /// <summary>
        /// Close the Form on click of the button.
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save the Differences Image.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "difference.png";
            sfd.DefaultExt = ".png";
            sfd.Filter = "Image Files (.png)|.png";
            sfd.Title = "Choose where to save your image";
            sfd.InitialDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Content\\Images");

            if (sfd.ShowDialog() == DialogResult.OK)
                pcbDifference.Image.Save(sfd.FileName);
        }
    }
}
