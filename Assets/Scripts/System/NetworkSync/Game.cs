using System.IO;
using SimpleJSON;

namespace NetworkSync {
    class Game : SluggedItem {
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

            if (AlreadyInstalled()) {
                string file = Path.Combine(installDirectory, "winnitron_metadata.json");
                string json = File.ReadAllText(file);
                installationMetadata = JSON.Parse(json);
            } else {
                installationMetadata = new JSONObject();
            }
        }


        public bool AlreadyInstalled() {
            return File.Exists(Path.Combine(installDirectory, "winnitron_metadata.json"));
        }

        public void WriteMetadataFile() {

            installationMetadata.Add("title", new JSONString(title));
            installationMetadata.Add("slug", new JSONString(slug));
            //installationMetadata.Add("description", new JSONData(description)); // TODO buggy for some reason? Blank?
            installationMetadata.Add("last_modified", new JSONString(System.DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
            installationMetadata.Add("min_players", new JSONNumber(minPlayers));
            installationMetadata.Add("max_players", new JSONNumber(maxPlayers));
            installationMetadata.Add("executable", new JSONString(executable));
            installationMetadata.Add("image_url", new JSONString(imageURL));

            JSONObject keymap = new JSONObject();
            keymap.Add("template", new JSONString(keyTemplate));
            keymap.Add("bindings", keyBindings);
            installationMetadata.Add("keys", keymap);

            string filename = Path.Combine(installDirectory, "winnitron_metadata.json");
            File.WriteAllText(filename, installationMetadata.ToString(2));
        }
    }
}