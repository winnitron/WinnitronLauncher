using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Logger: MonoBehaviour {

    public enum LogLevels { Debug, Info, Warn, Error }

    //Just here to expose it in the inspector
    public LogLevels initialLogLevel = LogLevels.Debug;
    public static LogLevels logLevel;

    public bool globalOn = false;
    public bool showNull = false;
    public bool showTimestamps = true;

    public List<UnityEngine.Object> supress;
    public List<UnityEngine.Object> show;

    private void Start()
    {
        logLevel = initialLogLevel;
    }

    public void Log(string msg, LogLevels msgLevel)
    {
        if ((int) msgLevel < (int) logLevel)
            return;

        if (showTimestamps) {
            string timestamp = DateTime.Now.ToLocalTime().ToString();
            msg = timestamp + " -- " + msg;
        }

        msg = msgLevel.ToString().ToUpper() + " -- " + msg;
        UnityEngine.Debug.Log(msg);
    }

    public void Log(UnityEngine.Object mb, string msg, LogLevels msgLevel)
    {
        if (CanShow(mb))
            Log(msg, msgLevel);
    }

    public void On() { globalOn = true; }
    public void Off() { globalOn = false; }


    public void Debug(string msg) {
        Log(msg, LogLevels.Debug);
    }

    public void Debug(UnityEngine.Object mb, string msg) {
        Log(mb, msg, LogLevels.Debug);
    }

    public void Info(string msg) {
        Log(msg, LogLevels.Info);
    }

    public void Info(UnityEngine.Object mb, string msg) {
        Log(mb, msg, LogLevels.Info);
    }

    public void Warn(string msg) {
        Log(msg, LogLevels.Warn);
    }

    public void Warn(UnityEngine.Object mb, string msg) {
        Log(mb, msg, LogLevels.Warn);
    }

    public void Error(string msg) {
        Log(msg, LogLevels.Error);
    }

    public void Error(UnityEngine.Object mb, string msg) {
        Log(mb, msg, LogLevels.Error);
    }

    private bool CanShow(UnityEngine.Object mb)
    {
        bool result = false;

        if (globalOn) result = true;
        if (mb == null && showNull) result = true;

        foreach (UnityEngine.Object m in show)
            if (m == mb) result = true;

        foreach (UnityEngine.Object m in supress)
            if (m == mb) result = false;

        return result;
    }
}

