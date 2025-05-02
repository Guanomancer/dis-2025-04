using UnityEngine;

public class MotionController : MonoBehaviour
{
    [field: SerializeField] public Vector2 CameraResolution { get; private set; } = new Vector2(800, 600);

    [Header("Position")]
    [SerializeField] private bool _applyPosition;
    [SerializeField] private bool _flipX;
    [SerializeField] private bool _flipY = true;
    [Tooltip("Default Z value for CurrentPositionWS, when the keypoint is not over a gameobject.")]
    [SerializeField] private float _defaultWorldspaceDepth = 15f;

    [Header("Rotation (Not Implemented, yet)")]
    [SerializeField] private bool _applyRotation;
    [SerializeField] private float _rotationScale = 2f;

    [Header("Keypoints")]
    [SerializeField] private int _pointerKeypointBindingIndex;
    [Tooltip("You can assign these manually or leave blank to have them auto generated for debug dots usage.")]
    [field: SerializeField] public KeypointBinding[] KeypointBindings { get; private set; }
    [field: SerializeField] public Keypoint[] GenerateKeypointBindingsFrom { get; private set; }

    public Vector2 CurrentPositionSS { get; private set; }
    public Vector2 CurrentPositionWS { get; private set; }
    public Vector3 CurrentRotaton { get; private set; }
    public RaycastHit CurrentRaycastHit { get; private set; }

    private void Awake()
    {
        GenerateKeypointBindings();



        bool GenerateKeypointBindings()
        {
            if (GenerateKeypointBindingsFrom == null) return false;

            KeypointBindings = new KeypointBinding[GenerateKeypointBindingsFrom.Length];
            for (int i = 0; i < GenerateKeypointBindingsFrom.Length; ++i)
            {
                KeypointBindings[i].Keypoint = GenerateKeypointBindingsFrom[i];
            }

            return true;
        }
    }

    private void Update()
    {
        CurrentRaycastHit = default;
        for (int i = 0; i < KeypointBindings.Length; i++)
        {
            var binding = KeypointBindings[i];
            if (binding.Keypoint == null) continue;
            if (binding.Transform == null) continue;

            var rotation = binding.Keypoint.rotation * 360 * _rotationScale;
            if (_applyRotation && binding.Keypoint.keypointName == "wrist")
            {
                // yaw, pitch, roll
                binding.Transform.localEulerAngles = rotation;
            }

            var cameraPos = GetCameraPosition(binding);
            GetScreenPosition(cameraPos, out var screenPosN, out var screenPos);

            var ray = Camera.main.ScreenPointToRay(screenPos);
            var positionWS = Physics.Raycast(ray, out RaycastHit hit) ?
                hit.point :
                ray.GetPoint(_defaultWorldspaceDepth);

            if (i == _pointerKeypointBindingIndex)
            {
                if (_applyPosition && binding.Transform.TryGetComponent<CanvasRenderer>(out _))
                {
                    binding.Transform.position = screenPos;
                }

                CurrentPositionWS = positionWS;
                CurrentPositionSS = screenPos;
                CurrentRotaton = rotation;

                CurrentRaycastHit = hit;
            }
            else if (_applyPosition)
            {
                binding.Transform.position = binding.Transform.TryGetComponent<CanvasRenderer>(out _) ?
                    screenPos : positionWS;
            }
        }
    }

    public Vector2 GetCameraPosition(KeypointBinding binding) => new Vector2(
            _flipX ? CameraResolution.x - binding.Keypoint.screenPosition.x : binding.Keypoint.screenPosition.x,
            _flipY ? CameraResolution.y - binding.Keypoint.screenPosition.y : binding.Keypoint.screenPosition.y
        );

    public void GetScreenPosition(Vector2 cameraPosition, out Vector3 screenPosN, out Vector3 screenPos)
    {
        screenPosN = new Vector2(
            cameraPosition.x / CameraResolution.x,
            cameraPosition.y / CameraResolution.y
        );
        screenPos = screenPosN *
            new Vector2(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);
    }

    public Vector2 GetCurrentPositionScreenspace(Keypoint keypoint)
    {
        if (keypoint == null) return Vector2.zero;
        foreach (var binding in KeypointBindings)
        {
            if (binding.Keypoint == keypoint)
            {
                return binding.Keypoint.screenPosition;
            }
        }
        return Vector2.zero;
    }
}

public static class Extension_Methods
{
    public static Vector3 Swizzle(this Vector3 vector, SwizzleOrder order)
    {
        switch (order)
        {
            case SwizzleOrder.XZY:
                return new Vector3(vector.x, vector.z, vector.y);
            case SwizzleOrder.YXZ:
                return new Vector3(vector.y, vector.x, vector.z);
            case SwizzleOrder.YZX:
                return new Vector3(vector.y, vector.z, vector.x);
            case SwizzleOrder.ZXY:
                return new Vector3(vector.z, vector.x, vector.y);
            case SwizzleOrder.ZYX:
                return new Vector3(vector.z, vector.y, vector.x);
            default:
                return vector;
        }
    }
}

[System.Serializable]
public struct KeypointBinding
{
    public Keypoint Keypoint;
    public Transform Transform;
}

public enum SwizzleOrder
{
    YXZ,
    YZX,
    ZXY,
    ZYX,
    XZY,
    XYZ
}

public enum KeypointNames
{
    Wrist,
    Thumb_CMC,
    Thumb_MCP,
    Thumb_IP,
    Thumb_Tip,
    Index_Finger_MCP,
    Index_Finger_PIP,
    Index_Finger_DIP,
    Index_Finger_Tip,
    Middle_Finger_MCP,
    Middle_Finger_PIP,
    Middle_Finger_DIP,
    Middle_Finger_Tip,
    Ring_Finger_MCP,
    Ring_Finger_PIP,
    Ring_Finger_DIP,
    Ring_Finger_Tip,
    Pinky_Finger_MCP,
    Pinky_Finger_PIP,
    Pinky_Finger_DIP,
    Pinky_Finger_Tip
}