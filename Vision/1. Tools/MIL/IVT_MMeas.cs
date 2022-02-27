#if MIL
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

using System.Text;
using Matrox.MatroxImagingLibrary;
using System.Reflection;

namespace IntelligentFactory
{
    public class IVP_MMeas
    {
        public Rectangle ROI = new Rectangle();

        public int PITCH = 10;

        public const int POLARITY_WTOB = MIL.M_POSITIVE;
        public const int POLARITY_BTOW = MIL.M_NEGATIVE;

        public int POLARITY_LEFT  = MIL.M_POSITIVE;
        public int POLARITY_RIGHT = MIL.M_NEGATIVE;

        public int THICKNESS = 10;

        public int WIDTH = 50;
        public int WIDTH_VARIATION = 10;

        public bool ISDRAW = false;

    }

    public static class IVT_MMeas
    {
        public static Rectangle ROI = new Rectangle(0, 0, 4096, 4000);

        public static MIL_ID MilDisplay = MIL.M_NULL;           // Display identifier.
        public static MIL_ID MilImage = MIL.M_NULL;             // Image buffer identifier.
        public static MIL_ID MilOverlayImage = MIL.M_NULL;      // Image buffer identifier.
        public static MIL_ID MilGraphicList = MIL.M_NULL;       // Graphic list identifier.
        public static MIL_ID StripeMarker = MIL.M_NULL;         // Stripe marker identifier.

        public static double StripeCenterX = 0.0;               // Stripe X center position.
        public static double StripeCenterY = 0.0;               // Stripe Y center position.
        public static double StripeWidth = 0.0;                 // Stripe width.
        public static double StripeAngle = 0.0;                 // Stripe angle.
        public static double CrossColor = MIL.M_COLOR_YELLOW;   // Cross drawing color.
        public static double BoxColor = MIL.M_COLOR_RED;        // Box drawing color.


        public static bool Align(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID image, Rectangle rtROI, out int nFirstX, out int nSecondX)
        {
            nFirstX = 0;
            nSecondX = 0;

            try
            {                
                MilImage = image;

                double dFirst = 0.0D;
                double dSecond = 0.0D;
                // Allocate a stripe marker.
                MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

                //찾을 줄무늬 개수 설정 ==> ALL
                //MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);

                //찾을 ROI 설정, 원점 및 넓이/높이
                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, rtROI.X, rtROI.Y);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, rtROI.Width, rtROI.Height);

                //실행할 개수
                MIL.MmeasSetMarker(StripeMarker, MIL.M_SUB_REGIONS_NUMBER, 1, MIL.M_NULL);

