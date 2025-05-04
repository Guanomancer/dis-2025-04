using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HandTrackingData
{
    public string DeviceID { get; set; }
    public int FrameIndex { get; set; }
    public string Handedness { get; set; }
    public float Confidence { get; set; }

    public Dictionary<string, Keypoint> Keypoints = new Dictionary<string, Keypoint>();
    public void DeserializeJSON(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("Input JSON is empty or null!");
            return;
        }

        // Parse outer data
        JObject outer;

        try
        {
            outer = JObject.Parse(json);
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError($"Invalid input JSON: {ex.Message}\n{json}");
            return;
        }


        DeviceID = outer["DeviceId"].ToObject<string>();
        if (string.IsNullOrEmpty(DeviceID)) return;
        FrameIndex = outer["FrameIndex"].ToObject<int>();

        var frameDataString = outer["FrameDataJson"].ToObject<string>();
        var test = outer["FrameDataJson"].ToString();

        if (test.Length != frameDataString.Length)
        {
            Debug.LogError("string mismatch");
        }

        if (string.IsNullOrEmpty(frameDataString))
        {
            Debug.LogError("FrameDataJson is null or empty!");
            return;
        }

        JArray frameDataArray;
        try
        {
            frameDataArray = JArray.Parse(frameDataString);
        }
        catch (JsonReaderException ex)
        {
            return;
            // Debug.LogError($"Invalid FrameDataJson: {ex.Message}\n{frameDataString}");
        }

        if (frameDataArray.Count == 0)
        {
            Debug.LogError("FrameDataJson array is empty!");
            return;
        }


        // Parse inner data
        JObject innerData = (JObject)frameDataArray[0];
        JArray keypoints = (JArray)innerData["Keypoints"];
        JArray keypoints3D = (JArray)innerData["Keypoints3D"];

        // Parse keypoint data

        innerData.Remove("keypoints");
        innerData.Remove("keypoints3D");

        // Parse non-JArray data
        foreach (var property in innerData.Properties())
        {
            if (property.Value.Type != JTokenType.Object)
            {
                if (property.Name == "handedness")
                {
                    Handedness = property.Value.ToString();
                }
                else
                {
                    Confidence = (float)property.Value;
                }
                continue;
            }
            string keypointName = property.Name.ToString();


            if (!Keypoints.ContainsKey(keypointName))
            {
                Keypoints.Add(keypointName, ScriptableObject.CreateInstance<Keypoint>());
            }

            Keypoint current = Keypoints[keypointName];

            current.screenPosition.x = property.Value["x"].ToObject<float>();
            current.screenPosition.y = property.Value["y"].ToObject<float>();
            current.rotation.x = property.Value["x3D"].ToObject<float>();
            current.rotation.y = property.Value["y3D"].ToObject<float>();
            current.rotation.z = property.Value["z3D"].ToObject<float>();
            current.keypointName = property.Name.ToString();

        }

        return;
    }
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"DeviceID: {DeviceID}");
        sb.AppendLine($"FrameIndex: {FrameIndex}");
        sb.AppendLine($"Handedness: {Handedness}");
        sb.AppendLine($"Confidence: {Confidence}");

        sb.AppendLine("Keypoints:");
        if (Keypoints != null)
        {
            foreach (var kp in Keypoints)
                sb.AppendLine($"  {kp.Key}: {kp.Value}");
        }

        return sb.ToString();
    }

}
