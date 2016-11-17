using UnityEngine;
using UnityEngine.UI;
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

    public float gameLabelAlphaFalloff = 0.2f;
    public float gameScreenshotAlphaFalloff = 0.3f;

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

            //Position
            Vector3 newPosition = Vector3.zero + thisOffset;
            newPosition.z = Mathf.Abs(newPosition.z);

            //Rotation
            gameLabel.transform.localEulerAngles = Vector3.zero;

            //Scale
            Vector3 newScale;
            if (selectedGameIndex == i)
                newScale = new Vector3(selectedGameLabelScale, selectedGameLabelScale, 1);
            else
                newScale = new Vector3(gameLabelScale, gameLabelScale, 1);

            //Alpha
            float newAlpha = 1 - (gameLabelAlphaFalloff * Mathf.Abs(relativeIndex));
            gameLabel.GetComponent<Text>().color = new Color(1, 1, 1, newAlpha);

            //Commit!
            gameLabel.Tween(newPosition, newScale);

            i++;
        }

        i = 0;
        foreach (var gameScreenshot in gameScreenshots)
        {
            var relativeIndex = i - selectedGameIndex;
            var thisOffset = offsetGameScreenshots * relativeIndex;

            //Position
            Vector3 newPosition = Vector3.zero + thisOffset;
            newPosition.z = Mathf.Abs(newPosition.z);

            //Rotation
            gameScreenshot.transform.localEulerAngles = Vector3.zero;

            //Scale
            Vector3 newScale;
            if (selectedGameIndex == i)
                newScale = new Vector3(selectedGameScreenshotScale, selectedGameScreenshotScale, 1);
            else
                newScale = new Vector3(gameScreenshotScale, gameScreenshotScale, 1);

            //Alpha
            float newAlpha = 1 - (gameScreenshotAlphaFalloff * Mathf.Abs(relativeIndex));
            gameScreenshot.GetComponent<Image>().color = new Color(1, 1, 1, newAlpha);

            //Commit
            gameScreenshot.Tween(newPosition, newScale);

            i++;
        }
    }

    public void PreviousGame()
    {
        selectedGameIndex++;

        if (selectedGameIndex > playlist.games.Count - 1)
            selectedGameIndex = 0;

        RepositionGames();
    }

    public void NextGame()
    {
        selectedGameIndex--;

        if (selectedGameIndex < 0)
            selectedGameIndex = playlist.games.Count - 1;

        RepositionGames();
    }

    public Game GetCurrentGame()
    {
        return playlist.games[selectedGameIndex];
    }
}
