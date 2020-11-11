using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MutexConsumer.NETFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer started");
            bool createdNew = false;
            Mutex mutex = new Mutex(initiallyOwned: false, name: "Mutex.Demo", out createdNew);

            try
            {
                int c = 10;
                while (c > 0)
                {
                    Console.WriteLine("Waiting my turn...");
                    bool bReceived = mutex.WaitOne();
                    for (int i = 0; i < 10; ++i) Console.WriteLine($"Now I wanna print {i}");
                    mutex.ReleaseMutex();
                    --c;
                }
            }
            catch(AbandonedMutexException ex)
            {
                Console.WriteLine("Mutex was abandoned - press Enter to close");
                Console.ReadLine();
            }

            mutex.Close();
        }
    }
}
