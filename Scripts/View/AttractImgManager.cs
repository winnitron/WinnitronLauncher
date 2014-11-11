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
	
	private string ATTRACT_SUBDIRECTORY = "Attract";
	
	private string attractDirectory;

	public int slideNum = 0;
	public float timePassed = -1;
	public float WaitTime = 5;
	public float modulo = 0;

	public bool switchDelay = false;
	
	protected override void Awake() {
		base.Awake();
		attractDirectory = Path.Combine(Application.dataPath, ATTRACT_SUBDIRECTORY);
		BuildAttractImagesList();
		ResetSlides ();
	}

	void Update() {
		if (GM.worldState == GM.WorldState.Attract) {

			timePassed += Time.deltaTime;

			modulo = timePassed % WaitTime;

			if(modulo <= 0.5 && switchDelay) {
				NextSlide();
				switchDelay = false;
			}

			if(modulo > WaitTime * 0.5) switchDelay = true;

		} else {
			//start from the beginning next time
			timePassed = 0;

			ResetSlides();
		}
	}

	void ResetSlides() {
		foreach (var asp in attractSprites) {
			asp.animation.Rewind ();
			asp.animation.Stop ();
			asp.animation.enabled = false;
		}
	}

	void NextSlide() {
		Debug.Log ("Calling Next Slide: " + slideNum);

		slideNum += 1;

		if (slideNum > attractSprites.Count) slideNum = 0;

		attractSprites[slideNum].animation.Rewind();
		attractSprites[slideNum].animation.enabled = true;
		attractSprites[slideNum].animation.Play();
	}

	void BuildAttractImagesList() {	
		var attractDir = new DirectoryInfo(attractDirectory);
		
		foreach (var attractImg in attractDir.GetFiles()) {
			//Make sure only .png's get through!
			if(attractImg.FullName.Substring(Math.Max(0, attractImg.FullName.Length - 4)) == ".png") {
				GameObject newImgObj = Instantiate(attractSpritePrefab, new Vector3(0, -36.52403f, 0), transform.rotation) as GameObject; 

				Debug.Log("Load Attract Image: " + attractImg);
				// Load the screenshot from the games directory as a Texture2D
				var screenshot = new Texture2D(1024, 768);
				screenshot.LoadImage(File.ReadAllBytes(attractImg.FullName));

				newImgObj.GetComponent<Image>().sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
				newImgObj.transform.parent = transform;

				attractSprites.Add(newImgObj);
			}
		}
	}
}
	