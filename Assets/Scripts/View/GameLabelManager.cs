/*
 * All labels move at once when moving through the grid. The selected label is placed in the middle and scaled up. Labels above and below the selected label are placed
 * relative to that label and are then placed either above of below that
 */ 

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLabelManager : MonoBehaviour {    

    public Text gameLabelPrefab;
    public float GRID_Y_OFFSET = 60;

    public float smallScale;

    // The fixed positions of labels, all other labels are placed relative to these
    public GameObject labelAbove;
    public GameObject labelSelected;
    public GameObject labelBelow;        
    

    List<GameLabel> labelList;

    public bool moving { get; set; }
    public Playlist playlist { get; set; }


    void Start() {

        labelList = new List<GameLabel>();

        CreateLabels();      
        
    }

 

    void CreateLabels() {

        foreach (Game game in playlist.gamesList) {

            var label = Instantiate(gameLabelPrefab) as Text;
            label.text = game.name;            
            label.transform.parent = transform;

            label.GetComponent<GameLabel>().gameLabelManager = this;

            labelList.Add(label.GetComponent<GameLabel>());
        }

        StartCoroutine("waitThenSort");
    }

    IEnumerator waitThenSort() {

        yield return new WaitForSeconds(1.0f);

        sortLabelList(0);
    }

    public void sortLabelList(int selectedGameIndex) {        

        // Move and scale currently selected label        
        labelList[selectedGameIndex].move(labelSelected.transform.position, Vector3.one);
		labelList[selectedGameIndex].setAlpha(1);
        
        // Move all labels above it, starting with the closest
        var index = selectedGameIndex - 1;
        var startIndex = index;
        
        while (index >= 0) {

            labelList[index].move(new Vector3(labelAbove.transform.position.x, labelAbove.transform.position.y + (GRID_Y_OFFSET * (startIndex - index)), labelAbove.transform.position.z), new Vector3(smallScale, smallScale, smallScale));

			//Fade Text as they move away from the selection
			labelList[index].setAlpha(0.5f - (Mathf.Abs (startIndex - index) * 0.1f));
            
			index--;
        }

        // Move all labels below it, starting with the closest
        index = selectedGameIndex + 1;
        startIndex = index;

        while (index < labelList.Count) {

            labelList[index].move(new Vector3(labelBelow.transform.position.x, labelBelow.transform.position.y - (GRID_Y_OFFSET * (index - startIndex)), labelBelow.transform.position.z), new Vector3(smallScale, smallScale, smallScale));

			//Fade Text as they move away from the selection
			labelList[index].setAlpha(0.5f - (Mathf.Abs (startIndex - index) * 0.1f));

            index++;
        }

        moving = true;
    }

    public void stopTween() {

        foreach (GameLabel label in labelList) {

            label.stopTween();
        }

        moving = false;
    }

}