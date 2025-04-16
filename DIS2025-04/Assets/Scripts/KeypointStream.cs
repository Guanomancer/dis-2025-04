using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class KeypointStream : MonoBehaviour
{
    [Header("Keypoint Stream Settings")]
    [Tooltip("API endpoint")]
    [SerializeField] string _keypointStreamUrl = "https://localhost:7002/api/Retrieve";
    [Tooltip("Start fetching when this component is enabled.")]
    [SerializeField] bool _fetchRepeadetly;
    [Tooltip("Number of simultanious fetching coroutines.")]
    [SerializeField, Range(1, 5)] int _fetchRedundancy = 2;

    [Header("Events")]
    [SerializeField] private UnityEvent<string> _onFrameReceived;
    [SerializeField] private UnityEvent<string> _onRequestError;

    private List<Coroutine> _fetchCoroutines = new();
    
    private void OnEnable()
    {
        if (!_fetchRepeadetly) return;

        StartCoroutine(_startFetching());

        IEnumerator _startFetching()
        {
            for (int i = 0; i < _fetchRedundancy; i++)
            {
                yield return null;
                _fetchCoroutines.Add(StartCoroutine(FetchKeypointsCo()));
            }
        }
    }

    private IEnumerator FetchKeypointsCo()
    {
        while (enabled)
        {
            using (var webRequest = UnityWebRequest.Get(_keypointStreamUrl))
            {
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        var error = $"Error: {webRequest.error}";
                        Debug.LogError(error, this);
                        if (_onFrameReceived != null) _onRequestError.Invoke(error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        var httpError = $"HTTP Error: {webRequest.error}";
                        Debug.LogError(httpError, this);
                        if (_onFrameReceived != null) _onRequestError.Invoke(httpError);
                        break;
                    case UnityWebRequest.Result.Success:
                        var result = webRequest.downloadHandler.text;
                        Debug.Log(result, this);
                        if (_onFrameReceived != null) _onFrameReceived.Invoke(result);
                        break;
                }
            }
        }
    }
}
