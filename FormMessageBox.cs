using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;

namespace IntelligentFactory
{
    public partial class FormMessageBox : MetroForm
    {
        public enum MESSAGEBOX_TYPE { OKCANCEL, OK, CANCEL, EXIT };

        public FormMessageBox(string strHead, string strMessage)
        {
            InitializeComponent();

            lbMessage.Text = strMessage;
        }
        public FormMessageBox(string strHead, string strMessage, MESSAGEBOX_TYPE type)
        {
            InitializeComponent();

            lbMessage.Text = strMessage;

            switch (type)
            {
                case MESSAGEBOX_TYPE.OKCANCEL:
                    break;
                case MESSAGEBOX_TYPE.EXIT:
                    {
                        btnOK.Visible = false;
                        Point ptButton = btnCancel.Location;
                        btnCancel.Location = new Point((this.Size.Width / 2) - (btnCancel.Size.Width / 2), ptButton.Y);
                        btnCancel.Text = "EXIT";
                    }
                    break;
                case MESSAGEBOX_TYPE.OK:
                    {
                        btnCancel.Visible = false;
                        Point ptButton = btnOK.Location;
                        btnOK.Location = new Point((this.Size.Width / 2) - (btnOK.Size.Width / 2), ptButton.Y);
                    }
                    break;
                case MESSAGEBOX_TYPE.CANCEL:
                    {
                        btnOK.Visible = false;
                        Point ptButton = btnCancel.Location;
                        btnCancel.Location = new Point((this.Size.Width / 2) - (btnCancel.Size.Width / 2), ptButton.Y);
                    }
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
