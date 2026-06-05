[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=U7ZWQ2WHFEWH4)

# Common Utils for Unity
A collection of essential tools for any Unity project.

## Install ##

**Installation must be performed by project.**

1. Open the Package Manager in Unity (menu Window / Package Manager).
2. Press the "+" button in the top left corner of the Package Manager panel and select "Add package from git URL..."
3. When prompted, enter the URL https://github.com/edcasillas/unity-common-utils.git

Alternatively, you can manually add the following line to your Packages/manifest.json file under dependencies:

    "com.ecasillas.commonutils": "https://github.com/edcasillas/unity-common-utils.git"

Open Unity again; the Package Manager will run and the package will be installed.

## Update ##

1. Open the Package Manager in Unity (menu Window / Package Manager).
2. Look for the "Common Utils for Unity" package in the list of installed packages and select it.
3. Press the "Update" button.

Alternatively, you can manually remove the version lock the Package Manager creates in Packages/manifest.json so when it runs again it gets the newest version. The lock looks like this:

```
    "com.ecasillas.commonutils": {
      "hash": "someValue",
      "revision": "HEAD"
    }
```
## Features ##
Please refer to the [wiki](https://github.com/edcasillas/unity-common-utils/wiki) for a full description on the features and their documentation.

### Runtime

| Module | Description |
|--------|-------------|
| **Extensions** | Extension methods for Array, Camera, Color, DateTime, Dictionary, Enum, Enumerable, Float, GameObject, MonoBehaviour, Quaternion, RectTransform, Renderer, String, Transform, Vector2, Vector3, UnityWebRequest |
| **Object Pooling** | `PrefabPoolManager` for efficient GameObject reuse |
| **Coroutines** | `Coroutiner` helper and coroutine extensions |
| **REST SDK** | `RestClient` with async support for HTTP requests |
| **Scene Loading** | `SceneLoader` component with configurable loading screen |
| **Data Structures** | `PriorityQueue`, `DynamicPriorityQueue`, `CircularSequence`, `UniqueItemsQueue`, `RandomList` |
| **Input** | `SwipeManager`, `KeyboardStringReader`, `AndroidButtonsListener`, `SelectionFromKeyboard` |
| **UI** | Animated score displays, progress display, typewriter label, blinker elements, submenus, color picker, slider colors |
| **Serialization** | Serializable dictionaries, 2D arrays, randomizables |
| **Inspector** | `[HelpBox]`, `[UnityLayer]`, `[UnityTag]`, `[ShowInInspector]`, `SceneRefs` |
| **Local Persistence** | `PlayerPrefsDb` — lightweight local entity storage |
| **Logging** | `ILogger` / `UnityLogger` abstraction with log levels |
| **Verbosables** | Per-component verbosity control with global settings |
| **WebGL** | `WebGLBridge` for JS interop, pointer lock, browser detection |
| **Dynamic Enums** | Runtime-editable enum definitions via ScriptableObjects |
| **Debuggable Editors** | Reflect private fields/methods in play mode inspectors |
| **Draggables** | 2D/3D drag-and-drop components |
| **Misc** | `MathUtils`, `PhysicsUtils`, `NetworkAddress`, `CountdownTimer`, `IndexRandomizer`, `SingletonRegistry`, `AsyncUtils` |

### Editor

| Tool | Description |
|------|-------------|
| **Android** | ADB utilities, APK installer window, manifest parser |
| **Asset Cleaner** | Find and remove unused assets |
| **Built-in Icons** | Explorer window for Unity's built-in editor icons |
| **Launchctl Environment** | View/edit macOS launchctl environment variables |
| **Publitch** | One-click itch.io publishing via Butler |
| **Scene Auto Loader** | Auto-load a master scene when entering Play mode |
| **Screenshot Manager** | Capture screenshots at multiple resolutions |
| **Script GUID Swap** | Replace script references by GUID |
| **Minimap Generator** | Generate top-down minimap textures |
| **WebGL Server** | Local HTTP server for WebGL testing |
| **Linters** | Namespace validation |
| **System Processes** | `CommandLineRunner` for external process execution |

### Tests

- **Editor tests:** Index randomizer, random list, editor icons, extensions, heaps, web resources, local persistence, publitch, launchctl environment
- **Play mode tests:** Coroutines, extensions, UnityEventsNotifier
