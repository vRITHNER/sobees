#region

using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace Sobees.Tools.Helpers
{
  public class CompressionHelper
  {
    public static byte[] Compress(string text)
    {
      var buffer = Encoding.UTF8.GetBytes(text);
      var ms = new MemoryStream();
      using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
      {
        zip.Write(buffer, 0, buffer.Length);
      }

      ms.Position = 0;

      var compressed = new byte[ms.Length];
      ms.Read(compressed, 0, compressed.Length);

      var gzBuffer = new byte[compressed.Length + 4];
      Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
      Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
      return gzBuffer;
    }

    public static string Decompress(byte[] gzBuffer)
    {
      using (var ms = new MemoryStream())
      {
        var msgLength = BitConverter.ToInt32(gzBuffer, 0);
        ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

        var buffer = new byte[msgLength];

        ms.Position = 0;
        using (var zip = new GZipStream(ms, CompressionMode.Decompress))
        {
          zip.Read(buffer, 0, buffer.Length);
        }

        return Encoding.UTF8.GetString(buffer);
      }
    }


    public static string ZipFiles(string inputFolderPath,
                                  string outputPathAndFile,
                                  string filePattern,
                                  bool lookIntoSubFolder,
                                  string password)
    {
      var ar = GenerateFileList(inputFolderPath, filePattern, lookIntoSubFolder); // generate file list
      var TrimLength = (Directory.GetParent(inputFolderPath)).ToString().Length;
      // find number of chars to remove     // from orginal file path
      TrimLength += 1; //remove '\'
      FileStream ostream;
      byte[] obuffer;
      var outPath = outputPathAndFile;
      var oZipStream = new ZipOutputStream(File.Create(outPath)); // create zip stream
      if (!string.IsNullOrEmpty(password))
        oZipStream.Password = password;
      oZipStream.SetLevel(9); // maximum compression
      ZipEntry oZipEntry;
      foreach (string Fil in ar) // for each file, generate a zipentry
      {
        oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
        oZipStream.PutNextEntry(oZipEntry);

        if (!Fil.EndsWith(@"/")) // if a file ends with '/' its a directory
        {
          ostream = File.OpenRead(Fil);
          obuffer = new byte[ostream.Length];
          ostream.Read(obuffer, 0, obuffer.Length);
          oZipStream.Write(obuffer, 0, obuffer.Length);
        }
      }
      oZipStream.Finish();
      oZipStream.Close();
      return outPath;
    }

    private static ArrayList GenerateFileList(string dir,
                                              string pattern,
                                              bool lookInSubFolder)
    {
      var fils = new ArrayList();
      var Empty = true;
      foreach (var file in Directory.GetFiles(dir, pattern)) // add each file in directory
      {
        fils.Add(file);
        Empty = false;
      }

      if (Empty)
      {
        if (Directory.GetDirectories(dir).Length == 0)
          // if directory is completely empty, add it
        {
          fils.Add(dir + @"/");
        }
      }

      if (lookInSubFolder)
      {
        foreach (var dirs in Directory.GetDirectories(dir)) // recursive
        {
          foreach (var obj in GenerateFileList(dirs, pattern, lookInSubFolder))
          {
            fils.Add(obj);
          }
        }
      }

      return fils; // return file list
    }


    public static void UnZipFiles(string zipPathAndFile,
                                  string outputFolder,
                                  string password,
                                  bool deleteZipFile)
    {
      var s = new ZipInputStream(File.OpenRead(zipPathAndFile));
      if (password != null && password != String.Empty)
        s.Password = password;
      ZipEntry theEntry;
      var tmpEntry = String.Empty;
      while ((theEntry = s.GetNextEntry()) != null)
      {
        var directoryName = outputFolder;
        var fileName = Path.GetFileName(theEntry.Name);
        // create directory 
        if (directoryName != "")
        {
          Directory.CreateDirectory(directoryName);
        }
        if (fileName != String.Empty)
        {
          if (theEntry.Name.IndexOf(".ini") < 0)
          {
            var fullPath = directoryName + "\\" + theEntry.Name;
            fullPath = fullPath.Replace("\\ ", "\\");
            var fullDirPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullDirPath))
              Directory.CreateDirectory(fullDirPath);
            var streamWriter = File.Create(fullPath);
            var size = 2048;
            var data = new byte[2048];
            while (true)
            {
              size = s.Read(data, 0, data.Length);
              if (size > 0)
              {
                streamWriter.Write(data, 0, size);
              }
              else
              {
                break;
              }
            }
            streamWriter.Close();
          }
        }
      }
      s.Close();
      if (deleteZipFile)
        File.Delete(zipPathAndFile);
    }
  }
}