using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IntelligentFactory
{
    public class ISystem
    {
        #region MODE       
        public enum MODE { NO_LICENSE, READY, AUTO, ALARM, SIMULATION };
        private MODE m_eModePrev = MODE.READY;
        private MODE m_eMode = MODE.READY;
        public MODE Mode
        {
            get { return m_eMode; }
            set
            {
                m_eModePrev = m_eMode;
                m_eMode = value;

                if (m_eMode != m_eModePrev)
                {
                    if (EventChangedMode != null)
                    {
                        EventChangedMode(null, null);
                    }
                }
            }
        }

        public EventHandler EventChangedMode = null;
        #endregion

        #region MENU     
        public string m_strSelectedMenu = "Main";
        public string SelectedMenu
        {
            get { return m_strSelectedMenu; }
            set
            {
                m_strSelectedMenu = value;

                if (EventChangedMenu != null)
                {
                    EventChangedMenu(null, null);
                }
            }
        }

        public EventHandler EventChangedMenu = null;
        #endregion

        #region RECIPE
        private string m_strLastRecipe = "";
        public string LastRecipe
        {
            get => m_strLastRecipe;
            set
            {
                LastRecipeUpdateTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                m_strLastRecipe = value;

                if (EventChangedRecipe != null)
                {
                    EventChangedRecipe(null, null);
                }
            }                                
        }

        private string m_strLastRecipeUpdateTime = "";
        public string LastRecipeUpdateTime
        {
            get => m_strLastRecipeUpdateTime;
            set => m_strLastRecipeUpdateTime = value;
        }

        private string m_strRecipeName = "";
        public string RecipeName
        {
            get { return m_strRecipeName; }
            set 
            {
                string strRecipeNamePrev = m_Recipe.Name;
                string strRecipeNameNew = value;

                if (strRecipeNameNew != strRecipeNamePrev)
                {
                    m_Recipe.Name = strRecipeNameNew;

                    if (EventChangedRecipe != null)
                    {
                        EventChangedRecipe(null, null);
                    }
                }

                // 처음 실행시에 레시피 자체가 없을 경우 
                if (strRecipeNameNew.Contains("\r") || strRecipeNameNew.Contains("\n") || strRecipeNameNew.Contains("\t") || strRecipeNameNew == "")
                {
                    m_Recipe.Name = "0.TEST";

                    if (EventChangedRecipe != null)
                    {
                        EventChangedRecipe(null, null);
                    }
                }
                m_strRecipeName = m_Recipe.Name;
                m_strLastRecipe = m_Recipe.Name;
            }
        }
        private CRecipe m_Recipe = new CRecipe();
        public CRecipe Recipe
        {
            get { return m_Recipe; }
            set { m_Recipe = value; }
        }

        public EventHandler EventChangedRecipe = null;
        #endregion

        #region NOTICE
        private string m_strNotice = "-";
        public string Notice
        {
            get { return m_strNotice; }
            set
            {
                m_strNotice = value;

                if (m_strNotice != "")
                {
                    if (EventChangedNotice != null)
                    {
                        EventChangedNotice(null, null);
                    }
                }
                Logger.WriteLog(LOG.Normal, Notice);
                //ILogger.Add(LOG_TYPE.SYSTEM, Notice);
            }
        }
        public EventHandler EventChangedNotice = null;
        #endregion

        #region LICENSE
#if USE_LICENSE
        private bool m_bUseLicense = true;
#else
        private bool m_bUseLicense = false;
#endif
        private string m_strLicense = "";
        public string License
        {
            get { return m_strLicense; }
            set 
            {
                m_strLicense = value; 

                if(m_bUseLicense)
                {
                    bool bCertificated = false;
                    //License 확인 후 

                    if (bCertificated) Mode = MODE.READY;
                    else Mode = MODE.NO_LICENSE;
                }
            }
        }
        #endregion

        #region RESULT
        public enum RESULT { IDLE, OK, NG };
        private RESULT m_eResult = RESULT.IDLE;
        public RESULT Result
        {
            get { return m_eResult; }
            set { m_eResult = value; }
        }
        #endregion

        #region UI
        private int m_nStyleIndex = 6;
        public int StyleIndex
        {
            get => m_nStyleIndex;
            set
            {
                m_nStyleIndex = value;

                if (EventUpdateStyle != null)
                {
                    EventUpdateStyle(null, null);
                }
            }
        }

        public EventHandler<EventArgs> EventUpdateStyle;

        #endregion

        #region PLC IP
        private string m_strPlcIP = "192.168.100.101";
        public string PlcIp
        {
            get => m_strPlcIP;
            set => m_strPlcIP = value;
        }

        private int m_nPlcLogicalNo = 0;
        public int PlcLogicalNo
        {
            get => m_nPlcLogicalNo;
            set => m_nPlcLogicalNo = value;
        }
        #endregion

        #region IPC
        public IntPtr IF_Handle { get; set; } = IntPtr.Zero;
        #endregion

        #region Algorithm
        private string m_strAlgorithm = "Pattern Matching";
        public string Algorithm
        {
            get => m_strAlgorithm;
            set => m_strAlgorithm = value;
        }
        #endregion

        #region Image Delete / Log
        private string m_strDrivePath = "C";
        public string DrivePath
        {
            get { return m_strDrivePath; }
            set { m_strDrivePath = value; }
        }
        private int m_nDriveVolum = 80;
        public int DriveVolum
        {
            get { return m_nDriveVolum; }
            set { m_nDriveVolum = value; }
        }
        private int m_nDeleteImageDay = 7;
        public int DeleteImageDay
        {
            get { return m_nDeleteImageDay; }
            set { m_nDeleteImageDay = value; }
        }

        private int m_nDeleteLogDay = 7;
        public int DeleteLogDay
        {
            get { return m_nDeleteLogDay; }
            set { m_nDeleteLogDay = value; }
        }
        #endregion

        #region PROPERTIES
        public enum PERMISSION : uint { OPERATOR = 1, ENGINEER, ADMINISTRATOR };
        public PERMISSION Permission = PERMISSION.OPERATOR;

        public string Password_Engineer { get; set; } = "0000";
        public string Password_Administrator { get; set; } = "0000";
        #endregion

        private string m_strSendLicense = "";
        public string SendLicense
        {
            get { return m_strSendLicense; }
            set { m_strSendLicense = value; }
        }


        public EventHandler EventChangeSize = null;

        public ISystem()
        {
            CUtil.InitDirectory(DEFINE.IMAGE);
            CUtil.InitDirectory(DEFINE.SAVE_IMAGE);
            CUtil.InitDirectory(DEFINE.RECIPE);
            CUtil.InitDirectory(DEFINE.CAPTURE);
            CUtil.InitDirectory(DEFINE.CONFIG);

            LoadConfig();
        }

        public void Close()
        {
            
        }

        #region CONFIG BY XML              
        private string m_XMLName = "SYSTEM";
        public bool LoadConfig()
        {
            try
            {
                string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

                if (File.Exists(strPath))
                {
                    XmlTextReader xmlReader = new XmlTextReader(strPath);

                    try
                    {
                        LoadConfigFromXML(xmlReader);
                    }
                    catch (Exception Desc)
                    {
                        Logger.WriteLog(LOG.AbNormal, "SYSTEM Load Ex ==> {0}", Desc.Message);                        
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig();
                    return false;
                }

                //m_strLastRecipe = m_strLastRecipe.Replace("\r\n", "");

                if (m_strLastRecipe.Contains("\r") || m_strLastRecipe.Contains("\n") || m_strLastRecipe.Contains("\t"))
                {
                    m_strLastRecipe = "0.TEST";
                    SaveConfig();
                }

                RecipeName = m_strLastRecipe;
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
                        case "License":
                            if (!xmlReader.Read()) return false;
                            License = xmlReader.Value;
                            break;
                        case "Password_Engineer":
                            if (!xmlReader.Read()) return false;
                            Password_Engineer = xmlReader.Value;
                            break;
                        case "Password_Administrator":
                            if (!xmlReader.Read()) return false;
                            Password_Administrator = xmlReader.Value;
                            break;
                        case "Algorithm":
                            if (!xmlReader.Read()) return false;
                            Algorithm = xmlReader.Value;
                            break;
                        case "LastRecipe":
                            if (!xmlReader.Read()) return false;
                            LastRecipe = xmlReader.Value;
                            break;
                        case "LastRecipeUpdateTime":
                            if (!xmlReader.Read()) return false;
                            LastRecipeUpdateTime = xmlReader.Value;
                            break;
                        case "DeleteLogDay":
                            if (!xmlReader.Read()) return false;
                            DeleteLogDay = int.Parse(xmlReader.Value);
                            break;
                        case "DrivePath":
                            if (!xmlReader.Read()) return false;
                            DrivePath = xmlReader.Value;
                            break;
                        case "DriveVolum":
                            if (!xmlReader.Read()) return false;
                            DriveVolum = int.Parse(xmlReader.Value);
                            break;
                        case "DeleteImageDay":
                            if (!xmlReader.Read()) return false;
                            DeleteImageDay = int.Parse(xmlReader.Value);
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
            string strPath = Application.StartupPath + "\\" + m_XMLName + ".xml";

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

            Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
            return true;
        }      

        public bool SaveConfigToXML(XmlWriter xmlWriter)
        {
            try
            {
                xmlWriter.WriteStartElement("SYSTEM");
                xmlWriter.WriteElementString("License", License);
                xmlWriter.WriteElementString("Password_Engineer", Password_Engineer);
                xmlWriter.WriteElementString("Password_Administrator", Password_Administrator);
                xmlWriter.WriteElementString("Algorithm", Algorithm.ToString());
                xmlWriter.WriteElementString("LastRecipe", LastRecipe);
                xmlWriter.WriteElementString("LastRecipeUpdateTime", LastRecipeUpdateTime);
                xmlWriter.WriteElementString("DeleteLogDay", DeleteLogDay.ToString());
                xmlWriter.WriteElementString("DrivePath", DrivePath);
                xmlWriter.WriteElementString("DrivePath", DrivePath);
                xmlWriter.WriteElementString("DriveVolum", DriveVolum.ToString());
                xmlWriter.WriteElementString("DeleteImageDay", DeleteImageDay.ToString());
                xmlWriter.WriteElementString("PlcIp", PlcIp);
                xmlWriter.WriteEndElement();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "SYSTEM Save Ex ==> {0}", Desc.Message);                                
            }            
            
            return true;
        }
        #endregion
    }
}
