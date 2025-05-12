using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelTestApp.Process
{
    public class LiftFlow
    {
        public CancellationTokenSource LIftCts;
        public ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);  // true면 동작 가능
        public Task<int> motorTask;
        public Task<int> UnloadTask;
        private readonly SynchronizationContext _syncContext;
        public int nTimeTick = 0;
        public int nUnloadTimeTick = 0;
        public int[] SensorSet = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] OrgOnGoing = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int waitLoadGantry = 1;     //-1 = fail , 0 = ok , 1 = wait
        private int waitLoadPusher = 1;
        private int waitUnloadTray = 1;

        private bool Init = true;
        string[] trayName = { "LEFT", "RIGHT"};
        public LiftFlow()
        {
            _syncContext = SynchronizationContext.Current;
            LIftCts = new CancellationTokenSource();
            motorTask = Task.FromResult(0);      //<--실제 실행하지않고,즉시 완료된 상태로 반환
            UnloadTask = Task.FromResult(0);      //<--실제 실행하지않고,즉시 완료된 상태로 반환
        }
        #region [LIFT 원점 동작]
        public int ReadyProcess__1(int nStep)                 //  원점(1000 ~ 2000)
        {
            int nRetStep = nStep;

            switch (nStep)
            {
                case 1000:
                    Console.WriteLine("#1 HomeProcess - 1000");
                    Console.WriteLine($"[HomeProcess__1] Step: {nRetStep}, Time: {DateTime.Now:HH:mm:ss.fff}");
                    nRetStep = 1020;
                    break;
                case 1020:
                    Console.WriteLine("#1 HomeProcess - 1020");
                    Console.WriteLine($"[HomeProcess__1] Step: {nRetStep}, Time: {DateTime.Now:HH:mm:ss.fff}");
                    nRetStep = 1040;
                    break;
                case 1040:
                    Console.WriteLine("#1 HomeProcess - 1040");
                    Console.WriteLine($"[HomeProcess__1] Step: {nRetStep}, Time: {DateTime.Now:HH:mm:ss.fff}");
                    nRetStep = 1050;
                    break;
                case 1050:
                    nRetStep = 1900;
                    break;
                case 1900:
                    Console.WriteLine("#1 HomeProcess - 1900");
                    Thread.Sleep(1000);
;                    Console.WriteLine($"[HomeProcess__1] Step: {nRetStep}, Time: {DateTime.Now:HH:mm:ss.fff}");
                    nRetStep = 2000;
                    break;
                default:
                    //[ORIGIN] STEP ERR
                    nRetStep = -1;
                    break;
            }
            return nRetStep;
        }
        public int AutoReady(int nStep)                 //  원점(1000 ~ 2000)
        {
            int nRetStep = nStep;
            bool result = false;
            switch (nStep)
            {
                case 2000:
                   
                    if (Init)
                    {
                        Console.WriteLine(" AutoReady - 1000");
                        Console.WriteLine($"[AutoReady] Step: {nRetStep}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        Init = false;
                        LIftCts?.Dispose();
                        LIftCts = new CancellationTokenSource();


                        nRetStep = 2010;
                    }
                    
                    break;
                case 2010:
                    waitUnloadTray = 1;
                    if (UnloadTask == null || UnloadTask.IsCompleted)
                    {
                        UnloadTask = Task.Run(() =>
                        {
                            waitUnloadTray = UnloadTrayLift();
                            Console.WriteLine("-------------- #1 Task - end");

                            return waitUnloadTray;
                        }, LIftCts.Token);
                        nRetStep = 2020;
                    }
                    else
                    {
                        //일시정지
                        Console.WriteLine("motorTask still running! Skip this cycle.");
                        break;
                    }
                    break;
                case 2020:
                    waitLoadGantry = 1;
                    if (motorTask == null || motorTask.IsCompleted)
                    {
                        motorTask = Task.Run(() =>
                        {
                            waitLoadGantry = LiftChange();
                            Console.WriteLine("-------------- #2 Task - end");
                            return waitLoadGantry;
                        }, LIftCts.Token);
                        Console.WriteLine("AutoReady - 1020");
                        nRetStep = 2030;
                    }
                    else
                    {
                        //일시정지
                        Console.WriteLine("motorTask still running! Skip this cycle.");
                        break;
                    }
                    break;
                case 2030:
                    if (waitUnloadTray == 1)
                    {
                        break;
                    }
                    if (waitUnloadTray < 0)
                    {
                        //갠트리 tray 공급 실패
                        MessageBox.Show($"Tray 배출 실패 = {UnloadTask.Result}");
                        Console.WriteLine($"Tray 배출 실패 = {UnloadTask.Result}");
                        break;
                    }
                    //MessageBox.Show($"Tray 배출 완료 = {UnloadTask.Result}");
                    Console.WriteLine($"Tray 배출 완료 = {UnloadTask.Result}");
                    nRetStep = 2040;
                    break;
                case 2040:
                    if(waitLoadGantry == 1)
                    {
                        break;
                    }
                    if (waitLoadGantry < 0)
                    {
                        //갠트리 tray 공급 실패
                        MessageBox.Show($"Gantry 투입 실패 = {motorTask.Result}");
                        Console.WriteLine($"Gantry 투입 실패 = {motorTask.Result}");
                        break;
                    }
                    //MessageBox.Show($"Gantry 투입 완료 = {motorTask.Result}");
                    Console.WriteLine($"Gantry 투입 완료 = {motorTask.Result}");
                    nRetStep = 2050;
                    break;
                case 2050:
                    nRetStep = 2900;
                    break;
                case 2900:
                    Console.WriteLine("AutoReady - 1900");
                    nRetStep = 2000;
                    break;
                default:
                    //[ORIGIN] STEP ERR
                    nRetStep = -1;
                    break;
            }
            return nRetStep;

        }
        #endregion
        private int LiftChange()
        {
            int rtn = -1;
            int step = 10;
            while (true)
            {
                if (LIftCts.Token.IsCancellationRequested)      //정지시 while 빠져나가는 부분
                {
                    pauseEvent.Set();   //일시정지 해제
                    Console.WriteLine("LiftChange cancelled!");
                    rtn = -1;
                    break;
                }
                if (step != 50 && step != 60)       //리프트 상승 후 터치센서에 무조건 정지해야돼서 예외 적용 
                {
                    pauseEvent.Wait();  // 일시정지시 여기서 멈춰 있음
                }

                switch (step)
                {
                    case 10:
                        Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 20;
                        nTimeTick = Environment.TickCount;
                        break;
                    case 20:
                        if (Environment.TickCount - nTimeTick > 3000)
                        {
                            Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 30;
                        }
                        break;
                    case 30:
                        Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 40;
                        break;
                    case 40:
                        nTimeTick = Environment.TickCount;
                        Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 50;
                        break;
                    case 50:
                        rtn = 0;
                        if (Environment.TickCount - nTimeTick > 3000)
                        {
                            Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 60;
                            nTimeTick = Environment.TickCount;
                            break;
                        }
                        break;
                    case 60:
                        rtn = 0;
                        if (Environment.TickCount - nTimeTick > 3000)
                        {
                            Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 100;
                            break;
                        }
                        break;
                    case 100:
                        if (Environment.TickCount - nTimeTick > 3000)
                        {
                            Console.WriteLine($"[LiftChange] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 1000;
                            break;
                        }

                        break;
                    default:
                        break;
                }

                if (step == 1000)
                {
                    break;
                }
                Thread.Sleep(100);       //TODO: while문안에서는 최소 10ms 꼭 필요
            }
            if (step == 1000)
            {
                rtn = 0;
                Console.WriteLine("LiftChange 교체 완료- end");
            }
            else
            {
                rtn = -1;
                Console.WriteLine("LiftChange 교체 실패 xxxxxxxxxx- end");
            }
            return rtn;
        }

        private int UnloadTrayLift()
        {
            int rtn = -1;
            int step = 10;
            while (true)
            {
                if (LIftCts.Token.IsCancellationRequested)      //정지시 while 빠져나가는 부분
                {
                    pauseEvent.Set();   //일시정지 해제
                    Console.WriteLine("UnloadTrayLift cancelled!");
                    rtn = -1;
                    break;
                }
                if (step != 50 && step != 60)
                {
                    pauseEvent.Wait();  // 일시정지시 여기서 멈춰 있음
                }

                switch (step)
                {
                    case 10:
                        Console.WriteLine($"[UnloadTrayLift] Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 20;
                        nUnloadTimeTick = Environment.TickCount;
                        break;
                    case 20:
                        if (Environment.TickCount - nUnloadTimeTick > 1000)
                        {
                            Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 30;

                        }
                        break;
                    case 30:
                        Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 40;
                        break;
                    case 40:
                        nUnloadTimeTick = Environment.TickCount;
                        Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                        step = 50;
                        break;
                    case 50:
                        rtn = 0;
                        if (Environment.TickCount - nUnloadTimeTick > 2000)
                        {
                            Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 60;
                            nUnloadTimeTick = Environment.TickCount;
                            break;
                        }
                        break;
                    case 60:
                        rtn = 0;
                        if (Environment.TickCount - nUnloadTimeTick > 2000)
                        {
                            Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 100;
                            break;
                        }
                        break;
                    case 100:
                        if (Environment.TickCount - nUnloadTimeTick > 2000)
                        {
                            Console.WriteLine($"[UnloadTrayLift] - Step: {step}, Time: {DateTime.Now:HH:mm:ss.fff}");
                            step = 1000;
                            break;
                        }

                        break;
                    default:
                        break;
                }

                if (step == 1000)
                {

                    break;
                }
                Thread.Sleep(100);       //TODO: while문안에서는 최소 10ms 꼭 필요
            }
            if (step == 1000)
            {
                rtn = 0;
                Console.WriteLine("UnloadTrayLift 배출 완료- end");
            }
            else
            {
                rtn = -1;
                Console.WriteLine("UnloadTrayLift 배출 실패 xxxxxxxxxx- end");
            }
            return rtn;
        }
    }
    

    
}