                //FILTER _ SMOOTHING
                MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_TYPE, MIL.M_SHEN, MIL.M_NULL);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_SMOOTHNESS, 100.0D, MIL.M_NULL);

                //찾을 줄무늬의 좌우 에지를 찾을 극성
                MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, IVP_MMeas.POLARITY_BTOW, IVP_MMeas.POLARITY_BTOW);

                //측정
                MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_DEFAULT);

                //측정 개수 및 위치 가져오기
                MIL_INT MeasCount = 0;
                MIL.MmeasGetResult(StripeMarker, MIL.M_NUMBER + MIL.M_TYPE_LONG, ref MeasCount, MIL.M_NULL);

                if (MeasCount > 0)
                {
                    MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_FIRST, ref dFirst);
                    MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_SECOND, ref dSecond);

                    nFirstX = (int)dFirst;
                    nSecondX = (int)dSecond;
                }

                MIL.MmeasFree(StripeMarker);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public static List<IVT_MEAS_RESULT> TotalRun(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID image, IVP_MMeas Param1, IVP_MMeas Param2, IntPtr Handle)
        {
            List<IVT_MEAS_RESULT> Results = new List<IVT_MEAS_RESULT>();

            try
            {
                Rectangle ROI1 = new Rectangle();
                Rectangle ROI2 = new Rectangle();                

                if (Param1.ROI.Width == 0 || Param1.ROI.Height == 0) ROI1 = new Rectangle(0, 0, 4096, 4000);
                else ROI1 = Param1.ROI;

                if (Param2.ROI.Width == 0 || Param2.ROI.Height == 0) ROI2 = new Rectangle(0, 0, 4096, 4000);
                else ROI2 = Param2.ROI;

                MilImage = image;
                // 이미지 버퍼를 표시합니다.
                //MIL.MdispSelect(MilDisplay, MilImage);

                if (Param1.ISDRAW) MIL.MdispSelectWindow(MilDisplay, MilImage, Handle);

                // Allocate a graphic list to hold the subpixel annotations to draw.
                if (Param1.ISDRAW) MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

                // 오버레이 주석을 준비합니다.
                //MIL.MdispControl(MilDisplay, MIL.M_OVERLAY, MIL.M_ENABLE);
                //MIL.MdispInquire(MilDisplay, MIL.M_OVERLAY_ID, ref MilOverlayImage);

                if (Param1.ISDRAW) MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
                if (Param1.ISDRAW) MIL.MgraRect(MIL.M_DEFAULT, MilOverlayImage, ROI.X, ROI.Y, ROI.X + ROI.Width, ROI.Y + ROI.Height);

                int nRepeat = ROI.Height / Param1.PITCH;

                if (Param1.ISDRAW) MIL.MgraColor(MIL.M_DEFAULT, CrossColor);

                for (int i = 0; i < nRepeat; i++)
                {
                    double dFirst = 0.0D;
                    double dSecond = 0.0D;
                    // Allocate a stripe marker.
                    MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

                    //찾을 줄무늬 개수 설정 ==> ALL
                    //MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);

                    //찾을 ROI 설정, 원점 및 넓이/높이
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, ROI1.X, ROI1.Y + (i * Param1.PITCH));
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, ROI1.Width, Param1.PITCH);

                    //실행할 개수
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_SUB_REGIONS_NUMBER, 1, MIL.M_NULL);

                    //FILTER _ SMOOTHING
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_TYPE, MIL.M_SHEN, MIL.M_NULL);
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_SMOOTHNESS, 75D, MIL.M_NULL);

                    //찾을 줄무늬의 좌우 에지를 찾을 극성
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, Param1.POLARITY_LEFT, Param1.POLARITY_RIGHT);

                    //측정
                    MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_DEFAULT);

                    //측정 개수 및 위치 가져오기
                    MIL_INT MeasCount = 0;
                    MIL.MmeasGetResult(StripeMarker, MIL.M_NUMBER + MIL.M_TYPE_LONG, ref MeasCount, MIL.M_NULL);

                    if(MeasCount > 0) MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_FIRST, ref dFirst);

                    MIL.MmeasFree(StripeMarker);

                    // Allocate a stripe marker.
                    MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

                    //찾을 ROI 설정, 원점 및 넓이/높이
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, ROI2.X, ROI1.Y + (i*Param1.PITCH));
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, ROI2.Width, Param1.PITCH);

                    //실행할 개수
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_SUB_REGIONS_NUMBER, 1, MIL.M_NULL);

                    //FILTER _ SMOOTHING
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_TYPE, MIL.M_SHEN, MIL.M_NULL);
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_SMOOTHNESS, 75D, MIL.M_NULL);

                    //찾을 줄무늬의 좌우 에지를 찾을 극성
                    MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, Param2.POLARITY_LEFT, Param2.POLARITY_RIGHT);

                    //측정
                    MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_DEFAULT);

                    //측정 개수 및 위치 가져오기
                    MeasCount = 0;
                    MIL.MmeasGetResult(StripeMarker, MIL.M_NUMBER + MIL.M_TYPE_LONG, ref MeasCount, MIL.M_NULL);

                    if (MeasCount > 0)
                    {
                        //MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_FIRST, ref dSecond);
                        MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_SECOND, ref dSecond);
                    }
                 
                    if(dFirst != 0 && dSecond != 0)
                    {
                        double dDist = (dSecond - dFirst) * DEFINE.PIXELPERMM;

                        if (dDist > 3)
                        {
                            Results.Add(new IVT_MEAS_RESULT(new Point((int)dFirst, ROI1.Y + (i * Param1.PITCH)), new Point((int)dSecond, ROI1.Y + (i * Param1.PITCH))));
                        }
                    }

                    MIL.MmeasFree(StripeMarker);
                }

            }
            catch (Exception)
            {
                return Results;
            }

            return Results;
        }


        //public static List<IVT_MEAS_RESULT> Run(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID image, Rectangle rtROI)
        //{
        //    List<IVT_MEAS_RESULT> Results = new List<IVT_MEAS_RESULT>();           

        //    try
        //    {
        //        if (rtROI.Width == 0 || rtROI.Height == 0) ROI = new Rectangle(0, 0, 4096, 4000);
        //        else ROI = rtROI;


        //        MilImage = image;
        //        // 이미지 버퍼를 표시합니다.
        //        MIL.MdispSelect(MilDisplay, MilImage);

        //        // Allocate a graphic list to hold the subpixel annotations to draw.
        //        MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

        //        // 오버레이 주석을 준비합니다.
        //        MIL.MdispControl(MilDisplay, MIL.M_OVERLAY, MIL.M_ENABLE);
        //        MIL.MdispInquire(MilDisplay, MIL.M_OVERLAY_ID, ref MilOverlayImage);

        //        MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, STRIPE_POLARITY_LEFT, STRIPE_POLARITY_RIGHT);

        //        // Set score function to find the expected width.
        //        MIL.MmeasSetScore(StripeMarker,
        //            MIL.M_STRIPE_WIDTH_SCORE,
        //            STRIPE_WIDTH - STRIPE_WIDTH_VARIATION,
        //            STRIPE_WIDTH - STRIPE_WIDTH_VARIATION,
        //            STRIPE_WIDTH + STRIPE_WIDTH_VARIATION,
        //            STRIPE_WIDTH + STRIPE_WIDTH_VARIATION,
        //            MIL.M_DEFAULT,
        //            MIL.M_DEFAULT,
        //            MIL.M_DEFAULT);

        //        MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ANGLE_MODE, MIL.M_ENABLE, MIL.M_NULL);

        //        // Specify the search region size and position.
        //        MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, ROI.X, ROI.Y);
        //        MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, ROI.Width, ROI.Height);

        //        // Draw the contour of the measurement region.
        //        MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
        //        MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_SEARCH_REGION, MIL.M_DEFAULT, MIL.M_MARKER);
        //        MIL.MgraRect(MIL.M_DEFAULT, MilOverlayImage, ROI.X, ROI.Y, ROI.X + ROI.Width, ROI.Y + ROI.Height);

        //        // Clear the annotations.
        //        //MIL.MgraClear(MIL.M_DEFAULT, MilGraphicList);

        //        // Find the stripe and measure its width and angle.
        //        MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_DEFAULT);

        //        // Get the stripe position, width and angle.
        //        MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION, ref StripeCenterX, ref StripeCenterY);
        //        MIL.MmeasGetResult(StripeMarker, MIL.M_STRIPE_WIDTH, ref StripeWidth);
        //        MIL.MmeasGetResult(StripeMarker, MIL.M_ANGLE, ref StripeAngle);

        //        // Draw the contour of the found measurement box.
        //        MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
        //        MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_RESULT);

        //        // Draw a cross on the center, left edge and right edge of the found stripe.
        //        MIL.MgraColor(MIL.M_DEFAULT, CrossColor);
        //        MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_POSITION, MIL.M_DEFAULT, MIL.M_RESULT);
        //        MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_POSITION + MIL.M_EDGE_FIRST + MIL.M_EDGE_SECOND, MIL.M_DEFAULT, MIL.M_RESULT);

        //        //Results.Add(new IVT_MEAS_RESULT(new Point((int)StripeCenterX, (int)StripeCenterY)));

        //        //Console.Write("The stripe in the image is at position {0:0.00},{1:0.00} and\n", StripeCenterX, StripeCenterY);
        //        //Console.Write("is {0:0.00} pixels wide with an angle of {1:0.00} degrees.\n", StripeWidth, StripeAngle);

        //        // Remove the graphic list association to the display.
        //        MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MIL.M_NULL);

        //        // Free all allocations.
        //        MIL.MgraFree(MilGraphicList);
        //        MIL.MmeasFree(StripeMarker);
        //        MIL.MbufFree(MilImage);

        //    }
        //    catch (Exception)
        //    {
        //        return Results;
        //    }

        //    return Results;
        //}

        public static List<IVT_MEAS_RESULT> Run(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID image, IVP_MMeas Param, IntPtr Handle)
        {
            List<IVT_MEAS_RESULT> Results = new List<IVT_MEAS_RESULT>();

            try
            {
                if (Param.ROI.Width == 0 || Param.ROI.Height == 0) ROI = new Rectangle(0, 0, 4096, 4000);
                else ROI = Param.ROI;

                MilImage = image;
                // 이미지 버퍼를 표시합니다.
                //MIL.MdispSelect(MilDisplay, MilImage);

                if (Param.ISDRAW) MIL.MdispSelectWindow(MilDisplay, MilImage, Handle);

                // Allocate a graphic list to hold the subpixel annotations to draw.
                if (Param.ISDRAW) MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

                // 오버레이 주석을 준비합니다.
                //MIL.MdispControl(MilDisplay, MIL.M_OVERLAY, MIL.M_ENABLE);
                //MIL.MdispInquire(MilDisplay, MIL.M_OVERLAY_ID, ref MilOverlayImage);

                if (Param.ISDRAW) MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
                if (Param.ISDRAW) MIL.MgraRect(MIL.M_DEFAULT, MilOverlayImage, ROI.X, ROI.Y, ROI.X + ROI.Width, ROI.Y + ROI.Height);

                int nRepeat = ROI.Height / Param.PITCH;

                double[] SubPosXArr1 = new double[nRepeat];
                double[] SubPosYArr1 = new double[nRepeat];
                double[] SubPosXArr2 = new double[nRepeat];
                double[] SubPosYArr2 = new double[nRepeat];       

                if (Param.ISDRAW) MIL.MgraColor(MIL.M_DEFAULT, CrossColor);

                // Allocate a stripe marker.
                MIL.MmeasAllocMarker(MilSystem, MIL.M_STRIPE, MIL.M_DEFAULT, ref StripeMarker);

                //찾을 줄무늬 개수 설정 ==> ALL
                MIL.MmeasSetMarker(StripeMarker, MIL.M_NUMBER, 1, MIL.M_NULL);

                //찾을 ROI 설정, 원점 및 넓이/높이
                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_ORIGIN, ROI.X, ROI.Y);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_BOX_SIZE, ROI.Width, ROI.Height);

                //실행할 개수
                MIL.MmeasSetMarker(StripeMarker, MIL.M_SUB_REGIONS_NUMBER, nRepeat, MIL.M_NULL);

                //FILTER _ SMOOTHING
                MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_TYPE, MIL.M_SHEN, MIL.M_NULL);
                MIL.MmeasSetMarker(StripeMarker, MIL.M_FILTER_SMOOTHNESS, 100.0, MIL.M_NULL);

                //찾을 줄무늬의 좌우 에지를 찾을 극성
                MIL.MmeasSetMarker(StripeMarker, MIL.M_POLARITY, Param.POLARITY_LEFT, Param.POLARITY_RIGHT);

                //측정
                MIL.MmeasFindMarker(MIL.M_DEFAULT, MilImage, StripeMarker, MIL.M_DEFAULT);

                //측정 개수 및 위치 가져오기
                MIL_INT MeasCount = 0;
                MIL.MmeasGetResult(StripeMarker, MIL.M_NUMBER + MIL.M_TYPE_LONG, ref MeasCount, MIL.M_NULL);

                double dPos = 0.0D;
                //MIL.MmeasGetResult(StripeMarker, MIL.M_SUB_EDGES_POSITION + MIL.M_EDGE_FIRST, ref dPos);

                MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_FIRST, ref dPos);

                MIL.MmeasGetResult(StripeMarker, MIL.M_SUB_EDGES_POSITION + MIL.M_EDGE_FIRST, SubPosXArr1, SubPosYArr1);
                MIL.MmeasGetResult(StripeMarker, MIL.M_SUB_EDGES_POSITION + MIL.M_EDGE_SECOND, SubPosXArr2, SubPosYArr2);

                for (MIL_INT i = 0; i < nRepeat; i++)
                {
                    //MgraLine(M_DEFAULT, MgraList, SubPosXArr1[i], SubPosYArr1[i], SubPosXArr2[i], SubPosYArr2[i]); // Draw Width Line

                    int nFirstEdgeX = (int)SubPosXArr1[i];
                    int nFirstEdgeY = (int)SubPosYArr1[i];

                    int nSecondEdgeX = (int)SubPosXArr2[i];
                    int nSecondEdgeY = (int)SubPosYArr2[i];

                    if (nFirstEdgeX != 0
                        && nFirstEdgeY != 0)
                    {
                        Results.Add(new IVT_MEAS_RESULT(new Point(nFirstEdgeX, nFirstEdgeY), new Point(nSecondEdgeX, nSecondEdgeY)));
                    }

                    if (Param.ISDRAW) MIL.MgraLine(MIL.M_DEFAULT, MilGraphicList, SubPosXArr1[i], SubPosYArr1[i], SubPosXArr2[i], SubPosYArr2[i]); // Draw Width Line
                }

                //MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION, ref StripeCenterX, ref StripeCenterY);
                //MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_FIRST, ref FirstEdgePosX, ref FirstEdgePosY);
                //MIL.MmeasGetResult(StripeMarker, MIL.M_POSITION + MIL.M_EDGE_SECOND, ref SecondEdgePosX, ref SecondEdgePosY);

                //MIL.MmeasGetResult(StripeMarker, MIL.M_STRIPE_WIDTH, ref StripeWidth);           

                //MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_SEARCH_REGION, MIL.M_DEFAULT, MIL.M_MARKER);
                //MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
                //MIL.MgraLine(MIL.M_DEFAULT, MilOverlayImage, FirstEdgePosX, FirstEdgePosY, FirstSecondPosX, FirstSecondPosY);

                //// Clear the annotations.
                if (Param.ISDRAW) MIL.MgraClear(MIL.M_DEFAULT, MilGraphicList);

                //// Draw the contour of the found measurement box.
                if (Param.ISDRAW) MIL.MgraColor(MIL.M_DEFAULT, BoxColor);
                //MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilGraphicList, MIL.M_DRAW_BOX, MIL.M_DEFAULT, MIL.M_RESULT);

                //// Draw a cross on the center, left edge and right edge of the found stripe.
                //MIL.MgraColor(MIL.M_DEFAULT, CrossColor);                    
                if (Param.ISDRAW) MIL.MmeasDraw(MIL.M_DEFAULT, StripeMarker, MilOverlayImage, MIL.M_DRAW_POSITION + MIL.M_EDGE_FIRST + MIL.M_EDGE_SECOND, MIL.M_DEFAULT, MIL.M_RESULT);

                //if (Param.ISDRAW) MIL.MgraText(MIL.M_DEFAULT, MilOverlayImage, FirstEdgePosX + 40, FirstEdgePosY + 20, StripeWidth.ToString("F2"));
                MIL.MmeasFree(StripeMarker);

                // Remove the graphic list association to the display.
                if (Param.ISDRAW) MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MIL.M_NULL);

                // Free all allocations.
                if (Param.ISDRAW) MIL.MgraFree(MilGraphicList);
            }
            catch (Exception)
            {
                return Results;
            }

            return Results;
        }
    }

    public class IVT_MEAS_RESULT
    {
        public int Distance = 0;
        public Point Pos_First = new Point();
        public Point Pos_Second = new Point();

        public IVT_MEAS_RESULT(Point ptFirst, Point ptSecond)
        {
            Pos_First = ptFirst;
            Pos_Second = ptSecond;

            Distance = Math.Abs(Pos_Second.X - Pos_First.X);
        }

        public IVT_MEAS_RESULT Clone()
        {
            return new IVT_MEAS_RESULT(Pos_First, Pos_Second);
        }
    }
}

#endif