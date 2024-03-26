using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIPanel> m_panels = new();

    public void AddPanel(string name, UIPanel panel)
    {
        m_panels.TryAdd(name, panel);
    }

    public GameObject GetPanelObject(string name)
    {
        if (m_panels.TryGetValue(name, out UIPanel panel))
        {
            return panel.panel;
        }
        return null;
    }

    public UIPanel GetPanel(string name)
    {
        if (m_panels.TryGetValue(name, out UIPanel panel))
        {
            return panel;
        }
        return null;
    }

    public GameObject OpenPanel(string name)
    {
        Debug.Log(name);
        if (m_panels.TryGetValue(name, out UIPanel panel))
        {
            panel.Open();
            return panel.panel;
        }
        return null;
    }

    public void ClosePanel(string name)
    {
        if (m_panels.TryGetValue(name, out UIPanel panel))
        {
            panel.Close();
        }
    }
}