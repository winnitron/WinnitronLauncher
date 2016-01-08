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
		//Find out the name of the directory
		this.directory = new DirectoryInfo(directory);
		//Replace the underscores for a cleaner name
		name = this.directory.Name.Replace('_', ' ');

		//Init the games list
		this.games = new List<Game>();

		//Let's find all the gamesssss
		BuildPlaylist();
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

		Debug.Log ("Playlist Built! : " + name);
	}
}

