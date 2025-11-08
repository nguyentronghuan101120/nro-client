# System Patterns & Architecture

## Network Architecture

### Session Management Pattern

Each game tab has its own session manager:

- **Session_ME**: Main session handler (primary connection)
- **Session_ME2**: Secondary session handler (backup connection)

**Key Components:**

```
Session_ME
├── MessageCollector (Thread) - Reads incoming messages
├── Sender (Thread) - Sends outgoing messages
└── NetworkInit (Thread) - Handles connection setup
```

**Critical Files:**

- `Assets/Scripts/Game1/Session_ME.cs` - Primary session implementation
- `Assets/Scripts/Game1/Session_ME2.cs` - Secondary session implementation

### Message Flow Pattern

```
Server → NetworkStream → BinaryReader → MessageCollector → Controller → Screen
Screen → Service → Message → Sender → BinaryWriter → NetworkStream → Server
```

### Encryption Pattern

1. Client connects without encryption
2. Server sends encryption key (message command -27)
3. All subsequent messages are encrypted using XOR cipher
4. Key rotates through array for each byte

**Implementation:**

```csharp
readKey(byte) - Decrypts incoming bytes
writeKey(byte) - Encrypts outgoing bytes
```

## Screen Management Pattern

### Screen Hierarchy

```
mScreen (Base)
├── LoginScr - Login screen
├── ServerListScreen - Server selection
├── GameScr - Main game screen (in-game)
├── CreateCharScr - Character creation
└── ChooseCharScr - Character selection
```

**Screen Switching:**

```csharp
screen.switchToMe() → Updates GameCanvas.currentScreen
```

### Controller Pattern

**Single Controller** handles all server messages:

- `Controller.onMessage(Message msg)` - Main message dispatcher
- Uses switch-case on `msg.command` to route messages
- Updates game state and UI based on server commands

**Critical Message Commands:**

- `-27`: Encryption key exchange
- `2`: Character creation/selection
- `-101`: Login response
- `-107`: Pet system updates

## Thread Safety Pattern

### Thread Management

- **Main Thread**: Unity update loop, UI rendering
- **Network Threads**: Message reading/writing
- **Thread Communication**: Message queues (`MyVector recieveMsg`)

**Important:**

- Network threads add messages to queue
- Main thread processes queue in `Session_ME.update()`
- Never update UI directly from network threads

## Resource Management Pattern

### Asset Loading

- **Res.cs**: Central resource manager
- **SmallImage/BigImage**: Image caching system
- **TileMap**: Map tile management

### Memory Management

- Clear resources on screen transitions
- Use object pooling for frequently created objects
- Clean up on `doResetToLoginScr()`

## Error Handling Patterns

### Network Error Handling

1. **Connection Errors**: Retry with exponential backoff
2. **Read Errors**: Return null, let caller handle
3. **Disconnect**: Clean up resources, return to login

### Thread Cleanup Pattern

```csharp
1. Set connected = false (signal threads to exit)
2. Close streams (unblocks pending reads)
3. Wait for threads with Join(100ms)
4. Abort only if thread doesn't exit naturally
```

## Known Issues & Solutions

### Issue: Array Allocation Error

**Cause**: Reading from closed socket during thread abort
**Solution**:

- Check `dis != null && connected` before reads
- Validate array sizes before allocation
- Catch ObjectDisposedException silently

### Issue: Null Key Array

**Cause**: Encryption key not received yet
**Solution**: Return original byte if key is null

### Issue: Thread Abort During Read

**Cause**: Calling Thread.Abort() while blocked on socket read
**Solution**: Close streams first, then wait for natural exit
