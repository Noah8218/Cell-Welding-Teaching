#if EURESYS
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;

using Euresys.Open_eVision_2_14;

namespace IntelligentFactory
{
    public class IVT_ETemplateMatching
    {
        public List<IVT_ETemplateMatching_Result> Run(EImageBW8 imageSource, EImageBW8 imageTemplate, out long lTaktTime, out Bitmap ImageResult, double dMinScore = 0.75D)
        {
            List<IVT_ETemplateMatching_Result> listResult = new List<IVT_ETemplateMatching_Result>();

            lTaktTime = 0;
            ImageResult = new Bitmap(imageSource.Width, imageSource.Height);

            try
            {
                using (EMatcher Matching = new EMatcher())
                {
                    if (imageSource.IsVoid)
                    {
                        MessageBox.Show("Source is Empty");
                        return null;
                    }

                    if (imageTemplate.IsVoid)
                    {
                        MessageBox.Show("Template is Empty");
                        return null;
                    }


                    Stopwatch swTaktTimems = new Stopwatch();
                    swTaktTimems.Start();

                    Matching.LearnPattern(imageTemplate);

                    if (!Matching.PatternLearnt)
                    {
                        MessageBox.Show("Template Learn Error");
                        return null;
                    }

                    Matching.Match(imageSource);

                    swTaktTimems.Stop();
                    lTaktTime = swTaktTimems.ElapsedMilliseconds;

                    if (Matching.NumPositions > 0)
                    {
                        EMatchPosition Result = Matching.GetPosition(0);
                        listResult.Add(new IVT_ETemplateMatching_Result((int)Result.CenterX, (int)Result.CenterY, Result.Score));

                        using (Graphics g = Graphics.FromImage(ImageResult))
                        {
                            imageSource.Draw(g);

                            double dScore = Result.Score;

                            Font font = new Font("Microsoft Sans Serif", 7.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));

                            Rectangle rtResult = new Rectangle();

                            int nW = imageTemplate.Width;
                            int nH = imageTemplate.Height;

                            rtResult = new Rectangle((int)Result.CenterX - nW / 2, (int)Result.CenterY - nH / 2, nW, nH);

                            string strScore = $"Score : {dScore.ToString("F2")} %";
                            string strPos = $"X : {Result.CenterX} Y : {Result.CenterY}";
                            string strTaktTimems = $"Takt Time : {swTaktTimems.ElapsedMilliseconds} ms";

                            SolidBrush brush = new SolidBrush(Color.Red);

                            if (dScore >= dMinScore) brush = new SolidBrush(Color.Lime);

                            g.DrawString(strScore, font, brush, new PointF(rtResult.X + 10, rtResult.Y + 10));
                            g.DrawString(strPos, font, brush, new PointF(rtResult.X + 10, rtResult.Y + 25));
                            g.DrawRectangle(new Pen(brush, 2), rtResult);

                            g.DrawString(strTaktTimems, font, brush, new PointF(rtResult.Width - rtResult.Width / 2, rtResult.Height - (int)(rtResult.Width * 0.1)));

                            font.Dispose();
                            font = null;

                            brush.Dispose();
                            brush = null;
                        }

                    }
                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
                return null;
            }

            return listResult;
        }

        public List<IVT_ETemplateMatching_Result> Run(EImageBW8 imageSource, EImageBW8 imageTemplate, out long lTaktTime, double dMinScore = 0.75D)
        {
            List<IVT_ETemplateMatching_Result> listResult = new List<IVT_ETemplateMatching_Result>();

            lTaktTime = 0;

            try
            {
                using (EMatcher Matching = new EMatcher())
                {
                    if (imageSource.IsVoid)
                    {
                        MessageBox.Show("Source is Empty");
                        return null;
                    }

                    if (imageTemplate.IsVoid)
                    {
                        MessageBox.Show("Template is Empty");
                        return null;
                    }


                    Stopwatch swTaktTimems = new Stopwatch();
                    swTaktTimems.Start();

                    Matching.LearnPattern(imageTemplate);

                    if (!Matching.PatternLearnt)
                    {
                        MessageBox.Show("Template Learn Error");
                        return null;
                    }

                    Matching.Match(imageSource);

                    swTaktTimems.Stop();
                    lTaktTime = swTaktTimems.ElapsedMilliseconds;

                    if (Matching.NumPositions > 0)
                    {
                        EMatchPosition Result = Matching.GetPosition(0);
                        listResult.Add(new IVT_ETemplateMatching_Result((int)Result.CenterX, (int)Result.CenterY, Result.Score));
                    }
                }
            }
            catch (Exception Desc)
            {
                Debug.WriteLine($"Ex ==> {Desc.Message}");
                return null;
            }

            return listResult;
        }
    }

    public class IVT_ETemplateMatching_Result
    {
        public int CenterX { get; set; } = 0;
        public int CenterY { get; set; } = 0;
        public double Score { get; set; } = 0.0D;

        public IVT_ETemplateMatching_Result(int nCenterX, int nCenterY, double dScore)
        {
            CenterX = nCenterX;
            CenterY = nCenterY;
            Score = dScore;
        }
    }
}

#endif