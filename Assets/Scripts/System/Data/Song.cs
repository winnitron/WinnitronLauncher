using UnityEngine;
using System.Collections;

[System.Serializable]
public class Song: Object
{

	public new string name;
	public string author;    
	public AudioClip clip;

	public Song(string name, string author, AudioClip clip) {
		GM.dbug.Log(this, "Creating New Song!");
		this.name = name;
		this.author = author;
		this.clip = clip;
	}
}
