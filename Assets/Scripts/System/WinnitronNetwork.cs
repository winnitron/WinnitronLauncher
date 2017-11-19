using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ICSharpCode.SharpZipLib.Zip;
using System;

[System.Serializable]
public class WinnitronNetwork : MonoBehaviour {

    public void startGame(string game) {
        GM.logger.Debug("sending START GAME: " + game);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("User-Agent", "Winnitron Launcher/" + GM.VersionNumber + " (http://winnitron.com)");

        WWWForm form = new WWWForm();
        form.AddField("api_key", GM.options.GetSyncSettings()["apiKey"]);
        form.AddField("game", game);

        WWW www = new WWW(GM.options.GetSyncSettings()["libraryURL"] + "/api/v1/plays/start", form.data, headers);

        StartCoroutine(WaitForPlaySessionCreation(www));
    }

    public void stopGame(string game) {
        GM.logger.Debug("sending STOP GAME: " + game);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("User-Agent", "Winnitron Launcher/" + GM.VersionNumber + " (http://winnitron.com)");
        headers.Add("X-HTTP-Method-Override", "PUT");

        WWWForm form = new WWWForm();
        form.AddField("api_key", GM.options.GetSyncSettings()["apiKey"]);

        WWW www = new WWW(GM.options.GetSyncSettings()["libraryURL"] + "/api/v1/plays/" + game + "/stop", form.data, headers);

        StartCoroutine(WaitForPlaySessionUpdate(www));
    }

    private IEnumerator WaitForPlaySessionCreation(WWW www) {

        yield return www;

        if (www.error == null) {
            GM.logger.Debug("Created gameplay session: " + www.text);
            JSONNode response = JSON.Parse(www.text);

            if (response["errors"] == null) {
                GM.logger.Debug(response.ToString());
                yield return response["id"].AsInt;
            } else {
                GM.logger.Warn("Error creating gameplay session: " + www.text);
                yield return null;
            }

        } else {
            GM.logger.Warn("Error creating gameplay session: " + www.error);
        }
    }

    private IEnumerator WaitForPlaySessionUpdate(WWW www) {
        yield return www;

        if (www.error == null) {
            GM.logger.Debug("Updated gameplay session: " + www.text);
        } else {
            GM.logger.Warn("Error updating gameplay session: " + www.error);
        }
    }

}
