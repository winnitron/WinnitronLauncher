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
        return (KeyCode) findAHK(ahkStr)["unity"].AsInt;
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
