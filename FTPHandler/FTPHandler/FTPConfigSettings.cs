using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTPHandler
{
   public class FTPConfigSettings
   {
      public string DefaultDownloadPath
      {
         get
         {
            return Get("folderPath");
         }
      }

      public string FtpPath
      {
         get
         {
            return Get("ftpPath");
         }
      }

      protected string FtpUserId
      {
         get
         {
            return Get("ftpUserId");
         }
      }

      protected string FtpPassword
      {
         get
         {
            return Get("ftpPassword");
         }
      }

      public int BufferSize
      {
         get
         {
            return GetInt("buffersize");
         }
      }

      public bool SSLEnabled
      {
         get
         {
            return GetBool("sslEnabled");
         }
      }

      public static string Get(string key)
      {
         string value = System.Configuration.ConfigurationManager.AppSettings[key];

         if (string.IsNullOrEmpty(value))
            throw new Exception(string.Format("Unable to find the key: {0} ", key));

         return value;
      }

      public static int GetInt(string key) { return int.Parse(Get(key)); }
      public static bool GetBool(string key) { return bool.Parse(Get(key)); }



   }
}
