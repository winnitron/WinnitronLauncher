using UnityEngine;
using System.Collections;
using SimpleJSON;

public class OptionsManager : MonoBehaviour {

    //Launcher Options
    public bool widescreen = true;
    public enum ForceMode {LOCALONLY, REMOTEONLY, NONE};
    public ForceMode forceMode = ForceMode.NONE;

    public bool initializing = true;

    //Default Folders
    public string dataPath;
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";

    //language
    public JSONNode language;

    //Sync Settings
    public JSONNode sync;

    //Options JSON
    private JSONNode O;

    // Use this for initialization
    void Awake() {

        dataPath = Application.dataPath;

        //Load that JSON
        string optionsFile = dataPath + "/Options/winnitron_options.json";
        Debug.Log("Loading options from " + optionsFile);
        O = GM.data.LoadJson(optionsFile);


        //Widescreen
        if (O["launcher"]["widescreen"] == "false")
            widescreen = false;

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

        initializing = false;
    }

    public string GetText(string category, string text)
    {
        Debug.Log("getting text: " + category + "," + text);
        return language[category][text];
    }


    public JSONNode GetSyncSettings()
    {
        return O["sync"];
    }
}