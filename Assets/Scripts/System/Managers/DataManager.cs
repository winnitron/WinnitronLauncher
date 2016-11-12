using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class DataManager : Singleton<DataManager>  {

	public List<Playlist> playlists;
	public List<Song> songs;
	public List<Sprite> attractModeImages;

    PlaylistNavigationManager playlistNavManager;

	// Use this for initialization
	void Start () {
		playlists = new List<Playlist> ();
		songs = new List<Song> ();
		attractModeImages = new List<Sprite> ();
	}

	public void SyncData()
	{
		GM.runner.RunSync();
	}

	public void LoadData()
	{
		playlistNavManager = GameObject.Find("PlaylistNavigationManager").GetComponent<PlaylistNavigationManager>();

		//Load everything!
		GetPlaylists ();
		GetAttractModeImages ();
		GetMusic ();
		
        // Build the visuals from this data
        playlistNavManager.BuildPlaylists();

		//Do this when done loading

		GM.state.ChangeState(StateManager.WorldState.Intro);
	}

	// Builds a list of Game objects based on the game directory inside its main directory. Then instantiates the GameNavigationManager, which then instantiates the ScreenShotDisplayManager
	public void GetPlaylists() 
	{
		var playlistDir = new DirectoryInfo(GM.options.playlistsPath);
		
		foreach (var dir in playlistDir.GetDirectories()) 
		{
			//Don't pick any directories that start with a dot
			if(dir.Name.Substring(0, 1) != ".")
			{
				//Add the playlist to the list
				//The Playlist builds the games out from the Playlist constructor
				playlists.Add(new Playlist(dir.FullName));
			}
		}   
	}
	
	public void GetAttractModeImages()
	{
		//Get stuff here
	}

	public void GetMusic()
	{
		//Get stuff here
	}

	public bool UpdatePlaylists()
	{
		//For when there are new games added to the list without shutting the launcher down
		return true;
	}

	public JSONNode LoadJson(string fileLocation)
	{
		try
		{
			using (StreamReader r = new StreamReader(fileLocation))
				return JSONNode.Parse(r.ReadToEnd());
		} 

		catch
		{
			GM.Oops (GM.Text("error", "cannotFindJson"));
			return null;
		}
	}
}
