using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataManager : Singleton<DataManager>  {

	public List<Playlist> playlists;
	public List<Song> songs;
	public List<Sprite> attractModeImages;

	public string attractSubdirectory;
	public string playlistSubdirectory;
	public string musicSubdirectory;
	public string syncSubdirectory;

	private string playlistDirectory;
	private string musicDirectory;
	private string attractDirectory;
	private string syncDirectory;


    PlaylistNavigationManager playlistNavManager;

	// Use this for initialization
	void Start () {
		playlists = new List<Playlist> ();
		songs = new List<Song> ();
		attractModeImages = new List<Sprite> ();

		playlistDirectory = Path.Combine (Application.dataPath, playlistSubdirectory);
		musicDirectory = Path.Combine (Application.dataPath, musicSubdirectory);
		attractDirectory = Path.Combine (Application.dataPath, musicSubdirectory);
		syncDirectory = Path.Combine (Application.dataPath, syncSubdirectory);

        playlistNavManager = GameObject.Find("PlaylistNavigationManager").GetComponent<PlaylistNavigationManager>();

		//SyncData ();
		LoadData();
	}

	public void SyncData()
	{
		//GM.runner.RunSync();
		LoadData();
	}

	public void LoadData()
	{
		//GM.Load ();

		//Load everything!
		GetPlaylists ();
		GetAttractModeImages ();
		GetMusic ();
		
        // Build the visuals from this data
        playlistNavManager.BuildPlaylists();

		//Do this when done loading
		GM.ChangeState (GM.WorldState.Intro);
	}

	// Builds a list of Game objects based on the game directory inside its main directory. Then instantiates the GameNavigationManager, which then instantiates the ScreenShotDisplayManager
	public void GetPlaylists() 
	{
		var playlistDir = new DirectoryInfo(playlistDirectory);
		
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
}
