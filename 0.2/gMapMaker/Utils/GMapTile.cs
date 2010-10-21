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
using System.Globalization;

namespace gMapMaker
{
    /**
    * A set of simple routines to provide information about google tiles (API v2).
    * Internally a sort of offset mercator projection is used (as per google), this places the origin (0,0)
    * at the top left and goes to +1,+1 at the bottom right.
    * A proper mercator would have 0,0 in the middle and range from -0.5,-0.5 bottom left to +0.5,+0.5 top right.
    * 
    * http://intepid.com/2005-07-17/21.50/
    * http://www.internettablettalk.com/forums/archive/index.php/t-1947.html
    */
    class GMapTile
    {
        protected double topLat;
        public double TopLat
        {
            get
            {
                return topLat;
            }
        }
        protected double leftLong;
        public double LeftLong
        {
            get
            {
                return leftLong;
            }
        }
        protected double bottomLat;
        public double BottomLat
        {
            get
            {
                return bottomLat;
            }
        }
        protected double rightLong;
        public double RightLong
        {
            get
            {
                return rightLong;
            }
        }
        public double LongWidth
        {
            get
            {
                return rightLong - leftLong;
            }
        }
        public double LatHeight
        {
            get
            {
                return topLat - bottomLat;
            }
        }

        protected int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                quadtree = GetQuadtreeFromXYZoom(x, y, zoom);
                GetTileRect(x, y, zoom, out topLat, out leftLong, out bottomLat, out rightLong);
            }
        }
        protected int y;
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                quadtree = GetQuadtreeFromXYZoom(x, y, zoom);
                GetTileRect(x, y, zoom, out topLat, out leftLong, out bottomLat, out rightLong);
            }
        }
        protected uint zoom;
        public uint Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;
                quadtree = GetQuadtreeFromXYZoom(x, y, zoom);
                GetTileRect(x, y, zoom, out topLat, out leftLong, out bottomLat, out rightLong);
            }
        }

        protected string quadtree;
        public string Quadtree
        {
            get
            {
                return quadtree;
            }
            set
            {
                quadtree = value;
                XYZoom(quadtree, out x, out y, out zoom);
                GetTileRect(x, y, zoom, out topLat, out leftLong, out bottomLat, out rightLong);
            }
        }

        public double OnePixelLength
        {
            get
            {
                double lat = (topLat * Math.PI) / 180.0;
                double lngW = (LongWidth * Math.PI) / (180.0 * TILE_SIZE);
                return EARTH_RADIUS * Math.Acos(Math.Sin(lat) * Math.Sin(lat) + Math.Cos(lat) * Math.Cos(lat) * Math.Cos(lngW));
            }
        }

        public GMapTile(string q)
        {
            Quadtree = q;
        }

        public GMapTile(int x, int y, uint zoom)
        {
            this.x = x;
            this.y = y;
            Zoom = zoom; // call the setter !
        }

        public static GMapTile GetTileFromLatLongZoom(double lat, double lng, uint zoom)
        {
            int x, y;
            ToTileXY(lat, lng, zoom, out x, out y);
            return new GMapTile(x, y, zoom);
        }

        // due to GoogleMap Mercator projection limitations, absolute max value for latitude
        public const double AbsLatMax = 85.0511;
        // due to GoogleMap Mercator projection limitations, absolute max value for longitude
        public const double AbsLngMax = 179.9999;

        public static bool ParseLat(string text, out double d)
        {
            //gkl changes - commented method code and called new method
            /*Regex r1 = new Regex(@"^(?<degree>-?\d{1,2}([\.,]\d+)?)$"); //-34.56
            Regex r2 = new Regex(@"^(?<degree>\d{1,2}([\.,]\d+)?)\s*(?<dir>[NS])$"); //34.56 N
            Regex r3 = new Regex(@"^(?<degree>\d{1,2})°\s*(?<minute>\d{1,2}([\.,]\d+)?)'\s*(?<dir>[NS])$"); //34°4.56' N
            Regex r4 = new Regex(@"^(?<degree>\d{1,2})°\s*(?<minute>\d{1,2})'(?<seconde>\d{1,2}([\.,]\d+)?)''\s*(?<dir>[NS])$"); //34°4'56'' N

            Match m;

            if (r1.IsMatch(text)) m = r1.Match(text);
            else if (r2.IsMatch(text)) m = r2.Match(text);
            else if (r3.IsMatch(text)) m = r3.Match(text);
            else if (r4.IsMatch(text)) m = r4.Match(text);
            else
            {
                d = 0;
                return false;
            }

            double degree = double.Parse(m.Groups["degree"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double minute = String.IsNullOrEmpty(m.Groups["minute"].Value) ? 0 : double.Parse(m.Groups["minute"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double seconde = String.IsNullOrEmpty(m.Groups["seconde"].Value) ? 0 : double.Parse(m.Groups["seconde"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);

            d = (degree + minute / 60.0 + seconde / 3600.0) * ((String.IsNullOrEmpty(m.Groups["dir"].Value) || (m.Groups["dir"].Value == "N")) ? 1.0 : -1.0);

            return (Math.Abs(d) <= AbsLatMax);*/
            return ParseLatLong(text, AbsLatMax, out d);
        }

        public static bool ParseLong(string text, out double d)
        {
            //gkl changes - commented method code and called new method
            /*Regex r1 = new Regex(@"^(?<degree>-?\d{1,3}([\.,]\d+)?)$"); //-34.56
            Regex r2 = new Regex(@"^(?<degree>\d{1,3}([\.,]\d+)?)\s*(?<dir>[EW])$"); //34.56 E
            Regex r3 = new Regex(@"^(?<degree>\d{1,3})°\s*(?<minute>\d{1,2}([\.,]\d+)?)'\s*(?<dir>[EW])$"); //34°4.56' W
            Regex r4 = new Regex(@"^(?<degree>\d{1,3})°\s*(?<minute>\d{1,2})'(?<seconde>\d{1,2}([\.,]\d+)?)''\s*(?<dir>[EW])$"); //34°4'56'' E
            Match m;

            if (r1.IsMatch(text)) m = r1.Match(text);
            else if (r2.IsMatch(text)) m = r2.Match(text);
            else if (r3.IsMatch(text)) m = r3.Match(text);
            else if (r4.IsMatch(text)) m = r4.Match(text);
            else
            {
            d = 0;
            return false;
            }

            double degree = double.Parse(m.Groups["degree"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double minute = String.IsNullOrEmpty(m.Groups["minute"].Value) ? 0 : double.Parse(m.Groups["minute"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double seconde = String.IsNullOrEmpty(m.Groups["seconde"].Value) ? 0 : double.Parse(m.Groups["seconde"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);

            d = (degree + minute / 60.0 + seconde / 3600.0) * ((String.IsNullOrEmpty(m.Groups["dir"].Value) || (m.Groups["dir"].Value == "E")) ? 1.0 : -1.0);

            return (Math.Abs(d) <= AbsLngMax);*/
            return ParseLatLong(text, AbsLngMax, out d);
        }

        //gkl changes - added method ParseLatLong
        public static bool ParseLatLong(string text, double absLatOrLongMax, out double d)
        {
            string[] patterns = {@"^(?<degree>-?\d{1,3}([\.,]\d+)?)$",//-34.56
                                 @"^(?<degree>\d{1,3}([\.,]\d+)?)\s*(?<dir>[EW])$",//34.56 E
                                 @"^(?<degree>\d{1,3})°\s*(?<minute>\d{1,2}([\.,]\d+)?)'\s*(?<dir>[EW])$",//34°4.56' N
                                 @"^(?<degree>\d{1,3})°\s*(?<minute>\d{1,2})'(?<seconde>\d{1,2}([\.,]\d+)?)''\s*(?<dir>[EW])$"};//34°4'56'' E
            Regex r = null;
            Match m = null;
            for (int i = 0; i < 4; i++)
            {
                r = new Regex(patterns[i]);
                if (r.IsMatch(text))
                {
                    m = r.Match(text);
                    break;
                }
            }
            if (m == null)
            {
                d = 0;
                return false;
            }

            double degree = double.Parse(m.Groups["degree"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double minute = String.IsNullOrEmpty(m.Groups["minute"].Value) ? 0 : double.Parse(m.Groups["minute"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);
            double seconde = String.IsNullOrEmpty(m.Groups["seconde"].Value) ? 0 : double.Parse(m.Groups["seconde"].Value.Replace(',', '.'), MainForm.ciUS.NumberFormat);

            d = (degree + minute / 60.0 + seconde / 3600.0) * ((String.IsNullOrEmpty(m.Groups["dir"].Value) || (m.Groups["dir"].Value == "E")) ? 1.0 : -1.0);

            return (Math.Abs(d) <= absLatOrLongMax);
        }

        public const int TILE_SIZE = 256;
        public const int EARTH_RADIUS = 6378137; /* earth radius in meters */

        /**
         * Returns the pixel offset of a latitude and longitude within a single typical google tile.
         */
        // not used !!!!
        /*public static void GetPixelOffsetInTile(double lat, double lng, uint zoom, out int x, out int y)
        {
          ToZoomedPixelCoords(lat, lng, zoom, out x, out y);
          x %= TILE_SIZE;
          y %= TILE_SIZE;
        }*/

        /**
        * returns a Rectangle2D with x = lon, y = lat, width=lonSpan, height=latSpan
        * for an x,y,zoom as used by google.
        */
        public static void GetTileRect(int x, int y, uint zoom, out double topLat, out double leftLong, out double bottomLat, out double rightLong)
        {
            int tilesAtThisZoom = 1 << (int)zoom;
            double lngWidth = 360.0 / tilesAtThisZoom; // width in degrees longitude
            leftLong = -180.0 + (x * lngWidth); // left edge in degrees longitude
            rightLong = leftLong + lngWidth;

            double latHeightMerc = 1.0 / tilesAtThisZoom; // height in "normalized" mercator 0,0 top left
            double topLatMerc = y * latHeightMerc; // top edge in "normalized" mercator 0,0 top left
            double bottomLatMerc = topLatMerc + latHeightMerc;

            // convert top and bottom lat in mercator to degrees
            // note that in fact the coordinates go from about -85 to +85 not -90 to 90!
            bottomLat = (180.0 / Math.PI) * ((2.0 * Math.Atan(Math.Exp(Math.PI * (1.0 - (2.0 * bottomLatMerc))))) - (Math.PI / 2.0));

            topLat = (180.0 / Math.PI) * ((2.0 * Math.Atan(Math.Exp(Math.PI * (1.0 - (2.0 * topLatMerc))))) - (Math.PI / 2.0));

            //latHeight = topLat - bottomLat;
        }

        /**
         * returns the lat/lng as an "Offset Normalized Mercator" pixel coordinate,
         * this is a coordinate that runs from 0..1 in latitude and longitude with 0,0 being
         * top left. Normalizing means that this routine can be used at any zoom level and
         * then multiplied by a power of two to get actual pixel coordinates.
         */
        public static void ToNormalisedPixelCoords(ref double lat, ref double lng)
        {
            // first convert to Mercator projection
            // first convert the lat lon to mercator coordintes.
            if (lng > 180.0)
            {
                lng -= 360.0;
            }

            lng = (180.0 + lng) / 360.0;
            lat = 0.5 - Math.Log(Math.Tan((Math.PI / 4.0) + ((Math.PI * lat) / (2.0 * 180.0)))) / (2.0 * Math.PI);
        }

        /**
         * returns a point that is a google tile reference for the tile containing the lat/lng and at the zoom level.
         */
        public static void ToTileXY(double lat, double lng, uint zoom, out int x, out int y)
        {
            ToNormalisedPixelCoords(ref lat, ref lng);
            int scale = 1 << (int)zoom;

            // can just truncate to integer, this looses the fractional "pixel offset"
            x = (int)(lng * scale);
            y = (int)(lat * scale);
        }

        /**
         * returns a point that is a google pixel reference for the particular lat/lng and zoom
         */
        // not used !!!!
        /*public static void ToZoomedPixelCoords(double lat, double lng, uint zoom, out int x, out int y)
        {
          ToNormalisedPixelCoords(ref lat, ref lng);
          double scale = (1 << (int)zoom) * TILE_SIZE;

          x = (int)(lng * scale);
          y = (int)(lat * scale);
        }*/

        readonly static char[] lookup = new char[] { 'q', 't', 'r', 's' };

        // not used !!!!
        /*public static string GetQuadtreeFromLatLongDig(double longitude, double latitude, int digits)
        {
          StringBuilder quad = new StringBuilder("t"); // google addresses start with t

          // now convert to normalized square coordinates
          // use standard equations to map into mercator projection
          double x = (180.0 + longitude) / 360.0;
          double y = - (latitude * Math.PI) / 180.0;  // convert to radians
          y = 0.5 * Math.Log((1.0 + Math.Sin(y)) / (1.0 - Math.Sin(y)));
          y /= 2.0 * Math.PI;                      // scale factor from radians to normalized
          y += 0.5;                                // and make y range from 0 - 1

          for (; digits > 0; digits--)
          {
            // make sure we only look at fractional part
            x -= Math.Floor(x);
            y -= Math.Floor(y);

            quad.Append(lookup[(y >= 0.5 ? 1 : 0) + (x >= 0.5 ? 2 : 0)]);

            // now descend into that square
            x *= 2;
            y *= 2;
          }
          return quad.ToString();
        }*/

        public static string GetQuadtreeFromXYZoom(int x, int y, uint zoom)
        {
            StringBuilder quad = new StringBuilder();
            for (int i = 0; i < zoom; i++)
            {
                int rx = x % 2;
                int ry = y % 2;
                x /= 2;
                y /= 2;
                quad.Insert(0, lookup[rx * 2 + ry]);
            }
            quad.Insert(0, 't');
            return quad.ToString();
        }

        public static void XYZoom(string quad, out int x, out int y, out uint zoom)
        {
            x = 0;
            y = 0;
            zoom = 0;
            char[] chars = quad.Remove(0, 1).ToCharArray();
            foreach (char c in chars)
            {
                int px, py;
                switch (c)
                {
                    default:
                    case 'q':
                        px = 0;
                        py = 0;
                        break;
                    case 't':
                        px = 0;
                        py = 1;
                        break;
                    case 'r':
                        px = 1;
                        py = 0;
                        break;
                    case 's':
                        px = 1;
                        py = 1;
                        break;
                }
                x = x * 2 + px;
                y = y * 2 + py;
                zoom++;
            }
        }
    }
}