using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using OpenCvSharp;
using OpenCvSharp.UserInterface;

using ImageGlass;

using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Controls;

#if MATROX
using Matrox.MatroxImagingLibrary;
#endif 
using System.Drawing.Imaging;

namespace IntelligentFactory
{
    public partial class FormMetroFrame : MetroForm
    {
        private IGlobal Global = IGlobal.Instance;

        #region MENU
        private FormRecipe m_FrmRecipe = null;
        private FormSettings_Camera m_FrmSettings_Camera = null;
        private FormAlarm FrmAlarm = new FormAlarm();
        #endregion

        #region EVENT HANDLER
        public EventHandler<StringEventArgs> EventInit;
        public EventHandler<EventArgs> EventInitEnd;
        public EventHandler<InspResultArgs> EventInspResult;
        #endregion

        private IntelligentImageView ImageViewTop = new IntelligentImageView();
        private IntelligentImageView ImageViewBtm = new IntelligentImageView();

        private float fFontSizeTop { get; set; } = 5;
        private float fFontSizeBottom { get; set; } = 5;
        private float fFontSizeFactor { get; set; } = 7.5f;
        private float fThicknessTop { get; set; } = 5;
        private float fThicknessBottom { get; set; } = 5;

        private Bitmap ImageSource = new Bitmap(10, 10);
        //private Mat ImageSource = new Mat();

        private bool LogFullScreen = false;
        private System.Drawing.Size LogSize = new System.Drawing.Size();

        private System.Drawing.Point ptClickedTop = new System.Drawing.Point();
        private System.Drawing.Point ptClickedBtm = new System.Drawing.Point();
        private int nSelecteIndexTop = 0;

        public Rectangle SelectRtTop = new Rectangle(new System.Drawing.Point(0, 0), new System.Drawing.Size(0, 0));

        private System.Drawing.Point _startPtTop = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _endPtTop = new System.Drawing.Point(0, 0);

        private int _MinY = 1;
        private int _MaxY = 3200;
        private int _MinX = 1;
        private int _MaxX = 3200;

        public Rectangle SelectRtBtm = new Rectangle(new System.Drawing.Point(0, 0), new System.Drawing.Size(0, 0));

        private System.Drawing.Point _startPtBtm = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _endPtBtm = new System.Drawing.Point(0, 0);

        public FormMetroFrame()
        {
            InitializeComponent();

            this.StyleManager = metroStyleManager;
            this.Location = new System.Drawing.Point(0, 0);

            string strLogPath = CUtil.InitLogDirectory();
            Logger.SetPath(strLogPath);

            LogView logview = new LogView();
            logview.Dock = DockStyle.Fill;
            pnLog.Controls.Add(logview);

            metroStyleManager.Style = MetroFramework.MetroColorStyle.Teal;

            this.KeyPreview = false;

            LogSize = pnLog.Size;
        }

        private void FormMetroFrame_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.WriteLog(LOG.Normal, "Start S/W");

                Global.iSystem.IF_Handle = this.Handle;
                IGlobal.Instance.iSystem.Recipe.LoadConfig();

