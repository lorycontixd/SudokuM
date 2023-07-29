using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField] private DebugPanel debugPanel;

    private void Start()
    {
        if (debugPanel != null)
            debugPanel.Close();
    }
    public void ButtonDebug()
    {
        if (debugPanel != null)
            debugPanel.Toggle();
    }
}
