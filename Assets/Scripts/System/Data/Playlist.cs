using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

[System.Serializable]
public class Playlist : Object
{
	public new string name;
	public DirectoryInfo directory;
	public List<Game> games;

	/*
	 *  Playlist Constructor
	 * 
	 *  Playlist only takes in the directory that the DataManager hands it 
	 *  and builds everything from there
	 */
	public Playlist(string directory) 
	{
		//Find out the name of the directory
		this.directory = new DirectoryInfo(directory);
		//Replace the underscores for a cleaner name
		name = this.directory.Name.Replace('_', ' ');
        name = this.directory.Name.Replace('-', ' ');

        //Init the games list
        this.games = new List<Game>();

		//Check for the Winnitron Metadata JSON, and use oldschool folder naming if it doesn't exist
		if (System.IO.File.Exists (this.directory + "winnitron_metadata.json")) {
			BuildPlaylistJSON ();
		} else {
			BuildPlaylist ();
		}
	}




	//Where the magic happens
	public void BuildPlaylist()
	{	
		foreach (var gameDir in directory.GetDirectories()) 
		{
			//Don't pick any directories that start with a dot
			if(!gameDir.Name.Substring(0, 1).Equals(".")) 
			{
				//Make a new game                
				Game newGame = new Game(gameDir.FullName);

				//Add a game!  Pass the Game constructor the directory where the game is contained
				games.Add(newGame);
			}
		}   

		GM.dbug.Log(this, "Playlist Built! : " + name);
	}

	public void BuildPlaylistJSON()
	{

	}
}

