using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

public class ZipTools {

    public static IEnumerator ExtractZipFile(byte[] zipFileData, string targetDirectory, int bufferSize = 256 * 1024)
    {
        //Directory.CreateDirectory(targetDirectory);

        using (MemoryStream fileStream = new MemoryStream())
        {
            fileStream.Write(zipFileData, 0, zipFileData.Length);
            fileStream.Flush();
            fileStream.Seek(0, SeekOrigin.Begin);

            ZipFile zipFile = new ZipFile(fileStream);
            

            foreach (ZipEntry entry in zipFile)
            {
                string targetFile = Path.Combine(targetDirectory, entry.Name);

                using (FileStream outputFile = File.Create(targetFile))
                {
                    if (entry.Size > 0)
                    {
                        Stream zippedStream = zipFile.GetInputStream(entry);
                        byte[] dataBuffer = new byte[bufferSize];

                        int readBytes;
                        while ((readBytes = zippedStream.Read(dataBuffer, 0, bufferSize)) > 0)
                        {
                            outputFile.Write(dataBuffer, 0, readBytes);
                            outputFile.Flush();
                            yield return null;
                        }
                    }
                }
            }
        }
    }
}
