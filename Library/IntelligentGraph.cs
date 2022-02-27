﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Globalization;
using System.Xml;
using System.Threading;
using System.Reflection;
using System.IO;
using OpenCvSharp.Flann;

namespace IntelligentFactory
{
    public partial class IntelligentGraph : Control
    {
        private string m_strDesc = "EMPTY";
        public string Desc
        {
            get { return m_strDesc; }
            set { m_strDesc = value; }
        }

        private string m_strUnit = "mVDC";
        public string Unit
        {
            get { return m_strUnit; }
            set { m_strUnit = value; }
        }

        private int m_nIndex = 0;
        public int Index
        {
            get { return m_nIndex; }
            set { m_nIndex = value; }
        }

        public class LineHandle
        {
            private Line m_Line = null;
            private IntelligentGraph m_Owner = null;

            // ===================================================================

            public LineHandle(ref Object line, IntelligentGraph owner)
            {
                /* A small hack to get around the compiler error CS0051: */
                if (string.Compare(line.GetType().Name, "Line") != 0)
                {
                    throw new System.ArithmeticException(
                        "LineHandle: First Parameter must be " +
                        "type of 'Line' cast to base 'Object'");
                }

                m_Line = (Line)line;
                m_Owner = owner;
            }

            // ===================================================================

            /// <summary> 
            /// Clears any currently displayed magnitudes.
            /// </summary>

            public void Clear()
            {
                m_Line.m_MagnitudeList.Clear();
                m_Owner.UpdateGraph();                    
            }  

            public Color Color
            {
                set
                {
                    if (m_Line.m_Color != value)
                    {
                        m_Line.m_Color = value;
                        m_Owner.Refresh();
                    }
                }
                get { return m_Line.m_Color; }

            }

            public uint Thickness
            {
                set
                {
                    if (m_Line.m_Thickness != value)
                    {
                        m_Line.m_Thickness = value;
                        m_Owner.Refresh();
                    }
                }
                get { return m_Line.m_Thickness; }
            }

            public bool Visible
            {
                set
                {
                    if (m_Line.m_bVisible != value)
                    {
                        m_Line.m_bVisible = value;
                        m_Owner.Refresh();
                    }
                }
                get { return m_Line.m_bVisible; }
            }

            public bool ShowAsBar
            {
                set
                {
                    if (m_Line.m_bShowAsBar != value)
                    {
                        m_Line.m_bShowAsBar = value;
                        m_Owner.Refresh();
                    }
                }
                get { return m_Line.m_bShowAsBar; }
            }  
        }

        private class Line
        {
            public List<int> m_HighLimitList = new List<int>();
            public List<int> m_LowLimitList = new List<int>();
            public List<double> m_MagnitudeList = new List<double>();
            public Color  m_Color = Color.Green;
            public string m_NameID = "IR";
            public int    m_NumID = -1;
            public uint   m_Thickness = 1;
            public bool   m_bShowAsBar = false;
            public bool   m_bVisible = true;

            public double m_dMax = double.MinValue;
            public double m_dMin = double.MaxValue;

            public Line(string name)
            {
                m_NameID = name;
            }

            public Line(int num)
            {
                m_NumID = num;
            }
        }

        private PowerGraphConfig m_PowerGraphConfig = new PowerGraphConfig();
        public PowerGraphConfig PowerGraphConfig
        {
            get { return m_PowerGraphConfig; }
            set { m_PowerGraphConfig = value;  }
        }

        private Color  m_TextColor = Color.Aquamarine;
        private Color  m_GridColor = Color.DimGray;
        private string m_MaxLabel = "Max";
        private string m_MidLabel = "Mid";
        private string m_MinLabel = "Minimum";
        private bool   m_bHighQuality = true;
        private bool   m_bAutoScale = false;
        private bool   m_bMinLabelSet = false;
        private bool   m_bMidLabelSet = false;
        private bool   m_bMaxLabelSet = false;
        private bool   m_bShowMinMax = true;
        private bool   m_bShowCurrentData = true;
        private bool   m_bShowGrid = true;
        private int    m_MoveOffset = 0;
        private int    m_MaxCoords = -1;
        private int    m_LineInterval = 5;
        private double m_MaxPeek = 100;
        private double m_MinPeek = 0;
        private int    m_GridSize = 15;
        private int    m_OffsetX = 0;

        private List<Line> m_Lines = new List<Line>();    
        
        public int m_CurrPos = 0;

        public IntelligentGraph()
        {                
            InitializeComponent();
            InitializeStyles();

            UpdateStyles(m_PowerGraphConfig);
        }

        public IntelligentGraph(int nIndex, string strDesc)
        {
            InitializeComponent();
            InitializeStyles();

            m_nIndex = nIndex;
            m_strDesc = strDesc;

            UpdateStyles(m_PowerGraphConfig);
        }

        public IntelligentGraph(Form Parent)
        {
            Parent.Controls.Add(this);

            InitializeComponent();
            InitializeStyles();
        }

        public IntelligentGraph(Form parent, Rectangle rectPos)
        {
            parent.Controls.Add(this);

            Location = rectPos.Location;
            Height = rectPos.Height;
            Width = rectPos.Width;

            InitializeComponent();
            InitializeStyles();
        }

