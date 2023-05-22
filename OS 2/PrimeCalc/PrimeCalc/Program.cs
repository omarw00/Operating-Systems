using System;
using System.Threading;
using System.IO;
namespace PrimeCalc
{
    class Program
    {
        public bool IsPrimehelp(long num)
        {
            if (num < 2)
            {
                return false;
            }
            for (long i = 2; i <= num / 2; i++)
            {
                if (num % i == 0)
                {
                    return false;
                }

            }
            return true;
        }
        public void PrimeCalchelp(long n, long m)
        {
            for (long i = n; i < m; i++)
            {
                if (IsPrimehelp(i) == true)
                {
                    Write("Thread[" + Thread.CurrentThread.ManagedThreadId + "]: " + i);
                    //Console.WriteLine("Thread[" + Thread.CurrentThread.ManagedThreadId + "]: " + i);
                }
            }
        }
        public static ReaderWriterLockSlim RWlock = new ReaderWriterLockSlim();

        public void Write(String text)
        {
            RWlock.EnterWriteLock();
            try
            {
                if (File.Exists("primes-output.txt"))
                {
                    using (StreamWriter writer = File.AppendText("primes-output.txt"))
                    {
                        writer.WriteLine(text);
                        writer.Close();
                    }
                }
                else
                {
                    using (StreamWriter writer = File.CreateText("primes-output.txt"))
                    {
                        writer.WriteLine(text);
                        writer.Close();
                    }

                }
            }
            finally
            {
                RWlock.ExitWriteLock();
            }

        }
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception("it has to be 3 arguments");
            }

            long n = long.Parse(args[0]);
            long m = long.Parse(args[1]);
            int nThreads = int.Parse(args[2]);

            if (m < n)
            {
                throw new Exception("m has to be greater than n");
            }
            if (nThreads <= 0)
            {
                throw new Exception("nThreads has to be greater than 0");
            }
            var primec = new Program();
            Thread[] threads = new Thread[nThreads];
            long range = (m - n) / nThreads;
            long count = n;
            for (int i = 0; i < nThreads; i++)
            {

                long s = count;
                long f = count + range;
                count += range;
                if (i == nThreads - 1)
                {
                    f = m;
                }

                threads[i] = new Thread(() => primec.PrimeCalchelp(s, f));

            }
            for (int i = 0; i < nThreads; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < nThreads; i++)
            {
                threads[i].Join();
            }
        }
    }
}
