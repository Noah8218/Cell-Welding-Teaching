using ActUtlTypeLib;
using ACTMULTILib;
using IntelligentFactory;
//using IntelligentFactory.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;

using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Security.Policy;

namespace IntelligentFactory
{
    public class CCommMelsec
    {
        #region Event Register        
        public EventHandler<EventArgs> EventChangedConnection;
        public EventHandler<EventArgs> EventReadEnd;
        #endregion


        //private ActUtlType plc;
        public ActEasyIF m_actType = new ActEasyIF();

        public bool IsOpen { get; set; } = false;

        #region IO        


        #region  Inspection Welding
        List<CSignal> ListInput = new List<CSignal>();
        List<CSignal> ListInput2 = new List<CSignal>();

        #endregion

        public CSignal DI_TOP_MODEL_UPDATE = null;
        public CSignal DI_BOTTOM_MODEL_UPDATE = null;

        public CSignal DI_TOP_LAMP_UPDATE = null;
        public CSignal DI_BOTTOM_LAMP_UPDATE = null;

        public CSignal DI_TOP_CALL_NUMBER = null;
        public CSignal DI_BOTTOM_CALL_NUMBER = null;

        public CSignal DI_TOP_LAMP_NUMBER = null;
        public CSignal DI_BOTTOM_LAMP_NUMBER = null;

        public CSignal DI_TOP_LAMP = null;
        public CSignal DI_BOTTOM_LAMP = null;

        public CSignal DI_DISPLAY_UPDATE = null;

        public CSignal DO_MODEL_NO = null;
        public CSignal DO_PC_READY = null;
        public CSignal DO_MODEL_NAME = null;
        public CSignal DO_TOP_POINT_TOTAL_COUN5T = null;
        public CSignal DO_BOTTOM_POINT_TOTAL_COUN5T = null;

        public CSignal DO_TOP_X_CALL_POSITION = null;
        public CSignal DO_TOP_Y_CALL_POSITION = null;
        public CSignal DO_TOP_Z_CALL_POSITION = null;
        public CSignal DO_TOP_CALL_CH = null;
        public CSignal DO_TOP_POINT_CALL_NUMBER = null;
        public CSignal DO_TOP_SAFETY_Z = null;

        public CSignal DO_BOTTOM_X_CALL_POSITION = null;
        public CSignal DO_BOTTOM_Y_CALL_POSITION = null;
        public CSignal DO_BOTTOM_Z_CALL_POSITION = null;
        public CSignal DO_BOTTOM_CALL_CH = null;
        public CSignal DO_BOTTOM_POINT_CALL_NUMBER = null;
        public CSignal DO_BOTTOM_SAFETY_Z = null;
        #endregion

        public CCommMelsec()
        {

        }

