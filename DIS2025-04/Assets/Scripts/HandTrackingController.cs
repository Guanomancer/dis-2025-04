using UnityEngine;
using UnityEngine.Events;

public class HandTrackingController : MonoBehaviour
{
    public UnityEvent OnGestureDetected;     // Gesture Detected
    public UnityEvent OnGestureEnded;         // Gesture Ended

    private HandTrackingData handTrackingData = new HandTrackingData();

    public float pinchThreshold = 10f;         // Distance threshold for pinch detection
    public float detectionHoldTime = 0.2f;     // 200ms to trigger
    public float lostHoldTime = 0.2f;           // 200ms to de-trigger

    private bool gestureActive = false;         // Whether gesture is currently active
    private float gestureTimer = 0f;             // Timer to track detection time
    private float lostTimer = 0f;                // Timer to track time gesture is *not* detected

    private void Update()
    {
        // string newJson = GetNewHandTrackingJson();
        // handTrackingData.DeserializeJSON(newJson);

        bool gestureDetected = DetectGesture();

        if (gestureDetected)
        {
            gestureTimer += Time.deltaTime;
            lostTimer = 0f;

            if (!gestureActive && gestureTimer >= detectionHoldTime)
            {
                gestureActive = true;
                OnGestureDetected?.Invoke();
            }
        }
        else
        {
            lostTimer += Time.deltaTime;
            gestureTimer = 0f;

            if (gestureActive && lostTimer >= lostHoldTime)
            {
                gestureActive = false;
                OnGestureEnded?.Invoke();
            }
        }
    }

    private bool DetectGesture()
    {
        if (handTrackingData.Keypoints.Count == 0)
            return false;

        if (handTrackingData.Keypoints.TryGetValue("thumb_tip", out var thumb) &&
            handTrackingData.Keypoints.TryGetValue("index_finger_tip", out var index))
        {
            float distance = Vector2.Distance(thumb.screenPosition, index.screenPosition);
            return distance < pinchThreshold;
        }

        return false;
    }

    public void UpdateHandTrackingData(string json)
    {
        handTrackingData.DeserializeJSON(json);
    }
}
