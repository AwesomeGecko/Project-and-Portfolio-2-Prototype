using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataAudioPersistence
{
    
    void LoadData(AudioData data);
    void SaveData(AudioData data);
}
