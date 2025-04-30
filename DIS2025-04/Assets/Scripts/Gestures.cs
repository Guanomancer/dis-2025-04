using System;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    [Header("Capture Settings")]
    [Tooltip("If you are running the HTTP capture host remotely, you can disable this.")]
    [SerializeField] private bool _runHttpHost;
    [Tooltip("The HTTP endpoint for the HTTP capture server. (Example: http://localhost:8080)")]
    [SerializeField] private string _httpHostEndpoint = "http://localhost:8080";
    [Tooltip("Example: /index.html and /handpose.js")]
    [SerializeField] private WebPageMapping[] _pageMappings;

    [Header("Motion Settings")]
    [SerializeField] private MotionController _motionController;

    private HttpHost _httpHost;
    private HandTrackingData _deserializer;

    private static Gestures _instance;

    private void Awake()
    {
        if (!InitializeSingleton()) return;
        InitializeHttpHost();
        InitializeDeserializer();

        

        bool InitializeSingleton()
        {
            if (_instance != null)
            {
                Debug.LogWarning($"{nameof(Gestures)} already exists. Destroying this instance.", this);
                DestroyImmediate(gameObject);
                return false;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            return true;
        }

        bool InitializeHttpHost()
        {
            if (!_runHttpHost) return false;
            if(string.IsNullOrEmpty(_httpHostEndpoint))
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
    }

    private void Update()
    {
        if (_httpHost != null && _deserializer != null)
        {
            var frame = _httpHost.RetrieveFrame();
            if (frame == null) return;
            _deserializer.DeserializeJSON(frame);
        }
    }

    public void WaitGesture(object gesture, Action action)
    {
        throw new NotImplementedException("WaitGesture is not implemented yet.");
    }

    public Vector2 GetCurrentPositionScreenspace() =>
        _motionController.CurrentPositionScreenspace;

    public Vector2 GetCurrentPositionScreenspace(Keypoint keypoint)
    {
        Debug.Assert(keypoint != null, "Keypoint is null", this);

        return _motionController.GetCurrentPositionScreenspace(keypoint);
    }

    public Vector3 GetCurrentPositionScreenspace(Camera camera)
    {
        Debug.Assert(camera != null, "Camera is null", this);
    
        return _motionController.CurrentPositionScreenspace;
    }

    public Vector3 GetCurrentPositionScreenspace(Camera camera, Keypoint keypoint)
    {
        Debug.Assert(camera != null, "Camera is null", this);
        Debug.Assert(keypoint != null, "Keypoint is null", this);

        return _motionController.CurrentPositionScreenspace;
    }

    //event GestureChanged(NewGesture)
    //event OnGesture(Gesture)
    //event OnSelectionChanged(NewSelection, OldSelection)
    //bool GetObjectUnder(Keypoint, out RaycastHit)
}
