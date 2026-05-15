# Phase 2: Game Engine & Detection Systems

## Overview
Implemented the backend core responsible for detecting, launching, and monitoring games across multiple platforms.

## Key Accomplishments
- **Game Detection:**
    - **Steam:** Parsing `libraryfolders.vdf` and `appmanifest_*.acf` for multi-drive support.
    - **Riot Games:** Detecting VALORANT and League of Legends via directory scanning.
    - **Epic Games:** Parsing `.item` manifest files for installed titles.
    - **Battle.net:** Registry and directory scanning for Blizzard/Activision titles.
- **Execution Engine:** Developed `LauncherService` to handle asynchronous process spawning with proper working directory and argument support.
- **Real-time Monitoring:** Implemented `GameMonitoringService` to poll active processes, detecting game starts, exits, and crashes in the background.
- **Core Infrastructure:**
    - `RegistryHelper`: Abstracted Windows Registry access.
    - `ProcessHelper`: Simplified process lookup and spawning.
    - `FileSystemHelper`: Efficient directory scanning and size calculation.

## Technical Details
- **Models:** Introduced `GameInfo`, `GameState`, and `GamePlatform`.
- **ViewModels:** Created `InstalledGamesViewModel` for game collection management and `GameCardViewModel` for individual game actions.
- **Concurrency:** Leveraged `async/await` and `Task.Run` extensively to ensure the UI remains responsive during heavy IO scans.
