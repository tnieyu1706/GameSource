using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _Project.Scripts.Persistence
{
    [Serializable]
    public class GameDataService : IDataService<GameData>
    {
        ISerializer _serializer;
        private string dataPath;
        private string fileExtension;
        public string testValue = "test";

        public GameDataService(ISerializer serializer)
        {
            _serializer = serializer;
            this.dataPath = Application.persistentDataPath;
            this.fileExtension = "json";
        }

        private string GetPathToFile(string fileName)
        {
            return Path.Combine(this.dataPath, string.Concat(fileName,".", this.fileExtension));
        }

        public void Save(string name, GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(name);
            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException("File already exists.");
            }
            
            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);
            if (!File.Exists(fileLocation))
            {
                throw new IOException("File does not exist.");
            }
            return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.GetFiles(this.dataPath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(this.dataPath))
            {
                if (Path.GetExtension(path) == this.fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}