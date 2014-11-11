using UnityEngine;
using System.Collections;

public static class Maths {
	public static int Mod(int a, int n) {
		int result = a % n;
		if ((a<0 && n>0) || (a>0 && n<0))
			result += n;
		return result;
	}
}
