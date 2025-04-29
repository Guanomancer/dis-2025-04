using UnityEngine;
using UnityEngine.UIElements;

public class MotionController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _cameraResolution = new Vector2(800, 600);

    [Header("Position")]
    [SerializeField] private bool _applyPosition;
    [SerializeField] private bool _flipX;
    [SerializeField] private bool _flipY = true;

    [Header("Rotation")]
    [SerializeField] private bool _applyRotation;
    [SerializeField] private SwizzleOrder _swizzle = SwizzleOrder.XYZ;
    [SerializeField] private float _rotationScale = 1000.0f;

    [Header("Keypoints")]
    [SerializeField] private KeypointBinding[] _keypointBindings;

    Vector3 maxR = Vector3.zero;
    private void Update()
    {
        foreach (var binding in _keypointBindings)
        {
            if (binding.Keypoint == null) return;
            if (binding.Transform == null) return;

            //if (ApplyPosition)
            if (binding.Keypoint.keypointName == "wrist" && _applyPosition)
            {
                var screenPos = new Vector2(
                    _flipX ? _cameraResolution.x - binding.Keypoint.screenPosition.x : binding.Keypoint.screenPosition.x,
                    _flipY ? _cameraResolution.y - binding.Keypoint.screenPosition.y : binding.Keypoint.screenPosition.y
                    );
                var screenPosN = new Vector2(
                    screenPos.x / _cameraResolution.x,
                    screenPos.y / _cameraResolution.y
                    );
                var cameraPos = screenPosN *
                    new Vector2(Camera.main.scaledPixelWidth, Camera.main.scaledPixelHeight);

                var positionWS = Vector3.zero;
                var ray = Camera.main.ScreenPointToRay(cameraPos);
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
                Debug.DrawRay(ray.origin, ray.direction, Color.blue);

                binding.Transform.position = positionWS;
            }

            if (_applyRotation)
            {
                // yaw, pitch, roll
                var rotation = binding.Keypoint.worldPosition * 360 * _rotationScale;

                if (binding.Keypoint.keypointName == "index_finger_tip")
                {
                    Debug.Log($"Rotation (Estimated Euler): {rotation}, Model prediction: <b>{binding.Keypoint.worldPosition}</b>");
                }

                if (binding.Keypoint.keypointName == "wrist")
                {
                    binding.Transform.localEulerAngles = rotation;
                }
            }
        }
        //Debug.Log(maxR);
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