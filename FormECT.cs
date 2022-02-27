using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;


//IF 전용 Library
using OpenCvSharp;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;

using MetroFramework;
using MetroFramework.Forms;
using System.Xml;

namespace IntelligentFactory
{
    public partial class FormECT : MetroForm
    {
        public EventHandler<EventArgs> EventUpdateUi;      



        public FormECT()
        {
            InitializeComponent();
        }

        private void FormECT_Load(object sender, EventArgs e)
        {
            EventUpdateUi += OnDriveVolumeUpdate;

            if (EventUpdateUi != null)
            {
                EventUpdateUi(null, null);
            }


            
        }


        private void OnDriveVolumeUpdate(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnDriveVolumeUpdate(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {
                    DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

                    foreach (var drive in drives)
                    {
                        string strDrive = drive.ToString().Substring(0, 1);
                        cbDrivePath.Items.Add(strDrive);
                    }

                    nbDeleteRemainDay.Value = IGlobal.Instance.iSystem.Recipe.DeleteImageDay;
                    cbDrivePath.Text = IGlobal.Instance.iSystem.Recipe.DrivePath;
                    nbDiskVolume.Value = IGlobal.Instance.iSystem.Recipe.DriveVolum;
                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        private void btnImageDeleteSetting_Click(object sender, EventArgs e)
        {
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(string.Format("Save Image"), string.Format("Do you want to Save?"));
                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    string strDrivePath = cbDrivePath.Text;
                    int nDriveVolume = int.Parse(nbDiskVolume.Value.ToString());
                    int nDeleteDay = int.Parse(nbDeleteRemainDay.Value.ToString());

                    //iData.DrivePath = strDrivePath;
                    //iData.DriveVolum = nDriveVolume;
                    //iData.DeleteImageDay = nDeleteDay;

                    IGlobal.Instance.iSystem.Recipe.DrivePath = strDrivePath;
                    IGlobal.Instance.iSystem.Recipe.DriveVolum = nDriveVolume;
                    IGlobal.Instance.iSystem.Recipe.DeleteImageDay = nDeleteDay;

                    IGlobal.Instance.iSystem.Recipe.SaveConfig();
                    FrmMessageBox.Close();
                }
                else
                {
                    FrmMessageBox.Close();
                    
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }      

        




    }
}
