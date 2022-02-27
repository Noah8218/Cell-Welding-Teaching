using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentFactory
{

    // 실제로 사용되는 소스가 아닙니다.
    // 예제 인용을 위한 클래스입니다.
 
    class ISample
    {
        //========== 1. Invoke / BeginInvoke 를 잘 구분하여 사용하세요. ==========//

        #region INVOKE / BEGININVOKE
        /*private void OnActionInvoke(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        OnMeasEnd(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {

                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }

        // Event 발생 우선 순위가 후순위 일 떼
        // 중요하지 않은 UI 업데이트 등
        private void OnActionBeginInvoke(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        OnMeasEnd(sender, e);
                    }));
                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
            else
            {
                try
                {

                }
                catch (Exception Desc)
                {
                    Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
                }
            }
        }*/
        #endregion

        //========== 2. Thread 사용 시 , Start / Stop / Status 관리를 아래와 같이 해주세요. ==========//

        #region THREAD 사용 START / STOP / STATUS
        /*private IThreadStatus m_ThreadStatusAction = new IThreadStatus();
        public IThreadStatus ThreadStatusAction
        {
            get { return m_ThreadStatusAction; }
        }

        private void StartThreadAction()
        {
            if (m_ThreadStatusAction.IsExit())
            {
                Thread t = new Thread(new ParameterizedThreadStart(ThreadAction));
                t.Start(m_ThreadStatusAction);
            }
        }

        public void StopThreadAction()
        {
            if (!ThreadStatusAction.IsExit())
            {
                ThreadStatusAction.Stop(100);
            }
        }

        private void ThreadAction(object ob)
        {
            IThreadStatus ThreadStatus = (IThreadStatus)ob;

            ThreadStatus.Start("Start the Action");
            Logger.WriteLog(LOG.Inspection, "Start the Action");

            try
            {
                Stopwatch sw = new Stopwatch();
                while (!ThreadStatus.IsExit())
                {
                    Thread.Sleep(1);


                }

                ThreadStatus.End();
                return;

                Logger.WriteLog(LOG.SYSTEM, "[OK] {0}==>{1}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception Desc)
            {
                ThreadStatus.End();
                Logger.WriteLog(LOG.ERROR, "[FAILED] {0}==>{1} Ex ==> {2}", MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, Desc.Message);
            }
        }*/
        #endregion
    }
}
