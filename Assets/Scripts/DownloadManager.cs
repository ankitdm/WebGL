using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DownloadManager : MonoBehaviour
{
    private string webGLZipUrl = "https://github.com/ankitdm/WebGL/blob/main/Games/AlienShooter.zip";
    private string localPath;

    private void Start()
    {
        
    }

    public void DownloadAndUnzipFile()
    {
       webGLZipUrl = Path.Combine(Application.streamingAssetsPath , "AlienShooter.zip");
        Debug.Log(webGLZipUrl);   

        localPath = Application.persistentDataPath; //Path.Combine(Application.persistentDataPath, "AlienShooter");
        
        // StartCoroutine(DownloadAndUnzipWebGLGame());
        StartCoroutine(DownloadAndUnzipLocal());
    }

    private IEnumerator DownloadAndUnzipWebGLGame()
    {
        UnityWebRequest request; //  = UnityWebRequest.Get(webGLZipUrl);
        using (request = UnityWebRequest.Get(webGLZipUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SaveAndUnzipFile(request.downloadHandler.data);
            }
            else
            {
                Debug.LogError("Error downloading WebGL game zip: " + request.error);
            }
        }        
    }

    private IEnumerator DownloadAndUnzipLocal()
    {
         if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }
        
        ZipFile.ExtractToDirectory(webGLZipUrl, localPath);
        Debug.Log("LocalPath where zip extracted: " + localPath);

        yield return new WaitForSeconds(1);

        // CreateHtmlWrapper();
    }

    private void SaveAndUnzipFile(byte[] data)
    {
        string zipFilePath = Path.Combine(Application.persistentDataPath, "AlienShooter.zip");
        File.WriteAllBytes(zipFilePath, data);
        Debug.Log("Zip file saved to: " + zipFilePath);

        // Unzip the file
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }
        
        ZipFile.ExtractToDirectory(zipFilePath, localPath);
        Debug.Log("Unzipped WebGL game to: " + localPath);

        // Create an HTML wrapper
        CreateHtmlWrapper();
    }

    private void CreateHtmlWrapper()
    {
        string htmlWrapperPath = Path.Combine(localPath, "wrapper.html");
        string indexHtmlPath = Path.Combine(localPath, "index.html");

        string htmlContent = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>WebGL Game</title>
        </head>
        <body>
            <iframe src=""{indexHtmlPath}"" width=""100%"" height=""100%"" frameborder=""0""></iframe>
        </body>
        </html>";

        File.WriteAllText(htmlWrapperPath, htmlContent);
        Debug.Log("HTML wrapper created at: " + htmlWrapperPath);

        // Now load the WebGL game using the HTML wrapper
        PlayerPrefs.SetString("GameUrl", ("file://" + htmlWrapperPath));
        PlayerPrefs.SetInt("GameType", 4);
       
        SceneManager.LoadScene("SampleScene");

        // GetComponent<WebViewManager>().LoadWebGLGame("file://" + htmlWrapperPath);
    }
}
