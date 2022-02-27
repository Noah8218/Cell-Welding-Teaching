using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Reflection;

namespace IntelligentFactory
{
    public enum LOG
    {
        Normal = 0,
        AbNormal,
        Comm,     
        IO,
        Thread,
        Inspection,
        Alert
    }

    public class LogItem
    {
        private LOG m_etype;
        public LOG Type
        {
            get
            {
                return m_etype;
            }

            set
            {
                m_etype = value;
            }
        }

        private string m_strLog;
        public string StrLog
        {
            get
            {
                return m_strLog;
            }

            set
            {
                m_strLog = value;
            }
        }

        private bool m_bDisplay;
        public bool IsDisplay
        {
            get
            {
                return m_bDisplay;
            }

            set
            {
                m_bDisplay = value;
            }
        }

        public LogItem(LOG etype, string strLog)
        {
            this.m_etype = etype;
            this.m_strLog = strLog;
            this.m_bDisplay = true;
        }
    }

    public static class Logger
    {   
        private const int MAX_TRY_WRITE_LOG_FILE = 10;
        private static List<string> m_lstFilebuffer = new List<string>();
        private static List<LogItem> m_lstLogbuffer = new List<LogItem>();        

        private static DateTime timestampOld;
        public static bool m_bStartLog = true;
        private static string m_strLogPath;// = @"D:\";
        private static string m_strAppName;

        public delegate void LogEvent(LogItem item);
        public static event LogEvent EventLog;

        public static Color GetColor(LOG type)
        {        
            switch (type)
            {
                case LOG.Normal :
                    return Color.White;
                case LOG.IO:
                    return Color.Lime;
                case LOG.AbNormal:
                    return Color.Red;
                case LOG.Comm:
                    return Color.Blue;                
                case LOG.Thread:
                    return Color.Silver;
                case LOG.Inspection:
                    return Color.Yellow;
                case LOG.Alert:
                    return Color.Orange;
            }

            return Color.Black;
        }

        public static void SetPath(string strPath)
        {
            try
            {
                string strDir = strPath + "\\Log";
                if (Directory.Exists(strDir) == false)
                {
                    Directory.CreateDirectory(strPath + "\\Log");
                }

                m_strLogPath = strDir;
                WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception Desc)
            {
                WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }

        public static string GetPath()
        {
            return m_strLogPath;
        }

        public static void SetAppName(string strAppName)
        {
            m_strAppName = strAppName;
        }

        public static string GetAppName()
        {
            return m_strAppName;
        }
        
        public static void AddEvent( LogEvent newMethod )
        {
            EventLog += newMethod;
        }

        static private object lockObject = new object();
        
        public static string LogStrToFile(LOG Type, string strLog)
        {
            string str = "";
            lock (lockObject)
            {
                strLog = strLog.TrimEnd('\0');

                DateTime timestampNew = DateTime.Now;
                if (Logger.m_bStartLog == true)
                {
                    Logger.timestampOld = timestampNew;
                    Logger.m_bStartLog = false;
                }

                TimeSpan timeSpanDelay;
                timeSpanDelay = timestampNew - Logger.timestampOld;

                int nDelay = (int)timeSpanDelay.TotalMilliseconds;

                string strLogType = string.Format("{0,-10} ", "[" + Type.ToString().ToUpper() + "]");
                string strDelay = string.Format("{0,6}", nDelay);

                str = timestampNew.ToString("yy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " [" + strDelay + "]  " + strLog;
                Debug.WriteLine(str);

                if (GetPath() != "")
                {
                    string filename = GetPath() + '\\' + timestampNew.ToString("yyMMdd") + (m_strAppName != null ? "_" : "") + m_strAppName + ".log";

                    if (m_lstFilebuffer.Count > 0)
                    {
                        foreach (string strItem in m_lstFilebuffer)
                        {
                            try
                            {
                                WriteFileLog(filename, strItem);
                            }
                            catch (Exception e)
                            {
                                WriteLog("Log File Write Fail : " + e.Message);
                            }
                        }
                        m_lstFilebuffer.Clear();
                        try
                        {
                            WriteFileLog(filename, strLogType + str);
                        }
                        catch (Exception e)
                        {
                            WriteLog("Log File Write Fail : " + e.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            WriteFileLog(filename, strLogType + str);
                        }
                        catch (Exception e)
                        {
                            WriteLog("Log File Write Fail : " + e.Message);
                        }
                    }
                }
                else
                {
                    m_lstFilebuffer.Add(strLogType + str);
                }

                Logger.timestampOld = timestampNew;

                if (str.Length > 3)
                {
                    str = str.Remove(0, 3);
                }

            }
            return str;
        }

        private static void WriteFileLog(string strFileName, string strLog)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(strFileName))
                    {
                        writer.WriteLine(strLog);
                    }
                    break;
                }
                catch (IOException)
                {                    
                    Thread.Sleep(10);
                    i++;
                    if (i >= MAX_TRY_WRITE_LOG_FILE )
                    {
                        throw new IOException("Log file \"" + strFileName + "\" not accessible after 5 tries");
                    }
                }
            }            
        }
    
