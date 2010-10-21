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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;

namespace gMapMaker
{
    public partial class MainForm : Form
    {
        public static CultureInfo ciUS;

        ComponentResourceManager resources;
        ProgressWindow pw;

        public MainForm()
        {
            ciUS = CultureInfo.GetCultureInfo("en-US");

            InitializeComponent();

            resources = new ComponentResourceManager(typeof(MainForm));

            InitUIwithValues();
        }

        private void InitUIwithValues()
        {
            if (!Path.IsPathRooted(Properties.Settings.Default.CacheFullPath))
            {
                Properties.Settings.Default.CacheFullPath = Path.GetFullPath(Properties.Settings.Default.CacheFullPath);
            }

            tlLatTextBox.Text = Properties.Settings.Default.TopLat.ToString(ciUS.NumberFormat);
            tlLongTextBox.Text = Properties.Settings.Default.LeftLong.ToString(ciUS.NumberFormat);
            brLatTextBox.Text = Properties.Settings.Default.BottomLat.ToString(ciUS.NumberFormat);
            brLongTextBox.Text = Properties.Settings.Default.RightLong.ToString(ciUS.NumberFormat);
            googleMVtextBox.Text = Properties.Settings.Default.CacheFullPath;
            zoomLevelComboBox.SelectedItem = Properties.Settings.Default.ZoomLevel.ToString();
            grayscaleCheckBox.Checked = Properties.Settings.Default.GrayScale;
            imageFormatComboBox.Text = Properties.Settings.Default.ImageFormat;
            tileTypeComboBox.SelectedIndex = Properties.Settings.Default.TileType;
            ditherCheckBox.Checked = Properties.Settings.Default.Dither;
            proxyUseCheckBox.Checked = Properties.Settings.Default.UseProxy;
            waitRequestsCheckBox.Checked = Properties.Settings.Default.SlowDown;
            delayNumericUpDown.Value = Properties.Settings.Default.SlowDownDelay;
            proxyListUrlsTextBox.Text = Properties.Settings.Default.ProxyListURLs;
            proxyListRegexpTextBox.Text = Properties.Settings.Default.ProxyListRegexp;
            slicingComboBox.SelectedIndex = (int)Math.Sqrt(Properties.Settings.Default.NumberOfSlices) - 1;
            operatingModeComboBox.SelectedIndex = Properties.Settings.Default.OperatingMode;
            //gkl changes - added new controls and their values
            groupByZoomCheckBox.Checked = Properties.Settings.Default.GroupByZoom;
            maxParallDnldsComboBox.Text = Properties.Settings.Default.MaxDownloadsInParallel.ToString();
            hashSizeComboBox.Text = Properties.Settings.Default.MGMapsHashSize.ToString();
            tilesPerFileComboBox.Text = Properties.Settings.Default.MGMapsTilesPerFile.ToString();
            enableDisableTiles();

            //operatingModeComboBox.Items.RemoveAt(4);

            uint transp = Math.Max(Properties.Settings.Default.DrivingMapTransparency, 0);
            transp = Math.Min(transp, 100);
            transpComboBox.SelectedItem = transp.ToString() + '%';

            errorProvider.SetError(tlLongTextBox, "");
            errorProvider.SetError(tlLatTextBox, "");
            errorProvider.SetError(brLongTextBox, "");
            errorProvider.SetError(brLatTextBox, "");
            errorProvider.SetError(hashSizeComboBox, "");
            errorProvider.SetError(tilesPerFileComboBox, "");

            ValidateChildren(ValidationConstraints.Enabled);

            updateStatus(null, null);
        }
        
