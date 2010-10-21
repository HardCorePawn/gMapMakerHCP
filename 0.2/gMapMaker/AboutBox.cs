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
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace gMapMaker
{
  partial class AboutBox : Form
  {
    public AboutBox()
    {
      InitializeComponent();

      //  Initialize the AboutBox to display the product information from the assembly information.
      //  Change assembly information settings for your application through either:
      //  - Project->Properties->Application->Assembly Information
      //  - AssemblyInfo.cs
      this.Text += AssemblyTitle;
      this.labelProductName.Text = AssemblyProduct + " v" + AssemblyInformationalVersion;
      this.labelCopyright.Text = AssemblyCopyright;
      this.labelCompanyName.Text = @"http://www.mgmaps.com/cache/";
      this.labelCompanyName.Links[0].LinkData = this.labelCompanyName.Text;
  }

    #region Assembly Attribute Accessors

    static string AssemblyTitle
    {
      get
      {
        // Get all Title attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        // If there is at least one Title attribute
        if (attributes.Length > 0)
        {
          // Select the first one
          AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
          // If it is not an empty string, return it
          if (String.IsNullOrEmpty(titleAttribute.Title))
          {
            return titleAttribute.Title;
          }
        }
        // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    static string AssemblyVersion
    {
      get
      {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }

    static string AssemblyProduct
    {
      get
      {
        // Get all Product attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        // If there aren't any Product attributes, return an empty string
        if (attributes.Length == 0)
          return "";
        // If there is a Product attribute, return its value
        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    static string AssemblyInformationalVersion
    {
        get
        {
            // Get all Product attributes on this assembly
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            // If there aren't any InformationalVersion attributes, return an empty string
            if (attributes.Length == 0)
                return "";
            // If there is a InformationalVersion attribute, return its value
            return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
        }
    }

    static string AssemblyCopyright
    {
      get
      {
        // Get all Copyright attributes on this assembly
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        // If there aren't any Copyright attributes, return an empty string
        if (attributes.Length == 0)
          return "";
        // If there is a Copyright attribute, return its value
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    #endregion

    private void labelCompanyName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      string target = e.Link.LinkData as string;
      System.Diagnostics.Process.Start(target);
  }
}
}
