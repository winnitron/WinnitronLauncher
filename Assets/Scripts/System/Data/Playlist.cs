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
        GM.logger.Info(null, "Playlist Built! : " + name);
    }

    public void SetName() {
        string file = Path.Combine(this.directory.FullName, "winnitron_metadata.json");
        if(System.IO.File.Exists(file)) {
            string json = File.ReadAllText(file);
            JSONNode data = JSONNode.Parse(json);
            name = data["title"];
            description = data["description"];

            GM.logger.Debug("Setting playlist name from json: " + file);
        } else {
            name = this.directory.Name;
            name = name.TrimStart('_');
            name = name.Replace('_', ' ').Replace('-', ' ');
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

            GM.logger.Debug("Playlist metadata file not found: " + file);
            GM.logger.Debug("Setting playlist name from directory: " + name);
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

