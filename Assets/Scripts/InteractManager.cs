using UnityEngine;
using UnityEngine.UI;

public class InteractManager
{
    private GameObject m_boxPanel;
    private Sprite m_switchOn;
    private Sprite m_switchOff;
    private bool[] m_boxButtonStates;
    private bool[] m_boxAnswer;

    public InteractManager()
    {
        //m_boxPanel = boxPanel;
        //m_switchOn = switchOn;
        //m_switchOff = switchOff;
        //m_boxAnswer = boxAnswer;

        var items = GameObject.FindObjectsOfType<InteractiveItem>();
        foreach (var item in items)
        {
            switch(item.itemName)
            {
                case "Box":
                    ProcessBox(item);
                    break;
                case "Door":
                    ProcessDoor(item);
                    break;
            }
        }
    }

    private void ProcessBox(InteractiveItem item)
    {
        m_boxPanel = UIManager.Instance.GetPanelObject("Puzzle1_Box");
        m_boxPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.ClosePanel("Puzzle1_Box");
        });
        m_switchOn = GameLoop.Instance.switchON;
        m_switchOff = GameLoop.Instance.switchOFF;
        m_boxAnswer = new bool[] { true, true, false, true, false };
        //m_boxPanel.GetComponent<SpriteButton>().OnClick = () =>
        //{
        //    //m_boxPanel.SetActive(false);
        //    UIManager.Instance.ClosePanel("Puzzle1_Box");

        //};

        item.MouseDownAction = () =>
        {
            UIManager.Instance.OpenPanel("Puzzle1_Box");
            CameraController.Instance.Enable = false;
        };

        //m_boxButtons = m_boxPanel.GetComponentsInChildren<SpriteButton>();
        var buttons = m_boxPanel.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Button>();
        m_boxButtonStates = new bool[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.transform.GetChild(0).GetComponent<Image>().sprite = m_switchOff;
            int j = i;
            button.onClick.AddListener(() =>
            {
                m_boxButtonStates[j] = !m_boxButtonStates[j];
                if (m_boxButtonStates[j])
                {
                    button.transform.GetChild(0).GetComponent<Image>().sprite = m_switchOn;
                }
                else
                {
                    button.transform.GetChild(0).GetComponent<Image>().sprite = m_switchOff;
                }
            });
        }

        m_boxPanel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(CheckBoxAnswer);
    }

    private void CheckBoxAnswer()
    {
        bool result = true;
        for (int i = 0; i < 5; i++)
        {
            result = result && m_boxButtonStates[i] == m_boxAnswer[i];
        }
        if (result)
        {
            Debug.Log("TRUE");
        }
        else
        {
            Debug.Log("FALSE");
        }
    }

    private SpriteButton m_doorButton;
    private void ProcessDoor(InteractiveItem item)
    {
        item.MouseDownAction = () =>
        {
            item.transform.GetChild(0).gameObject.SetActive(true);
        };

        var go = item.transform.GetChild(0).GetChild(0);
        m_doorButton = go.GetComponent<SpriteButton>();
        m_doorButton.OnClick = () =>
        {
            m_mouseX = Input.mousePosition.x;
            m_pressingDoor = true;
        };
        m_doorButton.OnUp = () => 
        {
            m_pressingDoor = false;
            m_doorButton.transform.eulerAngles = new Vector3(0, 0, 0);
            m_doorAngle = 0;
        };

        item.transform.GetChild(0).GetChild(2).GetComponent<SpriteButton>().OnClick = () =>
        {
            item.transform.GetChild(0).gameObject.SetActive(false);
        };
    }

    private bool m_pressingDoor;
    private float m_mouseX;
    private float m_doorAngle;

    public void Update()
    {
        if (m_pressingDoor)
        {
            float offset = Input.mousePosition.x - m_mouseX;

            m_doorAngle += (-offset * 1f);
            m_doorAngle = Mathf.Clamp(m_doorAngle, -90, 60);
            m_doorButton.transform.eulerAngles = new Vector3(0, 0, m_doorAngle);

            if (m_doorAngle == -90)
            {
                GameLoop.Instance.ChangeScene();
                m_pressingDoor = false;
                m_doorAngle = 0;
            }

            m_mouseX = Input.mousePosition.x;
        }
    }
}