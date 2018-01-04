using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyBindings
{

    public static string[] CONTROLS = new string[6] {
        "up",
        "down",
        "left",
        "right",
        "button1",
        "button2"
    };

    private Dictionary<string, KeyCode>[] keymap = new Dictionary<string, KeyCode>[4];

    public KeyBindings()
    {
        // Just set up the defaults
        for (int i = 0; i < keymap.Length; i++)
        {
            keymap[i] = new Dictionary<string, KeyCode>();
        }

        SetKey(1, "up", KeyCode.UpArrow);
        SetKey(1, "down", KeyCode.DownArrow);
        SetKey(1, "left", KeyCode.LeftArrow);
        SetKey(1, "right", KeyCode.RightArrow);
        SetKey(1, "button1", KeyCode.Period);
        SetKey(1, "button2", KeyCode.Slash);

        SetKey(2, "up", KeyCode.W);
        SetKey(2, "down", KeyCode.S);
        SetKey(2, "left", KeyCode.A);
        SetKey(2, "right", KeyCode.D);
        SetKey(2, "button1", KeyCode.BackQuote);
        SetKey(2, "button2", KeyCode.Alpha1);

        SetKey(3, "up", KeyCode.I);
        SetKey(3, "down", KeyCode.K);
        SetKey(3, "left", KeyCode.J);
        SetKey(3, "right", KeyCode.L);
        SetKey(3, "button1", KeyCode.G);
        SetKey(3, "button2", KeyCode.H);

        SetKey(4, "up", KeyCode.Keypad8);
        SetKey(4, "down", KeyCode.Keypad5);
        SetKey(4, "left", KeyCode.Keypad4);
        SetKey(4, "right", KeyCode.Keypad6);
        SetKey(4, "button1", KeyCode.Keypad1);
        SetKey(4, "button2", KeyCode.Keypad2);
    }


    public void SetKey(int playerNum, string control, KeyCode key)
    {

        try
        {
            if (keymap[playerNum - 1][control] == key)
            {
                return; // nothing to do here.
            }

            if (allKeys().Contains(key))
            {
                GM.Instance.logger.Error("Duplicate key found: " + key);
                GM.Instance.Oops(GM.Instance.Text("error", "invalidKeymap"), true);
            }
        }
        catch (KeyNotFoundException)
        {
            // NOP. This (only) gets thrown on initialization. We can ignore it.
        }

        keymap[playerNum - 1][control] = key;
    }

    public KeyCode GetKey(int playerNum, string control)
    {
        return keymap[playerNum - 1][control];
    }

    private List<KeyCode> allKeys()
    {
        List<KeyCode> keys = new List<KeyCode>();

        for (int p = 1; p <= 4; p++)
        {
            foreach (string control in CONTROLS)
            {
                keys.Add(GetKey(p, control));
            }
        }
        return keys;
    }

}
