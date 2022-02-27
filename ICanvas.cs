using System;
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
    public partial class ICanvas : Control
    {
        private string m_strDesc = "EMPTY";
        public string Desc
        {
            get { return m_strDesc; }
            set { m_strDesc = value; }
        }

        private int m_nIndex = 0;
        public int Index
        {
            get { return m_nIndex; }
            set { m_nIndex = value; }
        }

        private string m_strMode = "IDLE";
        public string Mode
        {
            get { return m_strMode; }
            set { m_strMode = value; }
        }

        private bool m_bCancelDraw = false;
        public bool CancelDraw
        {
            set
            {
                m_bCancelDraw = value;

                if(m_bCancelDraw)
                {
                    m_nMouseMode = MOUSEMODE_IDLE;
                    m_ptStart = new Point();
                    m_ptEnd = new Point();
                }
            }
            get
            {
                return m_bCancelDraw;
            }
        }

        private const int MOUSEMODE_NONE = -1;
        private const int MOUSEMODE_IDLE = 0;
        private const int MOUSEMODE_START = 1;
        private const int MOUSEMODE_END = 2;
        private const int MOUSEMODE_MOVE = 3;

        private int m_nMouseMode = MOUSEMODE_IDLE;
        public int MouseMode
        {
            set { m_nMouseMode = value; }
        }

        private Color  m_ColorCrossLine = Color.Red;
        private bool   m_bHighQuality = true;

        private Bitmap m_ImageOriginal = null;

        private float m_fScaleFactorX = 1.0F;
        private float m_fScaleFactorY = 1.0F;

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

        public float m_fPixelPerum = 5.0F;

        private bool m_bMouseFollow = false;
        public bool MouseFollow
        {
            get { return m_bMouseFollow; }
            set { m_bMouseFollow = value; }
        }

        private Point m_ptClickPos = new Point();

        private Point m_ptStart = new Point();
        private Point m_ptEnd = new Point();
        public  PointF ActualPosStart
        {
            get
            {
                PointF ptActual = new PointF(m_ptStart.X, m_ptStart.Y);
                ptActual.X = ptActual.X * m_fScaleFactorX;
                ptActual.Y = ptActual.Y * m_fScaleFactorY;

                return ptActual;
            }
        }

        public PointF ActualPosEnd
        {
            get
            {
                PointF ptActual = new PointF(m_ptEnd.X, m_ptEnd.Y);
                ptActual.X = ptActual.X * m_fScaleFactorX;
                ptActual.Y = ptActual.Y * m_fScaleFactorY;

                return ptActual;
            }
        }

        private Rectangle m_rtDisplay = new Rectangle();
        private Rectangle m_rtDisplayPrev = new Rectangle();

        #region Event Register        
        public event EventHandler<LineEventArgs> EventDrawEnd;
        #endregion

        public ICanvas()
        {                
            InitializeComponent();
            InitializeStyles();
        }

        public ICanvas(int nIndex, string strDesc)
        {
            InitializeComponent();
            InitializeStyles();

            m_nIndex = nIndex;
            m_strDesc = strDesc;
        }

        public ICanvas(Form Parent)
        {
            Parent.Controls.Add(this);

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
            
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            
        }

        public bool InitImage(Bitmap ImageSource)
        {
            m_ImageOriginal = ImageSource;

            m_fScaleFactorX = (float)this.Width / (float)m_ImageOriginal.Width;
            m_fScaleFactorY = (float)this.Height / (float)m_ImageOriginal.Height;

            UpdateDisplay();

            return true;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            m_ptClickPos = new Point(e.X, e.Y);

            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    m_nMouseMode = MOUSEMODE_IDLE;
                    m_ptStart = new Point();
                    m_ptEnd = new Point();
                    return;
                }

                int intX = e.X;
                int intY = e.Y;    
                
                if(Mode == "DRAW")
                {
                    if (m_nMouseMode == MOUSEMODE_IDLE)
                    {
                        CancelDraw = false;

                        int nX = m_rtDisplay.X + (int)(e.X);

                        m_ptStart = new System.Drawing.Point(intX, intY);
                        m_nMouseMode = MOUSEMODE_START;
                    }
                    else if (m_nMouseMode == MOUSEMODE_START)
                    {
                        m_ptEnd = new System.Drawing.Point(intX, intY);
                        m_nMouseMode = MOUSEMODE_END;

                        OpenCvSharp.Point ptStart = new OpenCvSharp.Point((int)(m_ptStart.X), (int)(m_ptStart.Y));
                        OpenCvSharp.Point ptEnd = new OpenCvSharp.Point((int)(m_ptEnd.X), (int)(m_ptEnd.Y));

                        Line line = new Line(ptStart, ptEnd);

                        if (EventDrawEnd != null)
                        {
                            EventDrawEnd(this, new LineEventArgs(line));
                        }
                    }
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                int intX = e.X;
                int intY = e.Y;

                if (Mode == "DRAW")
                {
                    if (m_nMouseMode == MOUSEMODE_START && m_ImageOriginal != null)
                    {
                        m_ptEnd = new Point(intX, intY);

                        Graphics g = this.CreateGraphics();

                        g.DrawLine(new Pen(Color.Red), m_ptStart, m_ptEnd);
                        g.DrawLine(new Pen(Color.Red), new Point(MousePosDraw.X, 0), new Point(MousePosDraw.X, this.Height));
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        m_rtDisplay.X = m_rtDisplay.X + (int)Math.Round((double)(e.X - m_ptClickPos.X) / 2.5D);
                        if (m_rtDisplay.X >= 0) m_rtDisplay.X = 0;
                        if (Math.Abs(m_rtDisplay.X) >= Math.Abs(m_rtDisplay.Width - this.Width)) m_rtDisplay.X = -(m_rtDisplay.Width - this.Width);

                        m_rtDisplay.Y = m_rtDisplay.Y + (int)Math.Round((double)(e.Y - m_ptClickPos.Y) / 2.5D);
                        if (m_rtDisplay.Y >= 0) m_rtDisplay.Y = 0;
                        if (Math.Abs(m_rtDisplay.Y) >= Math.Abs(m_rtDisplay.Height - this.Height)) m_rtDisplay.Y = -(m_rtDisplay.Height - this.Height);
                    }
                }
            }
            catch (Exception Desc)
            {
            }

            //this.Invalidate();
        }

        private void UpdateDisplay()
        {
            m_rtDisplayPrev.Width = m_rtDisplay.Width;
            m_rtDisplayPrev.Height = m_rtDisplay.Height;
            m_rtDisplayPrev.X = m_rtDisplay.X;
            m_rtDisplayPrev.Y = m_rtDisplay.Y;

            //m_rtDisplay.Width = (int)Math.Round(m_ImageOriginal.Width * m_fZoomScale);
            //m_rtDisplay.Height = (int)Math.Round(m_ImageOriginal.Height * m_fZoomScale);
            //m_rtDisplay.X = (int)Math.Round(m_ImageOriginal.Width / 2 - m_ptWheelMousePos.X * m_fZoomScale);
            //m_rtDisplay.Y = (int)Math.Round(m_ImageOriginal.Height / 2 - m_ptWheelMousePos.Y * m_fZoomScale);

            //m_rtDisplay.Width = (int)Math.Round(this.Width * m_fZoomScale);
            //m_rtDisplay.Height = (int)Math.Round(this.Height * m_fZoomScale);
            //m_rtDisplay.X = (int)Math.Round(this.Width / 2 - m_ptWheelMousePos.X * m_fZoomScale);
            //m_rtDisplay.Y = (int)Math.Round(this.Height / 2 - m_ptWheelMousePos.Y * m_fZoomScale);

            if (m_rtDisplay.X > 0) m_rtDisplay.X = 0;
            if (m_rtDisplay.Y > 0) m_rtDisplay.Y = 0;
            if (m_rtDisplay.X + m_rtDisplay.Width < this.Width) m_rtDisplay.X = this.Width - m_rtDisplay.Width;
            if (m_rtDisplay.Y + m_rtDisplay.Height < this.Height) m_rtDisplay.Y = this.Height - m_rtDisplay.Height;

            this.Update();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Refresh();
            base.OnSizeChanged(e); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();

            SmoothingMode prevSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = (m_bHighQuality ? SmoothingMode.HighQuality
                                                : SmoothingMode.Default);

            if(m_ImageOriginal != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;                
                g.DrawImage(CUtil.Resize(m_ImageOriginal, new OpenCvSharp.Size(this.Width, this.Height)), new Rectangle(0, 0, this.Width, this.Height));

                if (MouseFollow)
                {
                    g.DrawLine(new Pen(Color.Red), new Point(0, MousePosDraw.Y), new Point(this.Width, MousePosDraw.Y));
                    g.DrawLine(new Pen(Color.Red), new Point(MousePosDraw.X, 0), new Point(MousePosDraw.X, this.Height));
                }

                this.Focus();
            }

            DrawLabels(ref g);
            DrawLine(ref g);

            g.ResetClip();
            g.SmoothingMode = prevSmoothingMode;
        }


        protected void DrawLabels(ref Graphics g)
        {   

        }

        protected void DrawLine(ref Graphics g)
        {
            if (CancelDraw) return;

            if(Mode == "DRAW")
            {
                if(m_nMouseMode == MOUSEMODE_START || m_nMouseMode == MOUSEMODE_END)
                {
                    Pen PenLine = new Pen(Color.Red, 2);

                    if (m_ptStart.X != 0 && m_ptStart.Y != 0
                         && m_ptEnd.X != 0 && m_ptEnd.Y != 0)
                    {
                        g.DrawLine(PenLine, m_ptStart, m_ptEnd);
                    }

                    PenLine.Dispose();
                }
            }
        }

        #region File Manager              
        private string m_XMLName = "Canvas";
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
            //while (xmlReader.Read())
            //{
            //    if (xmlReader.NodeType == XmlNodeType.Element)
            //    {
            //        switch (xmlReader.Name)
            //        {
            //            case "Unit":
            //                if (!xmlReader.Read()) return false;
            //                Unit = xmlReader.Value;
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        if (xmlReader.NodeType == XmlNodeType.EndElement)
            //        {
            //            if (xmlReader.Name == m_XMLName) break;
            //        }
            //    }
            //}


            Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool WriteInitFileToXML(XmlWriter xmlWriter)
        {
            //xmlWriter.WriteStartElement("MainConfig");
            //xmlWriter.WriteElementString("MinValue", MinValue.ToString("F2"));
            xmlWriter.WriteEndElement();
            return true;
        }
        #endregion

        public Point MousePosActual = new Point();
        public Point MousePosDraw = new Point();
        public EventHandler<MouseEventArgs> EventMouseMove;
        public EventHandler<MouseEventArgs> EventMouseClick;

        private void ICanvas_MouseMove(object sender, MouseEventArgs e)
        {
            MousePosActual.X = (int)(e.X * m_fScaleFactorX);
            MousePosActual.Y = (int)(e.Y * m_fScaleFactorY);

            if(MousePosDraw.X != e.X
                || MousePosDraw.Y != e.Y)
            {
                MousePosDraw.X = e.X;
                MousePosDraw.Y = e.Y;

                OnPaint(null);
            }

            if (EventMouseMove != null)
            {
                EventMouseMove(this, e);
            }
        }

        private void ICanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (EventMouseClick != null)
            {
                EventMouseClick(this, e);
            }
        }
    }

    public class LineEventArgs : EventArgs
    {
        private Line m_Line = null;
        public Line iLine
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        public LineEventArgs(Line line)
        {
            try
            {
                m_Line = new Line(line.Start, line.End);
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return;
            }
        }
    }
}

