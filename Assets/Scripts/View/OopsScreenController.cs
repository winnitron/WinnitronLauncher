using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OopsScreenController : MonoBehaviour {

	public Text errorText;

	public void SetErrorText(string t)
	{
		errorText.text = t;
	}
}
