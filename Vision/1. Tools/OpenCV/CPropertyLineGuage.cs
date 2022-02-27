using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


using OpenCvSharp;

namespace IntelligentFactory
{
    public class CPropertyLineGuage
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE_EQULIZE_HIST")]
        public bool USE_EQUALIZE_HIST { get; set; } = false;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE_MORP")]
        public bool USE_MORP { get; set; } = false;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("MORP_OPEN")]
        public int MORP_OPEN { get; set; } = 0;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("MORP_CLOSE")]
        public int MORP_CLOSE { get; set; } = 0;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("MORP_DIALATE")]
        public int MORP_DILATE { get; set; } = 0;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("MORP_ERODE")]
        public int MORP_ERODE { get; set; } = 0;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE_THRESHOLD")]
        public bool USE_THRESHOLD { get; set; } = false;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("USE_THRESHOLD_INV")]
        public bool USE_THRESHOLD_INV { get; set; } = false;

        [CategoryAttribute("Image Process"), DescriptionAttribute(""), DisplayNameAttribute("THRESHOLD")]
        public int THRESHOLD { get; set; } = 128;

        [CategoryAttribute("Image Parameter"), DescriptionAttribute(""), DisplayNameAttribute("CONTRAST")]
        public int CONTRAST { get; set; } = 30;

        [CategoryAttribute("Image Parameter"), DescriptionAttribute(""), DisplayNameAttribute("THICKNESS")]
        public int THICKNESS { get; set; } = 5;

        public enum PROJECTION_POLARITY : uint { BTOW = 0, WTOB, ALL };

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("POLARITY")]
        public PROJECTION_POLARITY PRJ_PORALITY { get; set; } = PROJECTION_POLARITY.BTOW;

        public enum PROJECTION_DIR : uint { X_LTOR = 0, X_RTOL, Y_TTOB, Y_BTOT, DIAG };
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("DIRECTION")]
        public PROJECTION_DIR PRJ_DIR { get; set; } = PROJECTION_DIR.X_LTOR;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("OUTFITTING")]
        public int OUTFITTING { get; set; } = 30;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("ROI")]
        public string SROI { get { return ROI.ToString(); } }
        public Rectangle ROI = new Rectangle();
        public Rect CVROI { get => CConverter.RectToCVRect(ROI); }

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("SAMPLING_STEP")]
        public int SAMPLING_STEP { get; set; } = 10;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("SAMPLING_CYCLE")]
        public int SAMPLING_CYCLE { get; set; } = 10;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("USE_FITTING")]
        public bool USE_FITTING { get; set; } = false;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public CPropertyLineGuage(string strName)
        {
            NAME = strName;
        }


        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_LINEGUAGE";
        public bool LoadConfig(string strRecipeName)
        {
            try
            {
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

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
                                    case "USE_EQUALIZE_HIST": if (xmlReader.Read()) USE_EQUALIZE_HIST = bool.Parse(xmlReader.Value); break;
                                    case "USE_MORP": if (xmlReader.Read()) USE_MORP = bool.Parse(xmlReader.Value); break;
                                    case "MORP_OPEN": if (xmlReader.Read()) MORP_OPEN = int.Parse(xmlReader.Value); break;
                                    case "MORP_CLOSE": if (xmlReader.Read()) MORP_CLOSE = int.Parse(xmlReader.Value); break;
                                    case "MORP_DILATE": if (xmlReader.Read()) MORP_DILATE = int.Parse(xmlReader.Value); break;
                                    case "MORP_ERODE": if (xmlReader.Read()) MORP_ERODE = int.Parse(xmlReader.Value); break;

                                    case "USE_THRESHOLD": if (xmlReader.Read()) USE_THRESHOLD = bool.Parse(xmlReader.Value); break;
                                    case "USE_THRESHOLD_INV": if (xmlReader.Read()) USE_THRESHOLD_INV = bool.Parse(xmlReader.Value); break;
                                    case "USE_FITTING": if (xmlReader.Read()) USE_FITTING = bool.Parse(xmlReader.Value); break;
                                    case "THRESHOLD": if (xmlReader.Read()) THRESHOLD = int.Parse(xmlReader.Value); break;

                                    case "CONTRAST": if (xmlReader.Read()) CONTRAST = int.Parse(xmlReader.Value); break;
                                    case "THICKNESS": if (xmlReader.Read()) THICKNESS = int.Parse(xmlReader.Value); break;
                                    case "SAMPLING_STEP": if (xmlReader.Read()) SAMPLING_STEP = int.Parse(xmlReader.Value); break;
                                    case "SAMPLING_CYCLE": if (xmlReader.Read()) SAMPLING_CYCLE = int.Parse(xmlReader.Value); break;

                                    case "PRJ_PORALITY": if (xmlReader.Read()) PRJ_PORALITY = (PROJECTION_POLARITY)Enum.Parse(typeof(PROJECTION_POLARITY), xmlReader.Value); break;
                                    case "PRJ_DIR": if (xmlReader.Read()) PRJ_DIR = (PROJECTION_DIR)Enum.Parse(typeof(PROJECTION_DIR), xmlReader.Value); break;

                                    case "ROI": if (xmlReader.Read()) ROI = CConverter.StringToRoi(xmlReader.Value); break;
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
                        Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                        xmlReader.Close();
                    }

                    xmlReader.Close();
                }
                else
                {
                    SaveConfig(strRecipeName);
                    return false;
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            return true;
        }
        public bool SaveConfig(string strRecipeName)
        {
            string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

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
                xmlWriter.WriteElementString("USE_EQUALIZE_HIST", USE_EQUALIZE_HIST.ToString());
                xmlWriter.WriteElementString("USE_MORP", USE_MORP.ToString());
                xmlWriter.WriteElementString("MORP_OPEN", MORP_OPEN.ToString());
                xmlWriter.WriteElementString("MORP_CLOSE", MORP_CLOSE.ToString());
                xmlWriter.WriteElementString("MORP_DILATE", MORP_DILATE.ToString());
                xmlWriter.WriteElementString("MORP_ERODE", MORP_ERODE.ToString());

                xmlWriter.WriteElementString("ROI", CConverter.RoiToString(ROI));
                xmlWriter.WriteElementString("USE_THRESHOLD", USE_THRESHOLD.ToString());
                xmlWriter.WriteElementString("USE_FITTING", USE_FITTING.ToString());
                xmlWriter.WriteElementString("USE_THRESHOLD_INV", USE_THRESHOLD_INV.ToString());
                xmlWriter.WriteElementString("THRESHOLD", THRESHOLD.ToString());
                xmlWriter.WriteElementString("CONTRAST", CONTRAST.ToString());
                xmlWriter.WriteElementString("THICKNESS", THICKNESS.ToString());
                xmlWriter.WriteElementString("SAMPLING_STEP", SAMPLING_STEP.ToString());
                xmlWriter.WriteElementString("SAMPLING_CYCLE", SAMPLING_CYCLE.ToString());
                xmlWriter.WriteElementString("PRJ_PORALITY", PRJ_PORALITY.ToString());
                xmlWriter.WriteElementString("PRJ_DIR", PRJ_DIR.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
            finally
            {
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return true;
        }
        #endregion

    }
}
