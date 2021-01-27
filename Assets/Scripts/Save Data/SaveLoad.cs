using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public static class SaveLoad
    {
        private static string SaveDataPath => Application.persistentDataPath + "/save.dat";

        /// <summary>
        /// Save the current save data to save.dat.
        /// </summary>
        public static void Save()
        {
            BunniesToSaveData();

            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Create(SaveDataPath))
            {
                bf.Serialize(file, SaveData.current);
            }

            Debug.Log("DATA SAVED");
        }

        /// <summary>
        /// Load save data from save.dat.
        /// </summary>
        public static void Load()
        {
            if (SaveExists())
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream file = File.Open(SaveDataPath, FileMode.Open))
                {
                    SaveData.current = (SaveData)bf.Deserialize(file);
                }

                Debug.Log("DATA LOADED");
            }
            else
            {
                SaveData.current = new SaveData();
                Debug.Log("SAVE DATA NOT FOUND! NEW SAVE DATA CREATED!");
            }
        }

        /// <summary>
        /// Delete save.dat if it exists.
        /// </summary>
        public static void Delete()
        {
            if (!SaveExists())
                return;

            File.Delete(SaveDataPath);
            Debug.Log("SAVE DATA DELETED!");
        }

        /// <summary>
        /// Check whether save.dat exists.
        /// </summary>
        /// <returns>True if save.dat exists. False otherwise.</returns>
        public static bool SaveExists()
        {
            return File.Exists(SaveDataPath);
        }

        /// <summary>
        /// Get save data values for the bunnies from the game manager.
        /// </summary>
        private static void BunniesToSaveData()
        {
            GameManager gameManager = GameManager.Instance;
            SaveData.current.bunnightName = gameManager.Bunnight.name;
            SaveData.current.bunnightExp = gameManager.Bunnight.Experience;
            SaveData.current.bunnecromancerName = gameManager.Bunnecromancer.name;
            SaveData.current.bunnecromancerExp = gameManager.Bunnecromancer.Experience;
            SaveData.current.bunnurseName = gameManager.Bunnurse.name;
            SaveData.current.bunnurseExp = gameManager.Bunnurse.Experience;
            SaveData.current.bunneerdowellName = gameManager.Bunneerdowell.name;
            SaveData.current.bunneerdowellExp = gameManager.Bunneerdowell.Experience;
        }
    }
}