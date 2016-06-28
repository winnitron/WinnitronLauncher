using UnityEngine;
using System.Collections;

public class Dbug {
	
	public bool globalOn = false;

	public void Log(string x) 
	{
		if(globalOn) Debug.Log(x);
	}

	public void On() { globalOn = true; }
	public void Off() { globalOn = false; }
}
