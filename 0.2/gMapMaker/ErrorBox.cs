using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace gMapMaker
{
    public partial class ErrorBox : Form
    {
        private static ErrorBox instance = new ErrorBox();
        private delegate void AddMessageDelegate(string message);
        private AddMessageDelegate addMessageHandler;
        private ErrorBox()
        {
            InitializeComponent();
            this.Text = Properties.Resources.ApplicationErrorTitle;
            addMessageHandler = new AddMessageDelegate(this.AddMessage);
            CreateHandle();
            labelCommonError.Text = Properties.Resources.ApplicationErrorText;
        }

        public static ErrorBox GetInstance()
        {
            return instance;
        }

        public static void ShowErrorMessage(string message)
        {
            instance.Invoke(instance.addMessageHandler, new object[1] { message });
        }

        public void AddMessage(string message)
        {
            if (instance.Visible == false)
            {
                instance.Show();
            }

            instance.BringToFront();
            int index = listBox.Items.Add(message);
            listBox.SelectedIndex = index;
        }

        public static void ResetAndHideErrors()
        {
            instance.listBox.Items.Clear();
            instance.Hide();
        }
    }
}