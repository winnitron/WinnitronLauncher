; DEFAULT RUN SCRIPT

#Persistent
#MaxHotkeysPerInterval 200
#SingleInstance Force

debug := true

executable := "C:\WINNITRON_UserData\Playlists\first-friday\4forths\4fourths.exe"
exec_file := "4fourths.exe"
game_name := "4Forths"
forceQuitHoldTime := 3000
idleLimit := 180000
initialWait := 30000

start := SecondsToday()
WriteLog("START ---------------- " . A_Now)

SetTitleMatchMode, 2

; RUN THE GAME
Run, %executable%, , , process_id_1
WriteLog("Launched " . executable . " with pid " . process_id_1)

SetTimer, InitialWait, -%initialWait% ; negative period disables timer after first trigger
MouseMove 3000, 3000, 0

; This is the function that quits the game (hopefully)
KillApp()
{
  global process_id_1
  global process_id_2

  WriteLog("Killing app with pids " . process_id_1 . " and " . process_id_2)

  ; Nuke the site from orbit. SOMETHING should work....

  WinKill, ahk_exe %executable% ; Tries to close using .exe
  WinKill, ahk_exe %exec_file%

  WinKill, ahk_pid process_id_2 ; Tries to close using process id
  WinKill, ahk_pid process_id_1

  WinKill, %game_name%

  Run, TaskKill /f /pid %process_id_2%
  Run, TaskKill /f /pid %process_id_1%
  Run, TaskKill /f /im %exec_file%

  SetTitleMatchMode, RegEx
  IfWinExist, i)WinnitronLauncher
  {
    WriteLog("winnitron window id " . WinExist("A"))
    WinActivate, i)WinnitronLauncher
    WinWaitActive, i)WinnitronLauncher, , 2
    PostMessage, 0x112, 0xF030,,, i)WinnitronLauncher  ; 0x112 = WM_SYSCOMMAND, 0xF030 = SC_MAXIMIZE
  } else {
    WriteLog("couldn't find winnitron window")
  }

  ExitApp
}

Loop
{
  ; Ensure that the AHK script exits when the game does, because it's *this*
  ; process that the Launcher is watching so it knows when to wake up and
  ; kick back to menu.
  Process, Exist, %process_id_1%
  if (ErrorLevel == 0) {
    WriteLog("detected game not running")
    KillApp()
  }

}

InitialWait:
  ; Some games launch a second process
  WinGet, process_id_2, PID, %game_name%

  WriteLog("Completed initial wait (pid2: " . process_id_2 . ")")

  SetTimer,  CloseOnIdle, % idleLimit+150
return

; This is the timer
CloseOnIdle:
  if (A_TimeIdle >= idleLimit)
  {
    WriteLog("Idle timeout!")
    KillApp()
    SetTimer,CloseOnIdle, Off
  }
  else
  {
    SetTimer,CloseOnIdle, % idleLimit-A_TimeIdle+150
  }
return

; Do this stuff when Esc is pressed
~Esc::
  If escIsPressed
    return
  escIsPressed := true
  SetTimer, WaitForESCRelease, %forceQuitHoldTime%
return

; Do this stuff when Esc is UP
~Esc Up::
  SetTimer, WaitForESCRelease, Off
  escIsPressed := false
return

WaitForESCRelease:
  SetTimer, WaitForESCRelease, Off
  KillApp()
return


; DEBUGGING STUFF

; Number of seconds since midnight.
SecondsToday() {
  return A_Hour * 3600 + A_Min * 60 + A_Sec
}

WriteLog(message)
{
  global debug
  global start

  if (debug) {
    runningTimeSec := SecondsToday() - start
    debugLog := "ahk_output.txt"
    FileAppend,
    (
    %runningTimeSec%s %A_Tab% %message%

    ), %debugLog%, UTF-8
  }
}

; KEYMAPS BELOW
Up::Up
Down::Down
Left::Left
Right::Right
.::x
/::z
w::i
s::k
a::j
d::l
`::n
1::m
i::i
k::k
j::j
l::l
g::g
h::h
Numpad8::Numpad8
Numpad5::Numpad5
Numpad4::Numpad4
Numpad6::Numpad6
Numpad1::Numpad1
Numpad2::Numpad2
