using IntelligentFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Concurrent;

using MetroFramework.Controls;

using OpenCvSharp;

namespace IntelligentFactory
{
    public class IData
    {
        public CPropertyCellPosition Property_CellPosition = new CPropertyCellPosition("CELL_POSITION");


#if DPS
        private ILightController_DPS m_LightController = null;
        public ILightController_DPS LightController
        {
            get { return m_LightController; }
            set { m_LightController = value; }
        }
#endif

        #region COUNT
        public int CountOK = 0;
        public int CountNG = 0;
        public int CountTotal = 0;
        #endregion

        public ConcurrentQueue<CGrabBuffer> GrabQueue = new ConcurrentQueue<CGrabBuffer>();

        #region FLAG
        public bool MODE_IO_BOARD = true;
        public bool MODE_BCR_PASS = false;
        public bool MODE_USE_MES  = false;
        public bool MODE_USE_DRY_RUN = false;
                
        
        

        public bool IsCycleStart = false;
        public int  InspIndex = 0;

        public int MAX_CELL_COUNT = 10;

        public bool[] HEAD_USE_INFO = new bool[10] { false, false, false, false, false, false, false, false, false, false};
        public bool[] HEAD_USE_RESULT_DIST = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public bool[] HEAD_USE_RESULT_BCR = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public string[] HEAD_CELL_BCR = new string[10] { "", "", "", "", "", "", "", "", "", "" };
        
        #endregion

        public IData()
        {
#if DPS
            m_LightController = new ILightController_DPS();            
#endif
        }

        public void ResetCount()
        {
            CountOK = 0;
            CountNG = 0;
            CountTotal = 0;
        }

        #region CONFIG BY XML              
        private string m_XMLName = "LastStatus";
        public bool LoadConfig()
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\LastStatus.xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        LoadConfigFromXML(xmlReader);
                    }
                    catch (Exception Desc)
                    {
                        Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
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
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            return true;
        }

        public bool LoadConfigFromXML(XmlReader xmlReader)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    Logger.WriteLog(LOG.Normal, "CONFIG [{0}] ==> {1}", xmlReader.Name, xmlReader.Value);


                    switch (xmlReader.Name)
                    {
                        case "Threshold":
                            if (!xmlReader.Read()) return false;
                            //Tools[nIndex].Threshold = int.Parse(xmlReader.Value);
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

        public bool SaveConfig()
        {
            string strPath = $"{Application.StartupPath}\\LastStatus.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();
                SaveConfigToXML(xmlWriter);
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
            return true;



            Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }

        public bool SaveConfigToXML(XmlWriter xmlWriter)
        {
            try
            {
                xmlWriter.WriteStartElement("Parameter");
                //xmlWriter.WriteElementString("Threshold", Tools[nIndex].Threshold.ToString());
                //xmlWriter.WriteElementString("InspY", Tools[nIndex].InspY.ToString());
                //xmlWriter.WriteElementString("UseInsp", Tools[nIndex].UseInsp.ToString());
                //xmlWriter.WriteElementString("InvalidSpec", Tools[nIndex].InvalidSpec.ToString());
                //xmlWriter.WriteElementString("DirectionWB", Tools[nIndex].DirectionWB.ToString());
                xmlWriter.WriteEndElement();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

            return true;
        }
        #endregion



    }

    public class CGrabBuffer
    {
        public int Index = 0;
        public Mat ImageGrab = new Mat();
        public IntPtr Handle = IntPtr.Zero;

        
    }
}
