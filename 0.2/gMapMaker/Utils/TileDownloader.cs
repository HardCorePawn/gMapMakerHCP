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
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using FreeImageAPI;
using System.Globalization;

namespace gMapMaker
{
    class TileDownloader
    {
        //gkl changes - changed this folder name to configurable via Settings
        readonly string MGMapsCacheFolderName = Properties.Settings.Default.MGMapsCacheFolderName;
        const string MGMapsConfFilename = "cache.conf";
        const string MGMapsConfFileContent = "\ncenter={2},{3},{4},{5}";
        const int BUFSIZE = 4096;

        static int tpfx, tpfy, tpf;

        static string lastProxyUsed = "";
        static int currentProxyRepetition;
        static List<string> proxy_list = new List<string>();
        public static bool needToWriteCenter = false;

        //gkl changes - made this 'rand' static so that it works for multiple instances without giving any problems
        readonly static Random rand = new Random();
        //gkl changes - renamed the delegate and provided a wrapper for calling the delegate to pass in the worker object
        readonly ReportProgressFunction outsidereportFunction;
        readonly BackgroundWorker worker;

        public TileDownloader(ReportProgressFunction reportFunction, BackgroundWorker worker)
        {
            this.outsidereportFunction = reportFunction;
            this.worker = worker;
        }

        public static void setTPF()
        {
            if (!Properties.Settings.Default.MGMapsMode) {
                tpf = 1;
                Properties.Settings.Default.MGMapsTilesPerFile = 1;
                return;
            }

            tpf = (int)Properties.Settings.Default.MGMapsTilesPerFile;
            int tpflog = 0;
            while (tpf > 1)
            {
                tpflog++;
                tpf >>= 1;
            }
            tpf = (int)Properties.Settings.Default.MGMapsTilesPerFile;
            tpfy = tpflog >> 1;
            tpfx = 1 << (tpfy + (tpflog & 1));
            tpfy = 1 << tpfy;
        }

