using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LoadInfo
{
    public Vector3 playerPosition;
    public UnityEvent OnLoad;
}

public class SceneLoader : MonoBehaviour
{
    //-------PROPERTY




    //-------FIELD
    [SerializeField]
    private PlayerContorller _player;

    [SerializeField]
    private List<LoadInfo> _loadsInfo = new List<LoadInfo>();




    //-------METODS
    void Start()
    {
        LoadCurrent();
    }

    public void LoadCurrent()
    {
        if (!PlayerPrefs.HasKey("LoadInfo"))
            PlayerPrefs.SetInt("LoadInfo", 0);

        SetLoad(PlayerPrefs.GetInt("LoadInfo"));
    }

    public void NextLoad()
    {
        if (!PlayerPrefs.HasKey("LoadInfo"))
            PlayerPrefs.SetInt("LoadInfo", 0);

        int currentInfo = PlayerPrefs.GetInt("LoadInfo");

        currentInfo++;
        SetLoad(currentInfo);
    }

    public void SetLoad(int load)
    {
        if (load >= _loadsInfo.Count)
            return;

        if (_player != null)
            _player.MyTransform.position = _loadsInfo[load].playerPosition;

        _loadsInfo[load].OnLoad?.Invoke();

        PlayerPrefs.SetInt("LoadInfo", load);
    }

    public void ResetLoad()
    {
        PlayerPrefs.SetInt("LoadInfo", 0);
    }
}
