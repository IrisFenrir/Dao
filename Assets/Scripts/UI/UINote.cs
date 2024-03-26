using UnityEngine;
using UnityEngine.UI;

public class UINote : UIPanel
{
    private int m_count;
    private GameObject m_wordItem;

    public UINote(GameObject panel, GameObject wordItem) : base(panel)
    {
        m_wordItem = wordItem;
        panel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            Close();
        });
    }

    public void GenerateWordItem(Word word)
    {
        GameObject item = GameObject.Instantiate(m_wordItem, panel.transform.GetChild(0));
        item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70 * m_count);
        Debug.Log(item);
        item.transform.GetChild(0).GetComponent<Image>().sprite = word.GameWord;
        item.transform.GetChild(1).GetComponent<InputField>().text = word.CustomTranslation;
        item.transform.GetChild(1).GetComponent<InputField>().onEndEdit.AddListener(str =>
        {
            Debug.Log(str);
            word.CustomTranslation = str;
        });
        m_count++;
    }
}

