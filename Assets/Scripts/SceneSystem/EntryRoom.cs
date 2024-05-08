using Dao.CameraSystem;
using Dao.WordSystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Dao.SceneSystem
{
    public class EntryRoom : IScene
    {
        public bool enable;

        private GameObject m_root;
        private List<Responder> m_responders = new();

        public EntryRoom()
        {
            m_root = FindUtility.Find("EntryRoom");
            InitItems();
        }

        public override void OnEnter()
        {
            GameObject cameraSetting = FindUtility.Find("CameraSetting", m_root.transform);
            Bound2D bound = cameraSetting.transform.Find("Bound").GetComponent<Bound2D>();
            float screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            CameraController.Instance.MoveRange = new Vector2(bound.Rect.xMin + screenWidth / 2, bound.Rect.xMax - screenWidth / 2);
            CameraController.Instance.SetPosition(new Vector3(CameraController.Instance.MoveRange.x, 0, -10));
            CameraController.Instance.Enable = true;
        }

        public override void Enable()
        {
            

            m_responders.ForEach(p => p.enable = true);
        }

        public override void Show()
        {
            m_root.SetActive(true);
        }

        public override void Disable()
        {
            m_responders.ForEach(p => p.enable = false);
        }

        public override void Hide()
        {
            m_root.SetActive(false);
        }

        private void InitItems()
        {
            Door();
        }

        private void Door()
        {
            GameObject background = FindUtility.Find("Background", m_root.transform);
            GameObject nearDoor = FindUtility.Find("NearDoor", m_root.transform);
            GameObject openDoor = FindUtility.Find("OpenDoor", m_root.transform);

            background.SetActive(true);
            nearDoor.SetActive(false);
            openDoor.SetActive(false);

            //Word word = WordManager.Instance.GetWord("开");

            var backgroundResponder = background.AddComponent<Responder>();
            m_responders.Add(backgroundResponder);
            backgroundResponder.onMouseDown = () =>
            {
                background.SetActive(false);
                nearDoor.SetActive(true);
                CameraController.Instance.Enable = false;
            };

            var closeButton = nearDoor.transform.Find("CloseButton").gameObject.AddComponent<Responder>();
            closeButton.onMouseDown = () =>
            {
                nearDoor.SetActive(false);
                background.SetActive(true);
            };
            GameObject uiword = FindUtility.Find("UIWord", nearDoor.transform);
            UIWord uIWord = new UIWord(WordManager.Instance.GetWord("Open"), uiword, FindUtility.Find("WorldCanvas/EntryRoom_Door_UIWord_Open"));

            Responder doorHandle = FindUtility.Find("DoorHandle", nearDoor.transform).AddComponent<Responder>();
            Vector3 origin = doorHandle.transform.position;
            Vector3 startPos = Vector3.zero;
            bool isMouseDown = false;
            Vector2 angleRange = GameLoop.Instance.doorHandleAngleRange;
            doorHandle.onMouseDown = () =>
            {
                startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                startPos.Set(startPos.x, startPos.y, origin.z);
                isMouseDown = true;
            };
            doorHandle.onMouseOver = async () =>
            {
                if (!isMouseDown) return;
                Vector3 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPos.Set(currentPos.x, currentPos.y, origin.z);
                float angle = Vector3.SignedAngle(startPos - origin, currentPos - origin, Vector3.back);
                angle = Mathf.Clamp(angle, angleRange.x, angleRange.y);
                doorHandle.transform.eulerAngles = new Vector3(0, 0, -angle);
                if (angle == angleRange.y)
                {
                    nearDoor.SetActive(false);
                    openDoor.SetActive(true);
                    var audio = openDoor.GetComponent<AudioSource>();
                    audio.Play();
                    while (audio.isPlaying)
                    {
                        await Task.Yield();
                    }
                    openDoor.SetActive(false);
                    SceneManager.Instance.LoadScene("LivingRoom");
                }
            };
            doorHandle.onMouseUp = () =>
            {
                doorHandle.transform.eulerAngles = Vector3.zero;
                isMouseDown = false;
            };


        }
    }
}
