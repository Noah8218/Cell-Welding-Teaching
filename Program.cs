using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentFactory
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]        
        static void Main()
        {
            
            Mutex mutex = new Mutex(true, "IntelligentVision", out bool bNew);
            if (bNew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {                    
                    FormInit FrmInit = new FormInit("Start the S/W");

#if TOP
                    FormMetroFrame FrmMain = new FormMetroFrame();     
#else
                    FormMetroFrame FrmMain = new FormMetroFrame();
#endif


                    FrmMain.EventInitEnd += FrmInit.OnInitEnd;
                    FrmMain.EventInit += FrmInit.OnInitEnd;

#if !DEBUG
                    FrmInit.TopMost = true;
                    FrmInit.StartPosition = FormStartPosition.CenterScreen;
                    FrmInit.Show();

                    FrmInit.Refresh();
                    System.Threading.Thread.Sleep(1);
#endif                    
                    Application.Run(FrmMain);                    
                }
                catch(Exception Desc)
                {
                    Logger.WriteLog(LOG.AbNormal, "Ex ==> {0}", Desc.Message);                    
                }

                mutex.ReleaseMutex();
            }
            else
            {
                CUtil.ShowMessageBox("Program Already Running", "Check Job Process");

                Application.Exit();
            }
        }
    }
}
