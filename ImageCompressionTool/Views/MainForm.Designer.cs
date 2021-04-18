using System;

namespace ImageCompressionTool.Views
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFractalCompression = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Vent_Fractal = new System.Windows.Forms.Button();
            this.btnReduceHalf = new System.Windows.Forms.Button();
            this.btnAffineTransformation = new System.Windows.Forms.Button();
            this.btnKochSnowflakeTransformation = new System.Windows.Forms.Button();
            this.btnReduceTransformation = new System.Windows.Forms.Button();
            this.btnFractalFernTransformation = new System.Windows.Forms.Button();
            this.chkUseTransformedImage = new System.Windows.Forms.CheckBox();
            this.btnFractalTriangleTransformation = new System.Windows.Forms.Button();
            this.btnGreyscale = new System.Windows.Forms.Button();
            this.imgComparison = new ImageCompressionTool.Views.Controls.ImageComparisonControl();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnFractalCompression);
            this.groupBox2.Location = new System.Drawing.Point(12, 494);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(654, 50);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Compression Algorithms";
            // 
            // btnFractalCompression
            // 
            this.btnFractalCompression.Location = new System.Drawing.Point(6, 19);
            this.btnFractalCompression.Name = "btnFractalCompression";
            this.btnFractalCompression.Size = new System.Drawing.Size(75, 23);
            this.btnFractalCompression.TabIndex = 0;
            this.btnFractalCompression.Text = "Fractal";
            this.btnFractalCompression.UseVisualStyleBackColor = true;
            this.btnFractalCompression.Click += new System.EventHandler(this.btnFractalCompression_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.Vent_Fractal);
            this.groupBox3.Controls.Add(this.btnReduceHalf);
            this.groupBox3.Controls.Add(this.btnAffineTransformation);
            this.groupBox3.Controls.Add(this.btnKochSnowflakeTransformation);
            this.groupBox3.Controls.Add(this.btnReduceTransformation);
            this.groupBox3.Controls.Add(this.btnFractalFernTransformation);
            this.groupBox3.Controls.Add(this.chkUseTransformedImage);
            this.groupBox3.Controls.Add(this.btnFractalTriangleTransformation);
            this.groupBox3.Controls.Add(this.btnGreyscale);
            this.groupBox3.Location = new System.Drawing.Point(12, 413);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(654, 77);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Image Transformations";
            // 
            // Vent_Fractal
            // 
            this.Vent_Fractal.Location = new System.Drawing.Point(346, 47);
            this.Vent_Fractal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Vent_Fractal.Name = "Vent_Fractal";
            this.Vent_Fractal.Size = new System.Drawing.Size(96, 24);
            this.Vent_Fractal.TabIndex = 8;
            this.Vent_Fractal.Text = "Vent";
            this.Vent_Fractal.UseVisualStyleBackColor = true;
            this.Vent_Fractal.Click += new System.EventHandler(this.Vent_Fractal_Click);
            // 
            // btnReduceHalf
            // 
            this.btnReduceHalf.Enabled = false;
            this.btnReduceHalf.Location = new System.Drawing.Point(526, 19);
            this.btnReduceHalf.Name = "btnReduceHalf";
            this.btnReduceHalf.Size = new System.Drawing.Size(75, 23);
            this.btnReduceHalf.TabIndex = 7;
            this.btnReduceHalf.Text = "Reduce Half";
            this.btnReduceHalf.UseVisualStyleBackColor = true;
            this.btnReduceHalf.Click += new System.EventHandler(this.btnReduceHalf_Click);
            // 
            // btnAffineTransformation
            // 
            this.btnAffineTransformation.Enabled = false;
            this.btnAffineTransformation.Location = new System.Drawing.Point(445, 19);
            this.btnAffineTransformation.Name = "btnAffineTransformation";
            this.btnAffineTransformation.Size = new System.Drawing.Size(75, 23);
            this.btnAffineTransformation.TabIndex = 6;
            this.btnAffineTransformation.Text = "Affine";
            this.btnAffineTransformation.UseVisualStyleBackColor = true;
            this.btnAffineTransformation.Click += new System.EventHandler(this.btnAffineTransformation_Click);
            // 
            // btnKochSnowflakeTransformation
            // 
            this.btnKochSnowflakeTransformation.Enabled = false;
            this.btnKochSnowflakeTransformation.Location = new System.Drawing.Point(346, 19);
            this.btnKochSnowflakeTransformation.Name = "btnKochSnowflakeTransformation";
            this.btnKochSnowflakeTransformation.Size = new System.Drawing.Size(93, 23);
            this.btnKochSnowflakeTransformation.TabIndex = 5;
            this.btnKochSnowflakeTransformation.Text = "Koch Snowflake";
            this.btnKochSnowflakeTransformation.UseVisualStyleBackColor = true;
            this.btnKochSnowflakeTransformation.Click += new System.EventHandler(this.btnKochSnowflakeTransformation_Click);
            // 
            // btnReduceTransformation
            // 
            this.btnReduceTransformation.Enabled = false;
            this.btnReduceTransformation.Location = new System.Drawing.Point(265, 19);
            this.btnReduceTransformation.Name = "btnReduceTransformation";
            this.btnReduceTransformation.Size = new System.Drawing.Size(75, 23);
            this.btnReduceTransformation.TabIndex = 4;
            this.btnReduceTransformation.Text = "Reduce";
            this.btnReduceTransformation.UseVisualStyleBackColor = true;
            this.btnReduceTransformation.Click += new System.EventHandler(this.btnReduceTransformation_Click);
            // 
            // btnFractalFernTransformation
            // 
            this.btnFractalFernTransformation.Enabled = false;
            this.btnFractalFernTransformation.Location = new System.Drawing.Point(184, 19);
            this.btnFractalFernTransformation.Name = "btnFractalFernTransformation";
            this.btnFractalFernTransformation.Size = new System.Drawing.Size(75, 23);
            this.btnFractalFernTransformation.TabIndex = 3;
            this.btnFractalFernTransformation.Text = "Fractal Fern";
            this.btnFractalFernTransformation.UseVisualStyleBackColor = true;
            this.btnFractalFernTransformation.Click += new System.EventHandler(this.btnFractalFernTransformation_Click);
            // 
            // chkUseTransformedImage
            // 
            this.chkUseTransformedImage.AutoSize = true;
            this.chkUseTransformedImage.Enabled = false;
            this.chkUseTransformedImage.Location = new System.Drawing.Point(7, 49);
            this.chkUseTransformedImage.Name = "chkUseTransformedImage";
            this.chkUseTransformedImage.Size = new System.Drawing.Size(264, 17);
            this.chkUseTransformedImage.TabIndex = 2;
            this.chkUseTransformedImage.Text = "Utiliser l\'image transformée pour les transformations";
            this.chkUseTransformedImage.UseVisualStyleBackColor = true;
            // 
            // btnFractalTriangleTransformation
            // 
            this.btnFractalTriangleTransformation.Enabled = false;
            this.btnFractalTriangleTransformation.Location = new System.Drawing.Point(87, 19);
            this.btnFractalTriangleTransformation.Name = "btnFractalTriangleTransformation";
            this.btnFractalTriangleTransformation.Size = new System.Drawing.Size(91, 23);
            this.btnFractalTriangleTransformation.TabIndex = 1;
            this.btnFractalTriangleTransformation.Text = "Fractal Triangle";
            this.btnFractalTriangleTransformation.UseVisualStyleBackColor = true;
            this.btnFractalTriangleTransformation.Click += new System.EventHandler(this.btnFractalTriangleTransformation_Click);
            // 
            // btnGreyscale
            // 
            this.btnGreyscale.Enabled = false;
            this.btnGreyscale.Location = new System.Drawing.Point(6, 19);
            this.btnGreyscale.Name = "btnGreyscale";
            this.btnGreyscale.Size = new System.Drawing.Size(75, 23);
            this.btnGreyscale.TabIndex = 0;
            this.btnGreyscale.Text = "Grayscale";
            this.btnGreyscale.UseVisualStyleBackColor = true;
            this.btnGreyscale.Click += new System.EventHandler(this.btnGreyscale_Click);
            // 
            // imgComparison
            // 
            this.imgComparison.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgComparison.DefaultExt = "jpg";
            this.imgComparison.DefaultSaveFileName = "transformed.jpg";
            this.imgComparison.FileFilter = "Images (*.png)|*.png|Images (*.jpg)|*.jpg";
            this.imgComparison.Location = new System.Drawing.Point(12, 12);
            this.imgComparison.Margin = new System.Windows.Forms.Padding(4);
            this.imgComparison.Name = "imgComparison";
            this.imgComparison.OriginalImage = null;
            this.imgComparison.Size = new System.Drawing.Size(654, 377);
            this.imgComparison.TabIndex = 5;
            this.imgComparison.TransformedImage = null;
            this.imgComparison.OriginalImageChanged += new ImageCompressionTool.Views.Controls.ImageComparisonControl.OriginalImageChangedEventHandler(this.imgComparison_OriginalImageChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 556);
            this.Controls.Add(this.imgComparison);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.MinimumSize = new System.Drawing.Size(300, 398);
            this.Name = "MainForm";
            this.Text = "Image Compression Tool 0.1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFractalCompression;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnGreyscale;
        private Controls.ImageComparisonControl imgComparison;
        private System.Windows.Forms.Button btnFractalTriangleTransformation;
        private System.Windows.Forms.CheckBox chkUseTransformedImage;
        private System.Windows.Forms.Button btnFractalFernTransformation;
        private System.Windows.Forms.Button btnReduceTransformation;
        private System.Windows.Forms.Button btnKochSnowflakeTransformation;
        private System.Windows.Forms.Button btnAffineTransformation;
        private System.Windows.Forms.Button btnReduceHalf;
        private EventHandler imgComparison_Load;
        private System.Windows.Forms.Button Vent_Fractal;
    }
}

