using UnityEngine;
using System.IO;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class Game: Object
{

	public DirectoryInfo directory;

	public new string name;
	public string author;
	public Sprite screenshot;
	public string executable;

	/*
	 *	Game Constructor
	 *
	 *	The game only takes in a directory handed down by the Playlist class
	 *	It then finds all the relevant information for the Game
	 */
	public Game(string directory) 
	{
		this.directory = new DirectoryInfo (directory);

		//Check for the Winnitron Metadata JSON, and use oldschool folder naming if it doesn't exist
		if (System.IO.File.Exists (this.directory + "/winnitron_metadata.json")) {
			BuildGameJSON ();
		} else {
            BuildGame ();
		}
	}







	//Where the magic happens
	public void BuildGame()
	{        
		this.name = GetGameNameFromFolderName();
		this.author = null; //No author stuff just yet
		this.screenshot = GetScreenshot();
		this.executable = GetExecutablePath();

		GM.dbug.Log(this, "Game Built! Name: " + name + " Screenshot: " + screenshot.name + " exe path: " + executable);
	}

	public void BuildGameJSON()
	{
		var J = GM.data.LoadJson (directory.FullName + "/winnitron_metadata.json");

		this.name = J ["name"];
		this.author = null; //No author stuff just yet
		this.screenshot = GetScreenshot();
		this.executable = Path.Combine(directory.FullName + "/", J["executable"]);

        GM.dbug.Log(this, "Game Built JSON! Name: " + name + " Screenshot: " + screenshot.name + " exe path: " + executable);
	}






	//Private Functions (there's a joke here somewhere)

	private Sprite GetScreenshot()
	{
		// Load the screenshot from the games directory as a Texture2D
		var screenshotTex = new Texture2D(1024, 768);

        //screenshotTex.LoadImage(File.ReadAllBytes(Path.Combine(directory.FullName, directory.Name + ".png")));
        screenshotTex.LoadImage(File.ReadAllBytes(Directory.GetFiles(directory.FullName + "/", "*.png")[0]));

        // Turn the Texture2D into a sprite
        return Sprite.Create(screenshotTex, new Rect(0, 0, screenshotTex.width, screenshotTex.height), new Vector2(0.5f, 0.5f));
	}

	private string GetExecutablePath()
	{
		//Find the .exe in the directory and save a reference
		return Path.Combine(directory.FullName, executable);
	}

	private string GetGameNameFromFolderName()
	{
		//Figure out the name of the game from the directory title
		var directoryName = directory.Name;

		//Replace the underscores and dashes with blank spaces
		var name = directoryName.Replace('_', ' ');
		name = name.Replace('-', ' ');

		return name;
	}
}
