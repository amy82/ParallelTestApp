using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTestApp.MotionControl
{
    //class MotionBase
    public abstract class MotorController
    {
        public FThread.MotorAutoThread AutoUnitThread;
        //protected FThread.MotorManualThread motorManualThread;
        
        public Process.ProcessManager processManager;
        public string MachineName { get; protected set; }
        protected CancellationTokenSource CancelToken;      //TODO: 사용안하는듯?
        public bool MotorUse = true;
        ////protected bool isMotorBusy = false; //실행중 체크용 플래그
        //abstract : 추상 메서드
        //반드시 자식 클래스에 구현해야 함, 내용없이 선언가능, 강제 특정 메서드 오버라이딩 유도
        //추상 클래스 안에서만 사용가능 (abstract class)

        //virtual : 가상 메서드
        //기본 구현을 제공하지만, 필요하면 자식 클래스에서 재정의 가능
        //오버라이딩(재정의)하지 않아도 사용가능
        //선택적으로 변경할수 있도록 유도


        public MotorController()//string name
        {
            AutoUnitThread = new FThread.MotorAutoThread(this);        //TODO: MotorAutoThread 쓰레드 종료하는 거 추가해야된다.
            //motorManualThread = new FThread.MotorManualThread(this);

            processManager = new Process.ProcessManager();

            CancelToken = new CancellationTokenSource();
        }

        public abstract void StopAuto();
        public abstract bool AutoRun();
        public abstract void PauseAuto();

        public abstract bool OriginRun();
        public abstract bool ReadyRun();

        public abstract bool IsMoving();
        public abstract void MovingStop();
        public abstract void MotorDataSet();
        public abstract bool TaskSave();


        public virtual void MachineClose()
        {
            AutoUnitThread.Close();
        }

    }
}
