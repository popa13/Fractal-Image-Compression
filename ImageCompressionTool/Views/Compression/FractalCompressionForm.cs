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
using System.IO;
using ImageTools.Utils;

namespace ImageCompressionTool.Views.Compression
{
    /// <summary>
    /// Form for Encoding/Decoding images using Fractal Compression.
    /// </summary>
    public partial class FractalCompressionForm : BaseCompressionForm
    {
        private FractalCompressionResult _fractalResult = null; 
        private FractalImageWriter _writer = new FractalImageWriter();
        private DefaultImageWriter _defaultWriter = new DefaultImageWriter();
        private long _originalImageSize;

        /// <summary>
        /// Obtient le contrôle de comparaison d'image dans la Form.
        /// </summary>
        protected override ImageComparisonControl ImageComparisonControl
        {
            get { return imgComparisonControl; }
        }

        /// <summary>
        /// Constructor of the Form.
        /// </summary>
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
        private void PerformUncompress(bool singleIteration)
        {
            if (_fractalResult != null && imgComparisonControl.HasOriginalImage)
            {
                CustomImage img;
                var compression = new FractalCompression(GetFractalCompressionOptions());

                if (singleIteration)
                    img = compression.Uncompress(_fractalResult, new CustomImage(imgComparisonControl.TransformedImage ?? imgComparisonControl.OriginalImage));
                else
                    img = compression.CompleteUncompress(_fractalResult, new CustomImage(imgComparisonControl.OriginalImage));
                imgComparisonControl.TransformedImage = img;
                btnSaveUncompressedImage.Enabled = true;
            }
        }

        /// <summary>
        /// Get the fractal compression options from the window.
        /// </summary>
        private FractalCompressionOptions GetFractalCompressionOptions()
        {
            return new FractalCompressionOptions()
            {
                RSquareSize = Convert.ToByte(upDownRSize.Value),
                DSquareRatio = 2,
                SmoothImage = chkSmoothImage.Checked
            };
        }

        /// <summary>
        /// Called on the click of the button to compress the image.
        /// Compress the image.
        /// </summary>
        private void btnCompress_Click(object sender, EventArgs e)
        {
            _fractalResult = Controllers.MainController.Instance.CompressUsingFractalCompression(ImageComparisonControl.OriginalImage, GetFractalCompressionOptions());
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
            var result = Controllers.MainController.Instance.OpenFractalCompressedFileAndGetDecodingImage(e.FileName);

            if (result != null)
            {
                FileInfo info = new FileInfo(result.Item3);
                _originalImageSize = info.Length;
                ImageComparisonControl.SetOriginalBitmap(result.Item2, e.FileName); //Set decoding image as bitmap
                _fractalResult = result.Item1;
                btnUncompress.Enabled = true;
                btnCompleteUncompress.Enabled = true;
            }
        }

        /// <summary>
        /// Called when the user clicks on uncompress button.
        /// </summary>
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            PerformUncompress(true);
        }

        /// <summary>
        /// Save uncompressed image.
        /// </summary>
        private void btnSaveUncompressedImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "uncompressed.png";
            sfd.DefaultExt = ".png";
            sfd.Filter = "Images (*.png)|*.png";
            sfd.Title = "Choose where to save your image";
            sfd.InitialDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Content\\Images");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _defaultWriter.Write(sfd.FileName, imgComparisonControl.TransformedImage.GetBitmap());
            }
        }

        /// <summary>
        /// Uncompress image using complete decompression.
        /// </summary>
        private void btnCompleteUncompress_Click(object sender, EventArgs e)
        {
            PerformUncompress(false);
        }

        /// <summary>
        /// Compare original and transformed images.
        /// </summary>
        private void btnCalculatePSNR_Click(object sender, EventArgs e)
        {
            if (imgComparisonControl.HasOriginalImage && imgComparisonControl.HasTransformedImage)
            {
                double psnrY = ImageUtils.CalculatePSNRForY(imgComparisonControl.OriginalImage, imgComparisonControl.TransformedImage);
                double psnrI = ImageUtils.CalculatePSNRForI(imgComparisonControl.OriginalImage, imgComparisonControl.TransformedImage);
                double psnrQ = ImageUtils.CalculatePSNRForQ(imgComparisonControl.OriginalImage, imgComparisonControl.TransformedImage);
                long fileSize = _writer.GetFileSize(_fractalResult);
                long originalSize = _defaultWriter.GetFileSize(imgComparisonControl.OriginalImage.GetBitmap());

                ImageDifferenceForm form = new ImageDifferenceForm();
                form.ShowDialog("PSNR (Y): " + psnrY + Environment.NewLine +
                    "PSNR (I): " + psnrI + Environment.NewLine +
                    "PSNR (Q): " + psnrQ + Environment.NewLine +
                    "PSNR (YIQ) : " + ((psnrY + psnrI + psnrQ) / 3d) + Environment.NewLine +
                    "Fractal image size: " + fileSize + " bytes (" + (fileSize / 1024d).ToString("0.00") + " kb)" + Environment.NewLine +
                    "Original image size: " + _originalImageSize + " bytes (" + (_originalImageSize / 1024d).ToString("0.00") + " kb)" + Environment.NewLine +
                    "Compression Ratio: " + ((double)_originalImageSize / fileSize).ToString("0.00") + ":1",
                    imgComparisonControl.OriginalImage,
                    imgComparisonControl.TransformedImage);
            }
            else
            {
                MessageBox.Show("Please make sure you have an original and a transformed image before calculating PSNR!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
