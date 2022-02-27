using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace IntelligentFactory
{
    public class CSignal
    {
        public enum DEV_TYPE { LB, LW };
        private DEV_TYPE m_DevType = DEV_TYPE.LB;
        public DEV_TYPE DevType
        {
            get { return m_DevType; }
            set { m_DevType = value; }
        }
       
        private const short STATION_NO = 255;

        public const int SIGNAL_OFF = 0;
        public const int SIGNAL_ON = 1;
        public const int SIGNAL_NONE = 2;
        public const int MAX_SIGNAL_STATUS = 3;

        private string m_strAddress;
        public string ADDRESS
        {
            get
            {
                return m_strAddress;
            }
            set
            {
                m_strAddress = value;
            }
        }

        private int m_nWordCount = 1;
        public int WordCount
        {
            get { return m_nWordCount; }
            set { m_nWordCount = value; }
        }

        private double m_dCurrentActual;
        public double CurrentActual
        {
            get 
            {
                //Double nValue = Current;
                double dValue = (double)Current * m_dFactor;
                return dValue; 
            }
            set { m_dCurrentActual = value; }
        }

        private string m_strUnit = "";
        public string Unit
        {
            get { return m_strUnit; }
            set { m_strUnit = value; }
        }

        private double m_dFactor = 1.0D;
        public double Factor
        {
            get { return m_dFactor; }
            set { m_dFactor = value; }
        }

        public string DisplayData
        {
            get
            {
                return string.Format("{0} {1}", ((double)Current * m_dFactor).ToString("F2"), m_strUnit);
            }
        }

        private int m_nPrevious;
        private int m_nCurrent;
        public int Current
        {
            get
            {
                return m_nCurrent;
            }

            set
            {
                m_nCurrent = value;
                if (!m_bDisplay)
                    m_bDisplay = (m_nPrevious != m_nCurrent);
                m_nPrevious = m_nCurrent;

                if (m_bDisplay)
                {
                    if(EventUpdateSignal != null)
                    {
                        EventUpdateSignal(this, new EventArgs());
                    }
                }
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

        private string m_strName;

        public string Name
        {
            get
            {
                return m_strName;
            }
        }

        public EventHandler<EventArgs> EventUpdateSignal;

        public CSignal(string strName, string strAddr, DEV_TYPE devType, int nWordCount = 1, string strUnit = "", double dFactor = 1.0D)
        {
            this.m_strName = strName;
            this.m_strAddress = strAddr;
            this.m_DevType = devType;
            this.m_nWordCount = nWordCount;
            this.m_strUnit = strUnit;
            this.m_dFactor = dFactor;

            LoadConfig();
        }

        private string m_XMLName = "PROPERTY_IO";
        public bool LoadConfig()
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\CONFIG\\DEVICE\\IO\\" + m_strName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                switch (xmlReader.Name)
                                {
                                    case "ADDRESS": if (xmlReader.Read()) ADDRESS = xmlReader.Value; break;
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
                    }
                    catch (Exception ex)
                    {
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool SaveConfig()
        {
            CUtil.InitDirectory("CONFIG");
            CUtil.InitDirectory("CONFIG\\DEVICE\\IO");

            string strPath = $"{Application.StartupPath}\\CONFIG\\DEVICE\\IO\\" + m_strName + ".xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("PROPERTY");
                xmlWriter.WriteElementString("ADDRESS", ADDRESS.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception ex)
            {
                //CLogger.Add(LOG.ABNORMAL, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }
    }
}