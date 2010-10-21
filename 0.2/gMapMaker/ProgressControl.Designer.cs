namespace gMapMaker
{
    partial class ProgressControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusLabel.Location = new System.Drawing.Point(3, 31);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(387, 32);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "...";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statusLabel.UseMnemonic = false;
            // 
            // infoLabel
            // 
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.infoLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.infoLabel.Location = new System.Drawing.Point(160, 95);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(75, 20);
            this.infoLabel.TabIndex = 5;
            this.infoLabel.Text = " 0.0%";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            this.progressBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBar.Location = new System.Drawing.Point(4, 66);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(386, 26);
            this.progressBar.TabIndex = 4;
            // 
            // phaseLabel
            // 
            this.phaseLabel.Location = new System.Drawing.Point(3, 5);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(387, 26);
            this.phaseLabel.TabIndex = 7;
            this.phaseLabel.Text = "Phase:";
            this.phaseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.phaseLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.progressBar);
            this.Name = "ProgressControl";
            this.Size = new System.Drawing.Size(394, 124);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
