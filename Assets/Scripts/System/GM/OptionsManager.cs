using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;



public class OptionsManager : MonoBehaviour
{

    //Launcher Options
    public bool widescreen = true;
    public int launcherIdleTimeBeforeAttract;

    public bool initializing = true;

    public float tweenTime = 0.7f;

    //Default Folders
    public string dataPath;
    public string defaultOptionsPath;
    public string optionsPath;
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";

    //Runner Settings
    public int runnerSecondsIdle = 10;
    public int runnerSecondsESCHeld = 3;
    public int runnerSecondsIdleInitial = 30;

    //Keys
    public KeyBindings keys;
    public KeyTranslator keyTranslator;

    // Parsed winnitron_options.json
    public JSONNode O;

    public JSONNode language;
    public JSONNode logger;

    public void Start() {
        // we need to load default language here, so we can display errors
        language = GM.Instance.data.GetDefautLanguage();
        defaultOptionsPath = Path.Combine(Application.dataPath, "Options");
        optionsPath = defaultOptionsPath;
        keyTranslator = new KeyTranslator(defaultOptionsPath);

        GM.Instance.logger.Info("DEFAULT OPTIONS PATH:" + optionsPath);

        // Figure out where the Options are by reading the .json in Options file
        string userdataFile = Path.Combine(optionsPath, "winnitron_userdata_path.json");
        GM.Instance.logger.Info("reading userdata location from " + userdataFile);
        if (File.Exists(userdataFile))
        {
            dataPath = GM.Instance.data.LoadJson(userdataFile)["userDataPath"];
        }
        else
        {
            GM.Instance.Oops(GM.Instance.Text("error", "cannotFindUserDataPathJson"), true);
        }

        GM.Instance.logger.Info("DATA PATH:" + dataPath);
        optionsPath = Path.Combine(dataPath, "Options");
        GM.Instance.logger.Info("CONFIGURED OPTIONS PATH:" + optionsPath);
        string optionsFile = Path.Combine(optionsPath, "winnitron_options.json");

        //Load that JSON
        GM.Instance.logger.Info("Loading options from " + optionsFile);

        if (File.Exists(optionsFile))
        {
            O = GM.Instance.data.LoadJson(optionsFile);

            logger = O["logger"];
            switch (logger["level"].Value.ToLower())
            {
                case "debug":
                    Logger.logLevel = Logger.LogLevels.Debug;
                    break;
                case "info":
                case null:
                    Logger.logLevel = Logger.LogLevels.Info;
                    break;
                case "warn":
                    Logger.logLevel = Logger.LogLevels.Warn;
                    break;
                case "error":
                    Logger.logLevel = Logger.LogLevels.Error;
                    break;
                default:
                    Logger.logLevel = Logger.LogLevels.Info;
                    GM.Instance.logger.Warn("Invalid log level '" + logger["level"] + "'. Setting to default 'info'. Valid values are 'debug', 'info', 'warn', 'error'.");
                    break;
            }

            SetKeys();

            //Launcher Settings
            GM.Instance.logger.Info(this, "OPTIONS:" + O.ToString());
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
                GM.Instance.logger.Info(this, "LANGUAGE FILE: " + langFile);
                language = GM.Instance.data.LoadJson(langFile);
            }

            //Sync Options
            var mode = O["sync"]["type"].Value.ToLower();

            GM.Instance.logger.Info(this, "SYNCMODE " + mode);

            if (mode == "local" || mode == "localonly" || mode == "none")
                GM.Instance.sync.syncType = GameSync.SyncType.NONE;
            else if (mode == "daily")
                GM.Instance.sync.syncType = GameSync.SyncType.DAILY;
            else
                GM.Instance.sync.syncType = GameSync.SyncType.ALWAYS;

            GM.Instance.sync.timeToUpdate = new TimeSpan(O["sync"]["dailySyncTime"]["hour"].AsInt, O["sync"]["dailySyncTime"]["minute"].AsInt, 0);
            GM.Instance.logger.Info("OPTIONS: Time to Update is " + GM.Instance.sync.timeToUpdate.ToString());

            GM.Instance.sync.syncOnStartup = O["sync"]["syncOnStartup"].AsBool;
        }
        else
        {
            GM.Instance.logger.Error("Cannot find options file: " + optionsFile);
            GM.Instance.Oops(GM.Instance.Text("error", "cannotFindJson"), true);
        }

        GM.Instance.logger.Debug("OPTIONS: Done Init.");
    }

    public string GetText(string category, string text)
    {
        return language[category][text];
    }


    public JSONNode GetSyncSettings()
    {
        if (O == null) {
            GM.Instance.logger.Error("Trying to access options before they've been loaded.");
            return null;
        } else {
            return O["sync"];
        }

    }

    private string InitDataFolder(string jsonKey)
    {
        string path = O["folders"][jsonKey];
        string dirName = jsonKey[0].ToString().ToUpper() + jsonKey.Substring(1, jsonKey.Length - 1);

        if (path.ToString().Contains("default"))
            path = Path.Combine(dataPath, dirName);

        GM.Instance.logger.Info(this, "OPTIONS: " + dirName + " path is " + path);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private void SetKeys()
    {
        JSONNode[] players = new JSONNode[5];
        players[1] = O["keycodes"]["player1"];
        players[2] = O["keycodes"]["player2"];
        players[3] = O["keycodes"]["player3"];
        players[4] = O["keycodes"]["player4"];

        for (int pNum = 1; pNum <= 4; pNum++)
        {
            JSONNode player = players[pNum];

            foreach (string control in KeyBindings.CONTROLS)
            {
                if (player[control] != null)
                {
                    KeyCode customKey = keyTranslator.fromAHK(player[control]);
                    keys.SetKey(pNum, control, customKey);
                    GM.Instance.logger.Debug("CUSTOM KEY (player " + pNum + " " + control + "): " + customKey);
                }
            }
        }


    }
}