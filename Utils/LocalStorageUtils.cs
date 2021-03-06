using System;
using System.IO;
using Plugins.UnityMonstackCore.Loggers;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Utils
{
    public class LocalStorageUtils
    {
        public static string GetApplicationPathToFile(FileSourceType fileSourceType, string pathToFile)
        {
            switch (fileSourceType)
            {
                case FileSourceType.Resources:
                    return Application.dataPath + "/Resources/" + pathToFile;
                case FileSourceType.ApplicationPersistentData:
                    return Application.persistentDataPath + "/" + pathToFile;
            }

            throw new NotImplementedException();
        }

        public static string GetApplicationPathToFile(string pathToFile)
        {
            return Application.persistentDataPath + "/" + pathToFile;
        }

        public static byte[] LoadBytesFromFile(string pathToFile)
        {
            return LoadBytesFromFile(FileSourceType.ApplicationPersistentData, pathToFile);
        }

        public static byte[] LoadBytesFromFile(FileSourceType fileSourceType, string pathToFile)
        {
            try
            {
                var applicationPathToFile = GetApplicationPathToFile(fileSourceType, pathToFile);
                using (var reader = new BinaryReader(File.Open(applicationPathToFile, FileMode.Open)))
                {
                    return reader.ReadAllBytes();
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
            try
            {
                using (var reader = new StreamReader(pathToFile))
                {
                    return JsonUtility.FromJson<T>(reader.ReadToEnd());
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to load data from file " + pathToFile;
                UnityLogger.Error(errorMessage);
                throw;
            }
        }

        public static T LoadJSONSerializedObjectFromData<T>(string data)
        {
            try
            {
                return JsonUtility.FromJson<T>(data);
            }
            catch (Exception e)
            {
                UnityLogger.Error("Failed to load serialized json data", e);
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