        internal static void ShowErrorBox(string message)
        {
            MessageBox.Show(Properties.Resources.ApplicationErrorText + "\n\n" + message, Properties.Resources.ApplicationErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }

        internal static void ShowErrorBoxEx(string message)
        {
            //MessageBox.Show(Properties.Resources.ApplicationErrorText + "\n\n" + message, Properties.Resources.ApplicationErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            ErrorBox.ShowErrorMessage(message);
        }
        
        private void chooseFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            if (!String.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                googleMVtextBox.Text = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.CacheFullPath = googleMVtextBox.Text;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.MGMapsMode = (Properties.Settings.Default.OperatingMode > 2);

            ValidateChildren(ValidationConstraints.Enabled);

            if ((Properties.Settings.Default.OperatingMode == 1) || (Properties.Settings.Default.OperatingMode == 2))
            {
                switch (imageFormatComboBox.Text)
                {
                    case "PNG":
                        saveImageFileDialog.DefaultExt = "png";
                        saveImageFileDialog.Filter = Properties.Resources.PngWildcard;
                        break;
                    case "JPEG":
                        saveImageFileDialog.DefaultExt = "jpg";
                        saveImageFileDialog.Filter = Properties.Resources.JpegWildcard;
                        break;
                    case "TIFF":
                        saveImageFileDialog.DefaultExt = "tif";
                        saveImageFileDialog.Filter = Properties.Resources.TiffWildcard;
                        break;
                }

                saveImageFileDialog.FileName = "";
                saveImageFileDialog.ShowDialog();

                if (String.IsNullOrEmpty(saveImageFileDialog.FileName))
                {
                    return;
                }
            }
            else if ((Properties.Settings.Default.OperatingMode == 4) || (Properties.Settings.Default.OperatingMode == 5) || (Properties.Settings.Default.OperatingMode == 6))
            {
                openFileDialog.ShowDialog();

                if (String.IsNullOrEmpty(openFileDialog.FileName))
                {
                    return;
                }
            }

            Cursor.Current = Cursors.WaitCursor;
            saveButton.Enabled = false;
            //gkl changes - created new window holding all progress bars
            pw = new ProgressWindow((int)Properties.Settings.Default.MaxDownloadsInParallel);

            //gkl changes - added call to create directory instead of doing it in callback from worker
            if (!Directory.Exists(Properties.Settings.Default.CacheFullPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CacheFullPath);
            }
            
            if (Properties.Settings.Default.OperatingMode < 4)
            {
                if ((Properties.Settings.Default.TopLat < Properties.Settings.Default.BottomLat) || (Properties.Settings.Default.LeftLong > Properties.Settings.Default.RightLong))
                {
                    ShowErrorBox(Properties.Resources.ExceptionLatLongIncorrect);
                }
                else if ((Math.Abs(Properties.Settings.Default.TopLat) > GMapTile.AbsLatMax) ||
                         (Math.Abs(Properties.Settings.Default.BottomLat) > GMapTile.AbsLatMax) ||
                         (Math.Abs(Properties.Settings.Default.LeftLong) > GMapTile.AbsLngMax) ||
                         (Math.Abs(Properties.Settings.Default.RightLong) > GMapTile.AbsLngMax))
                {
                    ShowErrorBox(Properties.Resources.ExceptionLatLongOutside);
                }
                else
                {
                    GMapTile TopLeft = GMapTile.GetTileFromLatLongZoom(Properties.Settings.Default.TopLat, Properties.Settings.Default.LeftLong, Properties.Settings.Default.ZoomLevel);
                    GMapTile BottomRight = GMapTile.GetTileFromLatLongZoom(Properties.Settings.Default.BottomLat, Properties.Settings.Default.RightLong, Properties.Settings.Default.ZoomLevel);

                    int wh = (int)Math.Sqrt(Properties.Settings.Default.NumberOfSlices);
                    //gkl changes - added variables for clarity
                    int x1, y1, x2, y2;
                    string s1 = wh == 1 ? "" : Path.Combine(Path.GetPathRoot(saveImageFileDialog.FileName), Path.GetDirectoryName(saveImageFileDialog.FileName));
                    string szFormat = wh == 1 ? "" : Path.GetFileNameWithoutExtension(saveImageFileDialog.FileName) + "_{0}-{1}" + Path.GetExtension(saveImageFileDialog.FileName);

                    for (int i = 0; i < wh && !((pw != null) && pw.Cancelled); i++)
                    {
                        for (int j = 0; j < wh && !((pw != null) && pw.Cancelled); j++)
                        {
                            //gkl changes - added variables for clarity
                            x1 = TopLeft.X + ((BottomRight.X - TopLeft.X) * i) / wh + ((i == 0) ? 0 : Math.Sign(BottomRight.X - TopLeft.X));
                            y1 = TopLeft.Y + ((BottomRight.Y - TopLeft.Y) * j) / wh + ((j == 0) ? 0 : Math.Sign(BottomRight.Y - TopLeft.Y));
                            x2 = TopLeft.X + ((BottomRight.X - TopLeft.X) * (i + 1)) / wh;
                            y2 = TopLeft.Y + ((BottomRight.Y - TopLeft.Y) * (j + 1)) / wh;

                            //gkl changes - commented and called new method for clarity and reuse
                            #region commented code
                            /*pw = new ProgressWindow(computeMapBackgroundWorker, String.Format(" - {0}/{1}", i * wh + j + 1, wh * wh));
                            WorkerArguments arguments = new WorkerArguments();
                            arguments.TopLeft = new GMapTile(x1, y1, TopLeft.Zoom);
                            arguments.BottomRight = new GMapTile(x2, y2, BottomRight.Zoom);

                            if (wh == 1)
                            {
                                arguments.Filename = saveImageFileDialog.FileName;
                            }
                            else
                            {
                                //gkl changes - changed a bit
                                /*arguments.Filename = Path.Combine(Path.GetPathRoot(saveImageFileDialog.FileName), Path.GetDirectoryName(saveImageFileDialog.FileName));
                                arguments.Filename = Path.Combine(arguments.Filename, String.Format("{0}_{1}-{2}{3}",
                                    Path.GetFileNameWithoutExtension(saveImageFileDialog.FileName),
                                    i + 1, j + 1, 
                                    Path.GetExtension(saveImageFileDialog.FileName)));*
                                arguments.Filename = Path.Combine(s1, string.Format(szFormat, i + 1 , j + 1));
                            }

                            computeMapBackgroundWorker.RunWorkerAsync(arguments);

                            pw.ShowDialog();*/
                            #endregion

                            string phase = String.Format(" - {0}/{1}", i * wh + j + 1, wh * wh);
                            string filename = wh == 1 ? saveImageFileDialog.FileName : 
                                                        Path.Combine(s1, string.Format(szFormat, i + 1 , j + 1));
                            pw.AddPhase(phase, new GMapTile(x1, y1, TopLeft.Zoom), new GMapTile(x2, y2, BottomRight.Zoom), filename);
                        }
                    }
                }
            }//operating mode < 4
            else //operating mode = 4 || 5
            {
                //gkl changes - moved regex out
                Regex r = new Regex(@"(?<z1>\d+)\s*-\s*(?<z2>\d+)\s*:\s*(?<lat1>-?[\d.]*\d+)\s*,\s*(?<lng1>-?[\d.]*\d+)\s*:\s*(?<lat2>-?[\d.]*\d+)\s*,\s*(?<lng2>-?[\d.]*\d+)", RegexOptions.CultureInvariant);

                //MGMaps .map file mode
                uint f = 0;
                foreach (string fileName in openFileDialog.FileNames)
                {
                    f++;

                    if ((pw != null) && pw.Cancelled)
                    {
                        break;
                    }

                    string[] lines = File.ReadAllLines(fileName, Encoding.ASCII);

                    switch (lines[0].Trim())
                    {
                        case "GoogleMap":
                            Properties.Settings.Default.TileType = 0;
                            break;
                        case "GoogleSat":
                            Properties.Settings.Default.TileType = 1;
                            break;
                        case "GoogleHyb":
                            Properties.Settings.Default.TileType = 2;
                            break;
                        case "GoogleTer":
                            Properties.Settings.Default.TileType = 3;
                            break;
                        case "GoogleChina":
                            Properties.Settings.Default.TileType = 4;
                            break;
                        case "MicrosoftMap":
                            Properties.Settings.Default.TileType = 5;
                            break;
                        case "MicrosoftSat":
                            Properties.Settings.Default.TileType = 6;
                            break;
                        case "MicrosoftHyb":
                            Properties.Settings.Default.TileType = 7;
                            break;
                        case "MicrosoftTer":
                            Properties.Settings.Default.TileType = 8;
                            break;
                        case "MicrosoftBrMap":
                            Properties.Settings.Default.TileType = 9;
                            break;
                        case "YahooMap":
                            Properties.Settings.Default.TileType = 10;
                            break;
                        case "YahooSat":
                            Properties.Settings.Default.TileType = 11;
                            break;
                        case "YahooHyb":
                            Properties.Settings.Default.TileType = 12;
                            break;
                        case "YahooInMap":
                            Properties.Settings.Default.TileType = 13;
                            break;
                        case "YahooInHyb":
                            Properties.Settings.Default.TileType = 14;
                            break;
                        case "OpenStreetMap":
                            Properties.Settings.Default.TileType = 15;
                            break;
                        case "OSMARender":
                            Properties.Settings.Default.TileType = 16;
                            break;
                        case "OpenAerialMap":
                            Properties.Settings.Default.TileType = 17;
                            break;
                        case "OpenCycleMap":
                            Properties.Settings.Default.TileType = 18;
                            break;
                        case "BrutMap":
                            Properties.Settings.Default.TileType = 19;
                            break;
                        case "BrutMapAndNav":
                            Properties.Settings.Default.TileType = 20;
                            break;
                        case "BrutMapSat":
                            Properties.Settings.Default.TileType = 21;
                            break;
                        case "BrutMapAndNavSat":
                            Properties.Settings.Default.TileType = 22;
                            break;
                        case "BrutMapHyb":
                            Properties.Settings.Default.TileType = 23;
                            break;
                        case "BrutMapAndNavHyb":
                            Properties.Settings.Default.TileType = 24;
                            break;
                        default:
                            throw new InvalidOperationException(String.Format(Properties.Resources.ExceptionBadMapFile, fileName));
                    }
                    lines[0] = null;

                    //gkl changes - checked for group by zoom
                    Hashtable zoomMap = null;
                    ArrayList listOfLatsLons = null;
                    if (Properties.Settings.Default.GroupByZoom == true)
                    {
                        zoomMap = new Hashtable();
                    }

                    uint l = 0;
                    foreach (string line in lines)
                    {
                        l++;

                        if (String.IsNullOrEmpty(line))
                            continue;

                        if (line.StartsWith("#"))
                            continue;

                        if ((pw != null) && pw.Cancelled)
                            break;

                        //gkl changes - commented and moved out at the top
                        //Match m = Regex.Match(line.Trim(), @"(?<z1>\d+)\s*-\s*(?<z2>\d+)\s*:\s*(?<lat1>-?[\d.]*\d+)\s*,\s*(?<lng1>-?[\d.]*\d+)\s*:\s*(?<lat2>-?[\d.]*\d+)\s*,\s*(?<lng2>-?[\d.]*\d+)", RegexOptions.CultureInvariant);
                        Match m = r.Match(line.Trim());//, RegexOptions.CultureInvariant);
                        uint zMin = uint.Parse(m.Groups["z1"].Value, ciUS.NumberFormat);
                        uint zMax = uint.Parse(m.Groups["z2"].Value, ciUS.NumberFormat);
                        double lat1 = double.Parse(m.Groups["lat1"].Value, ciUS.NumberFormat);
                        double lat2 = double.Parse(m.Groups["lat2"].Value, ciUS.NumberFormat);
                        double lng1 = double.Parse(m.Groups["lng1"].Value, ciUS.NumberFormat);
                        double lng2 = double.Parse(m.Groups["lng2"].Value, ciUS.NumberFormat);

                        if ((lat2 < lat1) || (lng1 > lng2))
                        {
                            ShowErrorBox(Properties.Resources.ExceptionLatLongIncorrect);
                            break;
                        }

                        if ((Math.Abs(lat1) > GMapTile.AbsLatMax) || (Math.Abs(lat2) > GMapTile.AbsLatMax) ||
                            (Math.Abs(lng1) > GMapTile.AbsLngMax) || (Math.Abs(lng2) > GMapTile.AbsLngMax))
                        {
                            ShowErrorBox(Properties.Resources.ExceptionLatLongOutside);
                            break;
                        }

                        for (uint z = zMin; z <= zMax; z++)
                        {
                            if ((pw != null) && pw.Cancelled)
                            {
                                break;
                            }

                            //gkl changes - added this check and else part
                            if (Properties.Settings.Default.GroupByZoom == true)
                            {
                                listOfLatsLons = zoomMap[z] as ArrayList;
                                
                                if (listOfLatsLons == null)
                                {
                                    listOfLatsLons = new ArrayList(lines.Length - 1);
                                    zoomMap[z] = listOfLatsLons;
                                }

                                listOfLatsLons.Add(new LatLongPair(lat1, lng1, lat2, lng2));
                            }
                            else
                            {
                                //gkl changes - commented and called new method for clarity and reuse
                                #region commented code
                                /*pw = new ProgressWindow(computeMapBackgroundWorker, String.Format(" - File {0}, Zoom {1}, Line {2}/{3}", f, z, l - 1, lines.Length - 1));

                                WorkerArguments arguments = new WorkerArguments();
                                arguments.TopLeft = GMapTile.GetTileFromLatLongZoom(lat2, lng1, z);
                                arguments.BottomRight = GMapTile.GetTileFromLatLongZoom(lat1, lng2, z);
                                arguments.Filename = null;

                                computeMapBackgroundWorker.RunWorkerAsync(arguments);

                                pw.ShowDialog();*/
                                #endregion
                                
                                string phase = String.Format(" - File {0}, Zoom {1}, Line {2}/{3}", f, z, l - 1, lines.Length - 1);
                                pw.AddPhase(phase, GMapTile.GetTileFromLatLongZoom(lat2, lng1, z),
                                                   GMapTile.GetTileFromLatLongZoom(lat1, lng2, z), null);
                            }
                        }//foreach zoom
                    }//foreach line

                    //gkl changes - added this code
                    if (Properties.Settings.Default.GroupByZoom == true)
                    {
                        foreach(DictionaryEntry de in zoomMap)
                        {
                            if ((pw != null) && pw.Cancelled)
                            {
                                break;
                            }
                            
                            uint z = (uint)de.Key;
                            listOfLatsLons = (ArrayList)de.Value;
                            l = 0;
                            foreach (LatLongPair latlong in listOfLatsLons)
                            {
                                if ((pw != null) && pw.Cancelled)
                                {
                                    break;
                                }
                                
                                l++;
                                double lat1 = latlong.TopLat;
                                double lat2 = latlong.BottomLat;
                                double lng1 = latlong.LeftLong;
                                double lng2 = latlong.RightLong;

                                string phase = String.Format(" - File {0}, Zoom {1}, Line {2}/{3}", f, z, l, listOfLatsLons.Count);
                                pw.AddPhase(phase, GMapTile.GetTileFromLatLongZoom(lat2, lng1, z),
                                                   GMapTile.GetTileFromLatLongZoom(lat1, lng2, z), null);
                            }
                        }//for all zoom
                    }//if group by zoom
                }//foreach file
            }//operating mode = 4

            //gkl changes - start downloads and show the dialog finally after all phases added to queue
            pw.StartDownloads();
            pw.ShowDialog();

            Cursor.Current = Cursors.Default;
            saveButton.Enabled = true;
        }

        private void tlLatTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tlLatTextBox, "");
        }

