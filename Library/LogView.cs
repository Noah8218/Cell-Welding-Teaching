using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IntelligentFactory
{
    public partial class LogView : UserControl
    {
        private const int MAX_LOG_LINES = 500;
        private const int WM_VSCROLL = 0x115;
        private const int SB_BOTTOM = 7;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private List<LogItem> m_lstLog = new List<LogItem>();

        public LogView()
        {
            InitializeComponent();            
        }

        Semaphore m_SemLockLog = new Semaphore(1, 1);

        private void timerDisplayLog_Tick(object sender, EventArgs e)
        {        
            try
            {
                m_SemLockLog.WaitOne();
                timerDisplayLog.Enabled = false;

                if (richTextBoxExLog.Lines.Length > MAX_LOG_LINES)
                {
                    richTextBoxExLog.ReadOnly = false;
                    richTextBoxExLog.Select(0, richTextBoxExLog.GetFirstCharIndexFromLine(richTextBoxExLog.Lines.Length - MAX_LOG_LINES));
                    richTextBoxExLog.SelectedText = "";
                    richTextBoxExLog.ReadOnly = true;                
                }

                foreach (LogItem item in m_lstLog)
                {
                    if (item.IsDisplay)
                    {                        
                        this.richTextBoxExLog.SelectionStart = this.richTextBoxExLog.TextLength;
                        this.richTextBoxExLog.SelectionLength = 0;
                        this.richTextBoxExLog.SelectionColor = Logger.GetColor(item.Type);
                        this.richTextBoxExLog.AppendText(item.StrLog + "\r\n");
                        this.richTextBoxExLog.SelectionColor = this.richTextBoxExLog.ForeColor;                        
                        item.IsDisplay = false;
                    }
                }                
                int nRet = SendMessage(richTextBoxExLog.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);
                m_SemLockLog.Release();
            }
            catch (Exception Desc)
            {
                Debug.WriteLine("Log Error : " + Desc.Message);
            }           
            
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {            
            Logger.AddEvent(DisplayLog);   
        }
       
        private void DisplayLog(LogItem item)
        {
            m_SemLockLog.WaitOne();
            bool bClear = false;
            bool bLock = false;
            try
            {
                Monitor.Enter(m_lstLog, ref bLock);
                m_lstLog.Add(item);
                while (m_lstLog.Count > MAX_LOG_LINES)
                {
                    bClear = true;
                    m_lstLog.RemoveAt(0);                    
                }
            }
            finally
            {
                if (bLock)
                {
                    Monitor.Exit(m_lstLog);
                    GC.Collect();
                }
            }

            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {                       
                        this.timerDisplayLog.Enabled = true;
                    }));
                }
                catch(Exception Desc)
                {
                    Debug.WriteLine(Desc.Message);
                }
            }
            else
            {
                this.timerDisplayLog.Enabled = true;
            }
            m_SemLockLog.Release();
        }
    }
}
