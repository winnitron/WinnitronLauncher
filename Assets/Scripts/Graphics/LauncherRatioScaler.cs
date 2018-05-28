using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherRatioScaler : MonoBehaviour {

    public float targetaspect;
    public bool letterbox;
    public List<Transform> transformsToAlsoScale;

    private float windowaspect;
    private float scaleAmnt;
    private float scaleApplyTime = 1;

    public Camera mainCam;
    public Camera movieCam;

    // Use this for initialization
    void Awake()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        //targetaspect = 16.0f / 9.0f == 1.777778f;

        // determine the game window's current aspect ratio
        windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        scaleAmnt = windowaspect / targetaspect;

        Rect rect;

        // if scaled height is less than current height, add letterbox
        if (scaleAmnt < 1.0f)
        {
            rect = mainCam.rect;

            rect.width = 1.0f;
            rect.height = scaleAmnt;
            rect.x = 0;
            rect.y = (1.0f - scaleAmnt) / 2.0f;
        }
        else // add pillarbox
        {
            scaleAmnt = 1.0f / scaleAmnt;

            rect = mainCam.rect;

            rect.width = scaleAmnt;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleAmnt) / 2.0f;
            rect.y = 0;
        }

        if (letterbox)
        {
            mainCam.rect = rect;
            movieCam.rect = rect;
        }

        GM.Instance.logger.Info("LauncherRatioScaler: scale set to " + scaleAmnt);
        transform.localScale = new Vector3(scaleAmnt, scaleAmnt, scaleAmnt);

        foreach(Transform t in transformsToAlsoScale)
        {
            t.localScale = new Vector3(scaleAmnt, scaleAmnt, scaleAmnt);
        }
    }
}
