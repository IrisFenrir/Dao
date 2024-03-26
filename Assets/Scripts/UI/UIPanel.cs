using UnityEngine;

public class UIPanel
{
    public string name;

    public GameObject panel;

    public UIPanel(GameObject panel)
    {
        this.panel = panel;
    }

    public virtual void Open()
    {
        panel.SetActive(true);
    }

    public virtual void Close()
    {
        panel.SetActive(false);
    }
}

