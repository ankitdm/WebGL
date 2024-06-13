using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{
    private void Start()
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "AlienShooter");
        string destinationPath = Path.Combine(Application.persistentDataPath, "AlienShooter");

        CopyDirectory(sourcePath, destinationPath);

        Debug.Log("Copy completed");
    }

    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        // Create the destination directory if it doesn't exist
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }

        // Copy all files from source to destination
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            if (Application.platform == RuntimePlatform.Android)
            {
                // Android uses a special method to read StreamingAssets files
                WWW reader = new WWW(file);
                while (!reader.isDone) { }
                File.WriteAllBytes(destFile, reader.bytes);
            }
            else
            {
                File.Copy(file, destFile, true);
            }
        }

        // Recursively copy subdirectories
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, destDir);
        }
    }
}
