# Burritocraft 2.0 - Setup Guide

## Requirements

- Unity 2022 LTS or later
- .NET Framework 4.7.1 or later
- Git

## Installation

### 1. Clone Repository

```bash
git clone https://github.com/talhak7892025-a11y/burritocraft-2.0.git
cd burritocraft-2.0
```

### 2. Open in Unity

```bash
# Open with Unity Hub or directly
/path/to/Unity/2022.x/Editor/Unity -projectPath .
```

### 3. Install Dependencies

In Unity:
1. Go to `Window > TextMesh Pro > Import TMP Essentials`
2. Go to `Window > Netcode for GameObjects > Create Default PlayNetworkManager`

### 4. Create Main Scene

1. Create new scene: `Assets/Scenes/MainScene.unity`
2. Create a cube for ground (temporary)
3. Add GameManager prefab
4. Add WorldGenerator component
5. Add player prefab
6. Add main UI canvas

## Running the Game

### Single Player

1. Set `isMultiplayer = false` in GameManager
2. Press Play

### Multiplayer (Local)

**Server:**
1. Set `isMultiplayer = true` in GameManager
2. Set `isServer = true` in NetworkManager
3. Press Play

**Client:**
1. Set `isMultiplayer = true` in GameManager
2. Set `isServer = false` in NetworkManager
3. Set `serverAddress = "127.0.0.1"`
4. Press Play

## Building

### WebGL Build

1. File > Build Settings
2. Select WebGL platform
3. Click Build and Run
4. Enjoy in your browser!

### Standalone Build

1. File > Build Settings
2. Select PC/Mac/Linux Standalone
3. Click Build and Run

## Troubleshooting

**Issue: "Netcode not found"
**Solution:** 
- Go to Window > Netcode for GameObjects > Import latest version

**Issue: "No camera in scene"
**Solution:**
- Ensure player prefab has a Camera component as child

**Issue: "Chunks not rendering"
**Solution:**
- Check that material is assigned to WorldGenerator
- Ensure chunk layer is not being culled

## Performance Tips

1. Adjust `renderDistance` in WorldGenerator based on your GPU
2. Lower renderDistance for lower-end devices
3. Use WebGL for web deployment
4. Profile using Unity Profiler (Window > Analysis > Profiler)
