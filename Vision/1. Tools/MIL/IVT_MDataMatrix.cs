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
    public static class IVT_MDataMatrix
    {
        public const int STRING_LENGTH_MAX = 64;

        public static Rectangle ROI = new Rectangle(0, 0, 4096, 4000);

        //public static MIL_ID MilApplication = MIL.M_NULL;                     // Application identifier.
        //public static MIL_ID MilSystem = MIL.M_NULL;                          // System identifier.
        public static MIL_ID MilDisplay = MIL.M_NULL;                         // Display identifier.
        public static MIL_ID MilImage = MIL.M_NULL;                           // Image buffer identifier.
        public static MIL_ID MilOverlayImage = MIL.M_NULL;                    // Image buffer identifier.
        public static MIL_ID ImageROI = MIL.M_NULL;                   // Child containing DataMatrix.
        public static MIL_ID DataMatrixCode = MIL.M_NULL;                     // DataMatrix 2D code identifier.
        public static MIL_ID BarCodeRegion = MIL.M_NULL;                      // Child containing Code39.
        public static MIL_ID Barcode = MIL.M_NULL;                            // Code39 barcode identifier.
        public static MIL_ID CodeResults = MIL.M_NULL;                        // Barcode results identifier.
        public static MIL_INT BarcodeStatus = MIL.M_NULL;                         // Decoding status.
        public static MIL_INT DataMatrixStatus = MIL.M_NULL;                      // Decoding status.

        public static double AnnotationColor = MIL.M_COLOR_GREEN;
        public static double AnnotationBackColor = MIL.M_COLOR_GRAY;
        public static int n = 0;
        public static StringBuilder DataMatrixString = new StringBuilder(STRING_LENGTH_MAX); // Array of characters read.
        
        public static List<IVT_CODE_RESULT> Run(MIL_ID MilSystem, MIL_ID MilDisplay, MIL_ID image, Rectangle rtROI, IntPtr Handle, bool bDraw = false)
        {
            List<IVT_CODE_RESULT> Results = new List<IVT_CODE_RESULT>();

            try
            {
                int nOffsetX = 1250;
                int START_TIME = Environment.TickCount;
                if (rtROI.Width == 0 || rtROI.Height == 0)
                {
                    ROI = new Rectangle(0, 0, 4096, 4000);
                }
                else
                {
                    ROI = rtROI;
                }

                MilImage = image;
                // 이미지 버퍼를 표시합니다.
                if(bDraw) MIL.MdispSelectWindow(MilDisplay, MilImage, Handle);

                // 오버레이 주석을 준비합니다.
                if(bDraw) MIL.MdispControl(MilDisplay, MIL.M_OVERLAY, MIL.M_ENABLE);
                if(bDraw) MIL.MdispInquire(MilDisplay, MIL.M_OVERLAY_ID, ref MilOverlayImage);

                // 바코드 결과 버퍼 준비
                MIL.McodeAllocResult(MilSystem, MIL.M_DEFAULT, ref CodeResults);
                //ROI
                MIL.MbufChild2d(MilImage, nOffsetX, 0, 1250, 4000, ref ImageROI);
                
                // CODE 개체를 할당합니다.
                MIL.McodeAlloc(MilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref DataMatrixCode);
                MIL.McodeModel(DataMatrixCode, MIL.M_ADD, MIL.M_DATAMATRIX, MIL.M_NULL, MIL.M_DEFAULT, MIL.M_NULL);

                // DataMatrix가 기본값과 다르므로 DataMatrix의 전경 값을 설정합니다.
                
                MIL.McodeControl(DataMatrixCode, MIL.M_INITIALIZATION_MODE, MIL.M_IMPROVED_RECOGNITION);
                MIL.McodeControl(DataMatrixCode, MIL.M_USE_PRESEARCH, MIL.M_FINDER_PATTERN_BASE);
                MIL.McodeControl(DataMatrixCode, MIL.M_THRESHOLD_MODE, MIL.M_ADAPTIVE);
                MIL.McodeControl(DataMatrixCode, MIL.M_FOREGROUND_VALUE, MIL.M_FOREGROUND_BLACK);
                MIL.McodeControl(DataMatrixCode, MIL.M_DECODE_ALGORITHM, MIL.M_CODE_DEFORMED);
                //MIL.McodeControl(DataMatrixCode, MIL.M_SEARCH_ANGLE_MODE, MIL.M_DISABLE);
                MIL.McodeControl(DataMatrixCode, MIL.M_SPEED, MIL.M_LOW);               

                // 이미지에서 코드를 읽습니다.
                MIL.McodeRead(DataMatrixCode, ImageROI, CodeResults);

                // 디코딩 상태를 가져옵니다. DataMatrixStatus값이 0이면 Read OK상태 / 6이면 Not Found
                MIL.McodeGetResult(CodeResults, MIL.M_GENERAL, MIL.M_GENERAL, MIL.M_STATUS + MIL.M_TYPE_MIL_INT, ref DataMatrixStatus);

                // 디코딩이 성공적이었는지 확인합니다.
                if (DataMatrixStatus == MIL.M_STATUS_READ_OK)
                {
                    // 디코딩된 문자열을 가져옵니다.
                    MIL.McodeGetResult(CodeResults, 0, MIL.M_GENERAL, MIL.M_STRING, DataMatrixString);

                    // 오버레이 영상에서 디코딩된 문자열을 그리고 영역을 읽습니다.
                    MIL.MgraColor(MIL.M_DEFAULT, AnnotationColor);
                    MIL.MgraBackColor(MIL.M_DEFAULT, AnnotationBackColor);

                    double dX = 0.0D, dY = 0.0D, dW = 0.0D, dH = 0.0D;

                    MIL.McodeGetResult(CodeResults, 0, MIL.M_GENERAL, MIL.M_TOP_LEFT_X, ref dX);
                    MIL.McodeGetResult(CodeResults, 0, MIL.M_GENERAL, MIL.M_TOP_LEFT_Y, ref dY);
                    MIL.McodeGetResult(CodeResults, 0, MIL.M_GENERAL, MIL.M_SIZE_X, ref dW);
                    MIL.McodeGetResult(CodeResults, 0, MIL.M_GENERAL, MIL.M_SIZE_Y, ref dH);

                    for (n = 0; n < DataMatrixString.Length; n++) // 인쇄할 수 없는 문자를 공백으로 바꿉니다.*/
                    {
                        if ((DataMatrixString[n] < '0') || (DataMatrixString[n] > 'Z'))
                        {
                            DataMatrixString[n] = ' ';
                        }
                    }

                    Results.Add(new IVT_CODE_RESULT(DataMatrixString.ToString(), new Rectangle((int)dX + nOffsetX, (int)dY, (int)dW, (int)dH)));

                    if (bDraw) MIL.MgraText(MIL.M_DEFAULT, MilOverlayImage, ROI.X, ROI.Y + 120, " 2D DATA MATRIX CODE : " + DataMatrixString.ToString());
                    if (bDraw) MIL.MgraRect(MIL.M_DEFAULT, MilOverlayImage, ROI.X, ROI.Y, ROI.X + ROI.Width, ROI.Y + ROI.Height);
                }

                // Free objects.
                MIL.McodeFree(DataMatrixCode);
                MIL.MbufFree(ImageROI);

                // Free results buffer.
                MIL.McodeFree(CodeResults);

                Logger.WriteLog(LOG.Inspection, "BCR DT TIME : {0}ms", Environment.TickCount - START_TIME);
            }
            catch (Exception)
            {
                return Results;
            }

            return Results;
        }
    }

    public class IVT_CODE_RESULT
    {
        public string DecodedString = "";
        public Rectangle BoundingRect = new Rectangle();
        public IVT_CODE_RESULT(string strDecoded, Rectangle rtBounding)
        {
            DecodedString = strDecoded;
            BoundingRect = rtBounding;
        }

        public IVT_CODE_RESULT Clone()
        {
            return new IVT_CODE_RESULT(DecodedString, BoundingRect);
        }
    }
}

#endif