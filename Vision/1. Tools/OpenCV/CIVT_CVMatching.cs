﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

#if MATROX
using Matrox.MatroxImagingLibrary; 
#endif

using OpenCvSharp;
using OpenCvSharp.UserInterface;

namespace IntelligentFactory
{
    public partial class CIVT_CVMatching
    {
        public CPropertyMatching Property = new CPropertyMatching("DEFAULT");

        #region PROPERTIES
        public List<CResultMatching> Results = new List<CResultMatching>();
        public string ToolName = "EMPTY";
        public Mat ImageSource { get; set; } = null;
        public Mat ImageTemplate { get; set; } = null;
        public Mat ImageResult { get; set; } = null;
        public Mat ImageFinal { get; set; } = null;
        public int Threshold { get; set; } = 100;
        public int TimeOut { get; set; } = 30000;

        public bool UseROI { get; set; } = false;
        public bool UseInv { get; set; } = false;
        public bool UseMultiMatching { get; set; } = false;
        public Rect ROI { get; set; } = new Rect();

        private Rect m_rt = new Rect();
        public Rect rt
        {
            get { return m_rt; }
            set { m_rt = value; }
        }

        public string Algorism { get; set; } = "";
        #endregion

        public Stopwatch sw_TaktTimems = new Stopwatch();