        void UpdateProxyList()
        {
            //gkl changes - added synchronization based upon a name
            bool createdNew = false;
            Mutex synchobj = new Mutex(true, "ProxyListMutex", out createdNew);
            if (synchobj != null)
            {
                if (!createdNew)
                    synchobj.WaitOne();
            }

            //gkl changes - check again since some other thread might have updated the list
            try
            {
                if (proxy_list.Count <= 0)
                {
                    reportFunction("Downloading proxy list", 0);
                    proxy_list.AddRange(Properties.Settings.Default.Proxys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    proxy_list.AddRange(GetProxyListFromURLs(Properties.Settings.Default.ProxyListURLs, worker));
                    Properties.Settings.Default.Proxys = String.Join("|", proxy_list.ToArray());
                }
            }
            finally //always release the mutex
            {
                //gkl changes - added synchronization based upon a name
                if (synchobj != null)
                {
                    synchobj.ReleaseMutex();
                    synchobj.Close();
                }
            }
        }

        public static List<string> GetProxyListFromURLs(string SingleStringProxyListURLs, BackgroundWorker worker)
        {
            List<string> proxylist = new List<string>();
            string[] proxyListURLs = SingleStringProxyListURLs.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (proxyListURLs.Length == 0)
            {
                throw new ArgumentException(Properties.Resources.ExceptionNoProxyList);
            }

            using (WebClient wc = new WebClient())
            {
                wc.Proxy = WebRequest.DefaultWebProxy;
                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                foreach (string proxyListURL in proxyListURLs)
                {
                    if ((worker != null) && worker.CancellationPending)
                    {
                        return proxylist;
                    }

                    if (!proxyListURL.StartsWith("http"))
                    {
                        proxylist.Add(proxyListURL);
                        continue;
                    }

                    Uri proxyURI = new Uri(proxyListURL);

                    wc.Headers[HttpRequestHeader.Referer] = "http://" + proxyURI.Host + "/";
                    wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";

                    int repeatOnError = 0;
                    string webPage = null;
                    while (String.IsNullOrEmpty(webPage))
                    {
                        try
                        {
                            webPage = wc.DownloadString(proxyURI);
                        }
                        catch (WebException)
                        {
                            repeatOnError++;
                            if (repeatOnError == 3)
                            {
                                throw;
                            }
                            Thread.Sleep(500);
                            continue;
                        }
                    }
                    // webPage = Regex.Replace(webPage, @"<[^>]+>", string.Empty);
                    // webPage = Regex.Replace(webPage, Properties.Settings.Default.ProxyListRegexp, string.Empty);

                    // MatchCollection mc = Regex.Matches(webPage, @"[^\d\.:](?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\s?:\s?\d{1,4})[^\d:]", RegexOptions.None);

                    MatchCollection mc = Regex.Matches(webPage, Properties.Settings.Default.ProxyListRegexp, RegexOptions.None);

                    if (mc.Count == 0)
                    {
                        throw new InvalidOperationException(String.Format(Properties.Resources.ExceptionNoProxyUrlFound, proxyListURL));
                    }

                    foreach (Match m in mc)
                    {
                        // proxylist.Add(Regex.Replace(m.Groups["ip"].Value, @"\s+", String.Empty));
                        proxylist.Add(m.Groups["ip"].Value + ':' + m.Groups["port"].Value);
                    }
                }
            }

            return proxylist;
        }

        private bool IsCachedNoMutex(string filepath, GMapTile g)
        {
            // doesn't exist -- return false
            if (!File.Exists(filepath))
                return false;

            FileInfo fi = new FileInfo(filepath);
            // zero length -- delete and return false
            if (fi.Length == 0)
            {
                File.Delete(filepath);
                return false;
            }
            // non-zero length and tpf=1 - return true, exists and is cached
            else if (tpf == 1)
                return true;

            // tpf>1, file exists
            byte dx = (byte)(g.X % tpfx);
            byte dy = (byte)(g.Y % tpfy);
            FileStream f = null;
            try
            {
                f = File.Open(filepath, FileMode.Open, FileAccess.Read);
                byte[] buf = new byte[6 * tpf + 2];
                f.Read(buf, 0, 6 * tpf + 2);
                int num = (((int)buf[0]) << 8) + (int)buf[1];
                for (int i = 0; i < num; i++)
                    if (buf[2 + i * 6] == dx && buf[2 + i * 6 + 1] == dy)
                        return true;
            }
            finally
            {
                if (f != null)
                    try
                    {
                        f.Close();
                    }
                    catch (Exception) { }
            }

            return false;
        }

        public bool IsCached(string filepath, GMapTile g)
        {
            //gkl changes - added synchronization based upon filename
            bool createdNew = false;
            Mutex synchobj = new Mutex(true, Regex.Replace(filepath, "\\\\", "_"), out createdNew);
            if (synchobj != null)
            {
                if (!createdNew)
                    synchobj.WaitOne();

                try
                {
                    bool result = IsCachedNoMutex(filepath, g);
                    return result;
                }
                finally
                {
                    synchobj.ReleaseMutex();
                    synchobj.Close();
                }
            }

            return false;
        }

        public void DownloadImage(string url, string filepath, bool useProxy)
        {
            Console.WriteLine(url);
            bool error_downloading;
            int download_try = 0;

            // stay synchronized on this mutex
            bool createdNew = false;
            Mutex synchobj = new Mutex(true, Regex.Replace(filepath, "\\\\", "_"), out createdNew);
            if (synchobj != null)
            {
                if (!createdNew)
                    synchobj.WaitOne();

                try
                {
                    // if tiles per file != 1, we're saving to a tempfile, delete it first
                    if (tpf != 1)
                        File.Delete(filepath);
                    if (useProxy && (proxy_list.Count == 0))
                        UpdateProxyList();

                    using (WebClient wc = new WebClient())
                    {
                        while (true)
                        {
                            if (worker.CancellationPending) return;

                            // wc.Headers[HttpRequestHeader.UserAgent] = "Nokia6600/1.0 (4.03.24) SymbianOS/6.1 Series60/2.0 Profile/MIDP-2.0 Configuration/CLDC-1.0";
                            wc.Headers[HttpRequestHeader.CacheControl] = "No-Transform";
                            wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
                            // wc.Headers[HttpRequestHeader.Referer] = Properties.Settings.Default.GoogleReferrer;

                            if (useProxy && !String.IsNullOrEmpty(Properties.Settings.Default.ProxyListURLs))
                            {
                                if (proxy_list.Count == 0)
                                {
                                    if (Properties.Settings.Default.HaltOnProxyListExhausted)
                                    {
                                        throw new InvalidOperationException(Properties.Resources.ExceptionProxyListExhausted);
                                    }
                                    else
                                    {
                                        UpdateProxyList();
                                    }
                                }
                                if (String.IsNullOrEmpty(lastProxyUsed))
                                {
                                    lastProxyUsed = proxy_list[rand.Next(proxy_list.Count)];
                                    currentProxyRepetition = 0;
                                }
                                wc.Proxy = new WebProxy(lastProxyUsed);
                                currentProxyRepetition++;
                            }
                            else
                            {
                                wc.Proxy = WebRequest.DefaultWebProxy;
                                wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
                            }

                            error_downloading = false;
                            try
                            {
                                download_try++;
                                reportFunction("Downloading " + url + " (try " + download_try + ") " + (useProxy ? " from proxy " + lastProxyUsed : ""), 0);
                                wc.DownloadFile(url, filepath);

                                StreamReader sr = new StreamReader(filepath);
                                string filedata = sr.ReadToEnd();
                                sr.Close();
                                if (filedata.Trim().Substring(0, 5).ToLower().Equals("<html"))
                                {
                                    File.Delete(filepath);
                                    error_downloading = true;
                                }
                            }
                            catch (WebException e)
                            {
                                Debug.WriteLine(String.Format("Proxy {0} ; error: {1} ; message: {2}", lastProxyUsed, e.Message, (e.Response != null ? ((HttpWebResponse)e.Response).StatusCode.ToString() : "n/a")));
                                if ((e.Response != null) && (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound))
                                {
                                    //image does not exist in GG database
                                    break;
                                }
                                else
                                {
                                    error_downloading = true;
                                }
                            }

                            if (error_downloading || ((wc.ResponseHeaders[HttpResponseHeader.Server] != null) && wc.ResponseHeaders[HttpResponseHeader.Server].StartsWith("GCS")))
                            {
                                if (!String.IsNullOrEmpty(lastProxyUsed))
                                {
                                    proxy_list.Remove(lastProxyUsed);
                                    Properties.Settings.Default.Proxys = String.Join("|", proxy_list.ToArray());
                                    lastProxyUsed = "";
                                    continue;
                                }
                                else
                                {
                                    throw new InvalidOperationException(Properties.Resources.ExceptionDownloadingImage);
                                }
                            }

                            if (currentProxyRepetition >= 5)
                            {
                                //change every 3 requests to the same proxy, avoid proxy overload
                                lastProxyUsed = "";
                            }

                            if (Properties.Settings.Default.SlowDown)
                            {
                                Thread.Sleep((int)Properties.Settings.Default.SlowDownDelay); //in ms
                            }

                            break;
                        }
                    }
                }

                finally
                {
                    synchobj.ReleaseMutex();
                    synchobj.Close();
                }
            }
        }

        // copy image from temp file to destination file
        public void SaveImage(string tempfile, string src_filename, GMapTile g)
        {
            if (!File.Exists(tempfile))
                return;

            byte dx = (byte)(g.X % tpfx);
            byte dy = (byte)(g.Y % tpfy);

            // mutex to avoid writing simultaneously
            bool createdNew;
            Mutex synchobj = new Mutex(true, Regex.Replace(src_filename, "\\\\", "_"), out createdNew);
            if (synchobj != null)
            {
                if (!createdNew)
                    synchobj.WaitOne();

                try
                {
                    // do nothing if already cached
                    if (IsCachedNoMutex(src_filename, g))
                        return;

                    int num = 0;
                    int ofs = tpf * 6 + 2;
                    byte[] buf = new byte[ofs];
                    bool existed = File.Exists(src_filename);

                    FileStream f = null;
                    FileStream f2 = null;
                    try
                    {
                        f = File.Open(src_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        if (existed)
                        {
                            // read the header
                            f.Read(buf, 0, ofs);
                            num = (((int)buf[0]) << 8) + (int)buf[1];
                            ofs = (((int)buf[2 + num * 6 - 6 + 2]) << 24) +
                                (((int)buf[2 + num * 6 - 6 + 3]) << 16) +
                                (((int)buf[2 + num * 6 - 6 + 4]) << 8) +
                                (((int)buf[2 + num * 6 - 6 + 5]));
                        }
                        else
                        {
                            // write the header
                            f.Write(buf, 0, ofs);
                        }

                        // seek to ofs
                        f.Seek(ofs, SeekOrigin.Begin);

                        FileInfo fi = new FileInfo(tempfile);
                        int totalsize = (int) fi.Length;

                        // copy data
                        f2 = File.Open(tempfile, FileMode.Open, FileAccess.Read);
                        byte[] databuf = new byte[BUFSIZE];
                        for (int sz = 0; sz < totalsize; )
                        {
                            int cnt = Math.Min(BUFSIZE, totalsize - sz);
                            f2.Read(databuf, 0, cnt);
                            f.Write(databuf, 0, cnt);
                            sz += cnt;
                        }
                        f2.Close();
                        f2 = null;
                        totalsize += ofs;

                        // seek to beginning to write the new header
                        buf[0] = (byte)((num + 1) >> 8);
                        buf[1] = (byte)((num + 1) & 0xFF);
                        f.Seek(0, SeekOrigin.Begin);
                        f.Write(buf, 0, 2);

                        // new tile data
                        buf[0] = dx;
                        buf[1] = dy;
                        buf[2] = (byte) (totalsize >> 24);
                        buf[3] = (byte)((totalsize>>16)&0xFF);
                        buf[4] = (byte)((totalsize >> 8) & 0xFF);
                        buf[5] = (byte)(totalsize & 0xFF);
                        f.Seek(6 * num + 2, SeekOrigin.Begin);
                        f.Write(buf, 0, 6);

                        // delete temp file
                        File.Delete(tempfile);
                    }
                    finally
                    {
                        if (f != null)
                            try
                            {
                                f.Close();
                            }
                            catch (Exception) { }

                        if (f2 != null)
                            try
                            {
                                f2.Close();
                            }
                            catch (Exception) { }
                    }
                }
                finally
                {
                    synchobj.ReleaseMutex();
                    synchobj.Close();
                }
            }
        }

        public string BuildFileName(string tileType, GMapTile g)
        {
            string src_filename = Properties.Settings.Default.CacheFullPath;
            string prefix_MGMaps = "", prefix_file="", extension_file="";
            if (g.Zoom > 22) return "";

            if (Properties.Settings.Default.MGMapsMode && !src_filename.EndsWith(MGMapsCacheFolderName))
            {
                src_filename = Path.Combine(src_filename, MGMapsCacheFolderName);
            }

            switch (tileType)
            {
                default:
                case "GoogleSat":
                case "GoogleSatH":
                    prefix_MGMaps = "GoogleSat_";
                    prefix_file = "kh";
                    extension_file = ".jpg";
                    break;
                case "GoogleMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "mt";
                    extension_file = ".png";
                    break;
                case "GoogleHyb":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "tt";
                    extension_file = ".png";
                    break;
                case "GoogleTer":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "gt";
                    extension_file = ".jpg";
                    break;
                case "GoogleChina":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "gc";
                    extension_file = ".png";
                    break;
                case "MicrosoftMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "mm";
                    extension_file = ".png";
                    break;
                case "MicrosoftSat":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "ms";
                    extension_file = ".jpg";
                    break;
                case "MicrosoftHyb":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "mh";
                    extension_file = ".png";
                    break;
                case "MicrosoftTer":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "mr";
                    extension_file = ".png";
                    break;
                case "MicrosoftBrMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "mb";
                    extension_file = ".png";
                    break;
                case "YahooMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "ym";
                    extension_file = ".jpg";
                    break;
                case "YahooSat":
                case "YahooSatH":
                case "YahooSatH2":
                    prefix_MGMaps = "YahooSat_";
                    prefix_file = "ys";
                    extension_file = ".jpg";
                    break;
                case "YahooHyb":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "yh";
                    extension_file = ".png";
                    break;
                case "YahooInMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "yi";
                    extension_file = ".jpg";
                    break;
                case "YahooInHyb":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "yy";
                    extension_file = ".png";
                    break;
                case "OpenStreetMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "om";
                    extension_file = ".png";
                    break;
                case "OSMARender":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "os";
                    extension_file = ".png";
                    break;
                case "OpenAerialMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "oa";
                    extension_file = ".jpg";
                    break;
                case "OpenCycleMap":
                    prefix_MGMaps = tileType + "_";
                    prefix_file = "oc";
                    extension_file = ".png";
                    break;
                case "BrutMap":
                case "BrutMapSat":
                case "BrutMapSatH":
                case "BrutMapSH":
                    prefix_MGMaps = "";
                    prefix_file = "";
                    extension_file = "";
                    break;
                case "BrutMapAndNav":
                case "BrutMapAndNavSat":
                case "BrutMapAndNavSatH":
                case "BrutMapAndNavSH": 
                    prefix_MGMaps = "";
                    prefix_file = "";
                    extension_file = ".png.andnav";
                    break;
                case "BrutMapHyb":
                case "BrutMapAndNavHyb":
                    prefix_MGMaps = "";
                    prefix_file = "";
                    extension_file = ".png.andnav";
                    break;
            }

            if (Properties.Settings.Default.MGMapsMode)
            {
                if (tileType.Equals("BrutMap"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMaps"); 
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom + 1).ToString());
                }
                else if (tileType.Equals("BrutMapSat") || tileType.Equals("BrutMapSatH"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsSat");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom + 1).ToString());
                }
                else if (tileType.Equals("BrutMapSH"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsSH");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom + 1).ToString());
                }
                else if (tileType.Equals("BrutMapHyb"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsHyb");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom + 1).ToString());
                }
                else if (tileType.Equals("BrutMapAndNav"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsAndNav");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom).ToString());
                }
                else if (tileType.Equals("BrutMapAndNavSat") || tileType.Equals("BrutMapAndNavSatH") )
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsAndNavSat");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom).ToString());
                } else if (tileType.Equals("BrutMapAndNavSH")) 
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsAndNavSH");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom).ToString());
                }
                else if (tileType.Equals("BrutMapAndNavHyb"))
                {
                    src_filename = Path.Combine(src_filename, "BrutMapsAndNavHyb");
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + (g.Zoom).ToString());
                }
                else
                {
                    src_filename = Path.Combine(src_filename, prefix_MGMaps + g.Zoom.ToString());
                }
                if (Properties.Settings.Default.MGMapsHashSize != 1)
                    src_filename = Path.Combine(src_filename, ((g.X * 256 + g.Y) % Properties.Settings.Default.MGMapsHashSize).ToString());
                if (Properties.Settings.Default.MGMapsTilesPerFile == 1)
                    if (tileType.Equals("BrutMap") || tileType.Equals("BrutMapSat") || tileType.Equals("BrutMapSatH") || tileType.Equals("BrutMapHyb") || tileType.Equals("BrutMapSH"))
                    {
                        src_filename = Path.Combine(src_filename, g.Y.ToString());
                        src_filename = Path.Combine(src_filename, String.Format("{0:d}", g.X));
                    }
                    else if (tileType.Equals("BrutMapAndNav") || tileType.Equals("BrutMapAndNavSat") || tileType.Equals("BrutMapAndNavSatH") || tileType.Equals("BrutMapAndNavHyb") || tileType.Equals("BrutMapAndNavSH")) 
                    {
                        src_filename = Path.Combine(src_filename, String.Format("{0:d}", g.X)); 
                        src_filename = Path.Combine(src_filename, g.Y.ToString() + extension_file);
                    }
                    else {
                        src_filename = Path.Combine(src_filename, String.Format("{0:d}_{1:d}.mgm", g.X, g.Y));
                    }
                else
                    src_filename = Path.Combine(src_filename, String.Format("{0:d}_{1:d}.mgm", g.X / tpfx, g.Y / tpfy));
            }
            else
            {
                src_filename = Path.Combine(src_filename, prefix_file);
                src_filename = Path.Combine(src_filename, (g.Zoom + 1).ToString());
                src_filename = Path.Combine(src_filename, g.Quadtree + extension_file);
            }

            string downloadedFilePath = Path.GetDirectoryName(src_filename);
            if (!Directory.Exists(downloadedFilePath))
            {
                Directory.CreateDirectory(downloadedFilePath);
            }

            //check if conf file exists in (MGMaps) cache folder
            string MGMapsConfFileFullPath;
            if (Properties.Settings.Default.MGMapsHashSize == 1)
                MGMapsConfFileFullPath = Path.Combine(downloadedFilePath, "../" + MGMapsConfFilename);
            else
                MGMapsConfFileFullPath = Path.Combine(downloadedFilePath, "../../" + MGMapsConfFilename);
            if (Properties.Settings.Default.MGMapsMode && needToWriteCenter)
                {
                    needToWriteCenter = false;
                    //creates MGMaps cache .conf file
                    double lat = (g.TopLat + g.BottomLat) / 2;
                    double lng = (g.RightLong + g.LeftLong) / 2;
                    File.AppendAllText(MGMapsConfFileFullPath, String.Format(MGMapsConfFileContent, Properties.Settings.Default.MGMapsTilesPerFile, Properties.Settings.Default.MGMapsHashSize, lat.ToString("0.000000", CultureInfo.InvariantCulture), lng.ToString("0.000000", CultureInfo.InvariantCulture), (g.Zoom < 1 ? 1 : g.Zoom),
                        (tileType == "GoogleSatH" ? "GoogleHyb" :
                        tileType == "YahooSatH" ? "YahooHyb" :
                        tileType == "YahooSatH2" ? "YahooInHyb" :
                        tileType == "BrutMapSatH" ? "BrutMapHyb" :
                        tileType == "BrutMapAndNavSatH" ? "BrutMapAndNavHyb" :
                        tileType)), Encoding.ASCII);
                }

            return src_filename;
        }

        public string BuildURL(string tileType, GMapTile g, out bool useProxy)
        {
            useProxy = false;
            string url = "";
            // Edited by Shustrik - compacted the switch statement
            string quadcode;
            switch (tileType)
            {
                case "GoogleSat":
                case "GoogleSatH":
                case "BrutMapSat":
                case "BrutMapAndNavSat":
                case "BrutMapSatH":
                case "BrutMapAndNavSatH":
                    url = Properties.Settings.Default.GoogleSatURL;
                    break;
                case "GoogleMap":
                case "BrutMap":
                case "BrutMapAndNav":
                    url = Properties.Settings.Default.GoogleMapURL;
                    break;
                case "GoogleHyb":
                case "BrutMapHyb":
                case "BrutMapAndNavHyb":
                    url = Properties.Settings.Default.GoogleHybURL;
                    break;
                case "GoogleTer":
                    url = Properties.Settings.Default.GoogleTerURL;
                    break;
                case "GoogleChina":
                    url = Properties.Settings.Default.GoogleChinaURL;
                    break;
                case "MicrosoftMap":
                    url = Properties.Settings.Default.MicrosoftMapURL;
                    break;
                case "MicrosoftSat":
                    url = Properties.Settings.Default.MicrosoftSatURL;
                    break;
                case "MicrosoftHyb":
                    url = Properties.Settings.Default.MicrosoftHybURL;
                    break;
                case "MicrosoftTer":
                    url = Properties.Settings.Default.MicrosoftTerURL;
                    break;
                case "MicrosoftBrMap":
                    if (g.Zoom <= 10)
                        url = Properties.Settings.Default.MicrosoftMapURL;
                    else
                        url = Properties.Settings.Default.MicrosoftBrMapURL;
                    break;
                case "YahooMap":
                    url = Properties.Settings.Default.YahooMapURL;
                    break;
                case "YahooSat":
                case "YahooSatH":
                case "YahooSatH2":
                    url = Properties.Settings.Default.YahooSatURL;
                    break;
                case "YahooHyb":
                    url = Properties.Settings.Default.YahooHybURL;
                    break;
                case "YahooInMap":
                    url = Properties.Settings.Default.YahooInMapURL;
                    break;
                case "YahooInHyb":
                    url = Properties.Settings.Default.YahooInHybURL;
                    break;
                case "OpenStreetMap":
                    url = Properties.Settings.Default.OpenStreetMapURL;
                    break;
                case "OSMARender":
                    url = Properties.Settings.Default.OSMARenderURL;
                    break;
                case "OpenAerialMap":
                    url = Properties.Settings.Default.OpenAerialMapURL;
                    break;
                case "OpenCycleMap":
                    url = Properties.Settings.Default.OpenCycleMapURL;
                    break;
            }

            useProxy = Properties.Settings.Default.UseProxy &&
                (Properties.Settings.Default.UseProxyForAllMapTypes || url == Properties.Settings.Default.GoogleSatURL);

            // Edited by Shustrik - added variables for configuration settings other than Google
            quadcode = "";
            for (int i = (int)g.Zoom - 1; i >= 0; i--)
                quadcode = quadcode + (((((g.Y >> i) & 1) << 1) + ((g.X >> i) & 1)));

            url = url.Replace("{X}", g.X.ToString());
            url = url.Replace("{Y}", g.Y.ToString());
            url = url.Replace("{Z}", ((int)g.Zoom).ToString());
            url = url.Replace("{ZOOM}", ((int)g.Zoom).ToString());
            url = url.Replace("{QUAD}", quadcode);

            url = url.Replace("{YAHOO_Y}", (((1 << ((int)g.Zoom)) >> 1)-1-g.Y).ToString());
            url = url.Replace("{YAHOO_ZOOM}", ((int)g.Zoom + 1).ToString());
            url = url.Replace("{YAHOO_ZOOM_2}", (17 - (int)g.Zoom + 1).ToString());
            url = url.Replace("{OAM_ZOOM}", (17 - (int)g.Zoom).ToString());

            url = url.Replace("{GOOG_DIGIT}", ((g.X+g.Y) & 3).ToString());
            url = url.Replace("{GOOG_QUAD}", g.Quadtree);
            url = url.Replace("{MS_DIGITBR}", ((((g.Y & 1) << 1) + (g.X & 1)) + 1).ToString());
            url = url.Replace("{MS_DIGIT}", ((((g.Y & 3) << 1) + (g.X & 1))).ToString());
            url = url.Replace("{Y_DIGIT}", rand.Next(1, 3).ToString());

            url = url.Replace("{GALILEO}", "Galileo".Substring(0, ((3 * g.X + g.Y) & 7)));

            // support old style {} vars 
            url = url.Replace("QQQQ", g.Quadtree);
            url = url.Replace("XXXX", g.X.ToString());
            url = url.Replace("YYYY", g.Y.ToString());
            url = url.Replace("ZZZZ", (17 - (int)g.Zoom).ToString());
            url = url.Replace("{OSM_ZOOM}", ((int)g.Zoom).ToString());
            url = url.Replace("{MS_QUADCODE}", quadcode);
            url = url.Replace("*", rand.Next(4).ToString());

            return url;
        }

        private void reportFunction(string message, double inc)
        {
            outsidereportFunction(worker, message, inc);
        }
    }
}
