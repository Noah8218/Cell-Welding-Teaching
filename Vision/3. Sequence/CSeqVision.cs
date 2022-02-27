using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;

#if MATROX
using Matrox.MatroxImagingLibrary;
#endif

namespace IntelligentFactory
{
    public class CSeqVision
    {
        private IGlobal Global = IGlobal.Instance;
        public EventHandler<InspResultArgs> EventInspResult;

#region Thread
        public IThreadStatus ThreadStatusVision = new IThreadStatus();

        public void StartThreadVision()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadVision));
            t.Start(ThreadStatusVision);
        }

        public void ResetThreadVision()
        {
            ThreadStatusVision.End();
        }

        public void StopThreadVision()
        {
            if (!ThreadStatusVision.IsExit())
            {
                ThreadStatusVision.Stop(100);
            }

            ResetThreadVision();
        }

        private void ThreadVision(object ob)
        {
            IThreadStatus ThreadStatus = (IThreadStatus)ob;

            ThreadStatus.Start("Vision Inspection");
            Logger.WriteLog(LOG.Normal, "Vision Inspection");

            try
            {
                while (!ThreadStatus.IsExit())
                {
                    
                }
            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}/{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }

        }

        public async void RunInspByTask(Mat imageSource)
        {
            Task taskInspection = Task.Run(() => RunInsp(imageSource));
        }


        private void RunInsp(Mat imageSource)
        {
            try
            {

            }
            catch (Exception Desc)
            {
                Logger.WriteLog(LOG.AbNormal, "[FAILED] {0}==>{1}   Execption ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }
        #endregion
    }
}
