# Product Context

## What This Project Is

A Unity-based mobile MMORPG game client inspired by Dragon Ball. Players can:

- Create and customize characters
- Explore game worlds
- Battle monsters and other players
- Join clans and participate in events
- Manage inventory and equipment
- Chat with other players

## Why It Exists

- **Multi-tab Gaming**: Allows players to run 6 game instances simultaneously (multi-accounting)
- **Cross-platform**: Play on PC, Android, or iOS
- **Real-time Multiplayer**: Connect to game server for live gameplay

## Target Users

- **Primary**: Mobile gamers who enjoy MMORPG games
- **Secondary**: Players who want to multi-account (farm resources, trade, etc.)
- **Platform**: Primarily mobile (Android/iOS), with PC support

## Key Features

### Authentication

- Username/password login
- Server selection
- Character creation (up to 3 characters per account)
- Character selection

### Gameplay

- **Movement**: Touch/click to move character
- **Combat**: Skill-based combat system
- **Quests**: NPC interactions and quest system
- **Social**: Chat, friends, clans
- **Economy**: Trading, shops, inventory management

### Multi-tab System

- Switch between 6 game tabs
- Each tab is independent game instance
- Separate connections per tab
- Tab switching via UI buttons

## User Experience Goals

### Login Flow

```
Splash Screen → Server List → Login → Character Selection → Game World
```

### Expected Behavior

1. **Fast Loading**: Minimal wait times
2. **Stable Connection**: No unexpected disconnects
3. **Smooth Gameplay**: 60 FPS target
4. **Responsive Controls**: Immediate feedback on actions

### Error Handling

- **Connection Failed**: Show error, allow retry
- **Disconnected**: Auto-return to server selection
- **Invalid Login**: Show error message
- **Server Full**: Show waiting message

## Critical User Flows

### First Time User

1. Launch app → Splash screen
2. Download game data (if needed)
3. Select server
4. Enter credentials (or register)
5. Create character
6. Enter game world
7. Complete tutorial (if implemented)

### Returning User

1. Launch app → Auto-connect to last server
2. Login (credentials saved)
3. Select character
4. Enter game world at last location

### Multi-tab User

1. Login to first tab
2. Switch to second tab
3. Login with different account
4. Repeat for up to 6 tabs
5. Switch between tabs as needed

## Known User Pain Points

### Network Issues

- **Problem**: Game disconnects during play
- **Impact**: Loss of progress, frustration
- **Solution**: Improved error handling, graceful disconnects

### Loading Times

- **Problem**: Long initial load
- **Impact**: User drops off before playing
- **Solution**: Progressive loading, caching

### UI Confusion

- **Problem**: Complex UI with many buttons
- **Impact**: New users feel overwhelmed
- **Solution**: Tutorial system, simplified initial UI

## Success Metrics

- **Connection Success Rate**: >95%
- **Average Session Length**: >30 minutes
- **Crash Rate**: <1%
- **User Retention**: >50% after 7 days

## Future Considerations

- Auto-reconnect on disconnect
- Better error messages
- Tutorial system
- Performance optimizations
- More intuitive UI
