Create PHASE TWO of the WPF AAA Game Launcher project.

IMPORTANT:
The UI, navigation, animations, pages, styles, and launcher shell are ALREADY COMPLETE.



Focus on
1. Installed game detection
2. Game launching
3. Process monitoring
4. Installed launcher scanning
5. Steam library parsing
6. Registry scanning
7. Executable discovery
8. Runtime game state management

update the UI on the way

The architecture must integrate cleanly into the existing MVVM launcher project.

Use:
- WPF (.NET Framework)
- C#
- MVVM
- ObservableCollection
- ICommand
- async/await
- Clean architecture
- Dependency injection style service separation

==================================================
GOAL
==================================================

Transform the launcher prototype into a REAL launcher backend capable of:

- Detecting installed games
- Detecting launcher clients
- Detecting executable paths
- Launching installed games
- Monitoring running games
- Updating launcher UI state automatically

The launcher should behave similarly to:
- Riot Client
- Steam
- Battle.net
- Epic Games Launcher

==================================================
CREATE THESE NEW FILES ONLY
==================================================

Models/
├── GameInfo.cs
├── GamePlatform.cs
├── GameState.cs

Services/
├── GameDetectionService.cs
├── SteamDetectionService.cs
├── RiotDetectionService.cs
├── EpicDetectionService.cs
├── BattleNetDetectionService.cs
├── LauncherService.cs
├── GameMonitoringService.cs

ViewModels/
├── InstalledGamesViewModel.cs
├── GameCardViewModel.cs

Helpers/
├── RegistryHelper.cs
├── ProcessHelper.cs
├── FileSystemHelper.cs

==================================================
PART 1 — GAME MODEL
==================================================

Create GameInfo.cs.

Properties:
- Name
- InstallPath
- ExecutablePath
- IconPath
- BannerPath
- Platform
- Version
- IsInstalled
- IsRunning
- LastPlayed
- SizeOnDisk
- CurrentState
- ProcessId
- LaunchArguments

Implement:
- INotifyPropertyChanged

==================================================
PART 2 — GAME STATE ENUM
==================================================

Create:

public enum GameState
{
    NotInstalled,
    Installed,
    Updating,
    Launching,
    Running,
    Error
}

==================================================
PART 3 — PLATFORM ENUM
==================================================

Create:

public enum GamePlatform
{
    Steam,
    Riot,
    EpicGames,
    BattleNet,
    Standalone,
    Unknown
}

==================================================
PART 4 — GAME DETECTION SERVICE
==================================================

Create GameDetectionService.

Responsibilities:
- Central game scanning manager
- Aggregate results from all platform services
- Return ObservableCollection<GameInfo>

Methods:
- Task<ObservableCollection<GameInfo>> ScanForInstalledGamesAsync()
- Task RefreshGameStatesAsync()
- Task ScanCustomDirectoriesAsync()

Requirements:
- Fully asynchronous
- Non-blocking UI
- Thread-safe updates
- Handle invalid directories safely
- Extensive exception handling

Scan:
- Program Files
- Program Files (x86)
- Custom directories
- Windows registry
- Known launcher locations

==================================================
PART 5 — STEAM DETECTION
==================================================

Create SteamDetectionService.

Requirements:
- Detect Steam installation path
- Parse:
  steamapps/libraryfolders.vdf
- Parse:
  appmanifest_*.acf

Extract:
- Game names
- App IDs
- Install directories
- Steam libraries

Methods:
- string GetSteamInstallPath()
- List<string> GetSteamLibraries()
- Task<List<GameInfo>> DetectSteamGamesAsync()

Support:
- Multiple Steam libraries
- Multiple drives

Known Steam locations:
- C:\Program Files (x86)\Steam
- Registry entries

==================================================
PART 6 — RIOT DETECTION
==================================================

Create RiotDetectionService.

Detect:
- VALORANT
- League of Legends
- Riot Client

Scan:
- Riot registry entries
- Riot Games folders

