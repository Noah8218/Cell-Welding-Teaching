using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

using System.Xml;

#if EURESYS
using Euresys.Open_eVision_2_14;
#endif

#if OPENCV
using OpenCvSharp;
#endif

namespace IntelligentFactory
{
    public class CRecipe
    {   
#if EURESYS
        public EImageBW8 ImageTemplate = new EImageBW8();
#endif
#if OPENCV
        public Mat ImageTemplate = new Mat();
#endif

        public EventHandler<EventArgs> EventChagedRecipe;

        private const string RECIPE_DIRECTORY = "RECIPE";

        private string m_strName = "SETUP";
        private string m_strNamePrev = "";
        public int MODEL_NO { get; set; } = 0;
        public string Name
        {
            get { return m_strName; }
            set
            {
                m_strNamePrev = m_strName;
                m_strName = value;

                //LoadTools();
                LoadConfig();

                try
                {
                    MODEL_NO = int.Parse(m_strName.Split('.')[0]);
                }
                catch (Exception Desc)
                {

                }

                if (EventChagedRecipe != null)
                {
                    EventChagedRecipe(null, null);
                }
                if (m_strName != "")
                {
                    InitDirectory(m_strName);
                }
            }
        }

        public Rectangle ROI_BCR = new Rectangle();
        public Rectangle ROI_DISTANCE_INSP1 = new Rectangle();
        public Rectangle ROI_DISTANCE_INSP2 = new Rectangle();
        public Rectangle ROI_DISTANCE_ALIGN = new Rectangle();


        #region PARAMETER
        public int Ch1_LightValue         = 100;
        public int Ch2_LightValue         = 100;
        public string PreImageProcess = "";  //1,THRESHOLD,200;2,SOBEL,5;

        #region Drive
        public string DrivePath = "D";
        public int DriveVolum = 80;
        public int DeleteImageDay = 7;
        #endregion

        #region DISTANCE
        //public int    OverCount_Min = 5;
        public int    Insp1_Thickness      = 5;
        public int    Insp1_PitchPixel     = 5;
        public int    Insp1_Polarity_Left  = 0;
        public int    Insp1_Polarity_Right = 1;
        public double Insp1_MinDistancemm  = 6.5D;
        public double Insp1_MaxDistancemm  = 9.5D;
        public bool   Insp1_Use            = true;
                                           
        public int    Insp2_Thickness      = 5;
        public int    Insp2_PitchPixel     = 5;
        public int    Insp2_Polarity_Left  = 0;
        public int    Insp2_Polarity_Right = 1;
        public double Insp2_MinDistancemm  = 6.5D;
        public double Insp2_MaxDistancemm  = 9.5D;
        public bool   Insp2_Use            = true;

        public double Insp_Offset = 0.51D;
        #endregion

        #region Flag
        public bool Insp_Min_Use = false;
        public bool Insp_Max_Use = false;
        public bool Insp_Avg_Use = false;
        #endregion

        #region DISTANCE
        public int Timeoutms          = 5000;
        public int DataLength         = 17;
        public Rectangle ROI          = new Rectangle();
        #endregion

        #endregion
#if EURESYS
        public class CTools_Euresys
        {
            public int Index = 0;
            public string Name = "";
            public bool IsBlob = false;
            public ELineGauge eLineGuage { get; set; } = new ELineGauge();
            public EMatcher eMatcher { get; set; } = new EMatcher();
            public EImageBW8 ImageTemplate { get; set; } = new EImageBW8();
            public int Threshold { get; set; } = 100; // 저장
            public double InvalidSpec { get; set; } = 10;
            public bool InspY = false; // 저장 (y축)
            public bool UseInsp = true; // 저장 (x축)
            public bool IsSub = false;

            //0 : WB 1: BW
            public int DirectionWB = 0;

            public CTools_Euresys(int nIndex, string strName, bool bInspY = false, bool bBlob = false, bool bSub = false)
            {
                Index = nIndex;
                Name = strName;
                InspY = bInspY;
                IsBlob = bBlob;
            }
        }
        public List<CTools_Euresys> Tools = new List<CTools_Euresys>();