        public bool InitIO(int nLogicalNumber)
        {
            try
            {   
                m_actType.ActLogicalStationNumber = nLogicalNumber;
                ListInput.Clear();
                ListInput2.Clear();
                // Input                
                DI_TOP_MODEL_UPDATE = new CSignal("DI_TOP_MODEL_UPDATE", "1404", CSignal.DEV_TYPE.LB, 1);
                ListInput2.Add(DI_TOP_MODEL_UPDATE);

                DI_BOTTOM_MODEL_UPDATE = new CSignal("DI_BOTTOM_MODEL_UPDATE", "1406", CSignal.DEV_TYPE.LB, 1);
                ListInput2.Add(DI_BOTTOM_MODEL_UPDATE);

                DI_TOP_LAMP_UPDATE = new CSignal("DI_TOP_LAMP_UPDATE", "1408", CSignal.DEV_TYPE.LB, 1);
                ListInput2.Add(DI_TOP_LAMP_UPDATE);

                DI_BOTTOM_LAMP_UPDATE = new CSignal("DI_BOTTOM_LAMP_UPDATE", "1410", CSignal.DEV_TYPE.LB, 1);
                ListInput2.Add(DI_BOTTOM_LAMP_UPDATE);

                DI_TOP_CALL_NUMBER = new CSignal("DI_TOP_CALL_NUMBER", "4034", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_TOP_CALL_NUMBER);

                DI_BOTTOM_CALL_NUMBER = new CSignal("DI_BOTTOM_CALL_NUMBER", "4036", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_BOTTOM_CALL_NUMBER);

                DI_TOP_LAMP_NUMBER = new CSignal("DI_TOP_LAMP_NUMBER", "4042", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_TOP_LAMP_NUMBER);

                DI_BOTTOM_LAMP_NUMBER = new CSignal("DI_BOTTOM_LAMP_NUMBER", "4044", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_BOTTOM_LAMP_NUMBER);

                DI_TOP_LAMP = new CSignal("DI_TOP_LAMP", "4050", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_TOP_LAMP);

                DI_BOTTOM_LAMP = new CSignal("DI_BOTTOM_LAMP", "4052", CSignal.DEV_TYPE.LW, 2);
                ListInput.Add(DI_BOTTOM_LAMP);

                DI_DISPLAY_UPDATE = new CSignal("DI_DISPLAY_UPDATE", "4120", CSignal.DEV_TYPE.LB, 1);
                ListInput.Add(DI_DISPLAY_UPDATE);

                // Output
                DO_MODEL_NO = new CSignal("DO_MODEL_NO", "1400", CSignal.DEV_TYPE.LW);

                DO_PC_READY = new CSignal("DO_PC_READY", "1402", CSignal.DEV_TYPE.LW);

                DO_MODEL_NAME = new CSignal("DO_MODEL_NAME", "4000", CSignal.DEV_TYPE.LW, 20);                               

                DO_TOP_X_CALL_POSITION = new CSignal("DO_TOP_X_CALL_POSITION", "4100", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_Y_CALL_POSITION = new CSignal("DO_TOP_Y_CALL_POSITION", "4102", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_Z_CALL_POSITION = new CSignal("DO_TOP_Z_CALL_POSITION", "4104", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_CALL_CH = new CSignal("DO_TOP_CALL_CH", "4106", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_POINT_CALL_NUMBER = new CSignal("DO_TOP_POINT_CALL_NUMBER", "4108", CSignal.DEV_TYPE.LW, 2);                

                DO_BOTTOM_X_CALL_POSITION = new CSignal("DO_BOTTOM_X_CALL_POSITION", "4110", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_Y_CALL_POSITION = new CSignal("DO_BOTTOM_Y_CALL_POSITION", "4112", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_Z_CALL_POSITION = new CSignal("DO_BOTTOM_Z_CALL_POSITION", "4114", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_CALL_CH = new CSignal("DO_BOTTOM_CALL_CH", "4116", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_POINT_CALL_NUMBER = new CSignal("DO_BOTTOM_POINT_CALL_NUMBER", "4118", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_POINT_TOTAL_COUN5T = new CSignal("DO_TOP_POINT_TOTAL_COUN5T", "4030", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_POINT_TOTAL_COUN5T = new CSignal("DO_BOTTOM_POINT_TOTAL_COUN5T", "4032", CSignal.DEV_TYPE.LW, 2);

                DO_TOP_SAFETY_Z = new CSignal("DO_TOP_SAFETY_Z", "4038", CSignal.DEV_TYPE.LW, 2);

                DO_BOTTOM_SAFETY_Z = new CSignal("DO_BOTTOM_SAFETY_Z", "4040", CSignal.DEV_TYPE.LW, 2);

                return true;
            }
            catch (Exception Desc)
            {

                return false;
            }
        }

        public bool Init()
        {
            try
            {
                InitIO(0);

                if (!PingTest())
                {
                    IsOpen = false;
                    return false;
                }

                if (!m_actType.Open().Equals(0))
                {
                    IsOpen = false;
                    return false;
                }

                IsOpen = true;

                //WriteB(DO_PC_READY.Name, DO_PC_READY.Address, 1);

                //StartThreadReadInput();
                //StartThreadWrite();

                // 로그
                return true;
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool PingTest()
        {
            Ping pingSender = new Ping();
            int nTimeout = 1000;
            // IP를 따로 빼야함

            // plc 서버 ip 필요
            // 하드코딩된 ip 전부 레시피로 빼기
            // 적용
            PingReply reply = pingSender.Send("192.168.100.102", nTimeout);

            if (reply.Status == IPStatus.Success)
            {
                Logger.WriteLog(LOG.Normal, "[OK] - {0}", "PLC Server Ping TEST");
                return true;
            }
            // IP를 따로 빼야함

            Logger.WriteLog(LOG.AbNormal, "[Fail] - {0}", "PLC Server Ping TEST");
            return false;
        }

        public bool Close()
        {
            try
            {
                if (IsOpen)
                {
                    m_actType.Close();
                    StopThreadReadInput();
                }

                //Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }

        }
        public bool OutputClear()
        {
            try
            {
                if (IsOpen)
                {

                    //WriteB(OutputLeftJuge.Name, OutputLeftJuge.Address, 0);
                    //WriteB(OutputLeftResultJuge.Name, OutputLeftResultJuge.Address, 0);
                    //WriteB(OutputRightComple.Name, OutputRightComple.Address, 0);
                    //WriteB(OutputRightJuge.Name, OutputRightJuge.Address, 0);
                }

                //  Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return true;
            }
            catch (Exception Desc)
            {
                // Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
        public string ReadBCR(int nAddr, int nSize = 20)
        {
            try
            {
                Thread.Sleep(10);
                int nAddress = 0;
                nAddress = nAddr;

                short[] sarrReadData = new short[20];
                string strBarcode = "";

                int[] nArrayValue = new int[40];
                int nRet = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), 20, out nArrayValue[0]);

                if (nRet == 0)
                {
                    for (int i = 0; i < nArrayValue.Length; i++)
                    {
                        byte[] bytes = BitConverter.GetBytes(nArrayValue[i]);
                        string str = Encoding.ASCII.GetString(bytes);

                        str = str.Replace("\0", "");
                        strBarcode += str;
                    }

                }

                Console.WriteLine(strBarcode);
                // Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return strBarcode;
            }
            catch (Exception Desc)
            {
                //   Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "BCR Parsing Error";
            }
        }

        public string ReadBCR(CSignal signal, int nSize = 20)
        {
            try
            {
                Thread.Sleep(1);
                int nAddress = 0;
                nAddress = int.Parse(signal.ADDRESS);

                short[] sarrReadData = new short[20];
                string strBarcode = "";

                int[] nArrayValue = new int[40];
                int nRet = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), 20, out nArrayValue[0]);

                if (nRet == 0)
                {
                    for (int i = 0; i < nArrayValue.Length; i++)
                    {
                        byte[] bytes = BitConverter.GetBytes(nArrayValue[i]);
                        string str = Encoding.ASCII.GetString(bytes);

                        str = str.Replace("\0", "");
                        strBarcode += str;
                    }

                }

                Console.WriteLine(strBarcode);
                // Logger.WriteLog(LOG.Normal, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);
                return strBarcode;
            }
            catch (Exception Desc)
            {
                //  Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return "BCR Parsing Error";
            }
        }
        public short[] IntToShort(int[] nArray)
        {
            short[] shArray = new short[nArray.Length * 2];

            for (int i = 0; i < nArray.Length; i += 2)
            {
                shArray[i] = (short)nArray[i];
                shArray[i + 1] = (short)(nArray[i] >> 16);
            }

            return shArray;
        }

        public short[] IntToShort2(int[] nArray)
        {
            short[] shArray = new short[nArray.Length * 2];

            for (int i = 0; i < nArray.Length; i += 2)
            {
                shArray[i] = (short)nArray[i];
                shArray[i + 1] = (short)(nArray[i] >> 16);
            }

            return shArray;
        }

        object m_ob = new object();

        public int WriteArray(string strStartAddress, int[] nArrayData, int nSize)
        {
            try
            {
                lock(m_ob)
                {
                    Stopwatch swWrite = new Stopwatch();

                    int nAddress = int.Parse(strStartAddress);
                    short[] sBuf = IntToShort(nArrayData);
                    int ret = -1;

                    string strAddr = "";
                    for (int i = 0; i < nSize; i++)
                    {
                        strAddr += (string.Format("D{0}", nAddress + i) + "\n");
                    }
                    swWrite.Start();

                    int nLength = nArrayData.Length;
                    if (nArrayData.Length > 10) nLength = 10;
                    ret = m_actType.WriteDeviceRandom(strAddr, nArrayData.Length, ref nArrayData[0]);

                    swWrite.Stop();
                    return ret;
                }                               
            }
            catch (Exception Desc)
            {
                //  Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteB(string strName, string strAddress, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(strAddress);

                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                if (ret == 0)
                {
                    //   Logger.WriteLog(LOG.IO, "[{0}] ==> {1}", strName, nData);
                }
                else
                {
                    //  Logger.WriteLog(LOG.AbNormal, "[{0}] ==> FAIL", strName, nData);
                }

                return ret;
            }
            catch (Exception Desc)
            {// Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteWord(CSignal signal, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(signal.ADDRESS);

                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                if (ret == 0)
                {
                    // Logger.WriteLog(LOG.IO, "[{0}] ==> {1}", signal.Name, nData);
                }
                else
                {
                    // Logger.WriteLog(LOG.AbNormal, "[{0}] ==> FAIL", signal.Name, nData);
                }

                return ret;
            }
            catch (Exception Desc)
            {
                //Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int WriteD(string strAddress, int nData, int nSize = 2)
        {
            try
            {
                int nAddress = int.Parse(strAddress);
                int ret = m_actType.WriteDeviceBlock(string.Format("D{0}", nAddress), nSize, nData);

                return ret;
            }
            catch (Exception Desc)
            {
                //  Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }

        public int ReadD(string strAddress, out int nData, int nSize = 1)
        {
            nData = -1;
            try
            {
                int nAddress = int.Parse(strAddress);
                int ret = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), nSize, out nData);

                return ret;
            }
            catch (Exception Desc)
            {
                //  Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return -1;
            }
        }


        public int ReadDW(string strLowAddress, string strHighAddress, out double dValue, int nSize = 1)
        {
            try
            {
                dValue = -1;
                int nValueLow = 0;
                int nValueHigh = 0;

                int nLowValue = 0;
                int nHighValue = 0;

                int ret = m_actType.ReadDeviceBlock(strLowAddress, nSize, out nValueLow);
                nLowValue = (int)nValueLow;

                if (ret != 0)
                {
                    return ret;
                }

                ret = m_actType.ReadDeviceBlock(strHighAddress, nSize, out nValueHigh);
                nHighValue = nValueHigh << 16;

                if (ret != 0)
                {
                    return ret;
                }

                dValue = nLowValue + nHighValue;
                return ret;
            }
            catch (Exception Desc)
            {
                // Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                dValue = -1;
                return -1;
            }
        }

        public int ReadB(string strAddress, out int nData, int nSize = 1)
        {
            nData = -1;
            try
            {
                int nAddress = int.Parse(strAddress);
                int ret = m_actType.ReadDeviceBlock(string.Format("D{0}", nAddress), nSize, out nData);

                return ret;
            }
            catch (Exception Desc)
            {
                return nData;
            }
        }

        private Stopwatch m_Timeout = new Stopwatch();
        public Stopwatch PlcConnectionTimeout
        {
            get => m_Timeout;
            set
            {
                m_Timeout = value;
            }
        }

        #region Thread                    
        private IThreadStatus m_ThreadStatusReadInput = new IThreadStatus();
        public IThreadStatus ThreadStatusReadInput
        {
            get { return m_ThreadStatusReadInput; }
        }

        public void StartThreadReadInput()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadReadInput));
            t.Start(m_ThreadStatusReadInput);
        }

        public void StopThreadReadInput()
        {
            if (!ThreadStatusReadInput.IsExit())
            {
                ThreadStatusReadInput.Stop(100);
            }
        }


        bool bIsAlive = false;
        int m_nAliveTime = 0;

        private const int START_ADDRESS = 4012;
        private const int COUNT_ADDRESS = 150;

        private const int START_ADDRESS2 = 4000;
        private const int COUNT_ADDRESS2= 11;

        private void ThreadReadInput(object ob)
        {
            IThreadStatus ThreadStatus = (IThreadStatus)ob;

            ThreadStatus.Start("Read Input");
            Logger.WriteLog(LOG.Inspection, "Read the Input Signal");

            int nPrevSum = 0;
            int nCurrSum = 0;
            Stopwatch sw = new Stopwatch();
            while (!ThreadStatus.IsExit())
            {
                Thread.Sleep(1);
                if (IsOpen)
                {
                    try
                    {
                        if (IGlobal.Instance.iSystem.Mode == ISystem.MODE.AUTO)
                        {
                            if (!PlcConnectionTimeout.IsRunning)
                            {
                                PlcConnectionTimeout.Start();
                                // Write: 30~50ms

                                WriteD(DO_PC_READY.ADDRESS, 1);
                            }

                            if (PlcConnectionTimeout.ElapsedMilliseconds >= 500)
                            {
                                WriteD(DO_PC_READY.ADDRESS, 1);
                                PlcConnectionTimeout.Restart();
                            }
                        }

                        sw.Start();
                        nCurrSum = 0;

                        short[] shArrayValue = new short[COUNT_ADDRESS];
                        short[] shArrayValue2 = new short[COUNT_ADDRESS2];
                        int ret = -1;

                        ret = m_actType.ReadDeviceBlock2(string.Format("D{0}", START_ADDRESS), COUNT_ADDRESS, out shArrayValue[0]);
                        ret = m_actType.ReadDeviceBlock2(string.Format("D{0}", START_ADDRESS2), COUNT_ADDRESS2, out shArrayValue2[0]);

                        for (int i = 0; i < ListInput2.Count; i++)
                        {
                            CSignal InputSignal = ListInput2[i];
                            string strAddr = InputSignal.ADDRESS;

                            int nWordCount = InputSignal.WordCount;
                            int nValueW = 0;                

                            int nAddr = int.Parse(strAddr);
                            nAddr = nAddr - START_ADDRESS2;

                            if (nWordCount == 1)
                            {
                                nValueW = shArrayValue2[nAddr];
                            }
                            else
                            {
                                for (int j = 0; j < nWordCount; j++)
                                {
                                    int nValueTemp = shArrayValue2[nAddr + j];

                                    if (j != 0)
                                    {
                                        nValueTemp = nValueTemp << 16;
                                    }

                                    nValueW += nValueTemp;
                                }
                            }

                            if (ret == 0)
                            {
                                InputSignal.Current = nValueW;
                            }
                            else
                            {
                                Logger.WriteLog(LOG.AbNormal, "[FAILED] Read Input {0}==>{1} Err Code ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ret);
                            }
                        }

                        for (int i = 0; i < ListInput.Count; i++)
                        {
                            CSignal InputSignal = ListInput[i];
                            string strAddr = InputSignal.ADDRESS;

                            int nWordCount = InputSignal.WordCount;
                            int nValueW = 0;

                            //if (InputSignal.Name == "DI_MODEL_UPDATE" || InputSignal.Name == "DI_LAMP_UPDATE")
                            //{
                            //    ReadB(InputSignal.Address, out nValueW);
                            //    InputSignal.Current = nValueW;
                            //    continue;
                            //}

                            int nAddr = int.Parse(strAddr);
                            nAddr = nAddr - START_ADDRESS;

                            if (nWordCount == 1)
                            {
                                nValueW = shArrayValue[nAddr];
                            }
                            else
                            {
                                for (int j = 0; j < nWordCount; j++)
                                {
                                    int nValueTemp = shArrayValue[nAddr + j];

                                    if (j != 0)
                                    {
                                        nValueTemp = nValueTemp << 16;
                                    }

                                    nValueW += nValueTemp;
                                }
                            }

                            if (ret == 0)
                            {
                                InputSignal.Current = nValueW;
                            }
                            else
                            {
                                Logger.WriteLog(LOG.AbNormal, "[FAILED] Read Input {0}==>{1} Err Code ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, ret);
                            }
                        }
                        sw.Stop();

                        string str = sw.ElapsedMilliseconds.ToString();

                        sw.Restart();
                    }
                    catch (Exception Desc)
                    {
                        Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                    }
                }
            }
        }
        #endregion

    }
}
