using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour
{
    public static DebugLogger Instance;

    [Header("UI")]
    public Text debugText;

    [Header("Data")]
    public bool isDebug = true;

    #region MonoBehaviour func

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        debugText.text = "start log...";
    }

    #endregion

    public static void SendDebug(string log) 
    {
        if (Instance == null)
        {
            //Debug.Log("[] DebugLogger : SendDebug is null.");
            Instance = new DebugLogger();
        }

        if (Instance.debugText != null)
            Instance.debugText.text = log;

        if (Instance.isDebug)
            Debug.Log(log);
    }
}
