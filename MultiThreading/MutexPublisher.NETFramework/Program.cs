using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MutexPublisher.NETFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publisher started");
            bool createdNew = false;
            Mutex mutex = new Mutex(initiallyOwned: true, name: "Mutex.Demo", out createdNew);

            string command = string.Empty;
            try
            {
                while (!command.Equals("QUIT"))
                {

                    command = Console.ReadLine();
                    switch (command)
                    {
                        case "ME":
                            {
                                for (int i = 0; i < 10; ++i) Console.WriteLine($"I wanna print {i}");
                            }
                            break;
                        case "YOU":
                            mutex.ReleaseMutex();
                            mutex.WaitOne();
                            break;
                    }

                }
            }
            catch (AbandonedMutexException ex)
            {
                Console.WriteLine("Mutex was abandoned - press Enter to close");
                Console.ReadLine();
            }

            mutex.Close();
        }
    }
}
