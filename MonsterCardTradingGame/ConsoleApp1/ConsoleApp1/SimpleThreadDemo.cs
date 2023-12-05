using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class SimpleThreadDemo
    {
        private int counter = 0;

        public void counterFunc()
        {
            while (counter < 50)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} counts from {counter} to {counter++}");
                Thread.Sleep(100);
            }

        }

        internal static void RunThreadsDemo()
        {
            Console.WriteLine($"RunThreadsDemo started in thread {Thread.CurrentThread.ManagedThreadId}");

            Thread threadA = new Thread(new ThreadStart(std.CounterFunc));
            threadA.Start();
            Thread threadB = new Thread(std.CounterFunc);
            threadB.Start();
            Thread threadC = new Thread(() => {
                std.CounterFunc();
            });
            threadC.Start();

        }
    }
}
