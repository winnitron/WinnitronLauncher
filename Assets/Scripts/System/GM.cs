using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class GM : Singleton<GM> {

    public string versionNumber;
    public static string VersionNumber;

    //reference to the
    public static Runner runner;
    public static DataManager data;
    public static OptionsManager options;
    public static StateManager state;
    public static Logger logger;
    public static GameSync sync;
    public static WinnitronNetwork network;
    public static LogOutputHandler logOutput;
    public static VideoManager video;

    public static Jukebox jukebox;

    new void Awake() {
        Cursor.visible = false;

        runner = GetComponent<Runner>();
        data = GetComponent<DataManager> ();
        //options = GetComponent<OptionsManager> ();
        state = GetComponent<StateManager> ();
        logger = GetComponent<Logger>();
        //sync = GetComponent<GameSync>();
        //network = GetComponent<WinnitronNetwork>();
        logOutput = GetComponent<LogOutputHandler>();
        video = GetComponent<VideoManager>();

        VersionNumber = versionNumber;
        GM.logger.Info("#####  VERSION " + versionNumber + " #####");
        writeProcessInfo();

        //Not 100% sure why the jukebox is here. :S
        //if (GameObject.Find("Jukebox"))
        //    jukebox = GameObject.Find("Jukebox").GetComponent<Jukebox>();

        //Do Windows window management shizzle
        ResetScreen();
    }


    void OnApplicationQuit()
    {
        // File.Delete(PidFile());
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

    /// <summary>
    /// Gets text from the apporpriate language file.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="type"></param>
    /// <returns>A plain text string.</returns>
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
        ResetScreen();
    }

    private void writeProcessInfo() {
        string info = System.Diagnostics.Process.GetCurrentProcess().Id +
                      "\n" +
                      Path.Combine(Path.GetFullPath("."), "WINNITRON.bat");

        GM.logger.Debug("writing pid and exe path to " + PidFile());
        File.WriteAllText(PidFile(), info);
    }


    //*
    #if UNITY_STANDALONE_WIN

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    public static extern long SetWindowLong(long hwnd, long nIndex, long dwNewLong);

    public static void ResetScreen() {
        SetWindowLong(FindWindow(null, Application.productName).ToInt32(), -16L, 0x00800000L);
        SetWindowPos(FindWindow(null, Application.productName), 0, 0, 0, Screen.currentResolution.width, Screen.currentResolution.height, 0x0020);
        Screen.fullScreen = true;
    }

    public static string PidFile() {
        string drive = Environment.GetEnvironmentVariable("HOMEDRIVE");
        string path = Environment.GetEnvironmentVariable("HOMEPATH");

        return Path.Combine(Path.Combine(drive, path), "winnitron.pid");
    }

    #else

    public static void ResetScreen() {
    }

    public static string PidFile() {
        string path = Environment.GetEnvironmentVariable("HOME");
        return Path.Combine(path, "winnitron.pid");
    }

    #endif
    //*/

}