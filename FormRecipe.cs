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
    public partial class FormRecipe : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        int originalExStyle = -1;
        bool enableFormLevelDoubleBuffering = true;
        public EventHandler EventUpdateControl;

        public FormRecipe()
        {
            InitializeComponent();
        }

        private void FormRecipe_Load(object sender, EventArgs e)
        {
            try
            {
                InitEvent();

                this.KeyPreview = true;

                dgvRecipeList.SortCompare += DgvRecipeList_SortCompare;

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            
        }

        private void dgvRecipeList_SortCompare_1(object sender, DataGridViewSortCompareEventArgs e)
        {
            int a = int.Parse(e.CellValue1.ToString());
            int b = int.Parse(e.CellValue2.ToString());
            e.SortResult = a.CompareTo(b);
            e.Handled = true;
        }

        private void DgvRecipeList_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            int a = int.Parse(e.CellValue1.ToString());
            int b = int.Parse(e.CellValue2.ToString());
            e.SortResult = a.CompareTo(b);
            e.Handled = true;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keys)e.KeyValue == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                if (originalExStyle == -1)
                    originalExStyle = base.CreateParams.ExStyle;

                CreateParams cp = base.CreateParams;
                if (enableFormLevelDoubleBuffering)
                    cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                else
                    cp.ExStyle = originalExStyle;

                return cp;
            }
        }

        private bool InitEvent()
        {
            try
            {
                Global.iSystem.EventChangedRecipe += OnChangedRecipe;
                EventUpdateControl += OnUpdateControl;

                if (Global.iSystem.EventChangedRecipe != null)
                {
                    Global.iSystem.EventChangedRecipe(null, null);
                }

                if(EventUpdateControl != null)
                {
                    EventUpdateControl(null, null);
                }

                cbRecipeRefList.SelectedIndex = 0;
                cbRecipeRefModifyList.SelectedIndex = 0;

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
                    lbModelName.Text = strRecipeArr[0];
                    lbLotIdName.Text = strRecipeArr[1];
                    lbLastUpdateTime.Text = Global.iSystem.LastRecipeUpdateTime;

                    InitRecipeList();

                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        private void OnUpdateControl(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnUpdateControl(sender, e);
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
                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }


        public bool InitRecipeList()
        {
            try
            {
                dgvRecipeList.Rows.Clear();
                cbRecipeRefList.Items.Clear();
                cbRecipeRefModifyList.Items.Clear();

                List<string> listRecipe = new List<string>();

                DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\RECIPE");
                if (di.Exists)
                {
                    DirectoryInfo[] diRecipies = di.GetDirectories();

                    for (int i = 0; i < diRecipies.Length; i++)
                    {
                        string strRecipe = diRecipies[i].Name;
                        listRecipe.Add(strRecipe);
                    }
                }

                for (int i = 0; i < listRecipe.Count; i++)
                {
                    string strName = listRecipe[i];

                    string[] strArr = strName.Split('.');

                    if(strArr.Length < 2)
                    {
                        continue;
                    }

                    string strModelNo = strArr[0];
                    string strRecipeName = strArr[1];

                    string strNewName = string.Format("{0}.{1}", i, strRecipeName);

                    dgvRecipeList.Rows.Add(strModelNo, strRecipeName);

                }

                dgvRecipeList.Sort(dgvRecipeList.Columns[0], ListSortDirection.Ascending);

                for(int i =0; i < dgvRecipeList.Rows.Count; i++)
                {                    
                    string strName = dgvRecipeList.Rows[i].Cells[0].Value.ToString() + "." + dgvRecipeList.Rows[i].Cells[1].Value.ToString();
                    cbRecipeRefList.Items.Add(strName);
                    cbRecipeRefModifyList.Items.Add(strName);
                }

                if (dgvRecipeList.Rows.Count > 0)
                {
                    cbRecipeRefList.SelectedIndex = 0;
                    cbRecipeRefModifyList.SelectedIndex = 0;
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private void btnNewRecipe_Click(object sender, EventArgs e)
        {
            pnRecipeNew.Visible = true;
        }

        private void btnNewRecipeCancel_Click(object sender, EventArgs e)
        {
            pnRecipeNew.Visible = false;
        }

        private void btnDeleteRecipe_Click(object sender, EventArgs e)
        {
            try
            {
                int nIndex = dgvRecipeList.CurrentRow.Index;
                string strRecipeName = dgvRecipeList.Rows[nIndex].Cells[0].Value.ToString() + "." + dgvRecipeList.Rows[nIndex].Cells[1].Value.ToString();
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName;

                if (strRecipeName == Global.iSystem.Recipe.Name)
                {
                    CUtil.ShowMessageBox("Unable to delete recipe.", "This recipe is currently applied.");
                    return;
                }

                FormMessageBox FrmMessageBox = new FormMessageBox("Select the Recipe", string.Format("Do you want to Delete the Recipe ==> [{0}]?", strRecipeName));
                if(FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo di = new DirectoryInfo(strPath);
                    di.Delete(true);
                    InitRecipeList();

                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
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

        private void btnNewRecipeCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string strRecipeName = tbRecipeNewName.Text;

                if (strRecipeName == "")
                {
                    CUtil.ShowMessageBox("Notice", "Please enter your model name.");
                    return;
                }

                string strPrevRecipe = cbRecipeRefList.Text;

                List<int> ContinueIndex = new List<int>();
                int nNotContinnueIndex = 0;
                for(int i =0; i < dgvRecipeList.RowCount - 1; i++)
                {
                    int nIndePre = int.Parse(dgvRecipeList.Rows[i].Cells[0].Value.ToString());
                    int nIndexAft = int.Parse(dgvRecipeList.Rows[i+1].Cells[0].Value.ToString());
                    if ((nIndexAft - nIndePre) != 1)
                    {
                        nNotContinnueIndex = nIndePre + 1;
                        break;
                    }
                }                
                if (nNotContinnueIndex == 0) { nNotContinnueIndex = int.Parse(dgvRecipeList.Rows[dgvRecipeList.RowCount - 1].Cells[0].Value.ToString()) + 1; }

                string strPLCNo = nNotContinnueIndex.ToString();

                FormMessageBox FrmMessageBox = new FormMessageBox("Select the Recipe", string.Format("Do you want to Select the Recipe ==> [{0}]?", strRecipeName));
                if(FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    string strNewName = string.Format("{0}.{1}", strPLCNo, strRecipeName);

                    string strPrevPath = Application.StartupPath + "\\RECIPE\\" + strPrevRecipe;
                    string strNewPath = Application.StartupPath + "\\RECIPE\\" + strNewName;

                    DirectoryInfo dicCheck = new DirectoryInfo(strNewPath);
                    if (dicCheck.Exists)
                    {
                        CUtil.ShowMessageBox("Notice", "The same recipe name exists.");
                        return;
                    }

                    DirectoryInfo existingDir = new DirectoryInfo(strPrevPath);
                    DirectoryInfo copyDir = new DirectoryInfo(strNewPath);

                    SynchFolder(existingDir, copyDir);

                    //Global.iSystem.ChangeRecipe(string.Format("{0}.{1}", strPLCNo, strRecipeName));

                    Global.iSystem.LastRecipe = Global.iSystem.Recipe.Name;

                    Global.iSystem.SaveConfig();

                    Global.iSystem.Notice = string.Format("Change the Recipe {0} ==> {1}", strPrevRecipe, strRecipeName);

                    InitRecipeList();
                }
                else
                {
                    CUtil.ShowMessageBox("ALARM", "ENTER THE ONLY NUMBER");
                }
               
                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            

        }

        /// <summary>
        /// 원본/대상 폴더의 파일들을 비교하여 데이터를 backup합니다.
        /// </summary>
        /// <param name="existingDir"></param>
        /// <param name="copyDir"></param>
        private void SynchFolder(DirectoryInfo existingDir, DirectoryInfo copyDir)
        {
            try
            {
                // 각각의 폴더에 있는 파일을 얻습니다.
                FileInfo[] existingFiles = existingDir.GetFiles(); // 원본

                if (!copyDir.Exists)
                {
                    copyDir.Create();
                }

                FileInfo[] copyFiles = copyDir.GetFiles(); // 대상 파일

                bool findFile = false;
                int nIndex = 0;

                #region 파일 비교
                foreach (var existingFile in existingFiles)
                {
                    findFile = false;
                    nIndex = -1;
                    foreach (var copyFile in copyFiles)
                    {
                        nIndex++;

                        if (copyFile == null)
                        {
                            continue;
                        }

                        // 두 파일의 이름이 같다면
                        if (existingFile.Name == copyFile.Name)
                        {
                            findFile = true;

                            // 두 파일의 마지막 쓰기 시간이 틀리다면
                            if (existingFile.LastWriteTime != copyFile.LastWriteTime)
                            {
                                try
                                {
                                    if (existingFile.LastWriteTime > copyFile.LastWriteTime)
                                    {
                                        File.Copy(existingFile.FullName, copyFile.FullName, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                copyFiles[nIndex] = null;

                                break;
                            }
                        }
                    }

                    // 원본에는 있는데, 대상 폴더에 없는 경우에는 무조건 복사
                    if (!findFile)
                    {
                        try
                        {
                            String path = copyDir.FullName + "\\" + existingFile.Name;
                            existingFile.CopyTo(path);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                #endregion

                #region 폴더 비교
                DirectoryInfo[] existingFolders = existingDir.GetDirectories();
                DirectoryInfo[] copyFolders = copyDir.GetDirectories();

                foreach (var existingFolder in existingFolders)
                {
                    findFile = false;
                    nIndex = -1;

                    foreach (var copyFolder in copyFolders)
                    {
                        nIndex++;

                        if (copyFolder == null)
                        {
                            continue;
                        }

                        // 폴더가 있다면
                        if (existingFolder.Name == copyFolder.Name)
                        {
                            findFile = true;

                            // 재귀함수를 호출하여 폴더안에 폴더를 검사
                            // 재귀함수이기에 첫번째부터 진행하였던 파일들을 다시 검사
                            // 매개변수는 foreach문으로 처음에 가져왔던 폴더들로 다시 진행
                            SynchFolder(existingFolder, copyFolder);

                            copyFolders[nIndex] = null;

                            //break;
                        }
                    }

                    // 원본에는 있는데, 대상 폴더에 없는 경우에는 무조건 복사
                    if (!findFile)
                    {
                        try
                        {
                            string path = copyDir.FullName + "\\" + existingFolder.Name;
                            Directory.CreateDirectory(path);
                            SynchFolder(existingFolder, new DirectoryInfo(path));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            #endregion
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnRecipeBackupView.Visible = true;
        }

        private void btnOriginalPath_Click(object sender, EventArgs e)
        {
            OpenFolderPath(out string strPath);
            
            if(strPath == "")
            {
                return;
            }

            tbRecipeNewName.Text = strPath;
        }

        private void btnBackupPath_Click(object sender, EventArgs e)
        {
            OpenFolderPath(out string strPath);

            if(strPath == "")
            {
                return;
            }

            tbBackupPath.Text = strPath;
        }

        private bool OpenFolderPath(out string strdirPath)
        {
            strdirPath = "";
            try
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        strdirPath = fbd.SelectedPath;
                    }
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public int m_nSelectedIndex = 0;
        private void dgvRecipeList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                m_nSelectedIndex = dgvRecipeList.CurrentRow.Index;
                string strRecipeName = dgvRecipeList.Rows[m_nSelectedIndex].Cells[0].Value.ToString() + "." + dgvRecipeList.Rows[m_nSelectedIndex].Cells[1].Value.ToString();

                string[] strArr = strRecipeName.Split('.');

                lbSelectedModel.Text = strRecipeName;

                //Logger.WriteLog(LOG.SYSTEM, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }


        private void btnSpecSave_Click(object sender, EventArgs e)
        {
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(string.Format("Select the Recipe {0}", Global.iSystem.Recipe.Name), string.Format("Do you want to Save the Spec?"));

                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    if (EventUpdateControl != null)
                    {
                        EventUpdateControl(null, null);
                    }
                }
                FrmMessageBox.Close();

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void tbRecipe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.')
                e.KeyChar = Convert.ToChar(0);
        }

        private void btnRecipeNew_Click(object sender, EventArgs e)
        {
            pnRecipeNew.Visible = true;
        }

        private void btnRecipeDel_Click(object sender, EventArgs e)
        {
            try
            {
                int nIndex = dgvRecipeList.CurrentRow.Index;
                string strRecipeName = dgvRecipeList.Rows[nIndex].Cells[0].Value.ToString() + "." + dgvRecipeList.Rows[nIndex].Cells[1].Value.ToString();
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName;

                if (strRecipeName == Global.iSystem.Recipe.Name)
                {
                    CUtil.ShowMessageBox("Unable to delete recipe.", "This recipe is currently applied.");
                    return;
                }

                FormMessageBox FrmMessageBox = new FormMessageBox("Select the Recipe", string.Format("Do you want to Delete the Recipe ==> [{0}]?", strRecipeName));
                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    DirectoryInfo di = new DirectoryInfo(strPath);
                    di.Delete(true);
                    
                    InitRecipeList();                    
                }
                
                FrmMessageBox.Close();
                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnRecipeSelect_Click(object sender, EventArgs e)
        {
            try
            {
                string strRecipeName = "";
                strRecipeName = dgvRecipeList[0, m_nSelectedIndex].Value.ToString() + "." + dgvRecipeList[1, m_nSelectedIndex].Value.ToString();
                
                FormMessageBox FrmMessageBox = new FormMessageBox("Select the Recipe", string.Format("Do you want to Select the Recipe ==> [{0}]?", strRecipeName));

                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    if (strRecipeName != "")
                    {
                        string strPrevRecipe = Global.iSystem.Recipe.Name;
                        Global.iSystem.RecipeName = strRecipeName;

                        Global.iSystem.LastRecipe = strRecipeName;

                        Global.iSystem.SaveConfig();

                        Global.iSystem.Notice = string.Format("Change the Recipe {0} ==> {1}", strPrevRecipe, strRecipeName);
                    }
                    else { CUtil.ShowMessageBox("Notice", "Please Select the Recipe."); }
                   
                }                
                FrmMessageBox.Close();
                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnRecipeModify_Click(object sender, EventArgs e)
        {
            try
            {
                pnRecipeModify.Visible = true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void tbRecipeNewName_Click(object sender, EventArgs e)
        {

        }

        private void btnModifyRecipeCancel_Click(object sender, EventArgs e)
        {
            pnRecipeModify.Visible = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        private void btnModifyRecipeCreate_Click(object sender, EventArgs e)
        {
            try
            {
                string strRecipeNo = tbRecipeModifyNo.Text;
                string strRecipeName = tbRecipeModifyName.Text;

                if (strRecipeName == "")
                {
                    CUtil.ShowMessageBox("Notice", "Please enter your model name.");                    
                    return;
                }

                if (strRecipeNo == "")
                {
                    CUtil.ShowMessageBox("Notice", "Please enter your model number.");                    
                    return;
                }

                string strPrevRecipe = cbRecipeRefModifyList.Text;

                FormMessageBox FrmMessageBox = new FormMessageBox("Select the Recipe", string.Format("Do you want to Modify the Recipe ==> [{0}]?", strRecipeName));
                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    string strNewName = string.Format("{0}.{1}", strRecipeNo, strRecipeName);

                    string strPrevPath = Application.StartupPath + "\\RECIPE\\" + strPrevRecipe;
                    string strNewPath = Application.StartupPath + "\\RECIPE\\" + strNewName;

                    DirectoryInfo dicCheck = new DirectoryInfo(strNewPath);
                    if (dicCheck.Exists)
                    {
                        CUtil.ShowMessageBox("Notice", "The same recipe name exists.");                        
                        return;                        
                    }

                    DirectoryInfo existingDir = new DirectoryInfo(strPrevPath);
                    if (existingDir.Exists)
                    {
                        existingDir.MoveTo(strNewPath);
                        existingDir = new DirectoryInfo(strNewPath);
                    }

                    Invoke((MethodInvoker)delegate ()
                    {
                        InitRecipeList();
                    });   
                }                

                FrmMessageBox.Close();

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }


    }
}

