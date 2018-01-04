using UnityEngine;
using System.Collections;

[System.Serializable]
public class Song
{

	public string name;
	public string author;
	public AudioClip clip;

	public Song(string name, string author, AudioClip clip) {
		GM.Instance.logger.Info(null, "Creating New Song!");
		this.name = name;
		this.author = author;
		this.clip = clip;
	}
}
