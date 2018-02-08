using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningWindow : MonoBehaviour {
    [SerializeField]
    private Text text;

    public void Active(string value)
    {
        text.text = value;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
