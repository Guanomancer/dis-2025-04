# Motion Control System for Unity
Colab project with DIS Study Abroad computer science students, April 2025

# Table of Contents
- [Brief](#brief)
- [Contributors](#contributors)
- [Getting Started](#getting-started)
- [Requirements For Contributors](#requirements-for-contributors)
  - [SmartMerge - Windows](#windows)
  - [SmartMerge - Mac OSX](#mac-osx)
  - [SmartMerge - Other](#other)
- [Hand Tracking Module Overview](#hand-tracking-module-overview)
  - [1. Keypoint Class](#1-keypoint-class)
  - [2. HandTrackingData Class](#2-handtrackingdata-class)
  - [3. HandTrackingController Component](#3-handtrackingcontroller-component)

## Brief
This Motion Control System is a basic framework, intended to demonstrate how to use cheap webcams and phone cameras to capture and identify geometric keypoints for hands, and applying them to GameObjects in Unity in real-time.

This is a two-part project utilizing ML.js to capture hand keypoint data from any web-enabled camera source and stream it to a backend, and a Unity based stream consumer, which applies the keypoint data to a visual representation and allows the user to manipulate scene objects in real time using hand gestures.

The web-based capture can be served from either a Unity based HTTP server and backend, or a .NET core stand-alone service.

We have tried to make the project extensible, as this is only a demonstration. It would be possible to animate non-humanoid static meshes with this approach or create any number of interactions based on this framework.

## Contributers
(Name, Organization, Role)
- [Ethan Kabatchnik](https://github.com/e-kab), DIS computer science student, Systems Engineering for Unity plugin.
- [Sarah Than](https://github.com/sarahthan123), DIS computer science student, UI and UX engineering for Unity plugin.
- Troels Windekilde, Guanomancer ApS, Tech lead and ML.js integration.

## Getting started
The Unity project has a Demo scene. When running it, hit the Launch Browser button.
In the browser, grant camera access when prompted and press Restart.
Wave your hand in front of your camera and follow the onscreen instructions.

# Requirements For Contributers
The following are required for contributors to set up locally.

## SmartMerge
To minimize the chance of unresolvable merge conflicts, please add Unity SmartMerge Merge to your local .config file.

You can find more info here: [SmartMerge](https://docs.unity3d.com/Manual/SmartMerge.html) and in the following subsections:

### Windows
- Locate your git repo folder. If you're using Github For Desktop simply press Ctrl + Shift + F to open the folder in Windows Explorer.
- Create a file named .gitconfig in the Unity project folder.
- Open the file in Notepad, VS Code or any other code editor, and paste in the following:
  - Please check that the path at the "cmd = 'C:\Prog..." is correct.
  - Change the editor version (6000.0.45f1) to match your current version.

```
[merge]
  tool = unityyamlmerge
[mergetool "unityyamlmerge"]
  trustExitCode = false
  cmd = 'C:\Program Files\Unity\Hub\Editor\6000.0.45f1\Editor\Data\Tools\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

### Mac OSX
- Locate your git repo folder. If you're using Github For Desktop simply press Command + Apple + Donut + Fruitfly to (maybe) open the folder in that thing that does the thing on Mac OSX.
- Create a file named .gitconfigin the Unity project folder.
- Open the file in VS Code or any other code editor, and paste in the following:
  - Please check that the path at the "cmd = '/Applica..." is correct.
  - Change the editor version (6000.0.45f1) to match your current version.

```
[merge]
  tool = unityyamlmerge
[mergetool "unityyamlmerge"]
  trustExitCode = false
  cmd = '/Applications/Unity/Hub/Editor/6000.0.45f1/Unity.app/Contents/Tools/UnityYAMLMerge' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

### Other
- Step 1: You're on your own.
- Step 2: There's a SmartMerge link somewhere above.
- Step 3: Me not that kind of orc...

```
[merge]
  tool = unityyamlmerge
[mergetool "unityyamlmerge"]
  trustExitCode = false
  cmd = '<path to UnityYAMLMerge>' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

# Hand Tracking Module Overview

This module processes real-time hand tracking data from an API stream and makes it easily usable within Unity. It is composed of three main components:

## 1. Keypoint Class

A flexible `ScriptableObject` representing a single tracked point for hand tracking in Unity, supporting both 2D and 3D data.

## Overview

The `Keypoint` class is designed to represent individual hand keypoints (e.g., fingertips, joints) for real-time tracking applications. It includes metadata, 2D/3D positional data, hierarchical parenting, and several utility methods to ease working with keypoint data across Unity projects.

## Features

- **Metadata**: 
  - `keypointName` — the name of the keypoint for identification.
- **Tracked Positions**:
  - `screenPosition` — the 2D position of the keypont in the camera capture window, ideal for screen-space or UI interactions.
  - `rotation` — a 3D vector representing rotation of the keypoint
- **Parent Keypoint**:
  - Optional reference to another `Keypoint`, allowing hierarchical relationships (e.g., a fingertip as a child of a finger joint).

## Public Methods

| Method | Description |
| :----- | :---------- |
| `Vector2 ToVector2()` | Returns the `screenPosition` as a `Vector2`. |
| `static implicit operator Vector2(Keypoint k)` | Allows implicit conversion of a `Keypoint` into its `Vector2` screen position. |
| `static Keypoint FromVector2(string name, Vector2 vec)` | Creates a new `Keypoint` from a `Vector2` and a name. |
| `Keypoint Clone()` | Creates a deep copy of the `Keypoint`. |
| `void Deconstruct(out string name, out Vector2 screenPos, out Vector3 rotationVector)` | Allows deconstruction into individual fields for tuple unpacking. |
| `override string ToString()` | Returns a readable string representation. |
| `override bool Equals(object obj)` | Compares two `Keypoint` instances based on `keypointName` and `screenPosition`. |
| `override int GetHashCode()` | Generates a hash code based on `keypointName` and `screenPosition`. |

## Inspector Setup

When creating a `Keypoint` in Unity (via **Assets > Create > Hand Tracking > Keypoint**), you will see the following fields:

- **Keypoint Name**: Assign a unique name for identification.
- **Screen Position (Vector2)**: Set initial 2D screen coordinates.
- **Rotation (Vector3)**: Set initial 3D rotation/position.
- **Parent Keypoint**: (Optional) Link to a parent keypoint for hierarchy.

## Usage Example

```csharp
// Creating a keypoint from Vector2
Keypoint thumbTip = Keypoint.FromVector2("ThumbTip", new Vector2(0.5f, 0.9f));

// Implicitly converting Keypoint to Vector2
Vector2 screenPos = thumbTip;

// Cloning a keypoint
Keypoint thumbTipClone = thumbTip.Clone();

// Deconstructing a keypoint
(string name, Vector2 pos, Vector3 rot) = thumbTip;
```

## Notes

- `Keypoint` is implemented as a `ScriptableObject` for reusability across scenes and ease of asset creation.
- Designed to be extensible for future tracking systems or hierarchical animations.

---

## 2. HandTrackingData Class

A data manager class for handling real-time hand tracking frames in Unity, including robust JSON deserialization, storage, and debugging utilities.

## Overview

The `HandTrackingData` class bridges the gap between incoming raw JSON data and Unity's runtime data structures, providing clean access to hand tracking information, including device ID, frame index, handedness, confidence, and keypoints.

## Features

- **Device Information**:
  - `DeviceID` — the device identifier.
  - `FrameIndex` — the index of the frame being processed.
  - `Handedness` — whether the hand is left or right.
  - `Confidence` — the confidence level of tracking data.

- **Keypoint Management**:
  - `Keypoints` — a `Dictionary<string, Keypoint>` storing keypoints by their names for fast retrieval.

## Public Methods

| Method | Description |
| :----- | :---------- |
| `void DeserializeJSON(string json)` | Parses and deserializes a raw JSON string containing hand tracking frame data into structured Unity objects. |
| `override string ToString()` | Returns a human-readable summary of the hand tracking data for easy debugging and logging. |

## Detailed Method Breakdown

### DeserializeJSON(string json)

Parses a multi-layered JSON structure into usable Unity objects:

- **Validation**:
  - Checks for empty or null JSON input.
  - Handles invalid or corrupted JSON with error reporting.
- **Deserialization**:
  - Parses outer JSON (device ID, frame index, and frame data string).
  - Parses nested `FrameDataJson` (array of keypoints and 3D keypoints).
  - Extracts handedness and confidence.
  - Creates or updates `Keypoint` instances based on the parsed data.
- **Error Handling**:
  - Includes multiple fallback points to prevent crashes if data is missing or malformed.

### ToString()

Generates a multi-line string summarizing:

- Device ID
- Frame Index
- Handedness
- Confidence
- All available keypoints and their details

## Usage Example

```csharp
// Assume you have a JSON string from your tracking API
string rawJson = GetHandTrackingJson();

// Create a new HandTrackingData instance
HandTrackingData handData = new HandTrackingData();

// Populate it
handData.DeserializeJSON(rawJson);

// Access keypoints
if (handData.Keypoints.TryGetValue("ThumbTip", out Keypoint thumbTip))
{
    Debug.Log($"ThumbTip Position: {thumbTip.screenPosition}");
}

// Print all tracking data
Debug.Log(handData.ToString());
```

## Notes

- **Dependency**: Requires [Newtonsoft.Json](https://www.newtonsoft.com/json) (Json.NET) for JSON parsing.
- **Unity Integration**: Uses `ScriptableObject` to create `Keypoint` instances at runtime.
- **Design Philosophy**: Prioritizes robustness and stability — designed to tolerate incomplete or inconsistent JSON data.

---

## 3. HandTrackingController Component

A modular and extensible Unity MonoBehaviour for detecting hand gestures using screen-space keypoint tracking data.

## Overview

The `HandTrackingController` handles real-time gesture detection based on hand keypoints. It processes incoming `HandTrackingData`, detects gestures such as **Pinch** and **Thumbs Up**, and triggers Unity events when gestures are detected or end.

## Features

- **Event Triggers**:
  - `OnGestureDetected` — called when a gesture is detected and held for a configured duration.
  - `OnGestureEnded` — called when a detected gesture is lost for a configured duration.

- **Timing Control**:
  - `detectionHoldTime` — how long a gesture must be held to trigger detection.
  - `lostHoldTime` — how long a gesture must be lost before ending.

- **Threshold Customization**:
  - `pinchThreshold` — distance in screen space between thumb and index for pinch detection.
  - `thumbsUpThreshold` — angular deviation allowed for thumbs up detection.

- **Gesture Tracking**:
  - Detects gestures using simple, robust screen-space logic.
  - `GestureType` enum enables clean and scalable gesture categorization.

- **Easy Gesture Expansion**:
  - Adding new gestures only requires implementing a new `DetectXXX` method that returns a `GestureType`.

## Public Properties

| Property | Type | Description |
| :------- | :--- | :----------- |
| `GestureType CurrentGesture` | `GestureType` | Current active gesture, or `None`. |

## Public Methods

| Method | Description |
| :----- | :----------- |
| `void UpdateHandTrackingData(string json)` | Updates internal hand tracking data from a JSON string. |
| `GestureType DetectPinch(HandTrackingData data)` | Detects a pinch gesture based on keypoint proximity. |
| `GestureType DetectThumbsUp(HandTrackingData data)` | Detects a thumbs-up gesture based on thumb orientation and position. |
| `void LogGestureDetected()` | Debug log when a gesture is detected. |
| `void LogGestureEnded()` | Debug log when a gesture ends. |

## Gesture Detection Logic

Each frame:
- Attempts to detect a gesture with `DetectGesture()`.
- If a gesture is detected:
  - Starts a timer (`gestureTimer`).
  - When held long enough (`detectionHoldTime`), triggers `OnGestureDetected`.
- If no gesture is detected:
  - Starts a lost timer (`lostTimer`).
  - When lost long enough (`lostHoldTime`), triggers `OnGestureEnded`.

Gesture-specific detection (like pinch or thumbs up) is cleanly separated into their own methods (`DetectPinch`, `DetectThumbsUp`).

### How New Gestures Can Be Added

**Step 1** — Create a new detection function:

```csharp
public GestureType DetectWave(HandTrackingData handTrackingData)
{
    // Add wave detection logic here
    return GestureType.Wave;
}
```

**Step 2** — Add it into the main `DetectGesture()` method:

```csharp
gesture = DetectWave(handTrackingData);
if (gesture == GestureType.Wave)
    return GestureType.Wave;
```

**Step 3** — Extend the `GestureType` enum:

```csharp
public enum GestureType
{
    None,
    Pinch,
    ThumbsUp,
    Wave  // <- newly added
}
```

This design **keeps all gestures independent** and **makes gesture management scalable** without cluttering the `Update()` logic.

---

## Usage Example

```csharp
public class HandTrackingInputManager : MonoBehaviour
{
    public HandTrackingController handController;

    void Start()
    {
        handController.OnGestureDetected.AddListener(OnGestureDetected);
        handController.OnGestureEnded.AddListener(OnGestureEnded);
    }

    void Update()
    {
        string incomingJson = GetHandTrackingJsonFromDevice();
        handController.UpdateHandTrackingData(incomingJson);
    }

    void OnGestureDetected()
    {
        Debug.Log($"Gesture Detected: {handController.CurrentGesture}");
    }

    void OnGestureEnded()
    {
        Debug.Log("Gesture Ended");
    }
}
```

---

## Notes

- **Dependency**: Requires the `HandTrackingData` class and its populated keypoints.
- **Gesture Sensitivity**: Can be finely tuned via thresholds (`pinchThreshold`, `thumbsUpThreshold`) and timing variables (`detectionHoldTime`, `lostHoldTime`).
- **Best Practice**: Add only one gesture detection at a time to keep performance high and maintain clean gesture priority ordering.

---
