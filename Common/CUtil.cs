using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.UserInterface;

using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Runtime.InteropServices;

using OpenCvSharp;

namespace IntelligentFactory
{
    public static class CUtil
    {

        public static OpenCvSharp.Point RectOfCenter(Rect rt)
        {
            return new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        }

        public static  OpenCvSharp.Point RectangleOfCenter(Rectangle rt)
        {
            return new OpenCvSharp.Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        }

        public static void CaptureScreen()
        {
            try
            {
                Bitmap FullScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(FullScreen);
                g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
                g.Dispose();
                g = null;
                FullScreen.Save(@"D:\SAVE IMAGE\FullScreen\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bmp");
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public static bool IsImageEmpty(Mat image)
        {
            if (image == null)
            {
                Logger.WriteLog(LOG.AbNormal, "Image is null");
                return true;
            }

            if (image.IsDisposed)
            {
                Logger.WriteLog(LOG.AbNormal, "Image Disposed");
                return true;
            }

            if (image.Width == 0 || image.Height == 0)
            {
                Logger.WriteLog(LOG.AbNormal, "Image Size Empty");
                return true;
            }

            return false;
        }

        public static bool SetImageChannel4(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2RGBA);

            return true;
        }

        public static bool SetImageChannel3(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2RGB);

            return true;
        }

        public static Mat SetImageChannel3ToMat(Mat image)
        {
            if (IsImageEmpty(image)) return null;

            if (image.Channels() == 1) Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2RGB);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2RGB);

