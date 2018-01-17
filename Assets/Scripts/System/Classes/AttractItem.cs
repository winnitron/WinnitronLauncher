using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttractItem {

    public enum AttractItemType { Video, Image, Text }
    public AttractItemType type;

    public string pathToItem;
    public WWW loadPath;

    public bool voidItem = false;

    /// <summary>
    /// Constructs an AttractItem to be used later.
    /// </summary>
    /// <param name="pathToItem">Path to file, must be absolute.</param>
    /// <returns>If the construction was successful.</returns>
    public AttractItem (string pathToItem)
    {
        if (!Uri.IsWellFormedUriString(pathToItem, UriKind.Absolute))
        {
            GM.Instance.logger.Info("AttractItem - String is not valid: " + pathToItem);
            voidItem = true;
        }

        else
        {

            if (pathToItem.Contains("mp4"))
            {
                type = AttractItemType.Video;
                LoadVideo();
            }
            else if (pathToItem.Contains("png") || pathToItem.Contains("jpg") || pathToItem.Contains("jpeg"))
            {
                type = AttractItemType.Image;
                LoadImage();
            }
            else if (pathToItem.Contains("txt"))
            {
                type = AttractItemType.Text;
                LoadText();
            }
            else
            {
                GM.Instance.logger.Info("AttractItem - No valid file found.  Voiding file " + pathToItem);
                voidItem = true;
            }
        }
    }

    private void LoadImage()
    {
        
    }

    private void LoadVideo()
    {

    }

    private void LoadText()
    {

    }

}
