using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class Gestures : MonoBehaviour
{
    [SerializeField] private UnityEvent<GameObject> _onHoverChanged;
    [SerializeField] private UnityEvent<GameObject> _onSelectionChanged;

    [Header("Capture Settings")]
    [Tooltip("If you are running the HTTP capture host remotely, you can disable this.")]
    [SerializeField] private bool _runHttpHost;
    [Tooltip("The HTTP endpoint for the HTTP capture server. (Example: http://localhost:8080)")]
    [SerializeField] private string _httpHostEndpoint = "http://localhost:8080";
    [Tooltip("Example: /index.html and /handpose.js")]
    [SerializeField] private WebPageMapping[] _pageMappings;

    [Header("Motion Settings")]
    [SerializeField] private MotionController _motionController;
    [SerializeField] private bool _debugDrawHands;
    [SerializeField] private Canvas _debugCanvas;
    [SerializeField] private GameObject _debugDotTemplate;
    
    private HttpHost _httpHost;
    private HandTrackingData _deserializer;

    public GameObject CurrentHover { get; private set; }
    public GameObject CurrentSelected { get; private set; }

    static Gestures _instance;

    private void Awake()
    {
        if (!InitializeSingleton()) return;
        InitializeHttpHost();
        InitializeDeserializer();
        InitializeDebugDots();



        bool InitializeSingleton()
        {
            if (_instance != null)
            {
                Debug.LogWarning($"{nameof(Gestures)} already exists. Destroying this instance.", this);
                DestroyImmediate(gameObject);
                return false;
            }

            _instance = this;
            return true;
        }

        bool InitializeHttpHost()
        {
            if (!_runHttpHost) return false;
            if (string.IsNullOrEmpty(_httpHostEndpoint))
            {
                Debug.LogError("HTTP Host endpoint is not set.", this);
                return false;
            }

            _httpHost = new(_httpHostEndpoint, _pageMappings);

            return true;
        }

        bool InitializeDeserializer()
        {
            if (_motionController == null)
            {
                Debug.LogError("MotionController is not assigned.", this);
                return false;
            }

            _deserializer = new();
            foreach (var keypoint in _motionController.KeypointBindings)
            {
                _deserializer.Keypoints.Add(keypoint.Keypoint.keypointName, keypoint.Keypoint);
            }

            return true;
        }

        bool InitializeDebugDots()
        {
            if (_debugCanvas == null)
            {
                Debug.LogError("Debug canvas is not assigned.", this);
                return false;
            }
            if (_motionController == null)
            {
                Debug.LogError("MotionController is not assigned.", this);
                return false;
            }
            SpawnAndBindDots(_motionController.KeypointBindings);
            return true;
        }
    }

    private void Update()
    {
        if (_httpHost != null && _deserializer != null)
        {
            var frame = _httpHost.RetrieveFrame();
            if (frame == null) return;
            _deserializer.DeserializeJSON(frame);

            var hit = _motionController.CurrentRaycastHit;
            if (hit.collider != null && CurrentHover == null)
            {
                Debug.Log($"Start hover {hit.transform.name}");
                OnHoverChanged(hit.transform.gameObject);
            }
            else if (hit.collider == null && CurrentHover != null)
            {
                Debug.Log($"End hover {CurrentHover}");
                OnHoverChanged(null);
            }
            else if (hit.collider != null && CurrentHover != hit.transform.gameObject)
            {
                Debug.Log($"Change hover {CurrentHover} to {hit.transform.name}");
                OnHoverChanged(hit.transform.gameObject);
            }
        }
    }

    protected virtual void OnHoverChanged(GameObject newHover)
    {
        _onHoverChanged?.Invoke(newHover);
        CurrentHover = newHover;
    }
    
    protected virtual void OnSelectionChanged(GameObject newSelection)
    {
        _onSelectionChanged?.Invoke(newSelection);
        CurrentSelected = newSelection;
    }

    private void SpawnAndBindDots(KeypointBinding[] keypointBindings)
    {
        int len = keypointBindings.Length;
        for (int i = 0; i < len; i++)
        {
            var keypoint = keypointBindings[i];
            var go = Instantiate(_debugDotTemplate, _debugCanvas.transform);
            go.name = keypoint.Keypoint.keypointName;
            keypoint.Transform = go.transform;
            keypointBindings[i] = keypoint;
            go.GetComponent<UnityEngine.UI.Image>().color = GetColorFromKeypointIndex(i);
            go.SetActive(true);
        }
    }

    private Color GetColorFromKeypointIndex(int keypointIndex)
    {
        if (keypointIndex == 0)
        {
            return new(0, 0, 0);
        }
        int digit = 1 + Mathf.FloorToInt((keypointIndex - 1) / 4);
        int index = 1 + (keypointIndex - 1) % 4;
        var saturation = Mathf.FloorToInt(255 * index / 4);
        var r = ((digit & 1) != 0) ? saturation : 0;
        var g = ((digit & 2) != 0) ? saturation : 0;
        var b = ((digit & 4) != 0) ? saturation : 0;
        return new(r, g, b, .5f);
    }

    public void WaitGesture(object gesture, Action action)
    {
        throw new NotImplementedException("WaitGesture is not implemented yet.");
    }

    public Vector2 GetCurrentPositionScreenspace() =>
        _motionController.CurrentPositionWorldspace;

    public Vector2 GetCurrentPositionScreenspace(Keypoint keypoint)
    {
        Debug.Assert(keypoint != null, "Keypoint is null", this);

        return _motionController.GetCurrentPositionScreenspace(keypoint);
    }

    public Vector3 GetCurrentPositionScreenspace(Camera camera)
    {
        Debug.Assert(camera != null, "Camera is null", this);

        return _motionController.CurrentPositionWorldspace;
    }

    public Vector3 GetCurrentPositionScreenspace(Camera camera, Keypoint keypoint)
    {
        Debug.Assert(camera != null, "Camera is null", this);
        Debug.Assert(keypoint != null, "Keypoint is null", this);

        return _motionController.CurrentPositionWorldspace;
    }

    //event GestureChanged(NewGesture)
    //event OnGesture(Gesture)
    //event OnSelectionChanged(NewSelection, OldSelection)
    //bool GetObjectUnder(Keypoint, out RaycastHit)
}
