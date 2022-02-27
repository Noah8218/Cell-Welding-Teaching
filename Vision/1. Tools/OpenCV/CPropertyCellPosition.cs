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
    public class CPropertyCellPosition
    {
        [CategoryAttribute("1. Parameter"), DescriptionAttribute(""), DisplayNameAttribute("NAME")]
        public string NAME { get; set; } = "";
        public enum ARRAY_DIRECTION : int { ZIGZAG_Y, ZIGZAG_X };
        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("배열 생성 조건")]
        public ARRAY_DIRECTION ARRAYDIRCETION_TOP { get; set; } = ARRAY_DIRECTION.ZIGZAG_Y;
        
        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("안전 위치 Z")]
        public double SAFETY_Z_TOP { get; set; } = 0;

        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("열")]
        public int COLUMNS_TOP { get; set; } = 10;

        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("행")]
        public int ROWS_TOP { get; set; } = 10;

        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("카운트")]
        public int COUNT_TOP { get; set; } = 100;

        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("셀 간격 X (mm)")]
        public float GAP_X_MM_TOP { get; set; } = 5;

        [CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("셀 간격 Y (mm)")]
        public float GAP_Y_MM_TOP { get; set; } = 5;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("배열 생성 조건")]
        public ARRAY_DIRECTION ARRAYDIRCETION_BOTTOM { get; set; } = ARRAY_DIRECTION.ZIGZAG_Y;       

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("안전 위치 Z")]
        public double SAFETY_Z_BOTTOM { get; set; } = 0;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("열")]
        public int COLUMNS_BOTTOM { get; set; } = 10;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("행")]
        public int ROWS_BOTTOM { get; set; } = 10;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("카운트")]
        public int COUNT_BOTTOM { get; set; } = 100;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("셀 간격 X (mm)")]
        public float GAP_X_MM_BOTTOM { get; set; } = 5;

        [CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("셀 간격 Y (mm)")]
        public float GAP_Y_MM_BOTTOM { get; set; } = 5;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("Z 기본값")]
        public float DEFAULT_Z { get; set; } = 0.0F;
        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("채널 기본값")]
        public int DEFAULT_CH { get; set; } = 1;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("클릭 영역")]
        public int CLICK_BOUNDARY { get; set; } = 25;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("글자 크기")]
        public double FONT_SIZE { get; set; } = 10;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("극성 표기")]
        public bool IS_DISPLAY_POLARITY { get; set; } = true;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("화면 오프셋 X")]
        public int DISPLAY_OFFSET_X { get; set; } = 3000;

        [CategoryAttribute("Format"), DescriptionAttribute(""), DisplayNameAttribute("화면 오프셋 Y")]
        public int DISPLAY_OFFSET_Y { get; set; } = 300;


        //[CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 X")]
        public float ORIGIN_OFFSET_X_TOP  = 0.0F;
        //[CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 Y")]
        public float ORIGIN_OFFSET_Y_TOP = 0.0F;
        //[CategoryAttribute("2. A"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 Z")]
        public float ORIGIN_OFFSET_Z_TOP = 0.0F;

        //[CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 X")]
        public float ORIGIN_OFFSET_X_BOTTOM  = 0.0F;

        //[CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 Y")]
        public float ORIGIN_OFFSET_Y_BOTTOM = 0.0F;
        //[CategoryAttribute("3. B"), DescriptionAttribute(""), DisplayNameAttribute("원점 오프셋 Z")]
        public float ORIGIN_OFFSET_Z_BOTTOM = 0.0F;

        public List<CFormatCellPosition> CellPositions_Top = new List<CFormatCellPosition>();
        public List<CFormatCellPosition> CellPositions_Bottom = new List<CFormatCellPosition>();

        public List<CFormatCellPosition> CellPositions_Backup_Top = new List<CFormatCellPosition>();
        public List<CFormatCellPosition> CellPositions_Backup_Bottom = new List<CFormatCellPosition>();
        public CPropertyCellPosition(string strName)
        {
            NAME = strName;
        }

        public void AddCellPosition_Top(CFormatCellPosition pos)
        {
            CellPositions_Top.Add(pos);
        }

        public void AddCellPosition_Bottom(CFormatCellPosition pos)
        {
            CellPositions_Bottom.Add(pos);
        }

        public int GetSelectedIndexTop()
        {
            for (int i = 0; i < CellPositions_Top.Count; i++)
            {
                if (CellPositions_Top[i].Selected) return i;
            }

            return -1;
        }

        public int GetSelectedIndexBottom()
        {
            for (int i = 0; i < CellPositions_Bottom.Count; i++)
            {
                if (CellPositions_Bottom[i].Selected) return i;
            }

            return -1;
        }

        public void UpdateIndexTop()
        {
            for (int i = 0; i < CellPositions_Top.Count; i++) CellPositions_Top[i].Index = i + 1;
        }

        public void UpdateIndexBottom()
        {
            for (int i = 0; i < CellPositions_Bottom.Count; i++) CellPositions_Bottom[i].Index = i + 1;
        }
     

        #region CONFIG BY XML              
        private string m_XMLName = "PROPERTY_CELL_POSITION";
        public bool LoadConfig(string strRecipeName, bool bRecipePath = true)
        {
            try
            {
                string strPath = "";
                if (bRecipePath)
                {
                    strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";
                }
                else
                {
                    strPath = strRecipeName;
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
                                    case "ROWS_TOP": if (xmlReader.Read()) ROWS_TOP = int.Parse(xmlReader.Value); break;
                                    case "COLUMNS_TOP": if (xmlReader.Read()) COLUMNS_TOP = int.Parse(xmlReader.Value); break;
                                    case "COUNT_TOP": if (xmlReader.Read()) COUNT_TOP = int.Parse(xmlReader.Value); break;
                                    case "ARRAYDIRCETION_TOP": if (xmlReader.Read()) ARRAYDIRCETION_TOP = (ARRAY_DIRECTION)int.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_X_TOP": if (xmlReader.Read()) ORIGIN_OFFSET_X_TOP = float.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_Y_TOP": if (xmlReader.Read()) ORIGIN_OFFSET_Y_TOP = float.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_Z_TOP": if (xmlReader.Read()) ORIGIN_OFFSET_Z_TOP = float.Parse(xmlReader.Value); break;
                                    case "SAFETY_Z_TOP": if (xmlReader.Read()) SAFETY_Z_TOP = int.Parse(xmlReader.Value); break;

                                    case "FONT_SIZE": if (xmlReader.Read()) FONT_SIZE = double.Parse(xmlReader.Value); break;

                                    case "GAP_X_MM_TOP": if (xmlReader.Read()) GAP_X_MM_TOP = float.Parse(xmlReader.Value); break;
                                    case "GAP_Y_MM_TOP": if (xmlReader.Read()) GAP_Y_MM_TOP = float.Parse(xmlReader.Value); break;

                                    case "ROWS_BOTTOM": if (xmlReader.Read()) ROWS_BOTTOM = int.Parse(xmlReader.Value); break;
                                    case "COLUMNS_BOTTOM": if (xmlReader.Read()) COLUMNS_BOTTOM = int.Parse(xmlReader.Value); break;
                                    case "COUNT_BOTTOM": if (xmlReader.Read()) COUNT_BOTTOM = int.Parse(xmlReader.Value); break;
                                    case "ARRAYDIRCETION_BOTTOM": if (xmlReader.Read()) ARRAYDIRCETION_BOTTOM = (ARRAY_DIRECTION)int.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_X_BOTTOM": if (xmlReader.Read()) ORIGIN_OFFSET_X_BOTTOM = float.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_Y_BOTTOM": if (xmlReader.Read()) ORIGIN_OFFSET_Y_BOTTOM = float.Parse(xmlReader.Value); break;
                                    case "ORIGIN_OFFSET_Z_BOTTOM": if (xmlReader.Read()) ORIGIN_OFFSET_Z_BOTTOM = float.Parse(xmlReader.Value); break;
                                    case "SAFETY_Z_BOTTOM": if (xmlReader.Read()) SAFETY_Z_BOTTOM = int.Parse(xmlReader.Value); break;

                                    case "GAP_X_MM_BOTTOM": if (xmlReader.Read()) GAP_X_MM_BOTTOM = float.Parse(xmlReader.Value); break;
                                    case "GAP_Y_MM_BOTTOM": if (xmlReader.Read()) GAP_Y_MM_BOTTOM = float.Parse(xmlReader.Value); break;

                                    case "CLICK_BOUNDARY": if (xmlReader.Read()) CLICK_BOUNDARY = int.Parse(xmlReader.Value); break;
                                    case "IS_DISPLAY_POLARITY": if (xmlReader.Read()) IS_DISPLAY_POLARITY = bool.Parse(xmlReader.Value); break;

                                    case "DISPLAY_OFFSET_X": if (xmlReader.Read()) DISPLAY_OFFSET_X = int.Parse(xmlReader.Value); break;
                                    case "DISPLAY_OFFSET_Y": if (xmlReader.Read()) DISPLAY_OFFSET_Y = int.Parse(xmlReader.Value); break;

                                    case "POSITIONS_TOP":
                                        if (xmlReader.Read())
                                        {
                                            string[] strData = xmlReader.Value.Split(':');

                                            CellPositions_Top.Clear();
                                            CellPositions_Backup_Top.Clear();
                                            for (int i = 0; i < strData.Length; i++)
                                            {
                                                CFormatCellPosition pos = new CFormatCellPosition();
                                                for(int j = 0; j <2; j++)
                                                {
                                                    pos.SetStringData(strData[i]);
                                                }
                                                
                                                CellPositions_Top.Add(pos);
                                            }
                                            var deepList = CellPositions_Top.ConvertAll(o => new CFormatCellPosition(o.Index, o.Part, o.RESULT, o.Selected, o.ACTUAL_POS_X, o.ACTUAL_POS_Y, o.ACTUAL_POS_Z, o.Boundary, o.CHANNEL, o.Cell_Size));
                                            CellPositions_Backup_Top = deepList;
                                        }
                                        break;

                                    case "POSITIONS_BOTTOM":
                                        if (xmlReader.Read())
                                        {
                                            string[] strData = xmlReader.Value.Split(':');

                                            CellPositions_Bottom.Clear();
                                            CellPositions_Backup_Bottom.Clear();

                                            for (int i = 0; i < strData.Length; i++)
                                            {
                                                CFormatCellPosition pos = new CFormatCellPosition();
                                                for (int j = 0; j < 2; j++)
                                                {
                                                    pos.SetStringData(strData[i]);
                                                }

                                                CellPositions_Bottom.Add(pos);
                                            }
                                            var deepList = CellPositions_Bottom.ConvertAll(o => new CFormatCellPosition(o.Index, o.Part, o.RESULT, o.Selected, o.ACTUAL_POS_X, o.ACTUAL_POS_Y, o.ACTUAL_POS_Z, o.Boundary, o.CHANNEL, o.Cell_Size));
                                            CellPositions_Backup_Bottom = deepList;
                                        }
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
        public bool SaveConfig(string strRecipeName, bool bRecipePath = true)
        {
            string strPath = "";
            if (bRecipePath)
            {
                strPath = Application.StartupPath + "\\RECIPE\\" + strRecipeName + "\\" + NAME + ".xml";
            }
            else
            {
                strPath = strRecipeName;
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

                xmlWriter.WriteElementString("ROWS_TOP", ROWS_TOP.ToString());
                xmlWriter.WriteElementString("COLUMNS_TOP", COLUMNS_TOP.ToString());
                xmlWriter.WriteElementString("COUNT_TOP", COUNT_TOP.ToString());
                xmlWriter.WriteElementString("ARRAYDIRCETION_TOP", ((int)ARRAYDIRCETION_TOP).ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_X_TOP", ORIGIN_OFFSET_X_TOP.ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_Y_TOP", ORIGIN_OFFSET_Y_TOP.ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_Z_TOP", ORIGIN_OFFSET_Z_TOP.ToString());
                xmlWriter.WriteElementString("GAP_X_MM_TOP", GAP_X_MM_TOP.ToString());
                xmlWriter.WriteElementString("GAP_Y_MM_TOP", GAP_Y_MM_TOP.ToString());

                xmlWriter.WriteElementString("ROWS_BOTTOM", ROWS_BOTTOM.ToString());
                xmlWriter.WriteElementString("COLUMNS_BOTTOM", COLUMNS_BOTTOM.ToString());
                xmlWriter.WriteElementString("COUNT_BOTTOM", COUNT_BOTTOM.ToString());
                xmlWriter.WriteElementString("ARRAYDIRCETION_BOTTOM", ((int)ARRAYDIRCETION_BOTTOM).ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_X_BOTTOM", ORIGIN_OFFSET_X_BOTTOM.ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_Y_BOTTOM", ORIGIN_OFFSET_Y_BOTTOM.ToString());
                xmlWriter.WriteElementString("ORIGIN_OFFSET_Z_BOTTOM", ORIGIN_OFFSET_Z_BOTTOM.ToString());
                xmlWriter.WriteElementString("GAP_X_MM_BOTTOM", GAP_X_MM_BOTTOM.ToString());
                xmlWriter.WriteElementString("GAP_Y_MM_BOTTOM", GAP_Y_MM_BOTTOM.ToString());

                xmlWriter.WriteElementString("CLICK_BOUNDARY", CLICK_BOUNDARY.ToString());
                xmlWriter.WriteElementString("FONT_SIZE", FONT_SIZE.ToString());
                xmlWriter.WriteElementString("IS_DISPLAY_POLARITY", IS_DISPLAY_POLARITY.ToString());

                xmlWriter.WriteElementString("SAFETY_Z_TOP", SAFETY_Z_TOP.ToString());
                xmlWriter.WriteElementString("SAFETY_Z_BOTTOM", SAFETY_Z_BOTTOM.ToString());
                xmlWriter.WriteElementString("DISPLAY_OFFSET_X", DISPLAY_OFFSET_X.ToString());
                xmlWriter.WriteElementString("DISPLAY_OFFSET_Y", DISPLAY_OFFSET_Y.ToString());

                string strSumDataTop = "";
                for (int i = 0; i < CellPositions_Top.Count; i++)
                {
                    if (i != CellPositions_Top.Count - 1)
                    {
                        strSumDataTop += CellPositions_Top[i].GetStringData() + ":";
                    }
                    else
                    {
                        strSumDataTop += CellPositions_Top[i].GetStringData();
                    }
                }

                xmlWriter.WriteElementString("POSITIONS_TOP", strSumDataTop);


                string strSumDataBottom = "";
                for (int i = 0; i < CellPositions_Bottom.Count; i++)
                {
                    if (i != CellPositions_Bottom.Count - 1)
                    {
                        strSumDataBottom += CellPositions_Bottom[i].GetStringData() + ":";
                    }
                    else
                    {
                        strSumDataBottom += CellPositions_Bottom[i].GetStringData();
                    }
                }

                xmlWriter.WriteElementString("POSITIONS_BOTTOM", strSumDataBottom);

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

    public class CFormatCellPosition
    {
        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("셀 번호")]
        public int Index { get; set; } = 0;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("극성")]
        public string Part { get; set; } = "+";

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("셀 사이즈")]//20220221_SYH
        public int Cell_Size { get; set; } = 0;

        //[CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("ADDRESS")]
        //public string Address { get; set; } = "D0000";

        //[CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("ENABLE")]
        //public bool Enable { get; set; } = true;

        //[CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("IMAGE BOUNDARY")]
        public Rect Boundary  = new Rect();

        private Rect DisplayBoundary = new Rect();

        private float m_fAcutalPosX = 0.0F;
        private float m_fAcutalPosY = 0.0F;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("실제 X 위치")]
        public float ACTUAL_POS_X
        {
            get => m_fAcutalPosX;
            set
            {
                float fMovedPos = value - m_fAcutalPosX;
                MovePos(fMovedPos, 0);
            }
        }

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("실제 Y 위치")]
        public float ACTUAL_POS_Y
        {
            get => m_fAcutalPosY;
            set
            {
                float fMovedPos = value - m_fAcutalPosY;
                MovePos(0, fMovedPos);
            }
        }


        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("실제 Z 위치")]
        public float ACTUAL_POS_Z { get; set; } = 0.0F;

        [CategoryAttribute("Parameter"), DescriptionAttribute(""), DisplayNameAttribute("채널")]
        public int CHANNEL { get; set; } = 0;
        public bool Selected { get; set; } = false;
        public int RESULT { get; set; } = 0; //0 DEFAULT 1 OK 2 Ng

        public CFormatCellPosition(string strPart = "+")        
        {
            Part = strPart;
        }

        public CFormatCellPosition(int nIndex, string strPart, int nResult, bool bSelected, float ActX, float ActY, float ActZ, Rect rect, int nChannel, int intSize)
        {
            this.Index = nIndex;
            this.Part = strPart;
            this.RESULT = nResult;
            this.Selected = bSelected;
            this.ACTUAL_POS_X = ActX;
            this.ACTUAL_POS_Y = ActY;
            this.ACTUAL_POS_Z = ActZ;
            this.Boundary = rect;
            this.CHANNEL = nChannel;
            this.Cell_Size = intSize; //20220221_SYH
        }

        public void MovePos(float nOffsetX, float nOffsetY)
        {
            float nX = Boundary.X + (nOffsetX * DEFINE.IMAGE_SIZE_FACTOR);
            float nY = Boundary.Y - (nOffsetY * DEFINE.IMAGE_SIZE_FACTOR);
            float nW = Boundary.Width;
            float nH = Boundary.Height;

            m_fAcutalPosX += nOffsetX;
            m_fAcutalPosY += nOffsetY;

            Boundary = new Rect((int)nX, (int)nY, (int)nW, (int)nH);
        }

        public string GetStringData()
        {
            return $"{Index};{Boundary.X},{Boundary.Y},{Boundary.Width},{Boundary.Height};{ACTUAL_POS_X};{ACTUAL_POS_Y};{ACTUAL_POS_Z};{CHANNEL};{Part};{Cell_Size}";
        }

        public void SetStringData(string strData)
        {
            try
            {
                string[] strItems = strData.Split(';');

                if (strItems.Length == 8) //20220224_SYH
                {
                    Index = int.Parse(strItems[0]);
                    Boundary = new Rect();
                    Boundary = CConverter.StringToCVRect(strItems[1]);
                    ACTUAL_POS_X = float.Parse(strItems[2]);
                    ACTUAL_POS_Y = float.Parse(strItems[3]);
                    ACTUAL_POS_Z = float.Parse(strItems[4]);
                    CHANNEL = int.Parse(strItems[5]);
                    Part = string.Format(strItems[6]); //20220221_SYH
                    Cell_Size = int.Parse(strItems[7]); //20220221_SYH
                }
            }
            catch
            {

            }
        }        
    }
}
