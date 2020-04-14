using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveLoad
{
    // Save the current SaveData to save.dat
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, SaveData.current);
        file.Close();
        Debug.Log("DATA SAVED");
    }

    // Load SaveData from save.dat
    public static void Load()
    {
        if (SaveExists())
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            SaveData.current = (SaveData)bf.Deserialize(file);
            file.Close();
            Debug.Log("DATA LOADED");
        }
        else
        {
            SaveData.current = new SaveData();
            Debug.LogWarning("DATA NOT FOUND! NEW DATA CREATED!");
        }
    }

    // Returns whether save.dat exists
    public static bool SaveExists()
    {
        return File.Exists(Application.persistentDataPath + "/save.dat");
    }
}
