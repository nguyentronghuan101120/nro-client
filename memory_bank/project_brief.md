# Project Brief: 6TabFixed - Unity Multi-Tab Game Client

## Overview
A Unity-based mobile game client that supports 6 simultaneous game tabs (Game1-Game6). This is a Dragon Ball-themed MMORPG client that connects to a game server via TCP sockets.

## Core Requirements
- **Multi-tab system**: Support 6 independent game instances running simultaneously
- **Network communication**: TCP socket-based client-server communication with encryption
- **Cross-platform**: Support for PC, Android, and iOS
- **Real-time gameplay**: Handle real-time multiplayer game mechanics

## Key Technologies
- **Unity Engine**: Game engine and UI framework
- **C# .NET**: Primary programming language
- **TCP Sockets**: Network communication protocol
- **Custom encryption**: Key-based message encryption/decryption

## Project Structure
```
Assets/Scripts/
├── Game1/          # Main game instance (primary tab)
├── Game2-Game6/    # Additional game tabs (clones of Game1)
├── Game5/          # Special game instance
└── Shared/         # Shared utilities and resources
```

## Critical Constraints
- Must maintain backward compatibility with existing server protocol
- Network messages must be encrypted after key exchange
- Each game tab must operate independently
- Session management must handle disconnections gracefully

