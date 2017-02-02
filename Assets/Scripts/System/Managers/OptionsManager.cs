using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class KeyBindings
{
    public KeyCode P1Up;
    public KeyCode P1Down;
    public KeyCode P1Left;
    public KeyCode P1Right;
    public KeyCode P1Button1;
    public KeyCode P1Button2;

    public KeyCode P2Up;
    public KeyCode P2Down;
    public KeyCode P2Left;
    public KeyCode P2Right;
    public KeyCode P2Button1;
    public KeyCode P2Button2;
}

public class OptionsManager : MonoBehaviour {

    //Launcher Options
    public bool widescreen = true;
    public enum SyncMode {LOCALONLY, NORMAL};
    public SyncMode syncMode = SyncMode.NORMAL;

    public bool initializing = true;

    public float tweenTime = 0.7f;

    //Default Folders
    public string dataPath;
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";

    //Utilities
    //public string legacyControlsPath = "/Util/WinnitronLegacy.exe";

    //Runner Settings
    public int runnerSecondsIdle = 10;
    public int runnerSecondsESCHeld = 3;
    public int runnerSecondsIdleInitial = 30;
    
    //Keys
    public KeyBindings keys;

    //language
    public JSONNode language;

    //Sync Settings
    public JSONNode sync;

    //Options JSON
    private JSONNode O;

    // Use this for initialization
    void Awake() {

        //Figure out where the Options are by reading the .json in Options file
        dataPath = GM.data.LoadJson(Application.dataPath + "/Options/winnitron_userdata_path.json")["userDataPath"];

        //Load that JSON
        string optionsFile = dataPath + "/Options/winnitron_options.json";
        Debug.Log("Loading options from " + optionsFile);
        O = GM.data.LoadJson(optionsFile);

        //Widescreen
        if (O["launcher"]["widescreen"] == "false")
            widescreen = false;

        //Runner Settings
        if(O["runner"]["timeToHoldESCToQuit"] != null) runnerSecondsESCHeld = O["runner"]["timeToHoldESCToQuit"];
        if(O["runner"]["idleTimeSeconds"] != null) runnerSecondsIdle = O["runner"]["idleTimeSeconds"];
        if(O["runner"]["initialIdleTimeSeconds"] != null) runnerSecondsIdleInitial = O["runner"]["initialIdleTimeSeconds"];

        //Playlists Path
        var path = O["defaultFolders"]["playlists"];
        if (path.ToString().Contains("default"))
            playlistsPath = dataPath + "/Playlists";
        else
            playlistsPath = path;

        GM.dbug.Log(this, "OPTIONS: Playlists path is: " + playlistsPath);

        //Find music path
        path = O["defaultFolders"]["music"];
        if (path.ToString().Contains("default"))
            musicPath = dataPath + "/Music";
        else
            musicPath = path;

        GM.dbug.Log (this, "OPTIONS: Music path is " + musicPath);

        //Find attract path
        path = O ["defaultFolders"] ["attract"];
        if (path.ToString().Contains("default"))
            attractPath = dataPath + "/Attract";
        else
            attractPath = path;

        GM.dbug.Log (this, "OPTIONS: Attract path is " + attractPath);
        
        //Load language file
        language = GM.data.LoadJson (dataPath + "/Options/winnitron_text_" + O ["launcher"] ["language"] + ".json");

        //Check forcemode
        GM.dbug.Log(this, "SYNCMODE " + O["launcher"]["syncMode"]);
        if(O["launcher"]["syncMode"].Value == "local" || O["launcher"]["syncMode"].Value == "localOnly")
            syncMode = SyncMode.LOCALONLY;

        initializing = false;
    }

    public string GetText(string category, string text)
    {
        return language[category][text];
    }


    public JSONNode GetSyncSettings()
    {
        return O["sync"];
    }
}