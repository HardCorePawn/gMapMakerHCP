/*
 * gMapMaker
 * Copyright (C) 2007  Damien Debin
 * http://debin.net/gMapMaker/
 * 
 * This file is part of gMapMaker.
 * 
 * Foobar is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * Foobar is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Foobar; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

namespace gMapMaker
{
  partial class AboutBox
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
        this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.labelCompanyName = new System.Windows.Forms.LinkLabel();
        this.logoPictureBox = new System.Windows.Forms.PictureBox();
        this.labelProductName = new System.Windows.Forms.Label();
        this.textBoxDescription = new System.Windows.Forms.TextBox();
        this.labelCopyright = new System.Windows.Forms.Label();
        this.okButton = new System.Windows.Forms.Button();
        this.tableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
        this.SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
        this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 2);
        this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
        this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
        this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 3);
        this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 1);
        this.tableLayoutPanel.Controls.Add(this.okButton, 1, 4);
        this.tableLayoutPanel.Name = "tableLayoutPanel";
        // 
        // labelCompanyName
        // 
        resources.ApplyResources(this.labelCompanyName, "labelCompanyName");
        this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
        this.labelCompanyName.Name = "labelCompanyName";
        this.labelCompanyName.TabStop = true;
        this.labelCompanyName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelCompanyName_LinkClicked);
        // 
        // logoPictureBox
        // 
        resources.ApplyResources(this.logoPictureBox, "logoPictureBox");
        this.logoPictureBox.Image = global::gMapMaker.Properties.Resources.browser64;
        this.logoPictureBox.Name = "logoPictureBox";
        this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 4);
        this.logoPictureBox.TabStop = false;
        // 
        // labelProductName
        // 
        resources.ApplyResources(this.labelProductName, "labelProductName");
        this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
        this.labelProductName.Name = "labelProductName";
        // 
        // textBoxDescription
        // 
        resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
        this.textBoxDescription.Name = "textBoxDescription";
        this.textBoxDescription.ReadOnly = true;
        this.textBoxDescription.TabStop = false;
        // 
        // labelCopyright
        // 
        resources.ApplyResources(this.labelCopyright, "labelCopyright");
        this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
        this.labelCopyright.Name = "labelCopyright";
        // 
        // okButton
        // 
        resources.ApplyResources(this.okButton, "okButton");
        this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.okButton.Name = "okButton";
        // 
        // AboutBox
        // 
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.tableLayoutPanel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "AboutBox";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.tableLayoutPanel.ResumeLayout(false);
        this.tableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.PictureBox logoPictureBox;
      private System.Windows.Forms.Label labelProductName;
    private System.Windows.Forms.TextBox textBoxDescription;
      private System.Windows.Forms.Button okButton;
      private System.Windows.Forms.Label labelCopyright;
      private System.Windows.Forms.LinkLabel labelCompanyName;
  }
}
