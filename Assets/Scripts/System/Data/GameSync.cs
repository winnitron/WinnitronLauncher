using UnityEngine;
using System.IO;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class GameSync : MonoBehaviour {

    private string api_key;
    private string library_url;
    private string games_dir;

    private ArrayList playlists = new ArrayList();

    private void initConfig() {
        // TODO read these from somewhere
        api_key = "";
        library_url = "http://network.winnitron.com";
        games_dir = "C:/Users/aaron/Desktop/winnitron_games/";
    }

    public void execute() {
        Debug.Log("----------------------------- Running sync -----------------------------");

        initConfig();

        fetchPlaylistSubscriptions();

        /*
         * delete local playlists that aren't in the subscription list
         * 
         * for each playlist
         * to_install = playlist.games
         * get list of local (already installed) games  
         * for each local
         *   if not in playlist
         *     delete
         *   else
         *     check metadata.json: if existing_install.installed_at >= game.last_modified
         *     remove from to_install
         * 
         * for each to_install
         *   mkdir (if it's not already there)
         *   download
         *   extract
         *   write metadata
         */

    }

    private void fetchPlaylistSubscriptions() {
        WWW www = new WWW(library_url + "/api/v1/playlists/?api_key=" + api_key);

        StartCoroutine(WaitForGET(www));
    }

    private IEnumerator WaitForGET(WWW www) {
        
        yield return www;

        if (www.error == null) {
            Debug.Log(www.text);
            var data = JSON.Parse(www.text);
            foreach (JSONNode playlist_data in data["playlists"].AsArray) {
                playlists.Add(new Playlist(playlist_data));
            }


            // debug/confirmation
            foreach (Playlist playlist in playlists) {
                Debug.Log(playlist.ToString());
            }
        } else {
            Debug.Log("Error fetching playlists: " + www.error);
        }
    }





    private class Playlist {
        public string title;
        public string slug;
        public ArrayList games = new ArrayList();

        public Playlist(JSONNode data) {
            title = data["title"];
            slug = data["slug"];

            foreach(JSONNode game_data in data["games"].AsArray) {
                games.Add(new Game(game_data));
            }
            
        }

        public override string ToString() {
            return "'" + title + "' (" + slug + ") " + games.Count + " games";
        }
    }

    private class Game {
        public string title;
        public string slug;
        public string description;
        public string downloadURL;
        public System.DateTime lastModified;

        public Game(JSONNode data) {
            title = data["title"];
            slug = data["slug"];
            description = data["description"];
            downloadURL = data["download_url"];
            lastModified = System.DateTime.Parse(data["last_modified"], null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
    }
}

