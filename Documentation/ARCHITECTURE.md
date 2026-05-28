# Burritocraft 2.0 - Architecture

## Overview

Burritocraft 2.0 is a voxel-based sandbox game built with Unity and C#, featuring infinite procedural worlds, multiplayer networking, and WebGL export support.

## Core Systems

### 1. Voxel System

**Components:**
- `BlockType.cs` - Defines block properties and IDs
- `Chunk.cs` - Manages 16x16x16 voxel chunks
- `WorldGenerator.cs` - Procedural terrain generation and chunk management

**How it works:**
- World is divided into chunks (16x16x16 voxels)
- Each voxel is represented by a single byte (BlockId)
- Chunks generate meshes on demand with face culling
- Only exposed faces are rendered (greedy meshing optimization planned)

### 2. Player System

**Components:**
- `PlayerController.cs` - Player movement, camera, and block interaction

**Features:**
- FPS camera with mouse look
- WASD movement + Shift sprint
- Space to jump (with ground detection)
- Left click to break blocks
- Right click to place blocks
- Network-synced through Netcode for GameObjects

### 3. Networking

**Technology:**
- Unity Netcode for GameObjects
- Unity Transport (UTP)

**Implementation:**
- Client-server architecture
- Server handles block modifications
- ClientRpc/ServerRpc for syncing
- Supports both multiplayer and single-player modes

### 4. Game Manager

**Responsibilities:**
- Singleton pattern for global game state
- Configuration (world seed, multiplayer mode, chunk size)
- Initialization of game systems

## Data Flow

```
Player Input
    ↓
PlayerController (Input handling)
    ↓
WorldGenerator (Chunk management)
    ↓
Chunk (Voxel data storage)
    ↓
Mesh Generation
    ↓
Rendering
```

## Multiplayer Flow

```
Local Block Modification
    ↓
PlayerController detects input
    ↓
ServerRpc called (sends to server)
    ↓
Server processes and validates
    ↓
ClientRpc broadcast to all clients
    ↓
All clients update local world state
```

## Performance Optimizations

1. **Chunk-based Loading** - Only chunks near player are loaded
2. **Face Culling** - Only exposed faces are rendered
3. **Mesh Caching** - Meshes regenerated only when dirty
4. **LOD System** (Planned) - Distance-based detail reduction

## File Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   └── GameManager.cs
│   ├── Voxel/
│   │   ├── BlockType.cs
│   │   ├── Chunk.cs
│   │   └── WorldGenerator.cs
│   ├── Player/
│   │   └── PlayerController.cs
│   ├── Network/
│   │   └── NetworkManager.cs
│   └── UI/
│       └── MainUI.cs
├── Resources/
├── Scenes/
└── Prefabs/
```

## Next Steps

1. Create main scene with proper hierarchy
2. Implement texture system
3. Add more block types
4. Implement saving/loading system
5. Optimize mesh generation (greedy meshing)
6. Add inventory system
7. Sound and visual effects
8. Mobile controls for WebGL
9. Performance profiling
