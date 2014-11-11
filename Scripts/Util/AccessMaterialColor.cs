using UnityEngine;
using System.Collections;

public class AccessMaterialColor : MonoBehaviour {

	public float r;
	public float g;
	public float b;
	public float a;
	
	public Color initialColor;

	public bool isSprite = false;

	void Start() {
		r = initialColor.r;
		g = initialColor.g;
		b = initialColor.b;
		a = initialColor.a;
	}

	// Update is called once per frame
	void Update () {
		if(!isSprite)renderer.material.color = new Color(r, g, b, a);
		else GetComponent<SpriteRenderer>().color = new Color(r, g, b, a);
	}
}
