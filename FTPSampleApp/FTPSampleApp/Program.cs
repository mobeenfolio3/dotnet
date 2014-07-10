using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTPSampleApp
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Starting uploader app\r\n\r\n");
         Console.WriteLine("Initializing Handler\r\n\r\n");
         FTPHandler.FTPRequest ftp = new FTPHandler.FTPRequest();
         Console.WriteLine("Starting uploading file");
         ftp.Upload(System.Environment.CurrentDirectory + "\\YourSoftwareLicence.txt");
         Console.WriteLine("Upload completed successfully\r\n\r\n");

         Console.WriteLine("Start downloading file");
         ftp.DownloadFile("YourSoftwareLicence.txt","d:\\");
         Console.WriteLine("Downlading completed\r\n\r\n");

         Console.WriteLine("Renaming file");
         ftp.Rename("YourSoftwareLicence.txt", "myfile.txt");
         Console.WriteLine("Rename completed\r\n\r\n");

         Console.WriteLine("Get Ftp File list");
         string [] lst =ftp.GetFTPFileList();
         foreach (var item in lst)
         {
            Console.WriteLine(item);
         }
         Console.WriteLine("Files listed\r\n\r\n");
         Console.WriteLine("\r\n");
         Console.WriteLine("Delete file from the server");
         ftp.Delete("myfile.txt");
         Console.WriteLine("Deletion completed\r\n\r\n");


         Console.ReadLine();
        
      }
   }
}