Known executables:
- VALORANT.exe
- LeagueClient.exe
- RiotClientServices.exe

Methods:
- Task<List<GameInfo>> DetectRiotGamesAsync()

==================================================
PART 7 — EPIC GAMES DETECTION
==================================================

Create EpicDetectionService.

Detect:
- Fortnite
- Unreal Engine installs
- Epic launcher games

Parse:
- Epic manifest files

Methods:
- Task<List<GameInfo>> DetectEpicGamesAsync()

==================================================
PART 8 — BATTLE.NET DETECTION
==================================================

Create BattleNetDetectionService.

Detect:
- Overwatch 2
- Diablo IV
- Call of Duty
- Battle.net launcher

Methods:
- Task<List<GameInfo>> DetectBattleNetGamesAsync()

==================================================
PART 9 — GAME LAUNCHING SYSTEM
==================================================

Create LauncherService.

Requirements:
- Launch executable files
- Launch using Process.Start
- Support launch arguments
- Detect invalid paths
- Detect already running games
- Update game state automatically

Methods:
- Task LaunchGameAsync(GameInfo game)
- Task StopGameAsync(GameInfo game)
- bool IsGameRunning(GameInfo game)

Use:
- Process.Start
- ProcessStartInfo

Example:
ProcessStartInfo startInfo = new ProcessStartInfo
{
    FileName = game.ExecutablePath,
    WorkingDirectory = game.InstallPath,
    UseShellExecute = true
};

Requirements:
- Proper exception handling
- Logging support
- State transitions
- Launch validation

==================================================
PART 10 — GAME MONITORING SYSTEM
==================================================

Create GameMonitoringService.

Responsibilities:
- Monitor running games
- Detect process exits
- Detect crashes
- Update states automatically

Methods:
- StartMonitoring()
- StopMonitoring()

Events:
- GameStarted
- GameStopped
- GameCrashed

Use:
- Process.GetProcesses()
- Process.GetProcessesByName()

Requirements:
- Poll every few seconds
- Low CPU usage
- Async background monitoring
- Thread-safe collection updates

==================================================
PART 11 — INSTALLED GAMES VIEWMODEL
==================================================

Create InstalledGamesViewModel.

Responsibilities:
- Store installed games collection
- Bind to UI
- Launch games
- Refresh scan
- Update game states

Properties:
- ObservableCollection<GameInfo> InstalledGames
- bool IsScanning
- string ScanStatus

Commands:
- ScanGamesCommand
- LaunchGameCommand
- StopGameCommand
- RefreshCommand

Requirements:
- Full MVVM binding support
- Async commands
- UI-safe collection updates

==================================================
PART 12 — PROCESS HELPERS
==================================================

Create:
- RegistryHelper
- FileSystemHelper
- ProcessHelper

Helpers should:
- Simplify scanning logic
- Simplify registry access
- Simplify process lookup
- Reduce duplicate code

==================================================
PART 13 — GAME STATES
==================================================

Implement automatic state handling.

State flow:
- NotInstalled
- Installed
- Launching
- Running
- Updating
- Error

Examples:
- If executable missing → NotInstalled
- If process active → Running
- During launch → Launching

==================================================
PART 14 — PERFORMANCE REQUIREMENTS
==================================================

Requirements:
- No UI freezing
- Use async/await everywhere possible
- Use Task.Run for heavy scanning
- Avoid blocking dispatcher thread
- Support hundreds of installed games

==================================================
PART 15 — CODE QUALITY
==================================================

Requirements:
- Add comments explaining logic
- Use clean architecture
- Avoid code duplication
- Proper separation of concerns
- Professional enterprise-level structure

==================================================
PART 16 — IMPORTANT
==================================================

DO NOT:
- Create UI
- Create XAML pages
- Create styles
- Create animations
- Create navigation
- Rebuild existing launcher shell

ONLY build:
- Detection systems
- Launch systems
- Monitoring systems
- Models
- Services
- Helpers
- ViewModels

The code should integrate directly into the existing launcher UI from Phase One.