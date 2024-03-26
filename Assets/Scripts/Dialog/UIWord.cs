using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWord : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string wordName;

    private Text m_translationText;
    private Image m_gameWordImage;

    private Word m_word;

    private void Awake()
    {
        m_translationText = transform.GetChild(0).GetComponent<Text>();
        m_gameWordImage = GetComponent<Image>();
    }

    private void Start()
    {
        BindWord();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_translationText.text = m_word.CustomTranslation;
        m_translationText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_translationText.gameObject.SetActive(false);
    }

    public void BindWord()
    {
        m_word = WordManager.Instance.GetWord(wordName);
        m_translationText.text = m_word.CustomTranslation;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WordManager.Instance.AddFoundWord(m_word);
        UIManager.Instance.OpenPanel("Note");
    }
}

