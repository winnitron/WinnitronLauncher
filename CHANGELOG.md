# CHANGELOG

## 2.4.1

- Assorted visual fixes
- Support resolutions above 1080
- Much simpler and more aggressive game quit to help with button mashers

## 2.4.0

- Unity 2018.2
- Customizable AHK templates
- Better error messages when parsing invalid json
- Fix options loading so we can see very early Oops screens
- Show Oops screen for any sync exception

## v2.3.0

- OH MAN IT'S A WHOLE NEW LOOK
    + customizable intro and background videos
    + display more game and playlist info
    + support for video and plaintext Attract screens
- Skip sync if there's a problem connecting to the Network (rather than getting stuck on an error screen).
- [More reliable return to menu after game exit](https://github.com/winnitron/WinnitronLauncher/pull/82)
    + uses native window ID to find the Launcher
- Like games, playlists now use a `winnitron_metadata.json` file
    + More flexible playlist naming
    + Backwards compatible: falls back to previous behaviour if it doesn't exist
    + [Documentation for local playlists.](https://github.com/winnitron/WinnitronLauncher/wiki/Managing-Your-Games#using-the-winnitron-launcher-locally)

## v2.2.0

- [Fix/improve key map loading](https://github.com/winnitron/winnitronlauncher/pull/70)
- Send start/stop game events to the Network
- Use actually-specified game cover images in menu
- [Bug fixes in writing key map AHK](https://github.com/winnitron/winnitronlauncher/pull/77)

## v2.1.0

- support for games with custom key bindings!
- support for the launcher with custom key bindings!
- remote logging
- configurable log levels
- support for [`launcher_keep_awake`](https://github.com/winnitron/launcher_keep_awake)

## v2.0.0

- Necromancy!