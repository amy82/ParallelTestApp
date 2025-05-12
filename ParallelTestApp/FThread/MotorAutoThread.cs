using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTestApp.FThread
{
    public class MotorAutoThread : BaseThread
    {
        private MotionControl.MotorController parent;
        public int m_nCurrentStep = 0;
        public int m_nStartStep = 0;
        public int m_nEndStep = 0;


        //LIFT
        public int Left_CurrentStep = 0;
        public int Right_CurrentStep = 0;

        //SOCKET
        public int A_Socket_CurrentStep = 0;
        public int B_Socket_CurrentStep = 0;
        public int C_Socket_CurrentStep = 0;
        public int D_Socket_CurrentStep = 0;

        public MotorAutoThread(MotionControl.MotorController _parent)
        {
            this.parent = _parent;
            this.name = this.parent.MachineName + " MotorAutoThread";
        }


       
        private void LiftFlow()
        {
            //Magazine 원점은 같이 잡고,
            //운전 준비는 따로 해야될듯
            if (this.m_nCurrentStep >= 1000 && this.m_nCurrentStep < 2000)
            {

            }else if (this.m_nCurrentStep >= 2000 && this.m_nCurrentStep < 3000)
            {
                this.m_nCurrentStep = this.parent.processManager.liftFlow.AutoReady(this.m_nCurrentStep);

                //if (this.Left_CurrentStep >= 1000 && this.Left_CurrentStep < 2000)
                //{
                //    this.Left_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__1(this.Left_CurrentStep);
                //}
                //if (this.Right_CurrentStep >= 1000 && this.Right_CurrentStep < 2000)
                //{
                //    this.Right_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__2(this.Right_CurrentStep);
                //}
            }

        }













        private void SocketFlow()
        {
            //if (this.A_Socket_CurrentStep >= 1000 && this.A_Socket_CurrentStep < 2000)
            //{
            //    this.A_Socket_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__1(this.A_Socket_CurrentStep);
            //}

            //if (this.B_Socket_CurrentStep >= 1000 && this.B_Socket_CurrentStep < 2000)
            //{
            //    this.B_Socket_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__2(this.B_Socket_CurrentStep);
            //}

            //if (this.C_Socket_CurrentStep >= 1000 && this.C_Socket_CurrentStep < 2000)
            //{
            //    this.C_Socket_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__2(this.C_Socket_CurrentStep);
            //}

            //if (this.D_Socket_CurrentStep >= 1000 && this.D_Socket_CurrentStep < 2000)
            //{
            //    this.D_Socket_CurrentStep = this.parent.processManager.liftFlow.ReadyProcess__2(this.D_Socket_CurrentStep);
            //}
        }

        protected override void ThreadRun()
        {

            if (this.m_nCurrentStep >= this.m_nStartStep && this.m_nCurrentStep < this.m_nEndStep)
            {

                if (this.parent.MachineName == Globalo.motionManager.liftMachine.GetType().Name)
                {
                    LiftFlow();

                }


            }
            else if(this.m_nCurrentStep < 0)
            {
                
                Console.WriteLine($"{this.parent.MachineName} Process Pause");
                this.Pause();
            }
            else
            {
                //stop
                m_nStartStep = 0;
                m_nEndStep = 0;
                this.Stop();

                Console.WriteLine($"{this.parent.MachineName} Process Stop");
            }
        }
        protected override void ThreadInit()
        {
            Console.WriteLine($"{this.parent.MachineName} ThreadInit");
        }

        protected override void ThreadDestructor()
        {
            Console.WriteLine($"{this.parent.MachineName} ThreadDestructor");
        }
    }
}
