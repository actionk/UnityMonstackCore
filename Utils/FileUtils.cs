using System;
using System.Collections.Generic;
using System.IO;
using Plugins.UnityMonstackCore.Loggers;
using Plugins.UnityMonstackCore.Utils;
using UnityEngine;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class FileUtils
    {
        public static string GetApplicationDirectory()
        {
            var path = Application.dataPath;
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path += "/../../";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path += "/../";
            }

            return path;
        }

        // public static void SaveBytesToFile(FileSourceType fileSourceType, string pathToFile, byte[] bytes)
        // {
        //     var applicationPathToFile = GetApplicationPathToFile(fileSourceType, pathToFile);
        //     CreateDirectoryIfNotExistsWithActualPath(Path.GetDirectoryName(applicationPathToFile));
        //
        //     try
        //     {
        //         File.Delete(applicationPathToFile);
        //         using (var writer = new BinaryWriter(File.OpenWrite(applicationPathToFile)))
        //         {
        //             writer.Write(bytes);
        //             writer.Flush();
        //         }
        //     }
        //     catch (Exception)
        //     {
        //         var errorMessage = "Failed to save bytes data to file " + applicationPathToFile;
        //         UnityLogger.Error(errorMessage);
        //         throw;
        //     }
        // }

        public static IEnumerable<string> GetFilesInDirectoryRecursive(string directory, string pattern)
        {
            return Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories);
        }

        public static void SaveSerializedObjectToFile(string pathToFile, object data)
        {
            CreateDirectoryIfNotExistsWithActualPath(Path.GetDirectoryName(pathToFile));

            try
            {
                using (var writer = new StreamWriter(pathToFile))
                {
                    var json = JsonUtility.ToJson(data);
                    writer.Write(json);
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to save data to file " + pathToFile;
                UnityLogger.Error(errorMessage);
                throw;
            }
        }

        private static void CreateDirectoryIfNotExistsWithActualPath(string pathToDirectory)
        {
            if (!Directory.Exists(pathToDirectory)) Directory.CreateDirectory(pathToDirectory);
        }
    }
}