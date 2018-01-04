using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ICSharpCode.SharpZipLib.Zip;
using System;

[System.Serializable]
public class GameSync : MonoBehaviour {

    public Text syncDescription;

    private string apiKey;
    private string libraryURL;
    private string gamesDir;

    private ArrayList playlists = new ArrayList();

    /// <summary>
    /// NONE: Never Sync
    /// DAILY: Sync and use the daily sync variables below
    /// ALWAYS: Syncs after every game
    /// </summary>
    public enum SyncType { NONE, DAILY, ALWAYS };
    public SyncType syncType = SyncType.ALWAYS;

    //Sync Time stamps
    public DateTime lastUpdate;

    //Daily sync time
    public TimeSpan timeToUpdate;

    public bool syncOnStartup = true;
    public bool isFinished = true;

    //Called during GM init
    public void Init()
    {
        isFinished = false;

        apiKey = GM.Instance.options.GetSyncSettings()["apiKey"];

        if (apiKey == "" || apiKey == null)
            GM.Instance.Oops(GM.Instance.options.GetText("error", "noAPIkey"));

        libraryURL = GM.Instance.options.GetSyncSettings()["libraryURL"];
        gamesDir = GM.Instance.options.playlistsPath;

        execute();
    }

    void Update()
    {
        if (timeToUpdate < DateTime.Now.TimeOfDay && syncType == SyncType.DAILY && SafeToSync())
            execute();
    }

    /// <summary>
    /// Initiates Sync, should only be called by StateManager.cs
    /// </summary>
    public void execute()
    {
        if (syncType != SyncType.NONE)
        {
            GM.Instance.logger.Info(this, "GameSync: Running Sync...");
            SyncText("INITIALIZING SYNC!");

            fetchPlaylistSubscriptions();
        }
        else
        {
            GM.Instance.logger.Info(this, "GameSync: Skipping Sync...");
            EndSync();
        }
    }

