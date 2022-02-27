using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace IntelligentFactory.Device
{
    public class CJSL200
    {
        public bool IsAlive { get; set; } = false;
        public string PortName { get; set; } = "COM2";

        public int BaudRate { get; set; } = 19200;

        public SerialPort Device { get; set; } = new SerialPort();

        public CJSL200() { }
    
        public bool Init()
        {
            try
            {
                LoadConfig();

                Device.PortName = PortName;
                Device.BaudRate = BaudRate;
                Device.DataBits = 8;
                Device.Parity = System.IO.Ports.Parity.None;
                Device.StopBits = System.IO.Ports.StopBits.One;
                Device.Open();
                if(Device.IsOpen)
                {
                    Device.DiscardOutBuffer();
                    Device.DiscardInBuffer();
                    IsAlive = true;

                    string strCommstr = "*IDN?";
                    Device.WriteLine(strCommstr);
                 
                    int nRec = Device.BytesToRead;

                    int nCnt = 0;
                    while (nRec < 20)
                    {
                        Thread.Sleep(100);
                        nRec = Device.BytesToRead;
                        nCnt += 100;

                        if (nCnt == 2000)
                        {
                            IsAlive = false;
                            return false;
                        }
                    }

                    byte[] btData = new byte[nRec];
                    Device.Read(btData, 0, nRec);

                    string strParshing = Encoding.Default.GetString(btData, 0, nRec);
                    Logger.WriteLog(LOG.Comm, "[OK] *IDN? : {0}", strParshing);

                    string strCommer = "TRIG:SOUR IMM";
                    Device.WriteLine(strCommer);
                    Logger.WriteLog(LOG.Comm, "[OK] Commer : {0}", strCommer);

                    strCommer = "INIT:CONT OFF";
                    Device.WriteLine(strCommer);
                    Logger.WriteLog(LOG.Comm, "[OK] Commer : {0}", strCommer);

                    strCommer = "FUNC RV";
                    Device.WriteLine(strCommer);
                    Logger.WriteLog(LOG.Comm, "[OK] Commer : {0}", strCommer);

                    //Device.DataReceived += Device_DataReceived;
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (Device.IsOpen)
                {
                    Device.DataReceived -= Device_DataReceived;
                    Device.Dispose();
                    IsAlive = false;
                }

                Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }


        private void Device_DataReceived(object Commer, SerialDataReceivedEventArgs e)
        {


            throw new NotImplementedException();
        }

        #region File Manager              
        private string m_XMLName = "JSL200";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\Recipe\\" + IGlobal.Instance.iSystem.Recipe.Name + "\\" + m_XMLName + ".xml";

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

        public bool SaveConfig()
        {
            string strPath = Application.StartupPath + "\\Recipe\\" + IGlobal.Instance.iSystem.Recipe.Name + "\\" + m_XMLName + ".xml";

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

                LoadConfig();
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
                        case "PortName":
                            if (!xmlReader.Read()) return false;
                            PortName = xmlReader.Value;
                            break;
                        case "BaudRate":
                            if (!xmlReader.Read()) return false;
                            BaudRate = int.Parse(xmlReader.Value);
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
            xmlWriter.WriteStartElement("Parameter");
            xmlWriter.WriteElementString("PortName", PortName);
            xmlWriter.WriteElementString("BaudRate", BaudRate.ToString());
            return true;
        }

        #endregion
    }
}
