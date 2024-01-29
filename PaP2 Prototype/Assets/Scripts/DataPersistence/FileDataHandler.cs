using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "ZaYnE Is tHe bEsT";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        //Path.Combine allows any system to use the file path. MAC, Windows, etc. all have their own set up for file paths
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData LoadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //deserialize the data from the JSON file back to the object
                LoadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                
                    //Debug.Log("Loaded Data!");
                
            }
            catch (Exception )
            {
               //Debug.LogError("Error occured when trying to load data from the file: " + fullPath + "\n" + e);
            }
        }
        
        return LoadedData;
    }

    public void Save(GameData data)
    {
        //Path.Combine allows any system to use the file path. MAC, Windows, etc. all have their own set up for file paths
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            //creates directory path if it doesnt exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize game data into a JSON file
            string dataToStore = JsonUtility.ToJson(data, true);

            //Encryptdata before it is saved
            if (useEncryption)
            { 
                dataToStore = EncryptDecrypt(dataToStore);
            }

            //writes the data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);

                }
            }

                //Debug.Log("Saved Data!");
            
        }
        catch (Exception )
        {
            //Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
        
        
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++) 
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}
