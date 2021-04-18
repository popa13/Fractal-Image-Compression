using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ImageTools.ImageReaders;
using ImageTools;

namespace ImageCompressionTool.Views.Controls
{
    /// <summary>
    /// Control used to compare images, one original and one transformed.
    /// </summary>
    public partial class ImageComparisonControl : UserControl
    {
        #region Events
        public delegate void OriginalImageChangedEventHandler(object sender, ImageChangedEventArgs e);
        /// <summary>
        /// Event occuring when the original image changes.
        /// </summary>
        public event OriginalImageChangedEventHandler OriginalImageChanged;

        public delegate void ImageChosenEventHandler(object sender, ImageChosenEventArgs e);
        /// <summary>
        /// Event occuring when an image is chosen.
        /// </summary>
        public event ImageChosenEventHandler ImageChosen;

        public delegate void ImageAboutToBeSavedEventHandler(object sender, ImageChosenEventArgs e);
        /// <summary>
        /// Event occuring when the transformed image is about to be saved.
        /// </summary>
        public event ImageAboutToBeSavedEventHandler ImageAboutToBeSaved;
        #endregion

        #region Properties
        /// <summary>
        /// Default save extension
        /// </summary>
        public string DefaultExt { get; set; } = "jpg";
        /// <summary>
        /// Default filter for saving file.
        /// </summary>
        public string FileFilter { get; set; } = "Images (*.png)|*.png|Images (*.jpg)|*.jpg";
        /// <summary>
        /// Default FileName for saving file.
        /// </summary>
        public string DefaultSaveFileName { get; set; } = "transformed.jpg";
        #endregion

        private CustomImage _originalImage;
        private CustomImage _transformedImage;

        /// <summary>
        /// Get or set the original image.
        /// </summary>
        public CustomImage OriginalImage
        {
            get
            {
                if (_originalImage == null)
                    return null;
                return new CustomImage(_originalImage);
            }
            set
            {
                var oldImage = _originalImage;
                _originalImage = value;

                if (value != null)
                {
                    lblOriginalDimensions.Text = $"{value.Width} x {value.Height}";
                }

                pcbOriginal.Image = value?.GetBitmap() ?? null;
                OnOriginalImageChanged(new ImageChangedEventArgs(oldImage, _originalImage));
            }
        }
        /// <summary>
        /// Get or set the Transformed Image
        /// </summary>
        public CustomImage TransformedImage
        {
            get { return _transformedImage; }
            set
            {
                _transformedImage = value;
                pcbTransformed.Image = value?.GetBitmap() ?? null;

                if (value != null)
                {
                    lblTransformedDimensions.Text = $"{value.Width} x {value.Height}";
                }
            }
        }

        /// <summary>
        /// Returns true if the original image is set.
        /// </summary>
        public bool HasOriginalImage => _originalImage != null;
        public bool HasTransformedImage => _transformedImage != null;

        /// <summary>
        /// Constructor of the control.
        /// </summary>
        public ImageComparisonControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs on a click on the button to choose an image.
        /// Load an image from disk and display it.
        /// </summary>
        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.DefaultExt = DefaultExt;
            ofd.Title = "Choose an image to open";
            ofd.Filter = FileFilter;
            ofd.InitialDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Content\\Images");

            //If image was chosen
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OnImageChosen(ofd.FileName);
            }
        }

        /// <summary>
        /// Occurs on a click on the save image button.
        /// Saves the transformed image on disk.
        /// </summary>
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = DefaultSaveFileName;
            sfd.DefaultExt = DefaultExt;
            sfd.Filter = FileFilter;
            sfd.Title = "Choose where to save your image";
            sfd.InitialDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Content\\Images");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                OnImageAboutToBeSaved(sfd.FileName);
            }
        }

        /// <summary>
        /// Set the Original Bitmap.
        /// </summary>
        /// <param name="bmp">Bitmap to show</param>
        public void SetOriginalBitmap(Bitmap bmp, string fileName)
        {
            var oldImage = _originalImage;
            var fileInfo = new FileInfo(fileName);
            OriginalImage = new CustomImage(bmp);

            lblOriginalSizeOnDisk.Text = (fileInfo.Length / (1024d * 1024d)).ToString("0.00") + " mb";
        }

        /// <summary>
        /// Called when the original image changed, raising the corresponding event.
        /// </summary>
        /// <param name="args">Args of the event</param>
        private void OnOriginalImageChanged(ImageChangedEventArgs args)
        {
            if (OriginalImageChanged != null)
                OriginalImageChanged(this, args);
        }

        /// <summary>
        /// Called when the original image is about to change
        /// </summary>
        /// <param name="fileName">Name of the image chosen</param>
        private void OnImageChosen(string fileName)
        {
            if (ImageChosen != null)
                ImageChosen(this, new ImageChosenEventArgs(fileName));
        }

        /// <summary>
        /// Called when the transformed image is about to be saved.
        /// </summary>
        /// <param name="fileName">Filename of the image</param>
        private void OnImageAboutToBeSaved(string fileName)
        {
            if (ImageAboutToBeSaved != null)
                ImageAboutToBeSaved(this, new ImageChosenEventArgs(fileName));
        }

        /// <summary>
        /// Class acting as event arguments when an image changes.
        /// </summary>
        public class ImageChangedEventArgs : EventArgs
        {
            public CustomImage OldImage { get; private set; }
            public CustomImage NewImage { get; private set; }

            public ImageChangedEventArgs(CustomImage oldBmp, CustomImage newBmp)
            {
                this.OldImage = oldBmp;
                this.NewImage = newBmp;
            }
        }

        /// <summary>
        /// Event Arguments for when an image is chosen.
        /// </summary>
        public class ImageChosenEventArgs : EventArgs
        {
            public string FileName { get; private set; }

            public ImageChosenEventArgs(string fileName)
            {
                this.FileName = fileName;
            }
        }
    }
}
