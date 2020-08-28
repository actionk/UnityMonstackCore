using System;
using System.IO;
using Plugins.UnityMonstackContentLoader;
using Plugins.UnityMonstackCore.Loggers;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Utils
{
    public class LocalStorageUtils
    {
        public static void SaveSerializedObjectToFile(string pathToFile, object data)
        {
            var applicationPathToFile = GetApplicationPathToFile(pathToFile);
            CreateDirectoryIfNotExistsWithActualPath(Path.GetDirectoryName(applicationPathToFile));

            try
            {
                using (var writer = new StreamWriter(applicationPathToFile))
                {
                    var json = JsonUtility.ToJson(data);
                    writer.Write(json);
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to save data to file " + applicationPathToFile;
                UnityLogger.Error(errorMessage);
                throw;
            }
        }

        public static void SaveBytesToFile(string pathToFile, byte[] bytes)
        {
            var applicationPathToFile = GetApplicationPathToFile(pathToFile);
            CreateDirectoryIfNotExistsWithActualPath(Path.GetDirectoryName(applicationPathToFile));

            try
            {
                File.Delete(applicationPathToFile);
                using (var writer = new BinaryWriter(File.OpenWrite(applicationPathToFile)))
                {
                    writer.Write(bytes);
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to save bytes data to file " + applicationPathToFile;
                UnityLogger.Error(errorMessage);
                throw;
            }
        }

        public static string GetApplicationPathToFile(string pathToFile)
        {
            return Application.persistentDataPath + "/" + pathToFile;
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            var pathToDirectory = GetApplicationPathToFile(path);
            CreateDirectoryIfNotExistsWithActualPath(pathToDirectory);
        }

        private static void CreateDirectoryIfNotExistsWithActualPath(string pathToDirectory)
        {
            if (!Directory.Exists(pathToDirectory)) Directory.CreateDirectory(pathToDirectory);
        }

        public static byte[] LoadBytesFromFile(string pathToFile)
        {
            return LoadBytesFromFile(FileSourceType.ApplicationPersistentData, pathToFile);
        }

        public static byte[] LoadBytesFromFile(FileSourceType fileSourceType, string pathToFile)
        {
            try
            {
                switch (fileSourceType)
                {
                    case FileSourceType.Resources:
                        return Resources.Load<TextAsset>(pathToFile).bytes;

                    case FileSourceType.ApplicationPersistentData:
                        var applicationPathToFile = GetApplicationPathToFile(pathToFile);
                        using (var reader = new BinaryReader(File.Open(applicationPathToFile, FileMode.Open)))
                        {
                            return reader.ReadAllBytes();
                        }
                }
            }
            catch (Exception)
            {
                var errorMessage = $"Failed to load byte data from source {fileSourceType}, path {pathToFile}";
                UnityLogger.Error(errorMessage);
                throw;
            }

            return null;
        }

        public static T LoadSerializedObjectFromFile<T>(string pathToFile)
        {
            var applicationPathToFile = GetApplicationPathToFile(pathToFile);
            try
            {
                using (var reader = new StreamReader(applicationPathToFile))
                {
                    return JsonUtility.FromJson<T>(reader.ReadToEnd());
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to load data from file " + applicationPathToFile;
                UnityLogger.Error(errorMessage);
                throw;
            }
        }

        public static Optional<T> LoadSerializedObjectFromFileIfExists<T>(string pathToFile)
        {
            var applicationPathToFile = GetApplicationPathToFile(pathToFile);
            if (!File.Exists(applicationPathToFile)) return Optional<T>.Empty();

            return Optional<T>.OfValue(LoadSerializedObjectFromFile<T>(pathToFile));
        }
    }
}