using System;
using System.Threading;
using System.IO;
using System.Collections;


class SharableSpreadSheet
{

    //the size of the sheet
    int rows;
    int cols;
    // the sheet
    string[,] sheet;

    // mutex locks on rows/columns
    Mutex[] cols_lock;
    Mutex[] rows_lock;

    // semaphone for the sheet lock
    Semaphore sheet_lock;
    // slots available counter lock
    Semaphore slotsAvailableLock;
    // Semaphore to orginaze the enterance
    Semaphore Queue;

    static long ConcurrentSearchLimit;
    static long SlotsAvailable;


    public SharableSpreadSheet(int nRows, int nCols, int nUsers = -1)
    {
        rows = nRows;
        cols = nCols;
        sheet = new string[rows, cols];

        cols_lock = new Mutex[cols];
        rows_lock = new Mutex[rows];
        for (int i = 0; i < cols; i++)
        //{
            cols_lock[i] = new Mutex();
        }
        for (int i = 0; i < rows; i++)
        {
            rows_lock[i] = new Mutex();
        }

        sheet_lock = new Semaphore(0, 1);
        slotsAvailableLock = new Semaphore(0, 1);
        Queue = new Semaphore(0, 1);

        sheet_lock.Release();
        slotsAvailableLock.Release();
        Queue.Release();

        Interlocked.Exchange(ref ConcurrentSearchLimit, nUsers);
        Interlocked.Exchange(ref SlotsAvailable, 0);

