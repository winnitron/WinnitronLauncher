using UnityEngine;
using System.Collections;

public static class Funcs {
	public static T Identity<T>(T t) { return t; }
	public static float SmoothStep(float t) { return Mathf.SmoothStep(0, 1, t); }
}
