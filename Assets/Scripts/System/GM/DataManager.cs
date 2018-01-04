using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class DataManager : Singleton<DataManager>  {

	public List<Playlist> playlists;
	public List<Song> songs;

    public void Init()
    {
        GM.Instance.logger.Debug("DATA: Starting Init...");
        //Do initial stuff here.
        ReloadData();
    }

    public void ReloadData()
	{
        //Start with new lists
        playlists = new List<Playlist>();
        songs = new List<Song>();

		//Load everything!
		GetPlaylists ();
		GetMusic ();

        //Call the delegate and all methods hooked in
        //Primarily used in LauncherUIController.cs to update the data model
        //But could be used elsewhere
        GM.Instance.logger.Info(this, "DataManager: finished updating data.");
	}

	// Builds a list of Game objects based on the game directory inside its main directory. Then instantiates the GameNavigationManager, which then instantiates the ScreenShotDisplayManager
	public void GetPlaylists()
	{
		var playlistDir = new DirectoryInfo(GM.Instance.options.playlistsPath);

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

	public void GetMusic()
	{
        // get all valid files
        var info = new DirectoryInfo(GM.Instance.options.musicPath);
        var songFiles = info.GetFiles();

        GM.Instance.logger.Info(this, "JUKEBOX: Started Loading Song Files.");

        foreach (var song in songFiles)
        {
            var fileExt = song.FullName.Substring(Mathf.Max(0, song.FullName.Length - 4));
            GM.Instance.logger.Info(this, "JUKEBOX: song extension is " + fileExt);

            if (song.Name.Substring(0, 1) != "." && fileExt == ".ogg")
            {
                GM.Instance.logger.Info(this, "JUKEBOX: Started loading " + song.FullName);

                WWW audioLoader = new WWW("file://" + song.FullName);

                while (!audioLoader.isDone) { }

                //Figure out the song/author names
                //take out the file extension
                var fullName = song.Name.Replace(".ogg", "");

                //find the '-' and split the string
                string[] words = fullName.Split('-');

                //First half is the song title, second the author
                var name = words[0];
                var author = words[1];

                //We're done!
                GM.Instance.logger.Info(this, "JUKEBOX: Can load song: " + audioLoader.GetAudioClip(false));
                songs.Add(new Song(name, author, audioLoader.GetAudioClip(false)));
                GM.Instance.logger.Info(this, "JUKEBOX: Done loading " + song.FullName);
            }
            else
            {
                GM.Instance.logger.Warn(this, "JUKEBOX: Skipped " + song.FullName + " // not an .ogg");
            }
        }
    }

    public JSONNode GetDefautLanguage()
    {
        TextAsset rawJson = Resources.Load<TextAsset>("winnitron_text_english");
        return JSONNode.Parse(rawJson.text);
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
			GM.Instance.logger.Error("Error loading JSON file: " + fileLocation);
			GM.Instance.Oops (GM.Instance.Text("error", "cannotFindJson"));
			return null;
		}
	}
}
