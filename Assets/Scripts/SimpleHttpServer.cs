using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public class SimpleHttpServer
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly string _baseFolder;
    private readonly int _port;

    public SimpleHttpServer(string baseFolder, int port)
    {
        _baseFolder = baseFolder;
        _port = port;
        _listener.Prefixes.Add($"http://*:{_port}/");
    }

    public void Start()
    {
        _listener.Start();
        Console.WriteLine($"Server started at http://localhost:{_port}/");

        while (true)
        {
            var context = _listener.GetContext();
            ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
        }
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void HandleRequest(HttpListenerContext context)
    {
        string url = context.Request.Url.AbsolutePath.Trim('/');
        string filePath = Path.Combine(_baseFolder, url);

        if (Directory.Exists(filePath))
        {
            filePath = Path.Combine(filePath, "index.html");
        }

        if (File.Exists(filePath))
        {
            byte[] content = File.ReadAllBytes(filePath);
            context.Response.ContentType = GetContentType(filePath);
            context.Response.ContentLength64 = content.Length;
            context.Response.OutputStream.Write(content, 0, content.Length);
        }
        else
        {
            context.Response.StatusCode = 404;
            byte[] content = Encoding.UTF8.GetBytes("404 - File Not Found");
            context.Response.OutputStream.Write(content, 0, content.Length);
        }

        context.Response.OutputStream.Close();
    }

    private string GetContentType(string path)
    {
        switch (Path.GetExtension(path))
        {
            case ".html":
                return "text/html";
            case ".css":
                return "text/css";
            case ".js":
                return "application/javascript";
            case ".png":
                return "image/png";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            default:
                return "application/octet-stream";
        }
    }
}