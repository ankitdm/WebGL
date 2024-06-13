using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    UnityWebRequest _request;
    bool isDownloading;
    float percent;

    private void Start()
    {
        isDownloading = false;
    }

    public void StartGame(int _hostingStyle)
    {
        PlayerPrefs.SetInt("GameType", _hostingStyle);
        
        SceneManager.LoadScene("SampleScene");
    }
}