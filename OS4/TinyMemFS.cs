using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TinyMemFS
{
    class TinyMemFS
    {
        //
        Dictionary<String, myFile> myFS;
        List<String> pass;
        //TODO: check how the date format is saved
        //
        public TinyMemFS()
        {
            pass = new List<String>();
            myFS = new Dictionary<string, myFile>();
        }
        public bool add(String fileName, String fileToAdd)
        {
            // fileName - The name of the file to be added to the file system
            // fileToAdd - The file path on the computer that we add to the system
            // return false if operation failed for any reason
            // Example:
            // add("name1.pdf", "C:\\Users\\user\Desktop\\report.pdf")
            // note that fileToAdd isn't the same as the fileName
            FileInfo fileInfo = new FileInfo(fileToAdd);
            string size;
            string date;
            byte[] data;
            myFile myFile;
            if (!myFS.ContainsKey(fileName))
            {
                if (fileInfo.Exists)
                {
                    try
                    {
                        myFile = new myFile(fileToAdd);
                        myFS.Add(fileName, myFile);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
            return true;
        }

        public bool remove(String fileName)
        {
            // fileName - remove fileName from the system
            // this operation releases all allocated memory for this file
            // return false if operation failed for any reason
            // Example:
            // remove("name1.pdf")
            if (myFS.ContainsKey(fileName))
            {
                myFS.Remove(fileName);
                return true;
            }
            else
            {
                return false;
            }
            return false;
        }
        public List<String> listFiles()
        {
            // The function returns a list of strings with the file information in the system
            // Each string holds details of one file as following: "fileName,size,creation time"
            // Example:{
            // "report.pdf,630KB,Friday, ‎May ‎13, ‎2022, ‏‎12:16:32 PM",
            // "table1.csv,220KB,Monday, ‎February ‎14, ‎2022, ‏‎8:38:24 PM" }
            // You can use any format for the creation time and date
            List<String> files = new List<string>();
            foreach (KeyValuePair<String, myFile> mf in myFS)
            {
                string str = mf.Key + "," + mf.Value.toString();
                files.Add(str);

            }
            return files;
        }
        public bool save(String fileName, String fileToAdd)
        {
            // this function saves file from the TinyMemFS file system into a file in the physical disk
            // fileName - file name from TinyMemFS to save in the computer
            // fileToAdd - The file path to be saved on the computer
            // return false if operation failed for any reason
            // Example:
            // save("name1.pdf", "C:\\tmp\\fileName.pdf")
            if (myFS.ContainsKey(fileName))
            {
                byte[] data = myFS[fileName].data;
                DateTime date = DateTime.Parse(myFS[fileName].date);



                if (!File.Exists(fileToAdd))
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    File.WriteAllBytes(fileToAdd, data);
                    File.SetCreationTime(fileToAdd, date);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public bool encrypt(String key)
        {
            // key - Encryption key to encrypt the contents of all files in the system 
            // You can use an encryption algorithm of your choice
            // return false if operation failed for any reason
            // Example:
            // encrypt("myFSpassword")
            if (key == null)
                return false;
            byte[] Aeskey = { };
            byte[] AesIV = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160 };
            using (SHA256 sha = SHA256.Create())
            {
                Aeskey = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
            bool ret = false;
            foreach (KeyValuePair<String, myFile> mf in myFS)
            {

                byte[] encrypted = EncryptFile(mf.Value.data, Aeskey, AesIV);
                if (encrypted != null)
                {
                    mf.Value.enc_counter++;
                    mf.Value.password.Add(key);
                    mf.Value.data = encrypted;
                    ret = true;
                }
                else
                {
                    if (!ret)
                        ret = false;
                }
            }
            if (ret)
            {
                pass.Add(key);
            }
            return ret;
        }

        public bool decrypt(String key)
        {
            // fileName - Decryption key to decrypt the contents of all files in the system 
            // return false if operation failed for any reason
            // Example:
            // decrypt("myFSpassword")
            if (key == null)
                return false;
            byte[] Aeskey = { };
            byte[] AesIV = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160 };
            using (SHA256 sha = SHA256.Create())
            {
                Aeskey = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
            bool ret = false;
            foreach (KeyValuePair<String, myFile> mf in myFS)
            {
                if (mf.Value.enc_counter != 0 && pass.Contains(key))
                {
                    if (mf.Value.password.Contains(key) && mf.Value.password[mf.Value.enc_counter -1].Equals(key))
                    {
                        byte[] decrypted = DecryptFile(mf.Value.data, Aeskey, AesIV);
                        if (decrypted != null)
                        {
                            mf.Value.enc_counter--;
                            mf.Value.password.Remove(key);
                            mf.Value.data = decrypted;
                            ret = true;
                        }
                    }
                }
            }
            if (ret)
            {
                pass.Remove(key);
            }
            return ret;
        }

        static byte[] EncryptFile(byte[] plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;


            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.FeedbackSize = 128;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = IV;


                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainText, 0, plainText.Length);
                        csEncrypt.FlushFinalBlock();
                        encrypted = msEncrypt.ToArray();

                    }

                }
            }
            return encrypted;
        }

        static byte[] DecryptFile(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherText, 0, cipherText.Length); //check count!!
                        csDecrypt.FlushFinalBlock();
                    }
                    plaintext = msDecrypt.ToArray();
                }
            }

            return plaintext;
        }

        // ************** NOT MANDATORY ********************************************
        // ********** Extended features of TinyMemFS ********************************
        public bool saveToDisk(String fileName)
        {
            /*
             * Save the FS to a single file in disk
             * return false if operation failed for any reason
             * You should store the entire FS (metadata and files) from memory to a single file.
             * You can decide how to save the FS in a single file (format, etc.) 
             * Example:
             * SaveToDisk("MYTINYFS.DAT")
             */
            return false;
        }


        public bool loadFromDisk(String fileName)
        {
            /*
             * Load a saved FS from a file  
             * return false if operation failed for any reason
             * You should clear all the files in the current TinyMemFS if exist, before loading the filenName
             * Example:
             * LoadFromDisk("MYTINYFS.DAT")
             */
            return false;
        }

        public bool compressFile(String fileName)
        {
            /* Compress file fileName
             * return false if operation failed for any reason
             * You can use an compression/uncompression algorithm of your choice
             * Note that the file size might be changed due to this operation, update it accordingly
             * Example:
             * compressFile ("name1.pdf");
             */
            return false;
        }

        public bool uncompressFile(String fileName)
        {
            /* uncompress file fileName
             * return false if operation failed for any reason
             * You can use an compression/uncompression algorithm of your choice
             * Note that the file size might be changed due to this operation, update it accordingly
             * Example:
             * uncompressFile ("name1.pdf");
             */
            return false;
        }

        public bool setHidden(String fileName, bool hidden)
        {
            /* set the hidden property of fileName
             * If file is hidden, it will not appear in the listFiles() results
             * return false if operation failed for any reason
             * Example:
             * setHidden ("name1.pdf", true);
             */
            return false;
        }

        public bool rename(String fileName, String newFileName)
        {
            /* Rename filename to newFileName
             * Return false if operation failed for any reason (E.g., newFileName already exists)
             * Example:
             * rename ("name1.pdf", "name2.pdf");
             */
            if (myFS.ContainsKey(fileName) && !myFS.ContainsKey(newFileName))
            {
                myFile temp = myFS[fileName];
                myFS.Remove(fileName);
                myFS.Add(newFileName, temp);
                return true;
            }
            return false;
        }

        public bool copy(String fileName1, String fileName2)
        {
            /* Rename filename1 to a new filename2
             * Return false if operation failed for any reason (E.g., fileName1 doesn't exist or filename2 already exists)
             * Example:
             * rename ("name1.pdf", "name2.pdf");
             */
            if (myFS.ContainsKey(fileName1))
            {
                myFile temp = myFS[fileName1];
                myFS[fileName2] = temp;
                return true;
            }
            return false;
        }


        public void sortByName()
        {
            /* Sort the files in the FS by their names (alphabetical order)
             * This should affect the order the files appear in the listFiles 
             * if two names are equal you can sort them arbitrarily
             */
            return;
        }

        public void sortByDate()
        {
            /* Sort the files in the FS by their date (new to old)
             * This should affect the order the files appear in the listFiles  
             * if two dates are equal you can sort them arbitrarily
             */
            return;
        }

        public void sortBySize()
        {
            /* Sort the files in the FS by their sizes (large to small)
             * This should affect the order the files appear in the listFiles  
             * if two sizes are equal you can sort them arbitrarily
             */
            return;
        }


        public bool compare(String fileName1, String fileName2)
        {
            /* compare fileName1 and fileName2
             * files considered equal if their content is equal 
             * Return false if the two files are not equal, or if operation failed for any reason (E.g., fileName1 or fileName2 not exist)
             * Example:
             * compare ("name1.pdf", "name2.pdf");
             */
            if (myFS.ContainsKey(fileName1) && myFS.ContainsKey(fileName2))
            {
                byte[] file1 = myFS[fileName1].data;
                byte[] file2 = myFS[fileName2].data;
                if (file1.Length != file2.Length)
                    return false;
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        public Int64 getSize()
        {
            /* return the size of all files in the FS (sum of all sizes)
             */
            Int64 ret = 0;
            foreach (KeyValuePair<String, myFile> mf in myFS)
            {
                ret += Int64.Parse(mf.Value.size);
            }
            return ret;
        }

        class myFile
        {
            public String name;
            public String size;
            public String date;
            public byte[] data;
            public List<String> password;
            public int enc_counter;
            public int dec_counter;

            public myFile(String path)
            {
                enc_counter = 0;
                dec_counter = 0;
                password = new List<string>();
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    try
                    {
                        size = fileInfo.Length.ToString();
                        date = fileInfo.CreationTime.ToString();
                        data = File.ReadAllBytes(path);
                        name = fileInfo.FullName;

                    }
                    catch (Exception e)
                    {
                        size = null;
                        date = null;
                        data = null;
                        name = null;
                    }
                }
                else
                {
                    size = null;
                    date = null;
                    data = null;
                    name = null;
                }
            }
            public string toString()
            {
                string str = size + "," + date;
                return str;
            }
        }
    }
}