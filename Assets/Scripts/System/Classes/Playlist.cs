using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SimpleJSON;

[System.Serializable]
public class Playlist
{
    public string name;
    public string description;
    public DirectoryInfo directory;
    public List<Game> games;
    public int selectedIndex = 0;

    /*
     *  Playlist Constructor
     *
     *  Playlist only takes in the directory that the DataManager hands it
     *  and builds everything from there
     */
    public Playlist(string directory) {
        this.directory = new DirectoryInfo(directory);
        this.games = new List<Game>();

        SetName();
        BuildGameList();
        GM.Instance.logger.Info(null, "Playlist Built! : " + name);
    }

    public void SetName() {
        string file = Path.Combine(this.directory.FullName, "winnitron_metadata.json");
        if (System.IO.File.Exists(file)) {
            JSONNode data = GM.Instance.data.LoadJson(file);

            name = data["title"];
            description = data["description"];

            GM.Instance.logger.Debug("Setting playlist name from json: " + file);
        }

        // If we tried but the json was malformed or something.
        if (!System.IO.File.Exists(file) || name == null) {
            name = this.directory.Name;
            name = name.TrimStart('_');
            name = name.Replace('_', ' ').Replace('-', ' ');
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

            GM.Instance.logger.Warn("Playlist metadata file not found or invalid: " + file);
            GM.Instance.logger.Debug("Setting playlist name from directory: " + name);
        }
    }


    public void BuildGameList() {
        foreach(var gameDir in directory.GetDirectories()) {
            //Don't pick any directories that start with a dot
            if(!gameDir.Name.Substring(0, 1).Equals(".")) {
                Game newGame = new Game(gameDir.FullName);
                games.Add(newGame);
            }
        }
    }
}

