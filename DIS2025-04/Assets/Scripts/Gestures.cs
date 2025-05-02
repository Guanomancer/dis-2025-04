using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

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
    private HandTrackingData _handTracker;

    private Button _currentUiHover;
    private GameObject _currentHover;
    private GameObject _currentSelected;
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults;

    #region Static fields
    private static Gestures _instance;
    public static HandTrackingData HandTracking => _instance != null ? _instance._handTracker : null;
    public static GameObject CurrentHover => _instance != null ? _instance._currentHover : null;
    public static GameObject CurrentSelected => _instance != null ? _instance._currentSelected : null;
    #endregion

    private void Awake()
    {
        if (!InitializeSingleton()) return;
        InitializeHttpHost();
        InitializeDeserializer();
        InitializeDebugDots();
        _pointerEventData = new(EventSystem.current);
        _raycastResults = new();

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

            _handTracker = new();
            foreach (var keypoint in _motionController.KeypointBindings)
            {
                _handTracker.Keypoints.Add(keypoint.Keypoint.keypointName, keypoint.Keypoint);
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
        _pointerEventData.position = GetCurrentPositionScreenspace();

        if (!UpdateHandTracker()) return;
        UpdatePointerStateWorld();
        UpdatePointerStateUI();

        bool UpdateHandTracker()
        {
            if (_httpHost == null || _handTracker == null) return false;

            var frame = _httpHost.RetrieveFrame();
            if (string.IsNullOrEmpty(frame)) return false;

            _handTracker.DeserializeJSON(frame);
            return true;
        }

        bool UpdatePointerStateWorld()
        {

            var hit = _motionController.CurrentRaycastHit;
            if (hit.collider != null && _currentHover == null)
            {
                Debug.Log($"Start hover {hit.transform.name}");
                OnHoverChanged(hit.transform.gameObject);
            }
            else if (hit.collider == null && _currentHover != null)
            {
                Debug.Log($"End hover {_currentHover}");
                OnHoverChanged(null);
            }
            else if (hit.collider != null && _currentHover != hit.transform.gameObject)
            {
                Debug.Log($"Change hover {_currentHover} to {hit.transform.name}");
                OnHoverChanged(hit.transform.gameObject);
            }
            return true;
        }

        void UpdatePointerStateUI()
        {
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
            var currentHit = _pointerEventData.pointerCurrentRaycast = FindFirstRaycast(_raycastResults);
            _raycastResults.Clear();
            if (currentHit.isValid)
            {
                var button = currentHit.gameObject.GetComponentInParent<Button>();
                if (button != null && button != _currentUiHover)
                {
                    if (button != null) button.OnPointerExit(_pointerEventData);
                    button.OnPointerEnter(_pointerEventData);
                    _currentUiHover = button;
                }
            }
            else if (_currentUiHover != null)
            {
                _currentUiHover.OnPointerExit(_pointerEventData);
                _currentUiHover = null;
            }
        }

        RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
        {
            var candidatesCount = candidates.Count;
            for (var i = 0; i < candidatesCount; ++i)
            {
                if (candidates[i].gameObject == null)
                    continue;

                return candidates[i];
            }
            return default;
        }
    }

    protected virtual void OnHoverChanged(GameObject newHover)
    {
        _onHoverChanged?.Invoke(newHover);
        _currentHover = newHover;
    }

    protected virtual void OnSelectionChanged(GameObject newSelection)
    {
        _onSelectionChanged?.Invoke(newSelection);
        _currentSelected = newSelection;
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

    public static Color GetColorFromKeypointIndex(int keypointIndex)
    {
        const int KEYPOINTS_PER_FINGER = 4;
        const int FINGERS_PER_HAND = 5;

        if (keypointIndex == 0) return new(0, 0, 0);

        int digit = 1 + Mathf.FloorToInt((keypointIndex - 1) / (FINGERS_PER_HAND - 1));
        int index = 1 + (keypointIndex - 1) % (FINGERS_PER_HAND - 1);
        var saturation = Mathf.FloorToInt(255 * index / KEYPOINTS_PER_FINGER);
        var r = ((digit & 1) != 0) ? saturation : 0;
        var g = ((digit & 2) != 0) ? saturation : 0;
        var b = ((digit & 4) != 0) ? saturation : 0;
        return new(r, g, b, .5f);
    }

    public static void WaitGesture(object gesture, Action action)
    {
        throw new NotImplementedException("WaitGesture is not implemented yet.");
    }

    public static bool GetObjectUnder(Keypoint keypoint, out RaycastHit hit)
    {
        throw new NotImplementedException();
    }

    public static Vector2 GetCurrentPositionScreenspace() =>
        _instance != null ? _instance._motionController.CurrentPositionSS : default;

    public static Vector2 GetCurrentPositionScreenspace(Keypoint keypoint)
    {
        if (_instance == null) return default;

        Debug.Assert(keypoint != null, "Keypoint is null", _instance);
        return _instance._motionController.GetCurrentPositionScreenspace(keypoint);
    }

    public static Vector3 GetCurrentPositionScreenspace(Camera camera)
    {
        if (_instance == null) return default;

        Debug.Assert(camera != null, "Camera is null", _instance);

        return _instance._motionController.CurrentPositionWS;
    }

    public static Vector3 GetCurrentPositionScreenspace(Camera camera, Keypoint keypoint)
    {
        if (_instance == null) return default;

        Debug.Assert(camera != null, "Camera is null", _instance);
        Debug.Assert(keypoint != null, "Keypoint is null", _instance);

        return _instance._motionController.CurrentPositionWS;
    }

    //event GestureChanged(NewGesture)
    //event OnGesture(Gesture)
    //event OnSelectionChanged(NewSelection, OldSelection)
}
