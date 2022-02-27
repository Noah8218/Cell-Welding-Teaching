#if MATROX
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Matrox.MatroxImagingLibrary;
using OpenCvSharp;

namespace IntelligentFactory
{
    public partial class CIVT_LineGuage
    {
        public CPropertyLineGuage Property { get; set; } = new CPropertyLineGuage("PROPERTY");

        public List<CIVT_LineGuage_Result> Results { get; set; } = new List<CIVT_LineGuage_Result>();
        public string ToolName { get; set; } = "";
        public Mat ImageOriginal { get; set; } = new Mat();
        public Mat ImageSource1 { get; set; } = new Mat();
        public Mat ImageSource2 { get; set; } = new Mat();
        public Mat ImageResult { get; set; } = new Mat();

        public Line FittedLine = new Line();
        public Stopwatch sw_TaktTimems { get; set; } = new Stopwatch();

        private string m_XMLName = "LINE_GUAGE";
        private string curToolName = "EMPTY";

        public CIVT_LineGuage(string ToolName)
        {
            curToolName = ToolName;
            m_XMLName = m_XMLName + curToolName;
        }

        public bool SetSourceImage(Mat Image)
        {
            try
            {
                ImageSource1 = Image.Clone();
                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

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

                ImageSource1 = OpenCvSharp.Extensions.BitmapConverter.ToMat(ImageIsp).Clone();
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }

        public bool SetSourceImage(Bitmap Image)
        {
            try
            {
                ImageSource1 = OpenCvSharp.Extensions.BitmapConverter.ToMat(Image).Clone();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private object lockList = new object();
        public bool Run(bool bRender = false)
        {
            try
            {
                sw_TaktTimems.Restart();
                Results.Clear();

                List<OpenCvSharp.Point> Points = new List<OpenCvSharp.Point>();

                using (Mat ImageOrg = ImageSource1.Clone())
                {
                    Mat ImageSrc = new Mat();

                    if (IVision.IsMatEmpty(ImageOrg))
                    {
                        Logger.WriteLog(LOG.AbNormal, "Image is Empty");
                        return false;
                    }

                    if (Property.CVROI.Width == 0 || Property.CVROI.Height == 0)
                    {
                        Logger.WriteLog(LOG.AbNormal, "ROI is Empty");
                        return false;
                    }

                    ImageSrc = ImageOrg.SubMat(Property.CVROI).Clone();
                    ImageResult = ImageSrc.Clone();

                    if (ImageSrc.Channels() != 1) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGBA2GRAY);
                    if (ImageResult.Channels() == 1) Cv2.CvtColor(ImageResult, ImageResult, ColorConversionCodes.GRAY2RGB);

                    int nRoiWidthX = Property.CVROI.X + Property.CVROI.Width;

                    #region THRESHOLD
                    if (Property.USE_THRESHOLD)
                    {
                        if (Property.USE_THRESHOLD_INV) Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.BinaryInv);
                        else Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.Binary);
                    }
                    #endregion

                    #region EQUALIZE HISTOGRAM
                    if (Property.USE_EQUALIZE_HIST)
                    {
                        Cv2.EqualizeHist(ImageSrc, ImageSrc);
                    }
                    #endregion

                    #region MORP
                    if (Property.USE_MORP)
                    {
                        if (Property.MORP_OPEN != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_OPEN, Property.MORP_OPEN));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Open, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_CLOSE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_CLOSE, Property.MORP_CLOSE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Close, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_DILATE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_DILATE, Property.MORP_DILATE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Dilate, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_ERODE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_ERODE, Property.MORP_ERODE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Erode, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }
                    }

                    if (bRender) Cv2.ImShow("PROCESS", ImageSrc.Resize(new OpenCvSharp.Size(1024, 1024)));

                    #endregion


                    int nStep = ImageSrc.Height / Property.SAMPLING_STEP;
                    uint nSum = 0;

                    for (int ny = 0; ny < ImageSrc.Rows; ny += nStep)
                    {
                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_LTOR)
                        {
                            for (int nx = 0; nx < ImageSrc.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx - 1);

                                    if (nx + Property.THICKNESS < ImageSrc.Cols - 1)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx + k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    //if(nx == 0 || ny == 0 || nx > nRoiWidthX)
                                                    //{
                                                    //    break;
                                                    //}                                                    

                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));
                                                    nSum += (uint)nx;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_RTOL)
                        {
                            for (int nx = ImageSrc.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < ImageSrc.Cols - 1)
                                {
                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx + 1);

                                    if (nx - Property.THICKNESS > 0)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx - k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    if (nx == 0 || ny == 0 || nx > nRoiWidthX)
                                                    {
                                                        break;
                                                    }

                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));

                                                    nSum += (uint)nx;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Results.Count == 0)
                    {
                        //Cv2.ImShow(DateTime.Now.ToString("HHmmsss"), ImageSrc.Resize(new OpenCvSharp.Size(1024, 1024)));
                        return false;
                    }

                    dAverageX = nSum / (double)Results.Count;

                    if (Property.USE_FITTING)
                    {
                        //List<OpenCvSharp.Point> PointsRansac = IVision.RansacLineFittingInt(Points, out double a, out double b);
                        //PointsRansac = PointsRansac.OrderBy(p => p.Y).ToList();

                        if (Points.Count >= 2)
                        {
                            FittedLine = new Line(Points[0], Points[Points.Count - 1]);
                        }
                    }

                    //var result = System.Threading.Tasks.Parallel.For(0, ImageSrc.Rows, (ny, state) =>
                    //{

                    //});


                    sw_TaktTimems.Stop();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

            return true;
        }

        public bool Run(List<Rect> Masking, bool bRender = false)
        {
            try
            {
                sw_TaktTimems.Restart();
                Results.Clear();

                List<OpenCvSharp.Point> Points = new List<OpenCvSharp.Point>();
                List<Rect> Maskings = new List<Rect>();

                using (Mat ImageOrg = ImageSource1.Clone())
                {
                    Mat ImageSrc = new Mat();

                    if (IVision.IsMatEmpty(ImageOrg))
                    {
                        Logger.WriteLog(LOG.AbNormal, "Image is Empty");
                        return false;
                    }

                    if (Property.CVROI.Width == 0 || Property.CVROI.Height == 0)
                    {
                        Logger.WriteLog(LOG.AbNormal, "ROI is Empty");
                        return false;
                    }

                    //Rect rtROI = new Rect(Property.CVROI.X, 750, Property.CVROI.Width, 2750);

                    ImageSrc = ImageOrg.SubMat(Property.CVROI).Clone();
                    //ImageSrc = ImageOrg.SubMat(rtROI).Clone();
                    ImageResult = ImageSrc.Clone();

                    for (int i = 0; i < Masking.Count; i++)
                    {
                        Maskings.Add(new Rect(Masking[i].X - Property.CVROI.X, Masking[i].Y - Property.CVROI.Y, Masking[i].Width, Masking[i].Height));
                    }

                    if (ImageSrc.Channels() != 1) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGBA2GRAY);
                    if (ImageResult.Channels() == 1) Cv2.CvtColor(ImageResult, ImageResult, ColorConversionCodes.GRAY2RGB);

                    #region THRESHOLD
                    if (Property.USE_THRESHOLD)
                    {
                        if (Property.USE_THRESHOLD_INV) Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.BinaryInv);
                        else Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.Binary);
                    }
                    #endregion

                    #region MORP
                    if (Property.USE_MORP)
                    {
                        if (Property.MORP_OPEN != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_OPEN, Property.MORP_OPEN));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Open, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_CLOSE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_CLOSE, Property.MORP_CLOSE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Close, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_DILATE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_DILATE, Property.MORP_DILATE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Dilate, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_ERODE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_ERODE, Property.MORP_ERODE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Erode, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }
                    }

                    if (bRender) Cv2.ImShow("PROCESS", ImageSrc.Resize(new OpenCvSharp.Size(1024, 1024)));

                    #endregion

                    int nStep = ImageSrc.Height / Property.SAMPLING_STEP;
                    uint nSum = 0;


                    for (int ny = 0; ny < ImageSrc.Rows; ny += nStep)
                    {
                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_LTOR)
                        {
                            for (int nx = 0; nx < ImageSrc.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    bool bMaskingContain = false;
                                    for (int i = 0; i < Maskings.Count; i++)
                                    {
                                        OpenCvSharp.Point ptIndex = new OpenCvSharp.Point(nx, ny);

                                        if (Maskings[i].Y <= ny
                                            && Maskings[i].Y + Maskings[i].Height >= ny)
                                        {
                                            bMaskingContain = true;
                                        }
                                        //if (Maskings[i].Contains(ptIndex)) bMaskingContain = true;
                                    }

                                    if (bMaskingContain) continue;

                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx - 1);

                                    if (nx + Property.THICKNESS < ImageSrc.Cols - 1)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx + k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));
                                                    nSum += (uint)nx;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_RTOL)
                        {
                            for (int nx = ImageSrc.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < ImageSrc.Cols - 1)
                                {
                                    bool bMaskingContain = false;
                                    for (int i = 0; i < Maskings.Count; i++)
                                    {
                                        OpenCvSharp.Point ptIndex = new OpenCvSharp.Point(nx, ny);
                                        if (Maskings[i].Y <= ny
                                           && Maskings[i].Y + Maskings[i].Height >= ny)
                                        {
                                            bMaskingContain = true;
                                        }
                                    }

                                    if (bMaskingContain) continue;

                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx + 1);

                                    if (nx - Property.THICKNESS > 0)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx - k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));

                                                    nSum += (uint)nx;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    dAverageX = nSum / (double)Results.Count;

                    if (Property.USE_FITTING)
                    {
                        List<OpenCvSharp.Point> PointsRansac = IVision.RansacLineFittingInt(Points, out double a, out double b);
                        PointsRansac = PointsRansac.OrderBy(p => p.Y).ToList();

                        if (PointsRansac.Count >= 2)
                        {
                            FittedLine = new Line(PointsRansac[0], PointsRansac[PointsRansac.Count - 1]);
                        }
                    }

                    //var result = System.Threading.Tasks.Parallel.For(0, ImageSrc.Rows, (ny, state) =>
                    //{

                    //});


                    sw_TaktTimems.Stop();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

            return true;
        }

        public bool RunSampling(List<Rect> Masking, out List<List<OpenCvSharp.Point>> SamplingResults, bool bRender = false)
        {
            SamplingResults = new List<List<OpenCvSharp.Point>>();

            try
            {
                sw_TaktTimems.Restart();
                Results.Clear();

                List<OpenCvSharp.Point> Points = new List<OpenCvSharp.Point>();
                List<Rect> Maskings = new List<Rect>();

                using (Mat ImageOrg = ImageSource1.Clone())
                {
                    Mat ImageSrc = new Mat();

                    if (IVision.IsMatEmpty(ImageOrg))
                    {
                        Logger.WriteLog(LOG.AbNormal, "Image is Empty");
                        return false;
                    }

                    if (Property.CVROI.Width == 0 || Property.CVROI.Height == 0)
                    {
                        Logger.WriteLog(LOG.AbNormal, "ROI is Empty");
                        return false;
                    }

                    ImageSrc = ImageOrg.SubMat(Property.CVROI).Clone();
                    ImageResult = ImageSrc.Clone();

                    for (int i = 0; i < Masking.Count; i++)
                    {
                        Maskings.Add(new Rect(Masking[i].X - Property.CVROI.X, Masking[i].Y - Property.CVROI.Y, Masking[i].Width, Masking[i].Height));
                    }

                    if (ImageSrc.Channels() != 1) Cv2.CvtColor(ImageSrc, ImageSrc, ColorConversionCodes.RGBA2GRAY);
                    if (ImageResult.Channels() == 1) Cv2.CvtColor(ImageResult, ImageResult, ColorConversionCodes.GRAY2RGB);

                    #region THRESHOLD
                    if (Property.USE_THRESHOLD)
                    {
                        if (Property.USE_THRESHOLD_INV) Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.BinaryInv);
                        else Cv2.Threshold(ImageSrc, ImageSrc, Property.THRESHOLD, 255, ThresholdTypes.Binary);
                    }
                    #endregion

                    #region MORP
                    if (Property.USE_MORP)
                    {
                        if (Property.MORP_OPEN != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_OPEN, Property.MORP_OPEN));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Open, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_CLOSE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_CLOSE, Property.MORP_CLOSE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Close, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_DILATE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_DILATE, Property.MORP_DILATE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Dilate, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }

                        if (Property.MORP_ERODE != 0)
                        {
                            Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(Property.MORP_ERODE, Property.MORP_ERODE));
                            Cv2.MorphologyEx(ImageSrc, ImageSrc, MorphTypes.Erode, Kernel, new OpenCvSharp.Point(-1, -1), 1);
                        }
                    }

                    if (bRender) Cv2.ImShow("PROCESS", ImageSrc.Resize(new OpenCvSharp.Size(1024, 1024)));

                    #endregion

                    int nStep = ImageSrc.Height / Property.SAMPLING_STEP;
                    uint nSum = 0;

                    for (int i = 0; i < Property.SAMPLING_CYCLE; i++) SamplingResults.Add(new List<OpenCvSharp.Point>());

                    for (int ny = 0; ny < ImageSrc.Rows; ny += nStep)
                    {
                        int nListIndex = ny / nStep * Property.SAMPLING_CYCLE;

                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_LTOR)
                        {
                            for (int nx = 0; nx < ImageSrc.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    bool bMaskingContain = false;
                                    for (int i = 0; i < Maskings.Count; i++)
                                    {
                                        OpenCvSharp.Point ptIndex = new OpenCvSharp.Point(nx, ny);
                                        if (Maskings[i].Contains(ptIndex)) bMaskingContain = true;
                                    }

                                    if (bMaskingContain) continue;

                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx - 1);

                                    if (nx + Property.THICKNESS < ImageSrc.Cols - 1)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx + k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));
                                                    nSum += (uint)nx;

                                                    SamplingResults[nListIndex].Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Property.PRJ_DIR == CPropertyLineGuage.PROJECTION_DIR.X_RTOL)
                        {
                            for (int nx = ImageSrc.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < ImageSrc.Cols - 1)
                                {
                                    bool bMaskingContain = false;
                                    for (int i = 0; i < Maskings.Count; i++)
                                    {
                                        OpenCvSharp.Point ptIndex = new OpenCvSharp.Point(nx, ny);
                                        if (Maskings[i].Contains(ptIndex)) bMaskingContain = true;
                                    }

                                    if (bMaskingContain) continue;

                                    int nGV_Curr = ImageSrc.At<byte>(ny, nx);
                                    int nGV_Prev = ImageSrc.At<byte>(ny, nx + 1);

                                    if (nx - Property.THICKNESS > 0)
                                    {
                                        bool bEdge = false;

                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_Curr) < -Property.CONTRAST) bEdge = true;
                                        if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_Curr) > Property.CONTRAST) bEdge = true;

                                        if (bEdge)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= Property.THICKNESS; k++)
                                            {
                                                int nGV_T = ImageSrc.At<byte>(ny, nx - k);

                                                bool bEdge_T = false;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.BTOW && (nGV_Prev - nGV_T) < -Property.CONTRAST) bEdge_T = true;
                                                if (Property.PRJ_PORALITY == CPropertyLineGuage.PROJECTION_POLARITY.WTOB && (nGV_Prev - nGV_T) > Property.CONTRAST) bEdge_T = true;

                                                if (bEdge_T)
                                                {

                                                }
                                                else
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                lock (lockList)
                                                {
                                                    Results.Add(new CIVT_LineGuage_Result(new System.Drawing.Point(nx, ny)));
                                                    Points.Add(new OpenCvSharp.Point(nx, ny));
                                                    nSum += (uint)nx;

                                                    nSum += (uint)nx; SamplingResults[nListIndex].Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    dAverageX = nSum / (double)Results.Count;

                    if (Property.USE_FITTING)
                    {
                        List<OpenCvSharp.Point> PointsRansac = IVision.RansacLineFittingInt(Points, out double a, out double b);
                        PointsRansac = PointsRansac.OrderBy(p => p.Y).ToList();

                        if (PointsRansac.Count >= 2)
                        {
                            FittedLine = new Line(PointsRansac[0], PointsRansac[PointsRansac.Count - 1]);
                        }
                    }

                    sw_TaktTimems.Stop();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[ERROR] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

            return true;
        }

        public double dAverageX = 0;
        public bool SetProperty(CPropertyLineGuage property)
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
    }

    public class CIVT_LineGuage_Result
    {
        public System.Drawing.Point MeasPos { get; set; } = new System.Drawing.Point();

        public CIVT_LineGuage_Result(System.Drawing.Point ptMeasPos)
        {
            MeasPos = ptMeasPos;
        }
    }
}

#endif