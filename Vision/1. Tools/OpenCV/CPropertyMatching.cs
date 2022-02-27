﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class CPropertyMatching
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NUM_MATCH")]
        public int NUM_MATCH { get; set; } = 1;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("SCORE_MIN")]
        public double SCORE_MIN { get; set; } = 0.6D;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("MAGNIFICATION")]
        public double MAGNIFIATION { get; set; } = 2.0D;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("ROI")]

        public string SROI { get { return InspectROI.ToString(); } }

        public Rectangle InspectROI = new Rectangle();

        public string STrainROI { get { return TrainROI.ToString(); } }

        public Rectangle TrainROI = new Rectangle();        

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("MAX_OVER_LAP")]
        public double MAX_OVER_LAP { get; set; } = 0.5D;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NUM_LEVEL")]
        public double NUM_LEVEL { get; set; } = 0.0D;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("GREEDINESS")]
        public double GREEDINESS { get; set; } = 0.9D;


        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("EMPTY")]
        public string EMPTY { get; set; } = "EMPTY";

        public Mat ImageTemplate = new Mat();

        public CPropertyMatching(string strName)
        {
            NAME = strName;
        }

        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_MATCHING";
        public bool LoadConfig(string strRecipeName)
        {
            try
            {
                string strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";

                try
                {
                    string strName = Application.StartupPath + "\\RECIPE\\" + strRecipeName + $"\\{NAME}.bmp";
                    ImageTemplate = Cv2.ImRead(Application.StartupPath + "\\RECIPE\\" + strRecipeName + $"\\{NAME}.bmp");
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }

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
                                    case "SCORE_MIN": if (xmlReader.Read()) SCORE_MIN = double.Parse(xmlReader.Value); break;
                                    case "MAGNIFIATION": if (xmlReader.Read()) MAGNIFIATION = double.Parse(xmlReader.Value); break;
                                    case "ROI": if (xmlReader.Read())  InspectROI = CConverter.StringToRoi(xmlReader.Value); break;
                                    case "NUM_MATCH": if (xmlReader.Read()) NUM_MATCH = int.Parse(xmlReader.Value); break;
                                    case "MAX_OVER_LAP": if (xmlReader.Read()) MAX_OVER_LAP = double.Parse(xmlReader.Value); break;
                                    case "NUM_LEVEL": if (xmlReader.Read()) NUM_LEVEL = double.Parse(xmlReader.Value); break;
                                    case "GREEDINESS": if (xmlReader.Read()) GREEDINESS = double.Parse(xmlReader.Value); break;

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

            try
            {
                ImageTemplate.SaveImage(Application.StartupPath + "\\RECIPE\\" + strRecipeName + $"\\{NAME}.bmp");
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

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
                xmlWriter.WriteElementString("SCORE_MIN", SCORE_MIN.ToString());
                xmlWriter.WriteElementString("MAGNIFIATION", MAGNIFIATION.ToString());
                xmlWriter.WriteElementString("ROI", CConverter.RoiToString(InspectROI));
                xmlWriter.WriteElementString("NUM_MATCH", NUM_MATCH.ToString());
                xmlWriter.WriteElementString("MAX_OVER_LAP", MAX_OVER_LAP.ToString());
                xmlWriter.WriteElementString("NUM_LEVEL", NUM_LEVEL.ToString());
                xmlWriter.WriteElementString("GREENINESS", GREEDINESS.ToString());




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
