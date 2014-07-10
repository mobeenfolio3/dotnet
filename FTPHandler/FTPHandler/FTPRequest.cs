using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace FTPHandler
{
   public class FTPRequest : FTPConfigSettings
   {

      /// <summary>
      /// Download the file from FTP server to the desired location 
      /// </summary>
      /// <param name="fileName">File to be downloaded(e.g. "test.xml")</param>
      /// <param name="downloadPath">Destination location(e.g. "C:\Users\metastruct\Desktop\")</param>
      /// <returns>Reponse after download is completed</returns>
      public FtpWebResponse DownloadFile(string fileName, string downloadPath)
      {
         Stream ftpStream = null;
         FileStream outputStream = null;
         FtpWebResponse response = null;
         try
         {
            
            outputStream = new FileStream(downloadPath + fileName, FileMode.Create);

            FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(FtpPath + fileName));
            reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(FtpUserId,
                                                        FtpPassword);
            response = (FtpWebResponse)reqFtp.GetResponse();
            ftpStream = response.GetResponseStream();

            byte[] buffer = new byte[BufferSize];

            if (ftpStream != null)
            {
               int readCount = ftpStream.Read(buffer, 0, BufferSize);
               while (readCount > 0)
               {
                  outputStream.Write(buffer, 0, readCount);
                  readCount = ftpStream.Read(buffer, 0, BufferSize);
               }

               ftpStream.Close();
            }
            return response;
         }
         catch
         {
            throw;
         }
         finally
         {
            if (ftpStream != null)
               ftpStream.Close();
            if (outputStream != null)
               outputStream.Close();
            if(response !=null && response.StatusCode != FtpStatusCode.ConnectionClosed)
            response.Close();
         }

      }

      /// <summary>
      /// Download's the file to the default download path specified in config
      /// </summary>
      /// <param name="fileName">File to be downloaded(e.g. "test.xml")</param>
      /// <returns>Reponse after download is completed</returns>
      public FtpWebResponse DownloadFile(string fileName)
      {
         return DownloadFile(fileName, DefaultDownloadPath);
      }

      /// <summary>
      /// Get list of FTP files present at the folder location
      /// </summary>
      /// <param name="folderPath">virtual folder path(e.g. "folder/subfolder")</param>
      /// <returns>List of files present at this path</returns>
      public string[] GetFTPFileList(string folderPath = "")
      {
         StringBuilder result = new StringBuilder();
         FtpWebRequest reqFTP;

         reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpPath + folderPath));
         reqFTP.UseBinary = true;
         reqFTP.Credentials = new NetworkCredential(FtpUserId,
                                                    FtpPassword);
         reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
         WebResponse response = reqFTP.GetResponse();
         StreamReader reader = new StreamReader(response
                                         .GetResponseStream());

         string line = reader.ReadLine();
         while (line != null)
         {
            result.Append(line);
            result.Append("\n");
            line = reader.ReadLine();
         }
         // to remove the trailing '\n'
         result.Remove(result.ToString().LastIndexOf('\n'), 1);
         reader.Close();
         response.Close();
         return result.ToString().Split('\n');

      }
      
      /// <summary>
      /// Upload a file to location specified at web config
      /// </summary>
      /// <param name="fileName">File path on the physical drive(e.g. "C:\Users\metastruct\Desktop\test.xml")</param>
      /// /// <param name="virtualDir">Virtual directory path where to upload(e.g. "folder/subfolder")</param>
      /// <returns>Reponse after download is completed</returns>
      public FtpWebResponse Upload(string fileName, string virtualDir = "")
      {
         FileInfo fileInf = new FileInfo(fileName);
         string uri = FtpPath + virtualDir + fileInf.Name;
         FtpWebRequest reqFTP;

         reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

         reqFTP.Credentials = new NetworkCredential(FtpUserId,
                                                    FtpPassword);
         reqFTP.EnableSsl = SSLEnabled;
         reqFTP.KeepAlive = false;
         reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
         reqFTP.UseBinary = true;
         reqFTP.ContentLength = fileInf.Length;
         byte[] buff = new byte[BufferSize];
         int contentLen;
         if (SSLEnabled)
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

         FileStream fs = fileInf.OpenRead();
         try
         {
            Stream strm = reqFTP.GetRequestStream();
            contentLen = fs.Read(buff, 0, BufferSize);
            while (contentLen != 0)
            {
               strm.Write(buff, 0, contentLen);
               contentLen = fs.Read(buff, 0, BufferSize);
            }
            strm.Close();
            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            response.Close();
            return response;
         }
         catch
         {
            throw;
         }
         finally
         {
            fs.Close();
         }
      }

      /// <summary>
      /// Delete file from FTP server
      /// </summary>
      /// <param name="deleteFile">Name of the file that will be deleted(e.g. "test.xml")</param>
      /// <param name="virtualDir">Virtual directory path where to upload(e.g. "folder/subfolder")</param>
      /// <returns></returns>
      public FtpWebResponse Delete(string deleteFile, string virtualDir = "")
      {
         FtpWebRequest reqFTP;

         reqFTP = (FtpWebRequest)WebRequest.Create(FtpPath + virtualDir + deleteFile);
         reqFTP.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
         reqFTP.UseBinary = true;
         reqFTP.UsePassive = true;
         reqFTP.KeepAlive = true;
         reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
         FtpWebResponse ftpResponse = (FtpWebResponse)reqFTP.GetResponse();
         ftpResponse.Close();
         reqFTP = null;
         return ftpResponse;

      }

      /// <summary>
      /// Rename file on the server
      /// </summary>
      /// <param name="currentFileNameAndPath">orignal file on the server(e.g. "etc/test.xml")</param>
      /// <param name="newFileName">New file name(e.g. "test2.xml")</param>
      /// <returns></returns>
      public FtpWebResponse Rename(string currentFileNameAndPath, string newFileName)
      {
         FtpWebRequest reqFTP;

         reqFTP = (FtpWebRequest)WebRequest.Create(FtpPath + currentFileNameAndPath);
         reqFTP.Credentials = new NetworkCredential(FtpUserId, FtpPassword);
         reqFTP.UseBinary = true;
         reqFTP.UsePassive = true;
         reqFTP.KeepAlive = true;
         reqFTP.Method = WebRequestMethods.Ftp.Rename;
         reqFTP.RenameTo = newFileName;
         FtpWebResponse ftpResponse = (FtpWebResponse)reqFTP.GetResponse();
         ftpResponse.Close();
         reqFTP = null;
         return ftpResponse;

      }


   }
}
