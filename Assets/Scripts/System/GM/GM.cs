using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class GM : Singleton<GM> {

    public string versionNumber;

    //reference to the
    public Runner runner;
    public DataManager data;
    public OptionsManager options;
    public Animator state;
    public Logger logger;
    public GameSync sync;
    public WinnitronNetwork network;
    public LogOutputHandler logOutput;
    public VideoManager video;
    public StateMachineHelper stateMachineHelper;
    public Jukebox jukebox;


    //SINGLETON STUFF

    protected GM () { }

    //END SINGLETON STUFF


    void Start() {

        //Get the version number and other bulljazz
        logger.Info("#####  VERSION " + versionNumber + " #####");
        writeProcessInfo();
    }

    /// <summary>
    /// Reinitializes all the data in the GM.Instance.
    /// </summary>
    public void Init()
    {
        logger.Debug("GM initializing!");

        Cursor.visible = false;

        //Do Windows window management shizzle
        ResetScreen();

        Instance.StartCoroutine("Initialize");
    }

    IEnumerator Initialize()
    {
        //Let's initialize stuff in order
        logger.Debug("Options INit!");
        options.Init();
        logger.Debug("runner INit!");
        runner.Init();
        logger.Debug("sync INit!");
        sync.Init();

        while (!sync.isFinished)
            yield return null;

        logger.Debug("Data INit!");
        data.Init();

        state.SetTrigger("NextState");
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
    public void Oops(string text, bool isCritical)
    {
        //state.Oops(text, isCritical);
    }

    /// <summary>
    /// Causes an Oops screen to appear and assumes Non-Critical.  This function calls the real Oops in StateManager.cs
    /// </summary>
    /// <param name="text">Text to show on the Oops screen</param>
    public void Oops(string text)
    {
        //state.Oops(text, false);
    }

    /// <summary>
    /// Gets text from the apporpriate language file.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="type"></param>
    /// <returns>A plain text string.</returns>
    public string Text(string category, string type)
    {
        return options.GetText (category, type);
    }

    /// <summary>
    /// Restarts the launcher
    /// </summary>
    public void Restart()
    {
        state.SetTrigger("Init");
        ResetScreen();
    }

    private void writeProcessInfo() {
        string info = System.Diagnostics.Process.GetCurrentProcess().Id +
                      "\n" +
                      Path.Combine(Path.GetFullPath("."), "WINNITRON.bat");

        GM.Instance.logger.Debug("writing pid and exe path to " + PidFile());
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