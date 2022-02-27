using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.CodeDom;
using System.Data.SqlTypes;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.UserInterface;

#if EURESYS
using Euresys.Open_eVision_2_14;
#endif

namespace IntelligentFactory
{
    public class IVision
    {
#region Base Algorithm
        public static int Contour(Mat ImageOriginal, int nMinArea, int nMaxArea, out Mat ImageDraw)
        {
            Mat ImageSource = new Mat();
            Mat ImageTemp = new Mat();

            ImageDraw = new Mat();

            if (IsMatEmpty(ImageOriginal)) return 0;

            int nContourConut = 0;

            try
            {
                OpenCvSharp.Point testpoint = new OpenCvSharp.Point();
                OpenCvSharp.Point[][] contours;

                Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(3, 3));

                HierarchyIndex[] hierarchy;

                Mat ImageCovert = ImageOriginal.Clone();
                ImageDraw = ImageOriginal.Clone();

                if (ImageDraw.Channels() == 4)
                {
                    Cv2.CvtColor(ImageDraw, ImageDraw, ColorConversionCodes.BGRA2BGR);
                }


                if (ImageCovert.Channels() != 1)
                {
                    Cv2.CvtColor(ImageCovert, ImageSource, ColorConversionCodes.BGR2GRAY);
                }
                else
                {
                    ImageSource = ImageOriginal.Clone();
                }

                ImageDraw.CopyTo(ImageTemp);

                Cv2.FindContours(ImageSource, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple, null);

                nContourConut = contours.Length;

                Parallel.For(0, contours.Length,
                   i =>
                   {
                       double peri = Cv2.ArcLength(contours[i], true);

                       OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.02 * peri, true);
                       Rect rt = Cv2.BoundingRect(pp);

                       if (rt.Width * rt.Height > nMinArea && rt.Width * rt.Height < nMaxArea)
                       {
                           RotatedRect rrect = Cv2.MinAreaRect(pp);
                           double areaRatio = Math.Abs(Cv2.ContourArea(contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);

                           Cv2.DrawContours(ImageTemp, contours, i, Scalar.Yellow, 2, LineTypes.Link4, hierarchy, 100, testpoint);
                           Cv2.PutText(ImageTemp, string.Format("Length : {0}", rt.Width * rt.Height), new OpenCvSharp.Point((int)rrect.Center.X, (int)rrect.Center.Y), HersheyFonts.HersheySimplex, 0.5D, new Scalar(125, 125, 255), 3);
                           Cv2.Rectangle(ImageTemp, rt, Scalar.Blue, 2);

                           for (int j = 0; j < pp.Length; j++)
                           {
                               Cv2.Circle(ImageTemp, new OpenCvSharp.Point(pp[j].X, pp[j].Y), 10, new Scalar(40, 40, 40), 10);
                               //Cv2.Circle(ImageTemp, new OpenCvSharp.Point(pp[j].X, pp[j].Y), 3, new Scalar(0, 0, 255), 2);
                           }
                       }
                   });

                ImageTemp.CopyTo(ImageDraw);
            }
            catch (Exception Desc)
            {
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }

            //if (!ImageSource.IsDisposed && ImageSource.IsEnabledDispose) ImageSource.Dispose();
            if (!ImageTemp.IsDisposed && ImageTemp.IsEnabledDispose) ImageTemp.Dispose();

            return nContourConut;
        }

        public static OpenCvSharp.Point[] ContourLargest(Mat ImageOriginal, int nMinArea, int nMaxArea, out Mat ImageDraw)
        {
            OpenCvSharp.Point[] ppLargest = null;

            Mat ImageSource = new Mat();
            Mat ImageTemp = new Mat();

            ImageDraw = new Mat();

            if (IsMatEmpty(ImageOriginal)) return ppLargest;

            int nContourConut = 0;

            try
            {
                OpenCvSharp.Point testpoint = new OpenCvSharp.Point();
                OpenCvSharp.Point[][] contours;

                Mat Kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(3, 3));

                HierarchyIndex[] hierarchy;

                Mat ImageCovert = ImageOriginal.Clone();
                ImageDraw = ImageOriginal.Clone();

                if (ImageDraw.Channels() == 4)
                {
                    Cv2.CvtColor(ImageDraw, ImageDraw, ColorConversionCodes.BGRA2BGR);
                }


                if (ImageCovert.Channels() != 1)
                {
                    Cv2.CvtColor(ImageCovert, ImageSource, ColorConversionCodes.BGR2GRAY);
                }
                else
                {
                    ImageSource = ImageOriginal.Clone();
                }

                ImageDraw.CopyTo(ImageTemp);

                Cv2.FindContours(ImageSource, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple, null);

                nContourConut = contours.Length;

                

                for (int i = 0; i < contours.Length; i++)
                {
                    double peri = Cv2.ArcLength(contours[i], true);

                    OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.0005 * peri, true);
                    Rect rt = Cv2.BoundingRect(pp);

                    if(ppLargest == null)
                    {
                        ppLargest = pp;
                    }
                    else
                    {
                        if(Cv2.ContourArea(ppLargest) < Cv2.ContourArea(pp))
                        {
                            ppLargest = pp;
                        }
                    }                    

                    if (rt.Width * rt.Height > nMinArea && rt.Width * rt.Height < nMaxArea)
                    {
                        RotatedRect rrect = Cv2.MinAreaRect(pp);
                        double areaRatio = Math.Abs(Cv2.ContourArea(contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);

                        Cv2.DrawContours(ImageTemp, contours, i, Scalar.Yellow, 2, LineTypes.Link4, hierarchy, 100, testpoint);
                        Cv2.PutText(ImageTemp, string.Format("Length : {0}", rt.Width * rt.Height), new OpenCvSharp.Point((int)rrect.Center.X, (int)rrect.Center.Y), HersheyFonts.HersheySimplex, 0.5D, new Scalar(125, 125, 255), 3);
                        Cv2.Rectangle(ImageTemp, rt, Scalar.Blue, 2);

                        for (int j = 0; j < pp.Length; j++)
                        {
                            Cv2.Circle(ImageTemp, new OpenCvSharp.Point(pp[j].X, pp[j].Y), 10, new Scalar(40, 40, 40), 10);
                        }
                    }
                }

                ImageTemp.CopyTo(ImageDraw);
            }
            catch (Exception Desc)
            {
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }

            //if (!ImageSource.IsDisposed && ImageSource.IsEnabledDispose) ImageSource.Dispose();
            if (!ImageTemp.IsDisposed && ImageTemp.IsEnabledDispose) ImageTemp.Dispose();

