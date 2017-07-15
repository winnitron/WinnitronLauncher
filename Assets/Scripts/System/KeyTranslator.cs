using UnityEngine;
using System;
using System.IO;
using SimpleJSON;

public class KeyTranslator {
    private JSONArray translation;

    public KeyTranslator(string optionsPath) {
        string file = Path.Combine(optionsPath, "key_translation.json");
        Debug.Log("KEY TRANSLATION FILE: " + file);
        translation = GM.data.LoadJson(file)["keys"].AsArray;
    }


    public KeyCode fromAHK(string ahkStr) {
        string needle = findAHK(ahkStr)["unity"];

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
            if (key.ToString().Equals(needle)) {
                return key;
            }
        }

        return KeyCode.None;
    }

    public string toAHK(KeyCode keyCode) {
        return findUnity(keyCode)["ahk"];
    }

    private JSONNode findUnity(KeyCode keyCode) {
        foreach (JSONNode keyData in translation) {
            if ((string) keyData["unity"] == keyCode.ToString()) {
                return keyData;
            }
        }

        return null;
    }

    private JSONNode findAHK(string ahkStr) {

        foreach (JSONNode keyData in translation) {
            if (String.Equals(keyData["ahk"], ahkStr, StringComparison.OrdinalIgnoreCase)) {
                return keyData;
            }
        }

        return null;
    }
}
