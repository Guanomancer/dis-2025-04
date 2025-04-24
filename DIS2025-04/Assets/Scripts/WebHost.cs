using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Playables;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private const long HTTP_POST_BUFFER_SIZE = 10240;

    [SerializeField] string _url = "http://localhost:8080/";
    [SerializeField] WebPageMapping[] _webPages;

    private HttpListener _listener;

    private Dictionary<string, string> _pages;
    private Dictionary<string, Func<string[], string>> _apiEndpoints;

    private Queue<FrameDataEntry> _frameBuffer;

    private void Start()
    {
        _pages = new();
        foreach (var page in _webPages)
        {
            _pages.Add(page.Url, page.Asset.text);
        }

        _apiEndpoints = new();
        _apiEndpoints.Add("Device", API_Device);
        _apiEndpoints.Add("Store", API_Store);
        _apiEndpoints.Add("Retrieve", API_Retrieve);

        _frameBuffer = new();

        _listener = new();
        _listener.Prefixes.Add(_url);
        _listener.Start();
        _listener.BeginGetContext(GetContextCallback, null);

        StartCoroutine(Fetch(_url));
        StartCoroutine(Fetch(_url + "api/Device"));
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

    private void GetContextCallback(IAsyncResult result)
    {
        var context = _listener.EndGetContext(result);
        var path = context.Request.Url.LocalPath;

        if (path.StartsWith("/api/")) RouteApiRequest(context, path);
        else ProcessWebRequest(context, path);

        _listener.BeginGetContext(GetContextCallback, null);
    }

    private void ProcessWebRequest(HttpListenerContext context, string url)
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
            Debug.Log($"Serving 404 for page {url}");
        }

        var buffer = System.Text.Encoding.UTF8.GetBytes(html);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private void RouteApiRequest(HttpListenerContext context, string path)
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
            Debug.Log($"Serving 400 for API /{endPointName}");
        }
        else
        {
            var postData = string.Empty;
            var inStream = context.Request.InputStream;
            if (inStream != null && inStream.Length < HTTP_POST_BUFFER_SIZE)
            {
                int len = (int)inStream.Length;
                var inBuffer = new byte[len];
                inStream.Read(inBuffer, 0, len);
                postData = Encoding.UTF8.GetString(inBuffer);
            }
            string[] query;
            if (pathParts.Length < 4)
            {
                query = new string[1];
            }
            else
            {
                query = new string[pathParts.Length - 3 + 1];
                Array.Copy(pathParts, 3, query, 1, query.Length);
            }
            query[0] = postData;
            json = _apiEndpoints[endPointName].Invoke(query);
            if (json == null)
            {
                context.Response.StatusCode = 404;
                json = string.Empty;
                Debug.Log($"Serving 400 for API /{endPointName}");
            }
            context.Response.StatusCode = 200;
            Debug.Log($"Serving API /{endPointName}");
        }

        var buffer = Encoding.UTF8.GetBytes(json);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private string API_Device(string[] query)
    {
        return Guid.NewGuid().ToString();
    }

    private string API_Store(string[] query)
    {
        if (!Guid.TryParse(query[1], out Guid deviceId)) return null;
        if (!int.TryParse(query[2], out int frameIndex)) return null;
        
        var frameData = query[0];
        _frameBuffer.Enqueue(new(deviceId.ToString(), frameIndex, frameData));
        Debug.Log($"Stored frame #{frameIndex} for device {{{deviceId}}} with => {frameData}");

        return "OK";
    }

    private string API_Retrieve(string[] query)
    {
        if (!_frameBuffer.TryDequeue(out var frame)) return null;

        var json = JsonUtility.ToJson(frame);
        Debug.Log($"Retrieved frame #{frame.FrameIndex} with {json}");

        return json;
    }
}

[Serializable]
public struct WebPageMapping
{
    public string Url;
    public TextAsset Asset;
}

[Serializable]
public struct FrameDataEntry
{
    public string DeviceId;
    public int FrameIndex;
    public string FrameDataJson;

    public FrameDataEntry(string deviceId, int frameIndex, string frameDataJson)
    {
        DeviceId = deviceId;
        FrameIndex = frameIndex;
        FrameDataJson = frameDataJson;
    }
}