            return ppLargest;
        }

        public static double Mean(Mat ImageOriginal)
        {
            if (IsMatEmpty(ImageOriginal)) return 0.0D;
            double dAverage = 0.0D;

            try
            {
                dAverage = Cv2.Mean(ImageOriginal).Val0;
            }
            catch (Exception Desc)
            {
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }

            return dAverage;
        }

        public static double Matching(Mat ImageOriginal, Mat ImageTemplate, Rect rtROI, double dMinimumFactor, out Mat ImageDraw, int nTimeOut = 10000)
        {
            Mat ImageSource = new Mat();
            Mat ImageResult = new Mat();

            ImageDraw = ImageOriginal.Clone();

            double dScore = 0;

            if (IsMatEmpty(ImageOriginal)) return 0;
            if (IsMatEmpty(ImageTemplate)) return 0;

            try
            {
                OpenCvSharp.Point ptMax = new OpenCvSharp.Point();
                double dMax = 0.0D;
                bool bFind = false;

                bool bUseROI = !(rtROI.Width == 0 || rtROI.Height == 0);

                try
                {
                    if (bUseROI)
                    {
                        ImageSource = ImageOriginal.SubMat(rtROI);
                    }
                    else
                    {
                        ImageSource = ImageOriginal.Clone();
                    }

                    if (ImageTemplate.Channels() != 1)
                    {
                        Cv2.CvtColor(ImageTemplate, ImageTemplate, ColorConversionCodes.BGR2GRAY);
                    }

                    Cv2.MatchTemplate(ImageSource, ImageTemplate, ImageResult, TemplateMatchModes.CCoeffNormed, null);

                    int nTimeOutCheck = Environment.TickCount;

                    while (true)
                    {
                        if ((Environment.TickCount - nTimeOutCheck) > nTimeOut) return 0.0D;
                        Cv2.MinMaxLoc(ImageResult, out double minvalue, out double maxvalue, out OpenCvSharp.Point minlocation, out OpenCvSharp.Point maxlocation);

                        if (maxvalue > dMinimumFactor)
                        {
                            ptMax = (maxvalue > dMax) ? maxlocation : ptMax;
                            dMax = (maxvalue > dMax) ? maxvalue : dMax;
                            Cv2.FloodFill(ImageResult, maxlocation, Scalar.Black);

                            bFind = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception Desc)
                {
                    CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
                }

                dScore = dMax * 100.0D;

                OpenCvSharp.Point ptStart = new OpenCvSharp.Point(ptMax.X, ptMax.Y);
                OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(ptMax.X + (ImageTemplate.Width), ptMax.Y + (ImageTemplate.Height));

                if (bUseROI)
                {
                    ptStart = new OpenCvSharp.Point(ptMax.X + rtROI.X, ptMax.Y + rtROI.Y);
                    ptEnd = new OpenCvSharp.Point(ptStart.X + (rtROI.Width), ptStart.Y + (rtROI.Height));
                }

                Cv2.Rectangle(ImageDraw, ptStart, ptEnd, new Scalar(0, 255, 0), 5);
            }
            catch (Exception Desc)
            {
                CUtil.ShowMessageBox("EXCEPTION", Desc.Message, FormMessageBox.MESSAGEBOX_TYPE.OK);
            }

            if (!ImageSource.IsDisposed && ImageSource.IsEnabledDispose) ImageSource.Dispose();
            if (!ImageResult.IsDisposed && ImageResult.IsEnabledDispose) ImageResult.Dispose();

            return dScore;
        }
#endregion

#region Util Algorithm
        public static double FocusValueOfLaplacian(Mat ImageOriginal)
        {
            using (var ImageLaplacian = new Mat())
            {
                Cv2.Laplacian(ImageOriginal, ImageLaplacian, MatType.CV_64FC1);
                Cv2.MeanStdDev(ImageLaplacian, out var mean, out var stddev);

                return stddev.Val0 * stddev.Val0;
            }
        }
#endregion

        public static void UpdateImage(PictureBoxIpl pbDisplay, Mat MatSource)
        {
            if (pbDisplay.InvokeRequired)
            {
                pbDisplay.Invoke(new Action<PictureBoxIpl, Mat>(UpdateImage), pbDisplay, MatSource);
            }
            else
            {

                if (!IVision.IsMatEmpty(MatSource))
                {
                    pbDisplay.ImageIpl = MatSource;
                }
            }
        }

        public static bool MatRelease(Mat MatDispose)
        {
            try
            {
                if (!MatDispose.IsDisposed)
                {
                    for (int i = 0; i < MatDispose.Total(); i++)
                    {
                        MatDispose.Release();
                        if (MatDispose.IsDisposed)
                        {
                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

#if EURESYS
        public static void EImageCopy(EImageBW8 ImageSrc, EImageBW8 ImageDst)
        {
            try
            {
                if (ImageSrc == null) return;
                if (ImageSrc.IsVoid || ImageSrc.Width == 0 || ImageSrc.Height == 0) return;

                ImageDst.SetSize(ImageSrc);
                //ImageSrc.CopyTo(ImageDst);

                EasyImage.Oper(EArithmeticLogicOperation.Copy, ImageSrc, ImageDst);
            }
            catch (EException Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
#endif

        public enum Direction { LeftToRight, RightToLeft, ToptoBottom, BottomToTop }
        public enum PixcelY { IncreaseToPixcelY = 1, DecreaseToPixcelY = -1, ParallelToPixcelY = 0 }

        public enum Position
        {
            Left = 0,
            Right = 1,
            TopLeft = 2,
            TopRight = 3,
            BottomLeft = 4,
            BottomRight = 5
        }

        public static List<OpenCvSharp.Point> m_listLeftInLine = new List<Point>();
        public static List<OpenCvSharp.Point> m_listRightInLine = new List<Point>();
        public static List<OpenCvSharp.Point> m_listTopLeftInLine = new List<Point>();
        public static List<OpenCvSharp.Point> m_listTopRightInLine = new List<Point>();
        public static List<OpenCvSharp.Point> m_listBottomLeftInLine = new List<Point>();
        public static List<OpenCvSharp.Point> m_listBottomRightInLine = new List<Point>();

        public static List<Point> Edge(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, Direction dr)
        {
            List<Point> listContour = new List<Point>();
            Object lockList = new Object();

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

                if (IsMatEmpty(MatMeasure))
                {
                    return null;
                }

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);
                Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);

                switch (dr)
                {
                    case Direction.LeftToRight:
                        var result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Rows, (ny, state) =>
                        {
                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if ((greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                lock (lockList)
                                                {
                                                    listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.RightToLeft:
                        result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Rows, (ny, state) =>
                        {
                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if ((greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                lock (lockObject)
                                                {
                                                    listContour.Add(ptContour);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.ToptoBottom:
                        result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Cols, (nx, state) =>
                        {
                            for (int ny = 0; ny < MatMeasure.Rows; ny++)
                            {
                                if (ny > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.BottomToTop:
                        result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Cols, (nx, state) =>
                        {
                            for (int ny = MatMeasure.Rows; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        Monitor.Exit(listContour);

                        break;
                }
            }
            catch (Exception Desc)
            {
                return null;
            }

            return listContour;
        }

        public static List<Point> EdgeSub(Mat MatMeasure, Rect rtROI, int nThreshold, int nContrast, int nThickness, Direction dr)
        {
            List<Point> listContour = new List<Point>();
            Object lockList = new Object();

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

                if (IsMatEmpty(MatMeasure))
                {
                    return null;
                }

                Mat MatSub = new Mat();
                MatMeasure.SubMat(rtROI).CopyTo(MatSub);

                Cv2.Threshold(MatSub, MatSub, nThreshold, 255, ThresholdTypes.Binary);
                Cv2.MorphologyEx(MatSub, MatSub, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);

                switch (dr)
                {
                    case Direction.LeftToRight:
                        var result = System.Threading.Tasks.Parallel.For(0, MatSub.Rows, (ny, state) =>
                        {
                            for (int nx = 0; nx < MatSub.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatSub.At<Vec3b>(ny, nx);
                                    var ptPrev = MatSub.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatSub.Cols - 1)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatSub.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if ((greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                lock (lockList)
                                                {
                                                    listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.RightToLeft:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Rows, (ny, state) =>
                        {
                            for (int nx = MatSub.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatSub.Cols - 1)
                                {
                                    var pt = MatSub.At<Vec3b>(ny, nx);
                                    var ptPrev = MatSub.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatSub.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if ((greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                lock (lockObject)
                                                {
                                                    listContour.Add(ptContour);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.ToptoBottom:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Cols, (nx, state) =>
                        {
                            for (int ny = 0; ny < MatSub.Rows; ny++)
                            {
                                if (ny > 1)
                                {
                                    var pt = MatSub.At<Vec3b>(ny, nx);
                                    var ptPrev = MatSub.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatSub.Rows)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatSub.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                lock (lockList)
                                                {
                                                    listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.BottomToTop:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Cols, (nx, state) =>
                        {
                            for (int ny = MatSub.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatSub.Rows - 1)
                                {
                                    var pt = MatSub.At<Vec3b>(ny, nx);
                                    var ptPrev = MatSub.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        if ((greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatSub.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                lock (lockList)
                                                {
                                                    listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        //Monitor.Exit(listContour);
                        break;
                }
            }
            catch (Exception Desc)
            {
                return null;
            }

            return listContour;
        }

        public static List<Point> EdgeSub(Mat MatMeasure, Rect rtROI, int nThreshold, int nContrast, int nThickness, Direction dr, int nEdgeColor)
        {
            List<Point> listContour = new List<Point>();
            Object lockList = new Object();

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

                if (IsMatEmpty(MatMeasure))
                {
                    return null;
                }

                Mat MatSub = new Mat();
                MatMeasure.SubMat(rtROI).CopyTo(MatSub);

                //Cv2.Threshold(MatSub, MatSub, nThreshold, 255, ThresholdTypes.Binary);
                //Cv2.MorphologyEx(MatSub, MatSub, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);

                switch (dr)
                {
                    case Direction.LeftToRight:
                        var result = System.Threading.Tasks.Parallel.For(0, MatSub.Rows, (ny, state) =>
                        {
                            for (int nx = 0; nx < MatSub.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    // 8비트를 기반으로 그레이벨류값을 얻어 오기에 
                                    // 3원색이 아닌 1원색으로 그레이벨류값 얻어오기
                                    var greyValue = MatSub.At<byte>(ny, nx);
                                    // 1
                                    var greyValuePrev = MatSub.At<byte>(ny, nx - 1);

                                    //var ptCurrent = MatSub.At<Vec3b>(ny, nx);
                                    //var ptPrev = MatSub.At<Vec3b>(ny, nx - 1);

                                    //int greyValue = (int)((ptCurrent.Item2 * 0.3) + (ptCurrent.Item1 * 0.59) + (ptCurrent.Item0 * 0.11));
                                    //int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatSub.Cols - 1)
                                    {

                                        if (nEdgeColor == 0)
                                        {
                                            if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny, nx + k);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx + k);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }

                                        }
                                        else if (nEdgeColor == 2)
                                        {
                                            if ((greyValuePrev - greyValue) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny, nx + k);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx + k);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if ((greyValuePrev - greyValueT) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        // 블랙 -> 화이트
                                        else if (nEdgeColor == 1)
                                        {
                                            if (-(greyValuePrev - greyValue) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny, nx + k);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx + k);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (-(greyValuePrev - greyValueT) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        });
                        break;
                    case Direction.RightToLeft:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Rows, (ny, state) =>
                        {
                            for (int nx = MatSub.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatSub.Cols - 1)
                                {
                                    // 8비트를 기반으로 그레이벨류값을 얻어 오기에 
                                    // 3원색이 아닌 1원색으로 그레이벨류값 얻어오기
                                    var greyValue = MatSub.At<byte>(ny, nx);
                                    // 1
                                    var greyValuePrev = MatSub.At<byte>(ny, nx - 1);
                                    if (nx - nThickness > 0)
                                    {
                                        // 화이트 -> 블랙
                                        if (nEdgeColor == 0)
                                        {
                                            if (Math.Abs(greyValue - greyValuePrev) > nContrast)
                                            {

                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny, nx - k);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx - k);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValuePrev) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx - 1, ny);
                                                    lock (lockObject)
                                                    {
                                                        listContour.Add(ptContour);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 2)
                                        {
                                            if ((greyValue - greyValuePrev) > nContrast)
                                            {

                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny, nx - k);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx - k);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if ((greyValueT - greyValuePrev) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx - 1, ny);
                                                    lock (lockObject)
                                                    {
                                                        listContour.Add(ptContour);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 1)
                                        {
                                            // 블랙 -> 화이트(높은값) 
                                            // 2463 - 2464 > 30
                                            // 212 - 
                                            if (-(greyValue - greyValuePrev) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny, nx - k + 1);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    var greyValueT = MatSub.At<byte>(ny, nx + k);

                                                //    if (-(greyValueT - greyValuePrev) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx - 1, ny);
                                                    lock (lockObject)
                                                    {
                                                        listContour.Add(ptContour);
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        break;
                    case Direction.ToptoBottom:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Cols, (nx, state) =>
                        {
                            for (int ny = 0; ny < MatSub.Rows; ny++)
                            {
                                if (ny > 1)
                                {

                                    // 0
                                    //var ptCurrent = MatSub.At<Vec3b>(ny, nx);
                                    //// 1
                                    //var ptNext = MatSub.At<Vec3b>(ny + 1, nx);
                                    // 8비트를 기반으로 그레이벨류값을 얻어 오기에 
                                    // 3원색이 아닌 1원색으로 그레이벨류값 얻어오기
                                    var greyValue = MatSub.At<byte>(ny, nx);
                                    // 1
                                    var greyValueNext = MatSub.At<byte>(ny + 1, nx);
                                    if (ny + nThickness < MatSub.Rows)
                                    {
                                        if (nEdgeColor == 0)
                                        {
                                            if (Math.Abs(greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny + k + 1, nx);

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    // 위에서 -> 아래로 가는 부분이기에 검출 데이터는 아래쪽 데이터 
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny + 1);
                                                    lock (lockList)
                                                    {
                                                        //Logger.WriteLog(LOG.SYSTEM, "윗값 Y : {0} - 아래값 Y : {1}", ny, ny + 1);
                                                        //Logger.WriteLog(LOG.SYSTEM, "GreyValue(윗값) - GreyValue(아랫값)  : {0} - {1} : {2}", greyValue, greyValueNext, greyValue - greyValueNext);
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny + 1));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 2)
                                        {
                                            // 화이트 -> 블랙
                                            if ((greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny + k + 1, nx);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny + k + 1, nx);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    // 위에서 -> 아래로 가는 부분이기에 검출 데이터는 아래쪽 데이터 
                                                    //OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny + 1);
                                                    lock (lockList)
                                                    {
                                                        //Logger.WriteLog(LOG.SYSTEM, "윗값 Y : {0} - 아래값 Y : {1}", ny, ny + 1);
                                                        //Logger.WriteLog(LOG.SYSTEM, "GreyValue(윗값) - GreyValue(아랫값)  : {0} - {1} : {2}", greyValue, greyValueNext, greyValue - greyValueNext);
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny + 1));
                                                        //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 1)
                                        {
                                            // 블랙 -> 화이트
                                            // -(0 - 255) > 30
                                            if (-(greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny + k + 1, nx);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny + k + 1, nx);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    // 위에서 -> 아래로 가는 부분이기에 검출 데이터는 아래쪽 데이터 
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny + 1);
                                                    lock (lockList)
                                                    {
                                                        //Logger.WriteLog(LOG.SYSTEM, "윗값 Y : {0} - 아래값 Y : {1}", ny, ny + 1);
                                                        //Logger.WriteLog(LOG.SYSTEM, "GreyValue(윗값) - GreyValue(아랫값)  : {0} - {1} : {2}", greyValue, greyValueNext, greyValue - greyValueNext);
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny + 1));
                                                    }
                                                    //OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                    //lock (lockList)
                                                    //{
                                                    //    listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    //}
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        });
                        break;
                    case Direction.BottomToTop:
                        result = System.Threading.Tasks.Parallel.For(0, MatSub.Cols, (nx, state) =>
                        {
                            for (int ny = MatSub.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatSub.Rows - 1)
                                {
                                    // 8비트를 기반으로 그레이벨류값을 얻어 오기에 
                                    // 3원색이 아닌 1원색으로 그레이벨류값 얻어오기
                                    var greyValue = MatSub.At<byte>(ny, nx);
                                    // 1
                                    var greyValueNext = MatSub.At<byte>(ny - 1, nx);

                                    if (ny - nThickness > 0)
                                    {
                                        if (nEdgeColor == 0)
                                        {
                                            if (Math.Abs(greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny - k - 1, nx);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny - k - 1, nx);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    //OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny );
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny - 1);
                                                    //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny - 1));
                                                        //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 2)
                                        {
                                            // 회이트 -> 블랙
                                            if ((greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny - k - 1, nx);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny - k - 1, nx);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    //OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny );
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny - 1);
                                                    //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny - 1));
                                                        //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        else if (nEdgeColor == 1)
                                        {
                                            // 블랙 -> 화이트
                                            if (-(greyValue - greyValueNext) > nContrast)
                                            {
                                                bool bThickness = true;

                                                //for (int k = 1; k <= nThickness; k++)
                                                //{
                                                //    var greyValueT = MatSub.At<byte>(ny - k - 1, nx);
                                                //    //var ptThickness = MatSub.At<Vec3b>(ny - k - 1, nx);
                                                //    //int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                //    if (Math.Abs(greyValueT - greyValueNext) < nContrast)
                                                //    {
                                                //        bThickness = false;
                                                //        break;
                                                //    }
                                                //}

                                                if (bThickness)
                                                {
                                                    //OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
                                                    OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny - 1);
                                                    //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    lock (lockList)
                                                    {
                                                        listContour.Add(new OpenCvSharp.Point(nx, ny - 1));
                                                        //listContour.Add(new OpenCvSharp.Point(nx, ny));
                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        });
                        //Monitor.Exit(listContour);
                        break;
                }

                if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    listContour = listContour.OrderBy(p => p.Y).ToList();

                }
                else
                {
                    //listContour = listContour.OrderBy(p => p.Y).ToList();
                    listContour = listContour.OrderBy(p => p.X).ToList();
                }

            }
            catch (Exception Desc)
            {
                return null;
            }

            return listContour;
        }

        //public static Point EdgeCorner(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, Direction dr, ref Mat MatDraw)
        //{
        //    List<Point> listContour = new List<Point>();
        //    Object lockList = new Object();
        //    Point ptCorner = new Point(-1, -1);

        //    try
        //    {
        //        Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

        //        if (IsMatEmpty(MatMeasure))
        //        {
        //            return new Point(-1, -1);
        //        }

        //        Masking(dr, ref MatMeasure);

        //        Cv2.Threshold(MatMeasure, MatDraw, nThreshold, 255, ThresholdTypes.Binary);                

        //        switch (dr)
        //        {
        //            case Direction.LeftToRight:
        //                var result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Rows, (ny, state) =>
        //                {
        //                    for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
        //                    {
        //                        if (nx > 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx + nThickness < MatMeasure.Cols - 1)
        //                            {
        //                                if ((greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if ((greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
        //                                        lock (lockList)
        //                                        {
        //                                            listContour.Add(new OpenCvSharp.Point(nx, ny));
        //                                        }
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                });
        //                break;
        //            case Direction.RightToLeft:
        //                result = System.Threading.Tasks.Parallel.For(0, MatMeasure.Rows, (ny, state) =>
        //                {
        //                    for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
        //                    {
        //                        if (nx < MatMeasure.Cols - 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx - nThickness > 0)
        //                            {
        //                                if ((greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if ((greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        OpenCvSharp.Point ptContour = new OpenCvSharp.Point(nx, ny);
        //                                        lock (lockObject)
        //                                        {
        //                                            listContour.Add(ptContour);
        //                                        }
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                });
        //                break;
        //        }
        //    }
        //    catch (Exception Desc)
        //    {
        //        return new Point(-1, -1);
        //    }

        //    try
        //    {
        //        if (listContour.Count > 0)
        //        {
        //            switch (dr)
        //            {
        //                case Direction.LeftToRight:
        //                    listContour = listContour.OrderBy(p => p.X).ToList();
        //                    break;
        //                case Direction.RightToLeft:
        //                    listContour = listContour.OrderByDescending(p => p.X).ToList();

        //                    break;
        //            }

        //            return listContour[0];
        //        }   
        //    }
        //    catch (Exception Desc)
        //    {
        //        return new Point(-1, -1);
        //    }

        //    return ptCorner;
        //}

        public static List<double> GapTest(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatResult, List<Point> listMaskingLine, Rect ROI)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();

            double dSum = 0.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

                if (IsMatEmpty(MatMeasure))
                {
                    Logger.WriteLog(LOG.Normal, "1");                    
                    return null;
                }

                Masking(dr, ref MatMeasure);

                MatMeasure = MatMeasure.SubMat(ROI);

                if (listMaskingLine != null)
                {
                    listMaskingLine = RansacLineFittingInt(listMaskingLine, out double a, out double b);

                    for (int i = 0; i < listMaskingLine.Count; i++)
                    {
                        if (i > 0)
                        {
                            Point ptPrev = listMaskingLine[i - 1];
                            Point ptCurrnt = listMaskingLine[i];

                            double dDistance = ptPrev.DistanceTo(ptCurrnt);

                            if (dDistance > 30)
                            {
                                continue;
                            }
                            else
                            {
                                MatMeasure.Line(ptPrev, ptCurrnt, Scalar.White, 5);
                            }
                        }
                    }
                }

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                if (nEdgeColor == 0)
                {

                }
                else if (nEdgeColor == 1)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }
                else
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }


                switch (dr)
                {
                    case Direction.LeftToRight:
                        listMaskingLine.OrderBy(p => p.X).ToList();

                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (listMaskingLine[0].X > ptSecond.X)
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        listMaskingLine.OrderByDescending(p => p.X).ToList();
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        //if (bFind)
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (listMaskingLine[0].X < ptSecond.X)
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        listMaskingLine.OrderBy(p => p.Y).ToList();
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (listMaskingLine[0].Y > ptSecond.Y)
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        listMaskingLine.OrderByDescending(p => p.Y).ToList();
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (listMaskingLine[0].Y < ptSecond.Y)
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "2");
                Logger.WriteLog(LOG.AbNormal, Desc.Message);                
                return null;
            }

            if (listLine.Count > 0)
            {
                for (int i = 0; i < listLine.Count; i++)
                {
                    MatMeasure.Line(listLine[i].Start, listLine[i].End, Scalar.Aquamarine);
                }
            }

            MatMeasure.CopyTo(MatResult);
            return listGap;
        }

        public static List<double> Gap(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatOverlay, ref Mat MatResult, out double dAngle)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();

            double dSum = 0.0D;
            dAngle = 361.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Rect rt = new Rect();

                switch (dr)
                {
                    case Direction.LeftToRight:
                        rt.X = 0;
                        rt.Y = 0;
                        rt.Width = (int)(MatMeasure.Width * 0.25D);
                        rt.Height = MatMeasure.Height;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.RightToLeft:
                        rt.Width = (int)(MatMeasure.Width * 0.25D);
                        rt.Height = MatMeasure.Height;
                        rt.X = MatMeasure.Width - rt.Width;
                        rt.Y = 0;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.ToptoBottom:
                        rt.X = 0;
                        rt.Y = 0;
                        rt.Width = MatMeasure.Width;
                        rt.Height = (int)(MatMeasure.Height * 0.2D);
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.BottomToTop:
                        rt.Width = MatMeasure.Width;
                        rt.Height = (int)(MatMeasure.Height * 0.2D);
                        rt.X = 0;
                        rt.Y = MatMeasure.Height - rt.Height;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                }



                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                if (nEdgeColor == 0)
                {

                }
                else if (nEdgeColor == 1)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }
                else
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }


                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        //if (bFind)
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) > nContrast)
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
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "2");
                //ILogger.Add(LOG_TYPE.ERROR, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    MatOverlay.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    MatOverlay.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }

                List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
                List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

                if (listFitOut.Count > 1)
                {
                    dAngle = IMath.Angle(new Point(listFitOut[0].X, listFitOut[0].Y), new Point(listFitOut[listFitOut.Count - 1].X, listFitOut[listFitOut.Count - 1].Y));

                    for (int i = 0; i < listFitOut.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                    }

                    for (int i = 0; i < listFitIn.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                    }

                    MatMeasure.PutText("Angle : " + dAngle.ToString("F4"), new Point((int)(listFitOut[0].X + (listFitOut[listFitOut.Count - 1].X - listFitOut[0].X) / 2.0D), listFitOut[0].Y - 50), HersheyFonts.HersheyDuplex, 2.0D, Scalar.Blue, 2);
                    //MatOverlay.PutText("Angle : " + dAngle.ToString("F4"), new Point((int)MatOverlay.Width * 0.5, MatOverlay.Height * 0.5), HersheyFonts.HersheyDuplex, 5.0D, Scalar.Blue, 2);
                }
            }

            MatMeasure.CopyTo(MatResult);
            return listGap;
        }

        public static List<double> Gap(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatResult, out double dAngle)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();

            double dSum = 0.0D;
            dAngle = 361.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Rect rt = new Rect();

                switch (dr)
                {
                    case Direction.LeftToRight:
                        rt.X = 0;
                        rt.Y = 0;
                        rt.Width = (int)(MatMeasure.Width * 0.25D);
                        rt.Height = MatMeasure.Height;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.RightToLeft:
                        rt.Width = (int)(MatMeasure.Width * 0.25D);
                        rt.Height = MatMeasure.Height;
                        rt.X = MatMeasure.Width - rt.Width;
                        rt.Y = 0;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.ToptoBottom:
                        rt.X = 0;
                        rt.Y = 0;
                        rt.Width = MatMeasure.Width;
                        rt.Height = (int)(MatMeasure.Height * 0.2D);
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                    case Direction.BottomToTop:
                        rt.Width = MatMeasure.Width;
                        rt.Height = (int)(MatMeasure.Height * 0.2D);
                        rt.X = 0;
                        rt.Y = MatMeasure.Height - rt.Height;
                        MatMeasure.Rectangle(rt, Scalar.White, -1);
                        break;
                }



                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                if (nEdgeColor == 0)
                {

                }
                else if (nEdgeColor == 1)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }
                else
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }


                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        //if (bFind)
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) > nContrast)
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
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "2");
                //ILogger.Add(LOG_TYPE.ERROR, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }

                List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
                List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

                if (listFitOut.Count > 1)
                {
                    dAngle = IMath.Angle(new Point(listFitOut[0].X, listFitOut[0].Y), new Point(listFitOut[listFitOut.Count - 1].X, listFitOut[listFitOut.Count - 1].Y));

                    for (int i = 0; i < listFitOut.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                    }

                    for (int i = 0; i < listFitIn.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                    }

                    MatMeasure.PutText("Angle : " + dAngle.ToString("F4"), new Point((int)(listFitOut[0].X + (listFitOut[listFitOut.Count - 1].X - listFitOut[0].X) / 2.0D), listFitOut[0].Y - 50), HersheyFonts.HersheyDuplex, 2.0D, Scalar.Blue, 2);

                }
            }

            MatMeasure.CopyTo(MatResult);
            return listGap;
        }

        
        public static List<double> Gap(Rect rtROI, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatMeasure, ref List<Point> listMaskingLine, out List<Point> inLine, out List<Point> outLine)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();
            listMaskingLine = new List<Point>();
            inLine = new List<Point>();
            outLine = new List<Point>();

            double dSum = 0.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
                //Mat matMatrix = new Mat();
                MatMeasure = MatMeasure.SubMat(rtROI);

                //int dWidth = MatMeasure.Width;
                //int dHeigh = MatMeasure.Height;
                //Point2f point2F = new Point2f((float)(MatMeasure.Width / 2.0), (float)(MatMeasure.Height / 2.0));
                //OpenCvSharp.Size size = new OpenCvSharp.Size();
                //size.Width = dWidth / 2;
                //size.Height = dHeigh / 2;
                //matMatrix = Cv2.GetRotationMatrix2D(point2F, 50, 1.0);

                //Cv2.WarpAffine(MatMeasure, MatMeasure, matMatrix, size);
                //Cv2.ImShow("test", MatMeasure);

                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Masking(dr, ref MatMeasure);

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                using (MatKernel)
                {
                    if (nEdgeColor == 1)
                    {
                        Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                    }
                    else if (nEdgeColor == 2)
                    {
                        Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                    }
                }

                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 010)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 10)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.Normal, "2");
                Logger.WriteLog(LOG.Normal, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    //MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatResult.Line(line.Start.X + rtROI.X, line.Start.Y + rtROI.Y, line.End.X + rtROI.X, line.End.Y + rtROI.Y, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    //MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }

                inLine = listInLine;
                outLine = listOutline;

                //List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
                //List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

                //if (listFitOut.Count > 1)
                //{
                //    for (int i = 0; i < listFitOut.Count; i++)
                //    {
                //        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                //        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                //        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                //    }

                //    for (int i = 0; i < listFitIn.Count; i++)
                //    {
                //        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
                //        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                //        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                //    }
                //}                

            }
            //MatMeasure.CopyTo(MatResult);
            //MatMeasure.Dispose();
            return listGap;
        }


        public static List<double> Gap(Rect rtROI, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatMeasure, ref List<Point> listMaskingLine)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();
            listMaskingLine = new List<Point>();

            double dSum = 0.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
                //Mat matMatrix = new Mat();
                MatMeasure = MatMeasure.SubMat(rtROI);

                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Masking(dr, ref MatMeasure);

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                using (MatKernel)
                {
                    if (nEdgeColor == 1)
                    {
                        Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                    }
                    else if (nEdgeColor == 2)
                    {
                        Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                    }
                }

                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 010)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 10)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "2");
                //ILogger.Add(LOG_TYPE.ERROR, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    //MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatResult.Line(line.Start.X + rtROI.X, line.Start.Y + rtROI.Y, line.End.X + rtROI.X, line.End.Y + rtROI.Y, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    //MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    //MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }

                //List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
                //List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

                //if (listFitOut.Count > 1)
                //{
                //    for (int i = 0; i < listFitOut.Count; i++)
                //    {
                //        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                //        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                //    }

                //    for (int i = 0; i < listFitIn.Count; i++)
                //    {
                //        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
                //        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                //    }
                //}

            }
            return listGap;
        }



        public static List<double> Gap(Mat MatMeasure, Rect rtROI, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatOverlay, ref Mat MatResult, ref List<Point> listMaskingLine)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();
            listMaskingLine = new List<Point>();

            double dSum = 0.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
                MatMeasure = MatMeasure.SubMat(rtROI);
                MatMeasure.CopyTo(MatOverlay);
                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Masking(dr, ref MatMeasure);

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                if (nEdgeColor == 1)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }
                else if (nEdgeColor == 2)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }

                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 010)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 10)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "2");
                //ILogger.Add(LOG_TYPE.ERROR, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    MatResult.Line(line.Start.X + rtROI.X, line.Start.Y + rtROI.Y, line.End.X + rtROI.X, line.End.Y + rtROI.Y, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Orange, 3);
                                    MatOverlay.Line(line.Start, line.End, Scalar.Orange, 3);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }

                List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
                List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

                if (listFitOut.Count > 1)
                {
                    for (int i = 0; i < listFitOut.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                    }

                    for (int i = 0; i < listFitIn.Count; i++)
                    {
                        OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
                        MatMeasure.Circle(pt, 5, Scalar.Red, 5);
                        MatOverlay.Circle(pt, 5, Scalar.Red, 5);
                    }
                }
            }

            MatMeasure.CopyTo(MatResult);
            //MatMeasure.Dispose();
            return listGap;
        }

        ///// <summary>
        ///// 이미지를 Overlay 합니다.
        ///// </summary>
        ///// <returns>Overlay 된 이미지 반환</returns>
        public static System.Drawing.Bitmap OverlayImage(System.Drawing.Image OriginalImage, System.Drawing.Bitmap OverlayImage, int nX, int nY)
        {
            try
            {
                GC.Collect();
                Thread.Sleep(10);
                System.Drawing.Bitmap result = new System.Drawing.Bitmap(OriginalImage.Width, OriginalImage.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((System.Drawing.Image)result);

                g.DrawImage(OriginalImage, 0, 0, OriginalImage.Width, OriginalImage.Height);
                g.DrawImage(OverlayImage, nX, nY, OverlayImage.Width, OverlayImage.Height);

                g.Dispose();
                return result;
            }
            catch (Exception Desc)
            {
                System.Drawing.Bitmap result = OverlayImage;
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return result;
            }
        }

        //public static List<double> Gap(Mat MatMeasure, Rect rtROI, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatResult, out double dAngle)
        //{
        //    List<Line> listLine = new List<Line>();
        //    List<Point> listContour = new List<Point>();
        //    List<double> listGap = new List<double>();

        //    double dSum = 0.0D;
        //    dAngle = 361.0D;

        //    try
        //    {
        //        MatMeasure = MatMeasure.SubMat(rtROI);
        //        Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

        //        if (IsMatEmpty(MatMeasure))
        //        {
        //            Logger.WriteLog(LOG.ERROR, "1");
        //            return null;
        //        }

        //        Rect rt = new Rect();

        //        switch (dr)
        //        {
        //            case Direction.LeftToRight:
        //                rt.X = 0;
        //                rt.Y = 0;
        //                rt.Width = (int)(MatMeasure.Width * 0.25D);
        //                rt.Height = MatMeasure.Height;
        //                MatMeasure.Rectangle(rt, Scalar.White, -1);
        //                break;
        //            case Direction.RightToLeft:
        //                rt.Width = (int)(MatMeasure.Width * 0.25D);
        //                rt.Height = MatMeasure.Height;
        //                rt.X = MatMeasure.Width - rt.Width;
        //                rt.Y = 0;
        //                MatMeasure.Rectangle(rt, Scalar.White, -1);
        //                break;
        //            case Direction.ToptoBottom:
        //                rt.X = 0;
        //                rt.Y = 0;
        //                rt.Width = MatMeasure.Width;
        //                rt.Height = (int)(MatMeasure.Height * 0.2D);
        //                MatMeasure.Rectangle(rt, Scalar.White, -1);
        //                break;
        //            case Direction.BottomToTop:
        //                rt.Width = MatMeasure.Width;
        //                rt.Height = (int)(MatMeasure.Height * 0.2D);
        //                rt.X = 0;
        //                rt.Y = MatMeasure.Height - rt.Height;
        //                MatMeasure.Rectangle(rt, Scalar.White, -1);
        //                break;
        //        }



        //        Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

        //        if (nEdgeColor == 0)
        //        {

        //        }
        //        else if (nEdgeColor == 1)
        //        {
        //            Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
        //        }
        //        else
        //        {
        //            Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
        //        }


        //        switch (dr)
        //        {
        //            case Direction.LeftToRight:
        //                for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
        //                    {
        //                        if (nx > 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx + nThickness < MatMeasure.Cols - 1)
        //                            {
        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.RightToLeft:
        //                for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
        //                    {
        //                        if (nx < MatMeasure.Cols - 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx - nThickness > 0)
        //                            {
        //                                bool bFind = false;

        //                                //if (bFind)
        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.ToptoBottom:
        //                for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
        //                    {
        //                        if (ny > 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (ny + nThickness < MatMeasure.Rows - 1)
        //                            {
        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) > nContrast)
        //                                        {

        //                                        }
        //                                        else
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.BottomToTop:
        //                for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
        //                    {
        //                        if (ny < MatMeasure.Rows - 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (ny - nThickness > 0)
        //                            {
        //                                bool bFind = false;
        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                if (line.Distance() < 10)
        //                                                {

        //                                                }
        //                                                else
        //                                                {
        //                                                    dSum += line.Distance();
        //                                                    listLine.Add(line);
        //                                                }
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception Desc)
        //    {
        //        Logger.WriteLog(LOG.ERROR, "2");
        //        Logger.WriteLog(LOG.ERROR, Desc.Message);
        //        return null;
        //    }

        //    if (listLine.Count > 0)
        //    {
        //        double dSumTemp = 0.0D;

        //        List<OpenCvSharp.Point> listOutline = new List<Point>();
        //        List<OpenCvSharp.Point> listInLine = new List<Point>();

        //        if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
        //        {
        //            List<double> listCenterY = new List<double>();                    

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];
        //                double dDist = line.Distance();
        //                dSumTemp += dDist;

        //                double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
        //                listCenterY.Add(dCenterY);
        //            }

        //            double dCenterYAvg = listCenterY.Average();
        //            double dAverageDist = dSumTemp / listCenterY.Count;

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];

        //                if (line != null)
        //                {
        //                    double dDist = line.Distance();
        //                    double dCenterY = (line.Start.Y + dDist / 2.0D);

        //                    if (dCenterY > (dCenterYAvg - (dAverageDist))
        //                        && dCenterY < (dCenterYAvg + (dAverageDist)))
        //                    {
        //                        if (dDist >= (dAverageDist * 0.8D)
        //                            && dDist <= (dAverageDist * 1.5D))
        //                        {
        //                            MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
        //                            lock (lockObject)
        //                            {
        //                                listGap.Add(dDist);
        //                            }

        //                            listOutline.Add(listLine[i].Start);
        //                            listInLine.Add(listLine[i].End);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if(dr == Direction.LeftToRight || dr == Direction.RightToLeft)
        //        {
        //            List<double> listCenterX = new List<double>();

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];
        //                double dDist = line.Distance();
        //                dSumTemp += dDist;

        //                double dCenterX = (line.Start.X + line.Distance() / 2.0D);
        //                listCenterX.Add(dCenterX);
        //            }

        //            double dCenterXAvg = listCenterX.Average();
        //            double dAverageDist = dSumTemp / listCenterX.Count;

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];

        //                if (line != null)
        //                {
        //                    double dDist = line.Distance();
        //                    double dCenterX = (line.Start.X + dDist / 2.0D);

        //                    if (dCenterX > (dCenterXAvg - (dAverageDist))
        //                        && dCenterX < (dCenterXAvg + (dAverageDist)))
        //                    {
        //                        if (dDist >= (dAverageDist * 0.8D)
        //                            && dDist <= (dAverageDist * 1.5D))
        //                        {
        //                            MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
        //                            MatResult.Line(line.Start.X + rtROI.X, line.Start.Y + rtROI.Y, line.End.X + rtROI.X, line.End.Y + rtROI.Y, Scalar.Aquamarine);
        //                            lock (lockObject)
        //                            {
        //                                listGap.Add(dDist);
        //                            }

        //                            listOutline.Add(listLine[i].Start);
        //                            listInLine.Add(listLine[i].End);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        List<Point2d> listFitOut = IVision.RansacLineFitting(listOutline, out double dAOut, out double dBOut);
        //        List<Point2d> listFitIn = IVision.RansacLineFitting(listInLine, out double dAIn, out double dBIn);

        //        if (listFitOut.Count > 1)
        //        {
        //            dAngle = IMath.Angle(new Point(listFitOut[0].X, listFitOut[0].Y), new Point(listFitOut[listFitOut.Count - 1].X, listFitOut[listFitOut.Count - 1].Y));

        //            for (int i = 0; i < listFitOut.Count; i++)
        //            {
        //                OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
        //                MatMeasure.Circle(pt, 5, Scalar.Red, 5);
        //            }

        //            for (int i = 0; i < listFitIn.Count; i++)
        //            {
        //                OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitIn[i].X, (int)listFitIn[i].Y);
        //                MatMeasure.Circle(pt, 5, Scalar.Red, 5);
        //            }
        //        }
        //    }

        //    return listGap;
        //}

        public static bool Masking(Direction dr, ref Mat MatMeasure)
        {
            Rect rt = new Rect();

            switch (dr)
            {
                case Direction.LeftToRight:
                    rt.X = 0;
                    rt.Y = 0;
                    rt.Width = (int)(MatMeasure.Width * 0.25D);
                    rt.Height = MatMeasure.Height;
                    break;
                case Direction.RightToLeft:
                    rt.Width = (int)(MatMeasure.Width * 0.25D);
                    rt.Height = MatMeasure.Height;
                    rt.X = MatMeasure.Width - rt.Width;
                    rt.Y = 0;
                    break;
                case Direction.ToptoBottom:
                    rt.X = 0;
                    rt.Y = 0;
                    rt.Width = MatMeasure.Width;
                    rt.Height = (int)(MatMeasure.Height * 0.2D);
                    break;
                case Direction.BottomToTop:
                    rt.Width = MatMeasure.Width;
                    rt.Height = (int)(MatMeasure.Height * 0.2D);
                    rt.X = 0;
                    rt.Y = MatMeasure.Height - rt.Height;
                    break;
            }

            MatMeasure.Rectangle(rt, Scalar.White, -1);
            return true;
        }

        public static bool Masking(Rect rtROI, Scalar cr, ref Mat MatMeasure)
        {
            MatMeasure.Rectangle(rtROI, cr, -1);
            return true;
        }

        public static List<double> Gap(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatResult, ref List<Point> listMaskingLine)
        {
            List<Line> listLine = new List<Line>();
            List<Point> listContour = new List<Point>();
            List<double> listGap = new List<double>();
            listMaskingLine = new List<Point>();

            double dSum = 0.0D;

            try
            {
                Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

                if (IsMatEmpty(MatMeasure))
                {
                    //ILogger.Add(LOG_TYPE.ERROR, "1");
                    return null;
                }

                Masking(dr, ref MatMeasure);

                Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

                if (nEdgeColor == 1)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }
                else if (nEdgeColor == 2)
                {
                    Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
                }

                switch (dr)
                {
                    case Direction.LeftToRight:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
                            {
                                if (nx > 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx + nThickness < MatMeasure.Cols - 1)
                                    {
                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.RightToLeft:
                        for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
                            {
                                if (nx < MatMeasure.Cols - 1)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (nx - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if (Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.ToptoBottom:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
                            {
                                if (ny > 010)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny + nThickness < MatMeasure.Rows - 1)
                                    {
                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        dSum += line.Distance();
                                                        listLine.Add(line);
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Direction.BottomToTop:
                        for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
                        {
                            Point ptFirst = new Point();
                            Point ptSecond = new Point();

                            bool bFirst = false;

                            Line line = new Line(new Point(), new Point());

                            for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
                            {
                                if (ny < MatMeasure.Rows - 10)
                                {
                                    var pt = MatMeasure.At<Vec3b>(ny, nx);
                                    var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

                                    int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
                                    int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

                                    if (ny - nThickness > 0)
                                    {
                                        bool bFind = false;

                                        if ((!bFirst && (greyValuePrev - greyValue) > nContrast)
                                            || Math.Abs(greyValuePrev - greyValue) > nContrast)
                                        {
                                            bool bThickness = true;

                                            for (int k = 1; k <= nThickness; k++)
                                            {
                                                var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
                                                int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

                                                if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
                                                {
                                                    bThickness = false;
                                                    break;
                                                }
                                            }

                                            if (bThickness)
                                            {
                                                listContour.Add(new OpenCvSharp.Point(nx, ny));

                                                if (bFirst == false)
                                                {
                                                    ptFirst = new OpenCvSharp.Point(nx, ny);
                                                    line.Start = ptFirst;
                                                    bFirst = true;
                                                }
                                                else
                                                {
                                                    ptSecond = new OpenCvSharp.Point(nx, ny);
                                                    line.End = ptSecond;

                                                    if (line != null)
                                                    {
                                                        if (line.Distance() < 10)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            dSum += line.Distance();
                                                            listLine.Add(line);
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                //ILogger.Add(LOG_TYPE.ERROR, "2");
                //ILogger.Add(LOG_TYPE.ERROR, Desc.Message);
                return null;
            }

            if (listLine.Count > 0)
            {
                double dSumTemp = 0.0D;

                List<OpenCvSharp.Point> listOutline = new List<Point>();
                List<OpenCvSharp.Point> listInLine = new List<Point>();

                if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);

                                }
                            }
                        }
                    }
                }
                else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.5D))
                                {
                                    MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }

                                    listOutline.Add(listLine[i].Start);
                                    listInLine.Add(listLine[i].End);

                                    listMaskingLine.Add(listLine[i].End);
                                }
                            }
                        }
                    }
                }
            }

            MatMeasure.CopyTo(MatResult);

            return listGap;
        }
        //public static List<double> Gap(Mat MatMeasure, int nThreshold, int nContrast, int nThickness, int nEdgeColor, Direction dr, ref Mat MatResult, List<Point> listMaskingLine)
        //{
        //    List<Line> listLine = new List<Line>();
        //    List<Point> listContour = new List<Point>();
        //    List<double> listGap = new List<double>();

        //    double dSum = 0.0D;

        //    try
        //    {
        //        Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));

        //        if (IsMatEmpty(MatMeasure))
        //        {
        //            Logger.WriteLog(LOG.ERROR, "1");
        //            return null;
        //        }

        //        Masking(dr, ref MatMeasure);


        //        if(listMaskingLine != null)
        //        {
        //            for (int i = 0; i < listMaskingLine.Count; i++)
        //            {
        //                if (i > 0)
        //                {
        //                    Point ptPrev = listMaskingLine[i - 1];
        //                    Point ptCurrnt = listMaskingLine[i];

        //                    double dDistance = ptPrev.DistanceTo(ptCurrnt);

        //                    if (dDistance > 30)
        //                    {
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        MatMeasure.Line(ptPrev, ptCurrnt, Scalar.White, 5);
        //                    }
        //                }
        //            }
        //        }

        //        Cv2.Threshold(MatMeasure, MatMeasure, nThreshold, 255, ThresholdTypes.Binary);

        //        if (nEdgeColor == 1)
        //        {
        //            Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Dilate, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
        //        }
        //        else
        //        {
        //            Cv2.MorphologyEx(MatMeasure, MatMeasure, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
        //        }

        //        switch (dr)
        //        {
        //            case Direction.LeftToRight:
        //                for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int nx = 0; nx < MatMeasure.Cols - 1; nx++)
        //                    {
        //                        if (nx > 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx - 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx + nThickness < MatMeasure.Cols - 1)
        //                            {
        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx + k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.RightToLeft:
        //                for (int ny = 0; ny < MatMeasure.Rows; ny = ny + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int nx = MatMeasure.Cols - 1; nx > 0; nx--)
        //                    {
        //                        if (nx < MatMeasure.Cols - 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny, nx + 1);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (nx - nThickness > 0)
        //                            {
        //                                bool bFind = false;

        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny, nx - k);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.ToptoBottom:
        //                for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int ny = 0; ny < MatMeasure.Rows - 1; ny++)
        //                    {
        //                        if (ny > 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny - 1, nx);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (ny + nThickness < MatMeasure.Rows - 1)
        //                            {
        //                                if (!bFirst && ((greyValuePrev - greyValue) > nContrast)
        //                                    || Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny + k, nx);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                dSum += line.Distance();
        //                                                listLine.Add(line);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //            case Direction.BottomToTop:
        //                for (int nx = 0; nx < MatMeasure.Cols; nx = nx + 5)
        //                {
        //                    Point ptFirst = new Point();
        //                    Point ptSecond = new Point();

        //                    bool bFirst = false;

        //                    Line line = new Line(new Point(), new Point());

        //                    for (int ny = MatMeasure.Rows - 1; ny > 0; ny--)
        //                    {
        //                        if (ny < MatMeasure.Rows - 1)
        //                        {
        //                            var pt = MatMeasure.At<Vec3b>(ny, nx);
        //                            var ptPrev = MatMeasure.At<Vec3b>(ny + 1, nx);

        //                            int greyValue = (int)((pt.Item2 * 0.3) + (pt.Item1 * 0.59) + (pt.Item0 * 0.11));
        //                            int greyValuePrev = (int)((ptPrev.Item2 * 0.3) + (ptPrev.Item1 * 0.59) + (ptPrev.Item0 * 0.11));

        //                            if (ny - nThickness > 0)
        //                            {
        //                                bool bFind = false;

        //                                if (Math.Abs(greyValuePrev - greyValue) > nContrast)
        //                                {
        //                                    bool bThickness = true;

        //                                    for (int k = 1; k <= nThickness; k++)
        //                                    {
        //                                        var ptThickness = MatMeasure.At<Vec3b>(ny - k, nx);
        //                                        int greyValueT = (int)((ptThickness.Item2 * 0.3) + (ptThickness.Item1 * 0.59) + (ptThickness.Item0 * 0.11));

        //                                        if (Math.Abs(greyValuePrev - greyValueT) < nContrast)
        //                                        {
        //                                            bThickness = false;
        //                                            break;
        //                                        }
        //                                    }

        //                                    if (bThickness)
        //                                    {
        //                                        listContour.Add(new OpenCvSharp.Point(nx, ny));

        //                                        if (bFirst == false)
        //                                        {
        //                                            ptFirst = new OpenCvSharp.Point(nx, ny);
        //                                            line.Start = ptFirst;
        //                                            bFirst = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            ptSecond = new OpenCvSharp.Point(nx, ny);
        //                                            line.End = ptSecond;

        //                                            if (line != null)
        //                                            {
        //                                                if (line.Distance() < 10)
        //                                                {

        //                                                }
        //                                                else
        //                                                {
        //                                                    dSum += line.Distance();
        //                                                    listLine.Add(line);
        //                                                }
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception Desc)
        //    {
        //        Logger.WriteLog(LOG.ERROR, "2");
        //        Logger.WriteLog(LOG.ERROR, Desc.Message);
        //        return null;
        //    }

        //    if (listLine.Count > 0)
        //    {
        //        double dSumTemp = 0.0D;

        //        List<OpenCvSharp.Point> listOutline = new List<Point>();
        //        List<OpenCvSharp.Point> listInLine = new List<Point>();

        //        if (dr == Direction.BottomToTop || dr == Direction.ToptoBottom)
        //        {
        //            List<double> listCenterY = new List<double>();

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];
        //                double dDist = line.Distance();
        //                dSumTemp += dDist;

        //                double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
        //                listCenterY.Add(dCenterY);
        //            }

        //            double dCenterYAvg = listCenterY.Average();
        //            double dAverageDist = dSumTemp / listCenterY.Count;

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];

        //                if (line != null)
        //                {
        //                    double dDist = line.Distance();
        //                    double dCenterY = (line.Start.Y + dDist / 2.0D);

        //                    if (dCenterY > (dCenterYAvg - (dAverageDist))
        //                        && dCenterY < (dCenterYAvg + (dAverageDist)))
        //                    {
        //                        if (dDist >= (dAverageDist * 0.8D)
        //                            && dDist <= (dAverageDist * 1.5D))
        //                        {
        //                            MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
        //                            lock (lockObject)
        //                            {
        //                                listGap.Add(dDist);
        //                            }

        //                            listOutline.Add(listLine[i].Start);
        //                            listInLine.Add(listLine[i].End);

        //                            listMaskingLine.Add(listLine[i].End);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
        //        {
        //            List<double> listCenterX = new List<double>();

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];
        //                double dDist = line.Distance();
        //                dSumTemp += dDist;

        //                double dCenterX = (line.Start.X + line.Distance() / 2.0D);
        //                listCenterX.Add(dCenterX);
        //            }

        //            double dCenterXAvg = listCenterX.Average();
        //            double dAverageDist = dSumTemp / listCenterX.Count;

        //            for (int i = 0; i < listLine.Count; i++)
        //            {
        //                Line line = listLine[i];

        //                if (line != null)
        //                {
        //                    double dDist = line.Distance();
        //                    double dCenterX = (line.Start.X + dDist / 2.0D);

        //                    if (dCenterX > (dCenterXAvg - (dAverageDist))
        //                        && dCenterX < (dCenterXAvg + (dAverageDist)))
        //                    {
        //                        if (dDist >= (dAverageDist * 0.8D)
        //                            && dDist <= (dAverageDist * 1.5D))
        //                        {
        //                            MatMeasure.Line(line.Start, line.End, Scalar.Aquamarine);
        //                            lock (lockObject)
        //                            {
        //                                listGap.Add(dDist);
        //                            }

        //                            listOutline.Add(listLine[i].Start);
        //                            listInLine.Add(listLine[i].End);

        //                            listMaskingLine.Add(listLine[i].End);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    MatMeasure.CopyTo(MatResult);

        //    return listGap;
        //}


        //public static void InspectionGap(Mat MatMeasure, int nContrast, int nThickness, int nEdgeColor, Direction dr)
        //{
        //    try
        //    {
        //        List<Line> listInspectionLine = new List<Line>();


        //    }
        //    catch (Exception Desc)
        //    {

        //    }
        //}


        public static Mat DeleteNonFocusingBackground(Mat MatSource, Direction dr, int nThreshold, out Mat MatDraw)
        {
            Mat MatCanny = new Mat();
            Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            MatDraw = new Mat();
            MatSource.CopyTo(MatDraw);
            if (IVision.IsMatEmpty(MatSource))
            {
                return new Mat();
            }

            Mat MatSoble = new Mat();
            Cv2.Canny(MatSource, MatCanny, nThreshold, 255);

            if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
            {
                Cv2.Sobel(MatCanny, MatSoble, MatType.CV_8U, 1, 0);
            }
            else
            {
                Cv2.Sobel(MatCanny, MatSoble, MatType.CV_8U, 0, 1);
            }

            LineSegmentPoint[] segHoughP = Cv2.HoughLinesP(MatSoble, 1, Math.PI / 180, 100, 100, 10);

            int nNearest = 0;

            if (dr == Direction.LeftToRight || dr == Direction.ToptoBottom)
            {
                nNearest = int.MaxValue;
            }

            List<OpenCvSharp.Point> listPts = new List<OpenCvSharp.Point>();

            List<double> listCenter = new List<double>();

            if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
            {
                for (int i = 0; i < segHoughP.Length; i++)
                {
                    listCenter.Add(segHoughP[i].P1.X);
                    listCenter.Add(segHoughP[i].P2.X);
                }
            }
            else
            {
                for (int i = 0; i < segHoughP.Length; i++)
                {
                    listCenter.Add(segHoughP[i].P1.Y);
                    listCenter.Add(segHoughP[i].P2.Y);
                }
            }

            double dCenterAvg = listCenter.Average();

            for (int i = 0; i < segHoughP.Length; i++)
            {
                LineSegmentPoint s = segHoughP[i];

                if (dr == Direction.LeftToRight)
                {
                    if (s.P1.X < (int)(dCenterAvg * 1.025D)
                        && s.P1.X > (int)(dCenterAvg * 0.9D)
                        && s.P2.X < (int)(dCenterAvg * 1.025D)
                        && s.P2.X > (int)(dCenterAvg * 0.9D))
                    {
                        if (nNearest > s.P1.X)
                        {
                            nNearest = s.P1.X;
                        }

                        if (nNearest > s.P2.X)
                        {
                            nNearest = s.P2.X;
                        }

                        listPts.Add(s.P1);
                        listPts.Add(s.P2);
                    }
                }
                else if (dr == Direction.RightToLeft)
                {
                    if (s.P1.X < (int)(dCenterAvg * 1.1D)
                        && s.P1.X > (int)(dCenterAvg)
                        && s.P2.X < (int)(dCenterAvg * 1.1D)
                        && s.P2.X > (int)(dCenterAvg))
                    {
                        if (nNearest > s.P1.X)
                        {
                            nNearest = s.P1.X;
                        }

                        if (nNearest > s.P2.X)
                        {
                            nNearest = s.P2.X;
                        }

                        listPts.Add(s.P1);
                        listPts.Add(s.P2);
                    }
                }
                else
                {
                    if (s.P1.Y < (int)(dCenterAvg * 1.1D)
                       && s.P1.Y > (int)(dCenterAvg * 0.9D)
                       && s.P2.Y < (int)(dCenterAvg * 1.1D)
                       && s.P2.Y > (int)(dCenterAvg * 0.9D))
                    {
                        if (nNearest > s.P1.Y)
                        {
                            nNearest = s.P1.Y;
                        }

                        if (nNearest > s.P2.Y)
                        {
                            nNearest = s.P2.Y;
                        }

                        listPts.Add(s.P1);
                        listPts.Add(s.P2);
                    }
                }
            }

            Mat MatFilter = new Mat();
            MatSource.CopyTo(MatFilter);

            List<Point2d> listFitOut = IVision.RansacLineFitting(listPts, out double dAOut, out double dBOut);

            int nMax = 0;
            int nMin = int.MaxValue;

            if (dr == Direction.LeftToRight || dr == Direction.RightToLeft)
            {
                for (int i = 0; i < listFitOut.Count; i++)
                {
                    if (listFitOut[i].X > nMax)
                    {
                        nMax = (int)listFitOut[i].X;
                    }

                    if (listFitOut[i].X < nMin)
                    {
                        nMin = (int)listFitOut[i].X;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listFitOut.Count; i++)
                {
                    if (listFitOut[i].Y > nMax)
                    {
                        nMax = (int)listFitOut[i].Y;
                    }

                    if (listFitOut[i].Y < nMin)
                    {
                        nMin = (int)listFitOut[i].Y;
                    }
                }
            }

            List<List<Point>> ListOfListOfPoint = new List<List<Point>>();
            List<Point> listPolygon = new List<Point>();

            if (dr == Direction.LeftToRight)
            {
                listPolygon.Add(new Point(0, 0));
                listPolygon.Add(new Point(0, MatFilter.Height));
                listPolygon.Add(new Point(nMax, MatFilter.Height));
                listPolygon.Add(new Point(nMin, 0));
                listPolygon.Add(new Point(0, 0));
            }
            else if (dr == Direction.RightToLeft)
            {
                listPolygon.Add(new Point(MatFilter.Width, 0));
                listPolygon.Add(new Point(MatFilter.Width, MatFilter.Height));
                listPolygon.Add(new Point(nMax, MatFilter.Height));
                listPolygon.Add(new Point(nMin, 0));
                listPolygon.Add(new Point(MatFilter.Width, 0));
            }
            else if (dr == Direction.ToptoBottom)
            {
                listPolygon.Add(new Point(0, 0));
                listPolygon.Add(new Point(0, nMin));
                listPolygon.Add(new Point(MatFilter.Width, nMax));
                listPolygon.Add(new Point(MatFilter.Width, 0));
                listPolygon.Add(new Point(0, 0));
            }


            ListOfListOfPoint.Add(listPolygon);

            MatFilter.FillPoly(ListOfListOfPoint, Scalar.White);

            if (listFitOut.Count > 1)
            {
                double dAngle = IMath.Angle(new OpenCvSharp.Point(listFitOut[0].X, listFitOut[0].Y), new OpenCvSharp.Point(listFitOut[listFitOut.Count - 1].X, listFitOut[listFitOut.Count - 1].Y));

                for (int i = 0; i < listFitOut.Count; i++)
                {
                    OpenCvSharp.Point pt = new OpenCvSharp.Point((int)listFitOut[i].X, (int)listFitOut[i].Y);
                    MatDraw.Circle(pt, 5, Scalar.Red, 5);
                }

                MatDraw.PutText("Angle : " + dAngle.ToString("F4"), new OpenCvSharp.Point((int)(listFitOut[0].X + (listFitOut[listFitOut.Count - 1].X - listFitOut[0].X) / 2.0D), listFitOut[0].Y - 50), HersheyFonts.HersheyDuplex, 2.0D, Scalar.Blue, 2);
            }
            GC.Collect();

            return MatFilter;

        }

        private static object lockObject = new object();
        public static List<Polygon> CenterofGravityFromContour(Mat MatSource, int nThreshold, int nMinArea, int nMaxArea)
        {
            Mat MatGray = new Mat();
            Mat MatMorp = new Mat();
            Mat MatContour = new Mat();
            Mat MatKernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            HierarchyIndex[] hierarchy;

            List<Polygon> ListCenterofGravity = new List<Polygon>();
            Point testpoint = new OpenCvSharp.Point();

            OpenCvSharp.Size cvsize;
            OpenCvSharp.Point[][] contours;

            int nThreshold1 = nThreshold;
            int nThreshold2 = 255;

            MatSource.CopyTo(MatContour);

            cvsize.Width = MatContour.Width;
            cvsize.Height = MatContour.Height;


            //Image Process
            //Cv2.CvtColor(MatContour, MatGray, ColorConversionCodes.BGRA2GRAY);
            //Cv2.ImShow("Test1", MatGray);
            Cv2.Threshold(MatContour, MatGray, nThreshold1, nThreshold2, ThresholdTypes.Binary);
            //Cv2.AdaptiveThreshold(MatGray, MatGray, nThreshold, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 15, 5);
            //Cv2.ImShow("Test2", MatGray);
            Cv2.MorphologyEx(MatGray, MatMorp, MorphTypes.Erode, MatKernel, new OpenCvSharp.Point(-1, -1), 1);
            //Cv2.ImShow("Test3", MatMorp);

            //Algorithm
            Cv2.FindContours(MatMorp, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

            try
            {
                for (int i = 0; i < contours.Length; i++)
                {
                    double peri = Cv2.ArcLength(contours[i], true);

                    OpenCvSharp.Point[] pp = Cv2.ApproxPolyDP(contours[i], 0.02 * peri, true);
                    Rect rt = Cv2.BoundingRect(pp);

                    RotatedRect rrect = Cv2.MinAreaRect(pp);
                    double areaRatio = Math.Abs(Cv2.ContourArea(contours[i], false)) / (rrect.Size.Width * rrect.Size.Height);

                    Cv2.DrawContours(MatMorp, contours, i, Scalar.Yellow, 2, LineTypes.AntiAlias, hierarchy, 100, testpoint);

                    if (IVision.AreaofRect(rt.Size) >= nMinArea && IVision.AreaofRect(rt.Size) <= nMaxArea)
                    {
                        Cv2.PutText(MatContour, string.Format("{0} Angles", pp.Length), new OpenCvSharp.Point((int)rrect.Center.X, (int)rrect.Center.Y), HersheyFonts.HersheySimplex, 0.5D, new Scalar(125, 125, 255), 3);

                        for (int j = 0; j < pp.Length; j++)
                        {
                            Cv2.Circle(MatContour, new OpenCvSharp.Point(pp[j].X, pp[j].Y), 5, new Scalar(0, 0, 255), 2);
                        }

                        if (pp.Length != 0)
                        {
                            Polygon pg = new Polygon(pp);
                            ListCenterofGravity.Add(pg);

                            Point[] parrCt = pg.ReturnCenterpointofPolygon();
                        }
                    }
                }
            }

            catch (Exception Desc)
            {

            }

            return ListCenterofGravity;
        }
        public static void LineIntersectionCross(Point2d pt1A, Point2d pt1B, Point2d pt2A, Point2d pt2B)
        {
            double dDiffX = 0.0;
            double dDiffY = 0.0;
            Point2d PtLineVertA = new Point2d();
            Point2d PtLineVertB = new Point2d();
            //// 기울기 구하기(밑변 / 높이)
            double dAngle1 = (pt1B.Y - pt1A.Y) / (pt1B.X - pt1A.X);
            double dAngle2 = (pt2B.Y - pt2A.Y) / (pt2B.X - pt2A.X);

            if (dAngle1 == dAngle2)
            {
                PtLineVertA.X = pt1A.X;
                PtLineVertA.Y = pt1A.Y;
                PtLineVertB.X = pt1B.X;
                PtLineVertB.Y = pt1B.Y;

                dDiffX = PtLineVertA.X - PtLineVertB.X;
                dDiffY = PtLineVertA.Y - PtLineVertB.Y;
            }
        }

        public static List<Point2d> RansacLineFitting(List<Point> points, out double dFactorA, out double dFactorB)
        {
            int nPointsCount = points.Count;

            if (nPointsCount == 0)
            {
                dFactorA = 0.0D;
                dFactorB = 0.0D;
                return new List<Point2d>();
            }

            double dMeanX = points.Average(point => point.X);
            double dMeanY = points.Average(point => point.Y);

            double dSumXSquared = 0.0D;// points.Sum(point => point.X * point.X);
            double dSumXY = 0.0D; //points.Sum(point => point.X * point.Y);

            for (int i = 0; i < points.Count; i++)
            {
                dSumXSquared += (points[i].X * points[i].X);
                dSumXY += (points[i].X * points[i].Y);
            }

            dFactorA = (dSumXY / nPointsCount - dMeanX * dMeanY) / (dSumXSquared / nPointsCount - dMeanX * dMeanX);
            dFactorB = (dFactorA * dMeanX - dMeanY);

            double dFacA = dFactorA;
            double dFacB = dFactorB;
            return points.Select(point => new Point2d() { X = point.X, Y = dFacA * point.X - dFacB }).ToList();
        }

        public static List<Point> RansacLineFitting2(List<Point> points, out double dFactorA, out double dFactorB)
        {
            int nPointsCount = points.Count;

            if (nPointsCount == 0)
            {
                dFactorA = 0.0D;
                dFactorB = 0.0D;
                return new List<Point>();
            }

            double dMeanX = points.Average(point => point.X);
            double dMeanY = points.Average(point => point.Y);

            double dSumXSquared = 0.0D;// points.Sum(point => point.X * point.X);
            double dSumXY = 0.0D; //points.Sum(point => point.X * point.Y);

            for (int i = 0; i < points.Count; i++)
            {
                dSumXSquared += (points[i].X * points[i].X);
                dSumXY += (points[i].X * points[i].Y);
            }

            dFactorA = (dSumXY / nPointsCount - dMeanX * dMeanY) / (dSumXSquared / nPointsCount - dMeanX * dMeanX);
            dFactorB = (dFactorA * dMeanX - dMeanY);

            double dFacA = dFactorA;
            double dFacB = dFactorB;
            return points.Select(point => new Point() { X = point.X, Y = (int)(dFacA * point.X - dFacB) }).ToList();
        }

        public static List<Point> RansacLineFittingInt(List<Point> points, out double dFactorA, out double dFactorB)
        {
            int nPointsCount = points.Count;

            if (nPointsCount == 0)
            {
                dFactorA = 0.0D;
                dFactorB = 0.0D;
                return new List<Point>();
            }

            double dMeanX = points.Average(point => point.X);
            double dMeanY = points.Average(point => point.Y);

            double dSumXSquared = 0.0D;// points.Sum(point => point.X * point.X);
            double dSumXY = 0.0D; //points.Sum(point => point.X * point.Y);

            for (int i = 0; i < points.Count; i++)
            {
                dSumXSquared += (points[i].X * points[i].X);
                dSumXY += (points[i].X * points[i].Y);
            }

            dFactorA = (dSumXY / nPointsCount - dMeanX * dMeanY) / (dSumXSquared / nPointsCount - dMeanX * dMeanX);
            dFactorB = (dFactorA * dMeanX - dMeanY);

            double dFacA = dFactorA;
            double dFacB = dFactorB;

            return points.Select(point => new Point() { X = point.X, Y = (int)(dFacA * point.X - dFacB) }).ToList();
        }

        public static int AreaofRect(Size sz)
        {
            return sz.Width * sz.Height;
        }

        public static Point CenterofRect(Rect rt)
        {
            return new Point(rt.X + rt.Width / 2, rt.Y + rt.Height / 2);
        }

        public static void GetLineCoef(Point ptStart/*하판 1Point*/, Point ptEnd/*하판 2Point*/, Rect rcRoi, IVision.Direction direction, double dAngle, out List<Point> listPtVertLine)
        {
            try
            {
                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nWidthRoiX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 포인트 1, 2의 기울기 구함
                int nVertX = 0;
                int nVertY = 0;
                listPtVertLine = new List<Point>();
                Point ptVertLineY = new Point();
                double dLineAngle = dAngle;//(ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                //(ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                // 직선 A와 직선 B가 수직이면
                // 직선 A 기울기 * 직선 B 기울기 == -1
                // 그러기 때문에 아래와 같은 공식이 됨
                double dVerticalAngle = -(1.0 / dLineAngle);
                // 수직 라인 구하기
                switch (direction)
                {
                    case IVision.Direction.LeftToRight:
                        {
                            for (int nCount = ptStart.X; nCount > nStartRoiX; nCount--)
                            {
                                nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                                ptVertLineY.X = nCount;
                                ptVertLineY.Y = nVertY;
                                listPtVertLine.Add(ptVertLineY);
                            }
                            // 수직선 구하기 -> 직선의 방정식

                        }
                        break;
                    case IVision.Direction.RightToLeft:
                        {
                            for (int nCount = ptStart.X; nCount < nWidthRoiX; nCount++)
                            {
                                nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                                ptVertLineY.X = nCount;
                                ptVertLineY.Y = nVertY;
                                listPtVertLine.Add(ptVertLineY);
                            }
                        }
                        break;
                    case IVision.Direction.ToptoBottom:
                        {
                            for (int nCount = ptStart.Y; nCount > nStartRoiY; nCount--)
                            {
                                nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                                ptVertLineY.X = nVertX;
                                ptVertLineY.Y = nCount;
                                listPtVertLine.Add(ptVertLineY);
                            }
                        }
                        break;
                    case IVision.Direction.BottomToTop:
                        {
                            for (int nCount = ptStart.Y; nCount < nEndRoiY; nCount++)
                            {
                                nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                                ptVertLineY.X = nVertX;
                                ptVertLineY.Y = nCount;
                                listPtVertLine.Add(ptVertLineY);
                            }
                        }
                        break;
                }

                //if (ptStart.X - ptEnd.X != 0)
                //{
                //    if (ptStart.Y - ptEnd.Y != 0)
                //    {
                //        //switch (direction)
                //        //{
                //        //    case IVision.Direction.LeftToRight:
                //        //        {
                //        //            for(int nCount = ptStart.X; nCount > nStartRoiX; nCount--)
                //        //            {
                //        //                nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                //        //                ptVertLineY.X = nCount;
                //        //                ptVertLineY.Y = nVertY;
                //        //                listPtVertLine.Add(ptVertLineY);
                //        //            }
                //        //            // 수직선 구하기 -> 직선의 방정식

                //        //        }
                //        //        break;
                //        //    case IVision.Direction.RightToLeft:
                //        //        {
                //        //            for (int nCount = ptStart.X; nCount < nWidthRoiX; nCount++)
                //        //            {
                //        //                nVertY = ((int)(dVerticalAngle * (nCount - ptStart.X)) + ptStart.Y);
                //        //                ptVertLineY.X = nCount;
                //        //                ptVertLineY.Y = nVertY;
                //        //                listPtVertLine.Add(ptVertLineY);
                //        //            }
                //        //        }
                //        //        break;
                //        //    case IVision.Direction.ToptoBottom:
                //        //        {
                //        //            for (int nCount = ptStart.Y; nCount > nStartRoiY; nCount--)
                //        //            {
                //        //                nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                //        //                ptVertLineY.X = nVertX;
                //        //                ptVertLineY.Y = nCount;
                //        //                listPtVertLine.Add(ptVertLineY);
                //        //            }
                //        //        }
                //        //        break;
                //        //    case IVision.Direction.BottomToTop:
                //        //        {
                //        //            for (int nCount = ptStart.Y; nCount < nEndRoiY; nCount++)
                //        //            {
                //        //                nVertX = ((int)((nCount - ptStart.Y) / dVerticalAngle) + ptStart.X);
                //        //                ptVertLineY.X = nVertX;
                //        //                ptVertLineY.Y = nCount;
                //        //                listPtVertLine.Add(ptVertLineY);
                //        //            }
                //        //        }
                //        //        break;
                //        //}
                //    }
                //    else
                //    {
                //        if(direction == IVision.Direction.ToptoBottom)
                //        {
                //            for (int nCount = ptStart.Y; nCount > nStartRoiY; nCount--)
                //            {
                //                ptVertLineY.X = ptStart.X;
                //                ptVertLineY.Y = nCount;
                //                listPtVertLine.Add(ptVertLineY);
                //            }
                //        }
                //        else if(direction == IVision.Direction.BottomToTop)
                //        {
                //            for (int nCount = ptStart.Y; nCount < nEndRoiY; nCount++)
                //            {
                //                ptVertLineY.X = ptStart.X;
                //                ptVertLineY.Y = nCount;
                //                listPtVertLine.Add(ptVertLineY);
                //            }
                //        }

                //    }
                //}
                //else
                //{
                //    if(direction == IVision.Direction.LeftToRight)
                //    {
                //        for (int nCount = ptStart.X; nCount > nStartRoiX; nCount--)
                //        {
                //            ptVertLineY.X = nCount;
                //            ptVertLineY.Y = ptStart.Y;
                //            listPtVertLine.Add(ptVertLineY);
                //        }
                //    }
                //    else if(direction == IVision.Direction.RightToLeft)
                //    {
                //        for (int nCount = ptStart.X; nCount < nWidthRoiX; nCount++)
                //        {
                //            ptVertLineY.X = nCount;
                //            ptVertLineY.Y = ptStart.Y;
                //            listPtVertLine.Add(ptVertLineY);
                //        }
                //    }

                //    //ptVertLineY.X = ptStart.X;
                //    //ptVertLineY.Y = ptStart.Y;
                //}
            }
            catch (Exception Desc)
            {
                listPtVertLine = new List<Point>();
                return;
            }
        }

        public static void GetLineCoef(Point2d ptStart, Point2d ptEnd, Point2d ptTop, Rect rcRoi, IVision.Direction direction, out Point2d ptVertLineY)
        {
            try
            {
                int nStartX = (int)ptTop.X;
                int nStartY = (int)ptTop.Y;

                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nEndRoiX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 포인트 1, 2의 기울기 구함
                int nCenterY = (int)(ptStart.Y + ptEnd.Y) / 2;
                int nCenterX = (int)(ptStart.X + ptEnd.X) / 2;
                int nVertX = 0;
                int nVertY = 0;
                ptVertLineY = new Point2d();
                if (ptEnd.X - ptStart.X != 0)
                {
                    if (ptEnd.Y - ptStart.Y != 0)
                    {
                        // 직선의 기울기 구하기(Start -> End)
                        double dLineAngle = 0;
                        if (ptStart.X > ptEnd.X)
                        {
                            dLineAngle = (ptStart.Y - ptEnd.Y) / (ptStart.X - ptEnd.X);
                        }
                        else
                        {
                            dLineAngle = (ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                        }


                        // 직선 A와 직선 B가 수직이면
                        // 직선 A 기울기 * 직선 B 기울기 == -1
                        // 그러기 때문에 아래와 같은 공식이 됨
                        double dVerticalAngle = -(1.0 / dLineAngle);
                        // 수직 라인 구하기
                        switch (direction)
                        {
                            case IVision.Direction.LeftToRight:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    nVertY = (int)((dVerticalAngle * (nStartX - ptEnd.X)) + ptEnd.Y);
                                    ptVertLineY.X = nStartX;
                                    ptVertLineY.Y = nVertY;
                                }
                                break;
                            case IVision.Direction.RightToLeft:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    nVertY = (int)((dVerticalAngle * (nEndRoiX - ptEnd.X)) + ptEnd.Y);
                                    ptVertLineY.X = nEndRoiX;
                                    ptVertLineY.Y = nVertY;
                                }
                                break;
                            case IVision.Direction.ToptoBottom:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    nVertX = (int)(((nStartRoiY - ptEnd.Y) / dVerticalAngle) + ptEnd.X);
                                    ptVertLineY.X = nVertX;
                                    ptVertLineY.Y = nStartRoiY;
                                }

                                break;
                            case IVision.Direction.BottomToTop:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    nVertX = (int)(((nEndRoiY - ptEnd.Y) / dVerticalAngle) + ptEnd.X);
                                    ptVertLineY.X = nVertX;
                                    ptVertLineY.Y = nEndRoiY;
                                }
                                break;
                        }
                    }
                    // 0이면 수직선은 Y축에 대해 평행함 
                    else
                    {
                        ptVertLineY.X = nCenterX;
                        ptVertLineY.Y = ptStart.Y;
                    }
                }
                // 0이면 수직선은 X축에 대해 평행함 
                else
                {
                    ptVertLineY.X = ptStart.X;
                    ptVertLineY.Y = nCenterY;
                }
            }
            catch (Exception Desc)
            {
                ptVertLineY = new Point2d();
                return;
            }
        }

        public static void GetLineDiffAngle(List<Point> listPtFit, IVision.Direction direction, out Dictionary<int, Point> dicPtIncrease, out Dictionary<int, Point> dicPtDecrease)
        {
            try
            {
                dicPtIncrease = new Dictionary<int, Point>();
                dicPtDecrease = new Dictionary<int, Point>();
                bool bFirstAdd = true;
                bool bLastIncreasePosition = true;
                if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    for (int nPtCount = 0; nPtCount < listPtFit.Count; nPtCount++)
                    {
                        if (nPtCount == listPtFit.Count - 1)
                        {
                            if (bLastIncreasePosition)
                            {
                                dicPtIncrease.Add(nPtCount, listPtFit[nPtCount]);
                            }
                            else
                            {
                                dicPtDecrease.Add(nPtCount, listPtFit[nPtCount]);
                            }
                            return;
                        }

                        if (listPtFit[nPtCount].Y - listPtFit[nPtCount + 1].Y != 0)
                        {
                            // 1. Y축값의 차가 +거나, -일 때를 구분지어서 딕셔너리 보관
                            int nDiffY = listPtFit[nPtCount].Y - listPtFit[nPtCount + 1].Y;
                            // Increase
                            if (nDiffY > 0)
                            {
                                // 처음에는 무조건 변곡점의 엣지부분을 가져와야함                                 
                                if (bFirstAdd)
                                {
                                    dicPtIncrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                                    bFirstAdd = false;
                                }
                                // 두번째부터는 변곡점의 -1의 엣지부분을 가져옴
                                else
                                {
                                    dicPtIncrease.Add(nPtCount, listPtFit[nPtCount]);
                                }
                                bLastIncreasePosition = true;
                            }
                            // Decrease 
                            else if (nDiffY < 0)
                            {
                                if (bFirstAdd)
                                {
                                    dicPtDecrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                                    bFirstAdd = false;
                                }
                                else
                                {
                                    dicPtDecrease.Add(nPtCount, listPtFit[nPtCount]);
                                }
                                bLastIncreasePosition = false;
                            }
                        }
                    }
                    return;
                }
                else if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    for (int nPtCount = 0; nPtCount < listPtFit.Count - 1; nPtCount++)
                    {
                        if (nPtCount == listPtFit.Count - 1)
                        {
                            if (bLastIncreasePosition)
                            {
                                dicPtIncrease.Add(nPtCount, listPtFit[nPtCount]);
                            }
                            else
                            {
                                dicPtDecrease.Add(nPtCount, listPtFit[nPtCount]);
                            }
                            return;
                        }

                        if (listPtFit[nPtCount].X - listPtFit[nPtCount + 1].X != 0)
                        {
                            int nDiffX = listPtFit[nPtCount].X - listPtFit[nPtCount + 1].X;
                            // Increase
                            if (nDiffX > 0)
                            {
                                if (bFirstAdd)
                                {
                                    dicPtIncrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                                    bFirstAdd = false;
                                }
                                else
                                {
                                    dicPtIncrease.Add(nPtCount, listPtFit[nPtCount]);
                                }
                                bLastIncreasePosition = true;
                            }
                            // Decrease 
                            else if (nDiffX < 0)
                            {
                                if (bFirstAdd)
                                {
                                    dicPtDecrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                                    bFirstAdd = false;
                                }
                                else
                                {
                                    dicPtDecrease.Add(nPtCount, listPtFit[nPtCount]);
                                }
                                bLastIncreasePosition = false;
                            }
                        }

                        //if (nPtCount == listPtFit.Count - 1)
                        //{
                        //    if (bLastIncreasePosition)
                        //    {
                        //        dicPtIncrease.Add(nPtCount, listPtFit[nPtCount]);
                        //    }
                        //    else
                        //    {
                        //        dicPtDecrease.Add(nPtCount, listPtFit[nPtCount]);
                        //    }
                        //    return;
                        //}

                        //if (listPtFit[nPtCount].X - listPtFit[nPtCount + 1].X != 0)
                        //{
                        //    int nDiffX = listPtFit[nPtCount].X - listPtFit[nPtCount + 1].X;
                        //    // Increase
                        //    if (nDiffX > 0)
                        //    {
                        //        dicPtIncrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                        //    }
                        //    // Decrease 
                        //    else if (nDiffX < 0)
                        //    {
                        //        dicPtDecrease.Add(nPtCount, listPtFit[nPtCount + 1]);
                        //    }
                        //}
                    }
                    return;
                }

            }
            catch (Exception Desc)
            {
                dicPtIncrease = new Dictionary<int, Point>();
                dicPtDecrease = new Dictionary<int, Point>();
                return;
            }
            dicPtIncrease = new Dictionary<int, Point>();
            dicPtDecrease = new Dictionary<int, Point>();
        }

        public static void GetLineCoef(Point ptStart, Point ptEnd, Rect rcRoi, IVision.Direction direction, double dAngle, out Point ptVertLineY)
        {
            try
            {
                int nDiffY = 0;
                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nEndRoiX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 포인트 1, 2의 기울기 구함
                int nCenterY = (int)(ptStart.Y + ptEnd.Y) / 2;
                int nCenterX = (int)(ptStart.X + ptEnd.X) / 2;
                int nVertX = 0;
                int nVertY = 0;
                ptVertLineY = new Point();
                double dLineAngle = dAngle;
                //(ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                // 직선 A와 직선 B가 수직이면
                // 직선 A 기울기 * 직선 B 기울기 == -1
                // 그러기 때문에 아래와 같은 공식이 됨
                double dVerticalAngle = -(1.0 / dLineAngle);
                // 수직 라인 구하기
                switch (direction)
                {
                    case IVision.Direction.LeftToRight:
                        {
                            // 수직선 구하기 -> 직선의 방정식
                            nVertY = ((int)(dVerticalAngle * (nStartRoiX - ptStart.X)) + ptStart.Y);
                            ptVertLineY.X = nStartRoiX;
                            ptVertLineY.Y = nVertY;
                        }
                        break;
                    case IVision.Direction.RightToLeft:
                        {
                            // 수직선 구하기 -> 직선의 방정식
                            nVertY = ((int)(dVerticalAngle * (nEndRoiX - ptStart.X)) + ptStart.Y);
                            ptVertLineY.X = nEndRoiX;
                            ptVertLineY.Y = nVertY;
                        }
                        break;
                    case IVision.Direction.ToptoBottom:
                        {
                            // 수직선 구하기 -> 직선의 방정식
                            nVertX = ((int)((nStartRoiY - ptStart.Y) / dVerticalAngle) + ptStart.X);
                            ptVertLineY.X = nVertX;
                            ptVertLineY.Y = nStartRoiY;
                        }
                        break;
                    case IVision.Direction.BottomToTop:
                        {
                            // 수직선 구하기 -> 직선의 방정식
                            nVertX = ((int)((nEndRoiY - ptStart.Y) / dVerticalAngle) + ptStart.X);
                            ptVertLineY.X = nVertX;
                            ptVertLineY.Y = nEndRoiY;
                        }
                        break;
                }
            }
            catch (Exception Desc)
            {
                ptVertLineY = new Point();
                return;
            }
        }

        public static void GetLineCoef(Point ptStart, Point ptEnd, Point ptBase, Point ptImageSize, out List<Point> listPtVert)
        {
            try
            {
                listPtVert = new List<Point>();
                int nImageWidth = ptImageSize.X;
                int nImageHeight = ptImageSize.Y;
                int nStartBaseX = ptBase.X;
                int nStartBaseY = ptBase.Y;

                // 포인트 1, 2의 기울기 구함
                int nVertY = 0;
                double dLineAngle = (double)(ptEnd.Y - ptStart.Y) / (double)(ptEnd.X - ptStart.X);

                // 직선 A와 직선 B가 수직이면
                // 직선 A 기울기 * 직선 B 기울기 == -1
                // 그러기 때문에 아래와 같은 공식이 됨
                double dVerticalAngle = -(1.0 / dLineAngle);

                if (ptStart.X - ptEnd.X != 0)
                {
                    if (ptStart.Y - ptEnd.Y != 0)
                    {
                        Point ptVerticalX = new Point();
                        for (int nIndex = 0; nIndex < nImageWidth; nIndex++)
                        {
                            nVertY = ((int)(dVerticalAngle * (nIndex - ptBase.X)) + ptBase.Y);
                            ptVerticalX.X = nIndex;
                            ptVerticalX.Y = nVertY;
                            listPtVert.Add(ptVerticalX);
                        }
                        //// 수직 라인 구하기               
                    }
                    else
                    {
                        Point ptVerticalY = new Point();
                        for (int nIndex = 0; nIndex < nImageHeight; nIndex++)
                        {
                            ptVerticalY.X = ptBase.X;
                            ptVerticalY.Y = nIndex;
                            listPtVert.Add(ptVerticalY);
                        }
                        // Y축에 평행

                    }
                }
                else
                {
                    Point ptVerticalX = new Point();
                    for (int nIndex = 0; nIndex < nImageWidth; nIndex++)
                    {
                        ptVerticalX.X = nIndex;
                        ptVerticalX.Y = ptBase.Y;
                        listPtVert.Add(ptVerticalX);
                    }
                    // X축에 평행
                }
            }
            catch (Exception Desc)
            {
                listPtVert = new List<Point>();
                return;
            }
        }

        public static double MeaseAngle(Point pt1, Point pt2, Point pt3)
        {
            double a, b, c;
            double Angle, temp;
            a = Math.Sqrt(Math.Pow(pt1.X - pt3.X, 2) + Math.Pow(pt1.Y - pt3.Y, 2));
            b = Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
            c = Math.Sqrt(Math.Pow(pt2.X - pt3.X, 2) + Math.Pow(pt2.Y - pt3.Y, 2));
            temp = (Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c);
            Angle = Math.Acos(temp);
            Angle = Angle * (180 / Math.PI);
            return Angle;
        }

        public static double threePointAngle(Point ptBase/*ROI와 수직점이 교차한 포인트*/, Point pt1/*하판점*/, Point pt2/*ROI점*/)
        {
            Point ptVectorVerticalToBottom = new Point();
            Point ptVectorVerticalToRoi = new Point();
            // B(PtBase) -> A(하판) 방향으로 가는 벡터1 생성
            ptVectorVerticalToBottom.X = pt1.X - ptBase.X;
            ptVectorVerticalToBottom.Y = pt1.Y - ptBase.Y;
            // B(PtBase) -> C(ROI) 방향으로 가는 벡터2 생성
            ptVectorVerticalToRoi.X = pt2.X - ptBase.X;
            ptVectorVerticalToRoi.Y = pt2.Y - ptBase.Y;

            // 벡터1과 벡터2의 내적
            int nMoleculatr = (ptVectorVerticalToBottom.X * ptVectorVerticalToRoi.X) + (ptVectorVerticalToBottom.Y * ptVectorVerticalToRoi.Y);

            // 벡터1과 벡터2의 Scalr값
            double dDistanceVerticalToBottom = Math.Sqrt(Math.Pow(ptVectorVerticalToBottom.X, 2) + Math.Pow(ptVectorVerticalToBottom.Y, 2));
            double dDistanceVerticalToRoi = Math.Sqrt(Math.Pow(ptVectorVerticalToRoi.X, 2) + Math.Pow(ptVectorVerticalToRoi.Y, 2));

            double nDenominator = dDistanceVerticalToBottom * dDistanceVerticalToRoi;
            // 각도 구하기 (내적 / Sclar값)
            double dAngle = Math.Acos(nMoleculatr / nDenominator) * (180 / Math.PI);

            return dAngle;
        }

        public static void GetLineCoef(Point2d ptStart, Point2d ptEnd, Rect rcRoi, IVision.Direction direction, out Point2d ptVertLineY)
        {
            try
            {
                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nEndRoiX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 포인트 1, 2의 기울기 구함
                double dCenterY = (ptStart.Y + ptEnd.Y) / 2;
                double dCenterX = (ptStart.X + ptEnd.X) / 2;
                double dVertX = 0;
                double dVertY = 0;
                ptVertLineY = new Point2d();
                if (ptEnd.X - ptStart.X != 0)
                {
                    if (ptEnd.Y - ptStart.Y != 0)
                    {
                        // 직선의 기울기 구하기(Start -> End)
                        double dLineAngle = 0;
                        if (ptStart.X > ptEnd.X)
                        {
                            dLineAngle = (ptStart.Y - ptEnd.Y) / (ptStart.X - ptEnd.X);
                        }
                        else
                        {
                            dLineAngle = (ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                        }

                        // 직선 A와 직선 B가 수직이면
                        // 직선 A 기울기 * 직선 B 기울기 == -1
                        // 그러기 때문에 아래와 같은 공식이 됨
                        double dVerticalAngle = -(1.0 / dLineAngle);
                        // 수직 라인 구하기
                        switch (direction)
                        {
                            case IVision.Direction.LeftToRight:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    dVertY = (dVerticalAngle * (nStartRoiX - ptEnd.X)) + ptEnd.Y;
                                    ptVertLineY.X = nStartRoiX;
                                    ptVertLineY.Y = dVertY;
                                }
                                break;
                            case IVision.Direction.RightToLeft:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    dVertY = (dVerticalAngle * (nEndRoiX - ptEnd.X)) + ptEnd.Y;
                                    ptVertLineY.X = nEndRoiX;
                                    ptVertLineY.Y = dVertY;
                                }
                                break;
                            case IVision.Direction.ToptoBottom:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    dVertX = ((nStartRoiY - ptEnd.Y) / dVerticalAngle) + ptEnd.X;
                                    ptVertLineY.X = dVertX;
                                    ptVertLineY.Y = nStartRoiY;
                                }

                                break;
                            case IVision.Direction.BottomToTop:
                                {
                                    // 수직선 구하기 -> 직선의 방정식
                                    dVertX = ((nEndRoiY - ptEnd.Y) / dVerticalAngle) + ptEnd.X;
                                    ptVertLineY.X = dVertX;
                                    ptVertLineY.Y = nEndRoiY;
                                }
                                break;
                        }
                    }
                    // 0이면 수직선은 Y축에 대해 평행함 
                    else
                    {
                        ptVertLineY.X = dCenterX;
                        ptVertLineY.Y = ptStart.Y;
                    }
                }
                // 0이면 수직선은 X축에 대해 평행함 
                else
                {
                    ptVertLineY.X = ptStart.X;
                    ptVertLineY.Y = dCenterY;
                }
            }
            catch (Exception Desc)
            {
                ptVertLineY = new Point2d();
                return;
            }
        }

        public static void GetLineCoef(Point2d PtInA, Point2d PtInB, out Point2d ptVertLineY)
        {
            try
            {
                // 포인트 1, 2의 기울기 구함
                double dCenterY = (PtInA.Y + PtInB.Y) / 2;
                double dCenterX = (PtInA.X + PtInB.X) / 2;
                if (PtInB.X - PtInA.X != 0)
                {
                    if (PtInB.Y - PtInA.Y != 0)
                    {
                        double dAngle1 = 0;
                        if (PtInA.X > PtInB.X)
                        {
                            dAngle1 = (PtInB.Y - PtInA.Y) / (PtInB.X - PtInA.X);
                        }
                        else
                        {
                            dAngle1 = (PtInA.Y - PtInB.Y) / (PtInA.X - PtInB.X);
                        }
                        ptVertLineY.X = PtInB.X + PtInA.X;
                        // 직선 A와 직선 B가 수직이면 직선 A 기울기 * 직선 B 기울기 == -1
                        // 그러기 때문에 직선 B의 기울기를 구한다.
                        double dAngle2 = (1.0 / dAngle1) * -1.0;
                        // 수직선 구하기 -> 직선의 방정식
                        //double dVertY = (dAngle2 * PtInB.X) + PtInB.Y;
                        double dVertY = (dAngle2 * PtInB.X) + PtInB.Y;
                        // X값을 +-해서 좀더 길게 해주려고 했지만 
                        // 테스트를 좀더 해보고 넣어줘야할듯                        
                        ptVertLineY.Y = (int)((dAngle2 * ptVertLineY.X) + dVertY);
                        var newK = (ptVertLineY.X - PtInB.X) / (float)(ptVertLineY.Y - PtInB.Y);

                        double dAngle3 = (ptVertLineY.Y - PtInA.Y) / (ptVertLineY.X - PtInA.X);
                        double test3 = dAngle3 * dAngle2;
                    }
                    // 0이면 수직선은 Y축에 대해 평행함 
                    else
                    {
                        //ptVertLineY = new Point2d();
                        ptVertLineY.X = dCenterX;
                        ptVertLineY.Y = PtInA.Y;
                    }
                }
                // 0이면 수직선은 X축에 대해 평행함 
                else
                {
                    //ptVertLineY = new Point2d();
                    ptVertLineY.X = PtInA.X;
                    ptVertLineY.Y = dCenterY;
                }
            }
            catch (Exception Desc)
            {
                ptVertLineY = new Point2d();
                return;
            }
        }

        public static bool FindIntersection(Line BaseLine/*수직선*/, Line BaseTarget/*상판이나 하판*/, out OpenCvSharp.Point ptIntersection)
        {
            try
            {
                ptIntersection = new OpenCvSharp.Point();

                OpenCvSharp.Point p1 = BaseLine.Start; // BaseLine.ptStart부분 필요
                OpenCvSharp.Point p2 = BaseLine.End;
                OpenCvSharp.Point p3 = BaseTarget.Start;
                OpenCvSharp.Point p4 = BaseTarget.End;

                double dFactor = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);
                if (dFactor == 0)
                {
                    return false;
                }

                double dPre = (p1.X * p2.Y - p1.Y * p2.X);
                double dPost = (p3.X * p4.Y - p3.Y * p4.X);

                double dX = (dPre * (p3.X - p4.X) - (p1.X - p2.X) * dPost) / dFactor;
                double dY = (dPre * (p3.Y - p4.Y) - (p1.Y - p2.Y) * dPost) / dFactor;

                ptIntersection = new OpenCvSharp.Point(dX, dY);
                return true;
            }
            catch (Exception Desc)
            {
                ptIntersection = new OpenCvSharp.Point();
                return false;
            }

        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection2(Line BaseLine/*수직선*/, Line BaseTarget,
            out bool lines_intersect, out bool segments_intersect,
            out Point intersection/*,out Point2d close_p1, out Point2d close_p2*/)
        {
            Point2d close_p1;
            Point2d close_p2;
            Point p1 = BaseLine.Start;
            Point p2 = BaseLine.End;
            Point p3 = BaseTarget.Start;
            Point p4 = BaseTarget.End;

            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point(float.NaN, float.NaN);
                close_p1 = new Point2d(float.NaN, float.NaN);
                close_p2 = new Point2d(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }
            close_p1 = new Point2d(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point2d(p3.X + dx34 * t2, p3.Y + dy34 * t2);
            if (!double.IsNaN(close_p1.X) && !double.IsInfinity(close_p1.Y))
            {
                intersection = new Point(close_p1.X, close_p1.Y);
            }
            else if (!double.IsNaN(close_p2.X) && double.IsInfinity(close_p2.Y))
            {
                intersection = new Point(close_p2.X, close_p2.Y);
            }
        }

        public static bool CrossCheck(Point pt1, Point pt2, Point pt3, Point pt4)
        {

            try
            {
                bool bIsDivideLine1 = CCW(pt1, pt2, pt3, pt4);
                bool bIsDivideLine2 = CCW(pt3, pt4, pt1, pt2);

                if (bIsDivideLine1 && bIsDivideLine2)
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
                return false;
            }
        }

        public static bool CCW(Point pt1, Point pt2, Point pt3, Point pt4)
        {
            try
            {
                double dFactor = 0;

                double d1 = (pt2.X - pt1.X) * (pt3.Y - pt1.Y) - (pt2.Y - pt1.Y) * (pt3.X - pt1.X);
                double d2 = (pt2.X - pt1.X) * (pt4.Y - pt1.Y) - (pt2.Y - pt1.Y) * (pt4.X - pt1.X);

                dFactor = d1 * d2;

                if (dFactor < 0)
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
                return false;
            }
        }

        public static void FillPointFromLine(Point ptStart, Point ptEnd, IVision.Direction direction, out List<Point> listPtLine)
        {
            listPtLine = new List<Point>();
            Point point;
            int nVertY = 0;
            int nVertX = 0;
            if (ptEnd.X - ptStart.X != 0)
            {
                // 두점에 수직
                if (ptEnd.Y - ptStart.Y != 0)
                {
                    double dLineAngle = (ptEnd.Y - ptStart.Y) / (ptEnd.X - ptStart.X);
                    switch (direction)
                    {
                        case IVision.Direction.LeftToRight:
                            {
                                for (int ny = ptStart.Y + 1; ny < ptEnd.Y; ny++)
                                {
                                    nVertX = ((int)((ny - ptStart.Y) / dLineAngle) + ptStart.X);
                                    point.X = nVertX;
                                    point.Y = ny;
                                    listPtLine.Add(point);
                                }
                            }
                            break;

                        case IVision.Direction.RightToLeft:
                            {
                                for (int ny = ptStart.Y + 1; ny < ptEnd.Y; ny++)
                                {
                                    nVertX = ((int)((ny - ptStart.Y) / dLineAngle) + ptStart.X);
                                    point.X = nVertX;
                                    point.Y = ny;
                                    listPtLine.Add(point);
                                }
                            }
                            break;

                        case IVision.Direction.ToptoBottom:
                            {
                                for (int nx = ptStart.X + 1; nx < ptEnd.X; nx++)
                                {
                                    nVertY = ((int)(dLineAngle * (nx - ptStart.X)) + ptStart.Y);
                                    point.X = nx;
                                    point.Y = nVertY;
                                    listPtLine.Add(point);
                                }
                            }
                            break;

                        case IVision.Direction.BottomToTop:
                            {
                                for (int nx = ptStart.X + 1; nx < ptEnd.X; nx++)
                                {
                                    nVertY = ((int)(dLineAngle * (nx - ptStart.X)) + ptStart.Y);
                                    point.X = nx;
                                    point.Y = nVertY;
                                    listPtLine.Add(point);
                                }
                            }
                            break;
                    }
                }
                // X축에 평행한 직선
                else
                {
                    for (int nx = ptStart.X + 1; nx < ptEnd.X; nx++)
                    {
                        point.X = nx;
                        point.Y = ptStart.Y;
                        listPtLine.Add(point);
                    }
                }
            }
            // Y축에 평행한 직선
            else
            {
                for (int ny = ptStart.Y + 1; ny < ptEnd.Y; ny++)
                {
                    point.X = ptStart.X;
                    point.Y = ny;
                    listPtLine.Add(point);
                }
            }
        }

        public static void RestoreRansacData(List<Point> listIn, IVision.Direction direction, out List<Point> listRestoredData)
        {
            listRestoredData = new List<Point>();

            try
            {
                // 1. 중복데이터는 한개만 리스트에 한개만 담는다.
                listIn = listIn.Distinct().ToList();
                int nCount = listIn.Count;
                listRestoredData = listIn;
                switch (direction)
                {
                    case IVision.Direction.LeftToRight:
                        {
                            // 2. 인덱스가 누락된 데이터를 찾아 전 후 인덱스로 직선을 구한다.                                                       
                            for (int i = 0; i < nCount - 1; i++)
                            {
                                int nDiffY = listRestoredData[i + 1].Y - listRestoredData[i].Y;
                                if (nDiffY > 1)
                                {
                                    List<Point> listFillPointFromLine = new List<Point>();
                                    // 3. 누락된 인덱스를 지나는 점의 데이터를 구한다.
                                    FillPointFromLine(listRestoredData[i], listRestoredData[i + 1], direction, out listFillPointFromLine);

                                    for (int k = 0; k < listFillPointFromLine.Count; k++)
                                    {
                                        // 4. 누락된 인덱스가 복원 되었으므로 리스트에 넣어준다.
                                        listRestoredData.Add(listFillPointFromLine[k]);
                                    }
                                }
                            }
                            // 5. 데이터 Sorting 
                            listRestoredData = listRestoredData.OrderBy(p => p.Y).ToList();
                        }
                        break;
                    case IVision.Direction.RightToLeft:
                        {
                            // 2. 인덱스가 누락된 데이터를 찾아 전 후 인덱스로 직선을 구한다.                                                       
                            for (int i = 0; i < nCount - 1; i++)
                            {
                                int nDiffY = listRestoredData[i + 1].Y - listRestoredData[i].Y;
                                if (nDiffY > 1)
                                {
                                    List<Point> listFillPointFromLine = new List<Point>();
                                    // 3. 누락된 인덱스를 지나는 점의 데이터를 구한다.
                                    FillPointFromLine(listRestoredData[i], listRestoredData[i + 1], direction, out listFillPointFromLine);

                                    for (int k = 0; k < listFillPointFromLine.Count; k++)
                                    {
                                        // 4. 누락된 인덱스가 복원 되었으므로 리스트에 넣어준다.
                                        listRestoredData.Add(listFillPointFromLine[k]);
                                    }
                                }
                            }
                            // 5. 데이터 Sorting 
                            listRestoredData = listRestoredData.OrderBy(p => p.Y).ToList();
                        }
                        break;
                    case IVision.Direction.ToptoBottom:
                        {
                            // 2. 인덱스가 누락된 데이터를 찾아 전 후 인덱스로 직선을 구한다.                                                       
                            for (int i = 0; i < nCount - 1; i++)
                            {
                                int nDiffX = listRestoredData[i + 1].X - listRestoredData[i].X;
                                if (nDiffX > 1)
                                {
                                    List<Point> listFillPointFromLine = new List<Point>();
                                    // 3. 누락된 인덱스를 지나는 점의 데이터를 구한다.
                                    FillPointFromLine(listRestoredData[i], listRestoredData[i + 1], direction, out listFillPointFromLine);

                                    for (int k = 0; k < listFillPointFromLine.Count; k++)
                                    {
                                        // 4. 누락된 인덱스가 복원 되었으므로 리스트에 넣어준다.
                                        listRestoredData.Add(listFillPointFromLine[k]);
                                    }
                                }
                            }
                            // 5. 데이터 Sorting 
                            listRestoredData = listRestoredData.OrderBy(p => p.X).ToList();
                        }
                        break;
                    case IVision.Direction.BottomToTop:
                        {
                            // 2. 인덱스가 누락된 데이터를 찾아 전 후 인덱스로 직선을 구한다.                                                       
                            for (int i = 0; i < nCount - 1; i++)
                            {
                                int nDiffX = listRestoredData[i + 1].X - listRestoredData[i].X;
                                if (nDiffX > 1)
                                {
                                    List<Point> listFillPointFromLine = new List<Point>();
                                    // 3. 누락된 인덱스를 지나는 점의 데이터를 구한다.
                                    FillPointFromLine(listRestoredData[i], listRestoredData[i + 1], direction, out listFillPointFromLine);

                                    for (int k = 0; k < listFillPointFromLine.Count; k++)
                                    {
                                        // 4. 누락된 인덱스가 복원 되었으므로 리스트에 넣어준다.
                                        listRestoredData.Add(listFillPointFromLine[k]);
                                    }
                                }
                            }
                            // 5. 데이터 Sorting 
                            listRestoredData = listRestoredData.OrderBy(p => p.X).ToList();
                        }
                        break;
                }
            }
            catch
            {
                listRestoredData = new List<Point>();
            }
        }

        public static List<double> CrossVerticalGap(List<Point> listFitOut/*상판*/, List<Point> listFitIn/*하판*/, Rect rcRoi, int nRansacInterval, double dRansacAngleUpperLimit, double dRansacAngleLowerLimit, IVision.Direction direction, IVision.Position position, Mat matTest, out List<Line> listVerticalLine)
        {
            try
            {
                List<List<OpenCvSharp.Point>> listManageVertLine = new List<List<Point>>();
                Dictionary<int, Point> dicInPtIncrease = new Dictionary<int, Point>();
                Dictionary<int, Point> dicInPtDecrease = new Dictionary<int, Point>();
                List<OpenCvSharp.Point> listFitOutDistinct = new List<OpenCvSharp.Point>();
                List<OpenCvSharp.Point> listFitInDistinct = new List<OpenCvSharp.Point>();
                List<OpenCvSharp.Point> listFitVert = new List<OpenCvSharp.Point>();
                listVerticalLine = new List<Line>();
                bool bInterSection = false;
                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nRoiWidthToX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 중복 데이터 삭제
                if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    listFitOutDistinct = new List<OpenCvSharp.Point>(listFitOut);
                    listFitInDistinct = new List<OpenCvSharp.Point>(listFitIn);
                    listFitOutDistinct = listFitOutDistinct.Distinct().ToList();
                    listFitInDistinct = listFitInDistinct.Distinct().ToList();
                }
                else if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    listFitOutDistinct = new List<OpenCvSharp.Point>();
                    listFitInDistinct = new List<OpenCvSharp.Point>();
                    for (int nVertCount = 0; nVertCount < listFitOut.Count; nVertCount++)
                    {
                        if (!listFitOutDistinct.Contains(listFitOut[nVertCount]))
                        {
                            listFitOutDistinct.Add(listFitOut[nVertCount]);
                        }
                    }
                    for (int nInCount = 0; nInCount < listFitIn.Count; nInCount++)
                    {
                        if (!listFitInDistinct.Contains(listFitIn[nInCount]))
                        {
                            listFitInDistinct.Add(listFitIn[nInCount]);
                        }
                    }
                }
                switch (position)
                {
                    case IVision.Position.Left:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.LeftToRight, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.LeftToRight, out listFitInDistinct);
                        break;
                    case IVision.Position.Right:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.RightToLeft, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.RightToLeft, out listFitInDistinct);
                        break;
                    case IVision.Position.TopLeft:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.ToptoBottom, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.ToptoBottom, out listFitInDistinct);
                        break;
                    case IVision.Position.TopRight:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.ToptoBottom, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.ToptoBottom, out listFitInDistinct);
                        break;
                    case IVision.Position.BottomLeft:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.BottomToTop, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.BottomToTop, out listFitInDistinct);
                        break;
                    case IVision.Position.BottomRight:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.BottomToTop, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.BottomToTop, out listFitInDistinct);
                        break;
                }

                List<Point> listPoint = new List<Point>();
                bool bFindDiff = false;
                bool bFirstClear = true;
                double dInclination = 0;
                IVision.GetLineDiffAngle(listFitInDistinct, direction, out dicInPtIncrease, out dicInPtDecrease);
                if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    // 1. 하판 기준 수직선을 만들어서 리스트에 보관      
                    for (int nInCount = 0; nInCount < listFitInDistinct.Count; nInCount++)
                    {
                        Point PtVerticalLine = new Point();
                        Point ptStart = new Point();
                        Point ptEnd = new Point();

                        // 1. 기울기를 얻기전 리스트에 엣지를 보관
                        if (!bFindDiff)
                        {
                            listPoint.Add(listFitInDistinct[nInCount]);
                        }

                        // 2. 증가된 엣지의 인덱스가 키값과 같다면
                        if (dicInPtIncrease.ContainsKey(nInCount))
                        {
                            // 3. 변곡점을 End로
                            if (dicInPtIncrease.TryGetValue(nInCount, out ptEnd))
                            {
                                // 4. 변곡점을 얻기 이전의 엣지의 First 엣지를 Start로 
                                ptStart = listPoint[0];
                                // 5. 기울기 계산
                                dInclination = (double)(ptEnd.Y - ptStart.Y) / (double)(ptEnd.X - ptStart.X);
                                // 6. bFindDiff = true면 이제 수직선 생성
                                bFindDiff = true;
                            }
                        }
                        // 7. Increase와 동일
                        else if (dicInPtDecrease.ContainsKey(nInCount))
                        {
                            if (dicInPtDecrease.TryGetValue(nInCount, out ptEnd))
                            {
                                ptStart = listPoint[0];
                                dInclination = (double)(ptEnd.Y - ptStart.Y) / (double)(ptEnd.X - ptStart.X);
                                bFindDiff = true;
                            }
                        }

                        if (bFindDiff)
                        {
                            int nListCount = listPoint.Count;
                            for (int nIndex = 0; nIndex < nListCount - 1; nIndex++)
                            {
                                // 8. 처음에는 변곡점 +1의 값을 가져와야 하기에 아래의 부분처럼 플래그 처리
                                if (bFirstClear)
                                {
                                    ptStart = listPoint[nIndex];
                                    ptEnd = listPoint[nIndex + 1];
                                }
                                else
                                {
                                    ptStart = listPoint[0];
                                    ptEnd = listPoint[1];
                                }

                                GetLineCoef(ptStart, ptEnd, rcRoi, direction, dInclination, out PtVerticalLine);
                                listFitVert.Add(PtVerticalLine);
                                if (!bFirstClear)
                                {
                                    listPoint.RemoveAt(0);
                                }
                            }
                            if (bFirstClear)
                            {
                                listPoint.Clear();
                                bFirstClear = false;
                            }
                            bFindDiff = false;
                        }
                    }
                }
                else if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    int nDistance = (int)(Math.Abs(listFitInDistinct[0].DistanceTo(listFitInDistinct[listFitInDistinct.Count - 1]) / 2));

                    OpenCvSharp.Line2D Line = new OpenCvSharp.Line2D(0, 0, 0, 0);
                    List<OpenCvSharp.Line2D> listfit = new List<OpenCvSharp.Line2D>();
                    Line = Cv2.FitLine(listFitInDistinct, DistanceTypes.L2, 0, 0.01, 0.01);
                    int nStartX = (int)(Line.X1 - nDistance * Line.Vx);
                    int nEndX = (int)(Line.X1 + nDistance * Line.Vx);
                    int nStartY = (int)(Line.Y1 - nDistance * Line.Vy);
                    int nEndY = (int)(Line.Y1 + nDistance * Line.Vy);
                    Point ptStart = new Point(nStartX, nStartY);
                    Point ptEnd = new Point(nEndX, nEndY);
                    double angle2 = (double)(ptEnd.Y - ptStart.Y) / (double)(ptEnd.X - ptStart.X);
                    double angle = Math.Tan(Line.GetVectorRadian());

                    //matTest.Line(ptStart, ptEnd, Scalar.Yellow, 1);
                    //matTest.Rectangle(rcRoi, Scalar.Blue, 1);
                    //matTest.SaveImage(Application.StartupPath + "\\Cam_vector" + ".bmp");
                    for (int nInCount = 0; nInCount < listFitInDistinct.Count - 1; nInCount++)
                    {
                        List<Point> listPtVerticalLine = new List<Point>();
                        GetLineCoef(listFitInDistinct[nInCount], listFitInDistinct[nInCount + 1], rcRoi, direction, angle, out listPtVerticalLine);
                        listManageVertLine.Add(listPtVerticalLine);
                        //listFitVert.Add(PtVerticalLine);
                    }
                }

                if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    listFitVert = listFitVert.OrderBy(p => p.Y).ToList();
                    listFitOutDistinct = listFitOutDistinct.OrderBy(p => p.Y).ToList();
                    listFitInDistinct = listFitInDistinct.OrderBy(p => p.Y).ToList();
                }
                else if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    listFitVert = listFitVert.OrderBy(p => p.X).ToList();
                    listFitOutDistinct = listFitOutDistinct.OrderBy(p => p.X).ToList();
                    listFitInDistinct = listFitInDistinct.OrderBy(p => p.X).ToList();
                }

                // 2. 하판 기준 수직선이 상판에도 교차하는지 여부를 확인
                Line lineTop = new Line();
                Line lineVertical = new Line();
                Line lineGap = new Line();
                List<Line> listLine = new List<Line>();
                OpenCvSharp.Point ptIntersection = new OpenCvSharp.Point();
                OpenCvSharp.Point ptIntersec = new OpenCvSharp.Point();
                List<double> listDouble = new List<double>();
                Line lineRoi = new Line();

                if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    for (int nVertCount = 0; nVertCount < listFitVert.Count - 1; nVertCount++)
                    {
                        if (nVertCount >= listFitOutDistinct.Count - 1)
                        {
                            break;
                        }
                        for (int nOutCount = 0; nOutCount < listFitOutDistinct.Count - 1; nOutCount++)
                        {
                            bInterSection = CrossCheck(listFitOutDistinct[nOutCount], listFitOutDistinct[nOutCount + 1], listFitInDistinct[nVertCount], listFitVert[nVertCount]);

                            // 3. 교차한다면 
                            if (bInterSection)
                            {
                                Point ptStart = new Point();
                                Point ptEnd = new Point();

                                switch (direction)
                                {
                                    case IVision.Direction.LeftToRight:
                                        ptStart.X = nStartRoiX;
                                        ptStart.Y = nStartRoiY;
                                        ptEnd.X = nStartRoiX;
                                        ptEnd.Y = nEndRoiY;
                                        break;
                                    case IVision.Direction.RightToLeft:
                                        ptStart.X = nRoiWidthToX;
                                        ptStart.Y = nStartRoiY;
                                        ptEnd.X = nRoiWidthToX;
                                        ptEnd.Y = nEndRoiY;
                                        break;
                                    case IVision.Direction.ToptoBottom:
                                        ptStart.X = nStartRoiX;
                                        ptStart.Y = nStartRoiY;
                                        ptEnd.X = nRoiWidthToX;
                                        ptEnd.Y = nStartRoiY;
                                        break;
                                    case IVision.Direction.BottomToTop:
                                        ptStart.X = nStartRoiX;
                                        ptStart.Y = nEndRoiY;
                                        ptEnd.X = nRoiWidthToX;
                                        ptEnd.Y = nEndRoiY;
                                        break;
                                }
                                // ROI 직선
                                lineRoi = new Line(ptStart, ptEnd);
                                // 상판
                                lineTop = new Line((OpenCvSharp.Point)listFitOutDistinct[nOutCount], ((OpenCvSharp.Point)listFitOutDistinct[nOutCount + 1]));
                                // 수직선 line
                                lineVertical = new Line((OpenCvSharp.Point)listFitInDistinct[nVertCount], (OpenCvSharp.Point)listFitVert[nVertCount]);
                                // 교점 구하기
                                // 수직선 교점
                                FindIntersection(lineVertical, lineTop, out ptIntersection);
                                // ROI 교점
                                FindIntersection(lineVertical, lineRoi, out OpenCvSharp.Point ptIntersectionRoi);

                                lineGap = new Line((OpenCvSharp.Point)listFitInDistinct[nVertCount], ptIntersection);
                                listFitVert[nVertCount] = new Point(ptIntersection.X, ptIntersection.Y);
                                lineVertical = new Line((OpenCvSharp.Point)listFitInDistinct[nVertCount], (OpenCvSharp.Point)listFitVert[nVertCount]);

                                // 4. Gap 측정 
                                double dDegree = IVision.threePointAngle(ptIntersectionRoi, lineVertical.Start, ptStart);
                                if (nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineVertical);
                                }
                                else if (nVertCount % nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineVertical);
                                }
                                if (dDegree < dRansacAngleUpperLimit && dDegree > dRansacAngleLowerLimit)// 15)
                                {

                                }
                                listDouble.Add(dDegree);
                                listLine.Add(lineGap);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int nManagerLineCount = 0; nManagerLineCount < listManageVertLine.Count - 1; nManagerLineCount++)
                    {
                        for (int nOutCount = 0; nOutCount < listFitOutDistinct.Count - 1; nOutCount++)
                        {
                            bInterSection = CrossCheck(listFitOutDistinct[nOutCount], listFitOutDistinct[nOutCount + 1], listManageVertLine[nManagerLineCount][0], listManageVertLine[nManagerLineCount][listManageVertLine[nManagerLineCount].Count - 1]);

                            // 3. 교차한다면 
                            if (bInterSection)
                            {
                                Line lineIntersection = new Line();
                                // 상판
                                lineTop = new Line((OpenCvSharp.Point)listFitOutDistinct[nOutCount], ((OpenCvSharp.Point)listFitOutDistinct[nOutCount + 1]));
                                // 수직선 line
                                lineVertical = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], (OpenCvSharp.Point)listManageVertLine[nManagerLineCount][listManageVertLine[nManagerLineCount].Count - 1]);
                                // 교점 구하기
                                // 수직선 교점
                                FindIntersection(lineVertical, lineTop, out ptIntersection);
                                // ROI 교점
                                FindIntersection(lineVertical, lineRoi, out OpenCvSharp.Point ptIntersectionRoi);

                                lineGap = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], ptIntersection);
                                //listManageVertLine[nManagerLineCount][nVertCount] = new Point(ptIntersection.X, ptIntersection.Y);
                                //lineVertical = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], (OpenCvSharp.Point)listManageVertLine[nManagerLineCount][nVertCount]);
                                lineIntersection = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], ptIntersection);
                                // 4. Gap 측정 
                                if (nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineIntersection);
                                    //listVerticalLine.Add(lineVertical);
                                }
                                else if (nOutCount % nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineIntersection);
                                    //listVerticalLine.Add(lineVertical);
                                }
                                //listVerticalLine.Add(lineVertical);
                                listLine.Add(lineGap);
                                break;
                            }
                        }
                    }
                }

                List<double> listGap = new List<double>();
                double dSumTemp = 0.0D;

                if (direction == Direction.BottomToTop || direction == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();
                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (direction == Direction.LeftToRight || direction == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();
                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }
                                }
                            }
                        }
                    }
                }
                return listGap;
            }
            catch (Exception Desc)
            {
                listVerticalLine = new List<Line>();
                return null;
            }

        }

        public static List<double> CrossVerticalGap(List<Point> listFitOut/*상판*/, List<Point> listFitIn/*하판*/, Rect rcRoi, int nRansacInterval, double dRansacAngleUpperLimit, double dRansacAngleLowerLimit, IVision.Direction direction, IVision.Position position, out List<Line> listVerticalLine)
        {
            try
            {
                List<List<OpenCvSharp.Point>> listManageVertLine = new List<List<Point>>();
                Dictionary<int, Point> dicInPtIncrease = new Dictionary<int, Point>();
                Dictionary<int, Point> dicInPtDecrease = new Dictionary<int, Point>();
                List<OpenCvSharp.Point> listFitOutDistinct = new List<OpenCvSharp.Point>();
                List<OpenCvSharp.Point> listFitInDistinct = new List<OpenCvSharp.Point>();
                List<OpenCvSharp.Point> listFitVert = new List<OpenCvSharp.Point>();
                listVerticalLine = new List<Line>();
                bool bInterSection = false;
                int nStartRoiX = rcRoi.X;
                int nStartRoiY = rcRoi.Y;
                int nRoiWidthToX = rcRoi.X + rcRoi.Width;
                int nEndRoiY = rcRoi.Y + rcRoi.Height;

                // 중복 데이터 삭제
                if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    listFitOutDistinct = new List<OpenCvSharp.Point>(listFitOut);
                    listFitInDistinct = new List<OpenCvSharp.Point>(listFitIn);
                    listFitOutDistinct = listFitOutDistinct.Distinct().ToList();
                    listFitInDistinct = listFitInDistinct.Distinct().ToList();
                }
                else if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    listFitOutDistinct = new List<OpenCvSharp.Point>();
                    listFitInDistinct = new List<OpenCvSharp.Point>();
                    for (int nVertCount = 0; nVertCount < listFitOut.Count; nVertCount++)
                    {
                        if (!listFitOutDistinct.Contains(listFitOut[nVertCount]))
                        {
                            listFitOutDistinct.Add(listFitOut[nVertCount]);
                        }
                    }
                    for (int nInCount = 0; nInCount < listFitIn.Count; nInCount++)
                    {
                        if (!listFitInDistinct.Contains(listFitIn[nInCount]))
                        {
                            listFitInDistinct.Add(listFitIn[nInCount]);
                        }
                    }
                }
                switch (position)
                {
                    case IVision.Position.Left:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.LeftToRight, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.LeftToRight, out listFitInDistinct);
                        break;
                    case IVision.Position.Right:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.RightToLeft, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.RightToLeft, out listFitInDistinct);
                        break;
                    case IVision.Position.TopLeft:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.ToptoBottom, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.ToptoBottom, out listFitInDistinct);
                        break;
                    case IVision.Position.TopRight:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.ToptoBottom, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.ToptoBottom, out listFitInDistinct);
                        break;
                    case IVision.Position.BottomLeft:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.BottomToTop, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.BottomToTop, out listFitInDistinct);
                        break;
                    case IVision.Position.BottomRight:
                        IVision.RestoreRansacData(listFitOut, IVision.Direction.BottomToTop, out listFitOutDistinct);
                        IVision.RestoreRansacData(listFitIn, IVision.Direction.BottomToTop, out listFitInDistinct);
                        break;
                }

                OpenCvSharp.Line2D Line = new OpenCvSharp.Line2D(0, 0, 0, 0);
                Line = Cv2.FitLine(listFitInDistinct, DistanceTypes.L2, 0, 0.01, 0.01);
                double dAngle = Math.Tan(Line.GetVectorRadian());

                for (int nInCount = 0; nInCount < listFitInDistinct.Count - 1; nInCount++)
                {
                    List<Point> listPtVerticalLine = new List<Point>();
                    GetLineCoef(listFitInDistinct[nInCount], listFitInDistinct[nInCount + 1], rcRoi, direction, dAngle, out listPtVerticalLine);
                    listManageVertLine.Add(listPtVerticalLine);
                }

                if (direction == IVision.Direction.LeftToRight || direction == IVision.Direction.RightToLeft)
                {
                    listFitVert = listFitVert.OrderBy(p => p.Y).ToList();
                    listFitOutDistinct = listFitOutDistinct.OrderBy(p => p.Y).ToList();
                    listFitInDistinct = listFitInDistinct.OrderBy(p => p.Y).ToList();
                }
                else if (direction == IVision.Direction.ToptoBottom || direction == IVision.Direction.BottomToTop)
                {
                    //for (int nIndex = 0; nIndex < listManageVertLine.Count - 1; nIndex++)
                    //{
                    //    listManageVertLine[nIndex].OrderBy(p => p.Y).ToList();
                    //}
                    //listFitVert = listFitVert.OrderBy(p => p.X).ToList();
                    listFitOutDistinct = listFitOutDistinct.OrderBy(p => p.X).ToList();
                    listFitInDistinct = listFitInDistinct.OrderBy(p => p.X).ToList();
                }

                // 2. 하판 기준 수직선이 상판에도 교차하는지 여부를 확인
                Line lineTop = new Line();
                Line lineVertical = new Line();
                Line lineGap = new Line();
                List<Line> listLine = new List<Line>();
                OpenCvSharp.Point ptIntersection = new OpenCvSharp.Point();
                OpenCvSharp.Point ptIntersec = new OpenCvSharp.Point();
                List<double> listDouble = new List<double>();
                Line lineRoi = new Line();

                for (int nManagerLineCount = 0; nManagerLineCount < listManageVertLine.Count - 1; nManagerLineCount++)
                {
                    for (int nOutCount = 0; nOutCount < listFitOutDistinct.Count - 1; nOutCount++)
                    {
                        bInterSection = CrossCheck(listFitOutDistinct[nOutCount], listFitOutDistinct[nOutCount + 1], listManageVertLine[nManagerLineCount][0], listManageVertLine[nManagerLineCount][listManageVertLine[nManagerLineCount].Count - 1]);
                        //bInterSection = CCW(listFitOutDistinct[nOutCount], listManageVertLine[nManagerLineCount][0], listManageVertLine[nManagerLineCount][listManageVertLine[nManagerLineCount].Count - 1]);
                        // 3. 교차한다면 
                        if (bInterSection)
                        {
                            Point ptStart = new Point();
                            Point ptEnd = new Point();

                            switch (direction)
                            {
                                case IVision.Direction.LeftToRight:
                                    ptStart.X = nStartRoiX;
                                    ptStart.Y = nStartRoiY;
                                    ptEnd.X = nStartRoiX;
                                    ptEnd.Y = nEndRoiY;
                                    break;
                                case IVision.Direction.RightToLeft:
                                    ptStart.X = nRoiWidthToX;
                                    ptStart.Y = nStartRoiY;
                                    ptEnd.X = nRoiWidthToX;
                                    ptEnd.Y = nEndRoiY;
                                    break;
                                case IVision.Direction.ToptoBottom:
                                    ptStart.X = nStartRoiX;
                                    ptStart.Y = nStartRoiY;
                                    ptEnd.X = nRoiWidthToX;
                                    ptEnd.Y = nStartRoiY;
                                    break;
                                case IVision.Direction.BottomToTop:
                                    ptStart.X = nStartRoiX;
                                    ptStart.Y = nEndRoiY;
                                    ptEnd.X = nRoiWidthToX;
                                    ptEnd.Y = nEndRoiY;
                                    break;
                            }
                            // ROI 직선
                            lineRoi = new Line(ptStart, ptEnd);

                            Line lineIntersection = new Line();
                            // 상판
                            lineTop = new Line((OpenCvSharp.Point)listFitOutDistinct[nOutCount], ((OpenCvSharp.Point)listFitOutDistinct[nOutCount + 1]));
                            // 수직선 line
                            lineVertical = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], (OpenCvSharp.Point)listManageVertLine[nManagerLineCount][listManageVertLine[nManagerLineCount].Count - 1]);
                            // 교점 구하기
                            // 수직선 교점
                            FindIntersection(lineVertical, lineTop, out ptIntersection);
                            // ROI 교점
                            FindIntersection(lineVertical, lineRoi, out OpenCvSharp.Point ptIntersectionRoi);

                            lineGap = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], ptIntersection);
                            lineIntersection = new Line((OpenCvSharp.Point)listFitInDistinct[nManagerLineCount], ptIntersection);
                            double dDegree = IVision.threePointAngle(ptIntersectionRoi, lineVertical.Start, ptStart);
                            if (dDegree < dRansacAngleUpperLimit && dDegree > dRansacAngleLowerLimit)// 15)
                            {
                                if (nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineIntersection);
                                }
                                else if (nManagerLineCount % nRansacInterval == 0)
                                {
                                    listVerticalLine.Add(lineIntersection);
                                }
                                // 4. Gap 측정 
                                listLine.Add(lineGap);
                            }

                            break;
                        }
                    }
                }

                List<double> listGap = new List<double>();
                double dSumTemp = 0.0D;

                if (direction == Direction.BottomToTop || direction == Direction.ToptoBottom)
                {
                    List<double> listCenterY = new List<double>();
                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterY = (line.Start.Y + line.Distance() / 2.0D);
                        listCenterY.Add(dCenterY);
                    }

                    double dCenterYAvg = listCenterY.Average();
                    double dAverageDist = dSumTemp / listCenterY.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterY = (line.Start.Y + dDist / 2.0D);

                            if (dCenterY > (dCenterYAvg - (dAverageDist))
                                && dCenterY < (dCenterYAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (direction == Direction.LeftToRight || direction == Direction.RightToLeft)
                {
                    List<double> listCenterX = new List<double>();
                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];
                        double dDist = line.Distance();
                        dSumTemp += dDist;

                        double dCenterX = (line.Start.X + line.Distance() / 2.0D);
                        listCenterX.Add(dCenterX);
                    }

                    double dCenterXAvg = listCenterX.Average();
                    double dAverageDist = dSumTemp / listCenterX.Count;

                    for (int i = 0; i < listLine.Count; i++)
                    {
                        Line line = listLine[i];

                        if (line != null)
                        {
                            double dDist = line.Distance();
                            double dCenterX = (line.Start.X + dDist / 2.0D);

                            if (dCenterX > (dCenterXAvg - (dAverageDist))
                                && dCenterX < (dCenterXAvg + (dAverageDist)))
                            {
                                if (dDist >= (dAverageDist * 0.8D)
                                    && dDist <= (dAverageDist * 1.2D))
                                {
                                    lock (lockObject)
                                    {
                                        listGap.Add(dDist);
                                    }
                                }
                            }
                        }
                    }
                }
                return listGap;
            }
            catch (Exception Desc)
            {
                listVerticalLine = new List<Line>();
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return null;
            }

        }

        public static bool IsMatEmpty(Mat MatSource)
        {
            try
            {
                if (MatSource.IsDisposed)
                {
                    return true;
                }

                if (MatSource == null)
                {
                    return true;
                }

                if (MatSource.Width == 0 || MatSource.Height == 0)
                {
                    return true;
                }
                return false;
            }

            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
    }
}