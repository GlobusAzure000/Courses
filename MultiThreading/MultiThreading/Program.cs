using System;
using System.ComponentModel;
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

        #endregion

        #region Sample 4. Thread properties

        private static void SomeWork(object args)
        {
            string name = (string)args;

            Console.WriteLine(name + " started");
            
            for(int i = 0; i < 100000; ++i)
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

        #region Sample 6. Aborting the thread

        private static void SomeLongJob()
        {
            Console.WriteLine("SomeLongJob started");

            for(int i = 0; i < 1000000; ++i)
            {
                Console.WriteLine($"still working - step {i}");
                Thread.Sleep(500);
            }

            Console.WriteLine("SomeLongJob done");

        }

        private static void AbortingThread()
        {
            try
            {
                Thread tLongJob = new Thread(SomeLongJob);
                tLongJob.Start();

                Thread.Sleep(1000);

                tLongJob.Abort();

                Console.WriteLine("Thread aborted");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion

        static void Main(string[] args)
        {
            AbortingThread();
        }
    }
}
