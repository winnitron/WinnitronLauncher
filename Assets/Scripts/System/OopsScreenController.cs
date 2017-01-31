using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OopsScreenController : MonoBehaviour {

	public Text errorText;
    public Text quitText;
    public bool isCritical = false;

	public void SetErrorText(string t)
	{
		errorText.text = t;
	}

    void Update()
    {
        if(isCritical)
        {
            quitText.text = GM.Text("ui", "oopsQuit");
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        else
        {
            quitText.text = GM.Text("ui", "oopsRestart");
            if (Input.GetKeyDown(KeyCode.Escape))
                GM.Restart();
        }
    }
}
