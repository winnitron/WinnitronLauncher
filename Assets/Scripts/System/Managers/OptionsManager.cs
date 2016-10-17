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
    public string contentPath = "C:/WINNITRON/WINNITRON_Data";
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";
    public string syncPath = "/Sync";
    public string syncExe = "/sync.exe";

	//language
	public JSONNode language;

    //Sync Settings
    public JSONNode sync;

    //Options JSON
    private JSONNode O;

	// Use this for initialization
	void Awake () {

        contentPath = Application.dataPath;

        //Check options folder!
        if (!System.IO.Directory.Exists(contentPath + "/Options"))
        {
            initializing = false;
            GM.Oops("No Options folder!  You may need to redownload from GitHub.");
        }

        else
        {
            //Load that JSON
            O = GM.data.LoadJson(contentPath + "/Options/winnitron_options.json");

            //Load language file
            language = GM.data.LoadJson(contentPath + "/Options/winnitron_text_" + O["launcher"]["language"] + ".json");

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

            initializing = false;
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

    public JSONNode GetDirectory()
    {
        return O["defaultFolders"];
    }
}
