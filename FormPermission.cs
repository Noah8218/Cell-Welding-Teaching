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
using System.IO;

using OpenCvSharp;

using MetroFramework.Forms;


namespace IntelligentFactory
{
    public partial class FormPermission : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        public FormPermission()
        {
            InitializeComponent();
        }

        private void FormTeachingSelect_Load(object sender, EventArgs e)
        {
            try
            {                           

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string strIndex = cbPermission.Text;

                switch (strIndex)
                {
                    case "Operator":
                        Global.iSystem.Permission = ISystem.PERMISSION.OPERATOR;
                        this.Close();
                        break;
                    case "Engineer":
                        if (tbPassword.Text == Global.iSystem.Password_Engineer)
                            
                        {
                            Global.iSystem.Permission = ISystem.PERMISSION.ENGINEER;
                            this.Close();
                        }
                        else
                        {
                            lbNotice.Style = MetroFramework.MetroColorStyle.Red;
                            lbNotice.Text = "Password is Wrong";
                        }
                        break;
                    case "Administrator":
                        if (tbPassword.Text == Global.iSystem.Password_Administrator)
                        {
                            Global.iSystem.Permission = ISystem.PERMISSION.ADMINISTRATOR;
                            this.Close();
                        }
                        else
                        {
                            lbNotice.Style = MetroFramework.MetroColorStyle.Red;
                            lbNotice.Text = "Password is Wrong";
                        }
                        break;
                }
            }
            catch(Exception Desc)
            {

            }
        }
    }
}

