# Technical Context

## Development Environment

- **Unity Version**: 2021.x or later
- **Language**: C# (.NET Framework)
- **Platform Targets**: PC (Windows/Mac), Android, iOS
- **IDE**: Unity Editor, Visual Studio Code with Cursor

## Key Dependencies

- **Unity Modules**:
  - UnityEngine.UI
  - UnityEngine.Networking (legacy)
  - System.Net.Sockets
- **Custom Libraries**:
  - No external NuGet packages
  - All networking code is custom-built

## Network Protocol

- **Transport**: TCP (System.Net.Sockets.TcpClient)
- **Serialization**: Custom binary format (BinaryReader/BinaryWriter)
- **Encryption**: XOR cipher with rotating key
- **Default Port**: 14445
- **Default Host**: localhost or 127.0.0.1

## Message Format

```
[Command: 1 byte][Length: 2 bytes][Data: N bytes]
```

- Command: sbyte (-128 to 127)
- Length: ushort (0 to 65535)
- Data: byte array

## Build Configuration

- **PC**: Standalone build, windowed mode
- **Mobile**: Portrait orientation, touch controls
- **Resolution**: Adaptive (720x320 to 1024x600 on PC)

## Performance Considerations

- **Target FPS**: 60 FPS
- **Network Update**: Every 5ms (Thread.Sleep(5))
- **Message Queue**: Processed in main thread update
- **Max Message Size**: 10MB (validation added)

## File Structure

```
Assets/
├── Scripts/
│   ├── Game1/          # Primary game code
│   │   ├── Session_ME.cs       # Network session
│   │   ├── Controller.cs       # Message handler
│   │   ├── GameCanvas.cs       # Main canvas
│   │   ├── GameScr.cs          # Game screen
│   │   ├── LoginScr.cs         # Login screen
│   │   └── ...
│   ├── Game2-6/        # Cloned game instances
│   └── Shared/         # Shared utilities
├── Resources/          # Game assets (images, sounds)
└── Scenes/            # Unity scenes
```

## Critical Code Files

### Network Layer

1. **Session_ME.cs** (~770 lines)

   - TCP connection management
   - Message encryption/decryption
   - Thread management
   - **Key Methods**: `connect()`, `readMessage()`, `doSendMessage()`

2. **Controller.cs** (~6600 lines)
   - Message routing and handling
   - Game state management
   - **Key Method**: `onMessage(Message msg)` - 3000+ line switch statement

### UI Layer

3. **GameCanvas.cs** (~3200 lines)

   - Main canvas and screen management
   - Input handling
   - **Key Methods**: `doResetToLoginScr()`, `connect()`

4. **GameScr.cs** (~7800 lines)

   - In-game screen
   - Character rendering
   - **Key Methods**: `switchToMe()`, `loadGameScr()`

5. **LoginScr.cs** (~1000 lines)
   - Login UI
   - Authentication
   - **Key Methods**: `doLogin()`, `switchToMe()`

### Utilities

6. **mSystem.cs** - System utilities
7. **Res.cs** - Resource management
8. **Message.cs** - Message wrapper class

## Testing Approach

- **Manual Testing**: Primary method
- **Server**: Requires external game server running on localhost:14445
- **Debug**: Unity Console logs
- **Network**: Monitor with Wireshark if needed

## Common Development Tasks

### Adding New Message Handler

1. Add case in `Controller.onMessage()`
2. Read message data with `msg.reader()`
3. Update game state
4. Update UI if needed

### Adding New Screen

1. Extend `mScreen` base class
2. Override `switchToMe()` and `update()`
3. Register in screen management system
4. Handle input in `updateKey()`

### Debugging Network Issues

1. Check connection state: `Session_ME.connected`
2. Verify encryption key: `Session_ME.key != null`
3. Monitor message flow: Check `Session_ME.recieveMsg`
4. Test with server logs

## Known Limitations

- Single-threaded UI updates only
- No automatic reconnection (manual retry)
- Limited error messages to user
- Hard-coded server addresses
