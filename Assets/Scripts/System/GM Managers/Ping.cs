using UnityEngine;
using System;

public class Ping : MonoBehaviour {
    private float lastPing = 0;
    private int pingFreq;
    private bool pingEnabled = false;

    void Update() {
        if (GM.options.O["launcher"]["pingFreq"] != null && pingFreq == 0) {
            pingEnabled = true;
            pingFreq = Math.Max(GM.options.O["launcher"]["pingFreq"].AsInt, 1);
            GM.logger.Debug("Enabling " + pingFreq + "s ping.");
        }

        if (pingEnabled && Time.time >= lastPing + pingFreq) {
            GM.logger.Debug("PING " + (int) Time.time);
            lastPing = Time.time;
        }
    }
}
