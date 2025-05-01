using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKeypoint", menuName = "Hand Tracking/Keypoint")]
public class Keypoint : ScriptableObject
{
    [Header("Metadata")]
    public string keypointName;

    [Header("Tracked Positions")]
    public Vector2 screenPosition;    // e.g., 2D screen/UI space
    public Vector3 rotation;     // e.g., 3D Unity world or local space

    [Header("Parent Keypoint")]
    public Keypoint parent;

    public Vector2 ToVector2() => screenPosition;

    public static implicit operator Vector2(Keypoint k) => k.screenPosition;

    public static Keypoint FromVector2(string name, Vector2 vec)
    {
        Keypoint kp = CreateInstance<Keypoint>();
        kp.keypointName = name;
        kp.screenPosition = vec;
        return kp;
    }

    public override string ToString()
    {
        return $"(Name: {keypointName}, Screen: {screenPosition}, Rotation: {rotation})";
    }

    public override bool Equals(object obj)
    {
        if (obj is not Keypoint other) return false;
        return keypointName == other.keypointName && screenPosition == other.screenPosition;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(keypointName, screenPosition);
    }

    public Keypoint Clone()
    {
        Keypoint kp = CreateInstance<Keypoint>();
        kp.keypointName = this.keypointName;
        kp.screenPosition = this.screenPosition;
        kp.rotation = this.rotation;
        return kp;
    }

    public void Deconstruct(out string name, out Vector2 screenPos, out Vector3 rotationVector)
    {
        name = keypointName;
        screenPos = screenPosition;
        rotationVector = rotation;
    }
}
