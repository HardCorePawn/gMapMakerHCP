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
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        this.googleMVlabel = new System.Windows.Forms.Label();
        this.googleMVtextBox = new System.Windows.Forms.TextBox();
        this.imageFormatLabel = new System.Windows.Forms.Label();
        this.imageFormatComboBox = new System.Windows.Forms.ComboBox();
        this.grayscaleCheckBox = new System.Windows.Forms.CheckBox();
        this.saveButton = new System.Windows.Forms.Button();
        this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
        this.chooseFolderButton = new System.Windows.Forms.Button();
        this.topLeftLabel = new System.Windows.Forms.Label();
        this.bottomRightLabel = new System.Windows.Forms.Label();
        this.tlLatTextBox = new System.Windows.Forms.TextBox();
        this.brLongTextBox = new System.Windows.Forms.TextBox();
        this.brLatTextBox = new System.Windows.Forms.TextBox();
        this.tlLongTextBox = new System.Windows.Forms.TextBox();
        this.saveImageFileDialog = new System.Windows.Forms.SaveFileDialog();
        this.zoomLevelLabel = new System.Windows.Forms.Label();
        this.zoomLevelComboBox = new System.Windows.Forms.ComboBox();
        this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
        this.hashSizeComboBox = new System.Windows.Forms.ComboBox();
        this.tilesPerFileComboBox = new System.Windows.Forms.ComboBox();
        this.tileTypeComboBox = new System.Windows.Forms.ComboBox();
        this.ditherCheckBox = new System.Windows.Forms.CheckBox();
        this.operatingModeComboBox = new System.Windows.Forms.ComboBox();
        this.aboutButton = new System.Windows.Forms.Button();
        this.statusLabel = new System.Windows.Forms.Label();
        this.operatingModeLabel = new System.Windows.Forms.Label();
        this.networkGroupBox = new System.Windows.Forms.GroupBox();
        this.proxyListRegexpTextBox = new System.Windows.Forms.TextBox();
        this.proxyListRegexpLabel = new System.Windows.Forms.Label();
        this.groupByZoomCheckBox = new System.Windows.Forms.CheckBox();
        this.label1 = new System.Windows.Forms.Label();
        this.maxParallDnldsComboBox = new System.Windows.Forms.ComboBox();
        this.proxyListUrlsTextBox = new System.Windows.Forms.TextBox();
        this.proxyListUrlLabel = new System.Windows.Forms.Label();
        this.delayLabel = new System.Windows.Forms.Label();
        this.delayNumericUpDown = new System.Windows.Forms.NumericUpDown();
        this.waitRequestsCheckBox = new System.Windows.Forms.CheckBox();
        this.proxyUseCheckBox = new System.Windows.Forms.CheckBox();
        this.imageGroupBox = new System.Windows.Forms.GroupBox();
        this.slicingComboBox = new System.Windows.Forms.ComboBox();
        this.ggZoneGroupBox = new System.Windows.Forms.GroupBox();
        this.selectMapArea = new System.Windows.Forms.Button();
        this.label2 = new System.Windows.Forms.Label();
        this.transpLabel = new System.Windows.Forms.Label();
        this.transpComboBox = new System.Windows.Forms.ComboBox();
        this.generalToolTip = new System.Windows.Forms.ToolTip(this.components);
        this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
        this.hashSizeLabel = new System.Windows.Forms.Label();
        this.tilesPerFileLabel = new System.Windows.Forms.Label();
        this.resetButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
        this.networkGroupBox.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.delayNumericUpDown)).BeginInit();
        this.imageGroupBox.SuspendLayout();
        this.ggZoneGroupBox.SuspendLayout();
        this.SuspendLayout();
        // 
        // googleMVlabel
        // 
        resources.ApplyResources(this.googleMVlabel, "googleMVlabel");
        this.googleMVlabel.Name = "googleMVlabel";
        // 
        // googleMVtextBox
        // 
        resources.ApplyResources(this.googleMVtextBox, "googleMVtextBox");
        this.googleMVtextBox.Name = "googleMVtextBox";
        this.generalToolTip.SetToolTip(this.googleMVtextBox, resources.GetString("googleMVtextBox.ToolTip"));
        this.googleMVtextBox.Validated += new System.EventHandler(this.googleMVtextBox_Validated);
        // 
        // imageFormatLabel
        // 
        resources.ApplyResources(this.imageFormatLabel, "imageFormatLabel");
        this.imageFormatLabel.Name = "imageFormatLabel";
        // 
        // imageFormatComboBox
        // 
        this.imageFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.imageFormatComboBox.Items.AddRange(new object[] {
            resources.GetString("imageFormatComboBox.Items"),
            resources.GetString("imageFormatComboBox.Items1"),
            resources.GetString("imageFormatComboBox.Items2")});
        resources.ApplyResources(this.imageFormatComboBox, "imageFormatComboBox");
        this.imageFormatComboBox.Name = "imageFormatComboBox";
        this.generalToolTip.SetToolTip(this.imageFormatComboBox, resources.GetString("imageFormatComboBox.ToolTip"));
        this.imageFormatComboBox.Validated += new System.EventHandler(this.imageFormatComboBox_Validated);
        // 
        // grayscaleCheckBox
        // 
        resources.ApplyResources(this.grayscaleCheckBox, "grayscaleCheckBox");
        this.grayscaleCheckBox.Name = "grayscaleCheckBox";
        this.generalToolTip.SetToolTip(this.grayscaleCheckBox, resources.GetString("grayscaleCheckBox.ToolTip"));
        this.grayscaleCheckBox.Validated += new System.EventHandler(this.grayscalCheckBox_Validated);
        this.grayscaleCheckBox.CheckedChanged += new System.EventHandler(this.grayscalCheckBox_CheckedChanged);
        // 
        // saveButton
        // 
        resources.ApplyResources(this.saveButton, "saveButton");
        this.saveButton.Name = "saveButton";
        this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
        // 
        // folderBrowserDialog
        // 
        resources.ApplyResources(this.folderBrowserDialog, "folderBrowserDialog");
        // 
        // chooseFolderButton
        // 
        resources.ApplyResources(this.chooseFolderButton, "chooseFolderButton");
        this.chooseFolderButton.Name = "chooseFolderButton";
        this.chooseFolderButton.Click += new System.EventHandler(this.chooseFolderButton_Click);
        // 
        // topLeftLabel
        // 
        resources.ApplyResources(this.topLeftLabel, "topLeftLabel");
        this.topLeftLabel.Name = "topLeftLabel";
        // 
        // bottomRightLabel
        // 
        resources.ApplyResources(this.bottomRightLabel, "bottomRightLabel");
        this.bottomRightLabel.Name = "bottomRightLabel";
        // 
        // tlLatTextBox
        // 
        this.errorProvider.SetError(this.tlLatTextBox, resources.GetString("tlLatTextBox.Error"));
        this.errorProvider.SetIconAlignment(this.tlLatTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tlLatTextBox.IconAlignment"))));
        resources.ApplyResources(this.tlLatTextBox, "tlLatTextBox");
        this.tlLatTextBox.Name = "tlLatTextBox";
        this.generalToolTip.SetToolTip(this.tlLatTextBox, resources.GetString("tlLatTextBox.ToolTip"));
        this.tlLatTextBox.Validated += new System.EventHandler(this.tlLatTextBox_Validated);
        this.tlLatTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.tlLatTextBox_Validating);
        // 
        // brLongTextBox
        // 
        this.errorProvider.SetError(this.brLongTextBox, resources.GetString("brLongTextBox.Error"));
        this.errorProvider.SetIconAlignment(this.brLongTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brLongTextBox.IconAlignment"))));
        resources.ApplyResources(this.brLongTextBox, "brLongTextBox");
        this.brLongTextBox.Name = "brLongTextBox";
        this.generalToolTip.SetToolTip(this.brLongTextBox, resources.GetString("brLongTextBox.ToolTip"));
        this.brLongTextBox.Validated += new System.EventHandler(this.brLongTextBox_Validated);
        this.brLongTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.brLongTextBox_Validating);
        // 
        // brLatTextBox
        // 
        this.errorProvider.SetError(this.brLatTextBox, resources.GetString("brLatTextBox.Error"));
        this.errorProvider.SetIconAlignment(this.brLatTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("brLatTextBox.IconAlignment"))));
        resources.ApplyResources(this.brLatTextBox, "brLatTextBox");
        this.brLatTextBox.Name = "brLatTextBox";
        this.generalToolTip.SetToolTip(this.brLatTextBox, resources.GetString("brLatTextBox.ToolTip"));
        this.brLatTextBox.Validated += new System.EventHandler(this.brLatTextBox_Validated);
        this.brLatTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.brLatTextBox_Validating);
        // 
        // tlLongTextBox
        // 
        this.errorProvider.SetError(this.tlLongTextBox, resources.GetString("tlLongTextBox.Error"));
        this.errorProvider.SetIconAlignment(this.tlLongTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tlLongTextBox.IconAlignment"))));
        resources.ApplyResources(this.tlLongTextBox, "tlLongTextBox");
        this.tlLongTextBox.Name = "tlLongTextBox";
        this.generalToolTip.SetToolTip(this.tlLongTextBox, resources.GetString("tlLongTextBox.ToolTip"));
        this.tlLongTextBox.Validated += new System.EventHandler(this.tlLongTextBox_Validated);
        this.tlLongTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.tlLongTextBox_Validating);
        // 
        // saveImageFileDialog
        // 
        resources.ApplyResources(this.saveImageFileDialog, "saveImageFileDialog");
        // 
        // zoomLevelLabel
        // 
        resources.ApplyResources(this.zoomLevelLabel, "zoomLevelLabel");
        this.zoomLevelLabel.Name = "zoomLevelLabel";
        // 
        // zoomLevelComboBox
        // 
        this.zoomLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.zoomLevelComboBox.Items.AddRange(new object[] {
            resources.GetString("zoomLevelComboBox.Items"),
            resources.GetString("zoomLevelComboBox.Items1"),
            resources.GetString("zoomLevelComboBox.Items2"),
            resources.GetString("zoomLevelComboBox.Items3"),
            resources.GetString("zoomLevelComboBox.Items4"),
            resources.GetString("zoomLevelComboBox.Items5"),
            resources.GetString("zoomLevelComboBox.Items6"),
            resources.GetString("zoomLevelComboBox.Items7"),
            resources.GetString("zoomLevelComboBox.Items8"),
            resources.GetString("zoomLevelComboBox.Items9"),
            resources.GetString("zoomLevelComboBox.Items10"),
            resources.GetString("zoomLevelComboBox.Items11"),
            resources.GetString("zoomLevelComboBox.Items12"),
            resources.GetString("zoomLevelComboBox.Items13"),
            resources.GetString("zoomLevelComboBox.Items14"),
            resources.GetString("zoomLevelComboBox.Items15"),
            resources.GetString("zoomLevelComboBox.Items16"),
            resources.GetString("zoomLevelComboBox.Items17"),
            resources.GetString("zoomLevelComboBox.Items18"),
            resources.GetString("zoomLevelComboBox.Items19"),
            resources.GetString("zoomLevelComboBox.Items20"),
            resources.GetString("zoomLevelComboBox.Items21")});
        resources.ApplyResources(this.zoomLevelComboBox, "zoomLevelComboBox");
        this.zoomLevelComboBox.Name = "zoomLevelComboBox";
        this.generalToolTip.SetToolTip(this.zoomLevelComboBox, resources.GetString("zoomLevelComboBox.ToolTip"));
        this.zoomLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.updateStatus);
        this.zoomLevelComboBox.Validated += new System.EventHandler(this.zoomLevelComboBox_Validated);
        // 
        // errorProvider
        // 
        this.errorProvider.ContainerControl = this;
        // 
        // hashSizeComboBox
        // 
        resources.ApplyResources(this.hashSizeComboBox, "hashSizeComboBox");
        this.errorProvider.SetError(this.hashSizeComboBox, resources.GetString("hashSizeComboBox.Error"));
        this.hashSizeComboBox.FormattingEnabled = true;
        this.hashSizeComboBox.Items.AddRange(new object[] {
            resources.GetString("hashSizeComboBox.Items"),
            resources.GetString("hashSizeComboBox.Items1"),
            resources.GetString("hashSizeComboBox.Items2"),
            resources.GetString("hashSizeComboBox.Items3"),
            resources.GetString("hashSizeComboBox.Items4"),
            resources.GetString("hashSizeComboBox.Items5")});
        this.hashSizeComboBox.Name = "hashSizeComboBox";
        this.generalToolTip.SetToolTip(this.hashSizeComboBox, resources.GetString("hashSizeComboBox.ToolTip"));
        this.hashSizeComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.hashSizeComboBox_Validating);
        this.hashSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.hashSizeComboBox_SelectedIndexChanged);
        this.hashSizeComboBox.Validated += new System.EventHandler(this.hashSizeComboBox_Validated);
        // 
        // tilesPerFileComboBox
        // 
        resources.ApplyResources(this.tilesPerFileComboBox, "tilesPerFileComboBox");
        this.errorProvider.SetError(this.tilesPerFileComboBox, resources.GetString("tilesPerFileComboBox.Error"));
        this.tilesPerFileComboBox.FormattingEnabled = true;
        this.tilesPerFileComboBox.Items.AddRange(new object[] {
            resources.GetString("tilesPerFileComboBox.Items"),
            resources.GetString("tilesPerFileComboBox.Items1"),
            resources.GetString("tilesPerFileComboBox.Items2"),
            resources.GetString("tilesPerFileComboBox.Items3"),
            resources.GetString("tilesPerFileComboBox.Items4"),
            resources.GetString("tilesPerFileComboBox.Items5")});
        this.tilesPerFileComboBox.Name = "tilesPerFileComboBox";
        this.generalToolTip.SetToolTip(this.tilesPerFileComboBox, resources.GetString("tilesPerFileComboBox.ToolTip"));
        this.tilesPerFileComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.tilesPerFileComboBox_Validating);
        this.tilesPerFileComboBox.SelectedIndexChanged += new System.EventHandler(this.tilesPerFileComboBox_SelectedIndexChanged);
        this.tilesPerFileComboBox.Validated += new System.EventHandler(this.tilesPerFileComboBox_Validated);
        // 
        // tileTypeComboBox
        // 
        this.tileTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.tileTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("tileTypeComboBox.Items"),
            resources.GetString("tileTypeComboBox.Items1"),
            resources.GetString("tileTypeComboBox.Items2"),
            resources.GetString("tileTypeComboBox.Items3"),
            resources.GetString("tileTypeComboBox.Items4"),
            resources.GetString("tileTypeComboBox.Items5"),
            resources.GetString("tileTypeComboBox.Items6"),
            resources.GetString("tileTypeComboBox.Items7"),
            resources.GetString("tileTypeComboBox.Items8"),
            resources.GetString("tileTypeComboBox.Items9"),
            resources.GetString("tileTypeComboBox.Items10"),
            resources.GetString("tileTypeComboBox.Items11"),
            resources.GetString("tileTypeComboBox.Items12"),
            resources.GetString("tileTypeComboBox.Items13"),
            resources.GetString("tileTypeComboBox.Items14"),
            resources.GetString("tileTypeComboBox.Items15"),
            resources.GetString("tileTypeComboBox.Items16"),
            resources.GetString("tileTypeComboBox.Items17"),
            resources.GetString("tileTypeComboBox.Items18"),
            resources.GetString("tileTypeComboBox.Items19"),
            resources.GetString("tileTypeComboBox.Items20"),
            resources.GetString("tileTypeComboBox.Items21"),
            resources.GetString("tileTypeComboBox.Items22"),
            resources.GetString("tileTypeComboBox.Items23"),
            resources.GetString("tileTypeComboBox.Items24")});
        resources.ApplyResources(this.tileTypeComboBox, "tileTypeComboBox");
        this.tileTypeComboBox.Name = "tileTypeComboBox";
        this.generalToolTip.SetToolTip(this.tileTypeComboBox, resources.GetString("tileTypeComboBox.ToolTip"));
        this.tileTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.tileTypeComboBox_SelectedIndexChanged);
        this.tileTypeComboBox.Validated += new System.EventHandler(this.tileTypeComboBox_Validated);
        // 
        // ditherCheckBox
        // 
        resources.ApplyResources(this.ditherCheckBox, "ditherCheckBox");
        this.ditherCheckBox.Name = "ditherCheckBox";
        this.generalToolTip.SetToolTip(this.ditherCheckBox, resources.GetString("ditherCheckBox.ToolTip"));
        this.ditherCheckBox.Validated += new System.EventHandler(this.ditherCheckBox_Validated);
        this.ditherCheckBox.CheckedChanged += new System.EventHandler(this.ditherCheckBox_CheckedChanged);
        // 
        // operatingModeComboBox
        // 
        resources.ApplyResources(this.operatingModeComboBox, "operatingModeComboBox");
        this.operatingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.operatingModeComboBox.Items.AddRange(new object[] {
            resources.GetString("operatingModeComboBox.Items"),
            resources.GetString("operatingModeComboBox.Items1"),
            resources.GetString("operatingModeComboBox.Items2"),
            resources.GetString("operatingModeComboBox.Items3"),
            resources.GetString("operatingModeComboBox.Items4"),
            resources.GetString("operatingModeComboBox.Items5"),
            resources.GetString("operatingModeComboBox.Items6")});
        this.operatingModeComboBox.Name = "operatingModeComboBox";
        this.generalToolTip.SetToolTip(this.operatingModeComboBox, resources.GetString("operatingModeComboBox.ToolTip"));
        this.operatingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.operatingModeComboBox_SelectedIndexChanged);
        this.operatingModeComboBox.Validated += new System.EventHandler(this.operatingModeComboBox_Validated);
        // 
        // aboutButton
        // 
        resources.ApplyResources(this.aboutButton, "aboutButton");
        this.aboutButton.Name = "aboutButton";
        this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
        // 
        // statusLabel
        // 
        resources.ApplyResources(this.statusLabel, "statusLabel");
        this.statusLabel.Name = "statusLabel";
        // 
        // operatingModeLabel
        // 
        resources.ApplyResources(this.operatingModeLabel, "operatingModeLabel");
        this.operatingModeLabel.Name = "operatingModeLabel";
        // 
        // networkGroupBox
        // 
        this.networkGroupBox.Controls.Add(this.proxyListRegexpTextBox);
        this.networkGroupBox.Controls.Add(this.proxyListRegexpLabel);
        this.networkGroupBox.Controls.Add(this.groupByZoomCheckBox);
        this.networkGroupBox.Controls.Add(this.label1);
        this.networkGroupBox.Controls.Add(this.maxParallDnldsComboBox);
        this.networkGroupBox.Controls.Add(this.proxyListUrlsTextBox);
        this.networkGroupBox.Controls.Add(this.proxyListUrlLabel);
        this.networkGroupBox.Controls.Add(this.delayLabel);
        this.networkGroupBox.Controls.Add(this.delayNumericUpDown);
        this.networkGroupBox.Controls.Add(this.waitRequestsCheckBox);
        this.networkGroupBox.Controls.Add(this.proxyUseCheckBox);
        this.networkGroupBox.Controls.Add(this.googleMVtextBox);
        this.networkGroupBox.Controls.Add(this.googleMVlabel);
        this.networkGroupBox.Controls.Add(this.chooseFolderButton);
        resources.ApplyResources(this.networkGroupBox, "networkGroupBox");
        this.networkGroupBox.Name = "networkGroupBox";
        this.networkGroupBox.TabStop = false;
        // 
        // proxyListRegexpTextBox
        // 
        resources.ApplyResources(this.proxyListRegexpTextBox, "proxyListRegexpTextBox");
        this.proxyListRegexpTextBox.Name = "proxyListRegexpTextBox";
        this.generalToolTip.SetToolTip(this.proxyListRegexpTextBox, resources.GetString("proxyListRegexpTextBox.ToolTip"));
        this.proxyListRegexpTextBox.Validated += new System.EventHandler(this.proxyListRegexpTextBox_Validated);
        // 
        // proxyListRegexpLabel
        // 
        resources.ApplyResources(this.proxyListRegexpLabel, "proxyListRegexpLabel");
        this.proxyListRegexpLabel.Name = "proxyListRegexpLabel";
        // 
        // groupByZoomCheckBox
        // 
        resources.ApplyResources(this.groupByZoomCheckBox, "groupByZoomCheckBox");
        this.groupByZoomCheckBox.Name = "groupByZoomCheckBox";
        this.generalToolTip.SetToolTip(this.groupByZoomCheckBox, resources.GetString("groupByZoomCheckBox.ToolTip"));
        this.groupByZoomCheckBox.UseVisualStyleBackColor = true;
        this.groupByZoomCheckBox.CheckedChanged += new System.EventHandler(this.groupByZoomCheckBox_CheckedChanged);
        // 
        // label1
        // 
        resources.ApplyResources(this.label1, "label1");
        this.label1.Name = "label1";
        // 
        // maxParallDnldsComboBox
        // 
        this.maxParallDnldsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.maxParallDnldsComboBox.FormattingEnabled = true;
        this.maxParallDnldsComboBox.Items.AddRange(new object[] {
            resources.GetString("maxParallDnldsComboBox.Items"),
            resources.GetString("maxParallDnldsComboBox.Items1"),
            resources.GetString("maxParallDnldsComboBox.Items2"),
            resources.GetString("maxParallDnldsComboBox.Items3"),
            resources.GetString("maxParallDnldsComboBox.Items4"),
            resources.GetString("maxParallDnldsComboBox.Items5"),
            resources.GetString("maxParallDnldsComboBox.Items6"),
            resources.GetString("maxParallDnldsComboBox.Items7"),
            resources.GetString("maxParallDnldsComboBox.Items8"),
            resources.GetString("maxParallDnldsComboBox.Items9")});
        resources.ApplyResources(this.maxParallDnldsComboBox, "maxParallDnldsComboBox");
        this.maxParallDnldsComboBox.Name = "maxParallDnldsComboBox";
        this.generalToolTip.SetToolTip(this.maxParallDnldsComboBox, resources.GetString("maxParallDnldsComboBox.ToolTip"));
        this.maxParallDnldsComboBox.SelectedIndexChanged += new System.EventHandler(this.maxParallelDownloads_SelectedIndexChanged);
        // 
        // proxyListUrlsTextBox
        // 
        resources.ApplyResources(this.proxyListUrlsTextBox, "proxyListUrlsTextBox");
        this.proxyListUrlsTextBox.Name = "proxyListUrlsTextBox";
        this.generalToolTip.SetToolTip(this.proxyListUrlsTextBox, resources.GetString("proxyListUrlsTextBox.ToolTip"));
        this.proxyListUrlsTextBox.Validated += new System.EventHandler(this.proxyListUrlsTextBox_Validated);
        this.proxyListUrlsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.proxyListUrlsTextBox_Validating);
        // 
        // proxyListUrlLabel
        // 
        resources.ApplyResources(this.proxyListUrlLabel, "proxyListUrlLabel");
        this.proxyListUrlLabel.Name = "proxyListUrlLabel";
        // 
        // delayLabel
        // 
        resources.ApplyResources(this.delayLabel, "delayLabel");
        this.delayLabel.Name = "delayLabel";
        // 
        // delayNumericUpDown
        // 
        this.delayNumericUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
        resources.ApplyResources(this.delayNumericUpDown, "delayNumericUpDown");
        this.delayNumericUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
        this.delayNumericUpDown.Name = "delayNumericUpDown";
        this.generalToolTip.SetToolTip(this.delayNumericUpDown, resources.GetString("delayNumericUpDown.ToolTip"));
        this.delayNumericUpDown.Validated += new System.EventHandler(this.delayNumericUpDown_Validated);
        // 
        // waitRequestsCheckBox
        // 
        resources.ApplyResources(this.waitRequestsCheckBox, "waitRequestsCheckBox");
        this.waitRequestsCheckBox.Name = "waitRequestsCheckBox";
        this.generalToolTip.SetToolTip(this.waitRequestsCheckBox, resources.GetString("waitRequestsCheckBox.ToolTip"));
        this.waitRequestsCheckBox.UseVisualStyleBackColor = true;
        this.waitRequestsCheckBox.Validated += new System.EventHandler(this.waitRequestsCheckBox_Validated);
        this.waitRequestsCheckBox.CheckedChanged += new System.EventHandler(this.waitRequestsCheckBox_CheckedChanged);
        // 
        // proxyUseCheckBox
        // 
        resources.ApplyResources(this.proxyUseCheckBox, "proxyUseCheckBox");
        this.proxyUseCheckBox.Name = "proxyUseCheckBox";
        this.generalToolTip.SetToolTip(this.proxyUseCheckBox, resources.GetString("proxyUseCheckBox.ToolTip"));
        this.proxyUseCheckBox.UseVisualStyleBackColor = true;
        this.proxyUseCheckBox.Validated += new System.EventHandler(this.proxyUseCheckBox_Validated);
        this.proxyUseCheckBox.CheckedChanged += new System.EventHandler(this.proxyUseCheckBox_CheckedChanged);
        // 
        // imageGroupBox
        // 
        this.imageGroupBox.Controls.Add(this.imageFormatLabel);
        this.imageGroupBox.Controls.Add(this.grayscaleCheckBox);
        this.imageGroupBox.Controls.Add(this.ditherCheckBox);
        this.imageGroupBox.Controls.Add(this.imageFormatComboBox);
        resources.ApplyResources(this.imageGroupBox, "imageGroupBox");
        this.imageGroupBox.Name = "imageGroupBox";
        this.imageGroupBox.TabStop = false;
        // 
        // slicingComboBox
        // 
        resources.ApplyResources(this.slicingComboBox, "slicingComboBox");
        this.slicingComboBox.FormattingEnabled = true;
        this.slicingComboBox.Items.AddRange(new object[] {
            resources.GetString("slicingComboBox.Items"),
            resources.GetString("slicingComboBox.Items1"),
            resources.GetString("slicingComboBox.Items2"),
            resources.GetString("slicingComboBox.Items3")});
        this.slicingComboBox.Name = "slicingComboBox";
        this.generalToolTip.SetToolTip(this.slicingComboBox, resources.GetString("slicingComboBox.ToolTip"));
        this.slicingComboBox.SelectedIndexChanged += new System.EventHandler(this.updateStatus);
        this.slicingComboBox.Validated += new System.EventHandler(this.slicingComboBox_Validated);
        // 
        // ggZoneGroupBox
        // 
        this.ggZoneGroupBox.Controls.Add(this.selectMapArea);
        this.ggZoneGroupBox.Controls.Add(this.label2);
        this.ggZoneGroupBox.Controls.Add(this.transpLabel);
        this.ggZoneGroupBox.Controls.Add(this.transpComboBox);
        this.ggZoneGroupBox.Controls.Add(this.topLeftLabel);
        this.ggZoneGroupBox.Controls.Add(this.bottomRightLabel);
        this.ggZoneGroupBox.Controls.Add(this.tlLatTextBox);
        this.ggZoneGroupBox.Controls.Add(this.brLongTextBox);
        this.ggZoneGroupBox.Controls.Add(this.brLatTextBox);
        this.ggZoneGroupBox.Controls.Add(this.tlLongTextBox);
        this.ggZoneGroupBox.Controls.Add(this.zoomLevelLabel);
        this.ggZoneGroupBox.Controls.Add(this.tileTypeComboBox);
        this.ggZoneGroupBox.Controls.Add(this.zoomLevelComboBox);
        resources.ApplyResources(this.ggZoneGroupBox, "ggZoneGroupBox");
        this.ggZoneGroupBox.Name = "ggZoneGroupBox";
        this.ggZoneGroupBox.TabStop = false;
        // 
        // selectMapArea
        // 
        resources.ApplyResources(this.selectMapArea, "selectMapArea");
        this.selectMapArea.Name = "selectMapArea";
        this.generalToolTip.SetToolTip(this.selectMapArea, resources.GetString("selectMapArea.ToolTip"));
        this.selectMapArea.UseVisualStyleBackColor = true;
        this.selectMapArea.Click += new System.EventHandler(this.selectMapArea_Click);
        // 
        // label2
        // 
        resources.ApplyResources(this.label2, "label2");
        this.label2.Name = "label2";
        // 
        // transpLabel
        // 
        resources.ApplyResources(this.transpLabel, "transpLabel");
        this.transpLabel.Name = "transpLabel";
        // 
        // transpComboBox
        // 
        this.transpComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.transpComboBox.FormattingEnabled = true;
        this.transpComboBox.Items.AddRange(new object[] {
            resources.GetString("transpComboBox.Items"),
            resources.GetString("transpComboBox.Items1"),
            resources.GetString("transpComboBox.Items2"),
            resources.GetString("transpComboBox.Items3"),
            resources.GetString("transpComboBox.Items4"),
            resources.GetString("transpComboBox.Items5"),
            resources.GetString("transpComboBox.Items6"),
            resources.GetString("transpComboBox.Items7"),
            resources.GetString("transpComboBox.Items8"),
            resources.GetString("transpComboBox.Items9")});
        resources.ApplyResources(this.transpComboBox, "transpComboBox");
        this.transpComboBox.Name = "transpComboBox";
        this.generalToolTip.SetToolTip(this.transpComboBox, resources.GetString("transpComboBox.ToolTip"));
        this.transpComboBox.Validated += new System.EventHandler(this.transpComboBox_Validated);
        // 
        // generalToolTip
        // 
        this.generalToolTip.AutomaticDelay = 600;
        this.generalToolTip.IsBalloon = true;
        this.generalToolTip.ShowAlways = true;
        // 
        // openFileDialog
        // 
        this.openFileDialog.DefaultExt = "map";
        resources.ApplyResources(this.openFileDialog, "openFileDialog");
        this.openFileDialog.Multiselect = true;
        // 
        // hashSizeLabel
        // 
        resources.ApplyResources(this.hashSizeLabel, "hashSizeLabel");
        this.hashSizeLabel.Name = "hashSizeLabel";
        // 
        // tilesPerFileLabel
        // 
        resources.ApplyResources(this.tilesPerFileLabel, "tilesPerFileLabel");
        this.tilesPerFileLabel.Name = "tilesPerFileLabel";
        // 
        // resetButton
        // 
        resources.ApplyResources(this.resetButton, "resetButton");
        this.resetButton.Name = "resetButton";
        this.resetButton.UseVisualStyleBackColor = true;
        this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
        // 
        // MainForm
        // 
        this.AcceptButton = this.saveButton;
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.resetButton);
        this.Controls.Add(this.tilesPerFileComboBox);
        this.Controls.Add(this.tilesPerFileLabel);
        this.Controls.Add(this.hashSizeComboBox);
        this.Controls.Add(this.hashSizeLabel);
        this.Controls.Add(this.slicingComboBox);
        this.Controls.Add(this.ggZoneGroupBox);
        this.Controls.Add(this.imageGroupBox);
        this.Controls.Add(this.networkGroupBox);
        this.Controls.Add(this.operatingModeComboBox);
        this.Controls.Add(this.operatingModeLabel);
        this.Controls.Add(this.statusLabel);
        this.Controls.Add(this.aboutButton);
        this.Controls.Add(this.saveButton);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
        this.networkGroupBox.ResumeLayout(false);
        this.networkGroupBox.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.delayNumericUpDown)).EndInit();
        this.imageGroupBox.ResumeLayout(false);
        this.imageGroupBox.PerformLayout();
        this.ggZoneGroupBox.ResumeLayout(false);
        this.ggZoneGroupBox.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label imageFormatLabel;
    private System.Windows.Forms.TextBox googleMVtextBox;
    private System.Windows.Forms.Label googleMVlabel;
    private System.Windows.Forms.ComboBox imageFormatComboBox;
    private System.Windows.Forms.CheckBox grayscaleCheckBox;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    private System.Windows.Forms.Button chooseFolderButton;
    private System.Windows.Forms.Label topLeftLabel;
    private System.Windows.Forms.Label bottomRightLabel;
    private System.Windows.Forms.TextBox tlLatTextBox;
    private System.Windows.Forms.TextBox brLongTextBox;
    private System.Windows.Forms.TextBox brLatTextBox;
    private System.Windows.Forms.TextBox tlLongTextBox;
    private System.Windows.Forms.SaveFileDialog saveImageFileDialog;
    private System.Windows.Forms.Label zoomLevelLabel;
    private System.Windows.Forms.ComboBox zoomLevelComboBox;
    private System.Windows.Forms.ErrorProvider errorProvider;
      private System.Windows.Forms.ComboBox tileTypeComboBox;
    private System.Windows.Forms.ToolTip generalToolTip;
    private System.Windows.Forms.CheckBox ditherCheckBox;
    private System.Windows.Forms.Button aboutButton;
    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.ComboBox operatingModeComboBox;
    private System.Windows.Forms.Label operatingModeLabel;
    private System.Windows.Forms.GroupBox ggZoneGroupBox;
    private System.Windows.Forms.GroupBox imageGroupBox;
    private System.Windows.Forms.GroupBox networkGroupBox;
    private System.Windows.Forms.NumericUpDown delayNumericUpDown;
    private System.Windows.Forms.CheckBox waitRequestsCheckBox;
    private System.Windows.Forms.CheckBox proxyUseCheckBox;
    private System.Windows.Forms.Label delayLabel;
    private System.Windows.Forms.TextBox proxyListUrlsTextBox;
    private System.Windows.Forms.Label proxyListUrlLabel;
    private System.Windows.Forms.Label transpLabel;
    private System.Windows.Forms.ComboBox transpComboBox;
    private System.Windows.Forms.ComboBox slicingComboBox;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
      private System.Windows.Forms.CheckBox groupByZoomCheckBox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox maxParallDnldsComboBox;
      private System.Windows.Forms.Label hashSizeLabel;
      private System.Windows.Forms.Label tilesPerFileLabel;
      private System.Windows.Forms.ComboBox hashSizeComboBox;
      private System.Windows.Forms.ComboBox tilesPerFileComboBox;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label proxyListRegexpLabel;
      private System.Windows.Forms.TextBox proxyListRegexpTextBox;
      private System.Windows.Forms.Button resetButton;
      private System.Windows.Forms.Button selectMapArea;
  }
}