        public static bool WriteLog(string format, params object[] args)
        {
            string strLog = string.Format(format, args);
            return WriteLog(LOG.Normal, strLog);
        }

        public static bool WriteLog(LOG type, string format, params object[] args)
        {          
            string strLog = "";
            try
            {
                strLog = string.Format(format, args);
            }
            catch (Exception Desc)
            {
                strLog = format;
            }

            string str = LogStrToFile(type, strLog);

            LogItem item = new LogItem(type, str);

            if (EventLog != null)
            {
                if (m_lstLogbuffer.Count > 0)
                {
                    /*
                    foreach (LogItem Log in m_lstLogbuffer)
                    {
                        EventLog(Log);
                    }
                     */
                    for (int i = 0; i < m_lstLogbuffer.Count; i++)
                    {
                        EventLog(m_lstLogbuffer[i]);
                    }
                    m_lstLogbuffer.Clear();
                    EventLog(item);
                }
                else
                {
                    EventLog(item);
                }
            }
            else
            {
                m_lstLogbuffer.Add(item);                
            }
         
            return true;            
        }

        public static void LoggerFileDelete(int FileCount)
        {
            DirectoryInfo dinfo = new DirectoryInfo(GetPath());
            if (!dinfo.Exists) dinfo.Create();

            if (FileCount < dinfo.GetFiles().Length)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo f in dinfo.GetFiles())
                {
                    if (f.Extension == ".log") files.Add(f);
                }

                files.Sort(new CompareFileInfoEntries());

                for (int i = 0; i <= files.Count - FileCount; i++)
                {
                    File.Delete(dinfo.FullName +"\\"+ files[i].Name);
                    WriteLog("Delete File ==> " + dinfo.FullName + "\\" + files[i].Name);
                }
            }
        }

        public static void LoggerFileDelete(string strDir, string strExt, int FileCount)
        {
            DirectoryInfo dinfo = new DirectoryInfo(strDir);
            if (!dinfo.Exists) dinfo.Create();

            if (FileCount < dinfo.GetFiles().Length)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo f in dinfo.GetFiles())
                {
                    if (f.Extension == strExt) files.Add(f);
                }

                files.Sort(new CompareFileInfoEntries());

                for (int i = 0; i <= files.Count - FileCount; i++)
                {
                    File.Delete(dinfo.FullName + "\\" + files[i].Name);
                    WriteLog("Delete File ==> " + dinfo.FullName + "\\" + files[i].Name);
                }
            }
        }   
    }

    public class CompareFileInfoEntries : IComparer<FileInfo>
    {
        public int Compare(FileInfo f1, FileInfo f2)
        {
            return (DateTime.Compare(f1.CreationTime, f2.CreationTime));
        }
    }
}
