# CHANGELOG

All changes to this project are reported here. The versioning system is of semantic type (x.y.z) and the changes are reported for every release. Releases are sorted by date, with the latest release on top. Versions where the major release (x) is 0 is considered an alpha release and probably has numerous bugs. Versions that indicate a stable release have an ending L letter at the end of the version, for example (0.0.1L or 1.2.10L). A stable release is a release that passed all expected verifications and tests. Each release includes a brief description of the changes it brings, as well as a list of changes, additions and removals.

## [0.1.0] 30/07/2023
**Full Changelog**: https://github.com/lorycontixd/SudokuM/commits/v0.1.0

### Changes
1. Updated cell selection system:
- Previously it was necessary to selected the digit first and then click the cell to change the value.
- Now first click the cell to select it and then click the digit to change the value

2. Fixed bug on delete move, where the changed border would remain on the cell.

### Additions
1. Added game modes but no functionality yet



## [0.0.0] 29/07/2023
This is the first release of the application. It includes a working multiplayer lobby system with paired ui, working matchmaking, a gameplay where it is not clear if the players play together or against each other, and a system of statistics.

### Additions
- Lobby system (menu, matchmaking and lobby).
- Matchmaking system (currently no random matchmaking, only by invite or by joining a room).
- Sudoku generation algorithms and solution checkers.
- Multiplayer gameplay
- Stats system

### Next patch preview
- Addition of coop and time race gameplay.
- Persistent player stats locally.