            return image;
        }


        public static bool SetImageChannel1(Mat image)
        {
            if (IsImageEmpty(image)) return false;

            if (image.Channels() == 3) Cv2.CvtColor(image, image, ColorConversionCodes.RGB2GRAY);
            if (image.Channels() == 4) Cv2.CvtColor(image, image, ColorConversionCodes.RGBA2GRAY);

            return true;
        }
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetSystemTime([In] SystemTime st);
        public static List<string> GetExistsFolderList(string strFolderPath)
        {
            List<string> ListFolder = new List<string>();
            DirectoryInfo Info = new System.IO.DirectoryInfo(strFolderPath);

            if (Info.Exists)
            {
                DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.AllDirectories);

                foreach (DirectoryInfo info in CInfo)
                {
                    ListFolder.Add(info.Name);
                }
            }

            return ListFolder;
        }


        public static double DrivePercent(string strTargetDriver, out double TotalSize, out double AvaliableSize)
        {
            double dPercent = 0;

            TotalSize = 0.0D;
            AvaliableSize = 0.0D;

            try
            {
                System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
                foreach (System.IO.DriveInfo drive in drives)
                {
                    if (drive.Name == strTargetDriver)
                    {
                        // 드라이브 전체 용량
                        TotalSize = drive.TotalSize / 1000000.0D / 1024.0D;
                        AvaliableSize = drive.AvailableFreeSpace / 1000000.0D / 1024.0D;

                        // 사용중인 용량 ( 전체 용량 - 사용 가능한 용량 )
                        double dUsedSize = (int)((drive.TotalSize - drive.AvailableFreeSpace) / 1000000 / 1024.0D);

                        dPercent = dUsedSize / TotalSize * 100.0D;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return dPercent;
        }
        public static string LoadImage()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = ofd.FileName;
                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return strFilePath;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "";
            }

            return "";
        }

        public static string SaveImage()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = Application.StartupPath;
                sfd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = sfd.FileName;
                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return strFilePath;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "";
            }

            return "";
        }

        public static string[] LoadImages()
        {
            string[] Images = null;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Images = ofd.FileNames;
                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                    return Images;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return Images;
            }

            return Images;
        }


        public static string InitLogDirectory()
        {
            string strLogPath = Application.StartupPath;
            try
            {
                CUtil.InitDirectory("Log\\" + DateTime.Now.ToString("yyyy"));
                CUtil.InitDirectory("Log\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM"));
                return strLogPath = Application.StartupPath + "\\" + "Log\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM");
            }
            catch
            {
                return strLogPath;
            }
        }

        public static void InputOnlyNumber(object sender, KeyPressEventArgs e, bool nIsUsePoint, bool bUseMinus)
        {
            bool isValidInput = false;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if (nIsUsePoint == true) { if (e.KeyChar == '.') isValidInput = true; }
                if (bUseMinus == true) { if (e.KeyChar == '-') isValidInput = true; }

                if (isValidInput == false) e.Handled = true;
            }

            if (nIsUsePoint == true)
            {
                if (e.KeyChar == '.' && (string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('.') > -1)) e.Handled = true;
            }
            if (bUseMinus == true)
            {
                if (e.KeyChar == '-' && (!string.IsNullOrEmpty((sender as TextBox).Text.Trim()) || (sender as TextBox).Text.IndexOf('-') > -1)) e.Handled = true;
            }
        }
        
        public static bool ShowMessageBox(string strHead, string strMessage, FormMessageBox.MESSAGEBOX_TYPE type = FormMessageBox.MESSAGEBOX_TYPE.OKCANCEL)
        {
            try
            {
                FormMessageBox FrmMessageBox = new FormMessageBox(strHead, strMessage, type);

                Logger.WriteLog(LOG.Normal, "[{0}] ==> {1}", strHead, strMessage);                

                if (FrmMessageBox.ShowDialog() == DialogResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            try
            {
                Type dgvType = dgv.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

                pi.SetValue(dgv, setting, null);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public static string GetDirectory()
        {
            try
            {
                string strDirectory = "";

                FolderBrowserDialog fbd = new FolderBrowserDialog();

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    strDirectory = fbd.SelectedPath;
                    return strDirectory;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "";
            }
        }



        public static void UpdateLabelSignal(Label lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelSignal), lb, bOn);
            }
            else
            {
                if(bOn)
                {
                    lb.BackColor = Color.Aquamarine;
                    lb.ForeColor = Color.Black;
                    lb.Text = "ON";
                }
                else
                {
                    lb.BackColor = Color.DimGray;
                    lb.ForeColor = Color.White;
                    lb.Text = "OFF";
                }
            }
        }

        public static void UpdateLabelResult(Label lb, bool bOK)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOK);
            }
            else
            {
                if (bOK)
                {
                    lb.BackColor = Color.Aquamarine;
                    lb.ForeColor = Color.Black;
                    lb.Text = "OK";
                }
                else
                {
                    lb.BackColor = Color.Red;
                    lb.ForeColor = Color.Yellow;
                    lb.Text = "NG";
                }
            }
        }

        public static void UpdateLabelOnOff(Label lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOn);
            }
            else
            {
                if (bOn)
                {
                    lb.BackColor = Color.Aquamarine;
                    lb.ForeColor = Color.Black;
                }
                else
                {
                    lb.BackColor = Color.Black;
                    lb.ForeColor = Color.White;
                }
            }
        }

        public static void UpdateLabelOnOff(MetroTile lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOn);
            }
            else
            {
                if (bOn)
                {
                    lb.BackColor = Color.Green;
                    //lb.Style = MetroColorStyle.Lime;
                }
                else
                {
                    lb.BackColor = Color.DimGray;
                    //lb.Style = MetroColorStyle.Silver;
                }
            }
        }

        public static void UpdateLabelResult(MetroTile lb, bool bOn)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action<Label, bool>(UpdateLabelResult), lb, bOn);
            }
            else
            {
                if (bOn)
                {
                    lb.BackColor = Color.Green;
                    //lb.Style = MetroColorStyle.Lime;
                }
                else
                {
                    lb.BackColor = Color.Red;
                    //lb.Style = MetroColorStyle.Silver;
                }
            }
        }

        public static bool InitDirectory(string strFolderName)
        {
            try
            {
                string strFolderPath = Application.StartupPath + "\\" + strFolderName + "\\";
                DirectoryInfo dirRecipe = new DirectoryInfo(strFolderPath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public static bool InitImageDirectory(string strFolderName)
        {
            try
            {
                string strFolderPath = strFolderName;
                DirectoryInfo dirRecipe = new DirectoryInfo(strFolderPath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public static Bitmap Resize(Bitmap bmp, OpenCvSharp.Size sz)
        {
            Bitmap bmpResize = null;

            try
            {
                OpenCvSharp.Mat ImageResize = new OpenCvSharp.Mat();
                ImageResize = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);

                ImageResize = ImageResize.Resize(new OpenCvSharp.Size(sz.Width, sz.Height));
                bmpResize = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(ImageResize);

                ImageResize.Dispose();
                ImageResize = null;

                GC.Collect();
                return bmpResize;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return bmpResize;
            }
        }

        public static string[] AvalibleComports()
        {
            return  System.IO.Ports.SerialPort.GetPortNames();
        }


        //public static void MultipleGrab()
        //{
        //    try
        //    {
        //        for (int i = 0; i < IGlobal.Instance.iSystem.ListCamera.Count; i++)
        //        {
        //            IGlobal.Instance.iSystem.ListCamera[i].Grab();
        //        }

        //        int nTickCount = Environment.TickCount;

        //        while ((Environment.TickCount - nTickCount) < 5000)
        //        {
        //            Thread.Sleep(10);
        //            bool bIsGrabDone = true;

        //            for (int i = 0; i < IGlobal.Instance.iSystem.ListCamera.Count; i++)
        //            {
        //                if (!IGlobal.Instance.iSystem.ListCamera[i].IsGrab) bIsGrabDone = false;
        //            }

        //            if (bIsGrabDone)
        //            {
        //                for (int i = 0; i < IGlobal.Instance.iSystem.ListCamera.Count; i++)
        //                {                            
        //                    IGlobal.Instance.iSystem.ListCamera[i].IsGrab = false;
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception Desc)
        //    {
        //        ILogger.Add(LOG_TYPE.ERROR, "[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
        //    }
        //}

        //public static bool Grab(int nIndex)
        //{
        //    try
        //    {
        //        IGlobal.Instance.iSystem.ListCamera[nIndex].Grab();

        //        //int nTickCount = Environment.TickCount;

        //        //while ((Environment.TickCount - nTickCount) < 5000)
        //        //{
        //        //    Thread.Sleep(10);
        //        //    bool bIsGrabDone = true;

        //        //    if (!IGlobal.Instance.iSystem.ListCamera[nIndex].IsGrab) bIsGrabDone = false;

        //        //    if (bIsGrabDone)
        //        //    {
        //        //        IGlobal.Instance.iSystem.ListCamera[nIndex].IsGrab = false;
        //        //        return true;
        //        //    }
        //        //}

        //        return false;
        //    }
        //    catch (Exception Desc)
        //    {
        //        ILogger.Add(LOG_TYPE.ERROR, "[FAILED] {0}==>{1} Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
        //        return false;
        //    }
        //}

        
    }
}
