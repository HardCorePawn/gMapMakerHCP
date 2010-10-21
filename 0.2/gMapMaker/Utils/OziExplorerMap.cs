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
using System.IO;
using System.Globalization;
using System.Threading;

namespace gMapMaker
{
  struct MapReferencePoint
  {
    public int PixX;
    public int PixY;
    public double Lat;
    public double Long;

    public MapReferencePoint(int x, int y, double lat, double lng)
    {
      PixX = x;
      PixY = y;
      Lat = lat;
      Long = lng;
    }
  }

  class OziExplorerMap
  {
    public string MapName = "default";
    public string ImageFileFullPath = "";
    public List<MapReferencePoint> ReferencePoints = new List<MapReferencePoint>();
    public int ImageWidth = -1;
    public int ImageHeight = -1;
    public double OnePixelLength;

    public void SaveToMap(string mapFileFullPath)
    {
      CultureInfo currentCI = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = MainForm.ciUS;

      if (ReferencePoints.Count < 4)
      {
        throw new ArgumentException();
      }

      try
      {
        using (StreamWriter s = File.CreateText(mapFileFullPath))
        {
          s.WriteLine("OziExplorer Map Data File Version 2.2");
          s.WriteLine(MapName);
          s.WriteLine(ImageFileFullPath);
          s.WriteLine("1 ,Map Code,");
          s.WriteLine("WGS 84,,   0.0000,   0.0000,WGS 84");
          s.WriteLine("Reserved 1");
          s.WriteLine("Reserved 2");
          s.WriteLine("Magnetic Variation,,,E");
          s.WriteLine("Map Projection,Mercator,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No");
          for (int i = 0; i < 30; i++)
          {
            //Point01,xy, 494, 235,in, deg, 24, 0,S, 148, 0,E, grid, , , ,S
            if (i < ReferencePoints.Count)
            {
              double LatDeg = Math.Truncate(ReferencePoints[i].Lat);
              double LatMin = Math.Abs((ReferencePoints[i].Lat - LatDeg) * 60.0);
              double LongDeg = Math.Truncate(ReferencePoints[i].Long);
              double LongMin = Math.Abs((ReferencePoints[i].Long - LongDeg) * 60.0);
              char LatDirection = (ReferencePoints[i].Lat >= 0) ? 'N' : 'S';
              char LongDirection = (ReferencePoints[i].Long >= 0) ? 'E' : 'W';
              LatDeg = Math.Abs(LatDeg);
              LongDeg = Math.Abs(LongDeg);
              s.WriteLine("Point{0:00},xy,{1,7},{2,7},in, deg,{3,5:F0},{4,10:F6},{5},{6,5:F0},{7,10:F6},{8}, grid, , , ,N", i + 1, ReferencePoints[i].PixX, ReferencePoints[i].PixY, LatDeg, LatMin, LatDirection, LongDeg, LongMin, LongDirection);
            }
            else
            {
              s.WriteLine("Point{0:00},xy,       ,       ,in, deg,     ,          ,N,     ,          ,E, grid, , , ,N", i + 1);
            }
          }
          s.WriteLine("Projection Setup,,,,,,,,,,");
          s.WriteLine("Map Feature = MF ; Map Comment = MC     These follow if they exist");
          s.WriteLine("Track File = TF      These follow if they exist");
          s.WriteLine("Moving Map Parameters = MM?    These follow if they exist");
          s.WriteLine("MM0,Yes");
          s.WriteLine("MMPNUM,4");
          s.WriteLine("MMPXY,1,{0},{1}", ReferencePoints[0].PixX, ReferencePoints[0].PixY);
          s.WriteLine("MMPXY,2,{0},{1}", ReferencePoints[3].PixX, ReferencePoints[3].PixY);
          s.WriteLine("MMPXY,3,{0},{1}", ReferencePoints[1].PixX, ReferencePoints[1].PixY);
          s.WriteLine("MMPXY,4,{0},{1}", ReferencePoints[2].PixX, ReferencePoints[2].PixY);
          s.WriteLine("MMPLL,1, {0,11:F6}, {1,11:F6}", ReferencePoints[0].Long, ReferencePoints[0].Lat);
          s.WriteLine("MMPLL,2, {0,11:F6}, {1,11:F6}", ReferencePoints[3].Long, ReferencePoints[3].Lat);
          s.WriteLine("MMPLL,3, {0,11:F6}, {1,11:F6}", ReferencePoints[1].Long, ReferencePoints[1].Lat);
          s.WriteLine("MMPLL,4, {0,11:F6}, {1,11:F6}", ReferencePoints[2].Long, ReferencePoints[2].Lat);
          s.WriteLine("MM1B,{0:F6}", OnePixelLength);
          s.WriteLine("MOP,Map Open Position,0,0");
          s.WriteLine("IWH,Map Image Width/Height,{0},{1}", ImageWidth, ImageHeight);
        }
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = currentCI;
      }
    }
  }
}
