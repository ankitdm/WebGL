using System;
using System.Collections;
using UnityEngine;

public class WebViewManager : MonoBehaviour
{
    [SerializeField]
    private UniWebView webView;

    private void Start()
    {
        InitializeWebView();
    }

    private void InitializeWebView()
    {
        UniWebView.SetJavaScriptEnabled(true);
        
        // UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;
        // UniWebView.SetForwardWebConsoleToNativeOutput(true);
         
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.BackgroundColor = Color.black;
        
        //webView.OnMessageReceived += OnMessageReceived;
        //webView.OnLoadingErrorReceived += OnLoadingErrorReceived;
        
        webView.SetAllowFileAccess(true);   
    }

    private void OnLoadingErrorReceived(UniWebView webView, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
    {
        Debug.Log("Error Message: " + errorMessage);
    }

    public void LoadGame(string _fileUrl)
    {
        Debug.Log("Url to Launch: " + _fileUrl);
        StartCoroutine(LoadWebGLGame(_fileUrl));
    }

    public IEnumerator LoadWebGLGame(string fileUrl)
    {
        yield return new WaitForEndOfFrame();
        
        webView.Load(fileUrl);
        webView.Show();
    }

    private void OnMessageReceived(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log("Message from WebGL: " + message.RawMessage);
    }

    public void SendMessageToWebGL(string message)
    {
        webView.EvaluateJavaScript($"handleUnityMessage('{message}');");
    }

    public void CloseWebView()
    {
        if (webView != null)
        {
            webView.Hide();
            Destroy(webView);
        }
    }

    private void Update()
    {
        if (Screen.width != webView.Frame.width || Screen.height != webView.Frame.height)
        {
            if(webView)
            {
                webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            }
        }
    }
}
