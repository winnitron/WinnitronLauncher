using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Playlist 
{
	public string name;
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
		this.directory = new DirectoryInfo(directory);

		BuildPlaylist();
	}

	//Where the magic happens
	public void BuildPlaylist()
	{	
		foreach (var gameDir in directory.GetDirectories()) 
		{
			//Don't pick any directories that start with a dot
			if(gameDir.Name.Substring(0, 1) != ".") 
			{
				//Add a game!  Pass the Game constructor the directory where the game is contained
				games.Add(new Game(gameDir.FullName));
			}
		}   

		Debug.Log ("Playlist Built! Games:" + games.ToString());
	}
}

