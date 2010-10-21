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
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace gMapMaker
{
    public partial class ProgressWindow : Form
    {
        readonly string MGMapsCacheFolderName = Properties.Settings.Default.MGMapsCacheFolderName;
        const string MGMapsConfFilename = "cache.conf";
        const string MGMapsConfFileContent = "version=3\ntiles_per_file={0}\nhash_size={1}";
        const string MGMapsConfFileContent2 = "version=3\ntiles_per_file={0}\nhash_size={1}\ncenter={2}";

        Hashtable mapWorkerToProgressBar;
        ProgressControl progressctrl;
        BackgroundWorker worker;

        bool cancelled = false;
        public bool allCancelled;
        public bool closingAsked;
        public bool paused;
        private int numPhases;
        private int numCompleted;
        Queue queueOfPhases = new Queue();
        Queue syncQueue;
        int nMaxParallelJobs;
        bool cancelTimeout = false;
        int cancelTimeoutInterval = 5000;
        System.Windows.Forms.Timer cancelTimer = new System.Windows.Forms.Timer();
        
        public ProgressWindow(int nMaxParallel)
        {
            paused = false;
            numPhases = 0;
            numCompleted = 0;
            allCancelled = false;

            InitializeComponent();

            pauseButton.Enabled = true;
            syncQueue = Queue.Synchronized(queueOfPhases);
            nMaxParallelJobs = nMaxParallel;
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
            cancelTimer.Interval = cancelTimeoutInterval;
            cancelTimer.Tick += new EventHandler(cancelTimer_Tick);
            //force the window to be created
            ErrorBox.GetInstance();
        }

        void cancelTimer_Tick(object sender, EventArgs e)
        {
            cancelTimeout = true;
        }

        private void InitializeWorkers(int nMaxParallel)
        {
            mapWorkerToProgressBar = new Hashtable(nMaxParallel);

            for (int i = 0; i < nMaxParallel; i++)
            {
                worker = new BackgroundWorker();
                progressctrl = new ProgressControl();
                this.layoutPanel.Controls.Add(progressctrl);

                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                
                mapWorkerToProgressBar.Add(worker, progressctrl);
            }

            Rectangle bounds = this.Bounds;
            this.SetBounds(bounds.Left, bounds.Top - panel1.Height / 2, bounds.Width, bounds.Height + panel1.Height);
        }

        public bool Cancelled
        {
            get
            {
                return cancelled;
            }
        }

        #region worker

        class WorkerArguments
        {
            public GMapTile TopLeft;
            public GMapTile BottomRight;
            public string Filename;
            //gkl changes - added this since this is for each phase
            public string Phase;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
#if DEBUG
            TextWriterTraceListener textWriterTraceListener = new TextWriterTraceListener("log.txt");
            textWriterTraceListener.TraceOutputOptions = TraceOptions.DateTime;
            Debug.Listeners.Add(textWriterTraceListener);
#endif

            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            WorkerArguments arg = (WorkerArguments)e.Argument;

            if (arg.TopLeft.Zoom != arg.BottomRight.Zoom)
            {
                throw new ArgumentException();
            }

            //gkl changes - commented this and put this code in saveButton_click, so as to do it only once
            //check if cache folder exists
            /*if (!Directory.Exists(Properties.Settings.Default.CacheFullPath))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CacheFullPath);
            }*/

            BigImage bi = new BigImage(arg.TopLeft.Quadtree, arg.BottomRight.Quadtree, ReportProgressFunction, worker);
            bi.grayscale = Properties.Settings.Default.GrayScale;
            bi.tileTypeInt = Properties.Settings.Default.TileType;
            bi.dither8bpp = Properties.Settings.Default.Dither;

            if ((Properties.Settings.Default.OperatingMode == 0) ||
                (Properties.Settings.Default.OperatingMode == 3) ||
                (Properties.Settings.Default.OperatingMode == 4) ||
                (Properties.Settings.Default.OperatingMode == 5) ||
                (Properties.Settings.Default.OperatingMode == 6))
            {
                bi.imageFormat = FreeImageAPI.FREE_IMAGE_FORMAT.FIF_UNKNOWN;
            }
            else
            {
                switch (Properties.Settings.Default.ImageFormat)
                {
                    case "PNG":
                        bi.imageFormat = FreeImageAPI.FREE_IMAGE_FORMAT.FIF_PNG;
                        bi.saveflags = FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION;
                        break;
                    case "JPEG":
                        bi.imageFormat = FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG;
                        bi.saveflags = FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB;
                        break;
                    case "TIFF":
                        bi.imageFormat = FreeImageAPI.FREE_IMAGE_FORMAT.FIF_TIFF;
                        bi.saveflags = FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.TIFF_PACKBITS;
                        break;
                }
            }
            //gkl changes - added try/catch for InvalidOperationException exceptions thrown from TileDownloader
            try
            {
                bi.BuildBigImage(arg.Filename, this);
            }
            catch (InvalidOperationException ex)
            {
                if (this.Visible)
                {
                    MainForm.ShowErrorBoxEx(ex.Message);
                }
                ReportProgressFunction(worker, "Stopped on error", 0);
                return;
            }

            if (worker.CancellationPending)
            {
                return;
            }

            if (Properties.Settings.Default.OperatingMode == 2)
            {
                ReportProgressFunction(worker, "Saving map", 0);

                OziExplorerMap oem = new OziExplorerMap();
                oem.MapName = Path.GetFileName(arg.Filename);
                oem.ImageFileFullPath = arg.Filename;
                oem.ImageHeight = bi.ImageHeight;
                oem.ImageWidth = bi.ImageWidth;
                oem.OnePixelLength = arg.TopLeft.OnePixelLength;
                oem.ReferencePoints.Add(new MapReferencePoint(0, 0, arg.TopLeft.TopLat, arg.TopLeft.LeftLong));
                oem.ReferencePoints.Add(new MapReferencePoint(bi.ImageWidth, bi.ImageHeight, arg.BottomRight.BottomLat, arg.BottomRight.RightLong));
                oem.ReferencePoints.Add(new MapReferencePoint(0, bi.ImageHeight, arg.BottomRight.BottomLat, arg.TopLeft.LeftLong));
                oem.ReferencePoints.Add(new MapReferencePoint(bi.ImageWidth, 0, arg.TopLeft.TopLat, arg.BottomRight.RightLong));

                int YY = arg.BottomRight.Y - arg.TopLeft.Y;
                int XX = arg.BottomRight.X - arg.TopLeft.X;

                if ((XX > 1) && (YY > 1))
                {
                    double R = Math.Abs(((double)XX + 1.0) / ((double)YY + 1.0));
                    int NY = (int)Math.Sqrt(30.0 / R);
                    int NX = 30 / NY;
                    double stepY = (double)YY / Math.Min(YY, NY - 1);
                    double stepX = (double)XX / Math.Min(XX, NX - 1);

                    int y = 0;
                    double dy = 0;
                    for (; y <= YY; dy += stepY, y = (int)Math.Round(dy))
                    {
                        int x = 0;
                        double dx = 0;
                        for (; x <= XX; dx += stepX, x = (int)Math.Round(dx))
                        {
                            if (((x == 0) && (y == 0)) || ((x == 0) && (y == YY)) || ((x == XX) && (y == 0)) || ((x == XX) && (y == YY)))
                            {
                                continue;
                            }
                            GMapTile g = new GMapTile(arg.TopLeft.X + x, arg.TopLeft.Y + y, arg.TopLeft.Zoom);
                            oem.ReferencePoints.Add(new MapReferencePoint(x * GMapTile.TILE_SIZE, y * GMapTile.TILE_SIZE, g.TopLat, g.LeftLong));
                        }
                    }
                }

                oem.SaveToMap(Path.ChangeExtension(arg.Filename, ".map"));
            }

            if (Properties.Settings.Default.OperatingMode == 3 || Properties.Settings.Default.OperatingMode == 4 || Properties.Settings.Default.OperatingMode == 5 || Properties.Settings.Default.OperatingMode == 6)
                ReportProgressFunction(worker, "Download complete", 0.0);
            else
                ReportProgressFunction(worker, "Map saved", 20.0);
        }

        struct ReportInfo
        {
            public double inc;
            public string status;
            public ReportInfo(double inc_, string status_)
            {
                inc = inc_;
                status = status_;
            }
        }

        private void ReportProgressFunction(BackgroundWorker worker, string message, double inc)
        {
#if DEBUG
            Console.WriteLine("called reportprogress {0} {1}", message, inc);
#endif
            if (Properties.Settings.Default.OperatingMode == 3 || Properties.Settings.Default.OperatingMode == 4 || Properties.Settings.Default.OperatingMode == 5 || Properties.Settings.Default.OperatingMode == 6)
                worker.ReportProgress(0, new ReportInfo(inc / 100.0, message));
            else
                worker.ReportProgress(0, new ReportInfo(inc / 220.0, message));
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReportInfo r = (ReportInfo)e.UserState;
            progressctrl = mapWorkerToProgressBar[(BackgroundWorker)sender] as ProgressControl;
            if (progressctrl != null)
            {
                progressctrl.SetProgressValue(r.inc);
                progressctrl.SetStatusLabel(r.status);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.closingAsked = true;
            numCompleted++;
            // First, handle the case where an exception was thrown.
            if ((e.Error != null) && !e.Cancelled)
            {
                Cursor.Current = Cursors.Default;
                if (this.Visible)
                {
                    MainForm.ShowErrorBoxEx(e.Error.Message);
                }
                //gkl changes - don't pick up next in queue in case of error
                ReportProgressFunction(worker, "Stopped on error", 0);
                return;
            }
            else if (e.Cancelled)
            {
                // cancelButton_Click(null, null);
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
            }
            else if (numCompleted == numPhases)
            {
                // all done
                cancelButton_Click(null, null);
            }
            PickupNextInQueue((BackgroundWorker)sender); 
        }

        internal void AddPhase(string phase, GMapTile topleft, GMapTile bottomright, string filename)
        {
            WorkerArguments arguments = new WorkerArguments();
            arguments.TopLeft = topleft;
            arguments.BottomRight = bottomright;
            arguments.Filename = filename;
            //gkl changes - passed in phase as well
            arguments.Phase = phase;

            syncQueue.Enqueue(arguments);
            numPhases++;
        }

        public void StartDownloads()
        {
            bool needToWrite = true;
            //check if conf file exists in (MGMaps) cache folder
            string MGMapsConfFileFullPath = Properties.Settings.Default.CacheFullPath;
            if (Properties.Settings.Default.MGMapsMode && !MGMapsConfFileFullPath.EndsWith(MGMapsCacheFolderName))
                MGMapsConfFileFullPath = Path.Combine(MGMapsConfFileFullPath, MGMapsCacheFolderName);
            MGMapsConfFileFullPath = Path.Combine(MGMapsConfFileFullPath, MGMapsConfFilename);
            if (Properties.Settings.Default.MGMapsMode)
                if (!File.Exists(MGMapsConfFileFullPath))
                {
                    // create path to .conf file
                    Directory.CreateDirectory(Path.GetDirectoryName(MGMapsConfFileFullPath));
                    // create MGMaps cache .conf file
                    File.WriteAllText(MGMapsConfFileFullPath, String.Format(MGMapsConfFileContent, Properties.Settings.Default.MGMapsTilesPerFile, Properties.Settings.Default.MGMapsHashSize), Encoding.ASCII);
                }
                else
                {
                    // read .conf file and reset app settings
                    uint fTPF = 1;
                    uint fHS = 97;
                    uint fVer = 3;
                    String fCenter = null;
                    String[] lines = File.ReadAllLines(MGMapsConfFileFullPath);
                    foreach (String line in lines)
                    {
                        String[] splits = line.Split('=');
                        if (splits[0].Trim().Equals("tiles_per_file"))
                            fTPF = uint.Parse(splits[1].Trim());
                        else if (splits[0].Trim().Equals("version"))
                            fVer = uint.Parse(splits[1].Trim());
                        else if (splits[0].Trim().Equals("hash_size"))
                            fHS = uint.Parse(splits[1].Trim());
                        else if (splits[0].Trim().Equals("center"))
                            fCenter = splits[1].Trim();
                    }

                    // set tiles per file, hash size
                    if (fTPF > 1)
                        fHS = 1;

                    Properties.Settings.Default.MGMapsTilesPerFile = fTPF;
                    Properties.Settings.Default.MGMapsHashSize = fHS;

                    if (fCenter == null)
                        File.WriteAllText(MGMapsConfFileFullPath, String.Format(MGMapsConfFileContent, Properties.Settings.Default.MGMapsTilesPerFile, Properties.Settings.Default.MGMapsHashSize), Encoding.ASCII);
                    else
                    {
                        File.WriteAllText(MGMapsConfFileFullPath, String.Format(MGMapsConfFileContent2, Properties.Settings.Default.MGMapsTilesPerFile, Properties.Settings.Default.MGMapsHashSize, fCenter), Encoding.ASCII);
                        needToWrite = false;
                    }
                }

            TileDownloader.needToWriteCenter = needToWrite;
            TileDownloader.setTPF();

            //set the bar numbers to be minimum of Max jobs and count of phases in the queue
            InitializeWorkers(Math.Min(nMaxParallelJobs, syncQueue.Count));
            foreach (DictionaryEntry de in mapWorkerToProgressBar)
            {
                if (syncQueue.Count > 0)
                {
                    BackgroundWorker worker = (BackgroundWorker)de.Key;
                    ProgressControl pc = (ProgressControl)de.Value;

                    StartProgressControl(worker, pc);
                }
            }
        }

        private void PickupNextInQueue(BackgroundWorker worker)
        {
            if(mapWorkerToProgressBar.Contains(worker))
            {
                ProgressControl pc = (ProgressControl)mapWorkerToProgressBar[worker];
                StartProgressControl(worker, pc);
            }
        }

        private void StartProgressControl(BackgroundWorker worker, ProgressControl pc)
        {
            if (syncQueue.Count > 0)
            {
                WorkerArguments args = (WorkerArguments)syncQueue.Dequeue();
                if (args != null)
                {
                    pc.Initialize();
                    pc.SetPhaseLabel(args.Phase);
                    worker.RunWorkerAsync(args);
                }
            }
        }
        #endregion

        //gkl changes - changed a lot in functionality of the below functions
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (cancelButton.Text == "Close")
            {
                Close();
                return;
            }
            else if (cancelled)
                return;
            else if (mapWorkerToProgressBar.Count == 0)
                return;
            cancelButton.Text = "Cancelling...";
            cancelButton.Enabled = false;
            allCancelled = true;
            bool oldClosingAsked = closingAsked;
            closingAsked = true;
            if (!oldClosingAsked)
                foreach (BackgroundWorker worker in mapWorkerToProgressBar.Keys)
                {
                    Cancel(worker);
                }
            cancelled = true;
            cancelButton.Enabled = true;
            cancelButton.Text = "Close";
            pauseButton.Enabled = false;
            //dispose all workers since there is no need and user has only option of
            //closing the window
            if (!oldClosingAsked)
                DisposeAllWorkers();
        }
    
        private void Cancel(BackgroundWorker worker)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                cancelTimeout = false;
                cancelTimer.Start();
                while (cancelTimeout == false && worker.IsBusy)
                {
                    Application.DoEvents();
                }
                cancelTimer.Stop();
            }
        }

        private void ProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closingAsked)
            {
                cancelButton_Click(sender, e);
            }

            ErrorBox.ResetAndHideErrors();
        }

        private void DisposeAllWorkers()
        {
            foreach (BackgroundWorker worker in mapWorkerToProgressBar.Keys)
            {
                //dissociate event handlers
                worker.DoWork -= new DoWorkEventHandler(this.worker_DoWork);
                worker.ProgressChanged -= new ProgressChangedEventHandler(this.worker_ProgressChanged);
                worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);

                worker.Dispose();
            }
            mapWorkerToProgressBar.Clear();
        }

        private void ProgressWindow_Load(object sender, EventArgs e)
        {
            this.SetBounds(0, 0, 100, 100);
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            paused = !paused;
            if (paused)
            {
                pauseButton.Text = "Resume";
                cancelButton.Enabled = false;
            }
            else
            {
                pauseButton.Text = "Pause";
                cancelButton.Enabled = true;
            }
        }
    }
}