using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

[System.Serializable]
public class AttractItem {

    public enum AttractItemType { None, Video, Image, Text }
    public AttractItemType type;

    public Sprite sprite;
    public string text;

    public string pathToItem;
    public float displayTime = 10;

    /// <summary>
    /// Constructs an AttractItem to be used later.
    /// </summary>
    /// <param name="pathToItem">Path to file, must be absolute.</param>
    /// <returns>If the construction was successful.</returns>
    public AttractItem(string filePath)
    {
        pathToItem = filePath;

        string ext = Path.GetExtension(filePath);

        if (ext == ".mp4")
        {
            type = AttractItemType.Video;
            // Video player only requires a path to the video, so we don't need to load anything new here
        }
        else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
        {
            type = AttractItemType.Image;
            LoadImage();
        }
        else if (ext == ".txt")
        {
            type = AttractItemType.Text;
            LoadText();
        }
        else
        {
            GM.Instance.logger.Debug("AttractItem - No valid file found.  Voiding file " + pathToItem);
            type = AttractItemType.None;
        }
    }

    public AttractItem(JSONNode data)
    {
        System.DateTime startsAt = System.DateTime.Parse(data["starts_at"], null, System.Globalization.DateTimeStyles.RoundtripKind);
        System.DateTime endsAt = DateTime.Now.AddDays(365); // we don't need roads

        if (data["ends_at"] != null)
            endsAt = System.DateTime.Parse(data["ends_at"], null, System.Globalization.DateTimeStyles.RoundtripKind);

        bool started = startsAt <= System.DateTime.UtcNow;
        bool ended = endsAt <= System.DateTime.UtcNow;

        if (started && !ended)
        {
            type = AttractItemType.Text;
            text = data["text"];
        } else {
            type = AttractItemType.None;
        }
    }

    private void LoadImage()
    {
        // Load the screenshot from the games directory as a Texture2D
        var screenshotTex = new Texture2D(1920, 1080);

        if (pathToItem != null)
        {
            GM.Instance.logger.Debug("GAME: Loading custom screenshot " + pathToItem);
            screenshotTex.LoadImage(File.ReadAllBytes(pathToItem));

            // Turn the Texture2D into a sprite
            sprite = Sprite.Create(screenshotTex, new Rect(0, 0, screenshotTex.width, screenshotTex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            type = AttractItemType.None;
        }
    }

    private void LoadText()
    {
        if (pathToItem != null)
        {
            //Read the .txt and save the string
            StreamReader reader = new StreamReader(pathToItem);
            text = reader.ReadToEnd();
        }
        else
        {
            type = AttractItemType.None;
        }
    }

}
