using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaylistUIController : Tweenable {

    public Playlist playlist;

    public GameObject gameLabelPrefab;
    public GameObject gameScreenshotPrefab;

    public GameObject gameLabelContainer;
    public GameObject gameScreenshotContainer;

    public List<GameLabel> gameLabels;
    public List<GameScreenshot> gameScreenshots;

    public Vector3 offsetGameLabels;
    public Vector3 offsetGameScreenshots;

    public float gameLabelScale;
    public float selectedGameLabelScale;
    public float gameScreenshotScale;
    public float selectedGameScreenshotScale;

    public int selectedGameIndex = 0;

	// Use this for initialization
	public void Init () {
        BuildGames();

        //Gotta reset the scale cause parenting
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
	}


    private void BuildGames()
    {
        foreach (Game game in playlist.games)
        {
            // Instantiate a new playlist and set the path to its directory
            GameObject newGameLabel = Instantiate(gameLabelPrefab, gameLabelContainer.transform, true) as GameObject;
            GameLabel label = newGameLabel.GetComponent<GameLabel>();
            label.SetText(game.name);
            gameLabels.Add(label);             //Save a reference to the component we need

            GameObject newGameScreenshot = Instantiate(gameScreenshotPrefab, gameScreenshotContainer.transform, true) as GameObject;
            gameScreenshots.Add(newGameScreenshot.GetComponent<GameScreenshot>());
        }

        RepositionGames();
    }

    private void RepositionGames()
    {
        var i = 0;
        foreach (var gameLabel in gameLabels)
        {
            var relativeIndex = i - selectedGameIndex;
            var thisOffset = offsetGameLabels * relativeIndex;

            gameLabel.transform.localPosition = Vector3.zero + thisOffset;

            if(selectedGameIndex == i)
                gameLabel.transform.localScale = new Vector3(selectedGameLabelScale, selectedGameLabelScale, 1);
            else
                gameLabel.transform.localScale = new Vector3(gameLabelScale, gameLabelScale, 1);

            i++;
        }

        i = 0;
        foreach (var gameScreenshot in gameScreenshots)
        {
            var relativeIndex = i - selectedGameIndex;
            var thisOffset = offsetGameScreenshots * relativeIndex;

            var newPosition = Vector3.zero + thisOffset;
            newPosition.z = Mathf.Abs(newPosition.z);

            gameScreenshot.transform.localPosition = newPosition;

            if (selectedGameIndex == i)
                gameScreenshot.transform.localScale = new Vector3(selectedGameScreenshotScale, selectedGameScreenshotScale, 1);
            else
                gameScreenshot.transform.localScale = new Vector3(gameScreenshotScale, gameScreenshotScale, 1);

            i++;
        }
    }

    public void NextGame()
    {
        selectedGameIndex--;

        if (selectedGameIndex < 0)
            selectedGameIndex = playlist.games.Count - 1;

        RepositionGames();
    }

    public void PreviousGame()
    {
        selectedGameIndex--;

        if (selectedGameIndex < 0)
            selectedGameIndex = playlist.games.Count - 1;

        RepositionGames();
    }

    public void SelectGame()
    {

    }
}
