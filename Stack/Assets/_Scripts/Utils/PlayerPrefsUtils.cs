using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PlayerPrefsUtils : MonoBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/<<Clear Data>>", false, -100)]
    public static void ClearData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
#endif
}
