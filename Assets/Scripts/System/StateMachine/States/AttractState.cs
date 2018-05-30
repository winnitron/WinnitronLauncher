using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttractState : State {

    public int numberOfItems;
    public int currentItem = 0;

    public float displayTime;

    private Vector3 imageStartPos;
    private Vector3 textStartPos;

    //STATE BASE FUNCTIONS

    override public void OnStateEnter(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        //Call the inherited functions to get the helper variable
        base.OnStateEnter(animator, info, layerIndex);

        helper.attract.SetActive(true);

        numberOfItems = GM.Instance.data.attractItems.Count;

        if (numberOfItems <= 0)
        {
            GM.Instance.logger.Warn("No valid files found in Attract folder (" + GM.Instance.options.attractPath + ")");
            animator.SetTrigger("NextState");
        }
        else
        {
            ShowCurrentItem();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        helper.attract.SetActive(false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo info, int layerIndex)
    {
        if (Input.anyKeyDown)
            animator.SetTrigger("NextState");

        displayTime -= Time.deltaTime;

        //Do stuff when the delay is over
        if (displayTime < 0)
        {
            //When playing video, only switch if video is done
            if (GetCurrentItem().type == AttractItem.AttractItemType.Video)
            {
                if (!GM.Instance.video.player.isPlaying)
                    ShowNextItem();
            }
            else
            {
                // On text or image, just go to the next one
                ShowNextItem();
            }
        }
    }


    //STATE FUNCTIONS

    /// <summary>
    /// This shows the current attract item
    /// </summary>
    private void ShowCurrentItem()
    {
        if (GetCurrentItem() != null)
        {
            switch (GetCurrentItem().type)
            {
                case AttractItem.AttractItemType.Image:
                    DisplayImage();
                    return;

                case AttractItem.AttractItemType.Text:
                    DisplayText();
                    return;

                case AttractItem.AttractItemType.Video:
                    DisplayVideo();
                    return;
            }
        }
        else
        {
            GM.Instance.logger.Debug("Attract: Item Number does not exist.");
        }
    }

    private void ShowNextItem()
    {
        //Increment and loop back if over
        currentItem = (currentItem + 1) % numberOfItems;

        ShowCurrentItem();
    }

    private void DisplayImage()
    {
        //Turn off unused gameobjects
        helper.attractText.gameObject.SetActive(false);

        //Play dat background vid
        GM.Instance.video.PlayBackground();

        //Display the image
        if (imageStartPos == null) {
            imageStartPos = helper.attractImage.transform.position;
        }

        helper.attractImage.transform.position = imageStartPos;
        helper.attractImage.gameObject.SetActive(true);
        helper.attractImage.sprite = GetCurrentItem().sprite;
        displayTime = GetCurrentItem().displayTime;
    }

    private void DisplayVideo()
    {
        //Turn off unused GameObjects
        helper.attractImage.gameObject.SetActive(false);
        helper.attractText.gameObject.SetActive(false);

        //Play the video!
        GM.Instance.video.PlayVideo(GetCurrentItem().pathToItem, false, true);
        displayTime = 1; //just need a second to get the video player running.
    }

    private void DisplayText()
    {
        //Turn off unused gameObjects
        helper.attractImage.gameObject.SetActive(false);

        //Get the background goin'
        GM.Instance.video.PlayBackground();

        if (textStartPos == null) {
            textStartPos = helper.attractText.transform.position;
        }

        //Play background video, and display text
        helper.attractText.transform.position = textStartPos;
        helper.attractText.gameObject.SetActive(true);
        helper.attractText.text = GetCurrentItem().text;
        displayTime = GetCurrentItem().displayTime;
    }

    private AttractItem GetCurrentItem()
    {
        return GM.Instance.data.attractItems[currentItem];
    }
}
