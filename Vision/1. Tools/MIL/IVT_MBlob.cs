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
    public class IVT_MBlob
    {
        public int IMAGE_THRESHOLD_VALUE = 26;

        public int MIN_AREA = 50;
        public int MAX_AREA = 50000;

        public int MIN_RADIUS = 3;

        public double MIN_COMPACTNESS = 1.5;

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

        public bool SetSourceImage(Bitmap image)
        {
            try
            {
                //Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                //System.Drawing.Imaging.BitmapData bmpData =
                //    image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                //    image.PixelFormat);

                //unsafe
                //{

                //}
                //IntPtr* ptr = stackalloc IntPtr[1];
                //*ptr = input;
                //byte[] output = new byte[sizeof(IntPtr)];
                //Marshal.Copy((IntPtr)ptr, output, 0, sizeof(IntPtr));
                //return output;

                //byte[] managedArray = new byte[size];




                //Marshal.Copy(pnt, managedArray, 0, size);

                //MIL.MbufPut(MilImage, bmpData.Scan0);

                //// Declare an array to hold the bytes of the bitmap.
                //int bytes = Math.Abs(bmpData.Stride) * image.Height;
                //byte[] rgbValues = new byte[bytes];

                //// Copy the RGB values into the array.
                //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                //// Set every third value to 255. A 24bpp bitmap will look red.  
                //for (int counter = 2; counter < rgbValues.Length; counter += 3)
                //    rgbValues[counter] = 255;

                //// Copy the RGB values back to the bitmap
                //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                //// Unlock the bits.
                //image.UnlockBits(bmpData);

                //// Draw the modified image.
                //e.Graphics.DrawImage(image, 0, 150);

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

        public bool Thresholding()
        {
            try
            {
                // Binarize image.
                MIL.MimBinarize(MilImage, MilBinImage, MIL.M_FIXED + MIL.M_GREATER_OR_EQUAL, IMAGE_THRESHOLD_VALUE, MIL.M_NULL);
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
                // Remove small particles and then remove small holes.
                MIL.MimOpen(MilBinImage, MilBinImage, MIN_RADIUS, MIL.M_BINARY);
                MIL.MimClose(MilBinImage, MilBinImage, MIN_RADIUS, MIL.M_BINARY);

                // Allocate a context.
                MIL.MblobAlloc(MilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilBlobContext);

                // Enable the Center Of Gravity feature calculation.
                MIL.MblobControl(MilBlobContext, MIL.M_CENTER_OF_GRAVITY + MIL.M_BINARY, MIL.M_ENABLE);

                // Allocate a blob result buffer.
                MIL.MblobAllocResult(MilSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilBlobResult);

                // Calculate selected features for each blob.
                MIL.MblobCalculate(MilBlobContext, MilBinImage, MIL.M_NULL, MilBlobResult);

                // Exclude blobs whose area is too small.
                MIL.MblobSelect(MilBlobResult, MIL.M_EXCLUDE, MIL.M_AREA, MIL.M_LESS_OR_EQUAL, MIN_AREA, MIL.M_NULL);

                // Get the total number of selected blobs.
                MIL.MblobGetResult(MilBlobResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref TotalBlobs);
                Console.Write("There are {0} objects ", TotalBlobs);

                // Read and print the blob's center of gravity.
                CogX = new double[TotalBlobs];
                CogY = new double[TotalBlobs];
                if (CogX != null && CogY != null)
                {
                    // Get the results.
                    MIL.MblobGetResult(MilBlobResult, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_X + MIL.M_BINARY, CogX);
                    MIL.MblobGetResult(MilBlobResult, MIL.M_DEFAULT, MIL.M_CENTER_OF_GRAVITY_Y + MIL.M_BINARY, CogY);

                    // Print the center of gravity of each blob.
                    Console.Write("and their centers of gravity are:\n");
                    for (n = 0; n < TotalBlobs; n++)
                    {
                        Console.Write("Blob #{0}: X={1,5:0.0}, Y={2,5:0.0}\n", n, CogX[n], CogY[n]);
                    }

                }
                else
                {
                    Console.Write("\nError: Not enough memory.\n");
                }

                // Draw a cross at the center of gravity of each blob.
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_RED);
                MIL.MblobDraw(MIL.M_DEFAULT, MilBlobResult, MilGraphicList, MIL.M_DRAW_CENTER_OF_GRAVITY, MIL.M_INCLUDED_BLOBS, MIL.M_DEFAULT);

                // Reverse what is considered to be the background so that
                // holes are seen as being blobs.
                MIL.MblobControl(MilBlobContext, MIL.M_FOREGROUND_VALUE, MIL.M_ZERO);

                // Add a feature to distinguish between types of holes.Since area
                // has already been added to the context, and the processing 
                // mode has been changed, all blobs will be re-included and the area 
                // of holes will be calculated automatically.
                MIL.MblobControl(MilBlobContext, MIL.M_COMPACTNESS, MIL.M_ENABLE);

                // Calculate selected features for each blob.
                MIL.MblobCalculate(MilBlobContext, MilBinImage, MIL.M_NULL, MilBlobResult);

                // Exclude small holes and large (the area around objects) holes.
                MIL.MblobSelect(MilBlobResult, MIL.M_EXCLUDE, MIL.M_AREA, MIL.M_OUT_RANGE, MIN_AREA, MAX_AREA);

                // Get the number of blobs with holes.
                MIL.MblobGetResult(MilBlobResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref BlobsWithHoles);

                // Exclude blobs whose holes are compact (i.e.nuts).
                MIL.MblobSelect(MilBlobResult, MIL.M_EXCLUDE, MIL.M_COMPACTNESS, MIL.M_LESS_OR_EQUAL, MIN_COMPACTNESS, MIL.M_NULL);

                // Get the number of blobs with holes that are NOT compact.
                MIL.MblobGetResult(MilBlobResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref BlobsWithRoughHoles);

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

    public class IVT_MBlob_Result
    {
        public Point Center = new Point();
        public Point Gravity = new Point();

        public Rectangle BoundingRect = new Rectangle();

        public int Area = 0;
        public double Angle = 0.0D;

        public IVT_MBlob_Result()
        {
        }
    }
}

#endif