        public bool LoadTools()
        {
            try
            {
                Tools.Clear();
                Tools.Add(new CTools_Euresys(DEFINE.CAM1_X1, DEFINE.CAM1_X1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM1_X2, DEFINE.CAM1_X2.ToString(), false, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM2_X1, DEFINE.CAM2_X1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM2_X2, DEFINE.CAM2_X2.ToString(), false, false, true ));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_LX1, DEFINE.CAM3_LX1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_LX2, DEFINE.CAM3_LX2.ToString(), false, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_LY1, DEFINE.CAM3_LY1.ToString(), true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_LY2, DEFINE.CAM3_LY2.ToString(), true, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_RX1, DEFINE.CAM3_RX1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_RX2, DEFINE.CAM3_RX2.ToString(), false, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_RY1, DEFINE.CAM3_RY1.ToString(), true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM3_RY2, DEFINE.CAM3_RY2.ToString(), true, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_LX1, DEFINE.CAM4_LX1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_LX2, DEFINE.CAM4_LX2.ToString(), false, false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_LY1, DEFINE.CAM4_LY1.ToString(), true, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_LY2, DEFINE.CAM4_LY2.ToString(), true, true, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_RX1, DEFINE.CAM4_RX1.ToString()));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_RX2, DEFINE.CAM4_RX2.ToString(), false, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_RY1, DEFINE.CAM4_RY1.ToString(), true, true));
                Tools.Add(new CTools_Euresys(DEFINE.CAM4_RY2, DEFINE.CAM4_RY2.ToString(), true, true, true));

                string strRecipePath = Application.StartupPath + "\\" + DEFINE.RECIPE + "\\" + Name;
                DirectoryInfo dirRecipe = new DirectoryInfo(strRecipePath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                for (int i = 0; i < Tools.Count; i++)
                {
                    string strPathToolsLineGuage = $"{strRecipePath}\\{Tools[i].Name}_LineGuage.rcp";
                    FileInfo fileInfoLineGuage = new FileInfo(strPathToolsLineGuage);
                    if (fileInfoLineGuage.Exists) Tools[i].eLineGuage.Load(strPathToolsLineGuage);

                    string strPathToolsMatcher = $"{strRecipePath}\\{Tools[i].Name}_Matcher.rcp";
                    FileInfo fileInfoMatcher = new FileInfo(strPathToolsMatcher);
                    if (fileInfoMatcher.Exists) Tools[i].eMatcher.Load(strPathToolsMatcher);

                    string strPathToolsTemplate = $"{strRecipePath}\\{Tools[i].Name}_Template.bmp";
                    FileInfo fileInfoTemplate = new FileInfo(strPathToolsTemplate);
                    if (fileInfoTemplate.Exists)
                    {
                        Tools[i].ImageTemplate.Load(strPathToolsTemplate);
                        Tools[i].eMatcher.LearnPattern(Tools[i].ImageTemplate);
                    }
                }

                //string strPath_LineGuage_Cam1_X = $"{strRecipePath}\\LineGuage_Cam1_X.rcp";
                //FileInfo file_LineGuage_Cam1_X = new FileInfo(strPath_LineGuage_Cam1_X);
                //if (file_LineGuage_Cam1_X.Exists) eLineGuage_Cam1_X.Load(strPath_LineGuage_Cam1_X);
                //Tools[DEFINE.CAM1_X].eLineGuage = eLineGuage_Cam1_X;

                //string strPath_LineGuage_Cam2_X = $"{strRecipePath}\\LineGuage_Cam2_X.rcp";
                //FileInfo file_LineGuage_Cam2_X = new FileInfo(strPath_LineGuage_Cam2_X);
                //if (file_LineGuage_Cam2_X.Exists) eLineGuage_Cam2_X.Load(strPath_LineGuage_Cam2_X);
                //Tools[DEFINE.CAM2_X].eLineGuage = eLineGuage_Cam2_X;

                //string strPath_LineGuage_Cam3_X = $"{strRecipePath}\\LineGuage_Cam3_X.rcp";
                //FileInfo file_LineGuage_Cam3_X = new FileInfo(strPath_LineGuage_Cam3_X);
                //if (file_LineGuage_Cam3_X.Exists) eLineGuage_Cam3_X.Load(strPath_LineGuage_Cam3_X);
                //Tools[DEFINE.CAM3_X].eLineGuage = eLineGuage_Cam3_X;

                //string strPath_LineGuage_Cam3_Y = $"{strRecipePath}\\LineGuage_Cam3_Y.rcp";
                //FileInfo file_LineGuage_Cam3_Y = new FileInfo(strPath_LineGuage_Cam3_Y);
                //if (file_LineGuage_Cam3_Y.Exists) eLineGuage_Cam3_Y.Load(strPath_LineGuage_Cam3_Y);
                //Tools[DEFINE.CAM3_Y].eLineGuage = eLineGuage_Cam3_Y;

                //string strPath_LineGuage_Cam4_X = $"{strRecipePath}\\LineGuage_Cam4_X.rcp";
                //FileInfo file_LineGuage_Cam4_X = new FileInfo(strPath_LineGuage_Cam4_X);
                //if (file_LineGuage_Cam4_X.Exists) eLineGuage_Cam4_X.Load(strPath_LineGuage_Cam4_X);
                //Tools[DEFINE.CAM4_X].eLineGuage = eLineGuage_Cam4_X;


                //string strPath_LineGuage_Cam4_Y = $"{strRecipePath}\\LineGuage_Cam4_Y.rcp";
                //FileInfo file_LineGuage_Cam4_Y = new FileInfo(strPath_LineGuage_Cam4_Y);
                //if (file_LineGuage_Cam4_Y.Exists) eLineGuage_Cam4_Y.Load(strPath_LineGuage_Cam4_Y);
                //Tools[DEFINE.CAM4_Y].eLineGuage = eLineGuage_Cam4_Y;

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool SaveTools()
        {
            try
            {
                string strRecipePath = Application.StartupPath + "\\" + DEFINE.RECIPE + "\\" + Name;
                DirectoryInfo dirRecipe = new DirectoryInfo(strRecipePath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                for (int i = 0; i < Tools.Count; i++)
                {
                    string strPathToolsLineGuage = $"{strRecipePath}\\{Tools[i].Name}_LineGuage.rcp";
                    Tools[i].eLineGuage.Save(strPathToolsLineGuage);

                    string strPathToolsMatcher = $"{strRecipePath}\\{Tools[i].Name}_Matcher.rcp";
                    Tools[i].eMatcher.Save(strPathToolsMatcher);

                    string strPathToolsTemplate = $"{strRecipePath}\\{Tools[i].Name}_Template.bmp";
                    if (Tools[i].ImageTemplate != null && !Tools[i].ImageTemplate.IsVoid) Tools[i].ImageTemplate.Save(strPathToolsMatcher);
                }

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

#endif
        public bool InitDirectory(string strRecipeName)
        {
            try
            {
                string strRecipePath = Application.StartupPath + "\\" + RECIPE_DIRECTORY + "\\";
                DirectoryInfo dirRecipe = new DirectoryInfo(strRecipePath);
                if (dirRecipe.Exists == false) dirRecipe.Create();

                string strRecipeNamePath = Application.StartupPath + "\\" + RECIPE_DIRECTORY + "\\" + strRecipeName;
                DirectoryInfo dirRecipeName = new DirectoryInfo(strRecipeNamePath);
                if (dirRecipeName.Exists == false) dirRecipeName.Create();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

#region CONFIG BY XML              
        private string m_XMLName = "Parameter";
        public bool LoadConfig()
        {
            try
            {
                string strPath = $"{Application.StartupPath}\\RECIPE\\{Name}\\Parameter.xml";

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
                                    //case "MODEL_NO": if (xmlReader.Read()) MODEL_NO = uint.Parse(xmlReader.Value); break;

                                    case "DrivePath"           : if (!xmlReader.Read()) return false; DrivePath = xmlReader.Value; break;
                                    case "DriveVolum"          : if (!xmlReader.Read()) return false; DriveVolum = int.Parse(xmlReader.Value); break;
                                    case "DeleteImageDay"      : if (!xmlReader.Read()) return false; DeleteImageDay = int.Parse(xmlReader.Value); break;
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


        public bool SaveConfig()
        {
            string strPath = $"{Application.StartupPath}\\RECIPE\\{Name}\\Parameter.xml";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = "\r\n";
            XmlWriter xmlWriter = XmlWriter.Create(strPath, settings);
            try
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("Parameter");

                //xmlWriter.WriteElementString("OverCount_Min", OverCount_Min.ToString());
                //xmlWriter.WriteElementString("MODEL_NO", MODEL_NO.ToString());

                xmlWriter.WriteElementString("DrivePath", DrivePath.ToString());
                xmlWriter.WriteElementString("DriveVolum", DriveVolum.ToString());
                xmlWriter.WriteElementString("DeleteImageDay", DeleteImageDay.ToString());

                xmlWriter.WriteEndElement();

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
#endregion




    }
}
