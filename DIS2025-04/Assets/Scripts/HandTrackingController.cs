using UnityEngine;
using UnityEngine.Events;

public class HandTrackingController : MonoBehaviour
{
    public UnityEvent OnGestureDetected; // Event to fire when a gesture is detected

    private HandTrackingData handTrackingData = new HandTrackingData();

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

        // Example: simple "fist" detection based on distance between thumb and index finger
        if (handTrackingData.Keypoints.TryGetValue("ThumbTip", out var thumb) &&
            handTrackingData.Keypoints.TryGetValue("IndexTip", out var index))
        {
            float distance = Vector2.Distance(thumb.screenPosition, index.screenPosition);
            return distance < 0.05f; // Tune this threshold based on your app
        }

        return false;
    }

    // Optionally, you can add a public method to manually feed JSON:
    public void UpdateHandTrackingData(string json)
    {
        handTrackingData.DeserializeJSON(json);
    }
}