        private void InitializeStyles()
        {
            BackColor = Color.Black;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = true;

            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public void UpdateStyles(PowerGraphConfig config)
        {            
            m_PowerGraphConfig.TextColor = config.TextColor;
            m_PowerGraphConfig.CurrentDataColor = config.CurrentDataColor;
            m_PowerGraphConfig.BackgroundColor = config.BackgroundColor;
            m_PowerGraphConfig.GridColor = config.GridColor;
            m_PowerGraphConfig.SpecInColor = config.SpecInColor;
            m_PowerGraphConfig.SpecOutColor = config.SpecOutColor;
            m_PowerGraphConfig.WarningLineColor = config.WarningLineColor;
            m_PowerGraphConfig.AlarmLineColor = config.AlarmLineColor;
            m_PowerGraphConfig.BorderLineColor = config.BorderLineColor;

            m_PowerGraphConfig.IsViewModeLine = config.IsViewModeLine;
            m_PowerGraphConfig.IsReverse = config.IsReverse;

            m_PowerGraphConfig.GapX = config.GapX;
            m_PowerGraphConfig.GapY = config.GapY;

            m_PowerGraphConfig.FontSize = config.FontSize;
            m_PowerGraphConfig.LeftFontSize = config.LeftFontSize;
            m_PowerGraphConfig.PointSize = config.PointSize;

            m_PowerGraphConfig.Max = config.Max;
            m_PowerGraphConfig.Min = config.Min;

            m_PowerGraphConfig.ListMaxCount = config.ListMaxCount;

            m_PowerGraphConfig.IsLimitPeak = config.IsLimitPeak;

            this.MaxValue = m_PowerGraphConfig.Max;
            this.MinValue = m_PowerGraphConfig.Min;

            this.MaxLabel = m_PowerGraphConfig.Max.ToString();
            this.MinLabel = m_PowerGraphConfig.Min.ToString();

            LineInterval = (ushort)config.GapX;
            this.BackColor = m_PowerGraphConfig.BackgroundColor;

            this.Refresh();
        }

        public void UpdateStyles()
        {
            this.Refresh();
        }

        public Color TextColor
        {
            set
            {
                if (m_TextColor != value)
                {
                    m_TextColor = value;
                    Refresh();
                }
            }
            get { return m_TextColor; }
        }

        public Color GridColor
        {
            set
            {
                if (m_GridColor != value)
                {
                    m_GridColor = value;
                    Refresh();
                }
            }
            get { return m_GridColor; }
        }

        public ushort LineInterval
        {
            set
            {
                if ((ushort)m_LineInterval != value)
                {
                    m_LineInterval = (int)value;
                    m_MaxCoords = -1; // Recalculate
                    Refresh();
                }
            }
            get { return (ushort)m_LineInterval; }
        }

        public string MaxLabel
        {
            set
            {
                m_bMaxLabelSet = true;

                if (string.Compare(m_MaxLabel, value) != 0)
                {
                    m_MaxLabel = value;
                    m_MaxCoords = -1; 
                    Refresh();
                }
            }
            get { return m_MaxLabel; }
        }

        public string MidLabel
        {
            set
            {
                m_bMidLabelSet = true;

                if (string.Compare(m_MidLabel, value) != 0)
                {
                    m_MidLabel = value;
                    Refresh();
                }
            }
            get { return m_MidLabel; }
        }


        public string MinLabel
        {
            set
            {
                m_bMinLabelSet = true;

                if (string.Compare(m_MinLabel, value) != 0)
                {
                    m_MinLabel = value;
                    m_MaxCoords = -1; 
                    Refresh();
                }
            }
            get { return m_MinLabel; }
        }

        public ushort GridSize
        {
            set
            {
                if (m_GridSize != (int)value)
                {
                    m_GridSize = (int)value;
                    Refresh();

                }
            }
            get { return (ushort)m_GridSize; }
        }

        public double MaxPeekMagnitude
        {
            set 
            { 
                m_MaxPeek = value;
                RefreshLabels(); 
            }
            get { return m_MaxPeek; }
        }

        public double MinPeekMagnitude
        {
            set 
            { 
                m_MinPeek = value; 
                RefreshLabels(); 
            }
            get { return m_MinPeek; }
        }


        public bool AutoAdjustPeek
        {
            set
            {
                if (m_bAutoScale != value)
                {
                    m_bAutoScale = value;
                    Refresh();
                }
            }
            get { return m_bAutoScale; }
        }

        public bool HighQuality
        {
            set
            {
                if (value != m_bHighQuality)
                {
                    m_bHighQuality = value;
                    Refresh(); // Force redraw
                }
            }
            get { return m_bHighQuality; }
        }

        public bool ShowLabels
        {
            set
            {
                if (m_bShowMinMax != value)
                {
                    m_bShowMinMax = value;

                    m_MaxCoords = -1;

                    Refresh();
                }
            }
            get { return m_bShowMinMax; }
        }

        public bool ShowGrid
        {
            set
            {
                if (m_bShowGrid != value)
                {
                    m_bShowGrid = value;
                    Refresh();
                }
            }
            get { return m_bShowGrid; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            m_MaxCoords = -1;

            Refresh();

            base.OnSizeChanged(e); 
        }


        private float m_fZoomScale = 1.0F;
        public float ZoomScale
        {
            get { return m_fZoomScale; }
            set
            {
                float fZooomScale = value;
                if (fZooomScale < 1)
                {
                    m_fZoomScale = 1.0F;
                }
                else
                {
                    float fZoomGap = ((MaxValue - MinValue) * (fZooomScale - 1.0F)) / 2.0F;

                    float fMid = (MaxValue - MinValue) / 2.0F;
                    if (fMid - fZoomGap > 1)
                    {
                        m_fZoomScale = value;
                    }
                }
            }
        }

        int m_nDulplexPointCount = 1;
        public int DulplexPointCount
        {
            get
            {
                return m_nDulplexPointCount;
            }

            set
            {
                int nDulplexPointCount = value;
                if(nDulplexPointCount <= 0)
                {
                    nDulplexPointCount = 1;
                }

                m_nDulplexPointCount = nDulplexPointCount;
            }
        }

        public float MaxValue
        {
            get;
            set;
        }

        public float MinValue
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;// CreateGraphics();

            SmoothingMode prevSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = (m_bHighQuality ? SmoothingMode.HighQuality
                                                : SmoothingMode.Default);

            m_OffsetX = 0;

            DrawLabels(ref g);

            DrawGrid(ref g);

            DrawLines(ref g);

            if (m_OffsetX != 0)
            {
                g.Clip = new Region(new Rectangle(m_OffsetX, 0, Width - m_OffsetX, Height));
            }

            DrawCurrentData(ref g);           

            g.ResetClip();
            g.SmoothingMode = prevSmoothingMode;
        }

        public void UpdateGraph()
        {
            int greatestMCount = 0;
            foreach (Line line in m_Lines)
            {
                if (greatestMCount < line.m_MagnitudeList.Count)
                {
                    greatestMCount = line.m_MagnitudeList.Count;
                }
            }

            if (greatestMCount >= m_MaxCoords* DulplexPointCount)
            {
                m_MoveOffset =
                    (m_MoveOffset - (((greatestMCount - (m_MaxCoords/ DulplexPointCount)) + 1) * m_LineInterval))
                    % m_GridSize;
            }

            CullAndEqualizeMagnitudeCounts();
        }

        protected void CalculateMaxPushPoints()
        {
            m_MaxCoords = ((Width - m_OffsetX) / m_LineInterval) + 2
                                + (((Width - m_OffsetX) % m_LineInterval) != 0 ? 1 : 0);

            if (m_MaxCoords <= 0)
            {
                m_MaxCoords = 1;
            }
        }

        protected void DrawLabels(ref Graphics g)
        {   
            SizeF maxSize = g.MeasureString(m_MaxLabel, Font);
            SizeF minSize = g.MeasureString(m_MinLabel, Font);
            Font FontLeftData = new Font("Arial", m_PowerGraphConfig.LeftFontSize, FontStyle.Bold);

            int textWidth = (int)((maxSize.Width > minSize.Width)
                            ? maxSize.Width
                            : minSize.Width) + 6;

            SolidBrush textBrush = new SolidBrush(m_TextColor);          

            float fZoomGap = ((MaxValue - MinValue) * (m_fZoomScale - 1.0F))/2.0F;

            string maxlabel = string.Format("{0}", (Math.Round(MaxValue - fZoomGap) + OffsetY).ToString("F1"));

            g.DrawString(maxlabel, FontLeftData, textBrush,
                                textWidth / 2 - (maxSize.Width / 2),
                                2);                              
                        
            string midlabel = string.Format("{0}", (Math.Round((MaxValue - MinValue)/2) + MinValue+ OffsetY).ToString("F1"));
            g.DrawString(midlabel, FontLeftData, textBrush,
                            textWidth / 2 - (maxSize.Width / 2),
                            (Height - minSize.Height)/2 - 2 );

            string minlabel = string.Format("{0}", (Math.Round(MinValue + fZoomGap)+ OffsetY).ToString("F1"));
            
            g.DrawString(minlabel, FontLeftData, textBrush,
                                textWidth / 2 - (minSize.Width / 2),
                                Height - minSize.Height - 2);

            textBrush.Dispose();

            Pen borderPen = new Pen(m_GridColor, 1);
            g.DrawLine(borderPen, textWidth + 6, 0, textWidth + 6, Height);

            borderPen.Dispose();

            m_OffsetX = textWidth + 6;
        }

        public const int DATA1_LINE = 0;
        public const int DATA2_LINE = 1;
        protected void DrawCurrentData(ref Graphics g)
        {
            if (m_Lines.Count != 2) return;

            Line DataLineOCV = m_Lines[0];
            Line DataLineIR = m_Lines[1];

            Font FontCurrentData = new Font("Arial", m_PowerGraphConfig.FontSize, FontStyle.Bold);

            SizeF szText = g.MeasureString("OCV MAX : 00000000 V", FontCurrentData);

            int textWidth = (int)(szText.Width + 6.0F);
            int textHeight = (int)(szText.Height + 6.0F);

            if (DataLineOCV != null)
            {
                if (DataLineOCV.m_MagnitudeList.Count > 0)
                {
                    SolidBrush textBrush = new SolidBrush(DataLineOCV.m_Color);

                    string strDara = string.Format("OCV DATA : {0}VDC", DataLineOCV.m_MagnitudeList[DataLineOCV.m_MagnitudeList.Count - 1]);
                    //string strMax  = string.Format("OCV AVG : {0}V", DataLineOCV.m_MagnitudeList.Average().ToString("F3"));

                    g.DrawString(strDara, FontCurrentData, textBrush, new PointF((textWidth * 2), this.Height - (textHeight * 2)));
                    //g.DrawString(strMax, FontCurrentData, textBrush, new PointF((textWidth * 3), this.Height - (textHeight * 2)));

                    textBrush.Dispose();
                }
            }

            if (DataLineIR != null)
            {
                if (DataLineIR.m_MagnitudeList.Count > 0)
                {
                    SolidBrush textBrush = new SolidBrush(DataLineIR.m_Color);

                    string strDara = string.Format("{0} DATA : {1}{2}", DataLineIR.m_NameID, DataLineIR.m_MagnitudeList[DataLineIR.m_MagnitudeList.Count - 1], m_strUnit);
                    //string strMax = string.Format("IR AVG : {0}Ω", DataLineIR.m_MagnitudeList.Average().ToString("F3"));
                    
                    g.DrawString(strDara, FontCurrentData, textBrush, new PointF((textWidth * 2), this.Height - (textHeight * 1)));
                    //g.DrawString(strMax, FontCurrentData, textBrush, new PointF((textWidth * 3), this.Height - (textHeight * 1)));

                    textBrush.Dispose();
                }
            }
        }

        protected void DrawLimitLine(ref Graphics g)
        {
            float fHighAlarmPower = 0.0F;
            float fLowAlarmPower = 0.0F;
            float fHighWarningPower = 0.0F;
            float fLowWarningPower = 0.0F;           
            
            SolidBrush AlarmBrush = new SolidBrush(m_PowerGraphConfig.AlarmLineColor);
            SolidBrush WarningBrush = new SolidBrush(m_PowerGraphConfig.WarningLineColor);
           
            float fZoomGap = ((MaxValue - MinValue) * (m_fZoomScale - 1.0F)) / 2.0F;

            float fMaxline = MaxValue - fZoomGap;
            float fMinline = MinValue + fZoomGap;

            Brush BrushSpecIn = new SolidBrush(m_PowerGraphConfig.SpecInColor);
            Brush BrushSpecOut = new SolidBrush(m_PowerGraphConfig.SpecOutColor);
           
            int posY = Height - (int)(((fHighAlarmPower - fZoomGap - MinValue) / (fMaxline - fMinline)) * Height);
            PointF pHighAlarmStart = new PointF(0, posY);
            PointF pHighAlarmEnd = new PointF(this.Width, posY);

            g.DrawLine(new Pen(AlarmBrush), pHighAlarmStart, pHighAlarmEnd);

            posY = Height - (int)(((fHighWarningPower - fZoomGap - MinValue) / (fMaxline - fMinline)) * Height);
            PointF pHighWarningStart = new PointF(0, posY);
            PointF pHighWarningEnd = new PointF(this.Width, posY);

            g.DrawLine(new Pen(WarningBrush), pHighWarningStart, pHighWarningEnd);

            posY = Height - (int)(((fLowWarningPower - fZoomGap - MinValue) / (fMaxline - fMinline)) * Height);
            PointF pLowWarningStart = new PointF(0, posY);
            PointF pLowWarningEnd = new PointF(this.Width, posY);

            g.DrawLine(new Pen(WarningBrush), pLowWarningStart, pLowWarningEnd);

            posY = Height - (int)(((fLowAlarmPower - fZoomGap - MinValue) / (fMaxline - fMinline)) * Height);
            PointF pLowAlarmStart = new PointF(0, posY);
            PointF pLowAlarmEnd = new PointF(this.Width, posY);

            g.DrawLine(new Pen(AlarmBrush), pLowAlarmStart, pLowAlarmEnd);

            Color ColorAlarmArea = m_PowerGraphConfig.AlarmLineColor;
            ColorAlarmArea = Color.FromArgb(50, ColorAlarmArea.R, ColorAlarmArea.G, ColorAlarmArea.B);
            Color ColorWarningArea = m_PowerGraphConfig.WarningLineColor;
            ColorWarningArea = Color.FromArgb(50, ColorWarningArea.R, ColorWarningArea.G, ColorWarningArea.B);
                        
            Rectangle rectHighAlarm = new Rectangle(0, 0, this.Width, (int)(pHighAlarmStart.Y));            
            g.FillRectangle(new SolidBrush(ColorAlarmArea), rectHighAlarm);

            Rectangle rectHighWarning = new Rectangle(0, (int)pHighAlarmStart.Y, this.Width, (int)(pHighWarningStart.Y - pHighAlarmStart.Y));            
            g.FillRectangle(new SolidBrush(ColorWarningArea), rectHighWarning);

            Rectangle rectLowWarning = new Rectangle(0, (int)pLowWarningStart.Y, this.Width, (int)(pLowAlarmStart.Y - pLowWarningStart.Y));            
            g.FillRectangle(new SolidBrush(ColorWarningArea), rectLowWarning);

            Rectangle rectLowAlarm = new Rectangle(0, (int)pLowAlarmStart.Y, this.Width, (int)(pLowAlarmStart.Y + pLowWarningStart.Y));
            g.FillRectangle(new SolidBrush(ColorAlarmArea), rectLowAlarm);

            AlarmBrush.Dispose();
            WarningBrush.Dispose();
        }
        protected void RefreshLabels()
        {
            if (!m_bMinLabelSet)
            {
                m_MinLabel = m_MinPeek.ToString();
            }

            if (!m_bMaxLabelSet)
            {
                m_MaxLabel = m_MaxPeek.ToString();
            }
        }

        protected void DrawGrid(ref Graphics g)
        {
            Pen gridPen = new Pen(m_GridColor, 1);        

            Rectangle rect = new Rectangle(0 + 1, 0 + 1, Width - 2, Height - 2);

            for (int n = Height - 1; n >= 0; n -= m_GridSize)
            {
                g.DrawLine(gridPen, m_OffsetX, n, Width, n);
            }

            for (int n = m_OffsetX + m_MoveOffset; n < Width; n += m_GridSize)
            {
                if (n < m_OffsetX)
                {
                    continue;
                }

                g.DrawLine(gridPen, n, 0, n, Height);
            }

            g.DrawRectangle(gridPen, rect);
            gridPen.Dispose();
        }

        private void CullAndEqualizeMagnitudeCounts()
        {
            if (m_MaxCoords == -1)
            {
                CalculateMaxPushPoints();
            }

            int greatestMCount = 0;
            foreach (Line line in m_Lines)
            {
                if (greatestMCount < line.m_MagnitudeList.Count)
                {
                    greatestMCount = line.m_MagnitudeList.Count;
                }
            }

            if (greatestMCount == 0)
            {
                return; 
            }

            foreach (Line line in m_Lines)
            {
                if (line.m_MagnitudeList.Count == 0)
                {
                    line.m_MagnitudeList.Add(m_MinPeek);
                }

                while (line.m_MagnitudeList.Count < greatestMCount)
                {
                    line.m_MagnitudeList.Add(
                        line.m_MagnitudeList[line.m_MagnitudeList.Count - 1]);
                }

                try
                {
                    int cullsRequired = (line.m_MagnitudeList.Count - m_MaxCoords * DulplexPointCount) + 2;
                    if (cullsRequired > 0)
                    {
                        line.m_MagnitudeList.RemoveRange(0, cullsRequired);
                        line.m_LowLimitList.RemoveRange(0, cullsRequired);
                        line.m_HighLimitList.RemoveRange(0, cullsRequired);
                    }
                }
                catch(SystemException Desc)
                {

                }
                
            }
        }


        public int OffsetY = 0;
        protected void DrawLines(ref Graphics g)
        {
            float fZoomGap = ((MaxValue - MinValue) * (m_fZoomScale - 1.0F)) / 2.0F;

            float fMaxline = MaxValue - fZoomGap;
            float fMinline = MinValue + fZoomGap;
            int nMaxCount = m_MaxCoords;

            foreach (Line line in m_Lines)
            {
                if (line.m_MagnitudeList.Count == 0)
                {
                    return;
                }

                if (!line.m_bVisible)
                {
                    continue;
                }
                    

                Pen linePen = new Pen(line.m_Color, line.m_Thickness);
                Pen lineHighPen = new Pen(Color.Orange, line.m_Thickness);
                Pen lineLowPen = new Pen(Color.Orange, line.m_Thickness);

                Point lastPoint = new Point();
                    lastPoint.X = m_OffsetX;
                    lastPoint.Y = (int)(Height - ((line.m_MagnitudeList[0] * Height) / (m_MaxPeek - m_MinPeek)));


                bool bFirst = true;
                int nIndex = 0;
                for (int n = 0; n < line.m_MagnitudeList.Count; ++n)
                {
                    nIndex = n / DulplexPointCount;

                    if (line.m_bShowAsBar)
                    {
                        Rectangle barRect = new Rectangle();

                        Point p = barRect.Location;
                        p.X = m_OffsetX + (nIndex * m_LineInterval) + 1;
                        p.Y = Height - (int)((line.m_MagnitudeList[n] * Height) / (m_MaxPeek - m_MinPeek));
                        barRect.Location = p;

                        barRect.Width = m_LineInterval - 1;
                        barRect.Height = Height;

                        DrawBar(barRect, line, ref g);
                    }
                    else
                    {
                        try
                        {
                            int newX = m_OffsetX + (nIndex * m_LineInterval);

                            if (m_PowerGraphConfig.IsReverse)
                            {
                                newX = m_OffsetX + (nMaxCount - nIndex) * m_LineInterval;
                            }

                            int newY = Height - (int)(((line.m_MagnitudeList[n] - fZoomGap - MinValue - OffsetY) / (fMaxline - fMinline)) * Height);

                            if (bFirst)
                            {
                                lastPoint.X = newX;
                                lastPoint.Y = newY;
                            }

                            linePen = new Pen(line.m_Color, line.m_Thickness);
                            g.DrawLine(linePen, lastPoint.X, lastPoint.Y, newX, newY);

                            lastPoint.X = newX;
                            lastPoint.Y = newY;

                            if (bFirst)
                            {
                                bFirst = false;
                            }
                        }
                        catch(Exception Desc)
                        {

                        }
                    }
                }
                
                int CursorPosX = m_OffsetX + ((m_CurrPos/DulplexPointCount) * m_LineInterval);
                linePen.Color = Color.Red;

                if (m_PowerGraphConfig.IsReverse)
                {
                    CursorPosX = m_OffsetX + (nMaxCount - (m_CurrPos / DulplexPointCount)) * m_LineInterval;
                }

                g.DrawLine(linePen, CursorPosX, 0, CursorPosX, this.Height);

                linePen.Dispose();
            }
        }

        private void DrawBar(Rectangle rect, Line line, ref Graphics g)
        {
            SolidBrush barBrush = new SolidBrush(line.m_Color);
            g.FillRectangle(barBrush, rect);
            barBrush.Dispose();
        }
        
        public LineHandle GetLineHandle(int numID)
        {
            Object line = (Object)GetLine(numID);
            return (line != null ? new LineHandle(ref line, this) : null);
        }

        public LineHandle GetLineHandle(string nameID)
        {
            Object line = (Object)GetLine(nameID);
            return (line != null ? new LineHandle(ref line, this) : null);
        }

        public int GetLineCount(int numID)
        {
            Line pLine = GetLine(numID);

            return pLine.m_MagnitudeList.Count;
        }

        private Line GetLine(int numID)
        {
            foreach (Line line in m_Lines)
            {
                if (numID == line.m_NumID)
                {
                    return line;
                }
            }
            return null;
        }

        private Line GetLine(string nameID)
        {
            foreach (Line line in m_Lines)
            {
                if (string.Compare(nameID, line.m_NameID, true) == 0)
                {
                    return line;
                }
            }
            return null;
        }

        public bool LineExists(int numID)
        {
            return GetLine(numID) != null;
        }

        public bool LineExists(string nameID)
        {
            return GetLine(nameID) != null;
        }

        public LineHandle AddLine(string nameID, Color clr)
        {
            if (LineExists(nameID))
            {
                return null;
            }

            Line line = new Line(nameID);
            line.m_Color = clr;

            m_Lines.Add(line);

            Object objLine = (Object)line;
            return (new LineHandle(ref objLine, this));
        }

        public LineHandle AddLine(int numID, Color clr)
        {
            if (LineExists(numID))
            {
                return null;
            }

            Line line = new Line(numID);
            line.m_Color = clr;

            m_Lines.Add(line);
            Object objLine = (Object)line;
            return (new LineHandle(ref objLine, this));
        }

        public LineHandle AddLine(int numID, string strName, Color clr)
        {
            if (LineExists(numID))
            {
                return null;
            }

            Line line = new Line(numID);
            line.m_NameID = strName;
            line.m_Color = clr;

            m_Lines.Add(line);
            Object objLine = (Object)line;
            return (new LineHandle(ref objLine, this));
        }

        public bool RemoveLine(string nameID)
        {
            Line line = GetLine(nameID);
            if (line == null)
            {
                return false;
            }

            return m_Lines.Remove(line);
        }

        public bool RemoveLine(int numID)
        {
            Line line = GetLine(numID);
            if (line == null)
            {
                return false;
            }

            return m_Lines.Remove(line);
        }

        public bool Push(double magnitude, int nameID)
        {
            Line line = GetLine(nameID);
            if (line == null)
            {
                return false;
            }

            return PushDirect(magnitude, line); 
        }


        public void ClearLine(int numID)
        {
            Line line = GetLine(numID);

            if(line != null)
            {
                line.m_MagnitudeList.Clear();
                line.m_LowLimitList.Clear();
                line.m_HighLimitList.Clear();
                m_CurrPos = 0;
            }
            else
            {
                //if(numID == 1)
                //{
                //    Logger.Add("Can't clear the line, AVERAGE_POWER_LINE is null");
                //}
                //else if(numID == 2)
                //{
                //    Logger.Add("Can't clear the line, PEAK_POWER_LINE is null");
                //}
            }
        }
            
        private bool PushDirect(double magnitude, Line line)
        {
            if (!m_bAutoScale && magnitude > m_MaxPeek)
            {
                //magnitude = m_MaxPeek;
            }
            else if (m_bAutoScale && magnitude > m_MaxPeek)
            {
                //m_MaxPeek = magnitude;
                RefreshLabels();
            }
            else if (!m_bAutoScale && magnitude < m_MinPeek)
            {
                //magnitude = m_MinPeek;
            }
            else if (m_bAutoScale && magnitude < m_MinPeek)
            {
                //m_MinPeek = magnitude;
                RefreshLabels();
            }

            if(magnitude > line.m_dMax)
            {
                line.m_dMax = magnitude;
            }

            if (magnitude < line.m_dMin)
            {
                line.m_dMin = magnitude;
            }

            magnitude -= m_MinPeek;

            if(line.m_MagnitudeList.Count > m_PowerGraphConfig.ListMaxCount)
            {
            }

            line.m_MagnitudeList.Add(magnitude);

            m_CurrPos = line.m_MagnitudeList.Count - 1;
            return true;
        }

        #region File Manager              
        private string m_XMLName = "Graph";
        public bool ReadInitFile()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + Index.ToString() + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        ReadInitFileFromXML(xmlReader);
                    }
                    catch (Exception e)
                    {
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    WriteInitFile();
                    return false;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            return true;
        }

