using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        //Path.Combine allows any system to use the file path. MAC, Windows, etc. all have their own set up for file paths
        string fullPath = Path.Combine(Application.persistentDataPath, dataDirPath, dataFileName);
        GameData LoadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    { 
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize the data from the JSON file back to the object
                LoadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from the file: " + fullPath + "\n" + e);
            }
        }
        return LoadedData;
    }

    public void Save(GameData data)
    {
        //Path.Combine allows any system to use the file path. MAC, Windows, etc. all have their own set up for file paths
        string fullPath = Path.Combine(Application.persistentDataPath, dataDirPath, dataFileName);
        try
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
            //creates directory path if it doesnt exist
            //Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize game data into a JSON file
            string dataToStore = JsonUtility.ToJson(data, true);

            //writes the data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                { 
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
