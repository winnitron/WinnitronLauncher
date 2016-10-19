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
        api_key = GM.options.GetSyncSettings()["api_key"];
        library_url = GM.options.GetSyncSettings()["library_url"];
        games_dir = GM.options.GetDirectory()["playlists"];
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
            Debug.Log("fetched playlists: " + www.text);
            var data = JSON.Parse(www.text);
            foreach (JSONNode playlist_data in data["playlists"].AsArray) {
                playlists.Add(new Playlist(playlist_data));
            }


            deleteUnsubscribedPlaylists();


        } else {
            Debug.Log("Error fetching playlists: " + www.error);
        }
    }

    private void deleteUnsubscribedPlaylists() {
        string[] installed = Directory.GetDirectories(games_dir);

        foreach (string dir_full_path in installed) {
            string directory = new DirectoryInfo(dir_full_path).Name;
            if (directoryIsDeletable(directory, playlists)) {
                Debug.Log("deleting " + directory);
                Directory.Delete(dir_full_path, true);
            }
        }
    }
        
    // Careful here that you don't pass in a full path as `directory`
    private bool directoryIsDeletable(string directory, ArrayList keepers) {
        // Playlist or Game directories that start with an underscore are local additions,
        // and won't be deleted just because they're not in the website data.
        // In the future it'd be better to have that be a setting in the options or metadata json or something.
        if (directory[0] == '_')
            return false;

        foreach (SluggedItem keeper in keepers) {
            if (directory == keeper.slug)
                return false;
        }

        return true;
    }



    private class SluggedItem {
        public string title;
        public string slug;
    }

    private class Playlist : SluggedItem {
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

    private class Game : SluggedItem {
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