    private void fetchPlaylistSubscriptions()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " (http://winnitron.com)");
        WWW www = new WWW(libraryURL + "/api/v1/playlists/?api_key=" + apiKey, null, headers);

        StartCoroutine(WaitForSubscriptionList(www));
    }

    private IEnumerator WaitForSubscriptionList(WWW www) {

        yield return www;

        if (www.error == null) {
            GM.Instance.logger.Info(this, "fetched playlists: " + www.text);
            var data = JSON.Parse(www.text);
            foreach (JSONNode playlistData in data["playlists"].AsArray) {
                playlists.Add(new Playlist(playlistData, gamesDir));
            }

            // delete unsubscribed playlists
            SluggedItem.deleteExtraDirectories(gamesDir, playlists);

            foreach(Playlist playlist in playlists) {
                SyncText("Initializing games in " + playlist.title);

                playlist.deleteRemovedGames();


                // Only download games that have had a new version uploaded
                // since the last sync.
                ArrayList gamesToDownload = playlist.gamesToDownload();
                foreach (Game game in gamesToDownload) {
                    GM.Instance.logger.Info(this, "Downloading: " + game.title);

                    //Start the downloadin'!

                    WWW download = new WWW(game.downloadURL);

                    while (!download.isDone)
                    {
                        int progress = Mathf.FloorToInt(download.progress * 100);
                        SyncText("Downloading " + game.title + " %" + progress);
                        yield return null;
                    }


                    //Download complete!

                    if (!string.IsNullOrEmpty(download.error))
                    {
                        // error!
                        GM.Instance.logger.Error("Error downloading '" + download.url + "': " + download.error);
                        yield return SyncText("Error downloading " + game.title + "!", 3);
                    }
                    else
                    {
                        //Things downloaded fine!  Do fun stuff now pls thx.

                        yield return SyncText("Unzipping " + game.title + "...", 0.1f);

                        Directory.CreateDirectory(game.installDirectory);
                        string zipFile = Path.Combine(game.installDirectory, game.slug + ".zip");
                        File.WriteAllBytes(zipFile, download.bytes);

                        FastZip zip = new FastZip();
                        ZipConstants.DefaultCodePage = 0;

                        //Starts the unzip coroutine and waits till it's done
                        zip.ExtractZip(zipFile, game.installDirectory, null);

                        // Download the image
                        WWW imageDownload = new WWW(game.imageURL);
                        while (!imageDownload.isDone) {
                            SyncText("Downloading cover image");
                            yield return null;
                        }

                        System.Uri uri = new System.Uri(game.imageURL);
                        string imageFilename = Path.GetFileName(uri.AbsolutePath);
                        File.WriteAllBytes(Path.Combine(game.installDirectory, imageFilename), imageDownload.bytes);

                        File.Delete(zipFile);
                    }
                }

                // Re-write metadata for all games in case that info changes
                // even without a new file uploaded.
                foreach (Game game in playlist.games) {
                    game.writeMetadataFile();
                }
            }

            SyncText("Collecting data for launcher...");

            EndSync();

        } else {
            GM.Instance.logger.Error("Error fetching playlists: " + www.error);
            GM.Instance.Oops(GM.Instance.options.GetText("error", "fetchPlaylistError"));
        }
    }


    /// <summary>
    /// Called when the Sync successfully ends.
    /// </summary>
    private void EndSync()
    {
        lastUpdate = DateTime.Now;
        GM.Instance.logger.Info(this, "GameSync: Sync complete at " + lastUpdate.ToString());

        isFinished = true;
    }

    /// <summary>
    /// Sets what the text during the Syncing process will say.
    /// </summary>
    /// <param name="text">Text to be displayed.</param>
    private void SyncText(string text)
    {
        syncDescription.text = text;
    }

    private IEnumerator SyncText(string text, float timeToWait)
    {
        SyncText(text);
        yield return new WaitForSeconds(timeToWait);
    }

    /// <summary>
    /// Checks to make sure we're not in a state where the launcher is being actively used like while playing a game.
    /// </summary>
    /// <returns>True if safe to sync, false if not.</returns>
    private bool SafeToSync()
    {
        return GM.Instance.state.GetCurrentAnimatorStateInfo(0).IsName("Launcher");
    }




    ///
    /// DATA STRUCTURES FOR SYNCING
    ///

    private class SluggedItem {
        public string title;
        public string slug;


        public static void deleteExtraDirectories(string parent, ArrayList keepers) {
            if (!Directory.Exists(parent)) // just in case. avoid crash.
                return;

            string[] installed = Directory.GetDirectories(parent);

            foreach (string dirFullPath in installed) {
                if (SluggedItem.directoryIsDeletable(dirFullPath, keepers)) {
                    GM.Instance.logger.Info("deleting " + dirFullPath);
                    Directory.Delete(dirFullPath, true);
                }
            }
        }

        private static bool directoryIsDeletable(string dirFullPath, ArrayList keepers) {
            string name = new DirectoryInfo(dirFullPath).Name;
            JSONNode json = readMetadata(dirFullPath);

            // Playlist or Game directories that start with an underscore or are specified
            // "local": true in the metadatajson are local additions, and they won't be
            // deleted just because they're not in the website data.
            if (name[0] == '_' || json["local"].AsBool)
                return false;

            foreach (SluggedItem keeper in keepers) {
                if (name == keeper.slug)
                    return false;
            }

            return true;
        }

        private static JSONNode readMetadata(string directory) {
            string file = Path.Combine(directory, "winnitron_metadata.json");
            return JSONNode.Parse(File.ReadAllText(file));
        }
    }

    private class Playlist : SluggedItem {
        public ArrayList games = new ArrayList();
        public string parentDirectory;
        public string description;

        public Playlist(JSONNode data, string parentDir) {
            title = data["title"];
            slug = data["slug"];
            description = data["description"];
            parentDirectory = parentDir;

            writeMetadataFile(data);

            foreach(JSONNode gameData in data["games"].AsArray) {
                games.Add(new Game(gameData, Path.Combine(parentDirectory, slug)));
            }
        }

        public ArrayList gamesToDownload() {
            GM.Instance.logger.Info("Syncing games for playlist '" + title + "'");


            ArrayList toInstall = new ArrayList();
            foreach (Game game in games) {
                System.DateTime installModified = new System.DateTime(1982, 2, 2);

                if (game.alreadyInstalled()) {
                    installModified = System.DateTime.Parse(game.installationMetadata["last_modified"], null, System.Globalization.DateTimeStyles.RoundtripKind);
                }

                if (!game.alreadyInstalled() || game.lastModified > installModified) {
                    GM.Instance.logger.Info("Gonna install " + game.title);
                    toInstall.Add(game);
                }
            }

            return toInstall;
        }

        public void deleteRemovedGames() {
            SluggedItem.deleteExtraDirectories(Path.Combine(parentDirectory, slug), games);
        }

        private void writeMetadataFile(JSONNode data) {
            Directory.CreateDirectory(Path.Combine(parentDirectory, slug));
            data.Add("local", new JSONData(false));

            string filename = Path.Combine(Path.Combine(parentDirectory, slug), "winnitron_metadata.json");
            System.IO.File.WriteAllText(filename, data.ToString());
        }
    }

    private class Game : SluggedItem {
        public string description;
        public string downloadURL;
        public System.DateTime lastModified;
        public string installDirectory;
        public string imageURL;
        public int minPlayers;
        public int maxPlayers;
        public string executable;
        public string keyTemplate;
        public JSONNode keyBindings;

        public JSONNode installationMetadata;

        public Game(JSONNode data, string parentDir) {
            title = data["title"];
            slug = data["slug"];
            description = data["description"];
            downloadURL = data["download_url"];
            lastModified = System.DateTime.Parse(data["last_modified"], null, System.Globalization.DateTimeStyles.RoundtripKind);
            installDirectory = Path.Combine(parentDir, slug);
            minPlayers = data["min_players"].AsInt;
            maxPlayers = data["max_players"].AsInt;
            executable = data["executable"];
            imageURL = data["image_url"];
            keyTemplate = data["keys"]["template"];
            keyBindings = data["keys"]["bindings"];

            if (alreadyInstalled()) {
                string file = Path.Combine(installDirectory, "winnitron_metadata.json");
                string json = File.ReadAllText(file);
                installationMetadata = JSON.Parse(json);
            } else {
                installationMetadata = new JSONClass();
            }
        }


        public bool alreadyInstalled() {
            return File.Exists(Path.Combine(installDirectory, "winnitron_metadata.json"));
        }

        public void writeMetadataFile() {

            installationMetadata.Add("title", new JSONData(title));
            installationMetadata.Add("slug", new JSONData(slug));
            //installationMetadata.Add("description", new JSONData(description)); // TODO buggy for some reason? Blank?
            installationMetadata.Add("last_modified", new JSONData(System.DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
            installationMetadata.Add("min_players", new JSONData(minPlayers));
            installationMetadata.Add("max_players", new JSONData(maxPlayers));
            installationMetadata.Add("executable", new JSONData(executable));
            installationMetadata.Add("image_url", new JSONData(imageURL));

            JSONClass keymap = new JSONClass();
            keymap.Add("template", new JSONData(keyTemplate));
            keymap.Add("bindings", keyBindings);
            installationMetadata.Add("keys", keymap);

            string filename = Path.Combine(installDirectory, "winnitron_metadata.json");
            System.IO.File.WriteAllText(filename, installationMetadata.ToString());
        }
    }
}

