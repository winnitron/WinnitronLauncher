using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

	public List<Playlist> playlists;
	public List<Song> songs;
	public List<Sprite> attractModeImages;

	public string playlistDirectory;
	public string songsDirectory;
	public string attractDirectory;

	// Use this for initialization
	void Start () {
		GetPlaylists ();
		GetAttractModeImages ();
		GetMusic ();
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
