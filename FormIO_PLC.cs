﻿using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;


namespace IntelligentFactory
{
    public partial class FormIO_PLC : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private const int DGV_NO = 0;
        private const int DGV_NAME = 1;
        private const int DGV_STATUS = 2;
        public EventHandler<EventArgs> EventUpdateStatus;
        public FormIO_PLC()
        {
            InitializeComponent();
        }

        private void FormIO_Load(object sender, EventArgs e)
        {
            InitIO();
            InitUI();
            timer1.Enabled = true;

            Global.iSystem.EventUpdateStyle += OnChangeStyle;
            metroStyleManager.Style = (MetroFramework.MetroColorStyle)Global.iSystem.StyleIndex;
        }

        private void FormIO_FormClosing(object sender, FormClosingEventArgs e)
        {
            Global.iSystem.EventUpdateStyle -= OnChangeStyle;
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            
        }

        private void dgvDO_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void OnChangeStyle(object sender, EventArgs e)
        {
            metroStyleManager.Style = (MetroFramework.MetroColorStyle)Global.iSystem.StyleIndex;
        }        

        public bool InitUI()
        {
          

            return true;
        }

        public bool InitIO()
        {
            try
            {
                
                //for (int i = 0; i < Global.iDevice.DIO_PLC.Inputs.Count; i++)
                //{
                //    AddRowIO(Global.iDevice.DIO_PLC.Inputs[i], "Input");
                //}

                //for (int i = 0; i < Global.iDevice.DIO_PLC.Outputs.Count; i++)
                //{
                //    AddRowIO(Global.iDevice.DIO_PLC.Outputs[i], "Output");
                //}

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }
    }
}
