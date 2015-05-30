using UnityEngine;
using System.Collections;

public class Song : MonoBehaviour {

	public new string name;
	public string author;    
	public AudioClip clip;

	public Song(string name, string author, AudioClip clip) {
		this.name = name;
		this.author = author;
		this.clip = clip;
	}
}
