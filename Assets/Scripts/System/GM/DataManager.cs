using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.Video;

/// <summary>
/// This class holds all the necessary user data.
/// </summary>
public class DataManager : MonoBehaviour {

    public List<Playlist> playlists;
    public List<Song> songs;

    public string introVideo;
    public string launcherBackground;
    public List<string> attractFiles;

    /// <summary>
    /// Called by the GM to get the initial round of data.
    /// </summary>
    public void Init()
    {
        GM.Instance.logger.Debug("DATA: Starting Init...");
        ReloadData();
    }

    /// <summary>
    /// Rebuilds all the data.  This is required for when the launcher first starts up,
    /// as well as after every sync with the Winnitron Network.
    /// </summary>
    public void ReloadData()
    {
        //Start with new lists
        playlists = new List<Playlist>();
        songs = new List<Song>();

        //Load everything!
        GetPlaylists();
        GetMusic();
        GetVideos();

        //Call the delegate and all methods hooked in
        //Primarily used in LauncherUIController.cs to update the data model
        //But could be used elsewhere
        GM.Instance.logger.Info(this, "DataManager: finished updating data.");
    }

    public void GetVideos()
    {
        var attractDir = new DirectoryInfo(GM.Instance.options.attractPath).GetFiles();

        foreach(var file in attractDir)
        {
            string ext = Path.GetExtension(file.FullName);
            if (ext == ".mp4")
            {
                GM.Instance.logger.Debug("DATA: Getting attract file " + file.Name + " with extension: " + ext);

                if (file.Name.ToLower().Contains("intro"))
                    introVideo = file.FullName;
                else if (file.Name.ToLower().Contains("background"))
                    launcherBackground = file.FullName;
                else
                    attractFiles.Add(file.FullName);
            }

            else
            {
                GM.Instance.logger.Warn("DATA: Attract file " + file.Name + " is not a supported type.");
            }
        }

        if (introVideo == null || introVideo == "")
            GM.Instance.logger.Warn("DATA: No intro video found. (WINNITRON_UserData/Attract/intro.mp4)");
    }

    /// <summary>
    /// Builds a list of Game objects based on the game directory inside its main directory. 
    /// Then instantiates the GameNavigationManager, which then instantiates the ScreenShotDisplayManager
    /// </summary>
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

    /// <summary>
    /// Builds a list of all the user added music and stores references in the 
    /// music array to be accessed by the Jukebox.
    /// </summary>
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

    /// <summary>
    /// Finds the language JSON as required by the options JSON.
    /// </summary>
    /// <returns>A JSONNode object.</returns>
    public JSONNode GetDefautLanguage()
    {
        TextAsset rawJson = Resources.Load<TextAsset>("winnitron_text_english");
        return JSONNode.Parse(rawJson.text);
    }

    /// <summary>
    /// Loads a JSON from disk and parses it into a Unity readable JSONNode.
    /// </summary>
    /// <param name="fileLocation">Where on the disk the JSON is located.</param>
    /// <returns>A JSONNode parsing of the file.</returns>
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
