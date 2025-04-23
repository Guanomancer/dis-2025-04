using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] string _url = "http://localhost:8080/";

    [SerializeField] WebPageMapping[] _webPages;

    private System.Net.HttpListener _listener;
    private System.Net.WebClient _client;

    private Dictionary<string, string> _pages;
    private Dictionary<string, Func<string[], string>> _apiEndpoints;

    private void Start()
    {
        _pages = new();
        foreach (var page in _webPages)
        {
            _pages.Add(page.Url, page.Asset.text);
        }

        _apiEndpoints = new();
        _apiEndpoints.Add("Device", API_Device);

        _listener = new System.Net.HttpListener();
        _listener.Prefixes.Add(_url);
        _listener.Start();
        _listener.BeginGetContext(GetContextCallback, null);

        StartCoroutine(Fetch(_url));
        StartCoroutine(Fetch(_url + "api/Device"));
    }

    private void GetContextCallback(System.IAsyncResult result)
    {
        var context = _listener.EndGetContext(result);
        var path = context.Request.Url.LocalPath;

        if (path.StartsWith("/api/")) RouteApiRequest(context, path);
        else ProcessWebRequest(context, path);

        _listener.BeginGetContext(GetContextCallback, null);
    }

    private void ProcessWebRequest(System.Net.HttpListenerContext context, string url)
    {
        url = url == "/" ? "/index.html" : url.ToLower();
        var html = string.Empty;

        if (_pages.ContainsKey(url))
        {
            html = _pages[url];
            context.Response.StatusCode = 200;
            Debug.Log($"Serving page {url}");
        }
        else
        {
            html = "Page not found!<br/>" + url;
            context.Response.StatusCode = 404;
            Debug.Log($"Serving 404 for page " + url);
        }

        var buffer = System.Text.Encoding.UTF8.GetBytes(html);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private void RouteApiRequest(System.Net.HttpListenerContext context, string path)
    {
        var pathParts = path.Split("/");
        var endPointName = string.Empty;
        var json = string.Empty;
        if (pathParts.Length >= 3)
        {
            endPointName = pathParts[2];
        }

        if (string.IsNullOrEmpty(endPointName) || !_apiEndpoints.ContainsKey(endPointName))
        {
            json = "";
            context.Response.StatusCode = 404;
            Debug.Log($"Serving 404 for API /" + endPointName);
        }
        else
        {
            string[] query;
            if (pathParts.Length < 4)
            {
                query = new string[0];
            }
            else
            {
                query = new string[pathParts.Length - 3];
                Array.Copy(pathParts, 3, query, 0, query.Length);
            }
            json = _apiEndpoints[endPointName].Invoke(query);
            context.Response.StatusCode = 200;
            Debug.Log($"Serving API /" + endPointName);
        }

        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private string API_Device(string[] query)
    {
        return Guid.NewGuid().ToString();
    }

    private IEnumerator Fetch(string url)
    {
        yield return null;
        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    var error = $"Error: {webRequest.error}";
                    Debug.LogError(error, this);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    var httpError = $"HTTP Error: {webRequest.error}";
                    Debug.LogError(httpError, this);
                    break;
                case UnityWebRequest.Result.Success:
                    var result = webRequest.downloadHandler.text;
                    Debug.Log(result, this);
                    break;
            }
        }
    }
}

[System.Serializable]
public struct WebPageMapping
{
    public string Url;
    public TextAsset Asset;
}