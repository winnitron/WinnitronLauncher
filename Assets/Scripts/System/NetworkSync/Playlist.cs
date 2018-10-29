using System.IO;
using System.Collections;
using SimpleJSON;

namespace NetworkSync {
    class Playlist : SluggedItem {
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
            data.Add("local", new JSONBool(false));

            string filename = Path.Combine(Path.Combine(parentDirectory, slug), "winnitron_metadata.json");
            File.WriteAllText(filename, data.ToString(2));
        }
    }
}