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

namespace IntelligentFactory
{
    public partial class FormSettings_Camera : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;
        private int m_nCameraIndex = 0;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_Camera()
        {
            InitializeComponent();

            cbIndex.SelectedIndex = 0;
            cbTriggerMode.SelectedIndex = 0;
        }

        private void FormSettings_Camera_Load(object sender, EventArgs e)
        {
            InitEvent();
        }

        private bool InitEvent()
        {
            try
            {
                Global.iSystem.EventChangedRecipe += OnChangedRecipe;

                if (Global.iSystem.EventChangedRecipe != null)
                {
                    Global.iSystem.EventChangedRecipe(null, null);
                }

                if (EventUpdateUi != null)
                {
                    EventUpdateUi(null, null);
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
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
                    string[] strRecipeArr = Global.iSystem.Recipe.Name.Split('.');
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int nIndex = cbIndex.SelectedIndex;

            FormMessageBox FrmMessageBox = new FormMessageBox(string.Format("Save the Camera #{0} Parameter", (nIndex).ToString()), string.Format("Do you want the save?"));

            if (FrmMessageBox.ShowDialog() == DialogResult.OK)
            {
                //Global.CamManager.Cameras[nIndex].IP = tbIP.Text;
                //Global.CamManager.Cameras[nIndex].ExposureTime = double.Parse(tbExposure.Text);
                //Global.CamManager.Cameras[nIndex].Gain = double.Parse(tbGain.Text);

                //Global.CamManager.Cameras[nIndex].SetExposure(Global.CamManager.Cameras[nIndex].ExposureTime);
                //Global.CamManager.Cameras[nIndex].SetGain(Global.CamManager.Cameras[nIndex].Gain);

                //Global.CamManager.Cameras[nIndex].SaveConfig(Global.iSystem.Recipe.Name);
            }
        }

        private void cbIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_nCameraIndex = cbIndex.SelectedIndex;
            //if (m_nCameraIndex >= Global.CamManager.Cameras.Count)
            //{
            //    IUtil.ShowMessageBox("ALARM", "Camera Index Overflow");
            //    return;
            //}
        }
    }
 }

