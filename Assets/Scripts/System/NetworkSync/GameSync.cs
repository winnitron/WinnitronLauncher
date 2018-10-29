using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ICSharpCode.SharpZipLib.Zip;
using System;


namespace NetworkSync {

    /// <summary>
    /// This is responsible for Syncing the launcher with the Winnitron Network and handling
    /// all HTTP requests etc.
    /// </summary>
    [System.Serializable]
    public class GameSync : MonoBehaviour {

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
        private float timer = 0;
        private float timeout = 30;

        /// <summary>
        /// Called during GM's Init phase.
        /// </summary>
        public void Init() {
            isFinished = false;

            apiKey = GM.Instance.options.GetSyncSettings()["apiKey"];

            if (apiKey == "" || apiKey == null)
                GM.Instance.Oops(GM.Instance.options.GetText("error", "noAPIkey"));

            libraryURL = GM.Instance.options.GetSyncSettings()["libraryURL"];
            gamesDir = GM.Instance.options.playlistsPath;

            try {
                execute();
            } catch(Exception e) {
                GM.Instance.logger.Error(e.Message);
                GM.Instance.Oops(e.Message, true);
            }
        }

        /// <summary>
        /// This is where we check for scheduled updates.
        /// </summary>
        void Update() {
            if (timeToUpdate < DateTime.Now.TimeOfDay && syncType == SyncType.DAILY && SafeToSync())
                execute();
        }

        public void execute() {
            if (syncType != SyncType.NONE)
            {
                GM.Instance.logger.Info(this, "GameSync: Running Sync...");
                SyncText("Updating with Winnitron Network...");

                FetchPlaylistSubscriptions();
            } else {
                GM.Instance.logger.Info(this, "GameSync: Skipping Sync...");
                EndSync();
            }
        }

        /// <summary>
        /// Using the unique API_Key, it checks with the Winnitron Network to download and
        /// update all the games from the site.
        /// </summary>
        private void FetchPlaylistSubscriptions() {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " http://winnitron.com");
            WWW www = new WWW(libraryURL + "/api/v1/playlists/?api_key=" + apiKey, null, headers);

            StartCoroutine(WaitForSubscriptionList(www));
        }

        private IEnumerator WaitForSubscriptionList(WWW www) {

            bool timedOut = false;
            while(!www.isDone) {
                if(timer > timeout) {
                    timedOut = true;
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            if (timedOut) {
                GM.Instance.logger.Error("Could not connect to Winnitron Network. Request timed out after " + timeout + "s.");
                EndSync();
            }

            yield return www;




            if (www.error == null) {
                GM.Instance.logger.Info(this, "fetched playlists: " + www.text);
                var data = JSON.Parse(www.text);
                foreach (JSONNode playlistData in data["playlists"].AsArray) {
                    playlists.Add(new Playlist(playlistData, gamesDir));
                }

                // delete unsubscribed playlists
                SluggedItem.DeleteExtraDirectories(gamesDir, playlists);

                foreach(Playlist playlist in playlists) {
                    SyncText("Initializing games in " + playlist.title);

                    playlist.DeleteRemovedGames();


                    // Only download games that have had a new version uploaded
                    // since the last sync.
                    ArrayList gamesToDownload = playlist.GamesToDownload();
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
                        game.WriteMetadataFile();
                    }
                }

                SyncText("Collecting data for launcher...");

                EndSync();

            } else {
                GM.Instance.logger.Error("Error fetching playlists: " + www.error);
                EndSync();
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
            GM.Instance.InfoText("SYNCING WITH WINNITRON NETWORK", text);
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
    }
}