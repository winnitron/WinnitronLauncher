using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Dbug: MonoBehaviour {
	
	public bool globalOn = false;
    public bool showNull = false;
    public bool showTimestamps = true;

    public List<UnityEngine.Object> supress;
    public List<UnityEngine.Object> show;

	public void Log(UnityEngine.Object mb, string x) 
	{
        if (CanShow(mb))
        {
            if(showTimestamps) Debug.Log(DateTime.Now.ToLocalTime().ToString() + " -- " + x);
            else Debug.Log(x);
        }
	}

	public void On() { globalOn = true; }
	public void Off() { globalOn = false; }

    private bool CanShow(UnityEngine.Object mb)
    {
        bool result = false;

        if (globalOn) result = true;
        if (mb == null && showNull) result = true;

        foreach (UnityEngine.Object m in show)
            if (m == mb) result = true;

        foreach (UnityEngine.Object m in supress)
            if (m == mb) result = false;

        return result;
    }
}

