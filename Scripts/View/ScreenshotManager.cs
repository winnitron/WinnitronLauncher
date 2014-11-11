using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenshotManager : MonoBehaviour {

    public GameObject imagePosition;                //Object that marks the position where the current selected image should be placed;
    public Image gameImagePrefab;

    public float GRID_Y_OFFSET = 60;

    public float smallScale;

    public Material currentScreenshot;
    public Material otherScreenshot;
    

    Image[] screens;

    List<GameImage> imageList;

    public bool moving { get; set; }
    public Playlist playlist { get; set; }


    void Start() {

        imageList = new List<GameImage>();

        createScreenshots();
    }

    void createScreenshots() {

        foreach (Game game in playlist.gamesList) {

            var image = Instantiate(gameImagePrefab) as Image;
            image.sprite = game.screenshot;
            image.transform.parent = transform;

            image.GetComponent<GameImage>().screenshotManager = this;

            imageList.Add(image.GetComponent<GameImage>());
        }

        StartCoroutine("waitThenSort");
    }

    IEnumerator waitThenSort() {

        yield return new WaitForSeconds(1.0f);

        sortImageList(0);
    }

    public void sortImageList(int selectedGameIndex) {

        for (int i = 0; i < imageList.Count; i++) {

            if (i < selectedGameIndex) {

                imageList[i].GetComponent<Image>().material = otherScreenshot;
                imageList[i].move(new Vector3(imagePosition.transform.position.x, imagePosition.transform.position.y + (GRID_Y_OFFSET * (selectedGameIndex - i)), imagePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale));
            }
            else if (i == selectedGameIndex) {

                imageList[i].GetComponent<Image>().material = currentScreenshot;
                imageList[i].move(new Vector3(imagePosition.transform.position.x, imagePosition.transform.position.y, imagePosition.transform.position.z), new Vector3(1, 1, 1));
            }
            else if (i > selectedGameIndex) {

                imageList[i].GetComponent<Image>().material = otherScreenshot;
                imageList[i].move(new Vector3(imagePosition.transform.position.x, imagePosition.transform.position.y - (GRID_Y_OFFSET * (i - selectedGameIndex)), imagePosition.transform.position.z), new Vector3(smallScale, smallScale, smallScale));
            }
        }

        moving = true;
    }


    public void stopTween() {

        foreach (GameImage image in imageList) {

            image.stopTween();
        }

        moving = false;
    }
}
