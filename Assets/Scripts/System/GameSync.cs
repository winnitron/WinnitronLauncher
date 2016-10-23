using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
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
    }

    private void fetchPlaylistSubscriptions() {
        WWW www = new WWW(library_url + "/api/v1/playlists/?api_key=" + api_key);

        StartCoroutine(WaitForSubscriptionList(www));
    }

    private IEnumerator WaitForSubscriptionList(WWW www) {

        yield return www;

        if (www.error == null) {
            Debug.Log("fetched playlists: " + www.text);
            var data = JSON.Parse(www.text);
            foreach (JSONNode playlist_data in data["playlists"].AsArray) {
                playlists.Add(new Playlist(playlist_data, games_dir));
            }

            // delete unsubscribed playlists
            SluggedItem.deleteExtraDirectories(games_dir, playlists);

            foreach(Playlist playlist in playlists) {
                Debug.Log("creating " + playlist.parentDirectory);

                Directory.CreateDirectory(playlist.parentDirectory);
                playlist.deleteRemovedGames();
                ArrayList games = playlist.gamesToInstall();

                foreach (Game game in games) {
                    Debug.Log("Downloading: " + game.title);
                    WWW download = new WWW(game.downloadURL);
                    StartCoroutine(WaitForGameDownload(game, download));
                }
            }

        } else {
            Debug.Log("Error fetching playlists: " + www.error);
        }
    }


    private IEnumerator WaitForGameDownload(Game game, WWW download) {
        yield return download;

        if (download.error == null) {
            Debug.Log("success downloading '" + game.title + "'");
            Directory.CreateDirectory(game.installDirectory);

            string destFile = game.installDirectory + "/" + game.slug + ".zip";
            File.WriteAllBytes(destFile, download.bytes);

            ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
            zip.ExtractZip(destFile, game.installDirectory, null);

            game.writeMetadataFile();
            File.Delete(destFile);

        } else {
            Debug.Log("Error downloading '" + download.url + "': " + download.error);
        }
    }


    private class SluggedItem {
        public string title;
        public string slug;


        public static void deleteExtraDirectories(string parent, ArrayList keepers) {
            if (!Directory.Exists(parent)) // just in case. avoid crash.
                return;

            string[] installed = Directory.GetDirectories(parent);

            foreach (string dir_full_path in installed) {
                string directory = new DirectoryInfo(dir_full_path).Name;

                if (SluggedItem.directoryIsDeletable(directory, keepers)) {
                    Debug.Log("deleting " + dir_full_path);
                    Directory.Delete(dir_full_path, true);
                }
            }
        }

        // Careful here that you don't pass in a full path as `directory`
        private static bool directoryIsDeletable(string directory, ArrayList keepers) {
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
    }

    private class Playlist : SluggedItem {
        public ArrayList games = new ArrayList();
        public string parentDirectory;

        public Playlist(JSONNode data, string parentDir) {
            title = data["title"];
            slug = data["slug"];
            parentDirectory = parentDir;

            foreach(JSONNode game_data in data["games"].AsArray) {
                games.Add(new Game(game_data, parentDirectory + "/" + slug + "/"));
            }
        }

        public ArrayList gamesToInstall() {
            Debug.Log("Syncing games for playlist '" + title + "'");


            ArrayList toInstall = new ArrayList();
            foreach (Game game in games) {
                System.DateTime installModified = new System.DateTime(1982, 2, 2);

                if (game.alreadyInstalled()) {
                    installModified = System.DateTime.Parse(game.installationMetadata ["last_modified"], null, System.Globalization.DateTimeStyles.RoundtripKind);
                }

                if (!game.alreadyInstalled() || game.lastModified > installModified) {
                    toInstall.Add(game);
                }
            }

            return toInstall;
        }

        public void deleteRemovedGames() {
            SluggedItem.deleteExtraDirectories(parentDirectory + "/" + slug, games);
        }




    }

    private class Game : SluggedItem {
        public string description;
        public string downloadURL;
        public System.DateTime lastModified;
        public string installDirectory;

        public JSONNode installationMetadata;

        public Game(JSONNode data, string parentDir) {
            title = data["title"];
            slug = data["slug"];
            description = data["description"];
            downloadURL = data["download_url"];
            lastModified = System.DateTime.Parse(data["last_modified"], null, System.Globalization.DateTimeStyles.RoundtripKind);
            installDirectory = parentDir + slug + "/";

            if (alreadyInstalled()) {
                string json = System.IO.File.ReadAllText(installDirectory + "winnitron_metadata.json");
                installationMetadata = JSON.Parse(json);
            } else {
                installationMetadata = new JSONClass();
            }
        }


        public bool alreadyInstalled() {
            Debug.Log(installDirectory);
            return Directory.Exists(installDirectory);
        }

        public void writeMetadataFile() {
            
            installationMetadata.Add("title", new JSONData(title));
            installationMetadata.Add("slug", new JSONData(slug));
            //installationMetadata.Add("description", new JSONData(description)); // TODO buggy for some reason? Blank?
            installationMetadata.Add("last_modified", new JSONData(System.DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
            // TODO min/max player counts (needs to happen on the server side first)

            string filename = installDirectory + "/winnitron_metadata.json";
            Debug.Log("writing to " + filename + ": " + installationMetadata.ToString());
            System.IO.File.WriteAllText(filename, installationMetadata.ToString());
        }

    }
}

