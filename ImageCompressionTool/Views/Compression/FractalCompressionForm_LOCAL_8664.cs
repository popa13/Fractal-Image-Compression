using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageCompressionTool.Views.Controls;
using ImageCompressionTool.Controllers;
using ImageTools.Compression.Fractal;
using ImageTools.ImageWriters;
using ImageTools.ImageReaders;
using ImageTools;

namespace ImageCompressionTool.Views.Compression
{
    public partial class FractalCompressionForm : BaseCompressionForm
    {
        private FractalCompressionResult _fractalResult = null;
        private FractalImageWriter _writer = new FractalImageWriter();

        /// <summary>
        /// Obtient le contrôle de comparaison d'image dans la Form.
        /// </summary>
        protected override ImageComparisonControl ImageComparisonControl
        {
            get { return imgComparisonControl; }
        }

        public object MainController { get; private set; }

        public FractalCompressionForm()
        {
            InitializeComponent();
            imgComparisonControl.ImageAboutToBeSaved += ImgComparisonControl_ImageAboutToBeSaved;
            imgComparisonControl.ImageChosen += ImgComparisonControl_ImageChosen;
            imgComparisonControl.FileFilter = "Fractal Compressed Color Image Files (*.fcci)|*.fcci";
            imgComparisonControl.DefaultExt = "fcci";
            imgComparisonControl.DefaultSaveFileName = "transformed.fcci";
        }

        /// <summary>
        /// Méthode permettant de changer l'état actif/inactif des boutons.
        /// </summary>
        /// <param name="enabled">Si true, actif, sinon inactif</param>
        private void ChangeButtonStates(bool enabled)
        {
            btnCompress.Enabled = enabled;
        }

        /// <summary>
        /// Performs uncompression of fractal image result.
        /// </summary>
        private void PerformUncompress()
        {
            if (_fractalResult != null && imgComparisonControl.HasOriginalImage)
            {
                var compression = new FractalCompression(GetFractalCompressionOptions());
                compression.Uncompress(_fractalResult, new CustomImage(imgComparisonControl.OriginalImage));
                try
                {
                    var img = compression.Uncompress(_fractalResult, new CustomImage(imgComparisonControl.OriginalImage));
                    imgComparisonControl.TransformedImage = img;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Get the fractal compression options from the window.
        /// </summary>
        private FractalCompressionOptions GetFractalCompressionOptions()
        {
            return new FractalCompressionOptions()
            {
                RSquareSize = (byte)upDownRSize.Value,
                DSquareSize = (byte)(2 * (byte)upDownRSize.Value)
            };
        }

        /// <summary>
        /// Called on the click of the button to compress the image.
        /// Compress the image.
        /// </summary>
        private void btnCompress_Click(object sender, EventArgs e)
        {
            _fractalResult = MainFormController.Instance.CompressUsingFractalCompression(ImageComparisonControl.OriginalImage, GetFractalCompressionOptions());
        }

        /// <summary>
        /// Se produit lorsque l'image originale du contrôle de comparaison d'image change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Informations sur le changement d'image</param>
        private void imgComparisonControl_OriginalImageChanged(object sender, ImageComparisonControl.ImageChangedEventArgs e)
        {
            ChangeButtonStates(e.NewImage != null);
        }

        /// <summary>
        /// Called when the transformed image is about to be saved on disk.
        /// </summary>
        private void ImgComparisonControl_ImageAboutToBeSaved(object sender, ImageComparisonControl.ImageChosenEventArgs e)
        {
            _writer.Write(e.FileName, _fractalResult);
        }

        private void ImgComparisonControl_ImageChosen(object sender, ImageComparisonControl.ImageChosenEventArgs e)
        {
            var result = MainFormController.Instance.OpenFractalCompressedFileAndGetDecodingImage(e.FileName);

            if (result != null)
            {
                ImageComparisonControl.SetOriginalBitmap(result.Item2, e.FileName); //Set decoding image as bitmap
                _fractalResult = result.Item1;
                btnUncompress.Enabled = true;
            }
        }

        /// <summary>
        /// Called when the user clicks on uncompress button.
        /// </summary>
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            PerformUncompress();
        }
    }
}
