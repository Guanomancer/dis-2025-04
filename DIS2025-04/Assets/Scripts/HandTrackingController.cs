using UnityEngine;
using UnityEngine.Events;

public class HandTrackingController : MonoBehaviour
{
    public UnityEvent OnGestureDetected; // Event to fire when a gesture is detected

    private HandTrackingData handTrackingData = new HandTrackingData();
    public float pinchThreshold = 10f; // Initial Guess regarding Pinch

    private void Update()
    {
        // Here you would normally get updated JSON from your source
        // string newJson = GetNewHandTrackingJson();
        // handTrackingData.DeserializeJSON(newJson);

        if (DetectGesture())
        {
            OnGestureDetected?.Invoke();
        }
    }

    private bool DetectGesture()
    {
        // Example: Gesture detection logic
        // Let's say you detect a "fist" if keypoints for fingers are close together

        if (handTrackingData.Keypoints.Count == 0)
            return false;

        // Pinch detection
        if (handTrackingData.Keypoints.TryGetValue("thumb_tip", out var thumb) &&
            handTrackingData.Keypoints.TryGetValue("index_finger_tip", out var index))
        {
            float distance = Vector2.Distance(thumb.screenPosition, index.screenPosition);
            return distance < pinchThreshold; // Tune this threshold based on your app
        }

        return false;
    }

    // Optionally, you can add a public method to manually feed JSON:
    public void UpdateHandTrackingData(string json)
    {
        handTrackingData.DeserializeJSON(json);
    }
}
