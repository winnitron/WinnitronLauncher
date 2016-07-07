using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GM : Singleton<GM> {

	//reference to the 
	public static Runner runner;
	public static DataManager data;
    public static OptionsManager options;
	public static StateManager state;
    public static Dbug dbug;

	new void Awake() {
		//Cursor.visible = false;

		runner = GetComponent<Runner>();
		data = GetComponent<DataManager> ();
        options = GetComponent<OptionsManager> ();
		state = GetComponent<StateManager> ();
        dbug = GetComponent<Dbug>();

        //Do Windows window management shizzle
        ResetScreen();
	}


	//Shortcuts to often used functions in Managers
	public static void Oops(string o)
	{
		state.oops = o;
	}

	public static string Text(string category, string type)
	{
		return options.GetText (category, type);
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



#endif
    //*/

}
