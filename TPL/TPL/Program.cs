using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TPL
{
    class Program
    {
        #region Sync/Async I/O and CPU load examples
        // Synchronios execution
        // Call will wait till GetStringAsync returns control
        private static Task<string> GetHtmlSync()
        {
            var client = new HttpClient();

            var t = client.GetStringAsync("https://www.google.org");

            return t;
        }

        // Async call example
        // GetStringAsync returns Task object and passes control to caller
        private async static Task<string> GetHtmlAsync()
        {
            var client = new HttpClient();

            var content = await client.GetStringAsync("https://www.google.org");

            int i = 10;
            while (i-- > 0)
            {
                Console.WriteLine($"Awaiting: {i}");
                Thread.Sleep(100);
            }

            return content;
        }


        private static int DoExpensiveCalculation(int data)
        {
            int result = 0;
            for (int i = 0; i < data; ++i)
            {
                for (int j = 0; j < data; ++j)
                {
                    Console.WriteLine($"Multipl: {i} x {j} = {i * j}");
                    result = i * j;
                    Thread.Sleep(50);
                }
            }

            return data;

        }

        private static async Task<int> CalculateResult(int data)
        {
            // This queues up the work on the threadpool.
            var expensiveResultTask = Task.Run(() => DoExpensiveCalculation(data));

            // Note that at this point, you can do some other work concurrently,
            // as CalculateResult() is still executing!

            // Execution of CalculateResult is yielded here!
            var result = await expensiveResultTask;

            return result;
        }

        private static void RunExpensiveOpInParallel()
        {
            var t = CalculateResult(20);

            for (int i = 0; i < 100; ++i)
            {
                Console.WriteLine($"Main thread: {i}");
                Thread.Sleep(60);
            }
            Console.WriteLine(t.Result);
        }

        #endregion

        #region 1. - Running Tasks - Parallel.Invoke() and Task
        private static void Simple_Parallel_Invoke()
        {

            Parallel.Invoke(
                () =>
            {
                for (int i = 0; i < 100; ++i)
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                        Thread.Sleep(100);
                    }
                }
            },
                () =>
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        for (int j = 0; j < 100; ++j)
                        {
                            Console.WriteLine($"Add - {i} + {j} = {i + j}");
                            Thread.Sleep(80);
                        }
                    }
                });


        }

        // Task - class to handles the infrastructure details and provides methods and properties that are accessible 
        // from the calling thread throughout the lifetime of the task. For example, you can access the Status property of a task at any time 
        // to determine whether it has started running, ran to completion, was canceled, or has thrown an exception. 
        private static void Simple_Task_Run()
        {
            Task t = new Task(() => 
            {
                for (int i = 0; i < 10; ++i)
                {
                    for (int j = 0; j < 10; ++j)
                    {
                        Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                    }
                }
            });

            t.Start();

            for(int i = 0; i < 10; ++i)
            {
                Console.WriteLine($"Task status: {t.Status}");
            }

            t.Wait();

            Console.WriteLine($"Task status: {t.Status}");
            Console.WriteLine($"Task IsCompletedSuccessfully: {t.IsCompletedSuccessfully}");
        }

        class SomeCustomData
        {
            public int Count;
            public int LastResult;
        }

        // Creating and starting task in one operation
        // Can specify task creation options and cancellation token
        // AsyncState - can get access to task data which was pased.
        private static void Simple_TaskFactory()
        {
            var t = Task.Factory.StartNew((object obj) =>
           {
               SomeCustomData data = obj as SomeCustomData;
               if(data != null)
               {
                   for (int i = 0; i < data.Count; ++i)
                   {
                       for (int j = 0; j < data.Count; ++j)
                       {
                           Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                           data.LastResult = i * j;
                       }
                   }

               }
           }, new SomeCustomData() { Count = 100 });

            for (int i = 0; i < 100; ++i)
            {
                SomeCustomData data = t.AsyncState as SomeCustomData;
                Console.WriteLine($"Task status: {t.Status}, ID: {t.Id} last state: {data.LastResult}");
                Thread.Sleep(10);
            }

            t.Wait();

            Console.WriteLine($"Task status: {t.Status}");
            Console.WriteLine($"Task IsCompletedSuccessfully: {t.IsCompletedSuccessfully}");

        }

        private static void Simple_TaskCreationOptions()
        {
            var t = Task.Factory.StartNew((object obj) =>
            {
                SomeCustomData data = obj as SomeCustomData;
                if (data != null)
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        for (int j = 0; j < data.Count; ++j)
                        {
                            Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                            data.LastResult = i * j;
                        }
                    }

                }
            }, new SomeCustomData() { Count = 100 },
            // Specifies flags that control optional behavior for the creation and execution of tasks.
            TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);

            for (int i = 0; i < 100; ++i)
            {
                SomeCustomData data = t.AsyncState as SomeCustomData;
                Console.WriteLine($"Task status: {t.Status}, ID: {t.Id} last state: {data.LastResult}");
                Thread.Sleep(10);
            }
        }

        private static void Multiplication(object obj)
        {
            SomeCustomData data = obj as SomeCustomData;
            if (data != null)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    for (int j = 0; j < data.Count; ++j)
                    {
                        Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                        data.LastResult = i * j;
                        Thread.Sleep(50);
                    }
                }

            }
        }

        private static void MultiplicationNext(Task t, object obj)
        {
            Console.WriteLine($"Prev task result: {t.AsyncState}");
            SomeCustomData data = obj as SomeCustomData;
            if (data != null)
            {
                for (int i = 0; i < data.Count; ++i)
                {
                    for (int j = 0; j < data.Count; ++j)
                    {
                        Console.WriteLine($"Mul - {i} x {j} = {i * j}");
                        data.LastResult = i * j;
                        Thread.Sleep(50);
                    }
                }

            }
        }

        private static void Tasks_Continuations()
        {
            var t1 = Task.Factory.StartNew(Multiplication, new SomeCustomData() { Count = 5 });
            
            var t2 = t1.ContinueWith(MultiplicationNext, new SomeCustomData() { Count = 5 });

            var t3 = t2.ContinueWith(MultiplicationNext, new SomeCustomData() { Count = 5 });

            t3.Wait();


        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Hello TPL World!");
            Tasks_Continuations();
        }
    }
}
