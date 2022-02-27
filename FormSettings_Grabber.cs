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
    public partial class FormSettings_Grabber : MetroForm
    {
        IGlobal Global = IGlobal.Instance;

        #region Event Register        
        public EventHandler<EventArgs> EventUpdateUi;
        #endregion
        public FormSettings_Grabber()
        {
            InitializeComponent();

            try
            {
                string[] ComportList = CUtil.AvalibleComports();

                if (ComportList.Length > 0)
                {
                    cbPortName.Items.AddRange(ComportList);
                    cbPortName.SelectedIndex = 0;
                }

                if(cbBaudrate.Items.Count > 0) cbBaudrate.SelectedIndex = 0;

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);                
            }
        }

        private void FormSettings_Illumination_Load(object sender, EventArgs e)
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
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(string.Format("Save the Illumination Parameter"), string.Format("Do you want the save?"));

                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }
        }

        private void trbValue_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                int nValue = trbValue.Value;
                lbValue.Text = nValue.ToString();

                //if (Global.LightController.IsOpen)
                //{
                //    int nLightIndex = cbChannel.SelectedIndex + 1;                    
                //    Global.LightController.SetValue(nLightIndex, nValue);
                //}
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }
        }

        private void btnON_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }            
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }            
        }
    }
 }

