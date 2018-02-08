using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningManager : MonoBehaviour {

    public static List<string> errors = new List<string>();

    [SerializeField]
    private WarningWindow window;

    private void Update()
    {
        if (errors.Count>0)
        {
            string err = errors[0];
            errors.RemoveAt(0);
            window.Active(err);
        }
    }
}
