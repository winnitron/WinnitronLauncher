using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SyncJoke {
	public string textAbove;
	public string textSyncing;
	public string punctuation;
	public string textBelow;
}

public class SyncScreenManager : MonoBehaviour {

	public List<SyncJoke> jokes;

	public Text textAbove;
	public Text textSyncing;
	private string syncingText;
	public Text textBelow;
	private string syncingPunctuation;

	public float delayBetweenPunctuation;
	public float punctuationMax = 3;
	public float punctuation = 0;
	public float punctuationDelayCountdown;

	
	void Awake() 
	{
		punctuationDelayCountdown = delayBetweenPunctuation;
        PickRandomText();
    }

    void Start()
    {
        //execute is called from StateManager.ChangeState
    }
		
	// Update is called once per frame
	void Update () 
	{
        punctuationDelayCountdown -= Time.deltaTime;
        if (punctuationDelayCountdown < 0)
        {
            UpdateSyncText();
            punctuationDelayCountdown = delayBetweenPunctuation;
        }
        
	}

	void PickRandomText()
	{
		var rando = Random.Range(0, jokes.Count);
		//var rando = 1;

		textAbove.text = jokes[rando].textAbove;
		textBelow.text = jokes[rando].textBelow;
		syncingText = jokes[rando].textSyncing;
		syncingPunctuation = jokes[rando].punctuation;
	}

	void UpdateSyncText()
	{
		punctuation++;
		if(punctuation > punctuationMax) {
			punctuation = 0;
			PickRandomText();
		}

		textSyncing.text = syncingText;

		for(var i = 0; i < punctuation; i++)
			textSyncing.text += syncingPunctuation;
	}

}
