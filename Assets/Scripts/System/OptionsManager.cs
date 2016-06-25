using UnityEngine;
using System.Collections;
using SimpleJSON;

public class OptionsManager : MonoBehaviour {

    //Launcher Options
    public bool widescreen = true;
    public enum ForceMode {LOCALONLY, REMOTEONLY, NONE};
    public ForceMode forceMode = ForceMode.NONE;

    //Default Folders
    public string contentPath = "c:/WINNITRON/Winnitron_Data";
    public string playlistsPath = "/Playlists";
    public string musicPath = "/Music";
    public string attractPath = "/Attract";
    public string syncPath = "/Sync";
    public string syncBat = "/Sync/Bin";

	// Use this for initialization
	void Start () {

        //Load that JSON
        var O = GM.data.LoadJson(Application.dataPath + "/winnitron_options.json");

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

        playlistsPath = contentPath + O["defaultFolders"]["playlists"];
        musicPath = contentPath + O["defaultFolders"]["music"];
        attractPath = contentPath + O["defaultFolders"]["attract"];
        Debug.Log("attract " + attractPath);
        syncPath = contentPath + O["defaultFolders"]["sync"];
        syncBat = contentPath + O["defaultFolders"]["syncBat"];
    }
}
