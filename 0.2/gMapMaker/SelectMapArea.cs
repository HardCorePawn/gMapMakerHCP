using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace gMapMaker
{
    public partial class SelectMapArea : Form
    {
        HtmlDocument document = null;

        public SelectMapArea()
        {
            InitializeComponent();

            this.webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

            this.Cursor = Cursors.WaitCursor;

            this.webBrowser.Navigate(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "SelectMapArea.htm"));
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            document = this.webBrowser.Document;

            HtmlElement btnSubmit = document.GetElementById("btnSubmit");
            if (btnSubmit != null)
            {
                btnSubmit.Click += new HtmlElementEventHandler(btnSubmit_Click);
            }

            this.Cursor = Cursors.Default;
        }

        public string TLLat
        {
            get 
            {
                if (document == null)
                    return string.Empty;

                HtmlElement e = document.GetElementById("TLLatitude");
                
                return (e == null)  ? string.Empty : e.InnerHtml;
            }
        }

        public string TLLong
        {
            get
            {
                if (document == null)
                    return string.Empty;

                HtmlElement e = document.GetElementById("TLLongitude");

                return (e == null) ? string.Empty : e.InnerHtml;
            }
        }

        public string BRLat
        {
            get
            {
                if (document == null)
                    return string.Empty;

                HtmlElement e = document.GetElementById("BRLatitude");

                return (e == null) ? string.Empty : e.InnerHtml;
            }
        }

        public string BRLong
        {
            get
            {
                if (document == null)
                    return string.Empty;

                HtmlElement e = document.GetElementById("BRLongitude");

                return (e == null) ? string.Empty : e.InnerHtml;
            }
        }

        void btnSubmit_Click(object sender, HtmlElementEventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}