        private void tlLatTextBox_Validating(object sender, CancelEventArgs e)
        {
            double d;
            if (!GMapTile.ParseLat(tlLatTextBox.Text, out d))
            {
                e.Cancel = true;
                tlLatTextBox.Select(0, tlLatTextBox.Text.Length);
                errorProvider.SetError(tlLatTextBox, resources.GetString("tlLatTextBox.Error"));
            }
            else
            {
                Properties.Settings.Default.TopLat = d;
                updateStatus(sender, e);
            }
        }

        private void tlLongTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tlLongTextBox, "");
        }

        private void tlLongTextBox_Validating(object sender, CancelEventArgs e)
        {
            double d;
            if (!GMapTile.ParseLong(tlLongTextBox.Text, out d))
            {
                e.Cancel = true;
                tlLongTextBox.Select(0, tlLongTextBox.Text.Length);
                errorProvider.SetError(tlLongTextBox, resources.GetString("tlLatTextBox.Error"));
            }
            else
            {
                Properties.Settings.Default.LeftLong = d;
                updateStatus(sender, e);
            }
        }

        private void brLatTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(brLatTextBox, "");
        }

        private void brLatTextBox_Validating(object sender, CancelEventArgs e)
        {
            double d;
            if (!GMapTile.ParseLat(brLatTextBox.Text, out d))
            {
                e.Cancel = true;
                brLatTextBox.Select(0, brLatTextBox.Text.Length);
                errorProvider.SetError(brLatTextBox, resources.GetString("tlLatTextBox.Error"));
            }
            else
            {
                Properties.Settings.Default.BottomLat = d;
                updateStatus(sender, e);
            }
        }

        private void brLongTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(brLongTextBox, "");
        }

        private void brLongTextBox_Validating(object sender, CancelEventArgs e)
        {
            double d;
            if (!GMapTile.ParseLong(brLongTextBox.Text, out d))
            {
                e.Cancel = true;
                brLongTextBox.Select(0, brLongTextBox.Text.Length);
                errorProvider.SetError(brLongTextBox, resources.GetString("tlLatTextBox.Error"));
            }
            else
            {
                Properties.Settings.Default.RightLong = d;
                updateStatus(sender, e);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void googleMVtextBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.CacheFullPath = googleMVtextBox.Text;
        }

        private void zoomLevelComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ZoomLevel = Convert.ToUInt32(zoomLevelComboBox.SelectedItem);
        }

        private void grayscalCheckBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.GrayScale = grayscaleCheckBox.Checked;
            grayscalCheckBox_CheckedChanged(sender, e);
        }

        private void imageFormatComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ImageFormat = imageFormatComboBox.Text;
        }

        private void tileTypeComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileType = tileTypeComboBox.SelectedIndex;
            tileTypeComboBox_SelectedIndexChanged(sender, e);
        }

        private void ditherCheckBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.Dither = ditherCheckBox.Checked;
            ditherCheckBox_CheckedChanged(sender, e);
        }

        private void ditherCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ditherCheckBox.Checked)
            {
                grayscaleCheckBox.Checked = false;
            }
            updateStatus(sender, e);
        }

        private void grayscalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (grayscaleCheckBox.Checked)
            {
                ditherCheckBox.Checked = false;
            }
            updateStatus(sender, e);
        }

        private void operatingModeComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.OperatingMode = operatingModeComboBox.SelectedIndex;
            operatingModeComboBox_SelectedIndexChanged(sender, e);
        }

        private void operatingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //gkl changes - added new controls states
            switch (operatingModeComboBox.SelectedIndex)
            {
                default:
                case 1: //download & build image
                case 2: //download, build image & build .map
                    imageGroupBox.Enabled = true;
                    slicingComboBox.Enabled = true;
                    ggZoneGroupBox.Enabled = true;
                    statusLabel.Enabled = true;
                    groupByZoomCheckBox.Enabled = false;
                    hashSizeComboBox.Enabled = false;
                    tilesPerFileComboBox.Enabled = false;
                    break;
                case 0: //download only
                    slicingComboBox.Enabled = false;
                    imageGroupBox.Enabled = false;
                    statusLabel.Enabled = false;
                    ggZoneGroupBox.Enabled = true;
                    groupByZoomCheckBox.Enabled = false;
                    hashSizeComboBox.Enabled = false;
                    tilesPerFileComboBox.Enabled = false;
                    break;
                case 3: //MGMaps mode, download only
                    slicingComboBox.Enabled = false;
                    imageGroupBox.Enabled = false;
                    statusLabel.Enabled = false;
                    ggZoneGroupBox.Enabled = true;
                    groupByZoomCheckBox.Enabled = false;
                    hashSizeComboBox.Enabled = true;
                    enableDisableTiles();
                    break;
                case 4: //MGMaps mode, download from .map
                case 5: //BrutMaps mode, download from .map
                case 6: //BrutMapsAndNav mode, download from .map
                    slicingComboBox.Enabled = false;
                    imageGroupBox.Enabled = false;
                    ggZoneGroupBox.Enabled = false;
                    statusLabel.Enabled = false;
                    //gkl changes - added new controls states
                    groupByZoomCheckBox.Enabled = true;
                    hashSizeComboBox.Enabled = true;
                    enableDisableTiles();
                    break;
            }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void updateStatus(object sender, EventArgs e)
        {
            if ((Properties.Settings.Default.BottomLat > Properties.Settings.Default.TopLat) || (Properties.Settings.Default.RightLong < Properties.Settings.Default.LeftLong)
                || (slicingComboBox.SelectedIndex < 0) || (Properties.Settings.Default.OperatingMode == 0) || (Properties.Settings.Default.OperatingMode > 2))
            {
                statusLabel.Text = String.Format(resources.GetString("statusLabel.Text"), "??", "??", "??", "??", "");
            }
            else
            {
                uint zoom = uint.Parse(zoomLevelComboBox.Text);
                GMapTile TopLeft = GMapTile.GetTileFromLatLongZoom(Properties.Settings.Default.TopLat, Properties.Settings.Default.LeftLong, zoom);
                GMapTile BottomRight = GMapTile.GetTileFromLatLongZoom(Properties.Settings.Default.BottomLat, Properties.Settings.Default.RightLong, zoom);

                int nSlice = (int)((slicingComboBox.SelectedIndex + 1) * (slicingComboBox.SelectedIndex + 1));
                long width = (Math.Abs(TopLeft.X - BottomRight.X) + 1) * GMapTile.TILE_SIZE;
                long height = (Math.Abs(TopLeft.Y - BottomRight.Y) + 1) * GMapTile.TILE_SIZE;
                long memorySizePixel = width * height / nSlice;
                long memoryNeeded = 24 * 1024 * 1024 + memorySizePixel * (grayscaleCheckBox.Checked ? 1 : 3) * (ditherCheckBox.Checked ? 2 : 1);

                statusLabel.Text = String.Format(resources.GetString("statusLabel.Text"), width, height, memorySizePixel / 1000000, memoryNeeded / (1024 * 1024), (nSlice != 1) ? String.Format(" X{0}", nSlice) : "");
            }
        }

        private void proxyUseCheckBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseProxy = proxyUseCheckBox.Checked;
            proxyUseCheckBox_CheckedChanged(sender, e);
        }

        private void proxyUseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (proxyUseCheckBox.Checked)
            {
                waitRequestsCheckBox.Checked = true;
                delayNumericUpDown.Value = 150;
                waitRequestsCheckBox.Enabled = false;
                proxyListUrlsTextBox.Enabled = true;
                proxyListRegexpTextBox.Enabled = true;
            }
            else
            {
                waitRequestsCheckBox.Enabled = true;
                proxyListUrlsTextBox.Enabled = false;
                proxyListRegexpTextBox.Enabled = false;
            }
            waitRequestsCheckBox_CheckedChanged(sender, e);
        }

        private void waitRequestsCheckBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.SlowDown = waitRequestsCheckBox.Checked;
            waitRequestsCheckBox_CheckedChanged(sender, e);
        }

        private void waitRequestsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            delayNumericUpDown.Enabled = waitRequestsCheckBox.Checked && waitRequestsCheckBox.Enabled;
            delayLabel.Enabled = waitRequestsCheckBox.Checked && waitRequestsCheckBox.Enabled;
        }

        private void delayNumericUpDown_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.SlowDownDelay = (uint)delayNumericUpDown.Value;
        }

        private void proxyListUrlsTextBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProxyListURLs = proxyListUrlsTextBox.Text;
            //errorProvider.SetError(proxyListUrlsTextBox, "");
        }

        private void proxyListUrlsTextBox_Validating(object sender, CancelEventArgs e)
        {
            /*try
            {
              TileDownloader.GetProxListFromURLs(proxyListUrlsTextBox.Text, null);
            }
            catch (Exception ex)
            {
              e.Cancel = true;
              proxyListUrlsTextBox.Select(0, proxyListUrlsTextBox.Text.Length);
              errorProvider.SetError(proxyListUrlsTextBox, resources.GetString("proxyListUrlsTextBox.Error") + "\n" + ex.Message);
            }*/
        }

        private void transpComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrivingMapTransparency = Convert.ToUInt32(transpComboBox.SelectedItem.ToString().TrimEnd(new char[] { '%' }));
        }

        private void tileTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tileTypeComboBox.SelectedIndex != 2 && tileTypeComboBox.SelectedIndex != 12
                && tileTypeComboBox.SelectedIndex != 14)
            {
                transpComboBox.Enabled = false;
                transpLabel.Enabled = false;
            }
            else
            {
                transpComboBox.Enabled = true;
                transpLabel.Enabled = true;
            }
        }

        private void slicingComboBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.NumberOfSlices = (uint)((slicingComboBox.SelectedIndex + 1) * (slicingComboBox.SelectedIndex + 1));
        }

        private void groupByZoomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.GroupByZoom = groupByZoomCheckBox.Checked;
        }

        private void maxParallelDownloads_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MaxDownloadsInParallel = uint.Parse(maxParallDnldsComboBox.Text);
        }

        private void hashSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MGMapsHashSize = uint.Parse(hashSizeComboBox.Text);
            enableDisableTiles();
        }

        private void enableDisableTiles()
        {
            if (Properties.Settings.Default.MGMapsHashSize == 1)
            {
                tilesPerFileComboBox.Enabled = true;
                tilesPerFileLabel.Enabled = true;
            }
            else
            {
                tilesPerFileComboBox.Enabled = false;
                tilesPerFileLabel.Enabled = false;
                tilesPerFileComboBox.Text = "1";
                Properties.Settings.Default.MGMapsTilesPerFile = 1;
            }
        }

        private void hashSizeComboBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(hashSizeComboBox, "");
        }

        private void hashSizeComboBox_Validating(object sender, CancelEventArgs e)
        {
            uint value = 0;
            try
            {
                value = uint.Parse(hashSizeComboBox.Text);
            }
            catch (Exception)
            {
            }

            if (value <= 0 || value >= 100)
            {
                e.Cancel = true;
                hashSizeComboBox.Select(0, hashSizeComboBox.Text.Length);
                errorProvider.SetError(hashSizeComboBox, "hash size should be between 1 and 100");
            }
            else
            {
                Properties.Settings.Default.MGMapsHashSize = value;
                enableDisableTiles();
                updateStatus(sender, e);
            }
        }

        private void tilesPerFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MGMapsTilesPerFile = uint.Parse(tilesPerFileComboBox.Text);
        }

        private void tilesPerFileComboBox_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tilesPerFileComboBox, "");
        }

        private void tilesPerFileComboBox_Validating(object sender, CancelEventArgs e)
        {
            uint value = 0;
            try
            {
                value = uint.Parse(tilesPerFileComboBox.Text);
            }
            catch (Exception)
            {
            }

            if (value <= 0 || value >= 65536 || (value & (value-1)) != 0)
            {
                e.Cancel = true;
                tilesPerFileComboBox.Select(0, tilesPerFileComboBox.Text.Length);
                errorProvider.SetError(tilesPerFileComboBox, "tiles per file should be a power of two, between 1 and 32768");
            }
            else
            {
                Properties.Settings.Default.MGMapsTilesPerFile = value;
                updateStatus(sender, e);
            }
        }

        private void proxyListRegexpTextBox_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProxyListRegexp = proxyListRegexpTextBox.Text;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            InitUIwithValues();
        }

        private void selectMapArea_Click(object sender, EventArgs e)
        {
            SelectMapArea maparea = new SelectMapArea();

            if (maparea.ShowDialog(this) == DialogResult.OK)
            {
                tlLatTextBox.Text = maparea.TLLat;
                tlLongTextBox.Text = maparea.TLLong;
                brLatTextBox.Text = maparea.BRLat;
                brLongTextBox.Text = maparea.BRLong;
            }
        }
    }
}