using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using MetroFramework.Forms;
using MetroFramework.Controls;


namespace IntelligentFactory
{
    public partial class FormIO_BD : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private const int DGV_NO = 0;
        private const int DGV_NAME = 1;
        private const int DGV_STATUS = 2;
        public EventHandler<EventArgs> EventUpdateStatus;
        public FormIO_BD()
        {
            InitializeComponent();
        }

        private void FormIO_Load(object sender, EventArgs e)
        {
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
            //for ( int i = 0; i < Global.IO.Inputs.Count; i++ )
            //{
            //    for ( int j = 0; j < 2; j ++ )
            //    {
            //        dgvDI.Rows[i].Cells[j].Style.ForeColor = Color.Black;
            //        dgvDI.Rows[i].Cells[j].Style.BackColor = Color.White;

            //        dgvDI.Rows[i].Cells[j].Style.SelectionForeColor = Color.Black;
            //        dgvDI.Rows[i].Cells[j].Style.SelectionBackColor = Color.White;
            //    }
            //}

            //for ( int i = 0; i < Global.IO.Outputs.Count; i++ )
            //{
            //    for ( int j = 0; j < 3; j ++ )
            //    {
            //        dgvDO.Rows[i].Cells[j].Style.ForeColor = Color.Black;
            //        dgvDO.Rows[i].Cells[j].Style.BackColor = Color.White;

            //        dgvDO.Rows[i].Cells[j].Style.SelectionForeColor = Color.Black;
            //        dgvDO.Rows[i].Cells[j].Style.SelectionBackColor = Color.White;
            //    }
            //}

            return true;
        }

        

        

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void dgvDI_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDO_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void dgvDO_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
