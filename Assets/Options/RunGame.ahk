; LEGACY GAME RUN SCRIPT

#Persistent
#MaxHotkeysPerInterval 200


; RUN THE GAME
Run, C:\WINNITRON_UserData\Playlists\official-winnitron\sumo-topplers/Sumo Topplers.exe, , , process_pid
idleLimit:= 10000 ; three seconds
SetTimer, InitialWait, 30000
MouseMove 3000, 3000, 0

; This is the function that quits the app
KillApp()
{
	WinKill, ahk_exe C:\WINNITRON_UserData\Playlists\official-winnitron\sumo-topplers/Sumo Topplers.exe	; Tries to close using .exe
	WinKill, ahk_pid process_pid	; Tries to close using process id
	SetTitleMatchMode, 2
	WinKill, Sumo Topplers			; Tries to close hoping that part of the game name is in the title
	ExitApp
}

InitialWait:
	SetTimer,  CloseOnIdle, % idleLimit+150
return

; This is the timer
CloseOnIdle:
	if (A_TimeIdle>=idleLimit)
	{
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
	SetTimer, WaitForESCRelease, 3000		; 3 seconds
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

; KEYMAPS BELOW

.::z
/::x

w::i
a::j
s::k
d::l
`::n
1::m