using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LogText : MonoBehaviour
{
    private Text _textObject;

    private void Awake()
    {
        _textObject = GetComponent<Text>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }
    
    void LogCallback(string condition, string stackTrace, LogType type)
    {
        var text = "";
        
        switch (type)
        {
            case LogType.Log:
                text = condition;
                break;
            case LogType.Error:
                text = "<color=red>" + condition + "</color>";
                break;
            case LogType.Exception:
                text = "<color=red>" + condition + "\n" + stackTrace + "</color>";
                break;
        }
        
        var time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]\n";

        _textObject.text = time + text;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }
}
