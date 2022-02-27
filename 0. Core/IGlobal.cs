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

using OpenCvSharp;
using System.Xml;
using IntelligentFactory;
using System.CodeDom;

namespace IntelligentFactory
{
    // Version 2.0.0 : Main Data Property => Global Data Class 로 변경 (Reversion)
    public class IGlobal
    {
        private static readonly Lazy<IGlobal> instance = new Lazy<IGlobal>(() => new IGlobal());

        public static IGlobal Instance
        {
            get { return instance.Value; }
        }

        private ISystem m_iSystem = null;
        public ISystem iSystem
        {
            get { return m_iSystem; }
            set { m_iSystem = value; }
        }

        private IData m_iData = null;
        public IData iData
        {
            get => m_iData;
            set => m_iData = value;
        }

        private IDevice m_iDevice = null;
        public IDevice iDevice
        {
            get => m_iDevice;
            set => m_iDevice = value;
        }

        private CSeqVision m_seqVision = null;
        public CSeqVision SeqVision
        {
            get => m_seqVision;
            set => m_seqVision = value;
        }        

        #region Event Register                
        public EventHandler<StringEventArgs> EventSystemInitStart;
        public EventHandler<StringEventArgs> EventSystemInitEnd;
        public EventHandler<StringEventArgs> EventOccuredAlarm;
        #endregion

        private IGlobal()
        {
            string strLogPath = CUtil.InitLogDirectory();
            Logger.SetPath(strLogPath);

            //Logger.Init();

            m_iSystem = new ISystem();
            m_iData = new IData();
            m_iDevice = new IDevice();                       

            Init();
        }

        public bool Init()
        {
            try
            {
                iDevice.Init();
                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                m_iSystem.Close();
                m_iDevice.Close();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
        }
    }
}
