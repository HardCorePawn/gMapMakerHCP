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
using System.Text.RegularExpressions;
using FreeImageAPI;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

//using FIBITMAP = System.UInt32;

namespace gMapMaker
{
    public delegate void ReportProgressFunction(BackgroundWorker worker, string message, double inc);

    class BigImage
    {
        readonly string topleft;
        readonly string bottomright;
        //gkl changes - renamed the delegate and provided a wrapper for calling the delegate to pass in the worker object
        readonly ReportProgressFunction outsidereportFunction;
        readonly BackgroundWorker worker;
        public bool grayscale;
        public bool dither8bpp;
        public FREE_IMAGE_FORMAT imageFormat = FREE_IMAGE_FORMAT.FIF_PNG;
        public FREE_IMAGE_SAVE_FLAGS saveflags;
        public int tileTypeInt;
        int imageHeight = -1;
        int imageWidth = -1;
        TileDownloader td;

        public int ImageHeight
        {
            get
            {
                return imageHeight;
            }
        }
        public int ImageWidth
        {
            get
            {
                return imageWidth;
            }
        }

        public BigImage(string topleft, string bottomright, ReportProgressFunction reportFunction, BackgroundWorker worker)
        {
            this.topleft = topleft;
            this.bottomright = bottomright;
            //gkl changes - changed the delegate variable
            this.outsidereportFunction = reportFunction;
            this.worker = worker;
        }

        static void FreeImage_OutputMessageFunction(FREE_IMAGE_FORMAT format, string msg)
        {
            Debug.WriteLine("FreeImage: " + msg);
        }

