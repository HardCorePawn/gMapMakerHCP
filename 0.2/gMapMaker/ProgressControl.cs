using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace gMapMaker
{
    public partial class ProgressControl : UserControl
    {
        double progressValue;

        public ProgressControl()
        {
            InitializeComponent();
            this.Cursor = Cursors.Default;
        }

        public void Initialize()
        {
            progressValue = 0;
            SetProgressValue(0);
            statusLabel.Text = "...";
            phaseLabel.Text = "Phase";
        }

        public void SetProgressValue(double inc)
        {
            progressValue += inc;
            progressBar.Value = (int)Math.Round(progressValue * 100.0);
            infoLabel.Text = progressValue.ToString(" 0.0%", MainForm.ciUS.NumberFormat);
        }

        public void SetStatusLabel(string status)
        {
            statusLabel.Text = status + "...";
        }

        public void SetPhaseLabel(string phase)
        {
            phaseLabel.Text = "Phase" + phase;
        }
    }
}
