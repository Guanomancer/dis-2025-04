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

    [Header("Gesture Settings")]
    [SerializeField] private RadialMenuController _radialMenuController;
    [SerializeField] private HandTrackingController _handTrackingController;
    [SerializeField] private bool _sendClickEventsToButtons;

    private HttpHost _httpHost;
    private HandTrackingData _handTracker;

    private Button _currentUiHover;
    private GameObject _currentHover;
    private GameObject _currentSelected;
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults;
    private Ray _selectedObjectOffsetRay;
    private float _selectedObjectOffsetZ;
    private Vector3 _selectedObjectOffset;
    private Vector3 _selectedObjectRotation;
    private Vector3 _selectedObjectScale;
    private Vector2 _selectedObjectOffsetSS;
    private bool _selectedObjectWasKinematic;

    private GameObject[] _toolIcons;
    private GestureTool _gestureTool;
    public GestureTool GestureTool
    {
        get => _gestureTool;
        set
        {
            if (_gestureTool >= 0 && (int)_gestureTool < _toolIcons.Length)
                _toolIcons[(int)_gestureTool].SetActive(false);
            _gestureTool = value;
            if (_gestureTool >= 0 && (int)_gestureTool < _toolIcons.Length)
                _toolIcons[(int)_gestureTool].SetActive(true);
        }
    }

    public void SetGestureTool(int index) => GestureTool = (GestureTool)index;

    #region Static fields
    private static Gestures _instance;
    public static HandTrackingData HandTracking => _instance != null ? _instance._handTracker : null;
    public static GameObject CurrentHover => _instance != null ? _instance._currentHover : null;
    public static GameObject CurrentSelected => _instance != null ? _instance._currentSelected : null;

    public static GestureTool CurrentTool
    {
        get => _instance != null ? _instance.GestureTool : default;
        set { if (_instance != null) _instance.GestureTool = value; }
    }
    #endregion

    private void Awake()
    {
        if (!InitializeSingleton()) return;
        InitializeHttpHost();
        InitializeDeserializer();
        InitializeDebugDots();

        _pointerEventData = new(EventSystem.current);
        _raycastResults = new();

        _handTrackingController.OnGestureDetected.AddListener(GestureDetected);
        _handTrackingController.OnGestureEnded.AddListener(GestureEnded);

        #region Local Methods
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
        #endregion
    }

    private void OnDestroy()
    {
        if (_httpHost != null)
        {
            _httpHost.Dispose();
            _httpHost = null;
        }
        if (_instance != this) return;

        _instance = null;
    }

    private void Update()
    {
        _pointerEventData.position = GetCurrentPositionScreenspace();

        if (!UpdateHandTracker()) return;
        UpdatePointerStateWorld();
        UpdatePointerStateUI();

        #region Local Methods
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
            if (_currentSelected != null)
            {
                var deltaPosSS = _selectedObjectOffsetSS - _motionController.CurrentPositionSS;
                var ray = Camera.main.ScreenPointToRay(_motionController.CurrentPositionSS);
                var offsetPos = _selectedObjectOffsetRay.GetPoint(_selectedObjectOffsetZ);
                var currentPos = ray.GetPoint(_selectedObjectOffsetZ);
                var deltaPosWS = currentPos - offsetPos;
                switch (CurrentTool)
                {
                    case GestureTool.Move:
                        _currentSelected.transform.position = _selectedObjectOffset + deltaPosWS;
                        break;
                    case GestureTool.Rotate:
                        _currentSelected.transform.eulerAngles = _selectedObjectRotation + new Vector3(deltaPosSS.y, deltaPosSS.x, 0f) * .1f;
                        break;
                    case GestureTool.Scale:
                        _currentSelected.transform.localScale = _selectedObjectScale + new Vector3(deltaPosSS.x, deltaPosSS.y, 0f) * .01f;
                        break;
                    case GestureTool.Yeet:
                        if (!_currentSelected.TryGetComponent<Rigidbody>(out var rigidbody))
                        {
                            rigidbody = _currentSelected.AddComponent<Rigidbody>();
                            rigidbody.linearVelocity = UnityEngine.Random.insideUnitSphere * 100f;
                        }
                        break;
                    default:
                        break;
                }
                return true;
            }

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
                    if (_currentUiHover != null) _currentUiHover.OnPointerExit(_pointerEventData);
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
        #endregion
    }

    protected virtual void OnHoverChanged(GameObject newHover)
    {
        _onHoverChanged?.Invoke(newHover);
        _currentHover = newHover;
    }

    protected virtual void OnSelectionChanged(GameObject newSelection)
    {
        if (_currentSelected != null && _currentSelected.TryGetComponent<Rigidbody>(out var rb))
            rb.isKinematic = false;

        _onSelectionChanged?.Invoke(newSelection);
        _currentSelected = newSelection;
        if (_currentSelected != null)
        {
            _selectedObjectOffsetSS = _motionController.CurrentPositionSS;
            _selectedObjectOffsetRay = Camera.main.ScreenPointToRay(_motionController.CurrentPositionSS);
            _selectedObjectOffsetZ = _currentSelected.transform.position.z - Camera.main.transform.position.z;
            _selectedObjectOffset = _currentHover.transform.position;
            _selectedObjectRotation = _currentHover.transform.eulerAngles;
            _selectedObjectScale = _currentHover.transform.localScale;

            if (_currentSelected.TryGetComponent<Rigidbody>(out rb))
            {
                _selectedObjectWasKinematic = rb.isKinematic;
                rb.isKinematic = true;
            }
        }

        if (_gestureTool == GestureTool.None)
        {
            _radialMenuController.OpenMenu();
        }
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
            go.GetComponent<Image>().color = GetColorFromKeypointIndex(i);
            if (i == _motionController.PointerKeypointBindingIndex)
            {
                var iconsRenderers = go.GetComponentsInChildren<Image>(true);
                _toolIcons = new GameObject[iconsRenderers.Length - 1];
                for (int j = 0; j < iconsRenderers.Length - 1; j++)
                    _toolIcons[j] = iconsRenderers[j + 1].gameObject;
                _toolIcons[(int)_gestureTool].SetActive(true);
            }
            go.SetActive(true);
        }
    }

    private void GestureDetected()
    {
        if (_handTrackingController.CurrentGesture == HandTrackingController.GestureType.ThumbsUp)
        {
            if (PerformMenuAccess()) return;
        }
        else if (_handTrackingController.CurrentGesture == HandTrackingController.GestureType.Pinch)
        {
            if (PerformUiInteraction()) return;
            if (PerformWorldInteraction()) return;
        }

        #region Local Methods
        bool PerformUiInteraction()
        {
            if (_currentUiHover == null) return false;

            _currentUiHover.OnPointerClick(_pointerEventData);
            return true;
        }

        bool PerformWorldInteraction()
        {
            if (_currentHover == null) return false;

            OnSelectionChanged(_currentHover);
            return true;
        }

        bool PerformMenuAccess()
        {
            if (_radialMenuController == null) return false;

            if (_radialMenuController.gameObject.activeInHierarchy) return false;

            _radialMenuController.OpenMenu();

            return true;
        }
        #endregion
    }

    private void GestureEnded()
    {
        if (PerformWorldInteraction()) return;

        #region Local Methods
        bool PerformWorldInteraction()
        {
            if (_currentSelected == null) return false;

            OnSelectionChanged(null);
            return true;
        }
        #endregion
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
}

public enum GestureTool
{
    None = 0,

    Move,
    Rotate,
    Scale,

    Yeet,
}