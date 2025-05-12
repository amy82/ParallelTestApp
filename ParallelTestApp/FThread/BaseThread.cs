using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTestApp.FThread
{
    public class BaseThread
    {
        protected Thread thread;
        protected bool m_bPause = false;
        protected CancellationTokenSource cts;
        public event Action<bool> ThreadCompleted; // 쓰레드 종료 이벤트
        private bool _result;
        protected string name = "";
        protected int interval = Globalo.BASE_THREAD_INTERVAL;
        public int threadCount = 0;


        public BaseThread()
        {
            thread = null;
            cts = null;
        }

        protected virtual void ThreadInit() { }
        protected virtual void ThreadRun(){ }
        protected virtual void ThreadDestructor(){ }

        private void ProcessRun()
        {
            try
            {
                ThreadInit();
                while (!cts.Token.IsCancellationRequested)
                {
                    if (m_bPause == false)      //일시 정지가 아닐 때만 실행
                    {
                        ThreadRun();
                    }
                    Thread.Sleep(interval);
                }
                
                _result = true;
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine($"{this.name} Exception message: {0}", e.Message);
                _result = false;
            }
            finally
            {
                cts = null;
                thread = null;
            }


            Console.WriteLine($"{this.name} Thread end.");
            ThreadCompleted?.Invoke(_result); // 쓰레드 종료 시 이벤트 호출
        }
        public void Pause()
        {
            m_bPause = true;
        }
        public void Resume()
        {
            m_bPause = false;
            Console.WriteLine($"{this.name} thread Resume call");
        }

        public bool Start()
        {
            try
            {
                if (thread == null)
                {

                    cts = null;
                    cts = new CancellationTokenSource();
                    Console.WriteLine($"{this.name} Thread Start #1.");

                    threadCount++;
                    thread = new Thread(() => ProcessRun());//cts.Token));
                    thread.Start();

                    Console.WriteLine($"{this.name} Thread Start #2.");
                }
                else
                {
                    if (m_bPause == false)
                    {
                        if (thread.IsAlive)
                        {
                            Console.WriteLine($"thread.IsAlive {thread.IsAlive}");
                            return false;
                        }
                        else if (thread != null)
                        {
                            Console.WriteLine($"thread not null");
                            return false;
                        }
                        else
                        {
                            //운전준비 완료하고 thread 가 null 이 아니고 , IsAlive는 false 일때
                            bool Rtn = thread.Join(100);  // 쓰레드가 종료될 때까지 기다림
                            if (Rtn)
                            {
                                cts = null;
                                cts = new CancellationTokenSource();
                                thread = null;  // 종료 후 thread를 null로 설정
                                thread = new Thread(() => ProcessRun());// cts.Token));
                                thread.Start();
                            }
                            else
                            {
                                Abort();
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Resume();   // 정지 상태일 때만 실행
                        return true;
                    }
                }
                
                
            }
            catch (ThreadStateException ex)
            {
                Console.WriteLine($"[ERR] {this.name} ThreadStateException: {ex.Message}");
                return false;
            }
            return true;
        }
        public void Stop()
        {
            if (thread != null && cts != null)
            {
                //Console.WriteLine($"{this.name} Thread Stop() #1");
                
                cts.Cancel();
                m_bPause = false;       //일시정지 해제 cts.Cancel 보다 m_bPause를 먼저하면 ThreadRun 에서 일시 정지로 빠진다.

                //Console.WriteLine($"{this.name} Thread Stop() #End");
            }
        }
        public void Close()     //프로그램 종료시에만 호출
        {
            Stop();

            if (thread != null && cts != null)
            {
                Console.WriteLine($"{this.name} Thread Close() #1");
                bool Rtn = thread.Join(1000);  // 쓰레드가 종료될 때까지 기다림
                if (Rtn == false)
                {
                    Abort();
                    Console.WriteLine($"{this.name} Thread Close() Abort #2");
                }
                Console.WriteLine($"{this.name} Thread Close() #End [{this.interval}]");
            }
        }
        public bool GetThreadRun()
        {
            if (thread != null)
            {
                //Console.WriteLine($"{this.name} GetThreadRun() : {thread.IsAlive}");

                return thread.IsAlive;    //thread 동작 중
            }
            return false;
            //return thread?.IsAlive ?? false;  // thread가 null이면 false 반환
        }

        public bool GetThreadPause()
        {
            return m_bPause;
        }
        
        private void Abort()
        {
            thread.Abort();
            Console.WriteLine($"{this.name} thread Abort call");
        }
    }
}