        public CIVT_CVMatching(string strAlgorism)
        {
            this.Algorism = strAlgorism;
        }

#if MATROX
        public bool SetSourceImage(MIL_ID image)
        {
            Bitmap bmp = new Bitmap(4096, 4000, PixelFormat.Format8bppIndexed);

            try
            {

                int nSTART = Environment.TickCount;
                byte[] buff = new byte[4096 * 4000];

                MIL.MbufGet(image, buff); // MilImage-> MIL 이미지 ,  UserBuffer -> Array 버퍼
                bmp = CConverter.ByteToBitmap(buff, 4096, 4000);

                Bitmap ImageIsp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                ImageSource = OpenCvSharp.Extensions.BitmapConverter.ToMat(ImageIsp);
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        } 
#endif


        public bool SetSourceImage(System.Drawing.Bitmap Image)
        {
            try
            {                
                ImageSource = OpenCvSharp.Extensions.BitmapConverter.ToMat(Image).Clone();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        public bool SetSourceImage(Mat Image)
        {
            try
            {
                ImageSource = Image.Clone();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        public bool SetTemplateImage(Mat Image)
        {
            try
            {
                Property.ImageTemplate = Image.Clone();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        public bool SetProperty(CPropertyMatching property)
        {
            try
            {
                Property = property;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        public bool Run()
        {
            try
            {
                sw_TaktTimems.Restart();

                Results.Clear();

                Mat ImageMatching = new Mat();

                if(Property.TrainROI.Width == 0 || Property.TrainROI.Height == 0)
                {
                    Property.TrainROI = new System.Drawing.Rectangle(0, 0, ImageSource.Width, ImageSource.Height);
                }

                ImageTemplate = Property.ImageTemplate.Clone();


                if(IVision.IsMatEmpty(ImageSource))
                {
                    Logger.WriteLog(LOG.AbNormal, "Image is Empty");
                    return false;
                }

                using (Mat ImageSubMat = ImageSource.SubMat(CConverter.RectToCVRect(Property.TrainROI)).Clone())
                using (Mat ImageSrc = ImageSource.Resize(new OpenCvSharp.Size((int)(ImageSource.Width / Property.MAGNIFIATION), (int)(ImageSource.Height / Property.MAGNIFIATION))))
                using (Mat ImageTpl = ImageTemplate.Resize(new OpenCvSharp.Size((int)(ImageTemplate.Width / Property.MAGNIFIATION), (int)(ImageTemplate.Height / Property.MAGNIFIATION))))
                {
                    ImageResult = ImageSrc.Clone();

                    if (IVision.IsMatEmpty(ImageTemplate)
                           || IVision.IsMatEmpty(ImageSource)) return false;

                    double dScore = 0;

                    if (ImageResult.Channels() == 1) Cv2.CvtColor(ImageResult, ImageResult, ColorConversionCodes.GRAY2RGB);
                    if (ImageTemplate.Channels() == 4) Cv2.CvtColor(ImageTpl, ImageTpl, ColorConversionCodes.RGBA2GRAY);
                    if (ImageTemplate.Channels() == 3) Cv2.CvtColor(ImageTpl, ImageTpl, ColorConversionCodes.RGB2GRAY);
                    if (ImageSrc.Channels() == 4) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGBA2GRAY);
                    if (ImageSrc.Channels() == 3) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGB2GRAY);

                    OpenCvSharp.Point ptMax = new OpenCvSharp.Point();
                    double dMax = 0.0D;
                    bool bFind = false;

                    if (UseROI) { }

                    int nTimeOutCheck = Environment.TickCount;

                    while (true)
                    {
                        if ((Environment.TickCount - nTimeOutCheck) > TimeOut) return false;

                        Cv2.MatchTemplate(ImageSrc, ImageTpl, ImageMatching, TemplateMatchModes.CCoeffNormed, null);
                        Cv2.MinMaxLoc(ImageMatching, out double dMinScore, out double dMaxScore, out OpenCvSharp.Point ptMinLocation, out OpenCvSharp.Point ptMaxLocation);

                        if (dMaxScore > Property.SCORE_MIN)
                        {
                            ptMax = (dMaxScore > dMax) ? ptMaxLocation : ptMax;
                            dMax = (dMaxScore > dMax) ? dMaxScore : dMax;

                            dScore = dMaxScore * 100.0D;

                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(ptMaxLocation.X, ptMaxLocation.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(ptMaxLocation.X + (ImageTpl.Width), ptMaxLocation.Y + (ImageTpl.Height));

                            if (UseROI)
                            {
                                ptStart = new OpenCvSharp.Point(ptMax.X + ROI.X, ptMax.Y + ROI.Y);
                                ptEnd = new OpenCvSharp.Point(ptStart.X + (ROI.Width), ptStart.Y + (ROI.Height));
                            }

                            Rect rt = new Rect(new OpenCvSharp.Point(ptMaxLocation.X - 5, ptMaxLocation.Y - 5), new OpenCvSharp.Size(ImageTpl.Width + 10, ImageTpl.Height + 10));
                            ImageSrc.Rectangle(rt, Scalar.Black, -1);
                            bFind = true;

                            Rect rtBounding = new Rect(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);
                            OpenCvSharp.Point ptCenter = new OpenCvSharp.Point(ptStart.X + rtBounding.Width / 2, ptStart.Y + rtBounding.Height / 2);
                            Cv2.Rectangle(ImageResult, ptStart, ptEnd, Scalar.Aquamarine, 5, LineTypes.AntiAlias);
                            Cv2.PutText(ImageResult, string.Format("Score : {0} %", dScore.ToString("F2")), ptCenter, HersheyFonts.HersheyTriplex, 1.0D, Scalar.Aquamarine, 2, LineTypes.AntiAlias);

                            Results.Add(new CResultMatching(0, dScore, ptCenter, rtBounding));

                            if(Property.NUM_MATCH >= Results.Count)
                            {
                                break;
                            }

                            //Cv2.ImShow(DateTime.Now.ToString("hh:mm:ss"), ImageSrc);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                sw_TaktTimems.Stop();


                if (!ImageMatching.IsDisposed && ImageMatching.IsEnabledDispose) ImageMatching.Dispose();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }
    }

    public class CResultMatching
    {
        public int Index { get; set; } = 0;
        public double Score { get; set; } = 0.0D;
        public double Angle { get; set; } = 0.0D;
        public OpenCvSharp.Point Center { get; set; } = new OpenCvSharp.Point();
        public Rect Bounding { get; set; } = new Rect();

        public CResultMatching(int nIndex, double dScore, OpenCvSharp.Point ptCenter, Rect rt, double dAngle = 0.0D)
        {
            Index = nIndex;
            Score = dScore;
            Center = new OpenCvSharp.Point(ptCenter.X, ptCenter.Y);
            Bounding = new Rect(rt.X, rt.Y, rt.Width, rt.Height);
            Angle = dAngle;
        }
    }
}

