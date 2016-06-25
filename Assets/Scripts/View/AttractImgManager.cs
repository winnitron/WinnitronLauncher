using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AttractImgManager :  Singleton<AttractImgManager> {

	public List<GameObject> attractSprites;
	public GameObject attractSpritePrefab;
	
	private string attractDirectory;

	public float timePassed = 0;
	public int slideNum = 0;

	public bool switchDelay = false;
	
	protected override void Awake() {
		base.Awake();
		BuildAttractImagesList();
		ResetSlides();
	}

	void Update() {
		if (GM.worldState == GM.WorldState.Attract) {
			if(timePassed == 0) {
				Debug.Log ("hey!  first time running attract");
				slideNum = -1;
				NextSlide();
			}

			timePassed += Time.deltaTime;

			if(!attractSprites[slideNum].GetComponent<Animation>().isPlaying) NextSlide ();

		} else {
			//start from the beginning next time
			timePassed = 0;
			ResetSlides();
		}
	}

	void ResetSlides() {
		foreach (var asp in attractSprites) {
			asp.GetComponent<Animation>().Rewind();
			asp.GetComponent<Animation>().Stop();
			asp.SetActive(false);
		}
	}

	void NextSlide() {
		if(slideNum >= 0) attractSprites [slideNum].SetActive (false);

		slideNum += 1;

		Debug.Log ("Calling Next Slide: " + slideNum + " of " + attractSprites.Count);

		if (slideNum > attractSprites.Count - 1) slideNum = 0;

		attractSprites[slideNum].GetComponent<Animation>().enabled = true;
		attractSprites[slideNum].GetComponent<Animation>().Rewind();
		attractSprites[slideNum].SetActive(true);
		attractSprites[slideNum].GetComponent<Animation>().Play();
	}

	void BuildAttractImagesList() {
        Debug.Log("attract " + GM.options.attractPath);
		var attractDir = new DirectoryInfo(GM.options.attractPath);
		
		foreach (var attractImg in attractDir.GetFiles()) {
			//Make sure only .png's get through!
			Debug.Log ("Loading image with ending: " + attractImg.FullName.Substring(Math.Max(0, attractImg.FullName.Length - 4)) == ".png");
			if(attractImg.FullName.Substring(Math.Max(0, attractImg.FullName.Length - 4)) == ".png") {
				GameObject newImgObj = Instantiate(attractSpritePrefab, new Vector3(0, -36.52403f, 0), transform.rotation) as GameObject; 

				Debug.Log("Load Attract Image: " + attractImg);
				// Load the screenshot from the games directory as a Texture2D
				var screenshot = new Texture2D(1024, 768);
				screenshot.LoadImage(File.ReadAllBytes(attractImg.FullName));

				newImgObj.GetComponent<Image>().overrideSprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
				newImgObj.transform.SetParent (transform);

				attractSprites.Add(newImgObj);
			}
		}
	}
}
	