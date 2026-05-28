# Burritocraft 2.0

An infinite voxel-based sandbox game built with Unity and C#, with multiplayer support and WebGL export capabilities.

## Features

- **Infinite Voxel World**: Procedurally generated infinite terrain
- **Multiplayer**: Real-time multiplayer gameplay with networking
- **Single Player**: Offline sandbox mode
- **All Block Types**: Extensible block system
- **WebGL Export**: Play in browser
- **Chunk-Based Rendering**: Optimized performance for infinite worlds

## Project Structure

```
burritocraft-2.0/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Voxel/
│   │   ├── Player/
│   │   ├── Network/
│   │   └── UI/
│   ├── Resources/
│   ├── Scenes/
│   └── Prefabs/
├── ProjectSettings/
├── Packages/
└── Documentation/
```

## Setup

1. Clone this repository
2. Open with Unity 2022 LTS or later
3. Install required packages (see ProjectSettings)
4. Open MainScene in Assets/Scenes/
5. Press Play

## Building

### WebGL Export
```
File > Build Settings > Select WebGL Platform > Build
```

### Standalone
```
File > Build Settings > Select PC/Mac/Linux > Build
```

## Multiplayer

Uses Netcode for GameObjects and Transport for networking.

## License

MIT License - See LICENSE file
