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
    public bool enabled = false;

    public void RecordPlaySession(string game, System.DateTime startedAt) {
        string start = startedAt.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        string stop = DateTime.Now.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

        GM.Instance.logger.Debug("sending record play session for " + game + ": " + start + " - " + stop);

        string url = GM.Instance.options.GetSyncSettings()["libraryURL"] + "/api/v1/plays/";

        WWWForm fields = new WWWForm();
        fields.AddField("game", game);
        fields.AddField("start", start);
        fields.AddField("stop", stop);

        UnityWebRequest www = UnityWebRequest.Post(url, fields);
        AddHeaders(www);
        StartCoroutine(Wait(www));
    }

    public void GetAttracts() {
        GM.Instance.logger.Debug("sending GET ATTRACTS");

        UnityWebRequest www = UnityWebRequest.Get(GM.Instance.options.GetSyncSettings()["libraryURL"] + "/api/v1/attracts/");
        AddHeaders(www);
        StartCoroutine(Wait(www, WriteAttracts));
    }

    private UnityWebRequest AddHeaders(UnityWebRequest www) {
        www.SetRequestHeader("User-Agent", "Winnitron Launcher/" + GM.Instance.versionNumber + " http://winnitron.com");
        www.SetRequestHeader("Authorization", "Token " + GM.Instance.options.GetSyncSettings()["apiKey"]);
        return www;
    }

    private IEnumerator Wait(UnityWebRequest www, Success success = null) {
        if (!enabled)
            yield break;

        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            HandleError(www, "Network Error:");
        } else if (www.isHttpError) {
            HandleError(www, "HTTP Error:");
        } else {
            if (success == null)
                GM.Instance.logger.Debug("Network response: " + www.downloadHandler.text);
            else
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
            if (!string.IsNullOrEmpty(s))
                msg += (s + " ");
        }

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

}