                InitEvent();
                InitConfig();
                InitIO();
                InitMenu();
                InitProperty();
                InitUI();
                InitThread();
                InitVersion();

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void CreatePositionTop()
        {
            try
            {
                gridPositionListTop.Rows.Clear();

                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Top.Count; i++)
                {
                    CFormatCellPosition pos = Global.iData.Property_CellPosition.CellPositions_Top[i];

                    string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                    string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                    string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                    string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";

                    gridPositionListTop.Rows.Add(new string[] { (pos.Index).ToString(), "#1", strDisplay, "READY" });
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void CreatePositionBottom()
        {
            try
            {
                gridPositionListBottom.Rows.Clear();

                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Bottom.Count; i++)
                {
                    CFormatCellPosition pos = Global.iData.Property_CellPosition.CellPositions_Bottom[i];

                    string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                    string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                    string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                    string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";

                    gridPositionListBottom.Rows.Add(new string[] { (pos.Index).ToString(), "#1", strDisplay, "READY" });
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void FormMetroFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Global.iSystem.Mode == ISystem.MODE.AUTO)
                {
                    MessageBox.Show("Please Change the Ready of Mode");
                    e.Cancel = true;
                }
                else
                {
                    //StopThreadDeleteImage();
                    Global.Close();

                    #region PLC // 추가 예정
                    if (m_FrmRecipe != null) m_FrmRecipe.Close();
                    #endregion
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Global.iDevice.ComMelsec.DI_TOP_MODEL_UPDATE.Current == 1) { lbUpdate(lbUpdateTop, true); }
                else { lbUpdate(lbUpdateTop, false); }

                if (Global.iDevice.ComMelsec.DI_BOTTOM_MODEL_UPDATE.Current == 1) { lbUpdate(lbUpdateBottom, true); }
                else { lbUpdate(lbUpdateBottom, false); }               
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        
        private void lbUpdate(MetroTile metroTile, bool bUpdate)
        {
            if (bUpdate)
            {
                metroTile.BackColor = Color.Lime;
                metroTile.Style = MetroColorStyle.Lime;
                metroTile.Invalidate();
            }
            else
            {
                metroTile.BackColor = Color.Silver;
                metroTile.Style = MetroColorStyle.Silver;
                metroTile.Invalidate();
            }
        }
   
        private void ibTop_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (Global.iSystem.Mode == ISystem.MODE.AUTO) return;
                ptClickedTop = ibTop.PointToImage(e.Location);

                // 2022.02.25 Noah 마우스 우클릭 할 때 셀렉트값을 초기화 시키자.

                // 우선 초기화를 무조건 1번을 진행
                if (e.Button == MouseButtons.Right && !ImageViewTop.bUseShift) 
                {
                    ClearCell(Global.iData.Property_CellPosition.CellPositions_Top, true);
                }
              
                UpdateDispTop();                
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void ClearCell(List<CFormatCellPosition> CellPositions, bool bIsTop)
        {
            for (int i = 0; i < CellPositions.Count; i++) { CellPositions[i].Selected = false; }

            if (bIsTop) { SelectRtTop = new Rectangle(); }
            else { SelectRtBtm = new Rectangle(); }
        }


        private void ibBottom_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (Global.iSystem.Mode == ISystem.MODE.AUTO) return;

                ptClickedBtm = ibBottom.PointToImage(e.Location);

                // 2022.02.25 Noah 마우스 우클릭 할 때 셀렉트값을 초기화 시키자.

                // 우선 초기화를 무조건 1번을 진행
                if (e.Button == MouseButtons.Right && !ImageViewBtm.bUseShift)
                {
                    ClearCell(Global.iData.Property_CellPosition.CellPositions_Bottom, false);
                }

                UpdateDispBottom();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private Rectangle ImageActualSize(ImageBoxEx ibSource, Rectangle SelectRt)
        {
            System.Drawing.Point point = ibSource.PointToImage(new System.Drawing.Point(SelectRt.X, SelectRt.Y));

            System.Drawing.Size size = ibSource.GetScaledSize(SelectRt.Width, SelectRt.Height);

            Rectangle DisplRect = new Rectangle(point.X, point.Y, (int)(SelectRt.Width / ibSource.ZoomFactor), (int)(SelectRt.Height / ibSource.ZoomFactor));
            return DisplRect;
        }

        private void CellClickChangeColor(List<CFormatCellPosition> CellPositions, System.Drawing.Point ptClicked, MetroGrid gridPositionList)
        {
            for (int i = 0; i < CellPositions.Count; i++)
            {
                if (CellPositions[i].Boundary.Contains(ptClicked.X, ptClicked.Y))
                {
                    CellPositions[i].Selected = true;

                    if (CellPositions[i].Selected)
                    {
                        propertygrid_Rect.SelectedObject = CellPositions[i];
                    }

                    if (gridPositionList.Rows.Count > i) // 20220221_SYH
                    {
                        gridPositionList.Rows[i].Selected = true;
                        gridPositionList.CurrentCell = gridPositionList.Rows[i].Cells[1]; // 20220221_SYH
                    }
                    //break;
                }
            }
        }

        private void CellShiftChangeColor(List<CFormatCellPosition> CellPositions, ImageBoxEx ibBox, Rectangle SelectRt, bool bUseShift)
        {
            if (bUseShift)
            {
                // 여기서는 Shift에 한해서 Selected값을 변경
                for (int i = 0; i < CellPositions.Count; i++)
                {
                    Rectangle rt = new Rectangle(CellPositions[i].Boundary.X, CellPositions[i].Boundary.Y,
                                              CellPositions[i].Boundary.Width, CellPositions[i].Boundary.Height);

                    if (ImageActualSize(ibBox, SelectRt).IntersectsWith(rt))
                    {
                        CellPositions[i].Selected = true;

                        propertygrid_Rect.SelectedObject = CellPositions[i];
                    }
                }
            }
        }

    private void UpdateDispTop()
        {
            try
            {
                if (ImageSource == null) { return; }
                
                this.Invoke(new Action(delegate ()
                {
                    // 여기서는 클릭한 영역에 대해서 Selected값을 변경                 
                    CellClickChangeColor(Global.iData.Property_CellPosition.CellPositions_Top, ptClickedTop, gridPositionListTop);
                    CellShiftChangeColor(Global.iData.Property_CellPosition.CellPositions_Top, ibTop, SelectRtTop, ImageViewTop.bUseShift);
                    DrawPos(Global.iData.Property_CellPosition.CellPositions_Top, true);

                    GC.Collect();
                }));              
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void UpdateDispBottom()
        {
            try
            {
                if (ImageSource == null) { return; }
                
                this.Invoke(new Action(delegate ()
                {
                    CellClickChangeColor(Global.iData.Property_CellPosition.CellPositions_Bottom, ptClickedBtm, gridPositionListBottom);                   
                    CellShiftChangeColor(Global.iData.Property_CellPosition.CellPositions_Bottom, ibBottom, SelectRtBtm, ImageViewBtm.bUseShift);
                    DrawPos(Global.iData.Property_CellPosition.CellPositions_Bottom, false);

                    GC.Collect();
                }));
               
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void DrawPos(List<CFormatCellPosition> CellPositions, bool bIsTop)
        {
            if (bIsTop) { Global.iData.Property_CellPosition.COUNT_TOP = CellPositions.Count; }
            else { Global.iData.Property_CellPosition.COUNT_BOTTOM = CellPositions.Count; }

            Bitmap imageDisplay = (Bitmap)ImageSource.Clone();

            using (Graphics g = Graphics.FromImage(imageDisplay))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                g.DrawRectangle(new Pen(Color.White, 20), new Rectangle(0, 0, 15900, 7000));
                System.Drawing.PointF ptSelected = new System.Drawing.PointF();
                string strDisplay = "";

                for (int i = 0; i < CellPositions.Count; i++)
                {
                    string strPosX = CellPositions[i].ACTUAL_POS_X.ToString("F2") + "mm";
                    string strPosY = CellPositions[i].ACTUAL_POS_Y.ToString("F2") + "mm";
                    string strPosZ = CellPositions[i].ACTUAL_POS_Z.ToString("F2") + "mm";
                    string strCh = CellPositions[i].CHANNEL.ToString();

                    if (!CellPositions[i].Selected)
                    {
                        if (CellPositions[i].Part == "+")
                        {
                            g.DrawEllipse(new Pen(Color.Blue, fThicknessTop), CConverter.CVRectToRect(CellPositions[i].Boundary));
                        }
                        else if (CellPositions[i].Part == "-")
                        {
                            g.DrawEllipse(new Pen(Color.Magenta, fThicknessTop), CConverter.CVRectToRect(CellPositions[i].Boundary));
                        }
                        else
                        {
                            g.DrawEllipse(new Pen(Color.White, fThicknessTop), CConverter.CVRectToRect(CellPositions[i].Boundary));
                        }
                    }
                    else
                    {
                        if (Global.iSystem.Mode != ISystem.MODE.AUTO)
                        {
                            g.DrawEllipse(new Pen(Color.Red, fThicknessTop), CConverter.CVRectToRect(CellPositions[i].Boundary));

                            ptSelected = new System.Drawing.Point(CellPositions[i].Boundary.Location.X, CellPositions[i].Boundary.Location.Y);
                            strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ} CH : {strCh}";

                            lbPosNo_Top.Text = CellPositions[i].Index.ToString();
                            lbPosX_Top.Text = strPosX;
                            lbPosY_Top.Text = strPosY;
                            lbPosZ_Top.Text = strPosZ;
                            lbPosCh_Top.Text = strCh;
                        }
                    }

                    if (CellPositions[i].RESULT == 1)
                    {
                        g.FillEllipse(new SolidBrush(Color.Lime), CConverter.CVRectToRect(CellPositions[i].Boundary));
                    }
                    else if (CellPositions[i].RESULT == 2)
                    {
                        g.FillEllipse(new SolidBrush(Color.Red), CConverter.CVRectToRect(CellPositions[i].Boundary));
                    }
                }


                for (int i = 0; i < CellPositions.Count; i++)
                {
                    System.Drawing.Point ptCenter = new System.Drawing.Point();
                    int n = (int)fFontSizeTop;
                    ptCenter.X = CellPositions[i].Boundary.X + (CellPositions[i].Boundary.Width / 2) - n;
                    ptCenter.Y = CellPositions[i].Boundary.Y + (CellPositions[i].Boundary.Height / 2) - n;

                    if (Global.iData.Property_CellPosition.IS_DISPLAY_POLARITY)
                    {
                        g.DrawString($"{CellPositions[i].Index}\n({CellPositions[i].Part})", new Font("Arial", (float)Global.iData.Property_CellPosition.FONT_SIZE, FontStyle.Regular), new SolidBrush(Color.Yellow), ptCenter);
                    }
                    else
                    {
                        g.DrawString($"{CellPositions[i].Index}", new Font("Arial", (float)Global.iData.Property_CellPosition.FONT_SIZE, FontStyle.Regular), new SolidBrush(Color.Yellow), ptCenter);
                    }
                }
            }

            if (bIsTop) { ibTop.Image = imageDisplay; }
            else { ibBottom.Image = imageDisplay; }

        }

        private void UpdateDisplay()
        {
            try
            {
                UpdateDispTop();
                UpdateDispBottom();
                ibTop_MouseDoubleClick(ibTop, null);
                ibBottom_MouseDoubleClick(ibBottom, null);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnCreateCellArray_Top_Click(object sender, EventArgs e)
        {
            try
            {
                int nRows = Global.iData.Property_CellPosition.ROWS_TOP;
                int nCols = Global.iData.Property_CellPosition.COLUMNS_TOP;

                float fGapX = Global.iData.Property_CellPosition.GAP_X_MM_TOP;
                float fGapY = Global.iData.Property_CellPosition.GAP_Y_MM_TOP;

                //int nBoundary = (int)((float)Global.iData.Property_CellPosition.GAP_X_MM_TOP / 0.08F);
                int nBoundary = Global.iData.Property_CellPosition.CLICK_BOUNDARY * 10;

                Global.iData.Property_CellPosition.CellPositions_Top.Clear();
                gridPositionListTop.Rows.Clear();

                Global.iData.Property_CellPosition.COUNT_TOP = nRows * nCols;

                if(Global.iData.Property_CellPosition.GAP_X_MM_TOP == 1)
                {
                    fFontSizeFactor = 7.5f;
                }
                else
                {
                    fFontSizeFactor = 3.5f;
                }

                fFontSizeTop = Global.iData.Property_CellPosition.GAP_X_MM_TOP * fFontSizeFactor;
                fThicknessTop = Global.iData.Property_CellPosition.GAP_X_MM_TOP > 2 ? 5 : 1;

                if (Global.iData.Property_CellPosition.ARRAYDIRCETION_TOP == CPropertyCellPosition.ARRAY_DIRECTION.ZIGZAG_Y)
                {
                    for (int nColIndex = 0; nColIndex < nCols; nColIndex++)
                    {
                        if (nColIndex % 2 == 0)
                        {
                            for (int nRowIndex = 0; nRowIndex < nRows; nRowIndex++)
                            {
                                int nIndex = (nColIndex * nRows) + nRowIndex + 1;
                                nIndex = Global.iData.Property_CellPosition.CellPositions_Top.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 -(int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Top(pos);

                                gridPositionListTop.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                        else
                        {
                            for (int nRowIndex = nRows - 1; nRowIndex >= 0; nRowIndex--)
                            {
                                int nIndex = Global.iData.Property_CellPosition.CellPositions_Top.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Top(pos);

                                gridPositionListTop.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                    }
                }
                else if (Global.iData.Property_CellPosition.ARRAYDIRCETION_TOP == CPropertyCellPosition.ARRAY_DIRECTION.ZIGZAG_X)
                {
                    for (int nRowIndex = 0; nRowIndex < nRows; nRowIndex++)
                    {
                        if (nRowIndex % 2 == 0)
                        {
                            for (int nColIndex = 0; nColIndex < nCols; nColIndex++)
                            {
                                int nIndex = (nColIndex * nRows) + nRowIndex + 1;
                                nIndex = Global.iData.Property_CellPosition.CellPositions_Top.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Top(pos);

                                gridPositionListTop.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                        else
                        {
                            for (int nColIndex = nCols - 1; nColIndex >= 0; nColIndex--)
                            {
                                int nIndex = Global.iData.Property_CellPosition.CellPositions_Top.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+"; //20220217_SYH

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Top(pos);

                                gridPositionListTop.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                    }
                }
                UpdateDispTop();
                ibTop_MouseDoubleClick(ibTop, null);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnCreateCellArray_Btm_Click(object sender, EventArgs e)
        {
            try
            {
                int nRows = Global.iData.Property_CellPosition.ROWS_BOTTOM;
                int nCols = Global.iData.Property_CellPosition.COLUMNS_BOTTOM;

                float fGapX = Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM;
                float fGapY = Global.iData.Property_CellPosition.GAP_Y_MM_BOTTOM;

                //int nBoundary = (int)((float)Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM / 0.08F);
                int nBoundary = Global.iData.Property_CellPosition.CLICK_BOUNDARY * 10;
                if (Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM == 1)
                {
                    fFontSizeFactor = 7.5f;
                }
                else
                {
                    fFontSizeFactor = 3.5f;
                }

                fFontSizeBottom = Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM * fFontSizeFactor;
                fThicknessBottom = Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM > 2 ? 5 : 1;

                Global.iData.Property_CellPosition.CellPositions_Bottom.Clear();
                gridPositionListBottom.Rows.Clear();

                if (Global.iData.Property_CellPosition.ARRAYDIRCETION_BOTTOM == CPropertyCellPosition.ARRAY_DIRECTION.ZIGZAG_Y)
                {
                    for (int nColIndex = 0; nColIndex < nCols; nColIndex++)
                    {
                        if (nColIndex % 2 == 0)
                        {
                            for (int nRowIndex = 0; nRowIndex < nRows; nRowIndex++)
                            {
                                int nIndex = (nColIndex * nRows) + nRowIndex + 1;
                                nIndex = Global.iData.Property_CellPosition.CellPositions_Bottom.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+"; //20220217_SYH

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);

                                gridPositionListBottom.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                        else
                        {
                            for (int nRowIndex = nRows - 1; nRowIndex >= 0; nRowIndex--)
                            {
                                int nIndex = Global.iData.Property_CellPosition.CellPositions_Bottom.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);

                                gridPositionListBottom.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                    }
                }
                else if (Global.iData.Property_CellPosition.ARRAYDIRCETION_BOTTOM == CPropertyCellPosition.ARRAY_DIRECTION.ZIGZAG_X)
                {
                    for (int nRowIndex = 0; nRowIndex < nRows; nRowIndex++)
                    {
                        if (nRowIndex % 2 == 0)
                        {
                            for (int nColIndex = 0; nColIndex < nCols; nColIndex++)
                            {
                                int nIndex = (nColIndex * nRows) + nRowIndex + 1;
                                nIndex = Global.iData.Property_CellPosition.CellPositions_Bottom.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);

                                gridPositionListBottom.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                        else
                        {
                            for (int nColIndex = nCols - 1; nColIndex >= 0; nColIndex--)
                            {
                                int nIndex = Global.iData.Property_CellPosition.CellPositions_Bottom.Count + 1;

                                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                                ptCenter.X = ptCenter.X + (int)(fGapX * nColIndex) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;
                                ptCenter.Y = 7000 - (int)(fGapY * nRowIndex + ptCenter.Y) * DEFINE.IMAGE_SIZE_FACTOR;// - nBoundary / 2;

                                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                                //(int nIndex, float fX, float fY, float fZ, float fOffsetX, float fOffsetY, int nChannel, Rect rtBoundary, string strPart = "TOP")
                                CFormatCellPosition pos = new CFormatCellPosition();
                                pos.Index = nIndex;
                                pos.ACTUAL_POS_X = fGapX * nColIndex;
                                pos.ACTUAL_POS_Y = fGapY * nRowIndex;
                                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                                pos.Boundary = rtSubMat;
                                pos.Part = "+";

                                string strPosX = pos.ACTUAL_POS_X.ToString("F2") + "mm";
                                string strPosY = pos.ACTUAL_POS_Y.ToString("F2") + "mm";
                                string strPosZ = pos.ACTUAL_POS_Z.ToString("F2") + "mm";

                                string strDisplay = $"X : {strPosX} Y : {strPosY} Z : {strPosZ}";
                                Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);

                                gridPositionListBottom.Rows.Add(new string[] { (nIndex).ToString(), "#1", strDisplay, "READY" });
                            }
                        }
                    }
                }
                UpdateDispBottom();
                ibBottom_MouseDoubleClick(ibBottom, null);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnTopTotalCancel_Click(object sender, EventArgs e)
        {
            pnTopTotal.Visible = false;
        }

        private void btnTopTotalApply_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Top.Count; i++)
                {
                    CFormatCellPosition pos = Global.iData.Property_CellPosition.CellPositions_Top[i];

                    if (pos.Selected)
                    {
                        if (cbTopTotalOffsetX.Checked) pos.ACTUAL_POS_X += float.Parse(tbTopTotalOffsetX.Text);
                        if (cbTopTotalOffsetY.Checked) pos.ACTUAL_POS_Y += float.Parse(tbTopTotalOffsetY.Text);
                        if (cbTopTotalOffsetZ.Checked) pos.ACTUAL_POS_Z += float.Parse(tbTopTotalOffsetZ.Text);
                        if (cbTopTotalChannel.Checked) pos.CHANNEL = int.Parse(tbTopTotalChannel.Text);
                        if (cbTopTotalPart.Checked) pos.Part = string.Format(tbTopTotalPart.Text); //20220217_SYH
                        if (cbTopTotalCellSize.Checked) pos.Cell_Size = int.Parse(tbTopTotalCellSize.Text); //20220221_SYH

                        Rect rect = new Rect(Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.Y, pos.Cell_Size, pos.Cell_Size); // 202202221
                        if (cbTopTotalCellSize.Checked)
                            Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary = rect; // 202202221_SYH

                    }
                }

                UpdateDispTop();
            }
            catch (Exception Desc)
            {
                CUtil.ShowMessageBox("ALARM", "Please check the Value.");
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            pnTopTotal.Visible = false;
        }

        private void btnBtmTotalApply_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Bottom.Count; i++)
                {
                    CFormatCellPosition pos = Global.iData.Property_CellPosition.CellPositions_Bottom[i];

                    if (pos.Selected)
                    {
                        if (cbBtmTotalOffsetX.Checked) pos.ACTUAL_POS_X += float.Parse(tbBtmTotalOffsetX.Text);
                        if (cbBtmTotalOffsetY.Checked) pos.ACTUAL_POS_Y += float.Parse(tbBtmTotalOffsetY.Text);
                        if (cbBtmTotalOffsetZ.Checked) pos.ACTUAL_POS_Z += float.Parse(tbBtmTotalOffsetZ.Text);
                        if (cbBtmTotalChannel.Checked) pos.CHANNEL = int.Parse(tbBtmTotalChannel.Text);
                        if (cbBtmTotalPart.Checked) pos.Part = string.Format(tbBtmTotalPart.Text); //20220217_SYH
                        if (cbBtmTotalCellSize.Checked) pos.Cell_Size = int.Parse(tbBtmTotalCellSize.Text); //20220221_SYH

                        Rect rect = new Rect(Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary.Y, pos.Cell_Size, pos.Cell_Size); // 202202221
                        if (cbBtmTotalCellSize.Checked)
                            Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary = rect; // 202202221_SYH
                    }
                }

                UpdateDispBottom();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            pnBtmTotal.Visible = false;
        }

        private void btnBtmTotalCancel_Click(object sender, EventArgs e)
        {
            pnBtmTotal.Visible = false;
        }

        private void btnLoadTop_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Application.StartupPath;
            ofd.Filter = "Recipe Files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Global.iData.Property_CellPosition.LoadConfig(ofd.FileName, false);
                UpdateParameter();
            }
        }

        private void btnSaveTop_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath;
            sfd.DefaultExt = "xml";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Global.iData.Property_CellPosition.SaveConfig(sfd.FileName, false);
            }
        }

        private void ibTop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (Global.iSystem.Mode == ISystem.MODE.AUTO) return;

                this.Invoke(new Action(delegate ()
                {
                    int nFactor = (int)(170 * Global.iData.Property_CellPosition.GAP_X_MM_TOP);

                    if (Global.iData.Property_CellPosition.CellPositions_Top.Count <= Global.iData.Property_CellPosition.ROWS_TOP) return;
                    ibTop.ZoomToRegion(Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.Y,
                        nFactor, nFactor);

                    double dFactor = ibTop.ZoomFactor;

                    int nW = (int)(Global.iData.Property_CellPosition.GAP_X_MM_TOP * dFactor + 30);
                    //int nH = (int)(Global.iData.Property_CellPosition.GAP_Y_MM_TOP * dFactor + 30);

                    ibTop.ScrollTo(Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.X - nW, Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.Y,
                            Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.Width, Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.ROWS_TOP].Boundary.Height);
                }));
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void ibBottom_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                this.Invoke(new Action(delegate ()
                {
                    int nFactor = (int)(170 * Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM);

                    if (Global.iData.Property_CellPosition.CellPositions_Bottom.Count <= Global.iData.Property_CellPosition.ROWS_BOTTOM) return;
                    ibBottom.ZoomToRegion(Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.Y,
                        nFactor, nFactor);

                    double dFactor = ibBottom.ZoomFactor;

                    int nW = (int)(Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM * dFactor + 30);
                    //int nH = (int)(Global.iData.Property_CellPosition.GAP_Y_MM_TOP * dFactor + 30);

                    ibBottom.ScrollTo(Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.X - nW, Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.Y,
                            Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.Width, Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.ROWS_BOTTOM].Boundary.Height);
                }));              
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void ibTop_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (ImageViewTop.bUseShift)
                    {
                        if (!ImageViewTop.bUseControlMoveCell)
                        {
                            int intY = e.Y;
                            int intX = e.X;

                            if (e.Y > _MaxY) intY = _MaxY;
                            if (e.Y < _MinY) intY = _MinY;
                            if (e.X > _MaxX) intX = _MaxX;
                            if (e.X < _MinX) intX = _MinX;

                            _endPtTop = new System.Drawing.Point(intX, intY);

                            int curWidth = _endPtTop.X - _startPtTop.X;
                            int curHeight = _endPtTop.Y - _endPtTop.Y;

                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(_startPtTop.X, _startPtTop.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(_endPtTop.X, _endPtTop.Y);

                            Graphics g = ibTop.CreateGraphics();

                            try
                            {
                                if (ptStart.X > ptEnd.X)
                                {
                                    if (ptStart.Y < ptEnd.Y)
                                    {
                                        SelectRtTop = new Rectangle(ptEnd.X, ptStart.Y, ptStart.X - ptEnd.X, ptEnd.Y - ptStart.Y);

                                        //g.DrawRectangle(new Pen(Color.Orange, 5), rt);                                                                        
                                        g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                    }
                                    else
                                    {
                                        SelectRtTop = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y);
                                        //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                        g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                    }

                                }
                                else
                                {
                                    if (ptStart.Y < ptEnd.Y)
                                    {
                                        if (ptStart.X < ptEnd.X)
                                        {
                                            SelectRtTop = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);
                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                        }
                                        else
                                        {
                                            SelectRtTop = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);

                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                        }

                                    }
                                    else
                                    {
                                        if (ptStart.X < ptEnd.X)
                                        {
                                            SelectRtTop = new Rectangle(ptStart.X, ptEnd.Y, ptEnd.X - ptStart.X, ptStart.Y - ptEnd.Y);

                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                        }
                                        else
                                        {
                                            SelectRtTop = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y);
                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtTop);
                                        }

                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    ibTop.Invalidate();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        private void ibTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (Global.iSystem.Mode == ISystem.MODE.AUTO) return;

            if (e.Button == MouseButtons.Right)
            {
                int intY = e.Y;
                int intX = e.X;

                // 마우스 클릭 최대, 최소값 설정  
                if (e.Y < _MinY) intY = _MinY;
                if (e.Y > _MaxY) intY = _MaxY;
                if (e.X < _MinX) intX = _MinX;
                if (e.X > _MaxX) intX = _MaxX;

                _startPtTop = new System.Drawing.Point(intX, intY);
                _endPtTop = new System.Drawing.Point(intX, intY);
            }
        }

        private void ibBottom_MouseDown(object sender, MouseEventArgs e)
        {
            if (Global.iSystem.Mode == ISystem.MODE.AUTO) return;

            if (e.Button == MouseButtons.Right)
            {
                int intY = e.Y;
                int intX = e.X;

                // 마우스 클릭 최대, 최소값 설정  
                if (e.Y < _MinY) intY = _MinY;
                if (e.Y > _MaxY) intY = _MaxY;
                if (e.X < _MinX) intX = _MinX;
                if (e.X > _MaxX) intX = _MaxX;

                _startPtBtm = new System.Drawing.Point(intX, intY);
                _endPtBtm = new System.Drawing.Point(intX, intY);
            }
        }

        private void ibBottom_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (ImageViewBtm.bUseShift)
                    {
                        if (!ImageViewBtm.bUseControlMoveCell)
                        {
                            int intY = e.Y;
                            int intX = e.X;

                            if (e.Y > _MaxY) intY = _MaxY;
                            if (e.Y < _MinY) intY = _MinY;
                            if (e.X > _MaxX) intX = _MaxX;
                            if (e.X < _MinX) intX = _MinX;

                            _endPtBtm = new System.Drawing.Point(intX, intY);

                            int curWidth = _endPtBtm.X - _startPtBtm.X;
                            int curHeight = _startPtBtm.Y - _endPtBtm.Y;

                            OpenCvSharp.Point ptStart = new OpenCvSharp.Point(_startPtBtm.X, _startPtBtm.Y);
                            OpenCvSharp.Point ptEnd = new OpenCvSharp.Point(_endPtBtm.X, _endPtBtm.Y);

                            Graphics g = ibBottom.CreateGraphics();

                            try
                            {
                                if (ptStart.X > ptEnd.X)
                                {
                                    if (ptStart.Y < ptEnd.Y)
                                    {
                                        SelectRtBtm = new Rectangle(ptEnd.X, ptStart.Y, ptStart.X - ptEnd.X, ptEnd.Y - ptStart.Y);

                                        //g.DrawRectangle(new Pen(Color.Orange, 5), rt);                                                                        
                                        g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                    }
                                    else
                                    {
                                        SelectRtBtm = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y);
                                        //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                        g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                    }

                                }
                                else
                                {
                                    if (ptStart.Y < ptEnd.Y)
                                    {
                                        if (ptStart.X < ptEnd.X)
                                        {
                                            SelectRtBtm = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);
                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                        }
                                        else
                                        {
                                            SelectRtBtm = new Rectangle(ptStart.X, ptStart.Y, ptEnd.X - ptStart.X, ptEnd.Y - ptStart.Y);

                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                        }

                                    }
                                    else
                                    {
                                        if (ptStart.X < ptEnd.X)
                                        {
                                            SelectRtBtm = new Rectangle(ptStart.X, ptEnd.Y, ptEnd.X - ptStart.X, ptStart.Y - ptEnd.Y);

                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                        }
                                        else
                                        {
                                            SelectRtBtm = new Rectangle(ptEnd.X, ptEnd.Y, ptStart.X - ptEnd.X, ptStart.Y - ptEnd.Y);
                                            //g.DrawRectangle(new Pen(Color.Orange, 5), rt);
                                            g.DrawRectangle(new Pen(new SolidBrush(Color.Blue), 5), SelectRtBtm);
                                        }

                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    ibBottom.Invalidate();
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnAddCellArray_Top_Click(object sender, EventArgs e)
        {
            try
            {
                int nIndex = Global.iData.Property_CellPosition.CellPositions_Top.Count + 1;
                int nBoundary = Global.iData.Property_CellPosition.CLICK_BOUNDARY * DEFINE.IMAGE_SIZE_FACTOR;

                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                ptCenter.X = Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.CellPositions_Top.Count - 1].Boundary.X + 100;
                ptCenter.Y = Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.CellPositions_Top.Count - 1].Boundary.Y + 100;

                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                CFormatCellPosition pos = new CFormatCellPosition();
                pos.Index = nIndex;
                pos.ACTUAL_POS_X = Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.CellPositions_Top.Count - 1].ACTUAL_POS_X + 5;
                pos.ACTUAL_POS_Y = Global.iData.Property_CellPosition.CellPositions_Top[Global.iData.Property_CellPosition.CellPositions_Top.Count - 1].ACTUAL_POS_Y + 5;
                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                pos.Boundary = rtSubMat;
                pos.Part = "+";
                Global.iData.Property_CellPosition.AddCellPosition_Top(pos);

                UpdateDispTop();
                CreatePositionTop();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnAddCellArray_Bottom_Click(object sender, EventArgs e)
        {
            try
            {
                int nIndex = Global.iData.Property_CellPosition.CellPositions_Bottom.Count + 1;
                int nBoundary = Global.iData.Property_CellPosition.CLICK_BOUNDARY * DEFINE.IMAGE_SIZE_FACTOR;

                System.Drawing.Point ptCenter = new System.Drawing.Point(Global.iData.Property_CellPosition.DISPLAY_OFFSET_X, Global.iData.Property_CellPosition.DISPLAY_OFFSET_Y);
                ptCenter.X = Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.CellPositions_Bottom.Count - 1].Boundary.X + 100;
                ptCenter.Y = Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.CellPositions_Bottom.Count - 1].Boundary.Y + 100;

                Rectangle rtBoundary = new Rectangle(ptCenter, new System.Drawing.Size(nBoundary, nBoundary));

                Rect rtSubMat = new Rect(rtBoundary.X - nBoundary / 2, rtBoundary.Y - nBoundary / 2, nBoundary, nBoundary);

                CFormatCellPosition pos = new CFormatCellPosition();
                pos.Index = nIndex;
                pos.ACTUAL_POS_X = Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.CellPositions_Bottom.Count - 1].ACTUAL_POS_X + 5;
                pos.ACTUAL_POS_Y = Global.iData.Property_CellPosition.CellPositions_Bottom[Global.iData.Property_CellPosition.CellPositions_Bottom.Count - 1].ACTUAL_POS_Y + 5;
                pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.DEFAULT_Z;
                pos.CHANNEL = Global.iData.Property_CellPosition.DEFAULT_CH;
                pos.Boundary = rtSubMat;
                pos.Part = "+";
                Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);

                UpdateDispBottom();
                CreatePositionBottom();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnCopyAtoB_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iData.Property_CellPosition.CellPositions_Bottom.Clear();
                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Top.Count; i++)
                {
                    CFormatCellPosition pos = new CFormatCellPosition();
                    pos.ACTUAL_POS_X = Global.iData.Property_CellPosition.CellPositions_Top[i].ACTUAL_POS_X;
                    pos.ACTUAL_POS_Y = Global.iData.Property_CellPosition.CellPositions_Top[i].ACTUAL_POS_Y;
                    pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.CellPositions_Top[i].ACTUAL_POS_Z;

                    pos.Boundary = new Rect(Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.Y, Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.Width, Global.iData.Property_CellPosition.CellPositions_Top[i].Boundary.Height);
                    pos.CHANNEL = Global.iData.Property_CellPosition.CellPositions_Top[i].CHANNEL;
                    pos.Index = Global.iData.Property_CellPosition.CellPositions_Top[i].Index;
                    pos.Part = "+"; //220217_SYH

                    Global.iData.Property_CellPosition.AddCellPosition_Bottom(pos);                    
                }
                
                UpdateDispBottom();
                CreatePositionBottom();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void btnCopyBtoA_Click(object sender, EventArgs e)
        {
            try
            {
                Global.iData.Property_CellPosition.CellPositions_Top.Clear();
                for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Bottom.Count; i++)
                {
                    CFormatCellPosition pos = new CFormatCellPosition();
                    pos.ACTUAL_POS_X = Global.iData.Property_CellPosition.CellPositions_Bottom[i].ACTUAL_POS_X;
                    pos.ACTUAL_POS_Y = Global.iData.Property_CellPosition.CellPositions_Bottom[i].ACTUAL_POS_Y;
                    pos.ACTUAL_POS_Z = Global.iData.Property_CellPosition.CellPositions_Bottom[i].ACTUAL_POS_Z;

                    pos.Boundary = new Rect(Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary.X, Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary.Y, Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary.Width, Global.iData.Property_CellPosition.CellPositions_Bottom[i].Boundary.Height);
                    pos.CHANNEL = Global.iData.Property_CellPosition.CellPositions_Bottom[i].CHANNEL;
                    pos.Index = Global.iData.Property_CellPosition.CellPositions_Bottom[i].Index;
                    pos.Part = ""; //220217_SYH

                    Global.iData.Property_CellPosition.AddCellPosition_Top(pos);
                }


                UpdateDispTop();
                CreatePositionTop();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        #region INIT

        private bool InitUI()
        {
            try
            {
                //ibTop.ZoomToFit();
                //ibBottom.ZoomToFit();

                UpdateDisplay();
                //ibLeft.MouseDoubleClick += new MouseEventHandler(ImageBox_MouseDoubleClickEvent);
                CreatePositionTop();
                CreatePositionBottom();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool UpdateParameter()
        {
            try
            {
                propertygrid_Parameter.SelectedObject = Global.iData.Property_CellPosition;
                propertygrid_Parameter.Update();

                tbDefaultX_Top.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_TOP.ToString();
                tbDefaultY_Top.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_TOP.ToString();
                tbDefaultZ_Top.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_TOP.ToString();
                //tbTopTotalPart.Text = Global.iData.Property_CellPosition.IS_DISPLAY_POLARITY.ToString(); //20220217_SYH


                tbDefaultX_Bottom.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_BOTTOM.ToString();
                tbDefaultY_Bottom.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_BOTTOM.ToString();
                tbDefaultZ_Bottom.Text = Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_BOTTOM.ToString();
                //tbBtmTotalPart.Text = Global.iData.Property_CellPosition.IS_DISPLAY_POLARITY.ToString(); //20220217_SYH

                UpdateDisplay();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool InitProperty()
        {
            try
            {
                Global.iData.Property_CellPosition.LoadConfig(Global.iSystem.Recipe.Name);
                propertygrid_Parameter.SelectedObject = Global.iData.Property_CellPosition;
                UpdateParameter();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }




        private bool InitIO()
        {
            try
            {
                if (Global.iDevice.ComMelsec.IsOpen)
                {
                    lbConnectionPLC.Text = "CONNECTED";
                    lbConnectionPLC.Style = MetroColorStyle.Default;
                }
                else
                {
                    lbConnectionPLC.Text = "DISCONNECTED";
                    lbConnectionPLC.Style = MetroColorStyle.Red;
                }

                Global.iSystem.Notice = "Initialize The Init I/O";

                Global.iDevice.ComMelsec.DI_TOP_LAMP_UPDATE.EventUpdateSignal += OnLampUpdate;
                Global.iDevice.ComMelsec.DI_BOTTOM_LAMP_UPDATE.EventUpdateSignal += OnLampUpdate;

                Global.iDevice.ComMelsec.DI_TOP_MODEL_UPDATE.EventUpdateSignal += OnModelUpdate;
                Global.iDevice.ComMelsec.DI_BOTTOM_MODEL_UPDATE.EventUpdateSignal += OnModelUpdate;

                Global.iDevice.ComMelsec.DI_DISPLAY_UPDATE.EventUpdateSignal += OnDisplayUpdate;

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            return true;
        }

        private bool InitEvent()
        {
            try
            {
                Global.iSystem.EventChangedMenu += OnChangedMenu;
                Global.iSystem.EventChangedNotice += OnChangedNotice;
                Global.iSystem.EventChangedRecipe += OnChangedRecipe;
                Global.iSystem.EventChangedMode += OnChangedMode;
                Global.iSystem.EventChangeSize += OnChangeSizeLog;

                if (Global.iSystem.EventChangedRecipe != null)
                {
                    Global.iSystem.EventChangedRecipe(null, null);
                }

                if (EventInitEnd != null)
                {
                    EventInitEnd(this, null);
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private void InitVersion()
        {
            Version assVer = Assembly.GetExecutingAssembly().GetName().Version;

            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(assVer.Build).AddSeconds(assVer.Revision * 2);

            // 최종 수정 날짜가 컴파일 시 자동으로 업데이트 됨.
            lbVersion.Text = string.Format("Version {0}",
                                                        buildDate.ToString());
        }

        private bool InitConfig()
        {
            try
            {
                Global.iSystem.Notice = "Initialize the Config";

                ImageViewTop.LoadImageBox(ibTop);
                ImageViewBtm.LoadImageBox(ibBottom);

                ibTop.KeyDown += OnibTopKeyDown;
                ibTop.KeyUp += IbTop_KeyUp;
                ibBottom.KeyDown += OnibBottomKeyDown;
                ibBottom.KeyUp += IbBottom_KeyUp;

                ImageSource = new Bitmap(ibTop.Width * DEFINE.IMAGE_SIZE_FACTOR, ibTop.Height * DEFINE.IMAGE_SIZE_FACTOR);
                Graphics g = Graphics.FromImage(ImageSource);
                g.Clear(Color.Black);
                //g.DrawImage(ImageSource, 0, 0, ImageSource.Width, ImageSource.Height);

                ibTop.Image = ImageSource;
                ibBottom.Image = ImageSource;

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private void IbBottom_KeyUp(object sender, KeyEventArgs e)
        {
            ImageViewKeyUp(false);
        }

        private void IbTop_KeyUp(object sender, KeyEventArgs e)
        {
            ImageViewKeyUp(true);
        }

        private void ImageViewKeyUp(bool bIsTop)
        {
            if(bIsTop)
            {
                ImageViewTop.bUseControlMoveCell = false;
                ImageViewTop.bUseShift = false;
                ImageViewTop.bUseAlt = false;
                SelectRtTop = new Rectangle();
            }
            else
            {
                ImageViewBtm.bUseControlMoveCell = false;
                ImageViewBtm.bUseShift = false;
                ImageViewBtm.bUseAlt = false;
                SelectRtBtm = new Rectangle();
            }
        }

        private bool InitRecipe(bool bInit = false)
        {
            try
            {
                lbRecipeName.Text = Global.iSystem.Recipe.Name;
                lbModelNo.Text = Global.iSystem.Recipe.MODEL_NO.ToString();

                Global.iData.Property_CellPosition.LoadConfig(Global.iSystem.Recipe.Name);

                fFontSizeFactor = 1.0f;
                //if (Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM == 1 || Global.iData.Property_CellPosition.GAP_X_MM_TOP == 1)
                //{
                //    fFontSizeFactor = 7.5f;
                //}
                //else
                //{
                //    fFontSizeFactor = 3.5f;
                //}

                fFontSizeTop = Global.iData.Property_CellPosition.GAP_X_MM_TOP * 2;
                fThicknessTop = Global.iData.Property_CellPosition.GAP_X_MM_TOP > 2 ? 5 : 1;

                fFontSizeBottom = Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM * 2;
                fThicknessBottom = Global.iData.Property_CellPosition.GAP_X_MM_BOTTOM > 2 ? 5 : 1;

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private bool InitMenu()
        {
            try
            {
                this.Show();

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }

        private bool InitThread()
        {
            try
            {
                timerThreadStatus.Enabled = true;
                timerConnection.Enabled = true;
                Global.iDevice.ComMelsec.StartThreadReadInput();

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

            return true;
        }
        #endregion

        #region KeyDown
        private void OnibTopKeyDown(object sender, KeyEventArgs e)
        {
            if (Global.iSystem.Mode == ISystem.MODE.AUTO) { return; }

            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Control) pnTopTotal.Visible = !pnTopTotal.Visible;
                    break;
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    MovePos(Global.iData.Property_CellPosition.CellPositions_Top, e.KeyCode, true);
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    ChannelChange(Global.iData.Property_CellPosition.CellPositions_Top, e.KeyCode, true);
                    break;
                case Keys.Delete:
                    DeletePos(Global.iData.Property_CellPosition.CellPositions_Top, true);
                    break;
                case Keys.ControlKey:
                    ImageViewTop.bUseControlMoveCell = true;
                    break;
                case Keys.ShiftKey:
                    ImageViewTop.bUseShift = true;
                    break;
                case Keys.Alt:
                    ImageViewTop.bUseAlt = true;
                    break;
                case Keys.F5:
                    RecoveryPos(true);
                    CreatePositionTop();//20220221_최노아선임
                    propertygrid_Rect.Update();
                    propertygrid_Parameter.Update();
                    break;
                case Keys.F7:
                    SaveParameter();
                    break;
            }            
        }

        private void OnibBottomKeyDown(object sender, KeyEventArgs e)
        {
            if (Global.iSystem.Mode == ISystem.MODE.AUTO) { return; }

            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Control) pnBtmTotal.Visible = !pnBtmTotal.Visible;
                    break;
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    MovePos(Global.iData.Property_CellPosition.CellPositions_Bottom, e.KeyCode, false);
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    ChannelChange(Global.iData.Property_CellPosition.CellPositions_Bottom, e.KeyCode, false);
                    break;
                case Keys.Delete:
                    DeletePos(Global.iData.Property_CellPosition.CellPositions_Bottom, false);
                    break;
                case Keys.ControlKey:
                    ImageViewBtm.bUseControlMoveCell = true;
                    break;
                case Keys.ShiftKey:
                    ImageViewBtm.bUseShift = true;
                    break;
                case Keys.Alt:
                    ImageViewBtm.bUseAlt = true;
                    break;
                case Keys.F5:
                    RecoveryPos(false);
                    break;
                case Keys.F7:
                    SaveParameter();
                    break;
            }
            CreatePositionBottom(); //20220221_최노아선임
            propertygrid_Rect.Update();
            propertygrid_Parameter.Update();
        }

        private void SaveParameter()
        {
            FormMessageBox FrmMessageBox = new FormMessageBox(string.Format("저장"), string.Format("파라미터를 저장하시겠습니까?"));

            if (FrmMessageBox.ShowDialog() == DialogResult.OK)
            {
                Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_TOP = float.Parse(tbDefaultX_Top.Text);
                Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_TOP = float.Parse(tbDefaultY_Top.Text);
                Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_TOP = float.Parse(tbDefaultZ_Top.Text);

                Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_BOTTOM = float.Parse(tbDefaultX_Bottom.Text);
                Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_BOTTOM = float.Parse(tbDefaultY_Bottom.Text);
                Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_BOTTOM = float.Parse(tbDefaultZ_Bottom.Text);

                Global.iData.Property_CellPosition.SaveConfig(Global.iSystem.Recipe.Name);
                UpdateParameter();
            }
        }

        private void RecoveryPos(bool bUpdateDispTop)
        {
            if (bUpdateDispTop)
            {
                var deepList = Global.iData.Property_CellPosition.CellPositions_Backup_Top.ConvertAll(o => new CFormatCellPosition(o.Index, o.Part, o.RESULT, o.Selected, o.ACTUAL_POS_X, o.ACTUAL_POS_Y, o.ACTUAL_POS_Z, o.Boundary, o.CHANNEL, o.Cell_Size)); // 20220221_SYH
                Global.iData.Property_CellPosition.CellPositions_Top = deepList;
                ClearCell(Global.iData.Property_CellPosition.CellPositions_Top, true);
                UpdateDispTop();
            }
            else
            {
                var deepList = Global.iData.Property_CellPosition.CellPositions_Backup_Bottom.ConvertAll(o => new CFormatCellPosition(o.Index, o.Part, o.RESULT, o.Selected, o.ACTUAL_POS_X, o.ACTUAL_POS_Y, o.ACTUAL_POS_Z, o.Boundary, o.CHANNEL, o.Cell_Size)); //20220221_SYH
                Global.iData.Property_CellPosition.CellPositions_Bottom = deepList;
                ClearCell(Global.iData.Property_CellPosition.CellPositions_Bottom, false);
                UpdateDispBottom();
            }
        }

        private void DeletePos(List<CFormatCellPosition> CellPositions, bool bUpdateDispTop)
        {
            List<CFormatCellPosition> RemoveIndex = new List<CFormatCellPosition>();

            for (int i = 0; i < CellPositions.Count; i++)
            {
                if (CellPositions[i].Selected)
                {
                    RemoveIndex.Add(CellPositions[i]);
                }
            }

            for (int i = 0; i < RemoveIndex.Count; i++)
            {
                CellPositions.Remove(RemoveIndex[i]);
                if (bUpdateDispTop) { Global.iData.Property_CellPosition.UpdateIndexTop(); }
                else { Global.iData.Property_CellPosition.UpdateIndexBottom(); }
            }

            if (bUpdateDispTop) { UpdateDispTop(); }
            else { UpdateDispBottom(); }
        }

        private void ChannelChange(List<CFormatCellPosition> CellPositions, Keys keys, bool bUpdateDispTop)
        {
            switch (keys)
            {
                case Keys.D1:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 1;
                        }
                    }
                    break;
                case Keys.D2:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 2;
                        }
                    }
                    break;
                case Keys.D3:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 3;
                        }
                    }
                    break;
                case Keys.D4:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 4;
                        }
                    }
                    break;
                case Keys.D5:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 5;
                        }
                    }
                    break;
                case Keys.D6:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 6;
                        }
                    }
                    break;
                case Keys.D7:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 7;
                        }
                    }
                    break;
                case Keys.D8:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 8;
                        }
                    }
                    break;
                case Keys.D9:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].CHANNEL = 9;
                        }
                    }
                    break;
            }

            if (bUpdateDispTop) { UpdateDispTop(); }
            else { UpdateDispBottom(); }
        }

        private void MovePos(List<CFormatCellPosition> CellPositions, Keys keys, bool bUpdateDispTop)
        {
            int nMoveX = 1;
            int nMoveY = 1;
            switch (keys)
            {
                case Keys.Up:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].MovePos(0, +nMoveY);
                        }
                    }
                    break;
                case Keys.Down:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].MovePos(0, -nMoveY);
                        }
                    }
                    break;
                case Keys.Left:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].MovePos(-nMoveX, 0);
                        }
                    }
                    break;
                case Keys.Right:
                    for (int i = 0; i < CellPositions.Count; i++)
                    {
                        if (CellPositions[i].Selected)
                        {
                            CellPositions[i].MovePos(nMoveX, 0);
                        }
                    }
                    break;
            }

            if (bUpdateDispTop) { UpdateDispTop(); }
            else { UpdateDispBottom(); }
        }
        #endregion

        #region CALL BACK
        private void OnDoubleClickPictureBox(object sender, MouseEventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnDoubleClickPictureBox(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {
                    PictureBox pb = (sender as PictureBox);

                    FormImageView FrmImageView = new FormImageView((Bitmap)pb.Image);
                    FrmImageView.Show();
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        public async void RunSendRecicpeNama()
        {
            Task taskInspection = Task.Run(() => SendRecicpeNama());
        }

        public async void ClearRecipeName()
        {
            try
            {
                char[] charArr = ("                    ").ToCharArray();
                int[] ints = new int[charArr.Length];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = (int)charArr[i];
                }

                Global.iDevice.ComMelsec.WriteArray(Global.iDevice.ComMelsec.DO_MODEL_NAME.ADDRESS, ints, ints.Length);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public async void SendRecicpeNama()
        {
            try
            {
                ClearRecipeName();

                string[] strRecipeName = Global.iSystem.Recipe.Name.Split('.');

                char[] charArr = strRecipeName[1].ToCharArray();
                int[] ints = new int[charArr.Length];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = (int)charArr[i];
                }

                Global.iDevice.ComMelsec.WriteArray(Global.iDevice.ComMelsec.DO_MODEL_NAME.ADDRESS, ints, ints.Length);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        public async void RunLampUpdateTop()
        {
            Task taskInspection = Task.Run(() => LampUpdateTop());
        }

        public async void LampUpdateTop()
        {
            try
            {
                Global.iDevice.ComMelsec.ReadD(Global.iDevice.ComMelsec.DI_TOP_LAMP_NUMBER.ADDRESS, out int nCallNumber);
                Global.iDevice.ComMelsec.ReadD(Global.iDevice.ComMelsec.DI_TOP_LAMP.ADDRESS, out int nTopLamp);

                Logger.WriteLog(LOG.Normal, "[Info] Call Number : {0}", nCallNumber);
                Logger.WriteLog(LOG.Normal, "[Info] Top Lamp Number : {0}", nTopLamp);
                Logger.WriteLog(LOG.Normal, "[Info] Top Position Count : {0}", Global.iData.Property_CellPosition.CellPositions_Top.Count);

                Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].RESULT = nTopLamp;

                UpdateDispTop();
                ibTop_MouseDoubleClick(ibTop, null);

                Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DI_TOP_LAMP_UPDATE.ADDRESS, CSignal.SIGNAL_OFF, 1);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public async void RunLampUpdateBottom()
        {
            Task taskInspection = Task.Run(() => LampUpdateBottom());
        }

        public async void LampUpdateBottom()
        {
            try
            {
                Global.iDevice.ComMelsec.ReadB(Global.iDevice.ComMelsec.DI_BOTTOM_LAMP_NUMBER.ADDRESS, out int nCallNumber);
                Global.iDevice.ComMelsec.ReadD(Global.iDevice.ComMelsec.DI_BOTTOM_LAMP.ADDRESS, out int nBtmLamp);

                Logger.WriteLog(LOG.Normal, "[Info] Call Number : {0}", nCallNumber);
                Logger.WriteLog(LOG.Normal, "[Info] Btm Lamp Number : {0}", nBtmLamp);
                Logger.WriteLog(LOG.Normal, "[Info] Btm Position Count : {0}", Global.iData.Property_CellPosition.CellPositions_Bottom.Count);

                Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].RESULT = nBtmLamp;

                UpdateDispBottom();
                ibBottom_MouseDoubleClick(ibBottom, null);

                Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DI_BOTTOM_LAMP_UPDATE.ADDRESS, CSignal.SIGNAL_OFF, 1);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public async void RunUpdateTop()
        {
            Task taskInspection = Task.Run(() => UpdateTop());
        }

        public async void UpdateTop()
        {
            try
            {
                int nWriteLength = 10;

                Global.iDevice.ComMelsec.ReadD(Global.iDevice.ComMelsec.DI_TOP_CALL_NUMBER.ADDRESS, out int nCallNumber);

                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, Call Number : {0}", nCallNumber);

                if (nCallNumber == 1)
                {
                    for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Top.Count; i++)
                    {
                        Global.iData.Property_CellPosition.CellPositions_Top[i].RESULT = 0;
                    }
                }

                int[] buff = new int[nWriteLength];
                buff[0] = (int)((Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_X + Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_TOP) * 100); //X
                buff[2] = (int)((Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_Y + Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_TOP) * 100); //Y
                buff[4] = (int)((Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_Z + Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_TOP) * 100); //Z
                buff[6] = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].CHANNEL;
                buff[8] = nCallNumber;

                Logger.WriteLog(LOG.Normal, "[Info] ////////////////////////////////////");
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, Position Count : {0}", Global.iData.Property_CellPosition.CellPositions_Top.Count);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, ACTUAL_POS_X : {0}", buff[0]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, ACTUAL_POS_Y : {0}", buff[2]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, ACTUAL_POS_Z : {0}", buff[4]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, Channel : {0}", buff[6]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Top, Call Number : {0}", nCallNumber);
                Logger.WriteLog(LOG.Normal, "[Info] ////////////////////////////////////");
                if (Global.iSystem.Mode == ISystem.MODE.AUTO)
                {
                    string strPosX = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_X.ToString("F2") + "mm";
                    string strPosY = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_Y.ToString("F2") + "mm";
                    string strPosZ = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].ACTUAL_POS_Z.ToString("F2") + "mm";
                    string strCh = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].CHANNEL.ToString();

                    lbPosNo_Top.Text = Global.iData.Property_CellPosition.CellPositions_Top[nCallNumber - 1].Index.ToString();
                    lbPosX_Top.Text = strPosX;
                    lbPosY_Top.Text = strPosY;
                    lbPosZ_Top.Text = strPosZ;
                    lbPosCh_Top.Text = strCh;
                }

                Global.iDevice.ComMelsec.WriteArray(Global.iDevice.ComMelsec.DO_TOP_X_CALL_POSITION.ADDRESS, buff, nWriteLength);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public async void RunUpdateBottom()
        {
            Task taskInspection = Task.Run(() => UpdateBottom());
        }

        public async void UpdateBottom()
        {
            try
            {
                int nWriteLength = 10;

                Global.iDevice.ComMelsec.ReadB(Global.iDevice.ComMelsec.DI_BOTTOM_CALL_NUMBER.ADDRESS, out int nCallNumber);

                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, Call Number : {0}", nCallNumber);

                if (nCallNumber == 1)
                {
                    for (int i = 0; i < Global.iData.Property_CellPosition.CellPositions_Bottom.Count; i++)
                    {
                        Global.iData.Property_CellPosition.CellPositions_Bottom[i].RESULT = 0;
                    }
                }

                int[] buff = new int[nWriteLength];
                buff[0] = (int)((Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_X + Global.iData.Property_CellPosition.ORIGIN_OFFSET_X_BOTTOM) * 100); //X
                buff[2] = (int)((Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_Y + Global.iData.Property_CellPosition.ORIGIN_OFFSET_Y_BOTTOM) * 100); //Y
                buff[4] = (int)((Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_Z + Global.iData.Property_CellPosition.ORIGIN_OFFSET_Z_BOTTOM) * 100); //Z
                buff[6] = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].CHANNEL;
                buff[8] = nCallNumber;

                Logger.WriteLog(LOG.Normal, "[Info] ////////////////////////////////////");
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, Position Count : {0}", Global.iData.Property_CellPosition.CellPositions_Bottom.Count);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, ACTUAL_POS_X : {0}", buff[0]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, ACTUAL_POS_Y : {0}", buff[2]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, ACTUAL_POS_Z : {0}", buff[4]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, Channel : {0}", buff[6]);
                Logger.WriteLog(LOG.Normal, "[Info] Update Result Btm, Call Number : {0}", nCallNumber);
                Logger.WriteLog(LOG.Normal, "[Info] ////////////////////////////////////");

                if (Global.iSystem.Mode == ISystem.MODE.AUTO)
                {
                    string strPosX = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_X.ToString("F2") + "mm";
                    string strPosY = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_Y.ToString("F2") + "mm";
                    string strPosZ = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].ACTUAL_POS_Z.ToString("F2") + "mm";
                    string strCh = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].CHANNEL.ToString();

                    lbPosNo_Bottom.Text = Global.iData.Property_CellPosition.CellPositions_Bottom[nCallNumber - 1].Index.ToString();
                    lbPosX_Bottom.Text = strPosX;
                    lbPosY_Bottom.Text = strPosY;
                    lbPosZ_Bottom.Text = strPosZ;
                    lbChannel_Bottom.Text = strCh;
                }

                Global.iDevice.ComMelsec.WriteArray(Global.iDevice.ComMelsec.DO_BOTTOM_X_CALL_POSITION.ADDRESS, buff, nWriteLength);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void OnLampUpdate(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnLampUpdate(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                CSignal signal = (CSignal)sender;

                if (signal.Name == "DI_TOP_LAMP_UPDATE")
                {
                    if (signal.Current == CSignal.SIGNAL_ON)
                    {
                        signal.IsDisplay = false;
                        RunLampUpdateTop();
                    }
                }
                else
                {
                    if (signal.Current == CSignal.SIGNAL_ON)
                    {
                        signal.IsDisplay = false;
                        RunLampUpdateBottom();
                    }
                }
            }
        }


        private void OnModelUpdate(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnModelUpdate(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                CSignal signal = (CSignal)sender;

                if (signal.Name == "DI_TOP_MODEL_UPDATE")
                {
                    if (signal.Current == CSignal.SIGNAL_ON)
                    {
                        tcSection.SelectedIndex = 0;
                        signal.IsDisplay = false;
                        RunUpdateTop();
                    }
                }
                else
                {
                    if (signal.Current == CSignal.SIGNAL_ON)
                    {
                        tcSection.SelectedIndex = 1;
                        signal.IsDisplay = false;
                        RunUpdateBottom();
                    }
                }
            }
        }

        private void OnDisplayUpdate(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnDisplayUpdate(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                CSignal signal = (CSignal)sender;
                int nWriteLength = 8;

                if (signal.Current == CSignal.SIGNAL_ON)
                {
                    signal.IsDisplay = false;
                    Global.iDevice.ComMelsec.WriteD(signal.ADDRESS, 0);
                    UpdateDisplay();
                }
            }
        }

        private void OnClickOperation(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((MetroTile)sender).Text;
                SendRecicpeNama();
                switch (strIndex)
                {
                    case "AUTO":
                        if (Global.iSystem.Mode != ISystem.MODE.AUTO) Global.iSystem.Mode = ISystem.MODE.AUTO;
                        break;
                    case "STOP":
                        if (Global.iSystem.Mode != ISystem.MODE.READY) Global.iSystem.Mode = ISystem.MODE.READY;

                        break;
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        private void OnMainCameraControl(object sender, EventArgs e)
        {
            try
            {
                string strIndex = "";

                if (sender is MetroTile) strIndex = ((MetroTile)sender).Text;
                else if (sender is ToolStripButton) strIndex = ((ToolStripButton)sender).Text;
                else if (sender is Button) strIndex = ((Button)sender).Text;

                switch (strIndex)
                {
                    //case "그랩":
                    //    for (int i = 0; i < Global.CamManager.Cameras.Count; i++)
                    //    {
                    //        Global.CamManager.Cameras[i].Grab();
                    //    }
                    //    break;
                    //case "라이브":
                    //    (sender as MetroTile).Text = "라이브 중지";

                    //    for (int i = 0; i < Global.CamManager.Cameras.Count; i++)
                    //    {
                    //        Global.CamManager.Cameras[i].Live(true);
                    //    }
                    //    break;
                    //case "라이브 중지":
                    //    (sender as MetroTile).Text = "라이브";

                    //    for (int i = 0; i < Global.CamManager.Cameras.Count; i++)
                    //    {
                    //        Global.CamManager.Cameras[i].Live(false);
                    //    }
                    //    //StopThreadLive();
                    //    break;
                    case "불러오기":
                        LoadImage();
                        break;
                    case "저장하기":
                        SaveImage();
                        break;
                    case "보기 (전체)":
                        break;
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        private void OnChangedRecipe(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {
                    InitRecipe();
                    InitProperty();
                    InitUI();
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }
        private void OnChangedNotice(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnChangedRecipe(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                lbNotice.Text = Global.iSystem.Notice;
            }
        }

        private void OnChangedMenu(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedMenu(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                switch (Global.iSystem.SelectedMenu)
                {
                    case "메인":
                    case "MAIN":
                    case "Main":
                        if (m_FrmRecipe != null) { m_FrmRecipe.Hide(); }

                        splitContainerMain.Show();
                        break;
                    case "모델":
                    case "MODEL":
                    case "Model":
                    case "레시피":
                    case "RECIPE":
                    case "Recipe":
                        FormRecipe FrmRecipe = new FormRecipe();
                        FrmRecipe.ShowDialog();
                        break;
                    case "설정":
                        break;
                    case "SETTINGS":
                        break;
                    case "Settings":
                        break;
                    case "닫기":
                        break;
                    case "EXIT":
                    case "Exit":
                        if (Global.iSystem.Mode == ISystem.MODE.AUTO)
                        {
                            Global.iSystem.Notice = "Can't Close the Program, because Current Mode is Auto";
                            return;
                        }
                        else
                        {
                            //string strFileName = Path.GetFileName(Assembly.GetEntryAssembly().Location).Replace(".exe", "");
                            //Process[] processList = Process.GetProcessesByName("IntelligentVision");
                            //if (processList.Length > 0)
                            //{
                            //    processList[0].Kill();
                            //}
                            //Global.Close();
                            //this.Close();
                        }

                        Global.Close();
                        this.Close();
                        break;
                }
            }
        }
        private void OnClickMenu(object sender, EventArgs e)
        {
            try
            {
                string strIndex = ((MetroTile)sender).Text;
                Global.iSystem.SelectedMenu = strIndex;

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void OnChangeSizeLog(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnChangeSizeLog(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                LogFullScreen = !LogFullScreen;
                if (LogFullScreen)
                {
                    pnLog.Size = new System.Drawing.Size(LogSize.Width, LogSize.Height + 500);
                }
                else
                {
                    pnLog.Size = LogSize;
                }

                pnLog.Update();
            }
        }

        private void OnChangedMode(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnChangedMode(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                switch (Global.iSystem.Mode)
                {
                    case ISystem.MODE.AUTO:
                        EnableMenu(false);

                        btnOperationAuto.Enabled = false;
                        btnOperationAuto.Style = MetroFramework.MetroColorStyle.Teal;

                        btnOperationStop.Enabled = true;
                        btnOperationStop.Style = MetroFramework.MetroColorStyle.Silver;

                        propertygrid_Rect.Enabled = false;
                        propertygrid_Parameter.Enabled = false;
                        btnCreateCellArray_Top.Enabled = false;
                        btnCreateCellArray_Btm.Enabled = false;
                        btnLoadTop.Enabled = false;
                        btnSaveTop.Enabled = false;
                        tbDefaultX_Top.Enabled = false;
                        tbDefaultY_Top.Enabled = false;
                        tbDefaultZ_Top.Enabled = false;

                        tbDefaultX_Bottom.Enabled = false;
                        tbDefaultY_Bottom.Enabled = false;
                        tbDefaultZ_Bottom.Enabled = false;

                        RunSendRecicpeNama();

                        int nSafetyTopZ = (int)(IGlobal.Instance.iData.Property_CellPosition.SAFETY_Z_TOP * 100);
                        int nSafetyBtmZ = (int)(IGlobal.Instance.iData.Property_CellPosition.SAFETY_Z_BOTTOM * 100);

                        Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DO_TOP_SAFETY_Z.ADDRESS, nSafetyTopZ);
                        Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DO_BOTTOM_SAFETY_Z.ADDRESS, nSafetyBtmZ);

                        Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DO_TOP_POINT_TOTAL_COUN5T.ADDRESS, IGlobal.Instance.iData.Property_CellPosition.CellPositions_Top.Count);
                        Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DO_BOTTOM_POINT_TOTAL_COUN5T.ADDRESS, IGlobal.Instance.iData.Property_CellPosition.CellPositions_Bottom.Count);

                        Global.iDevice.ComMelsec.WriteD(Global.iDevice.ComMelsec.DO_MODEL_NO.ADDRESS, Global.iSystem.Recipe.MODEL_NO);

                        //Global.SeqVision.StartThreadVision();
                        break;
                    case ISystem.MODE.READY:
                        EnableMenu(true);

                        btnOperationAuto.Enabled = true;
                        btnOperationAuto.Style = MetroFramework.MetroColorStyle.Silver;

                        btnOperationStop.Enabled = false;
                        btnOperationStop.Style = MetroFramework.MetroColorStyle.Teal;

                        propertygrid_Rect.Enabled = true;
                        propertygrid_Parameter.Enabled = true;
                        btnCreateCellArray_Top.Enabled = true;
                        btnCreateCellArray_Btm.Enabled = true;
                        btnLoadTop.Enabled = true;
                        btnSaveTop.Enabled = true;

                        tbDefaultX_Top.Enabled = true;
                        tbDefaultY_Top.Enabled = true;
                        tbDefaultZ_Top.Enabled = true;

                        tbDefaultX_Bottom.Enabled = true;
                        tbDefaultY_Bottom.Enabled = true;
                        tbDefaultZ_Bottom.Enabled = true;

                        break;
                    case ISystem.MODE.ALARM:
                        EnableMenu(true);

                        btnOperationAuto.Enabled = true;
                        btnOperationAuto.Style = MetroFramework.MetroColorStyle.Teal;

                        btnOperationStop.Enabled = false;
                        btnOperationStop.Style = MetroFramework.MetroColorStyle.Silver;

                        CUtil.ShowMessageBox("Alarm", "Check the Alarm");
                        break;
                }
            }
        }

        private void OnUpdateUi(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnUpdateUi(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void InitImageDailyDirectory()
        {
            CUtil.InitImageDirectory((@"D:\\SAVE_IMAGE\\ORIGIANL\\OK\\" + DateTime.Now.ToString("yyMMdd")));
            CUtil.InitImageDirectory((@"D:\\SAVE_IMAGE\\ORIGIANL\\NG\\" + DateTime.Now.ToString("yyMMdd")));
            CUtil.InitImageDirectory((@"D:\SAVE_IMAGE\RESULT\OK\" + DateTime.Now.ToString("yyMMdd")) + "\\" + DateTime.Now.ToString("HH"));
            CUtil.InitImageDirectory((@"D:\SAVE_IMAGE\RESULT\NG\" + DateTime.Now.ToString("yyMMdd")) + "\\" + DateTime.Now.ToString("HH"));
        }

        private void OnInspResult(object sender, InspResultArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnInspResult(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {

                try
                {

                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        #endregion

        #region TIMER / THREAD
        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            lbDateTime.Text = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");

            try
            {
                double dDrivePercentC = CUtil.DrivePercent("C:\\", out double dCDriveTotalSize, out double dCDriveUsedSize);
                double dDrivePercentD = CUtil.DrivePercent("D:\\", out double dDDriveTotalSize, out double dDDriveUsedSize);

                lbDriveC.Text = $"Drive (C:) : {dDrivePercentC.ToString("F1")}%";
                pgbDriveC.Value = (int)dDrivePercentC;

                if (Global.iSystem.Permission == ISystem.PERMISSION.OPERATOR) lbPermission.Text = "OPERATOR";
                else if (Global.iSystem.Permission == ISystem.PERMISSION.ENGINEER) lbPermission.Text = "ENGINEER";
                else if (Global.iSystem.Permission == ISystem.PERMISSION.ADMINISTRATOR) lbPermission.Text = "ADMIN";
            }
            catch (Exception Desc)
            {

            }
        }

        private void timerConnection_Tick(object sender, EventArgs e)
        {
            try
            {

                if (Global.iDevice.ComMelsec.IsOpen)
                {
                    lbConnectionPLC.Text = "CONNECTED";
                    lbConnectionPLC.Style = MetroColorStyle.Default;
                }
                else
                {
                    lbConnectionPLC.Text = "DISCONNECTED";
                    lbConnectionPLC.Style = MetroColorStyle.Red;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        #endregion

        #region FUNCTION
        private bool EnableMenu(bool bEnable)
        {
            try
            {
                btnMenuMain.Enabled = bEnable;
                btnMenuRecipe.Enabled = bEnable;
                btnMenuClose.Enabled = bEnable;

                if (bEnable)
                {
                    btnMenuMain.Style = MetroFramework.MetroColorStyle.Default;
                    btnMenuRecipe.Style = MetroFramework.MetroColorStyle.Default;
                    btnMenuClose.Style = MetroFramework.MetroColorStyle.Default;
                }
                else
                {
                    btnMenuMain.Style = MetroFramework.MetroColorStyle.Silver;
                    btnMenuRecipe.Style = MetroFramework.MetroColorStyle.Silver;
                    btnMenuClose.Style = MetroFramework.MetroColorStyle.Silver;
                }
            }
            catch (Exception Desc)
            {
                return false;
            }

            return true;
        }

        private void LoadImage()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                ofd.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string strFilePath = ofd.FileName;

                    //if (Global.CamManager.Cameras[0].IsOpen)
                    //{
                    //    using (Mat matLoad = new Mat(strFilePath))
                    //    {
                    //        //Global.CamManager.Cameras[0].ImageLast = matLoad.Clone();
                    //        Global.CamManager.Cameras[0].Display.ImageIpl = matLoad.Clone();
                    //    }
                    //}

                    Global.iSystem.Notice = string.Format("Load Image ==> {0}", strFilePath);

                    Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void SaveImage()
        {
            try
            {
                //for (int nSaveCount = 0; nSaveCount < Global.CamManager.Cameras.Count; nSaveCount++)
                //{
                //    IUtil.InitDirectory(DEFINE.CAPTURE);
                //    string sDirPath = Application.StartupPath + "\\" + DEFINE.CAPTURE + "\\" + DateTime.Now.ToString("yyyyMMdd-HHmmssfff") + "Cam" + nSaveCount.ToString() + ".jpg";
                //    Global.CamManager.Cameras[nSaveCount].ImageGrab.Save(sDirPath);
                //    Global.iSystem.Notice = string.Format("Save Image ==> {0}", sDirPath);
                //    Logger.WriteLog(LOG.Normal, "Save Image ==> {0}", sDirPath);
                //}

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        #endregion

        #region UI EVENT
        private void cbMainColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            metroStyleManager.Style = (MetroFramework.MetroColorStyle)(int)Convert.ToInt32(cbMainColor.SelectedIndex + 1);
        }

        private void btnMenuIO_Click(object sender, EventArgs e)
        {
            try
            {
                FormIO_BD FrmIO = new FormIO_BD();
                FrmIO.Show();
            }
            catch (Exception Desc)
            {
            }
        }

        #endregion

        #region Thread
        public IThreadStatus ThreadStatusVision = new IThreadStatus();
        public void StartThreadVision()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadVision));
            t.Start(ThreadStatusVision);
        }

        public void ResetThreadVision()
        {
            ThreadStatusVision.End();
        }

        public void StopThreadVision()
        {
            if (!ThreadStatusVision.IsExit())
            {
                ThreadStatusVision.Stop(100);
            }
        }

        int m_nAliveTime = 0;
        private void ThreadVision(object ob)
        {
            IThreadStatus ThreadStatus = (IThreadStatus)ob;

            ThreadStatus.Start("Vision Inspection");
            Logger.WriteLog(LOG.Normal, "Vision Inspection");

            try
            {

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }
        #endregion
    }
}
