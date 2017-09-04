using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

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

        name = this.directory.Name;

        GM.dbug.Debug("Playlist: Name before fixes " + name);

        name = name.TrimStart('_');
        name = name.Replace('_', ' ').Replace('-', ' ');

        //Replace the underscores for a cleaner name
        name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

        GM.dbug.Debug("Playlist: Name after fixes " + name);

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

		GM.dbug.Info(null, "Playlist Built! : " + name);
	}

	public void BuildPlaylistJSON()
	{

	}
}

