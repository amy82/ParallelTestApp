using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTestApp.Machine
{
    public enum eLift : int
    {
        LIFT_L_Z = 0, LIFT_R_Z, GANTRYX_L, GANTRYX_R
    };

    public enum eLiftSensor : int
    {
        LIFT_TOPSTOP_POS = 0, LIFT_READY_POS, LIFT_HOME_POS
    };
    public class LiftMachine : MotionControl.MotorController
    {
        private Thread socketThread;
        private ConcurrentQueue<string> socketCommandQueue = new ConcurrentQueue<string>();
        private AutoResetEvent socketEvent = new AutoResetEvent(false);
        private bool running = true;
        public int MotorCnt { get; private set; } = 4;

        //LEFT Z / RIGHT Z / GANTRY FRONT X / GANTRY BACK X / 

        public string[] axisName = { "FRONT_X", "BACK_X", "LEFT_Z", "RIGHT_Z"};


        private static double[] MaxSpeeds = { 100.0, 100.0, 200.0, 200.0};
        private double[] OrgFirstVel = { 20000.0, 20000.0, 20000.0, 20000.0};
        private double[] OrgSecondVel = { 5000.0, 5000.0, 10000.0, 10000.0};
        private double[] OrgThirdVel = { 2500.0, 2500.0, 5000.0, 5000.0};

        public bool[] IsLiftOnTray = { false, false };      //리프트 위 Tray 유무 확인
        public bool[] IsTopLoadOnTray = { false, false };      //상단 Gantry , Pusher Tray 로드 확인

        public bool IsLoadingInputTray = false;         //투입 LIft 투입중
        public bool IsUnloadingOutputTray = false;      //배출 Lift 배출중

        public enum eTeachingPosList : int
        {
            WAIT_POS = 0, LOAD_POS, UNLOAD_POS, TOTAL_LIFT_TEACHING_COUNT
        };

        public string[] TeachName = { "WAIT_POS" , "LOAD_POS", "UNLOAD_POS" };


        public LiftMachine()// : base("LiftModule")
        {
            int i = 0;
            this.MachineName = this.GetType().Name;



        }
        private void SocketWorker()
        {

        }
        public override bool TaskSave()
        {
            return true;
        }
        public override void MotorDataSet()
        {



        }
        public override bool IsMoving()
        {
            if (AutoUnitThread.GetThreadRun() == true)
            {
                return true;
            }


            return true;
        }
        public override void StopAuto()
        {
            Console.WriteLine($"[INFO]---- Lift Run Stop---");
            if (processManager.liftFlow.LIftCts != null && !processManager.liftFlow.LIftCts.IsCancellationRequested)
            {
                processManager.liftFlow.LIftCts.Cancel();       //<-------Task.Run 종료
                processManager.liftFlow.pauseEvent.Set();       //다시 자동시 진행되게 Set으로 변경
            }
                
            AutoUnitThread.Stop();
            MovingStop();
            

        }

        public override void MovingStop()
        {
            if (CancelToken != null && !CancelToken.IsCancellationRequested)
            {
                CancelToken.Cancel();
            }


        }
        public override bool OriginRun()
        {
            
            return true;
        }
        public override bool ReadyRun()
        {
            Console.WriteLine($"motorTask.Status : {this.processManager.liftFlow.motorTask.Status}");

            if (AutoUnitThread.GetThreadRun() == true)
            {
                if (AutoUnitThread.GetThreadPause() == true)        //일시 정지 상태인지 확인
                {
                    if (this.processManager.liftFlow.motorTask != null && 
                        this.processManager.liftFlow.motorTask.IsCompleted == false)
                    {
                        //MessageBox.Show("motorTask 동작중입니다.");
                        //Task 동작중인데, 일시정지가 아니면 return
                        bool isSet = processManager.liftFlow.pauseEvent.IsSet;      //일시정지 체크
                        if (isSet)
                        {
                            //isSet= true진행중
                            //isSet= false 일시정지중
                            Console.WriteLine($"Task 자동 운전 중입니다. {isSet}");
                            return false;
                        }
                        
                    }

                    AutoUnitThread.m_nCurrentStep = Math.Abs(AutoUnitThread.m_nCurrentStep);
                    AutoUnitThread.Resume();                            //thread 재개
                    processManager.liftFlow.pauseEvent.Set();          //<-------Task.Run 일시정지 재개

                    //RunState = OperationState.AutoRunning;
                    Console.WriteLine($"자동운전 재개");
                    return true;
                }
                MessageBox.Show("자동 운전 중입니다.");
                Console.WriteLine($"자동 운전 중입니다. {this.processManager.liftFlow.motorTask.Status}");
                return true;
            }
            
            
            
            AutoUnitThread.m_nCurrentStep = 2000;
            AutoUnitThread.m_nEndStep = 3000;
            AutoUnitThread.m_nStartStep = AutoUnitThread.m_nCurrentStep;


            AutoUnitThread.Left_CurrentStep = 1000;
            AutoUnitThread.Right_CurrentStep = 1000;

            
            bool rtn = AutoUnitThread.Start();
            if (rtn)
            {
                Console.WriteLine($"[READY] LIFT Ready Start");
                Console.WriteLine($"모터 동작 성공.");
            }
            else
            {
                Console.WriteLine($"[READY] LIFT Ready Start Fail");
                Console.WriteLine($"모터 동작 실패.");
            }


            return true;
        }
        public override void PauseAuto()
        {
            Console.WriteLine($"[INFO]---- Lift 일시정지---");
            if (AutoUnitThread.GetThreadRun() == true)
            {
                processManager.liftFlow.pauseEvent.Reset(); //<-------Task.Run 일시정지
                AutoUnitThread.Pause();
                //RunState = OperationState.Paused;
            }

            return;
        }
        public override bool AutoRun()
        {
            bool rtn = true;
           
            return rtn;
        }

    }
}
