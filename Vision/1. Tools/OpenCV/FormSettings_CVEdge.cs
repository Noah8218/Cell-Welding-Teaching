#if MATROX
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
using Matrox.MatroxImagingLibrary;

using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Drawing.Imaging;

namespace IntelligentFactory
{
    public partial class FormSettings_CVEdge : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        private CPropertyLineGuage Property_LineGuage_Top = new CPropertyLineGuage("TOP_EDGE");
        private CPropertyLineGuage Property_LineGuage_Tubing = new CPropertyLineGuage("TUBING_EDGE");
        private CPropertyMatching  Property_Matching_Masking= new CPropertyMatching("MASKING");

        private CPropertyLineGuage Property_LineGuage_ContrastLow = new CPropertyLineGuage("ContrastLow");
        private CPropertyLineGuage Property_LineGuage_ContrastHigh = new CPropertyLineGuage("ContrastHigh");
        private CPropertyLineGuage Property_LineGuage_XStartoffset = new CPropertyLineGuage("X Start offset");
        private CPropertyLineGuage Property_LineGuage_XEndoffset = new CPropertyLineGuage("X End offset");
        private CPropertyLineGuage Property_LineGuage_YStartoffset = new CPropertyLineGuage("Y Start offset");
        private CPropertyLineGuage Property_LineGuage_YEndoffset = new CPropertyLineGuage("Y End Offset");

        private Mat ImageSource = new Mat();


        private MIL_ID ImageMSource = MIL.M_NULL; 


public FormSettings_CVEdge()
        {
            InitializeComponent();
        }

