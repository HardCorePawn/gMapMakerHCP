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
  partial class ProgressWindow
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressWindow));
        this.panel1 = new System.Windows.Forms.Panel();
        this.cancelButton = new System.Windows.Forms.Button();
        this.layoutPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.pauseButton = new System.Windows.Forms.Button();
        this.panel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // panel1
        // 
        this.panel1.Controls.Add(this.pauseButton);
        this.panel1.Controls.Add(this.cancelButton);
        this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
        resources.ApplyResources(this.panel1, "panel1");
        this.panel1.Name = "panel1";
        // 
        // cancelButton
        // 
        resources.ApplyResources(this.cancelButton, "cancelButton");
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.UseVisualStyleBackColor = true;
        this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
        // 
        // layoutPanel
        // 
        resources.ApplyResources(this.layoutPanel, "layoutPanel");
        this.layoutPanel.Name = "layoutPanel";
        // 
        // pauseButton
        // 
        resources.ApplyResources(this.pauseButton, "pauseButton");
        this.pauseButton.Name = "pauseButton";
        this.pauseButton.UseVisualStyleBackColor = true;
        this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
        // 
        // ProgressWindow
        // 
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.cancelButton;
        this.Controls.Add(this.layoutPanel);
        this.Controls.Add(this.panel1);
        this.Cursor = System.Windows.Forms.Cursors.Default;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.Name = "ProgressWindow";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressWindow_FormClosing);
        this.panel1.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.FlowLayoutPanel layoutPanel;
      private System.Windows.Forms.Button pauseButton;
}
}