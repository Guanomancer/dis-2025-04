# DIS-2025-04
Colab project with DIS Study Abroad computer science students, April 2025

Contributers:
(Name, Organization, Role)
Ethan Kabatchnik, DIS computer science student, Systems Engineering for Unity plugin.
Sarah Than, DIS computer science student, UI and UX engineering for Unity plugin.
Troels Windekilde, Guanomancer ApS, Lead and Web app development.

## Brief
This is a 3-part project, consisting of a web front- and backend for capturing hand gesture data, and a Unity project that consumes capture data and provides Unity compatible geometry data for interaction with GameObjects and Components.

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
