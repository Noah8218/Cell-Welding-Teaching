using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.XImgProc;

namespace IntelligentFactory
{
    public partial class FormAlarm : Form
    {
        public EventHandler<EventArgs> EventAlarmEnd;

        public FormAlarm()
        {
            InitializeComponent();
        }

        public void OnAlarm(object sender, StringEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnAlarm(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                lbReason.Text = e.Message;                
            }
        }

        public void OnAlarmEnd(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnAlarmEnd(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    //Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                this.Visible = false;

                if (EventAlarmEnd != null)
                {
                    EventAlarmEnd(null, null);
                }

                IAlarm.Clear();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            OnAlarmEnd(null, null);
        }

        private void FormAlarm_Load(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    
}
