using System;
using System.Threading;
using System.IO;
using System.Collections;

namespace ConsoleApp1
{
    class Program
    {
        // function that checks if StringToSearch has been found or nor (returns boolean
        public static bool isFound(String text, String stringToSearch, int from, int delta)
        {

            int i = from + delta + 1;
            int j = 1;
            while (i < text.Length && j < stringToSearch.Length && text[i] == stringToSearch[j])
            {
                i += delta + 1;
                j++;
            }
            if (j == stringToSearch.Length)
                return true;
            return false;
        }
        // searching with a loop in the array of the indexies for the StringToSearch
        public static void search(string text, string stringToSearch, int delta, Position[] pos, int from, int to, Printer printer)
        {

            for (int i = from; i <= to; i++)
            {
                if (isFound(text, stringToSearch, pos[i].GetIndex(), delta))
                    printer.printString("[" + pos[i].GetLine() + "," + pos[i].GetRow() + "]");
            }
        }

        public static Position[] searchAllStartWith(string text, string stringToSearch)
        {
            // creating an array of the position of the first character
            ArrayList lst = new ArrayList();
            int line = 0;
            int row = 0;
            char ch = stringToSearch[0];
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    row = 0;
                }
                else
                {
                    if (text[i] == ch)
                        lst.Add(new Position(line, row, i));
                    row++;
                }
            }
            return lst.ToArray(typeof(Position)) as Position[];

        }


        static void Main(string[] args)
        {
            //defining the parameters
            if (args.Length != 4)
            {
                throw new Exception("it has to be 4 arguments");
            }
            String textfile = args[0];
            String StringToSearch = args[1];
            int nThreads = int.Parse(args[2]);
            int delta = int.Parse(args[3]);
            Printer pr = new Printer();
            if (nThreads < 0)
            {
                throw new Exception("Threads has to be >0");
            }
            if (delta < 0)
            {
                throw new Exception("delta has to be >0");
            }
            try
            {
                //reading the text file
                String text = File.ReadAllText(textfile);



                // building a array of the indexes of the first character of StringTo Search
                Position[] pos = searchAllStartWith(text, StringToSearch);
                Thread[] threads = new Thread[nThreads];
                //calculating the range of every thread
                int range = pos.Length / nThreads;
                int from = 0;
                //assigning every thread in a certain range to work on it
                for (int i = 0; i < nThreads - 1; i++)
                {
                    int start = from;
                    from += range;
                    int end = from - 1;
                    //search function working on the array that we built that taking the indexies and searching the StringToSearch in the whole text 
                    threads[i] = new Thread(() => search(text, StringToSearch, delta, pos, start, end, pr));
                    threads[i].Start();

                    //from = from + range;

                }

                threads[nThreads - 1] = new Thread(() => search(text, StringToSearch, delta, pos, from, pos.Length - 1, pr));
                threads[nThreads - 1].Start();

                for (int i = 0; i < nThreads; i++)
                {
                    threads[i].Join();
                }

                // checking if there is results or not if not then print Not found.
                if (!pr.GetFound())
                {

                    Console.WriteLine("Not found.");
                }





            }
            catch (Exception e)
            {
                //Console.WriteLine("Not found.");
            }

        }

    }
    // a class that stores the position of the String and its index in the text
    public class Position
    {
        private int line;
        private int row;
        private int index;
        public Position(int line, int row, int index)
        {
            this.line = line;
            this.row = row;
            this.index = index;
        }
        public int GetLine()
        {
            return line;
        }
        public int GetRow()
        {
            return row;

        }
        public int GetIndex()
        {
            return index;
        }

        public override String ToString()
        {
            return "[" + line + "," + row + "," + index + "]";
        }
    }

    public class Printer
    {
        private bool found = false;
        public void printString(string str)
        {
            lock (this)
            {
                Console.WriteLine(str);
                found = true;
            }
        }
        public bool GetFound()
        {
            return found;
        }
    }
}

