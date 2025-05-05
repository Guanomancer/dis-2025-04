# Motion Control System for Unity
Colab project with DIS Study Abroad computer science students, April 2025

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

The HandTrackingData class manages the deserialization and organization of raw JSON hand tracking data.

- **Deserialization**: The `DeserializeJSON(string json)` method robustly parses the nested JSON data, extracting the device ID, frame index, handedness, confidence level, and individual keypoints.
- **Data Storage**: Keypoints are stored in a `Dictionary<string, Keypoint>`, allowing quick lookups by name.
- **Error Handling**: The deserialization process includes extensive validation to ensure stability even with incomplete or corrupted input.
- **Debugging**: A detailed `ToString()` override outputs a readable summary of the frame data, useful for logging or debugging.

This class acts as the main bridge between the raw API stream and Unity's internal data structures.

## 3. HandTrackingController Component

The HandTrackingController is a `MonoBehaviour` that listens for hand gestures in real-time.

**Gesture Detection:**

- **Pinch**: Recognized based on distance between thumb and index finger tips.
- **Thumbs Up**: Recognized based on thumb orientation and relative finger positions.

**Threshold Parameters**:  
Sensitivity and timing for gesture detection are customizable via inspector-exposed parameters like `pinchThreshold`, `thumbsUpThreshold`, `detectionHoldTime`, and `lostHoldTime`.

**Event System:**

- `OnGestureDetected`: Invoked when a gesture is successfully detected and held.
- `OnGestureEnded`: Invoked when a gesture is no longer detected after a grace period.

**Public Methods:**

- `UpdateHandTrackingData(string json)`: Manually update the controller with new JSON data if needed.
- `LogGestureDetected()` and `LogGestureEnded()`: Simple debug utilities for logging events.

This component provides an event-driven architecture for integrating hand gestures into gameplay mechanics, UI interactions, or any other dynamic behavior in Unity.

