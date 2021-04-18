namespace ImageCompressionTool.Views.Compression
{
    partial class FractalCompressionForm
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
            this.imgComparisonControl = new ImageCompressionTool.Views.Controls.ImageComparisonControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCalculatePSNR = new System.Windows.Forms.Button();
            this.chkSmoothImage = new System.Windows.Forms.CheckBox();
            this.btnSaveUncompressedImage = new System.Windows.Forms.Button();
            this.upDownRSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCompress = new System.Windows.Forms.Button();
            this.btnUncompress = new System.Windows.Forms.Button();
            this.btnCompleteUncompress = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownRSize)).BeginInit();
            this.SuspendLayout();
            // 
            // imgComparisonControl
            // 
            this.imgComparisonControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imgComparisonControl.DefaultExt = "jpg";
            this.imgComparisonControl.DefaultSaveFileName = "transformed.jpg";
            this.imgComparisonControl.FileFilter = "Images (*.png)|*.png|Images (*.jpg)|*.jpg";
            this.imgComparisonControl.Location = new System.Drawing.Point(12, 12);
            this.imgComparisonControl.Name = "imgComparisonControl";
            this.imgComparisonControl.OriginalImage = null;
            this.imgComparisonControl.Size = new System.Drawing.Size(655, 385);
            this.imgComparisonControl.TabIndex = 0;
            this.imgComparisonControl.TransformedImage = null;
            this.imgComparisonControl.OriginalImageChanged += new ImageCompressionTool.Views.Controls.ImageComparisonControl.OriginalImageChangedEventHandler(this.imgComparisonControl_OriginalImageChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnCalculatePSNR);
            this.groupBox1.Controls.Add(this.chkSmoothImage);
            this.groupBox1.Controls.Add(this.btnSaveUncompressedImage);
            this.groupBox1.Controls.Add(this.upDownRSize);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 403);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(655, 77);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compression options";
            // 
            // btnCalculatePSNR
            // 
            this.btnCalculatePSNR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculatePSNR.Location = new System.Drawing.Point(487, 20);
            this.btnCalculatePSNR.Name = "btnCalculatePSNR";
            this.btnCalculatePSNR.Size = new System.Drawing.Size(162, 23);
            this.btnCalculatePSNR.TabIndex = 5;
            this.btnCalculatePSNR.Text = "Compare images";
            this.btnCalculatePSNR.UseVisualStyleBackColor = true;
            this.btnCalculatePSNR.Click += new System.EventHandler(this.btnCalculatePSNR_Click);
            // 
            // chkSmoothImage
            // 
            this.chkSmoothImage.AutoSize = true;
            this.chkSmoothImage.Checked = true;
            this.chkSmoothImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSmoothImage.Location = new System.Drawing.Point(10, 44);
            this.chkSmoothImage.Name = "chkSmoothImage";
            this.chkSmoothImage.Size = new System.Drawing.Size(126, 17);
            this.chkSmoothImage.TabIndex = 4;
            this.chkSmoothImage.Text = "Smooth output image";
            this.chkSmoothImage.UseVisualStyleBackColor = true;
            // 
            // btnSaveUncompressedImage
            // 
            this.btnSaveUncompressedImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveUncompressedImage.Enabled = false;
            this.btnSaveUncompressedImage.Location = new System.Drawing.Point(487, 48);
            this.btnSaveUncompressedImage.Name = "btnSaveUncompressedImage";
            this.btnSaveUncompressedImage.Size = new System.Drawing.Size(162, 23);
            this.btnSaveUncompressedImage.TabIndex = 3;
            this.btnSaveUncompressedImage.Text = "Save uncompressed image";
            this.btnSaveUncompressedImage.UseVisualStyleBackColor = true;
            this.btnSaveUncompressedImage.Click += new System.EventHandler(this.btnSaveUncompressedImage_Click);
            // 
            // upDownRSize
            // 
            this.upDownRSize.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.upDownRSize.Location = new System.Drawing.Point(127, 18);
            this.upDownRSize.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.upDownRSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.upDownRSize.Name = "upDownRSize";
            this.upDownRSize.Size = new System.Drawing.Size(55, 20);
            this.upDownRSize.TabIndex = 2;
            this.upDownRSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "R size (small squares) :";
            // 
            // btnCompress
            // 
            this.btnCompress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompress.Enabled = false;
            this.btnCompress.Location = new System.Drawing.Point(592, 486);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(75, 23);
            this.btnCompress.TabIndex = 2;
            this.btnCompress.Text = "Compress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // btnUncompress
            // 
            this.btnUncompress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUncompress.Enabled = false;
            this.btnUncompress.Location = new System.Drawing.Point(511, 486);
            this.btnUncompress.Name = "btnUncompress";
            this.btnUncompress.Size = new System.Drawing.Size(75, 23);
            this.btnUncompress.TabIndex = 3;
            this.btnUncompress.Text = "Uncompress";
            this.btnUncompress.UseVisualStyleBackColor = true;
            this.btnUncompress.Click += new System.EventHandler(this.btnUncompress_Click);
            // 
            // btnCompleteUncompress
            // 
            this.btnCompleteUncompress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompleteUncompress.Enabled = false;
            this.btnCompleteUncompress.Location = new System.Drawing.Point(370, 486);
            this.btnCompleteUncompress.Name = "btnCompleteUncompress";
            this.btnCompleteUncompress.Size = new System.Drawing.Size(135, 23);
            this.btnCompleteUncompress.TabIndex = 4;
            this.btnCompleteUncompress.Text = "Complete uncompress";
            this.btnCompleteUncompress.UseVisualStyleBackColor = true;
            this.btnCompleteUncompress.Click += new System.EventHandler(this.btnCompleteUncompress_Click);
            // 
            // FractalCompressionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 521);
            this.Controls.Add(this.btnCompleteUncompress);
            this.Controls.Add(this.btnUncompress);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imgComparisonControl);
            this.Name = "FractalCompressionForm";
            this.Text = "Fractal Image Compression";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownRSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ImageComparisonControl imgComparisonControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown upDownRSize;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Button btnUncompress;
        private System.Windows.Forms.Button btnSaveUncompressedImage;
        private System.Windows.Forms.Button btnCompleteUncompress;
        private System.Windows.Forms.CheckBox chkSmoothImage;
        private System.Windows.Forms.Button btnCalculatePSNR;
    }
}