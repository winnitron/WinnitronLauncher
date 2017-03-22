; LEGACY GAME RUN SCRIPT

#Persistent
#MaxHotkeysPerInterval 200
#SingleInstance Force

debug := true

executable := "C:\WINNITRON_UserData\Playlists\official-winnitron\nidhogg\Nidhogg.exe"
forceQuitHoldTime := 3000
idleLimit := 30000
initialWait := 30000

start := SecondsToday()
WriteLog("START ---------------- " . A_Now)

; RUN THE GAME

Run, %executable%, , , process_pid
WriteLog("Launched " . executable . " with pid " . process_pid)

SetTimer, InitialWait, -%initialWait% ; negative period disables timer after first trigger
MouseMove 3000, 3000, 0

; This is the function that quits the game
KillApp()
{

	global process_pid
	WriteLog("Killing app with pid " . process_pid)
	WinKill, ahk_exe %executable%	; Tries to close using .exe
	WinKill, ahk_pid process_pid	; Tries to close using process id
	SetTitleMatchMode, 2
	WinKill, Sumo Topplers			; Tries to close hoping that part of the game name is in the title
	ExitApp
}

; Do this so we don't keep running through InitialWait and CloseOnIdle
Loop
{
}

InitialWait:
	WriteLog("Completed initial wait")
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
	WriteLog("ESC pressed")
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

; KEYMAPS BELOW (NONE IN DEFAULT SCRIPT)