        public bool WriteInitFile()
        {
            string strPath = Application.StartupPath + "\\" + m_XMLName + Index.ToString() + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                WriteInitFileToXML(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool ReadInitFileFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "MinValue":
                            if (!xmlReader.Read()) return false;
                            MinValue = float.Parse(xmlReader.Value);
                            break;
                        case "MaxValue":
                            if (!xmlReader.Read()) return false;
                            MaxValue = float.Parse(xmlReader.Value);
                            break;
                        case "Desc":
                            if (!xmlReader.Read()) return false;
                            Desc = xmlReader.Value;
                            break;
                        case "Unit":
                            if (!xmlReader.Read()) return false;
                            Unit = xmlReader.Value;
                            break;
                    }
                }
                else
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name == m_XMLName) break;
                    }
                }
            }


            Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool WriteInitFileToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("MainConfig");
            xmlWriter.WriteElementString("MinValue", MinValue.ToString("F2"));
            xmlWriter.WriteElementString("MaxValue", MaxValue.ToString("F2"));
            xmlWriter.WriteElementString("Desc", Desc);
            xmlWriter.WriteElementString("Unit", Unit);
            xmlWriter.WriteEndElement();
            return true;
        }
        #endregion
    }

    public class PowerGraphConfig
    {
        private int m_nSpecOutPeakCount = 0;
        public int SpecOutPeakCount
        {
            get
            {
                return m_nSpecOutPeakCount;
            }
            set
            {
                m_nSpecOutPeakCount = value;
            }
        }

        private int m_nSpecOutAverCount = 0;
        public int SpecOutAverCount
        {
            get
            {
                return m_nSpecOutAverCount;
            }
            set
            {
                m_nSpecOutAverCount = value;
            }
        }

        private Color m_TextColor = Color.Aquamarine;
        public Color TextColor
        {
            get
            {
                return m_TextColor;
            }
            set
            {
                m_TextColor = value;
            }
        }

        private Color m_CurrentDataColor = Color.Blue;
        public Color CurrentDataColor
        {
            get
            {
                return m_CurrentDataColor;
            }
            set
            {
                m_CurrentDataColor = value;
            }
        }

        private Color m_BackgroundColor = Color.Black;
        public Color BackgroundColor
        {
            get
            {
                return m_BackgroundColor;
            }
            set
            {
                m_BackgroundColor = value;
            }
        }

        private Color m_GridColor = Color.Black;
        public Color GridColor
        {
            get
            {
                return m_GridColor;
            }
            set
            {
                m_GridColor = value;
            }
        }

        private Color m_SpecInColor = Color.Blue;
        public Color SpecInColor
        {
            get
            {
                return m_SpecInColor;
            }
            set
            {
                m_SpecInColor = value;
            }
        }

        private Color m_SpecOutColor = Color.Red;
        public Color SpecOutColor
        {
            get
            {
                return m_SpecOutColor;
            }
            set
            {
                m_SpecOutColor = value;
            }
        }

        private Color m_WarningLineColor = Color.LightPink;
        public Color WarningLineColor
        {
            get
            {
                return m_WarningLineColor;
            }
            set
            {
                m_WarningLineColor = value;
            }
        }

        private Color m_AlarmLineColor = Color.Red;
        public Color AlarmLineColor
        {
            get
            {
                return m_AlarmLineColor;
            }
            set
            {
                m_AlarmLineColor = value;
            }
        }

        private Color m_BorderLineColor = Color.White;
        public Color BorderLineColor
        {
            get
            {
                return m_BorderLineColor;
            }
            set
            {
                m_BorderLineColor = value;
            }
        }

        private int m_nPointSize = 5;
        public int PointSize
        {
            get
            {
                return m_nPointSize;
            }
            set
            {
                m_nPointSize = value;
            }
        }

        private int m_nGapX = 1;
        public int GapX
        {
            get
            {
                return m_nGapX;
            }
            set
            {
                m_nGapX = value;
            }
        }

        private int m_nGapY = 1;
        public int GapY
        {
            get
            {
                return m_nGapY;
            }
            set
            {
                m_nGapY = value;
            }
        }

        private int m_nMax = 1000;
        public int Max
        {
            get
            {
                return m_nMax;
            }
            set
            {
                m_nMax = value;
            }
        }

        private int m_nMin = 0;
        public int Min
        {
            get
            {
                return m_nMin;
            }
            set
            {
                m_nMin = value;
            }
        }

        private bool m_bIsViewModeLine = true;
        public bool IsViewModeLine
        {
            get
            {
                return m_bIsViewModeLine;
            }
            set
            {
                m_bIsViewModeLine = value;
            }
        }

        private bool m_bIsLimitPeak = true;
        public bool IsLimitPeak
        {
            get
            {
                return m_bIsLimitPeak;
            }
            set
            {
                m_bIsLimitPeak = value;
            }
        }

        private int m_nFontSize = 12;
        public int FontSize
        {
            get
            {
                return m_nFontSize;
            }
            set
            {
                m_nFontSize = value;
            }
        }

        private int m_nLeftFontSize = 10;
        public int LeftFontSize
        {
            get
            {
                return m_nLeftFontSize;
            }
            set
            {
                m_nLeftFontSize = value;
            }
        }

        private int m_nListMaxCount = 20000;
        public int ListMaxCount
        {
            get
            {
                return m_nListMaxCount;
            }
            set
            {
                m_nListMaxCount = value;
            }
        }

        private bool m_bReverse = false;
        public bool IsReverse
        {
            get
            {
                return m_bReverse;
            }
            set
            {
                m_bReverse = value;
            }
        }

        public PowerGraphConfig()
        {

        }

        private string m_strPath = Application.StartupPath + "PowerGraphConfig.cfg";
        private string m_XMLName = "PowerGraphConfig";
        public bool ReadInitFile()
        {
            try
            {
                if (File.Exists(m_strPath))   //  xml 파일 존재 유무 검사
                {
                    XmlTextReader xmlReader = new XmlTextReader(m_strPath);    //  xml 파일 열기

                    try
                    {
                        ReadInitFileFromXML(xmlReader);
                    }
                    catch (Exception e)
                    {
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    Logger.WriteLog(LOG.AbNormal, "File is't exist => " + m_strPath);                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                return false;
            }
            return true;
        }

        public bool WriteInitFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(m_strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                WriteInitFileToXML(xmlWriter);
                xmlWriter.WriteEndDocument();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }
            return true;
        }

        public bool ReadInitFileFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {                      
                        case "TextColor":
                            if (!xmlReader.Read()) return false;
                            string[] strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                TextColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "CurrentDataColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                CurrentDataColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "BackgroundColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                BackgroundColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "GridColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                GridColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "SpecInColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                SpecInColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "SpecOutColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                SpecOutColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "WarningLineColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                WarningLineColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "AlarmLineColor":
                            if (!xmlReader.Read()) return false;
                            strColor = xmlReader.Value.Split(',');
                            if (strColor.Length == 3)
                                AlarmLineColor = Color.FromArgb(int.Parse(strColor[0]), int.Parse(strColor[1]), int.Parse(strColor[2]));
                            break;
                        case "PointSize":
                            if (!xmlReader.Read()) return false;
                            PointSize = int.Parse(xmlReader.Value);
                            break;
                        case "GapX":
                            if (!xmlReader.Read()) return false;
                            GapX = int.Parse(xmlReader.Value);
                            break;
                        case "GapY":
                            if (!xmlReader.Read()) return false;
                            GapY = int.Parse(xmlReader.Value);
                            break;
                        case "IsViewModeLine":
                            if (!xmlReader.Read()) return false;
                            IsViewModeLine = bool.Parse(xmlReader.Value);
                            break;
                        case "IsLimitPeak":
                            if (!xmlReader.Read()) return false;
                            IsLimitPeak = bool.Parse(xmlReader.Value);
                            break;
                        case "FontSize":
                            if (!xmlReader.Read()) return false;
                            FontSize = int.Parse(xmlReader.Value);
                            break;
                        case "LeftFontSize":
                            if (!xmlReader.Read()) return false;
                            LeftFontSize = int.Parse(xmlReader.Value);
                            break;
                        case "ListMaxCount":
                            if (!xmlReader.Read()) return false;
                            ListMaxCount = int.Parse(xmlReader.Value);
                            break;
                        case "Max":
                            if (!xmlReader.Read()) return false;
                            Max = int.Parse(xmlReader.Value);
                            break;
                        case "Min":
                            if (!xmlReader.Read()) return false;
                            Min = int.Parse(xmlReader.Value);
                            break;
                    }
                }
                else
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.Name == m_XMLName) break;
                    }
                }
            }
            return true;
        }

        public bool WriteInitFileToXML(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(m_XMLName);
            xmlWriter.WriteElementString("TextColor", ColorToString(TextColor));
            xmlWriter.WriteElementString("CurrentDataColor", ColorToString(CurrentDataColor));
            xmlWriter.WriteElementString("BackgroundColor", ColorToString(BackgroundColor));
            xmlWriter.WriteElementString("GridColor", ColorToString(GridColor));
            xmlWriter.WriteElementString("SpecInColor", ColorToString(SpecInColor));
            xmlWriter.WriteElementString("SpecOutColor", ColorToString(SpecOutColor));
            xmlWriter.WriteElementString("WarningLineColor", ColorToString(WarningLineColor));
            xmlWriter.WriteElementString("AlarmLineColor", ColorToString(AlarmLineColor));
            xmlWriter.WriteElementString("GapX", GapX.ToString());
            xmlWriter.WriteElementString("GapY", GapY.ToString());
            xmlWriter.WriteElementString("IsViewModeLine", IsViewModeLine.ToString());
            xmlWriter.WriteElementString("IsLimitPeak", IsLimitPeak.ToString());
            xmlWriter.WriteElementString("FontSize", FontSize.ToString());
            xmlWriter.WriteElementString("LeftFontSize", LeftFontSize.ToString());
            xmlWriter.WriteElementString("ListMaxCount", ListMaxCount.ToString());
            xmlWriter.WriteElementString("Max", Max.ToString());
            xmlWriter.WriteElementString("Min", Min.ToString());

            xmlWriter.WriteEndElement();
            return true;
        }
        private string ColorToString(Color cr)
        {
            return string.Format("{0},{1},{2}", cr.R, cr.G, cr.B);
        }
    }
}