        private void FormSettings_CVEdge_Load(object sender, EventArgs e)
        {
            try
            {
                Property_LineGuage_Top = Global.iData.Property_LineGuage_Top;
                Property_LineGuage_Tubing = Global.iData.Property_LineGuage_Tubing;
                Property_Matching_Masking = Global.iData.Property_Matching_Masking;

                ibMaskingTemplate.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Global.iData.Property_Matching_Masking.ImageTemplate);
                ibMaskingTemplate.ZoomToFit();

                ibSource.MouseWheel += new MouseEventHandler(MouseWheelEventSource);

                //Global.iData.Property_LineGuage_ContrastLow.LoadConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_ContrastHigh.LoadConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_XStartoffset.LoadConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_XEndoffset.LoadConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_YStartoffset.LoadConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_YEndoffset.LoadConfig(Global.iSystem.Recipe.Name);


                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }            
        }

        private void MouseWheelEventSource(object sender, MouseEventArgs e)
        {
            if ((e.Delta / 120) > 0) ibSource.ZoomIn();
            else ibSource.ZoomOut();
        }

        private void OnMouseClickParameter(object sender, MouseEventArgs e)
        {
            string strIndex = "";

            if (sender is MetroLabel) strIndex = (sender as MetroLabel).Text;
            lbParameter.Text = strIndex;

            try
            {
                switch(strIndex)
                {
                    case "1. TOP EDGE":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_Top;
                        break;
                    case "2. TUBING EDGE":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_Tubing;
                        break;
                    case "3. MASKING":
                        propertygrid_Parameter.SelectedObject = Property_Matching_Masking;
                        break;
                    case "4. X Start offset":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_XStartoffset;
                        break;
                    case "5. X end Offse":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_XEndoffset;
                        break;
                    case "6. Y Start offset":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_YStartoffset;
                        break;
                    case "7. Y end offset":
                        propertygrid_Parameter.SelectedObject = Property_LineGuage_YEndoffset;
                        break;

                }
            }
            catch(Exception Desc)
            {

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Property_LineGuage_Top.SaveConfig(Global.iSystem.Recipe.Name);
                Property_LineGuage_Tubing.SaveConfig(Global.iSystem.Recipe.Name);
                Property_Matching_Masking.SaveConfig(Global.iSystem.Recipe.Name);

                //Global.iData.Property_LineGuage_ContrastLow.SaveConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_ContrastHigh.SaveConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_XStartoffset.SaveConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_XEndoffset.SaveConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_YStartoffset.SaveConfig(Global.iSystem.Recipe.Name);
                //Global.iData.Property_LineGuage_YEndoffset.SaveConfig(Global.iSystem.Recipe.Name);
            }
            catch(Exception Desc)
            {

            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Stopwatch sw_TaktTime = new Stopwatch();
            try
            {
                CIVT_CVMatching Matching_Masking = new CIVT_CVMatching("MATCHING_MASKING");
                Matching_Masking.SetProperty(Property_Matching_Masking);
                Matching_Masking.SetSourceImage(ImageSource);
                Matching_Masking.Run();

                List<Rect> Masking = new List<Rect>();
                for (int i = 0; i < Matching_Masking.Results.Count; i++)
                {   
                    Masking.Add(CConverter.ScreenCVRectToLogicalCVRect(Matching_Masking.Results[i].Bounding, (float)Property_Matching_Masking.MAGNIFIATION, (float)Property_Matching_Masking.MAGNIFIATION));
                }

                CIVT_LineGuage LineGuage_Top = new CIVT_LineGuage("TOP_EDGE");
                LineGuage_Top.SetProperty(Property_LineGuage_Top);
                LineGuage_Top.SetSourceImage(ImageSource);

                LineGuage_Top.Run(true);                

                CIVT_LineGuage LineGuage_Tubing = new CIVT_LineGuage("TUBING_EDGE");
                LineGuage_Tubing.SetProperty(Property_LineGuage_Tubing);
                LineGuage_Tubing.SetSourceImage(ImageSource);

                LineGuage_Tubing.Run(Masking);

                System.Drawing.Point ptStart = new System.Drawing.Point();
                bool bFoundFirst = false;

                Bitmap ImageDisplay = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(ImageSource);
                using (Graphics g = Graphics.FromImage(ImageDisplay))
                {
                    for (int i = 0; i < Masking.Count; i++)
                    {
                        g.DrawRectangle(new Pen(Color.Red, 2), CConverter.CVRectToRect(Masking[i]));
                    }

                    if (LineGuage_Top.Results.Count > 0 ) LineGuage_Top.Results = LineGuage_Top.Results.OrderBy(p => p.MeasPos.Y).ToList();
                    if (LineGuage_Top.Results.Count < 2)
                    {
                        CUtil.ShowMessageBox("ALARM", "Can't Find the Edge of Top");
                        return;
                    }

                    Line TopFittedLine = new Line(CConverter.PointToCVPoint(LineGuage_Top.Results[0].MeasPos), CConverter.PointToCVPoint(LineGuage_Top.Results[LineGuage_Top.Results.Count - 1].MeasPos));

                    if (LineGuage_Top.Results.Count >= 2)
                    {
                        g.DrawLine(new Pen(Color.Orange, 2)
                            , LineGuage_Top.Results[0].MeasPos.X + Property_LineGuage_Top.ROI.X
                            , LineGuage_Top.Results[0].MeasPos.Y + Property_LineGuage_Top.ROI.Y
                            , LineGuage_Top.Results[LineGuage_Top.Results.Count - 1].MeasPos.X + Property_LineGuage_Top.ROI.X
                            , LineGuage_Top.Results[LineGuage_Top.Results.Count - 1].MeasPos.Y + Property_LineGuage_Top.ROI.Y
                            );
                    }

                    if (LineGuage_Tubing.Results.Count > 0)
                    {
                        for (int i = 0; i < LineGuage_Tubing.Results.Count; i++)
                        {
                            g.DrawEllipse(new Pen(Color.Red, 1), new RectangleF(Property_LineGuage_Tubing.ROI.X + LineGuage_Tubing.Results[i].MeasPos.X - 2, Property_LineGuage_Tubing.ROI.Y + LineGuage_Tubing.Results[i].MeasPos.Y - 2, 4, 5));

                            if (bFoundFirst == false
                                && LineGuage_Tubing.Results[i].MeasPos.X > LineGuage_Tubing.dAverageX - Property_LineGuage_Tubing.OUTFITTING
                                && LineGuage_Tubing.Results[i].MeasPos.X < LineGuage_Tubing.dAverageX + Property_LineGuage_Tubing.OUTFITTING
                                )
                            {
                                bFoundFirst = true;
                                ptStart = new System.Drawing.Point(Property_LineGuage_Tubing.ROI.X + LineGuage_Tubing.Results[i].MeasPos.X, Property_LineGuage_Tubing.ROI.Y + LineGuage_Tubing.Results[i].MeasPos.Y);

                                continue;
                            }

                            if (bFoundFirst)
                            {
                                if (i + 1 < LineGuage_Tubing.Results.Count)
                                {
                                    if (LineGuage_Tubing.Results[i].MeasPos.X > LineGuage_Tubing.dAverageX - Property_LineGuage_Tubing.OUTFITTING
                                        && LineGuage_Tubing.Results[i].MeasPos.X < LineGuage_Tubing.dAverageX + Property_LineGuage_Tubing.OUTFITTING)
                                    {
                                        System.Drawing.Point ptEnd = new System.Drawing.Point(Property_LineGuage_Tubing.ROI.X + LineGuage_Tubing.Results[i].MeasPos.X, Property_LineGuage_Tubing.ROI.Y + LineGuage_Tubing.Results[i].MeasPos.Y);
                                        g.DrawLine(new Pen(Color.Red, 1), ptStart, ptEnd);

                                        double dDistance = IMath.GetDistanceLineToPoint(CConverter.PointToCVPoint(ptStart), TopFittedLine.Start, TopFittedLine.End, out OpenCvSharp.Point ptCloset);

                                        ptCloset.X += Property_LineGuage_Top.ROI.X;

                                        g.DrawLine(new Pen(Color.Orange, 1), ptStart, CConverter.CVPointToPoint(ptCloset));                                       

                                        ptStart = ptEnd;
                                    }
                                }
                            }

                        }
                    }
                }

                FormImageView FrmImageView = new FormImageView(ImageDisplay);
                FrmImageView.Show();
            }
            catch (Exception Desc)
            {

            }

            sw_TaktTime.Stop();
            Logger.WriteLog(LOG.Inspection, "Takt Time : {0} ms", sw_TaktTime.ElapsedMilliseconds);
        }

        private void btnTopEdgeROI_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void OnRoiEnd(object sender, RectEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnRoiEnd(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {
                    //Test 
                    switch (e.Mode)
                    {
                        case "ROI_TOP_EDGE":
                            Property_LineGuage_Top.ROI = new Rectangle(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                            break;
                        case "ROI_TUBING_EDGE":
                            Property_LineGuage_Tubing.ROI = new Rectangle(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                            break;
                        case "ROI_MASKING":
                            Property_Matching_Masking.InspectROI = new Rectangle(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                            break;
                        case "TRAIN_MASKING":
                            Rect rtMasking = new Rect(e.Rect.X, e.Rect.Y, e.Rect.Width, e.Rect.Height);
                            Property_Matching_Masking.ImageTemplate = ImageSource.SubMat(rtMasking).Clone();

                            try
                            {
                                using (Mat ImageBin = ImageSource.Clone())
                                {
                                    if (ImageBin.Channels() == 3) Cv2.CvtColor(ImageBin, ImageBin, ColorConversionCodes.RGB2GRAY);
                                    Cv2.AdaptiveThreshold(ImageBin, ImageBin, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 25, 5);
                                    Property_Matching_Masking.ImageTemplate = ImageBin.SubMat(rtMasking).Clone();

                                    ibMaskingTemplate.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Property_Matching_Masking.ImageTemplate);
                                    ibMaskingTemplate.ZoomToFit();
                                }
                            }
                            catch (Exception Desc)
                            {
                                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                            }
                            
                            break;
                    }

                    (sender as FormSettings_MROI).EventRoiEnd -= OnRoiEnd;
                    (sender as FormSettings_MROI).Close();

                    propertygrid_Parameter.Refresh();
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        private void btnImageLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string strImagePath = CUtil.LoadImage();

                ImageSource = Cv2.ImRead(strImagePath);
                ibSource.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(ImageSource.Clone());
                ibSource.ZoomToFit();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void ibSource_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (sender is ImageGlass.ImageBox)
                {
                    FormImageView FrmImageViewer = new FormImageView((Bitmap)(sender as ImageGlass.ImageBox).Image);
                    FrmImageViewer.Show();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnTubingEdgeROI_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnMaskingTrain_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnMaskingROI_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void trbThreshold_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                int nThreshold = trbThreshold.Value;

                using (Mat ImageBin = ImageSource.Clone())
                {
                    if (ImageBin.Channels() == 3) Cv2.CvtColor(ImageBin, ImageBin, ColorConversionCodes.RGB2GRAY);
                    Cv2.Threshold(ImageBin, ImageBin, nThreshold, 255, ThresholdTypes.Binary);

                    ibSource.Image = (Image)OpenCvSharp.Extensions.BitmapConverter.ToBitmap(ImageBin).Clone();
                }
                
                lbThreshold.Text = nThreshold.ToString();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (Mat ImageBin = ImageSource.Clone())
                {
                    if (ImageBin.Channels() == 3) Cv2.CvtColor(ImageBin, ImageBin, ColorConversionCodes.RGB2GRAY);
                    Cv2.AdaptiveThreshold(ImageBin, ImageBin, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 25, 5);

                    ibSource.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(ImageBin.Clone());
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
    }
}

#endif