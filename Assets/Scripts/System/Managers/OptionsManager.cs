using UnityEngine;
using System;
using System.IO;
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
    public int launcherIdleTimeBeforeAttract;

    public bool initializing = true;

    public float tweenTime = 0.7f;

    //Default Folders
    public string dataPath;
    public string optionsPath;
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

    public JSONNode logger;

    //Options JSON
    private JSONNode O;

    // Use this for initialization
    void Awake() {

        // we need to load default language here, so we can display errors
        language = GM.data.GetDefautLanguage();
        optionsPath = Path.Combine(Application.dataPath, "Options");
        Debug.Log("DEFAULT OPTIONS PATH:" + optionsPath);

        // Figure out where the Options are by reading the .json in Options file
        string userdataFile = Path.Combine(optionsPath, "winnitron_userdata_path.json");
        Debug.Log("reading userdata location from " + userdataFile);
        if (System.IO.File.Exists(userdataFile))
        {
            dataPath = GM.data.LoadJson(userdataFile)["userDataPath"];
        } else {
            GM.Oops(GM.Text("error", "cannotFindUserDataPathJson"), true);
        }

        Debug.Log("DATA PATH:" + dataPath);
        optionsPath = Path.Combine(dataPath, "Options");
        Debug.Log("CONFIGURED OPTIONS PATH:" + optionsPath);
        string optionsFile = Path.Combine(optionsPath, "winnitron_options.json");

        //Load that JSON
        Debug.Log("Loading options from " + optionsFile);
        Debug.Log("Options file exists: " + System.IO.File.Exists(optionsFile));

        if (System.IO.File.Exists(optionsFile))
        {
            O = GM.data.LoadJson(optionsFile);

            //Launcher Settings
            GM.dbug.Log(this, "O:" + O.ToString());
            if (O["launcher"]["widescreen"] != null) widescreen = O["launcher"]["widescreen"].AsBool;
            if (O["launcher"]["idleTimeBeforeAttract"] != null) launcherIdleTimeBeforeAttract = O["launcher"]["idleTimeBeforeAttract"].AsInt;

            //Runner Settings
            if (O["runner"]["timeToHoldESCToQuit"] != null) runnerSecondsESCHeld = O["runner"]["timeToHoldESCToQuit"].AsInt;
            if (O["runner"]["idleTimeSeconds"] != null) runnerSecondsIdle = O["runner"]["idleTimeSeconds"].AsInt;
            if (O["runner"]["initialIdleTimeSeconds"] != null) runnerSecondsIdleInitial = O["runner"]["initialIdleTimeSeconds"].AsInt;


            playlistsPath = InitDataFolder("playlists");
            musicPath = InitDataFolder("music");
            attractPath = InitDataFolder("attract");

            //Load language file
            if (O["launcher"]["language"] != null)
            {
                string langFile = Path.Combine(optionsPath, "winnitron_text_" + O["launcher"]["language"] + ".json");
                GM.dbug.Log(this, "LANGUAGE FILE: " + langFile);
                language = GM.data.LoadJson(langFile);
            }

            //Sync Options
            var mode = O["sync"]["type"].Value.ToLower();

            GM.dbug.Log(this, "SYNCMODE " + mode);

            if (mode == "local" || mode == "localonly" || mode == "none")
                GM.sync.syncType = GameSync.SyncType.NONE;
            else if (mode == "daily")
                GM.sync.syncType = GameSync.SyncType.DAILY;
            else
                GM.sync.syncType = GameSync.SyncType.ALWAYS;

            GM.sync.timeToUpdate = new TimeSpan(O["sync"]["dailySyncTime"]["hour"].AsInt, O["sync"]["dailySyncTime"]["minute"].AsInt, 0);
            GM.dbug.Log(this, "OPTIONS: Time to Update is " + GM.sync.timeToUpdate.ToString());

            GM.sync.syncOnStartup = O["sync"]["syncOnStartup"].AsBool;

            logger = O["logger"];

            // Tell GM that Options is done with all the Init stuff
            initializing = false;
        }
        else
        {
            GM.Oops(GM.Text("error", "cannotFindJson"), true);
        }
    }

    public string GetText(string category, string text)
    {
        return language[category][text];
    }


    public JSONNode GetSyncSettings()
    {
        return O["sync"];
    }

    private string InitDataFolder(string jsonKey)
    {
        string path = O["folders"][jsonKey];
        string dirName = jsonKey[0].ToString().ToUpper() + jsonKey.Substring(1, jsonKey.Length - 1);

        if (path.ToString().Contains("default"))
            path = Path.Combine(dataPath, dirName);

        GM.dbug.Log(this, "OPTIONS: " + dirName + " path is " + path);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}
