using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;
using System;

public class DownloadAndExtract : MonoBehaviour
{
    private string zipUrl = "https://github.com/ankitdm/WebGL/releases/download/AlienShooter/AlienShooter.zip";
    private string zipFilePath;
    private string extractPath;

    void Start()
    {

    }

    public void StartDownload()
    {
        zipFilePath = Path.Combine(Application.persistentDataPath, "AlienShooter.zip");
        extractPath = Path.Combine(Application.persistentDataPath, "ExtractedFiles");

        StartCoroutine(DownloadZipWithRetries(zipUrl, zipFilePath));
    }

    private IEnumerator DownloadZipWithRetries(string url, string outputPath, int retries = 3)
    {
        int attempts = 0;
        bool success = false;

        while (attempts < retries && !success)
        {
            attempts++;
            Debug.Log($"Attempt {attempts} to download ZIP file from {url}");

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Check for valid content type
                    string contentType = www.GetResponseHeader("Content-Type");
                    Debug.Log($"Content-Type: {contentType}");

                    if (contentType == "application/zip" || contentType == "application/octet-stream")
                    {
                        File.WriteAllBytes(outputPath, www.downloadHandler.data);
                        Debug.Log($"ZIP file downloaded to: {outputPath}");
                        Debug.Log($"File Size: {www.downloadHandler.data.Length} bytes");

                        if (VerifyZipFile(outputPath))
                        {
                            success = true;
                            ExtractZip(outputPath, extractPath);
                        }
                        else
                        {
                            Debug.LogError("Downloaded ZIP file is invalid or corrupted.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Unexpected content type: {contentType}");
                    }
                }
                else
                {
                    Debug.LogError($"Error downloading ZIP file: {www.error} (Response Code: {www.responseCode})");
                }
            }

            if (!success && attempts < retries)
            {
                Debug.Log("Retrying download...");
                yield return new WaitForSeconds(2); // Wait before retrying
            }
        }

        if (!success)
        {
            Debug.LogError($"Failed to download and verify ZIP file after {attempts} attempts.");
        }
    }

    private bool VerifyZipFile(string zipFilePath)
    {
        try
        {
            using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            {
                using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Just iterating through entries to check integrity
                    }
                }
            }
            return true;
        }
        catch (InvalidDataException e)
        {
            Debug.LogError($"InvalidDataException: {e.Message}");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception: {e.Message}");
            return false;
        }
    }

    private void ExtractZip(string zipFilePath, string extractPath)
    {
        if (!Directory.Exists(extractPath))
        {
            Directory.CreateDirectory(extractPath);
        }

        try
        {
            using (FileStream fs = new FileStream(zipFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(fs))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string fullPath = Path.Combine(extractPath, entry.FullName);
                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(fullPath);
                        }
                        else
                        {
                            // Handle file conflict
                            if (File.Exists(fullPath))
                            {
                                Debug.LogWarning($"File already exists: {fullPath}. Skipping...");
                                continue;
                            }

                            // Ensure directory exists
                            string directoryName = Path.GetDirectoryName(fullPath);
                            if (!Directory.Exists(directoryName))
                            {
                                Directory.CreateDirectory(directoryName);
                            }

                            entry.ExtractToFile(fullPath);
                        }
                    }
                }
            }

            Debug.Log($"ZIP file extracted to: {extractPath}");

            GetComponent<MainMenuManager>().StartGame((int)Constants.HostingStyle.DownloadAndExtract);
        }
        catch (IOException ioEx)
        {
            Debug.LogError($"IOException: {ioEx.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
        }
    }
}
