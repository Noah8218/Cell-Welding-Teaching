using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentFactory
{
    public partial class FormInit : Form
    {
        public FormInit(string strNotice)
        {
            InitializeComponent();

            lbNotice.Text = strNotice;
        }

        public void OnInit(object sender, StringEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnInit(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                lbNotice.Text = e.Message;
            }
        }

        public void OnInitEnd(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnInitEnd(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                this.Close();
                
            }
        }
    }
}
