using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using ICSharpCode.SharpZipLib.Zip;
using System;

[System.Serializable]
public class WinnitronNetwork : MonoBehaviour {

    public delegate object Success(object results);

    public void startGame(string game) {
        if (game == null)
            return;

        GM.Instance.logger.Debug("sending START GAME: " + game);
        string apiKey = GM.Instance.options.GetSyncSettings()["apiKey"];

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " http://winnitron.com");

        WWWForm form = new WWWForm();
        form.AddField("api_key", apiKey);
        form.AddField("game", game);

        WWW www = new WWW(GM.Instance.options.GetSyncSettings()["libraryURL"] + "/api/v1/plays/start", form.data, headers);

        StartCoroutine(WaitForPlaySessionCreation(www));
    }

    public void stopGame(string game) {
        if (game == null)
            return;

        GM.Instance.logger.Debug("sending STOP GAME: " + game);
        // string apiKey = GM.Instance.options.GetSyncSettings()["apiKey"].ToString();

        // Dictionary<string, string> headers = new Dictionary<string, string>();
        // headers.Add("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " http://winnitron.com");
        // headers.Add("X-HTTP-Method-Override", "PUT");

        // WWWForm form = new WWWForm();
        // form.AddField("api_key", apiKey);

        // WWW www = new WWW(GM.Instance.options.GetSyncSettings()["libraryURL"] + "/api/v1/plays/" + game + "/stop", form.data, headers);

        //UnityWebRequest www = UnityWebRequest.Put("/plays/whatever/stop");
        // StartCoroutine(WaitForPlaySessionUpdate(www));
    }

    public void GetAttracts() {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/api/v1/attracts/");
        AddHeaders(www);
        StartCoroutine(Wait(www, WriteAttracts));
    }

    private UnityWebRequest AddHeaders(UnityWebRequest www) {
        www.SetRequestHeader("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " http://winnitron.com");
        www.SetRequestHeader("Authorization", "Token " + GM.Instance.options.GetSyncSettings()["apiKey"]);
        return www;
    }

    private IEnumerator Wait(UnityWebRequest www, Success success) {
        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            HandleError(www, "Network Error:");
        } else if (www.isHttpError) {
            HandleError(www, "HTTP Error:");
        } else {
            success(www.downloadHandler.text);
        }
    }

    protected void HandleError(UnityWebRequest www, string msgPrepend = null) {
        string[] components = {
            msgPrepend,
            (www.responseCode > 0 ? www.responseCode.ToString() : null),
            www.error,
            ParseErrors(www.downloadHandler.text)
        };

        string msg = "";
        foreach (string s in components) {
            if (string.IsNullOrEmpty(s))
                msg += (s + " ");
        }

        Debug.Log("ERROR RESPONSE:" + www.downloadHandler.text);
        GM.Instance.logger.Error(msg);
    }

    private string ParseErrors(string json) {
        return json.ToString(); // TODO
    }




    private object WriteAttracts(object responseJson) {
        string json = responseJson.ToString();

        string path = Path.Combine(GM.Instance.options.dataPath, GM.Instance.options.attractPath);
        string file = Path.Combine(path, "remote_attracts.json");
        GM.Instance.logger.Debug("Writing remote attracts: " + file + "\n" + responseJson);
        File.WriteAllText(file, JSONNode.Parse(json).ToString(2));
        return null;
    }


    private IEnumerator WaitForPlaySessionCreation(WWW www) {

        yield return www;

        if (www.error == null) {
            GM.Instance.logger.Debug("Created gameplay session: " + www.text);
            JSONNode response = JSON.Parse(www.text);

            if (response["errors"] == null) {
                GM.Instance.logger.Debug(response.ToString());
                yield return response["id"].AsInt;
            } else {
                GM.Instance.logger.Warn("Error creating gameplay session: " + www.text);
                yield return null;
            }

        } else {
            GM.Instance.logger.Warn("Error creating gameplay session: " + www.error);
        }
    }

    private IEnumerator WaitForPlaySessionUpdate(WWW www) {
        yield return www;

        if (www.error == null) {
            GM.Instance.logger.Debug("Updated gameplay session: " + www.text);
        } else {
            GM.Instance.logger.Warn("Error updating gameplay session: " + www.error);
        }
    }

}
