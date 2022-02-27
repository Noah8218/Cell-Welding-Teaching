using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFactory
{
    public static class CConverter
    {
        public static string ColorToString(Color cr)
        {
            return string.Format("{0},{1},{2}", cr.R, cr.G, cr.B);
        }

        public static Color StringToColor(string strColor)
        {
            try
            {
                string[] strSplit = strColor.Split(',');
                if (strSplit.Length == 3)
                {
                    return Color.FromArgb(int.Parse(strSplit[0]), int.Parse(strSplit[1]), int.Parse(strSplit[2]));
                }
                else
                {
                    return Color.White;
                }
            }
            catch
            {
                return Color.White;
            }
        }

        public static Bitmap ToBitmap(OpenCvSharp.Mat image)
        {
            try
            {
                return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            }
            catch(Exception ex)
            {
                return new Bitmap(10, 10);
            }
            
        }
        public static Bitmap ByteToBitmap(byte[] imgArr, int nW, int nH)
        {
            Bitmap bmp = new Bitmap(nW, 4000, PixelFormat.Format8bppIndexed);
            // IntPtr ptr = res.GetHbitmap();

            BitmapData data = bmp.LockBits(
                                    new Rectangle(0, 0, nW, nH),
                                    ImageLockMode.ReadWrite,
                                        PixelFormat.Format8bppIndexed);
            IntPtr ptr = data.Scan0;
            Marshal.Copy(imgArr, 0, ptr, nW * nH);
            bmp.UnlockBits(data);

            //모노이미지로 변환해준다 사용하지 않을경우 칼라이미지가 깨진채로 사용된다
            ColorPalette Gpal = bmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                Gpal.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = Gpal;

            return bmp;
        }

        public static Rectangle StringToRoi(string strROI)
        {
            Rectangle ROI = new Rectangle();
            string[] strSplit = strROI.Split(',');

            if (strSplit.Length == 4)
            {
                int nX = int.Parse(strSplit[0]);
                int nY = int.Parse(strSplit[1]);
                int nW = int.Parse(strSplit[2]);
                int nH = int.Parse(strSplit[3]);

                ROI = new Rectangle(nX, nY, nW, nH);
            }

            return ROI;
        }

        public static string RoiToString(OpenCvSharp.Rect ROI)
        {
            return string.Format("{0},{1},{2},{3}", ROI.X, ROI.Y, ROI.Width, ROI.Height);
        }

        public static string RoiToString(Rectangle ROI)
        {
            return string.Format("{0},{1},{2},{3}", ROI.X, ROI.Y, ROI.Width, ROI.Height);
        }

        public static System.Drawing.Point CVPointToPoint(OpenCvSharp.Point pt)
        {
            return new System.Drawing.Point(pt.X, pt.Y);
        }

        public static OpenCvSharp.Point PointToCVPoint(System.Drawing.Point pt)
        {
            return new OpenCvSharp.Point(pt.X, pt.Y);
        }

        public static string ShortToBinaryString(short shValue)
        {
            string strBinary = Convert.ToString(shValue, 2).PadLeft(8, '0');
            return strBinary;
        }

        public static string IntToBinaryString(int nValue, int nZeroCount)
        {
            string strBinary = Convert.ToString(nValue, 2).PadLeft(nZeroCount, '0');
            return strBinary;
        }

        public static byte IntToByte(int nValue)
        {
            //string hexValue = nValue.ToString("X");
            return Convert.ToByte(nValue.ToString());
        }

        public static string PointToString(OpenCvSharp.Point pt)
        {
            return string.Format("{0},{1}", pt.X, pt.Y);
        }

        public static OpenCvSharp.Point StringToPoint(string strPoint)
        {
            string[] strPointSplit = strPoint.Split(',');
            OpenCvSharp.Point pt = new OpenCvSharp.Point(0, 0);

            if (strPointSplit.Length == 2)
            {
                string strX = strPointSplit[0].Trim();
                string strY = strPointSplit[1].Trim();

                int nX = int.Parse(strX);
                int nY = int.Parse(strY);

                pt = new OpenCvSharp.Point(nX, nY);
            }

            return pt;
        }

        public static string RectToString(Rectangle rt)
        {
            return $"{rt.X},{rt.Y},{rt.Width},{rt.Height}";
        }

        public static Rectangle StringToRect(string strRect)
        {
            string[] sRT = strRect.Split(',');
            if(sRT.Length == 4)
            {
                int nX = int.Parse(sRT[0]);
                int nY = int.Parse(sRT[1]);
                int nW = int.Parse(sRT[2]);
                int nH = int.Parse(sRT[3]);

                return new Rectangle(nX, nY, nW, nH);
            }


            return new Rectangle();
        }

        public static OpenCvSharp.Rect StringToCVRect(string strRect)
        {
            string[] sRT = strRect.Split(',');
            if (sRT.Length == 4)
            {
                int nX = int.Parse(sRT[0]);
                int nY = int.Parse(sRT[1]);
                int nW = int.Parse(sRT[2]);
                int nH = int.Parse(sRT[3]);

                return new OpenCvSharp.Rect(nX, nY, nW, nH);
            }


            return new OpenCvSharp.Rect();
        }

        public static System.Drawing.Point RectangleToCenter(System.Drawing.Rectangle rt)
        {            
            return new System.Drawing.Point(rt.X + rt.Width/2, rt.Y + rt.Height/2);
        }

        public static System.Drawing.Point RectToCenter(OpenCvSharp.Rect rt)
        {
            return new System.Drawing.Point(rt.X, rt.Y);
        }

        public static System.Drawing.PointF RectToCenterF(System.Drawing.RectangleF rt)
        {
            return new System.Drawing.PointF(rt.X, rt.Y);
        }

        public static OpenCvSharp.Rect RectToCVRect(System.Drawing.Rectangle rt)
        {
            OpenCvSharp.Rect CvRt = new OpenCvSharp.Rect(rt.X, rt.Y, rt.Width, rt.Height);
            return CvRt;
        }
        public static System.Drawing.Rectangle CVRectToRect(OpenCvSharp.Rect rt)
        {
            System.Drawing.Rectangle dRt = new System.Drawing.Rectangle(rt.X, rt.Y, rt.Width, rt.Height);
            return dRt;
        }

        public static System.Drawing.Rectangle ScreenRectToLogicalRect(System.Drawing.Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X / fScaleFactorX);
            rt.Y = (int)(rt.Y / fScaleFactorY);
            rt.Width = (int)(rt.Width / fScaleFactorX);
            rt.Height = (int)(rt.Height / fScaleFactorY);

            return rt;
        }

        public static System.Drawing.Rectangle LogicalRectToScreenRect(System.Drawing.Rectangle rt, float fScaleFactorX, float fScaleFactorY)
        {
            System.Drawing.Rectangle rtScreen = new System.Drawing.Rectangle();
            rtScreen.X = (int)(rt.X * fScaleFactorX);
            rtScreen.Y = (int)(rt.Y * fScaleFactorY);
            rtScreen.Width = (int)(rt.Width * fScaleFactorX);
            rtScreen.Height = (int)(rt.Height * fScaleFactorY);

            return rtScreen;
        }

        public static OpenCvSharp.Rect ScreenCVRectToLogicalCVRect(OpenCvSharp.Rect rt, float fScaleFactorX, float fScaleFactorY)
        {
            rt.X = (int)(rt.X * fScaleFactorX);
            rt.Y = (int)(rt.Y * fScaleFactorY);
            rt.Width = (int)(rt.Width * fScaleFactorX);
            rt.Height = (int)(rt.Height * fScaleFactorY);

            return rt;
        }

        public static System.Drawing.Point LogicalPointToScreenPoint(System.Drawing.Point pt, float fScaleFactorX, float fScaleFactorY)
        {
            System.Drawing.Point ptScreen = new System.Drawing.Point();

            ptScreen.X = (int)(pt.X * fScaleFactorX);
            ptScreen.Y = (int)(pt.Y * fScaleFactorY);

            return ptScreen;
        }
    }
}
