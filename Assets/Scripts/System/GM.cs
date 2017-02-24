using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GM : Singleton<GM> {

    public string versionNumber;

	//reference to the 
	public static Runner runner;
	public static DataManager data;
    public static OptionsManager options;
	public static StateManager state;
    public static Dbug dbug;
    public static GameSync sync;

    public static Jukebox jukebox;

	new void Awake() {

        Debug.Log("#####  VERSION " + versionNumber + " #####");

		//Cursor.visible = false;

		runner = GetComponent<Runner>();
		data = GetComponent<DataManager> ();
        options = GetComponent<OptionsManager> ();
		state = GetComponent<StateManager> ();
        dbug = GetComponent<Dbug>();
        sync = GetComponent<GameSync>();

        //Not 100% sure why the jukebox is here. :S
        if (GameObject.Find("Jukebox"))
            jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();

        //Do Windows window management shizzle
        ResetScreen();
	}


	/// <summary>
    /// Causes an Oops screen to appear.  This function calls the real Oops in StateManager.cs
    /// </summary>
    /// <param name="text">Text to show on the Oops screen</param>
    /// <param name="isCritical">Critical will force quit the launcher</param>
	public static void Oops(string text, bool isCritical)
	{
        state.Oops(text, isCritical);
	}

    /// <summary>
    /// Causes an Oops screen to appear and assumes Non-Critical.  This function calls the real Oops in StateManager.cs
    /// </summary>
    /// <param name="text">Text to show on the Oops screen</param>
    public static void Oops(string text)
    {
        state.Oops(text, false);
    }

	public static string Text(string category, string type)
	{
		return options.GetText (category, type);
	}

    /// <summary>
    /// Restarts the launcher
    /// </summary>
    public static void Restart()
    {
        state.ChangeState(StateManager.WorldState.Intro);
    }


	/*
	 *   VERY IMPORTANT!!!!!!
	 * 	
	 * 	 Comment out the text below in order to debug on non-windows properly
	 *   
	 */

	//*
	#if UNITY_STANDALONE_WIN
	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern long SetWindowLong(long hwnd, long nIndex, long dwNewLong);


    public static void ResetScreen() {
        //SetWindowLong(FindWindow(null, Application.productName).ToInt32(), -16L, 0x00800000L);
        //SetWindowPos(FindWindow(null, Application.productName), 0, 0, 0, Screen.currentResolution.width, Screen.currentResolution.height, 0x0020);
        Screen.fullScreen = true;
    }

	#else

	public static void ResetScreen() {
	}

	#endif
    //*/

}