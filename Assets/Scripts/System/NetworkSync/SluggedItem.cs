using System.IO;
using System.Collections;
using SimpleJSON;

namespace NetworkSync {
    class SluggedItem {
        public string title;
        public string slug;


        public static void DeleteExtraDirectories(string parent, ArrayList keepers) {
            if (!Directory.Exists(parent)) // just in case. avoid crash.
                return;

            string[] installed = Directory.GetDirectories(parent);

            foreach (string dirFullPath in installed) {
                if (SluggedItem.DirectoryIsDeletable(dirFullPath, keepers)) {
                    GM.Instance.logger.Info("deleting " + dirFullPath);
                    Directory.Delete(dirFullPath, true);
                }
            }
        }

        private static bool DirectoryIsDeletable(string dirFullPath, ArrayList keepers) {
            string name = new DirectoryInfo(dirFullPath).Name;
            JSONNode json = ReadMetadata(dirFullPath);

            // Playlist or Game directories that start with an underscore or are specified
            // "local": true in the metadatajson are local additions, and they won't be
            // deleted just because they're not in the website data.
            // Treat a pre-v2.3.0 local playlist dir the same always.
            if (name[0] == '_' || (json != null && json["local"].AsBool))
                return false;

            foreach (SluggedItem keeper in keepers) {
                if (name == keeper.slug)
                    return false;
            }

            return true;
        }

        private static JSONNode ReadMetadata(string directory) {
            string file = Path.Combine(directory, "winnitron_metadata.json");

            try {
                return JSON.Parse(File.ReadAllText(file));
            } catch (FileNotFoundException ) {
                return null;
            }
        }
    }
}