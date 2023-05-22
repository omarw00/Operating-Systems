using System;
using System.Threading;
using System.Collections.Generic;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            int rows = Int32.Parse(args[0]);
            int cols = Int32.Parse(args[1]);
            int nThreads = Int32.Parse(args[2]);
            int nOperations = Int32.Parse(args[3]);
            int mssleep = Int32.Parse(args[4]);

            SharableSpreadSheet sharableSpreadSheet = new SharableSpreadSheet(rows, cols, nThreads);

            for (int r = 0;r< rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    string str = "testcell" + r.ToString() + c.ToString();
                    sharableSpreadSheet.setCell(r, c, str);
                }
            }

            for (int i = 0; i < nThreads; i++)
            {
                Thread thread = new Thread(() => ThreadProc(sharableSpreadSheet, nOperations,mssleep));
                thread.Start();
                
            }
            
        }
        static void ThreadProc(SharableSpreadSheet spreadSheet,int operations, int time)
        {
            Random random = new Random();
            for (int i = 0; i < operations; i++)
            {
                Tuple<int, int> size; 
                int randomCase = random.Next(0, 14);
                switch (randomCase)
                {
                    case 0:
                        size = spreadSheet.getSize();
                        int randomRow = random.Next(0,size.Item1);
                        int randomCol = random.Next(0, size.Item2);
                        string getCell = spreadSheet.getCell(randomRow, randomCol);
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: String" + getCell + " in cell [" + randomRow + "," + randomCol + "]");
                        break;
                    case 1:

                        size = spreadSheet.getSize();
                        
                        int randomRow1 = random.Next(0, size.Item1);
                        int randomCol1 = random.Next(0, size.Item2);
                        spreadSheet.setCell(randomRow1, randomCol1, "updated");
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: cell [" + randomRow1 + "," + randomCol1 + "]" + " has been setted to 'updated'");
                        break;
                    case 2:
                        Tuple<int,int> tuple = spreadSheet.searchString("updated");
                        if (tuple == null)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: did not found the string" + "'updated'" + " in the spread sheet");

                        }
                        else
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has found the string" + "'updated'" + " in the cell: [" + tuple.Item1 + "," + tuple.Item2 +"]");
                        }
                        break;
                    case 3:
                        size = spreadSheet.getSize();
                        int row1 = random.Next(0, size.Item1);
                        int row2 = random.Next(0, size.Item1);
                        if (row1 == row2)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: row1 and row2 are equal.. nothing exchanged!");
                        }
                        else
                        {
                            spreadSheet.exchangeRows(row1, row2);
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has exchanged the row " + row1 + " with the row " + row2);

                        }
                        break;
                    case 4:
                        size = spreadSheet.getSize();
                        int col1 = random.Next(0, size.Item2);
                        int col2 = random.Next(0, size.Item2);
                        if (col1 == col2)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: col1 and col2 are equal.. nothing exchanged!");
                        }
                        else
                        {
                            spreadSheet.exchangeCols(col1, col2);
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has exchanged the row " + col1 + " with the row " + col2);

                        }
                        break;
                    case 5:
                        size = spreadSheet.getSize();
                        int row = random.Next(0,size.Item1);
                        int col = spreadSheet.searchInRow(row, "updated");
                        if (col == -1)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: the string 'updated' is not found");
                        }
                        else
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has found the string 'updated' in row: " + row + "and col: " + col); 
                        }
                        break;
                    case 6:
                        size = spreadSheet.getSize();
                        int c = random.Next(0, size.Item2);
                        int r = spreadSheet.searchInCol(c, "updated");
                        if (r == -1)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: the string 'updated' is not found");
                        }
                        else
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has found the string 'updated' in row: " + r + "and col: " + c);
                        }
                        break;
                    case 7:
                        size = spreadSheet.getSize();
                        int r1 = random.Next(0, size.Item1);
                        int r2 = random.Next(r1, size.Item1);
                        int c1 = random.Next(0, size.Item2);
                        int c2 = random.Next(c1, size.Item2);
                        Tuple<int, int> cord = spreadSheet.searchInRange(c1, c2, r1, r2, "updated");
                        if (cord.Item1 ==-1 || cord.Item2 == -1)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: the string 'updated' is not found");

                        }
                        else
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has found the string 'updated' in row: " + cord.Item1 + "and col: " + cord.Item2);
                        }
                        break;
                    case 8:
                        size = spreadSheet.getSize();
                        int randR = random.Next(0, size.Item1 - 1);
                        spreadSheet.addRow(randR);
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: a row has been added after the row: " + randR);
                        break;
                    case 9:
                        size = spreadSheet.getSize();
                        int randC = random.Next(0, size.Item1 - 1);
                        spreadSheet.addCol(randC);
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: a column has been added after the column: " + randC);
                        break;
                    case 10:
                        Tuple<int, int>[] all = spreadSheet.findAll("updated", true);
                        if (all == null)
                        {
                            Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: nothing has been found");
                            break;
                        }

                        string str = "";
                        for (int ind = 0; ind< all.Length; ind++)
                        {
                            str += "[" + all[ind].Item1 + "," + all[ind].Item2 + "], ";
                        }
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has found the string in: " + str);
                        break;
                    case 11:
                        spreadSheet.setAll("UPDATED", "replaced", false);
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has changed every cell with 'updated' to 'replaced' ");
                        break;
                    case 12:
                        size = spreadSheet.getSize();
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has got the size of the sheet which is: rows = " + size.Item1 + " cols = " + size.Item2);
                        break;
                    case 13:
                        int users = random.Next(0, 7);
                        spreadSheet.setConcurrentSearchLimit(users);
                        Console.WriteLine("User[" + Thread.CurrentThread.ManagedThreadId + "]: has setted the users to: " + users);
                        break;
                }
                Thread.Sleep(time);
            }
        }
    }
}
