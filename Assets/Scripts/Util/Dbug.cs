using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dbug: MonoBehaviour {
	
	public bool globalOn = false;

    public List<Object> supress;
    public List<Object> show;

	public void Log(Object mb, string x) 
	{
        if (CanShow(mb)) Debug.Log(x);
	}

	public void On() { globalOn = true; }
	public void Off() { globalOn = false; }

    private bool CanShow(Object mb)
    {
        bool result = false;

        if (globalOn) result = true;

        foreach (Object m in show)
            if (m == mb) result = true;

        foreach (Object m in supress)
            if (m == mb) result = false;

        return result;
    }
}

