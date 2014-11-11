using UnityEngine;
using System.Collections;

public class Dbug {
	
	public static bool isOn = false;

	public static void Log(string x) 
	{
		if(isOn) Debug.Log(x);
	}

	public static void On() { isOn = true; }
	public static void Off() { isOn = false; }
}