        // nUsers used for setConcurrentSearchLimit, -1 mean no limit.
        // construct a nRows*nCols spreadsheet
    }
    public void userLogIn()
    {
        Queue.WaitOne();
        slotsAvailableLock.WaitOne();
        Interlocked.Increment(ref SlotsAvailable);
        if (SlotsAvailable == 1)
        {
            sheet_lock.WaitOne();
        }
        slotsAvailableLock.Release();
        Queue.Release();

    }
    public void userLogOut()
    {
        slotsAvailableLock.WaitOne();
        Interlocked.Decrement(ref SlotsAvailable);
        if (SlotsAvailable == 0)
        {
            sheet_lock.Release();
        }
        slotsAvailableLock.Release();

    }


    public String getCell(int row, int col)
    {
        string cell;
        if (rows <= row || cols <= col || row < 0 || col < 0)
        {
            Console.WriteLine("row/col outside of range.");
            return null;
        }
        userLogIn();
        rows_lock[row].WaitOne();
        cols_lock[col].WaitOne();
        cell = sheet[row, col];
        cols_lock[col].ReleaseMutex();
        rows_lock[row].ReleaseMutex();
        userLogOut();
        // return the string at [row,col]
        return cell;

    }
    public void setCell(int row, int col, String str)
    {
        // set the string at [row,col]
        if (rows <= row || cols <= col || row < 0 || col < 0)
        {
            Console.WriteLine("row/col outside of range.");
            return;
        }
        userLogIn();
        rows_lock[row].WaitOne();
        cols_lock[col].WaitOne();
        sheet[row, col] = str;
        cols_lock[col].ReleaseMutex();
        rows_lock[row].ReleaseMutex();
        userLogOut();
    }



    public Tuple<int, int> searchString(String str)
    {
        int row = 0;
        int col = 0;
        // return first cell indexes that contains the string (search from first row to the last row)
        userLogIn();
        int found = 0;
        for (int i = 0; i < rows && found != 1; i++)
        {
            rows_lock[i].WaitOne();
            for (int j = 0; j < cols && found != 1; j++)
            {
                cols_lock[j].WaitOne();
                if (sheet[i, j] != null)
                {
                    if (sheet[i, j].Equals(str))
                    {
                        row = i;
                        col = j;
                        found = 1;

                    }
                }
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        if (found == 0)
        {
            Console.WriteLine("str is not Exist");
            userLogOut();
            return null;
        }
        userLogOut();
        return new Tuple<int, int>(row, col);
    }
    public void exchangeRows(int row1, int row2)
    {
        // exchange the content of row1 and row2
        if (row1 == row2)
        {
            Console.WriteLine("row1 and row2 are equals.");
            return;
        }
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        if (row1 < 0 || row2 < 0 || row1 > rows || row2 > rows)
        {
            Console.WriteLine("row1/row2 outside of range.");
            sheet_lock.Release();
            return;
        }
        string[] tempstr = new string[cols];
        int i = 0;
        while (i < cols)
        {
            tempstr[i] = sheet[row2, i];
            sheet[row2, i] = sheet[row1, i];
            sheet[row1, i] = tempstr[i];
            i++;
        }
        sheet_lock.Release();
    }
    public void exchangeCols(int col1, int col2)
    {
        // exchange the content of col1 and col2
        if (col1 == col2)
        {
            Console.WriteLine("col1/col2 are equals.");
            return;
        }
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        if (col1 < 0 || col2 < 0 || col1 > cols || col2 > cols)
        {
            Console.WriteLine("col1/col2 outside of range.");
            sheet_lock.Release();
            return;
        }
        string[] tempstr = new string[rows];
        int i = 0;
        while (i < rows)
        {
            tempstr[i] = sheet[i, col1];
            sheet[i, col1] = sheet[i, col2];
            sheet[i, col2] = tempstr[i];
            i++;
        }
        sheet_lock.Release();

    }
    public int searchInRow(int row, String str)
    {
        int col = 0;
        if (row > rows || row < 0)
        {
            Console.WriteLine("row outside of range.");
            return -1;
        }
        // perform search in specific row
        userLogIn();
        int found = 0;
        int i = 0;
        while (i < cols && found != 1)
        {
            cols_lock[i].WaitOne();
            if (sheet[row, i].Equals(str))
            {
                col = i;
                found = 1;
            }
            cols_lock[i].ReleaseMutex();
            i++;
        }
        userLogOut();
        return col;
    }
    public int searchInCol(int col, String str)
    {
        int row = 0;
        if (col > cols || col < 0)
        {
            Console.WriteLine("col outside of range.");
            return -1;
        }
        // perform search in specific col
        userLogIn();
        int found = 0;
        int i = 0;
        while (i < rows && found != 1)
        {
            rows_lock[i].WaitOne();
            if (sheet[i, col].Equals(str))
            {
                row = i;
                found = 1;
            }
            rows_lock[i].ReleaseMutex();
            i++;
        }
        if (found == 0)
        {
            Console.WriteLine("string not found.");
            userLogOut();
            return -1;
        }
        userLogOut();
        return row;
    }
    public Tuple<int, int> searchInRange(int col1, int col2, int row1, int row2, String str)
    {
        int row = 0; int col = 0;
        if (col1 > cols || col2 > cols || row1 > rows || row2 > rows || col1 < 0 || col2 < 0 || row1 < 0 || row2 < 0)
        {
            row = -1;
            col = -1;
            Console.WriteLine("parameters are outside of range.");

            return new Tuple<int, int>(row, col);
        }
        // perform search within spesific range: [row1:row2,col1:col2] 
        //includes col1,col2,row1,row2
        userLogIn();
        int found = 0;
        for (int i = row1; i < row2 && found != 1; i++)
        {
            rows_lock[i].WaitOne();
            for (int j = col1; j < col2 && found != 1; j++)
            {
                cols_lock[j].WaitOne();
                if (sheet[i, j].Equals(str))
                {
                    row = i;
                    col = j;
                    found = 1;
                }
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        if (found == 0)
        {
            Console.WriteLine("string not found.");
            row = -1;
            col = -1;
        }
        userLogOut();
        return new Tuple<int, int>(row, col);
    }
    public void addRow(int row1)
    {
        //add a row after row1
        if (row1 > rows || row1 < 0)
        {
            Console.WriteLine("row outside of range.");
            return;
        }
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        string[,] newsheet = new string[rows + 1, cols];
        Mutex[] newrowslock = new Mutex[rows + 1];
        for (int i = 0; i < row1; i++)
        {
            rows_lock[i].WaitOne();
            newrowslock[i] = rows_lock[i];
            for (int j = 0; j < cols; j++)
            {
                cols_lock[j].WaitOne();
                newsheet[i, j] = sheet[i, j];
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        for (int j = 0; j < cols; j++)
        {
            newsheet[row1, j] = "the new row added";
        }
        rows = rows + 1;
        newrowslock[row1] = new Mutex();
        for (int i = row1; i < rows - 1; i++)
        {
            rows_lock[i].WaitOne();
            newrowslock[i + 1] = rows_lock[i];
            for (int j = 0; j < cols; j++)
            {
                cols_lock[j].WaitOne();
                newsheet[i + 1, j] = sheet[i, j];
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        rows_lock = newrowslock;
        sheet = newsheet;
        sheet_lock.Release();
    }
    public void addCol(int col1)
    {
        //add a column after col1
        if (col1 > cols || col1 < 0)
        {
            Console.WriteLine("col1 outside of range.");
            return;
        }
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        string[,] newsheet = new string[rows, cols + 1];
        Mutex[] newcolslock = new Mutex[cols + 1];
        for (int i = 0; i < rows; i++)
        {
            rows_lock[i].WaitOne();
            for (int j = 0; j < col1; j++)
            {
                cols_lock[j].WaitOne();
                newcolslock[j] = cols_lock[j];
                newsheet[i, j] = sheet[i, j];
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        for (int j = 0; j < rows; j++)
        {
            newsheet[j, col1] = "the new col added";
        }
        cols = cols + 1;
        newcolslock[col1] = new Mutex();
        for (int i = 0; i < rows; i++)
        {
            rows_lock[i].WaitOne();
            for (int j = col1; j < cols - 1; j++)
            {
                cols_lock[j].WaitOne();
                newcolslock[j + 1] = cols_lock[j];
                newsheet[i, j + 1] = sheet[i, j];
                cols_lock[j].ReleaseMutex();
            }
            rows_lock[i].ReleaseMutex();
        }
        cols_lock = newcolslock;
        sheet = newsheet;
        sheet_lock.Release();

    }
    public Tuple<int, int>[] findAll(String str, bool caseSensitive)
    {

        // perform search and return all relevant cells according to caseSensitive param
        ArrayList allStr = new ArrayList();
        int row, col;
        if (caseSensitive)
        {
            userLogIn();
            for (int i = 0; i < rows; i++)
            {
                rows_lock[i].WaitOne();
                for (int j = 0; j < cols; j++)
                {
                    cols_lock[j].WaitOne();
                    try
                    {
                        if (sheet[i, j].Equals(str))
                        {
                            Tuple<int, int> temp = new Tuple<int, int>(i, j);
                            allStr.Add(temp);


                        }
                    }
                    catch (Exception error)
                    {
                        throw new Exception("str is not Exist");
                    }
                    cols_lock[j].ReleaseMutex();
                }
                rows_lock[i].ReleaseMutex();
            }
            userLogOut();
        }
        else
        {
            str = str.ToLower();
            userLogIn();
            for (int i = 0; i < rows; i++)
            {
                rows_lock[i].WaitOne();
                for (int j = 0; j < cols; j++)
                {
                    cols_lock[j].WaitOne();
                    try
                    {
                        string t = sheet[i, j].ToString().ToLower();
                        if (t.Equals(str))
                        {
                            Tuple<int, int> temp = new Tuple<int, int>(i, j);
                            allStr.Add(temp);


                        }
                    }
                    catch (Exception error)
                    {
                        throw new Exception("str is not Exist");
                    }
                    cols_lock[j].ReleaseMutex();
                }
                rows_lock[i].ReleaseMutex();
            }
            userLogOut();
        }
        Tuple<int, int>[] ret = new Tuple<int, int>[allStr.Count];
        allStr.CopyTo(ret);

        return ret;

    }
    public void setAll(String oldStr, String newStr, bool caseSensitive)
    {
        // replace all oldStr cells with the newStr str according to caseSensitive param
        if (caseSensitive)
        {
            userLogIn();
            for (int i = 0; i < rows; i++)
            {
                rows_lock[i].WaitOne();
                for (int j = 0; j < cols; j++)
                {
                    cols_lock[j].WaitOne();
                    try
                    {
                        if (sheet[i, j].Equals(oldStr))
                        {
                            sheet[i, j] = newStr;
                        }
                    }
                    catch (Exception error)
                    {
                        throw new Exception("str is not Exist");
                    }
                    cols_lock[j].ReleaseMutex();
                }
                rows_lock[i].ReleaseMutex();
            }
            userLogOut();
        }
        else
        {
            oldStr = oldStr.ToLower();
            userLogIn();
            for (int i = 0; i < rows; i++)
            {
                rows_lock[i].WaitOne();
                for (int j = 0; j < cols; j++)
                {
                    cols_lock[j].WaitOne();
                    try
                    {
                        string temp = sheet[i, j].ToString().ToLower();
                        if (temp.Equals(oldStr))
                        {
                            sheet[i, j] = newStr;
                        }
                    }
                    catch (Exception error)
                    {
                        throw new Exception("str is not Exist");
                    }
                    cols_lock[j].ReleaseMutex();
                }
                rows_lock[i].ReleaseMutex();
            }
            userLogOut();
        }


    }
    public Tuple<int, int> getSize()
    {
        int nRows = 0; int nCols = 0;
        nRows = rows;
        nCols = cols;
        // return the size of the spreadsheet in nRows, nCols
        return new Tuple<int, int>(nRows, nCols);
    }
    public void setConcurrentSearchLimit(int nUsers)
    {
        // this function aims to limit the number of users that can perform the search operations concurrently.
        // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
        // In this case additional search operations will wait for existing search to finish.
        // This function is used just in the creation
        if (nUsers > 0)
        {
            Queue.WaitOne();
            sheet_lock.WaitOne();
            Queue.Release();
            Interlocked.Exchange(ref ConcurrentSearchLimit, nUsers);
            sheet_lock.Release();
        }

    }

    public void save(String fileName)
    {
        // save the spreadsheet to a file fileName.
        // you can decide the format you save the data. There are several options.
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        StreamWriter sw = new StreamWriter(fileName + ".txt");
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                sw.Write(sheet[r, c]);
                if (c < cols - 1)
                {
                    sw.Write(",");
                }

            }
            sw.Write("/n");
        }
        sheet_lock.Release();
        sw.Close();
    }
    public void load(String fileName)
    {
        // load the spreadsheet from fileName
        // replace the data and size of the current spreadsheet with the loaded data
        SharableSpreadSheet loadedSheet;
        StreamReader sr = new StreamReader(fileName);
        StreamReader text = sr;
        if (sr == null)
        {
            return;
        }
        Queue.WaitOne();
        sheet_lock.WaitOne();
        Queue.Release();
        string lines = sr.ReadToEnd();
        string[] s = lines.Split("/n");
        int rowNum = lines.Split("/n").Length - 1;
        int colNum = s[0].Split(",").Length;
        loadedSheet = new SharableSpreadSheet(rowNum, colNum);
        string line = text.ReadLine();
        for (int j = 0; j < rowNum; j++)
        {
            string[] splitLine = s[j].Split(",");
            for (int i = 0; i < splitLine.Length; i++)
            {
                loadedSheet.sheet[j, i] = splitLine[i];

            }
        }
        sr.Close();
        text.Close();
        this.cols = loadedSheet.cols;
        this.rows = loadedSheet.rows;
        this.sheet = loadedSheet.sheet;

        sheet_lock.Release();
    }
}



