using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace IntelligentFactory
{
    public class MessageEventArgs : EventArgs
    {
        public enum MESSAGEBOX_TYPE { OKCANCEL, OK };

        private string m_strHead = "";
        public string Head
        {
            get => m_strHead;
            set => m_strHead = value;
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public MessageEventArgs(string strMessage, string strHead)
        {            
            m_strMessage = strMessage;
            m_strHead = strHead;
        }

        public MessageEventArgs()
        {

        }
    }

    public class StringEventArgs : EventArgs
    {
        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public StringEventArgs(string strMessage)
        {
            m_strMessage = strMessage;
        }

        public StringEventArgs()
        {

        }
    }

    public class InspResultArgs : EventArgs
    {
#if MATROX
        public Matrox.MatroxImagingLibrary.MIL_ID ImageResult;
#endif 
        public int Index = 0;

        public string ResultData = "";
    }

    public class CVRectEventArgs : EventArgs
    {
        private Rect m_rt = new Rect();
        public Rect rt
        {
            get { return m_rt; }
            set { m_rt = value; }
        }



        public CVRectEventArgs(Rect rt)
        {
            m_rt = rt;
        }
    }

    public class RectEventArgs : EventArgs
    {
        public string Mode = "";
        public System.Drawing.Rectangle Rect = new System.Drawing.Rectangle();

        public RectEventArgs(System.Drawing.Rectangle rt, string strMode)
        {
            Rect = rt;
            Mode = strMode;
        }
    }

    public class GrabEventArgs : EventArgs
    {
        public Mat ImageGrab = new Mat();

        public int m_Index;

        public GrabEventArgs(Mat ImageArgs, int nIndex)
        {

            ImageGrab = ImageArgs.Clone();
            m_Index = nIndex;
        }
    }

    //public class LogEventArgs : EventArgs
    //{
    //    private ILog m_iLog;
    //    public ILog Log
    //    {
    //        get { return m_iLog; }
    //        set { m_iLog = value; }
    //    }

    //    public LogEventArgs(ILog iLog)
    //    {
    //        Log = iLog;
    //    }
    //}
}
