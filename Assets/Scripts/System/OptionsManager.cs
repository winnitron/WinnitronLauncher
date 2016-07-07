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
    public string contentPath = "c:/WINNITRON/Winnitron_Data";
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";
    public string syncPath = "/Sync";
    public string syncExe = "/sync.exe";

	//language
	public JSONNode language;

	// Use this for initialization
	void Awake () {

        //Load that JSON
        var O = GM.data.LoadJson(Application.dataPath + "/Options/winnitron_options.json");

        //Widescreen
       if (O["launcher"]["widescreen"] == "false") widescreen = false;

       //Force Mode
       //If local only is checked, the launcher will not run the sync program at all and preserve all folders
       //If remote only is checked, the launcher will nuke all folders and use the updated version only
        if (O["launcher"]["forceMode"] == "localOnly") forceMode = ForceMode.LOCALONLY;
        else if (O["launcher"]["forceMode"] == "remoteOnly") forceMode = ForceMode.REMOTEONLY;
        else forceMode = ForceMode.NONE;

        //Default Folders
        string path = O["defaultFolders"]["root"];
        if (path == "normal") contentPath = Application.dataPath;
        else contentPath = O["defaultFolders"]["root"];

        GM.dbug.Log(this, "OPTIONS: Root path for content is " + contentPath);

        //Find playlists path
        //Checks for a ":" to see if it's a absolute or relative path
        path = O["defaultFolders"]["playlists"];
        if (path.Contains(":")) playlistsPath = path;   //absolute
        playlistsPath = contentPath + path;             //relative

        GM.dbug.Log(this, "OPTIONS: Playlists path is: " + playlistsPath);

        //Find music path
        path = O["defaultFolders"]["music"];
        if (path.Contains(":")) musicPath = path;
        else musicPath = contentPath + path;

        GM.dbug.Log(this, "OPTIONS: Music path is " + musicPath);

        //Find attract path
        path = O["defaultFolders"]["attract"];
        if (path.Contains(":")) attractPath = path;
        else attractPath = contentPath + O["defaultFolders"]["attract"];

        GM.dbug.Log(this, "OPTIONS: Attract path is " + attractPath);

        //Find Sync Exe Path
        path = O["defaultFolders"]["sync"];
        if (path.Contains(":")) syncPath = path;
        else syncPath = contentPath + path;

        GM.dbug.Log(this, "OPTIONS: Sync path is " + syncPath);

        //Find Sync Exe Path
        path = O["defaultFolders"]["syncExe"];
        if (path.Contains(":")) syncExe = path;
        else syncExe = syncPath + path;

        GM.dbug.Log(this, "OPTIONS: Sync exe is " + syncExe);

        //Load language file
        language = GM.data.LoadJson(contentPath + "/Options/winnitron_text_" + O["launcher"]["language"] + ".json");

        initializing = false;
    }

	public string GetText(string category, string text)
	{
		return language[category][text];
	}
}
