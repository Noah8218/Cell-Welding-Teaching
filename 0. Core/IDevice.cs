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
using System.CodeDom;

namespace IntelligentFactory
{
    public class IDevice
    {

        public CCommMelsec ComMelsec = new CCommMelsec();
        
        public IDevice()
        {

        }

        public bool Init()
        {
            try
            {
                CUtil.InitDirectory("CONFIG");
                CUtil.InitDirectory("CONFIG\\DEVICE");

                if (!ComMelsec.Init()) { CUtil.ShowMessageBox("ALARM", "Failed the Init ComMelsec"); }

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
                CUtil.InitDirectory("CONFIG");
                CUtil.InitDirectory("CONFIG\\DEVICE");
                ComMelsec.StopThreadReadInput();

                return true;
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                return false;
            }
            
        }
    }
}
