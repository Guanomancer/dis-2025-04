using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class HandTrackingController : MonoBehaviour
{
    [Tooltip("Gesture Detected")]
    public UnityEvent OnGestureDetected;     
    [Tooltip("Gesture Ended")]
    public UnityEvent OnGestureEnded;        

    private HandTrackingData handTrackingData = new HandTrackingData();

    [Tooltip("Distance threshold for pinch detection")]
    public float pinchThreshold = 10f;

    [Tooltip("Angle threshold for thumbs up detection")]
    public float thumbsUpThreshold = 20f;
    [Tooltip("200ms to trigger")]
    public float detectionHoldTime = 0.2f;
    [Tooltip("200ms to de-trigger")]
    public float lostHoldTime = 0.2f;           

    private bool gestureActive = false;         // Whether gesture is currently active
    private float gestureTimer = 0f;             // Timer to track detection time
    private float lostTimer = 0f;                // Timer to track time gesture is *not* detected

    private GestureType currentGesture = GestureType.None; // Track the gesture
    public GestureType CurrentGesture => currentGesture;


    private void Update()
    {
        // string newJson = GetNewHandTrackingJson();
        // handTrackingData.DeserializeJSON(newJson);

        GestureType detectedGesture = DetectGesture();

        if (detectedGesture != GestureType.None)
        {
            if (currentGesture == GestureType.None || currentGesture != detectedGesture)
            {
                // New gesture started
                currentGesture = detectedGesture;
                gestureTimer = 0f;
                lostTimer = 0f;
            }

            gestureTimer += Time.deltaTime;
            lostTimer = 0f;

            if (!gestureActive && gestureTimer >= detectionHoldTime)
            {
                gestureActive = true;
                OnGestureDetected?.Invoke();
                Debug.Log($"{currentGesture} detected.");
            }
        }
        else
        {
            lostTimer += Time.deltaTime;
            gestureTimer = 0f;

            if (gestureActive && lostTimer >= lostHoldTime)
            {
                gestureActive = false;
                Debug.Log($"{currentGesture} ended.");
                currentGesture = GestureType.None;
                OnGestureEnded?.Invoke();
            }
        }
    }

    private GestureType DetectGesture()
    {

        var handTrackingData = Gestures.HandTracking;
        GestureType gesture = GestureType.None;

        if (handTrackingData.Keypoints.Count == 0)
            return GestureType.None;

        gesture = DetectPinch(handTrackingData);

        if (gesture == GestureType.Pinch)
        {
            return GestureType.Pinch;
        }

        gesture = DetectThumbsUp(handTrackingData);

        return gesture;

        

    }

    public GestureType DetectPinch(HandTrackingData handTrackingData)
    {
        // Detect Pinch
        if (handTrackingData.Keypoints.TryGetValue("thumb_tip", out var thumbTip) &&
            handTrackingData.Keypoints.TryGetValue("index_finger_tip", out var indexTip) &&
            handTrackingData.Keypoints.TryGetValue("index_finger_dip", out var indexDip) &&
            handTrackingData.Keypoints.TryGetValue("thumb_ip", out var thumbDip))
        {
            float tipDistance = Vector2.Distance(thumbTip.screenPosition, indexTip.screenPosition);

            float dipDistance = Vector2.Distance(thumbDip.screenPosition, indexDip.screenPosition);

            if ((tipDistance < pinchThreshold) && (dipDistance > pinchThreshold))
            {
                return GestureType.Pinch;
            }
        }
        return GestureType.None;
    }

    public GestureType DetectThumbsUp(HandTrackingData handTrackingData)
    {
        // Detect ThumbsUp
        if (handTrackingData.Keypoints.TryGetValue("thumb_tip", out var thumbTip) &&
            handTrackingData.Keypoints.TryGetValue("thumb_ip", out var thumbDip) &&
            handTrackingData.Keypoints.TryGetValue("index_finger_tip", out var indexTip) &&
            handTrackingData.Keypoints.TryGetValue("middle_finger_tip", out var middleTip) &&
            handTrackingData.Keypoints.TryGetValue("ring_finger_tip", out var ringTip) &&
            handTrackingData.Keypoints.TryGetValue("pinky_finger_tip", out var pinkyTip))
        {
            // Thumb should be vertical (almost straight up)
            Vector2 thumbDirection = (thumbTip.screenPosition - thumbDip.screenPosition).normalized;

            // Up vector in screen space is (0, 1)
            float angleToUp = Vector2.Angle(thumbDirection, Vector2.up);

            bool thumbIsVertical = Mathf.Abs(angleToUp - 180f) <= thumbsUpThreshold;


            // Thumb is higher than all other fingers
            bool thumbAboveFingers =
            thumbTip.screenPosition.y < indexTip.screenPosition.y &&
            thumbTip.screenPosition.y < middleTip.screenPosition.y &&
            thumbTip.screenPosition.y < ringTip.screenPosition.y &&
            thumbTip.screenPosition.y < pinkyTip.screenPosition.y;

            // Fingers are curled (close together)
            // float fingerSpread = Vector2.Distance(indexTip.screenPosition, pinkyTip.screenPosition);


            if (thumbIsVertical && thumbAboveFingers)
            {
                return GestureType.ThumbsUp;
            }
        }
        return GestureType.None;
    }

    public void UpdateHandTrackingData(string json)
    {
        handTrackingData.DeserializeJSON(json);
    }

    public void LogGestureDetected()
    {
        Debug.Log("Gesture detected!");
    }

    public void LogGestureEnded()
    {
        Debug.Log("Gesture ended.");
    }

    public enum GestureType
    {
        None,
        Pinch,
        ThumbsUp
    }
}
