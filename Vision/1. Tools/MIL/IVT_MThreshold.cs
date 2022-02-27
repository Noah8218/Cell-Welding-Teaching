#if MIL
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace IntelligentFactory
{
    public class IVT_MThreshold
    {
        public int IMAGE_THRESHOLD_VALUE = 26;

        public MIL_ID MilApplication = MIL.M_NULL;                 // Application identifier.
        public MIL_ID MilSystem = MIL.M_NULL;                      // System identifier.
        public MIL_ID MilDisplay = MIL.M_NULL;                     // Display identifier.
        public MIL_ID MilImage = MIL.M_NULL;                       // Image buffer identifier.
        public MIL_ID MilGraphicList = MIL.M_NULL;                 // Graphic list identifier.
        public MIL_ID MilBinImage = MIL.M_NULL;                    // Binary image buffer identifier.
        public MIL_ID MilBlobResult = MIL.M_NULL;                  // Blob result buffer identifier.
        public MIL_ID MilBlobContext = MIL.M_NULL;                // Blob Context identifier.
        public MIL_INT TotalBlobs = 0;                             // Total number of blobs.
        public MIL_INT BlobsWithHoles = 0;                         // Number of blobs with holes.
        public MIL_INT BlobsWithRoughHoles = 0;                    // Number of blobs with rough holes.
        public MIL_INT n = 0;                                      // Counter.
        public MIL_INT SizeX = 0;                                  // Size X of the source buffer
        public MIL_INT SizeY = 0;                                  // Size Y of the source buffer
        public double[] CogX = null;                               // X coordinate of center of gravity.
        public double[] CogY = null;                               // Y coordinate of center of gravity.

        public bool Init()
        {
            try
            {
                string strImagePath = @"C:\Users\IF\Desktop\1.jpg";
                // Allocate defaults.
                MIL.MappAllocDefault(MIL.M_DEFAULT, ref MilApplication, ref MilSystem, ref MilDisplay, MIL.M_NULL, MIL.M_NULL);

                // Restore source image into image buffer.
                MIL.MbufRestore(strImagePath, MilSystem, ref MilImage);

                // Allocate a graphic list to hold the subpixel annotations to draw.
                MIL.MgraAllocList(MilSystem, MIL.M_DEFAULT, ref MilGraphicList);

                // Associate the graphic list to the display.
                MIL.MdispControl(MilDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MilGraphicList);

                // Display the buffer.
                MIL.MdispSelect(MilDisplay, MilImage);

                // Allocate a binary image buffer for fast processing.
                MIL.MbufInquire(MilImage, MIL.M_SIZE_X, ref SizeX);
                MIL.MbufInquire(MilImage, MIL.M_SIZE_Y, ref SizeY);
                MIL.MbufAlloc2d(MilSystem, SizeX, SizeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref MilBinImage);
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }

        public bool Run()
        {
            try
            {
                // Binarize image.
                MIL.MimBinarize(MilImage, MilBinImage, MIL.M_FIXED + MIL.M_GREATER_OR_EQUAL, IMAGE_THRESHOLD_VALUE, MIL.M_NULL);
                Free();
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }


        public bool Free()
        {
            try
            {
                // Free all allocations.
                MIL.MgraFree(MilGraphicList);
                MIL.MblobFree(MilBlobResult);
                MIL.MblobFree(MilBlobContext);
                MIL.MbufFree(MilBinImage);
                MIL.MbufFree(MilImage);
                MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MIL.M_NULL, MIL.M_NULL);
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }
    }

}

#endif