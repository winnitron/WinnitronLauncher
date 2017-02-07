#Persistent
#MaxHotkeysPerInterval 200


; RUN THE GAME
Run, C:\WINNITRON_UserData\Playlists\classic-winnitron\canabalt\Canabalt.exe
idleLimit:= 10000 ; three seconds
SetTimer, InitialWait, 30000
; SetTimer,  CloseOnIdle, % idleLimit+150

; This is the function that quits the app
KillApp()
{
	WinKill, ahk_exe C:\WINNITRON_UserData\Playlists\classic-winnitron\canabalt\Canabalt.exe
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
