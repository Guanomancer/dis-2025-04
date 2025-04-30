using System.Collections.Generic;
using UnityEngine;

public class MotionController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _cameraResolution = new Vector2(800, 600);
    public Vector2 CameraResolution => _cameraResolution;

    [Header("Position")]
    [SerializeField] private bool _applyPosition;
    [SerializeField] private bool _flipX;
    [SerializeField] private bool _flipY = true;

    [Header("Rotation")]
    [SerializeField] private bool _applyRotation;
    //[SerializeField] private SwizzleOrder _swizzle = SwizzleOrder.XYZ;
    [SerializeField] private float _rotationScale = 2f;

    [Header("Keypoints")]
    [SerializeField] private KeypointBinding[] _keypointBindings;
    public IReadOnlyList<KeypointBinding> KeypointBindings => _keypointBindings;

    private Vector2 _currentPositionSS;
    public Vector2 CurrentPositionScreenspace => _currentPositionSS;

    private Vector3 _currentRotaton;
    public Vector3 CurrentRotation => _currentRotaton;

    private void Update()
    {
        foreach (var binding in _keypointBindings)
        {
            if (binding.Keypoint == null) return;
            if (binding.Transform == null) return;

            if (binding.Keypoint.keypointName == "wrist")
            {
                var cameraPos = GetCameraPosition(binding);
                GetScreenPosition(cameraPos, out var screenPosN, out var screenPos);

                var positionWS = Vector3.zero;
                var ray = Camera.main.ScreenPointToRay(screenPos);
                if (Physics.Raycast(
                    ray,
                    out RaycastHit hit))
                {
                    Debug.Log($"Hit: {hit.collider.name}");
                    positionWS = hit.point;
                }
                else
                {
                    positionWS = ray.GetPoint(20f);
                }

                _currentPositionSS = positionWS;
                if (_applyPosition)
                {
                    binding.Transform.position = positionWS;
                }
            }

            // yaw, pitch, roll
            var rotation = binding.Keypoint.worldPosition * 360 * _rotationScale;

            if (binding.Keypoint.keypointName == "wrist")
            {
                _currentPositionSS = rotation;
                if (_applyRotation)
                {
                    binding.Transform.localEulerAngles = rotation;
                }
            }
        }
        //Debug.Log(maxR);
    }

    public Vector2 GetCameraPosition(KeypointBinding binding) => new Vector2(
            _flipX ? _cameraResolution.x - binding.Keypoint.screenPosition.x : binding.Keypoint.screenPosition.x,
            _flipY ? _cameraResolution.y - binding.Keypoint.screenPosition.y : binding.Keypoint.screenPosition.y
        );

    private void GetScreenPosition(Vector2 cameraPosition, out Vector3 screenPosN, out Vector3 screenPos)
    {
        screenPosN = new Vector2(
            cameraPosition.x / _cameraResolution.x,
            cameraPosition.y / _cameraResolution.y
        );
        screenPos = screenPosN *
            new Vector2(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);
    }

    public Vector2 GetCurrentPositionScreenspace(Keypoint keypoint)
    {
        if (keypoint == null) return Vector2.zero;
        foreach (var binding in _keypointBindings)
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