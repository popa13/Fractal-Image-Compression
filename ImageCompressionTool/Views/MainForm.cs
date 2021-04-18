using ImageCompressionTool.Controllers;
using ImageCompressionTool.Views.Compression;
using ImageTools;
using ImageTools.ImageReaders;
using ImageTools.ImageWriters;
using ImageTools.Transformations;
using ImageTools.Utils;
using MathTools.Matrixes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressionTool.Views
{
    /// <summary>
    /// Principal Form.
    /// </summary>
    public partial class MainForm : Form
    {
        private DefaultImageReader _imageReader = new DefaultImageReader(); //Image Reader (JPG, PNG)

        /// <summary>
        /// Constructeur de la Form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            imgComparison.ImageChosen += ImgComparison_ImageChosen;
            imgComparison.ImageAboutToBeSaved += ImgComparison_ImageAboutToBeSaved;
        }

        /// <summary>
        /// Executed when Transformed image is about to be saved.
        /// Save Image.
        /// </summary>
        private void ImgComparison_ImageAboutToBeSaved(object sender, Controls.ImageComparisonControl.ImageChosenEventArgs e)
        {
            DefaultImageWriter writer = new DefaultImageWriter();
            writer.Write(e.FileName, imgComparison.TransformedImage.GetBitmap());
        }

        /// <summary>
        /// Se produit quand une image est choisie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgComparison_ImageChosen(object sender, Controls.ImageComparisonControl.ImageChosenEventArgs e)
        {
            imgComparison.SetOriginalBitmap(_imageReader.Read(e.FileName), e.FileName);
        }

        /// <summary>
        /// Méthode permettant de changer l'état actif/inactif des boutons.
        /// </summary>
        /// <param name="enabled">Si true, actif, sinon inactif</param>
        private void ChangeButtonStates(bool enabled)
        {
            btnGreyscale.Enabled = enabled;
            btnFractalTriangleTransformation.Enabled = enabled;
            btnFractalFernTransformation.Enabled = enabled;
            btnKochSnowflakeTransformation.Enabled = enabled;
            btnReduceTransformation.Enabled = enabled;
            chkUseTransformedImage.Enabled = enabled;
            btnAffineTransformation.Enabled = enabled;
            btnReduceHalf.Enabled = enabled;
        }

        /// <summary>
        /// Get the image to transform (Original or Transformed).
        /// </summary>
        /// <returns>Image to transform.</returns>
        private CustomImage GetImageToTransform()
        {
            if (chkUseTransformedImage.Checked && chkUseTransformedImage.Enabled && imgComparison.TransformedImage != null)
                return imgComparison.TransformedImage;
            else
                return imgComparison.OriginalImage;
        }

        /// <summary>
        /// Appelé lors d'Un clic sur le bouton pour la compression d'image par les fractals.
        /// Affiche la fenêtre de compression d'image par les fractals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFractalCompression_Click(object sender, EventArgs e)
        {
            FractalCompressionForm form = new FractalCompressionForm();
            form.ShowDialog(imgComparison.OriginalImage);
        }

        /// <summary>
        /// Transform image using greyscale.
        /// </summary>
        private void btnGreyscale_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new GreyscaleTransformation());
            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Se produit lorsque l'image originale dans le contrôle de comparaison d'image est changée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgComparison_OriginalImageChanged(object sender, Controls.ImageComparisonControl.ImageChangedEventArgs e)
        {
            ChangeButtonStates(e.NewImage != null);
        }

        /// <summary>
        /// Sierpinsky Triangle transformation of image.
        /// </summary>
        private void btnFractalTriangleTransformation_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new AffineTransformation(
                new DoubleMatrix[] { new double[,] { { 0.5, 0 }, { 0, 0.5 } }, new double[,] { { 0.5, 0 }, { 0, 0.5 } }, new double[,] { { 0.5, 0 }, { 0, 0.5 } } },
                new DoubleMatrix[] { new double[,] { { img.Width / 4 }, { 0 } }, new double[,] { { 0 }, { img.Height / 2 } }, new double[,] { { img.Width / 2 }, { img.Height / 2 } } }));
            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Attempt to transform image to make a Fractal Fern.
        /// </summary>
        private void btnFractalFernTransformation_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            var transformation = new AffineTransformation(
                new DoubleMatrix[] {
                    new double[,] { { 0, 0 }, { 0, 0.16 } },
                    new double[,] { { 0.3, 0.4}, { 0.4, 0.3 } },
                    new double[,] { { 0.4, 0.2 }, { 0.2, 0.4 } },
                    new double[,] { { 0.4, 0.2 }, { 0.2, 0.4 } }
                },
                new DoubleMatrix[] {
                    new double[,] { { img.Width/2 }, { img.Height * (1 - 0.16) } },
                    new double[,] { { 0 }, { 1.6 } },
                    new double[,] { { 0 }, { 1.6 } },
                    new double[,] { { 0 }, { 0.44 } }
                },
                new double[] { 0.01, 0.85, 0.07, 0.07 });
            img.Transform(transformation);

            //for (int i = 0; i < 1000; i++)
               // img.Transform(transformation, true);

            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Test button.
        /// </summary>
        private void btnReduceTransformation_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new AffineTransformation(
                new DoubleMatrix[] {
                    new double[,] { { 1d / img.Height, 0 }, { 0, 1d / img.Height } },
                },
                new DoubleMatrix[] {
                    new double[,] { { 200 }, { 200 } },
                }));
            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Attempt to make Koch Snowflake with image.
        /// </summary>
        private void btnKochSnowflakeTransformation_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            var transformation = new AffineTransformation(
                new DoubleMatrix[] {
                    new double[,] { { 1d / 3d, 0 }, { 0, 1d / 3d } },
                    new double[,] { { 1/3d * Math.Cos(3.1416/3d), 1/3d * Math.Sin(3.1416/3d) }, { -1/3d * Math.Sin(3.1416/3d), 1/3d * Math.Cos(3.1416/3d) } },
                    new double[,] { { 1/3d * Math.Cos(3.1416/3d), -1/3d * Math.Sin(3.1416/3d) }, { 1/3d * Math.Sin(3.1416/3d), 1/3d * Math.Cos(3.1416/3d) } },
                    new double[,] { { 1d / 3d, 0 }, { 0, 1d / 3d } }
                },
                new DoubleMatrix[] {
                    new double[,] { { 0 }, { 2 * img.Height/3 } },
                    new double[,] { { img.Width * (1 - Math.Sin(3.1416/3d) )/3d}, { img.Height * (1-(Math.Cos(3.1416/3d))/3d) } },
                    new double[,] { { 2 * img.Width/3 - (img.Width / 3d * Math.Cos(3.1416 / 3d) - img.Height / 3d * Math.Sin(3.1416 / 3d)) },
                                        { img.Height - (img.Width / 3d * Math.Sin(3.1416/3d) + img.Height / 3d * Math.Cos(3.1416/3d))} },
                    new double[,] { { 2 * img.Width / 3 }, { 2 * img.Height/3}  }
                });

            img.Transform(transformation);

            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Test button to do an Affine transformation on an image.
        /// </summary>
        private void btnAffineTransformation_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new AffineTransformation(
                new DoubleMatrix[] {
                    new double[,] { { -1, 0 }, { 0, 1 } }, //Reflection on Y axis
                },
                new DoubleMatrix[] {
                    new double[,] { { 511 }, { 0 } },
                }));
            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Reduce image to half its size.
        /// </summary>
        private void btnReduceHalf_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new AffineTransformation(
                new DoubleMatrix[]
                {
                    new double[,] { { 0.5, 0 }, { 0, 0.5 } }
                },
                new DoubleMatrix[] {
                    new double[,] { { 0 }, { 0 } }
                }));
            Matrix<Pixel> cropped = new Matrix<Pixel>(img.Width / 2, img.Height / 2);

            for (int x = 0; x < cropped.DimensionX; x++)
                for (int y = 0; y < cropped.DimensionY; y++)
                    cropped[x, y] = img.ImageData[x, y];

            img.ImageData = cropped;
            imgComparison.TransformedImage = img;
        }

        /// <summary>
        /// Make the Vent fractal from image.
        /// </summary>
        private void Vent_Fractal_Click(object sender, EventArgs e)
        {
            CustomImage img = GetImageToTransform();
            img.Transform(new AffineTransformation(
                new DoubleMatrix[]
                {
                new double[,] { {0,-0.5 }, { 0.5, 0} }, new double[,] { {0.5,0 }, {0, 0.5 } }, new double[,] { {0.5, 0 }, 
                { 0, 0.5} }   },

                new DoubleMatrix[]
                {
                    new double[,] { {img.Width/2}, {0 } }, new double[,] { {0},{ img.Height/2} }, new double[,] { { img.Width/2},{img.Height/2 } }
                }
                ));
            imgComparison.TransformedImage = img;
        }
    }
}
