// https://github.com/CharmedMatter/UnityEngine_Log_to_Loggly

using UnityEngine;
using System.Collections;

public class LogOutputHandler : MonoBehaviour {

    // Create a string to store log level in
    string level = "";

    // Register the HandleLog function on scene start to fire on debug.log events
    public void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    // Remove callback when object goes out of scope
    public void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    // Capture debug.log output, send logs to Loggly
    public void HandleLog(string logString, string stackTrace, LogType type) {

        // Initialize WWWForm and store log level as a string
        level = type.ToString();
        var loggingForm = new WWWForm();

        // Add log message to WWWForm
        loggingForm.AddField("LEVEL", level);
        loggingForm.AddField("Message", logString);
        loggingForm.AddField("Stack_Trace", stackTrace);
        loggingForm.AddField("API_Key", GM.options.GetSyncSettings()["apiKey"]);

        // Add any User, Game, or Device MetaData that would be useful to finding issues later
        loggingForm.AddField("Device_Model", SystemInfo.deviceModel);
        StartCoroutine(SendData(loggingForm));
  }

    public IEnumerator SendData(WWWForm form) {
        if (GM.options.logger != null) {
            string token = GM.options.logger["token"];
            string url = "http://logs-01.loggly.com/inputs/" + token;

            // Send WWW Form to Loggly, replace TOKEN with your unique ID from Loggly
            WWW sendLog = new WWW(url, form);
            yield return sendLog;
        }
    }
}