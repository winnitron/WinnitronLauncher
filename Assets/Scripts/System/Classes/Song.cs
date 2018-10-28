using UnityEngine;
using System.Collections;

[System.Serializable]
public class Song
{

	public string name;
	public AudioClip clip;

	public Song(string name, AudioClip clip) {
		GM.Instance.logger.Info(null, "Creating New Song!");
		this.name = name;
		this.clip = clip;
	}
}
