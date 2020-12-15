using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace MultiThreading
{
    class Program
    {       

        #region Sample 1. Starting simple thread
        private static void SimpleThreadEntryPoint()
        {
            Console.WriteLine($"SimpleThread started");
            int i = 0;
            while (i < 10)
            {
                Console.WriteLine($"Doing some work - step {i}");
                Thread.Sleep(1000);
                ++i;
            }

            Console.WriteLine("Work done!");
        }

        private static void SimpleThreadStart()
        {
            Thread simpleThread = new Thread(SimpleThreadEntryPoint);
            simpleThread.Start();

            int i = 0;
            while (i < 10)
            {
                Console.WriteLine($"Doing some other work - step {i}");
                Thread.Sleep(500);
                ++i;
            }
        }

        #endregion

        #region Sample 2. Starting thread with parameters

        class ThreadParams
        {
            public int Count
            {
                get;
                set;
            }

            public string MessageTemplate
            {
                get;
                set;
            }
        }
        private static void SimpleThreadWithParamsEntryPoint(object args)
        {
            ThreadParams trdParams = args as ThreadParams;
            if (trdParams != null)
            {
                Console.WriteLine($"SimpleThreadWithParamsEntryPoint started");
                int i = 0;
                while (i < trdParams.Count)
                {
                    Console.WriteLine(string.Format(trdParams.MessageTemplate, i));
                    Thread.Sleep(1000);
                    ++i;
                }

                Console.WriteLine("Work done!");
            }
            else
            {
                Console.WriteLine("No parameters provided");
            }
        }

        private static void SimpleThreadWithParamsStart()
        {
            Thread simpleThread = new Thread(SimpleThreadWithParamsEntryPoint);

            ThreadParams trdParams = new ThreadParams();
            trdParams.Count = 12;
            trdParams.MessageTemplate = "Doing some work - step {0}";

            simpleThread.Start(trdParams);

            int i = 0;
            while (i < 10)
            {
                Console.WriteLine($"Doing some main thread work - step {i}");
                Thread.Sleep(1000);
                ++i;
            }
        }
        #endregion

        #region Sample 2-a. Starting thread - other 

        private static void ThreadWithRecoursion(int step)
        {
            Console.WriteLine($"Step: {step}");
            ThreadWithRecoursion(step + 1);
        }

        private static void ThreadWithStackEntryPoint()
        {
            int[] arr = new int[1000];
            ThreadWithRecoursion(1);

        }

        private static void StartWithStackSize()
        {
            Thread t = new Thread(ThreadWithStackEntryPoint, 300000); // Demostrating stack oferflow
            t.Start();
        }

        private static void StartwithThreadPool()
        {
            int procCount = Environment.ProcessorCount;
            bool maxThreadsSet = ThreadPool.SetMaxThreads(workerThreads: procCount + 1, completionPortThreads: procCount);

            int workerThreads = 0;
            int complThreads = 0;
            ThreadPool.GetAvailableThreads(out workerThreads, out complThreads);

            Console.WriteLine($"Available threads: workerThreads {workerThreads}, complThreads {complThreads}");
        }

        private static void EmptyEntryPoint(object callback)
        {
            int i = 1000;
            while(i --> 0)
            {
                Thread.Sleep(100);
            }
        }

        private static long EvaluateTimeStartWithThread(int count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int i = count;
            while(i --> 0)
            {
                var t = new Thread(EmptyEntryPoint);
                t.Start();
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long EvaluateTimeStartWithThreadPool(int count)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int i = count;
            while (i-- > 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(EmptyEntryPoint));
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static void CompareTimeThread_vs_ThreadPool()
        {
            long timeThreads = EvaluateTimeStartWithThread(1000);
            long timePool = EvaluateTimeStartWithThreadPool(1000);

            Console.WriteLine($"Threads: {timeThreads}ms vs Pool: {timePool}ms");

        }
        #endregion

        #region Sample 3. Waiting the thread
        private static void CookingEggs()
        {
            Console.WriteLine("Cooking eggs - started...");
            int i = 0;
            while (i < 5)
            {
                Console.WriteLine($"Still cooking eggs...");
                Thread.Sleep(1000);
                ++i;
            }

            Console.WriteLine("Eggs done!");

        }

        private static void FryingBacon()
        {

            Console.WriteLine("Frying bacon - started...");
            int i = 0;
            while (i < 5)
            {
                Console.WriteLine($"Still frying bacon...");
                Thread.Sleep(1000);
                ++i;
            }

            Console.WriteLine("Bacon done!");

        }

        private static void CookBreakfast()
        {
            Thread tEggs = new Thread(CookingEggs);
            Thread tBacon = new Thread(FryingBacon);

            tEggs.Start();
            tBacon.Start();

            tEggs.Join();
            tBacon.Join();

            Console.WriteLine("Breakfast ready!");
        }

        private static void CookBreakfastInHurry()
        {            
            Thread tEggs = new Thread(CookingEggs);
            Thread tBacon = new Thread(FryingBacon);

            tEggs.Start();
            tBacon.Start();

            int weCanWait = 6000;

            bool isEggsTerminated = tEggs.Join(weCanWait);
            bool isBaconTerminated = tBacon.Join(weCanWait);

            Console.WriteLine($"Breakfast ready: eggs - {isEggsTerminated}, bacon - {isBaconTerminated}");
        }

        #endregion

        #region Sample 4. Thread priorities

        private static void SomeWork(object args)
        {
            string name = (string)args;

            Console.WriteLine(name + " started");

            for (int i = 0; i < 100000; ++i)
            {
                double x = 5 * 12 + i + 36 * Math.Sqrt(144 + i);
                Thread.Sleep(1);
            }

            Console.WriteLine(name + " done!");
        }

        private static void ThreadsPrioroties()
        {
            Thread tHigh = new Thread(SomeWork);
            Thread tLow = new Thread(SomeWork);

            tHigh.Priority = ThreadPriority.AboveNormal;
            tLow.Priority = ThreadPriority.BelowNormal;

            tHigh.Start("High priority");
            tLow.Start("Low priority");

            tHigh.Join();
            tLow.Join();

            Console.WriteLine("Threads with priorities completed!");
        }

        #endregion

        #region Sample 5. Thread types
        private static void StartBackgroundThread()
        {
            Thread tBacon = new Thread(FryingBacon);
            tBacon.IsBackground = true;

            tBacon.Start();

            Thread.Sleep(1000);
        }
        #endregion

        #region Sample 6. Thread properties
        private static void ThreadPropertiesEntryPoint()
        {
            Console.WriteLine($"ThreadProperties started: {Thread.CurrentThread.Name}");

            for(int i = 0; i < 20; ++i)
            {
                Console.WriteLine($"still working - step {i}");
                Thread.Sleep(500);
            }

            Console.WriteLine("ThreadProperties stopped");
        }

        private static void ThreadProperties()
        {
            Thread tProperties = new Thread(ThreadPropertiesEntryPoint);
            tProperties.Name = "Thread with properties";

            Console.WriteLine($"Is Alive: {tProperties.IsAlive}");
            Console.WriteLine($"Is Background: {tProperties.IsBackground}");
            Console.WriteLine($"Thread ID: {tProperties.ManagedThreadId}");
            Console.WriteLine($"Thread State: {tProperties.ThreadState}");
            Console.WriteLine($"Thread Name: {tProperties.Name}");

            tProperties.Start();

            Console.WriteLine($"Is Alive: {tProperties.IsAlive}");
            Console.WriteLine($"Thread State: {tProperties.ThreadState}");

            tProperties.Join();

            Console.WriteLine($"Is Alive: {tProperties.IsAlive}");
            Console.WriteLine($"Thread State: {tProperties.ThreadState}");




        }
        #endregion

        #region Sample 7. Aborting the thread

        private static void SomeLongJob()
        {
            try
            {
                Console.WriteLine("SomeLongJob started");

                for (int i = 0; i < 1000000; ++i)
                {
                    Console.WriteLine($"still working - step {i}");
                    Thread.Sleep(500);
                }

                Console.WriteLine("SomeLongJob done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private static void AbortingThread()
        {

            Thread tLongJob = new Thread(SomeLongJob);
            tLongJob.Start();

            Thread.Sleep(1000);

            tLongJob.Abort();

            Console.WriteLine("Thread aborted");


        }

        class CancellableThreadParams
        {
            public CancellableThreadParams()
            {
                IsRunning = true;
            }

            public bool IsRunning
            {
                get;
                set;
            }
        }

        private static void SomeLongJobCancellable(object args)
        {
            try
            {
                CancellableThreadParams tParams = args as CancellableThreadParams;
                Console.WriteLine("SomeLongJob started");

                for (int i = 0; i < 1000000 && tParams.IsRunning; ++i)
                {
                    Console.WriteLine($"still working - step {i}");
                    Thread.Sleep(500);
                }

                if (tParams.IsRunning)
                {
                    Console.WriteLine("SomeLongJob done");
                }
                else
                {
                    Console.WriteLine("SomeLongJob was cancelled");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }



        private static void AbortingThreadCorrectly()
        {
            CancellableThreadParams tParams = new CancellableThreadParams();
            Thread tLongJob = new Thread(SomeLongJobCancellable);
            tLongJob.Start(tParams);

            Thread.Sleep(1000);
            tParams.IsRunning = false;
        }

        #endregion

        #region Sample 8. Synchronization - lock

        private static object simpleLock = new object();

        class SimpleSyncParams
        {
            public string AppendThis
            {
                get;
                set;
            }

            public int Count
            {
                get;
                set;
            }
        }

        private static void SimpleSyncThread(object args)
        {
            Random rnd = new Random();
            SimpleSyncParams tParams = args as SimpleSyncParams;
            for (int i = 0; i < 10; ++i)
            {
                lock (simpleLock)
                {
                    for (int c = 0; c < tParams.Count; ++c)
                    {
                        Console.Write(tParams.AppendThis); // appending next piece
                        Thread.Sleep(rnd.Next(1, 5) * 100); // waiting some random time to simulate irregularity
                    }
                }
            }
        }

        private static void SimpleSyncLock()
        {

            Thread tOne = new Thread(SimpleSyncThread);
            Thread tTwo = new Thread(SimpleSyncThread);
            Thread tThree = new Thread(SimpleSyncThread);

            SimpleSyncParams tOneParams = new SimpleSyncParams()
            {
                AppendThis = "1",
                Count = 3
            };
            SimpleSyncParams tTwoParams = new SimpleSyncParams()
            {
                AppendThis = "2",
                Count = 3
            };
            SimpleSyncParams tThreeParams = new SimpleSyncParams()
            {
                AppendThis = "3",
                Count = 3
            };

            tOne.Start(tOneParams);
            tTwo.Start(tTwoParams);
            tThree.Start(tThreeParams);

            tOne.Join();
            tTwo.Join();

            Console.WriteLine();

        }
        #endregion

        #region Sample 9-a Synchronization - AutoResetEvent
        private static AutoResetEvent aeEvent = new AutoResetEvent(false);

        private static AutoResetEvent aeEvent2 = new AutoResetEvent(true);

        private static Queue<string> queue = new Queue<string>();

        private static void SimplePublisher()
        {
            string message = string.Empty;
            for (int i = 0; i < 3 && !message.Equals("QUIT"); ++i)
            {                
                Console.Write("[P] Publisher waits - type something: ");
                message = Console.ReadLine();
                
                lock (queue)
                {
                    queue.Enqueue($"{message}");
                    aeEvent.Set();
                }
            }

            queue.Enqueue("QUIT");
            aeEvent.Set();
        }

        private static void SimpleConsumer()
        {
            string message = string.Empty;
            while (!message.Equals("QUIT"))
            {
                aeEvent.WaitOne(); 
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        message = queue.Dequeue();
                        Console.WriteLine($"[C] Message received: {message}");
                    }
                }
                
            }
        }

        private static void SimplePublisherTwoEvents()
        {
 
            string message = string.Empty;
            for (int i = 0; i < 3 && !message.Equals("QUIT"); ++i)
            {
                aeEvent2.WaitOne();
                Console.Write("[P] Publisher waits - type something: ");
                message = Console.ReadLine();

                lock (queue)
                {
                    queue.Enqueue($"{message}");                    
                }
                aeEvent.Set();
            }

            queue.Enqueue("QUIT");
            aeEvent.Set();
        }

        private static void SimpleConsumerTwoEvents()
        {
            string message = string.Empty;
            while (!message.Equals("QUIT"))
            {
                aeEvent.WaitOne();
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        message = queue.Dequeue();                        
                        Console.WriteLine($"[C] Message received: {message}");
                        aeEvent2.Set();

                    }
                }

            }
        }

        private static void SimplePublishConsume()
        {
            Thread consumer = new Thread(SimpleConsumerTwoEvents);
            Thread publisher = new Thread(SimplePublisherTwoEvents);

            consumer.Start();
            publisher.Start();
        }

        private static void SimplePulishConsumeManualReset()
        {
            Thread consumer = new Thread(SimpleConsumerTwoEvents);
            Thread publisher = new Thread(SimplePublisherTwoEvents);

            consumer.Start();
            publisher.Start();
        }

        #endregion

        #region Sample 9-b Synchronization - ManualResetEvent
        private static ManualResetEvent meEvent = new ManualResetEvent(false);


        private static void ManualResetEventThread()
        {
            int i = 10000;
            while(i --> 0)
            {
                meEvent.WaitOne();
                Console.WriteLine($"Steps left {i}");
                Thread.Sleep(10);
            }
        }

        private static void ManualResetEvent()
        {
            var t = new Thread(ManualResetEventThread);
            t.Start();

            int i = 10;
            while(i --> 0)
            {
                Thread.Sleep(1000);
                meEvent.Reset();
                Thread.Sleep(1000);
                meEvent.Set();
            }
        }


        #endregion

        #region Sample 9-c Synchronization - CountdownEvent
        private static CountdownEvent cdEvent = new CountdownEvent(5);

        private static void CountdownEventThread()
        {
            int i = 10;
            while(i --> 0)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - {i}");
                Thread.Sleep(100);
            }
            cdEvent.Signal();
        }

        private static void SampleCountdownEvent()
        {
            for(int i = 0; i < 5; ++i)
            {
                var t = new Thread(CountdownEventThread);
                t.Start();
            }

            cdEvent.Wait();

            Console.WriteLine("All threads completed");
        }

        #endregion

        #region Sample 10. Synchronization - semaphores
        static Semaphore sem = new Semaphore(3, 3);    // Capacity of 3 and 3 "empty slots"

        public static void Semaphores()
        {
            for (int i = 1; i <= 5; i++)
            {
                new Thread(SemapthorEnterThread).Start(i);
            }
        }

        static void SemapthorEnterThread(object id)
        {
            Console.WriteLine(id + " wants to enter");
            sem.WaitOne();
            Console.WriteLine(id + " is in!");           // Only three threads
            Thread.Sleep(1000 * (int)id);               // can be here at
            Console.WriteLine(id + " is leaving");       // a time.
            sem.Release();

            
        }
        #endregion

        #region Sample 11. Deadlocks

        private static List<string> log1 = new List<string>();
        private static List<int> log2 = new List<int>();

        private static object lockLog1 = new object();
        private static object lockLog2 = new object();

        private static void DeadlockThread1()
        {
            Random rnd = new Random();
            int i = 10000;
            while(i --> 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                lock (lockLog1)
                {
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                    lock(lockLog2)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                        log1.Add(text);
                        log2.Add(number);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");
                    }
                }
            }
        }

        private static void DeadlockThread2()
        {
            Random rnd = new Random();
            int i = 10000;
            while (i --> 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                lock (lockLog2)
                {
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                    lock (lockLog1)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                        log2.Add(number);
                        log1.Add(text);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");
                    }
                }
            }
        }

        private static void DeadlockExamle()
        {
            var t1 = new Thread(DeadlockThread1);
            var t2 = new Thread(DeadlockThread2);

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }

        private static void ConcurentTryEnterThread1()
        {
            Random rnd = new Random();
            int i = 10000;
            while (i-- > 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                bool lock1Taken = false;
                Monitor.TryEnter(lockLog1, ref lock1Taken);
                if (lock1Taken)
                {
                    bool lock2Taken = false;
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                    Monitor.TryEnter(lockLog2, ref lock2Taken);
                    if (lock2Taken)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                        log1.Add(text);
                        log2.Add(number);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");

                        Monitor.Exit(lockLog2);
                    }

                    Monitor.Exit(lockLog1);
                }
            }
        }

        private static void ConcurentTryEnterThread2()
        {
            Random rnd = new Random();
            int i = 10000;
            while (i-- > 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                bool lock2Taken = false;
                Monitor.TryEnter(lockLog2, ref lock2Taken);
                if (lock2Taken)
                {
                    bool lock1Taken = false;
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                    Monitor.TryEnter(lockLog1, ref lock1Taken);
                    if (lock1Taken)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                        log1.Add(text);
                        log2.Add(number);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");

                        Monitor.Exit(lockLog1);
                    }

                    Monitor.Exit(lockLog2);
                }
            }
        }

        private static void DeadlockResolutionExample1()
        {
            var t1 = new Thread(ConcurentTryEnterThread1);
            var t2 = new Thread(ConcurentTryEnterThread2);

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }

        private static void ConcurentTryEnterThread1_Wait()
        {
            int waitMs = 3000;

            Random rnd = new Random();
            int i = 10000;
            while (i-- > 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                bool lock1Taken = false;
                Monitor.TryEnter(lockLog1, waitMs, ref lock1Taken);
                if (lock1Taken)
                {
                    bool lock2Taken = false;
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                    Monitor.TryEnter(lockLog2, waitMs, ref lock2Taken);
                    if (lock2Taken)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                        log1.Add(text);
                        log2.Add(number);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");

                        Monitor.Exit(lockLog2);
                    }

                    Monitor.Exit(lockLog1);
                }
            }
        }

        private static void ConcurentTryEnterThread2_Wait()
        {
            int waitMs = 3000;

            Random rnd = new Random();
            int i = 10000;
            while (i-- > 0)
            {
                int number = rnd.Next(1, 10000);
                string text = $"Next value is: {number}";

                bool lock2Taken = false;
                Monitor.TryEnter(lockLog2, waitMs, ref lock2Taken);
                if (lock2Taken)
                {
                    bool lock1Taken = false;
                    Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 2 locked");
                    Monitor.TryEnter(lockLog1, waitMs, ref lock1Taken);
                    if (lock1Taken)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: lock 1 locked");
                        log1.Add(text);
                        log2.Add(number);
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}]: {number}");

                        Monitor.Exit(lockLog1);
                    }

                    Monitor.Exit(lockLog2);
                }
            }
        }

        private static void DeadlockResolutionExample2()
        {
            var t1 = new Thread(ConcurentTryEnterThread1_Wait);
            var t2 = new Thread(ConcurentTryEnterThread2_Wait);

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }


        #endregion
        static void Main(string[] args)
        {
            AbortingThread();
        }
    }
}