        FIBITMAP LoadImageFromCache(string tileType, GMapTile g)
        {
            FIBITMAP image = new FIBITMAP();
            image.SetNull();

            while (image.IsNull)
            {
                bool useProxy;
                string src_filename = td.BuildFileName(tileType, g);
                if (src_filename != null && src_filename.Length > 0)
                {
                    string url = td.BuildURL(tileType, g, out useProxy);
                    // using temp file if tiles_per_file != 1
                    if (Properties.Settings.Default.MGMapsTilesPerFile == 1)
                    {
                        if (!td.IsCached(src_filename, g))
                            td.DownloadImage(url, src_filename, useProxy);
                    }
                    else
                    {
                        // FIX check if the image exists
                        String tempfile = Path.GetTempFileName();
#if DEBUG
                        Console.WriteLine("Temp file: " + tempfile);
#endif
                        try
                        {
                            if (!td.IsCached(src_filename, g))
                            {
                                td.DownloadImage(url, tempfile, useProxy);
#if DEBUG
                                Console.WriteLine("Downloaded image from {0}", url);
#endif
                                td.SaveImage(tempfile, src_filename, g);
                            }
                        }
                        finally
                        {
                            try
                            {
                                File.Delete(tempfile);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                if (worker.CancellationPending || Properties.Settings.Default.MGMapsTilesPerFile != 1)
                {
                    break;
                }

                //gkl changes - added synchronization based upon filename
                bool createdNew = false;
                Mutex synchobj = new Mutex(true, Regex.Replace(src_filename, "\\\\", "_"), out createdNew);
                if (synchobj != null)
                {
                    try
                    {
                        if (!createdNew)
                            synchobj.WaitOne();
                        if (!File.Exists(src_filename))
                        {
                            //blank image white
                            image = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 24, 0, 0, 0);
                            if (image.IsNull)
                            {
                                throw new InvalidOperationException(Properties.Resources.ExceptionCannotBuildBlankImage);
                            }
                            if (!FreeImage.Invert(image))
                            {
                                throw new InvalidOperationException(Properties.Resources.ExceptionCannotBuildBlankImage);
                            }
                            if (Properties.Settings.Default.SaveBlankImageForMissingTiles)
                            {
                                if (tileType == "GoogleSat" || tileType == "GoogleSatH" ||
                                    tileType == "GoogleTer" || tileType == "YahooMap" ||
                                     tileType == "YahooInMap" || tileType == "YahooSat" ||
                                     tileType == "YahooSatH" || tileType == "YahooSatH2" ||
                                     tileType == "MicrosoftSat" || tileType == "OpenAerialMap" ||
                                    tileType == "BrutMapSat" || tileType == "BrutMapSatH" ||
                                    tileType == "BrutMapAndNavSat" || tileType == "BrutMapAndNavSatH")
                                {
                                    if (!FreeImage.Save(
                                        FREE_IMAGE_FORMAT.FIF_JPEG,
                                        image, src_filename,
                                        FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYSUPERB))
                                    {
                                        throw new InvalidOperationException(Properties.Resources.ExceptionCannotBuildBlankImage);
                                    }
                                }

                                else
                                {
                                    if (!FreeImage.Save(
                                        FREE_IMAGE_FORMAT.FIF_PNG,
                                        image, src_filename,
                                        FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION))
                                    {
                                        throw new InvalidOperationException(Properties.Resources.ExceptionCannotBuildBlankImage);
                                    }
                                }
                            }
                        }
                        else
                        {
                            image = FreeImage.Load(FreeImage.GetFileType(src_filename, 0), src_filename, 0);
                            if (image.IsNull)
                            {
                                File.Delete(src_filename);
                                if (Properties.Settings.Default.HaltOnImageFileCorrupted)
                                {
                                    throw new InvalidOperationException(String.Format(Properties.Resources.ExceptionLoadingImage, src_filename));
                                }
                            }
                        }
                    }
                finally //always release mutex
                    {
                        //gkl changes - added synchronization based upon filename
                            synchobj.ReleaseMutex();
                            synchobj.Close();
                    }
                }
            }

            return image;
        }

        public void BuildBigImage(string filename, ProgressWindow pw)
        {
            //FreeImage.SetOutputMessage(FreeImage_OutputMessageFunction);

            int topleftx, toplefty, bottomrightx, bottomrighty;
            uint topleftzoom, bottomrightzoom;

            GMapTile.XYZoom(topleft, out topleftx, out toplefty, out topleftzoom);
            GMapTile.XYZoom(bottomright, out bottomrightx, out bottomrighty, out bottomrightzoom);

            //gkl changes - passed in new delegate
            td = new TileDownloader(outsidereportFunction, worker);

            FIBITMAP bigImage = new FIBITMAP(), image = new FIBITMAP();

            try
            {
                imageWidth = (bottomrightx - topleftx + 1) * GMapTile.TILE_SIZE;
                imageHeight = (bottomrighty - toplefty + 1) * GMapTile.TILE_SIZE;

                if (imageFormat != FREE_IMAGE_FORMAT.FIF_UNKNOWN)
                {
                    reportFunction("Allocating memory for image", 0);

                    if ((imageWidth > 65535) || (imageHeight > 65535))
                    {
                        throw new InvalidOperationException(Properties.Resources.ExceptionImageMemoryAllocation);
                    }
                    bigImage = FreeImage.Allocate(imageWidth, imageHeight, grayscale ? 8 : 24, 0, 0, 0);
                    if (bigImage.IsNull)
                    {
                        throw new InvalidOperationException(Properties.Resources.ExceptionImageMemoryAllocation);
                    }
                }

                int wholeQuant = (bottomrightx - topleftx + 1) * (bottomrighty - toplefty + 1);
                bool paletteInit = false;

                for (int yy = toplefty; yy <= bottomrighty; yy++)
                {
                    for (int xx = topleftx; xx <= bottomrightx; xx++)
                    {
                        if (worker.CancellationPending)
                        {
                            return;
                        }

                        try
                        {
                            GMapTile g = new GMapTile(xx, yy, topleftzoom);

                            while (pw.paused)
                            {
                                reportFunction("< Paused >", 0);
                                Thread.Sleep(1000);
                            }

                            if (pw.allCancelled)
                            {
                                reportFunction("< Cancelled >", 0);
                                return;
                            }

                            if (Properties.Settings.Default.OperatingMode == 3 || Properties.Settings.Default.OperatingMode == 4 || Properties.Settings.Default.OperatingMode == 5 || Properties.Settings.Default.OperatingMode == 6)
                                reportFunction("(Down)loading image (" + xx + "," + yy + ")", 0);
                            else
                                reportFunction("(Down)loading image " + g.Quadtree, ((!grayscale && dither8bpp ? 0.0 : 50.0) + 100.0) / (double)wholeQuant);

                            FIBITMAP image_alpha;
                            FIBITMAP image_alpha2;
                            FIBITMAP image_result;

                            switch (tileTypeInt)
                            {
                                case 0: //map only
                                    image = LoadImageFromCache("GoogleMap", g);
                                    break;
                                case 1: //satellite only
                                    image = LoadImageFromCache("GoogleSat", g);
                                    break;
                                case 2: //hybrid
                                    image = LoadImageFromCache("GoogleSatH", g);

                                    if (worker.CancellationPending)
                                    {
                                        return;
                                    }

                                    image_alpha = LoadImageFromCache("GoogleHyb", g);

                                    if (FreeImage.GetBPP(image_alpha) != 32)
                                    {
                                        //sometimes alpha tile from GG is a 2bits image, it means tile is blank
                                        break;
                                    }

                                    if (Properties.Settings.Default.DrivingMapTransparency != 100)
                                    {
                                        image_alpha2 = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 32, 0, 0, 0);
                                        if (!FreeImage.Paste(image_alpha2, image_alpha, 0, 0, ((int)Properties.Settings.Default.DrivingMapTransparency * 256) / 100))
                                        {
                                            //sometimes alpha tile from GG is corrupted, it means tile is blank
                                            //we keep tile
                                        }
                                        FreeImage.Unload(image_alpha);
                                        image_alpha = image_alpha2;
                                    }

                                    image_result = FreeImage.Composite(image_alpha, false, null, image);
                                    FreeImage.Unload(image_alpha);
                                    if (!image_result.IsNull)
                                    {
                                        FreeImage.Unload(image);
                                        image = image_result;
                                    }
                                    else
                                    {
                                        //sometimes alpha tile from GG is corrupted, it means tile is blank
                                        //we keep image
                                    }
                                    break;
                                case 3:
                                    image = LoadImageFromCache("GoogleTer", g);
                                    break;
                                case 4:
                                    image = LoadImageFromCache("GoogleChina", g);
                                    break;
                                case 5:
                                    image = LoadImageFromCache("MicrosoftMap", g);
                                    break;
                                case 6:
                                    image = LoadImageFromCache("MicrosoftSat", g);
                                    break;
                                case 7:
                                    image = LoadImageFromCache("MicrosoftHyb", g);
                                    break;
                                case 8:
                                    image = LoadImageFromCache("MicrosoftTer", g);
                                    break;
                                case 9:
                                    image = LoadImageFromCache("MicrosoftBrMap", g);
                                    break;
                                case 10:
                                    image = LoadImageFromCache("YahooMap", g);
                                    break;
                                case 11:
                                    image = LoadImageFromCache("YahooSat", g);
                                    break;
                                case 12:
                                    image = LoadImageFromCache("YahooSatH", g);

                                    if (worker.CancellationPending)
                                    {
                                        return;
                                    }

                                    image_alpha = LoadImageFromCache("YahooHyb", g);

                                    if (FreeImage.GetBPP(image_alpha) != 32)
                                    {
                                        //sometimes alpha tile from GG is a 2bits image, it means tile is blank
                                        break;
                                    }

                                    if (Properties.Settings.Default.DrivingMapTransparency != 100)
                                    {
                                        image_alpha2 = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 32, 0, 0, 0);
                                        if (!FreeImage.Paste(image_alpha2, image_alpha, 0, 0, ((int)Properties.Settings.Default.DrivingMapTransparency * 256) / 100))
                                        {
                                            //sometimes alpha tile from GG is corrupted, it means tile is blank
                                            //we keep tile
                                        }
                                        FreeImage.Unload(image_alpha);
                                        image_alpha = image_alpha2;
                                    }

                                    image_result = FreeImage.Composite(image_alpha, false, null, image);
                                    FreeImage.Unload(image_alpha);
                                    if (!image_result.IsNull)
                                    {
                                        FreeImage.Unload(image);
                                        image = image_result;
                                    }
                                    else
                                    {
                                        //sometimes alpha tile from GG is corrupted, it means tile is blank
                                        //we keep image
                                    }
                                    break;
                                case 13:
                                    image = LoadImageFromCache("YahooInMap", g);
                                    break;
                                case 14:
                                    image = LoadImageFromCache("YahooSatH2", g);

                                    if (worker.CancellationPending)
                                    {
                                        return;
                                    }

                                    image_alpha = LoadImageFromCache("YahooInHyb", g);

                                    if (FreeImage.GetBPP(image_alpha) != 32)
                                    {
                                        //sometimes alpha tile from GG is a 2bits image, it means tile is blank
                                        break;
                                    }

                                    if (Properties.Settings.Default.DrivingMapTransparency != 100)
                                    {
                                        image_alpha2 = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 32, 0, 0, 0);
                                        if (!FreeImage.Paste(image_alpha2, image_alpha, 0, 0, ((int)Properties.Settings.Default.DrivingMapTransparency * 256) / 100))
                                        {
                                            //sometimes alpha tile from GG is corrupted, it means tile is blank
                                            //we keep tile
                                        }
                                        FreeImage.Unload(image_alpha);
                                        image_alpha = image_alpha2;
                                    }

                                    image_result = FreeImage.Composite(image_alpha, false, null, image);
                                    FreeImage.Unload(image_alpha);
                                    if (!image_result.IsNull)
                                    {
                                        FreeImage.Unload(image);
                                        image = image_result;
                                    }
                                    else
                                    {
                                        //sometimes alpha tile from GG is corrupted, it means tile is blank
                                        //we keep image
                                    }
                                    break;
                                case 15:
                                    image = LoadImageFromCache("OpenStreetMap", g);
                                    break;
                                case 16:
                                    image = LoadImageFromCache("OSMARender", g);
                                    break;
                                case 17:
                                    image = LoadImageFromCache("OpenAerialMap", g);
                                    break;
                                case 18:
                                    image = LoadImageFromCache("OpenCycleMap", g);
                                    break;
                                case 19:
                                    image = LoadImageFromCache("BrutMap", g);
                                    break;
                                case 20:
                                    image = LoadImageFromCache("BrutMapAndNav", g);
                                    break;
                                case 21:
                                    image = LoadImageFromCache("BrutMapSat", g);
                                    break;
                                case 22:
                                    image = LoadImageFromCache("BrutMapAndNavSat", g);
                                    break;
                                case 23: //hybrid
                                    image = LoadImageFromCache("BrutMapSatH", g);

                                    if (worker.CancellationPending)
                                    {
                                        return;
                                    }

                                    image_alpha = LoadImageFromCache("BrutMapHyb", g);

                                    if (FreeImage.GetBPP(image_alpha) < 8)
                                    {
                                        //sometimes alpha tile from GG is a 2bits image, it means tile is blank
                                        break;
                                    }

                                    if (Properties.Settings.Default.DrivingMapTransparency != 100)
                                    {
                                        image_alpha2 = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 32, 0, 0, 0);
                                        if (!FreeImage.Paste(image_alpha2, image_alpha, 0, 0, ((int)Properties.Settings.Default.DrivingMapTransparency * 256) / 100))
                                        {
                                            //sometimes alpha tile from GG is corrupted, it means tile is blank
                                            //we keep tile
                                        }
                                        FreeImage.Unload(image_alpha);
                                        image_alpha = image_alpha2;
                                    }

                                    image_result = FreeImage.Composite(image_alpha, false, null, image);
                                    FreeImage.Unload(image_alpha);
                                    if (!image_result.IsNull)
                                    {
                                        FreeImage.Unload(image);
                                        image = image_result;
                                    }
                                    else
                                    {
                                        //sometimes alpha tile from GG is corrupted, it means tile is blank
                                        //we keep image
                                    }
                                    string hyb_file = td.BuildFileName("BrutMapSH", g);
                                    if (!FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, image, hyb_file, 0))
                                    {
                                        throw new IOException(Properties.Resources.ExceptionCannotSaveFile);
                                    }
                                    break;
                                case 24: //hybrid
                                    image = LoadImageFromCache("BrutMapAndNavSatH", g);

                                    if (worker.CancellationPending)
                                    {
                                        return;
                                    }

                                    image_alpha = LoadImageFromCache("BrutMapAndNavHyb", g);

                                    if (FreeImage.GetBPP(image_alpha) < 8)
                                    {
                                        //sometimes alpha tile from GG is a 2bits image, it means tile is blank
                                        break;
                                    }

                                    if (Properties.Settings.Default.DrivingMapTransparency != 100)
                                    {
                                        image_alpha2 = FreeImage.Allocate(GMapTile.TILE_SIZE, GMapTile.TILE_SIZE, 32, 0, 0, 0);
                                        if (!FreeImage.Paste(image_alpha2, image_alpha, 0, 0, ((int)Properties.Settings.Default.DrivingMapTransparency * 256) / 100))
                                        {
                                            //sometimes alpha tile from GG is corrupted, it means tile is blank
                                            //we keep tile
                                        }
                                        FreeImage.Unload(image_alpha);
                                        image_alpha = image_alpha2;
                                    }

                                    image_result = FreeImage.Composite(image_alpha, false, null, image);
                                    FreeImage.Unload(image_alpha);
                                    if (!image_result.IsNull)
                                    {
                                        FreeImage.Unload(image);
                                        image = image_result;
                                    }
                                    else
                                    {
                                        //sometimes alpha tile from GG is corrupted, it means tile is blank
                                        //we keep image
                                    }
                                    string hyb_file_andnav = td.BuildFileName("BrutMapAndNavSH", g);
                                    if (!FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, image, hyb_file_andnav, 0))
                                    {
                                        throw new IOException(Properties.Resources.ExceptionCannotSaveFile);
                                    }
                                    break;
                            }

                            if (Properties.Settings.Default.OperatingMode == 3 || Properties.Settings.Default.OperatingMode == 4 || Properties.Settings.Default.OperatingMode == 5 || Properties.Settings.Default.OperatingMode == 6)
                                reportFunction("Image download completed (" + xx + "," + yy + ")", 100.0 / (double)wholeQuant);

                            if (worker.CancellationPending)
                            {
                                return;
                            }

                            if (imageFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
                            {
                                continue;
                            }

                            if (grayscale)
                            {
                                //uint image2 = FreeImage.ConvertPalette(bigImage, image);
                                FIBITMAP image3 = FreeImage.ConvertTo24Bits(image);
                                if (image3.IsNull)
                                {
                                    throw new InvalidOperationException(Properties.Resources.ExceptionCannotConvert24bpp);
                                }
                                FIBITMAP image2 = FreeImage.ConvertTo8Bits(image3);
                                FreeImage.Unload(image3);
                                if (image2.IsNull)
                                {
                                    throw new InvalidOperationException(Properties.Resources.ExceptionCannotConvertGrayscale);
                                }
                                FreeImage.Unload(image);
                                image = image2;
                                /* FIXME
                                 * if (!paletteInit)
                                {
                                    if (!FreeImage.PaletteCopy(image, bigImage))
                                    {
                                        throw new InvalidOperationException(Properties.Resources.ExceptionGrayscalePalette);
                                    }
                                    paletteInit = true;
                                }
                                 */
                            }

                            reportFunction("Pasting " + g.Quadtree, 0);

                            if (!FreeImage.Paste(bigImage, image, (xx - topleftx) * GMapTile.TILE_SIZE, (yy - toplefty) * GMapTile.TILE_SIZE, 256))
                            {
                                throw new InvalidOperationException(Properties.Resources.ExceptionPastingImage);
                            }
                        }
                        catch (Exception)
                        {
                            if (Properties.Settings.Default.OperatingMode == 3 || Properties.Settings.Default.OperatingMode == 4 || Properties.Settings.Default.OperatingMode == 5 || Properties.Settings.Default.OperatingMode == 6)
                                reportFunction("Image download failed (" + xx + "," + yy + ")", 100.0 / (double)wholeQuant);
                        }
                        finally
                        {
                            FreeImage.Unload(image);
                            image.SetNull();
                        }
                    }
                }

                if (worker.CancellationPending || (imageFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN))
                {
                    return;
                }

                if (!grayscale && dither8bpp)
                {
                    reportFunction("Converting image to 8bpp", 0.0);
                    FIBITMAP bigImage8 = FreeImage.ColorQuantize(bigImage, FREE_IMAGE_QUANTIZE.FIQ_WUQUANT);
                    if (bigImage8.IsNull)
                    {
                        throw new InvalidOperationException(Properties.Resources.ExceptionConversion24bpp8bpp);
                    }
                    FreeImage.Unload(bigImage);
                    bigImage = bigImage8;
                    reportFunction("Image converted", 50.0);
                }

                if (worker.CancellationPending)
                {
                    return;
                }

                reportFunction("Saving image to " + Path.GetFileName(filename), 0.0);
                if (!FreeImage.Save(imageFormat, bigImage, filename, saveflags))
                {
                    throw new IOException(Properties.Resources.ExceptionCannotSaveFile);
                }

                reportFunction("Image saved", 50.0);
            }
            finally
            {
                FreeImage.Unload(bigImage);
            }
        }

        private void reportFunction(string message, double inc)
        {
            outsidereportFunction(worker, message, inc);
        }
    }
}
