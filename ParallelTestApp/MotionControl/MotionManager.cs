using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTestApp.MotionControl
{
    
    public class MotionManager
    {

        public int m_lAxisCounts = 0;                          // 제어 가능한 축갯수 선언 및 초기화
        public bool bConnected = false;

        public Machine.LiftMachine liftMachine;

        
        //#region test
        //#endregion

        #region test
        //test 1
        //test 2
        #endregion


        public MotionManager()
        {

            liftMachine = new Machine.LiftMachine();


            //FwSocket = Teaching 없음
        }

        private void OnPgExit(object sender, EventArgs e)
        {
            Console.WriteLine("MotionManager - OnPgExit");
            liftMachine.StopAuto();
            liftMachine.MachineClose();

        }
        public void AllMotorParameterSet()
        {
            liftMachine.MotorDataSet();
        }
        public void AllMotorStop()
        {

           
        }


        public bool MotionInit()
        {
            

            return true;
        }
        
        


        
    }
}
