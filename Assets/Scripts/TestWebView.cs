using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class TestWebView : MonoBehaviour
{
    [SerializeField]
    Transform backButton;

    private string url = "https://ankitdm.github.io/AlienShooter/index.html";

    private Constants.HostingStyle hostingStyle;

    private SimpleHttpServer httpServer;

    void Start()
    {
        hostingStyle = (Constants.HostingStyle)PlayerPrefs.GetInt("GameType");

        StartCoroutine(LoadGame());
    }
    
    IEnumerator LoadGame()
    {
        yield return null;

        switch (hostingStyle)
        {
            case Constants.HostingStyle.GitHosting:
            {
                UnityEngine.Debug.Log("Launch from GitHUb");
                GetComponent<WebViewManager>().LoadGame(url);

                break;
            }
            case Constants.HostingStyle.PersistentDataPath:
            {
                StartLocalServer();

                yield return new WaitForSeconds(1);
                
                string _localServerUrl = "http://localhost:8080/index.html";
                
                GetComponent<WebViewManager>().LoadGame(_localServerUrl);

                break;
            }
            default:
            {
                UnityEngine.Debug.Log("Launch Default");
                GetComponent<WebViewManager>().LoadGame(url);
                break;
            }
        }
    }

    private void StartLocalServer()
    {
        string webGLPath = Path.Combine(Application.persistentDataPath, "AlienShooter");

        // Set up the HTTP server
        httpServer = new SimpleHttpServer(webGLPath, 8080);

        // Start the server in a new thread to prevent blocking the main thread
        new Thread(httpServer.Start).Start();

        Debug.Log("LocalWebGLServer=> StartLocalServer=> Starting..");
    }

    public void BackBtn()
    {
        SceneManager.LoadScene("MainMenu");
    }
}