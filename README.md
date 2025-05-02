# Motion Control System for Unity
Colab project with DIS Study Abroad computer science students, April 2025

## Brief
This Motion Control System is a basic framework, intended to demonstrate how to use cheap webcams and phone cameras to capture and identify geometric keypoints for hands, and applying them to GameObjects in Unity in real-time.

This is a two-part project utilizing ML.js to capture hand keypoint data from any web-enabled camera source and stream it to a backend, and a Unity based stream consumer, which applies the keypoint data to a visual representation and allows the user to manipulate scene objects in real time using hand gestures.

The web-based capture can be served from either a Unity based HTTP server and backend, or a .NET core stand-alone service.

We have tried to make the project extensible, as this is only a demonstration. It would be possible to animate non-humanoid static meshes with this approach or create any number of interactions based on this framework.

## Contributers
(Name, Organization, Role)
- Ethan Kabatchnik, DIS computer science student, Systems Engineering for Unity plugin.
- Sarah Than, DIS computer science student, UI and UX engineering for Unity plugin.
- Troels Windekilde, Guanomancer ApS, Tech lead and Web development.

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
