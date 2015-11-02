using UnityEngine;
using System.IO;
using System.Collections;

[System.Serializable]
public class Game 
{

	public DirectoryInfo directory;

	public string name;
	public string author;
	public Sprite screenshot;
	public string executablePath;

	/*
	 *	Game Constructor
	 *
	 *	The game only takes in a directory handed down by the Playlist class
	 *	It then finds all the relevant information for the Game
	 */
	public Game(string directory) 
	{
		this.directory = new DirectoryInfo (directory);

		BuildGame ();
	}

	//Where the magic happens
	public void BuildGame()
	{        
		//Figure out the name of the game from the directory title
		var directoryName = directory.Name;
		//Replace the underscores and dashes with blank spaces
		var name = directoryName.Replace('_', ' ');
		name = name.Replace('-', ' ');

		//No author stuff just yet
		string author = null;
		
		// Load the screenshot from the games directory as a Texture2D
		var screenshotTex = new Texture2D(1024, 768);
		screenshotTex.LoadImage(File.ReadAllBytes(Path.Combine(directory.FullName, directory.Name + ".png")));
		
		// Turn the Texture2D into a sprite
		var screenshot = Sprite.Create(screenshotTex, new Rect(0, 0, screenshotTex.width, screenshotTex.height), new Vector2(0.5f, 0.5f));

		//Find the .exe in the directory and save a reference
		var executablePath = Path.Combine(directory.FullName, directory.Name + ".exe");

		this.name = name;
		this.author = author;
		this.screenshot = screenshot;
		this.executablePath = executablePath;

		Debug.Log ("Game Built! Name: " + name + " Screenshot: " + screenshot.name + " exe path: " + executablePath);
	}
}
