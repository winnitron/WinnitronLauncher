using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameRepository : Singleton<GameRepository> {

	public List<Game> games;
	
	string GAMES_SUBDIRECTORY = "Games";

	string gamesDirectory;

	protected override void Awake() {
		base.Awake();
		gamesDirectory = Path.Combine(Application.dataPath, GAMES_SUBDIRECTORY);
		BuildList();
	}

	void BuildList() {

		var gamesDir = new DirectoryInfo(gamesDirectory);

		foreach (var gameDir in gamesDir.GetDirectories()) {
			games.Add(CreateRepresentation(gameDir));
		}
	}

	Game CreateRepresentation(DirectoryInfo gameDirectory) {
		//var metaInfoPath = Path.Combine(gameDirectory.FullName, gameDirectory.Name + ".txt");
		//var metaInfo = File.ReadAllLines(metaInfoPath);
		var directoryName = gameDirectory.Name;//metaInfo[0];
        var name = directoryName.Replace('_', ' ');
		//var author = metaInfo[1];
		string author = null;
		
        // Load the screenshot from the games directory as a Texture2D
        var screenshot = new Texture2D(1024, 768);
		screenshot.LoadImage(File.ReadAllBytes(Path.Combine(gameDirectory.FullName, gameDirectory.Name + ".png")));

        // Turn the Texture2D into a sprite
        var screenshotSprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));

		var executablePath = Path.Combine(gameDirectory.FullName, gameDirectory.Name + ".exe");

        return new Game(name, author, screenshotSprite, executablePath);
	}

    void Update() {

        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            Application.LoadLevel(0);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2)) {
            Application.LoadLevel(1);
        }
    }
}
