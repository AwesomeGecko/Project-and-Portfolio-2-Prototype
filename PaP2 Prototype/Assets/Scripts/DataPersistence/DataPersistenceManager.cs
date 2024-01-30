using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TransitionsPlus;


public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    //[SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string GameSaveName;
    [SerializeField] private string AudioSaveName;

    [SerializeField] private bool useEncryption;

    [Header("Auto Save")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private GameData gameData;
    private AudioData audioData;
    private List<IDataPersistence> dataPersistenceObjects;
    private List<IDataAudioPersistence> audioDataPersistenceObjects;

    private FileDataHandler gameDataHandler;
    private AudioFileDataHandeler audioDataHandler;

    private Coroutine autoSaveCoroutine;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        //initializeDataIfNull = true;

        if (instance != null)
        {
            //Debug.LogError("Found more than one Data Manager in the scene. I will destroy the newest one!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        gameDataHandler = new FileDataHandler(Application.persistentDataPath, GameSaveName, useEncryption);
        audioDataHandler = new AudioFileDataHandeler(Application.persistentDataPath, AudioSaveName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Loaded Scene method called!");
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        audioDataPersistenceObjects = FindAllAudioDataPersistenceObjects();
        LoadGame();
        LoadAudio();

        //start autoSaving after loading the scene
        if (autoSaveCoroutine != null)
        { 
            StopCoroutine(AutoSave());
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    public void OnSceneUnloaded(Scene scene)
    {
        //Debug.Log("Unloaded Scene method called!");
        SaveGame();
        SaveAudio();
    }


    public void NewAudio()
    { 
        audioData = new AudioData();
    }
    public void NewGame()
    { 
        this.gameData = new GameData();
    }

    #region GameData
    public void LoadGame()
    {
        //loads any saved data from a file using the data handler
        gameData = gameDataHandler.Load();

        //if no data can be loaded, dont continue
        if (this.gameData == null)
        {
            
            //Debug.Log("No data was found. A new Game needs to be started before data can be loaded");
            return;
            
        }
        //push all loaded data to their respective scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //if theres no data to save, log a warning here
        if (this.gameData == null)
        {
            //Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
            return;
        }

        //passes the data to other scripts so it can be updated
        //then saves data to a file using the data handler
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        
        //saves data to the file
        gameDataHandler.Save(gameData);
        
    }
    #endregion

    #region AudioData
    public void LoadAudio()
    {
        //loads any saved data from a file using the data handler
        audioData = audioDataHandler.Load();

        //if no data can be loaded, dont continue
        if (audioData == null)
        {
            //Debug.Log("No 'AudioSave' file was found. I'll make one!");
            NewAudio();
            return;

        }
        //push all loaded data to their respective scripts
        foreach (IDataAudioPersistence dataPersistenceObj in audioDataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(audioData);
        }
        //Debug.Log($"Loaded Main: {audioData.mainSlider}, Music: {audioData.musicSlider}, Sfx: {audioData.sfxSlider}");
    }

    public void SaveAudio()
    {
        foreach (IDataAudioPersistence dataPersistenceObj in audioDataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(audioData);
            //Debug.Log($"Saved: {audioData}");
        }

        //saves data to the file
        audioDataHandler.Save(audioData);
        //Debug.Log($"Saved Main: {audioData.mainSlider}, Music: {audioData.musicSlider}, Sfx: {audioData.sfxSlider}");
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveGame();
        SaveAudio();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private List<IDataAudioPersistence> FindAllAudioDataPersistenceObjects()
    {
        IEnumerable<IDataAudioPersistence> audioDataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataAudioPersistence>();

        return new List<IDataAudioPersistence>(audioDataPersistenceObjects);
    }

    public int PlayerLevel()
    {
        return gameData.level;
    }

    public bool HasGameData()
    { 
        return gameData != null;
    }

    public bool HasAudioData()
    {
        return audioData != null;
    }

    private IEnumerator AutoSave()
    { 
        while (true) 
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            //Debug.Log("Auto Saved!");
        }
    }
}
