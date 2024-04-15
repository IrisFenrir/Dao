using Dao.CameraSystem;
using Dao.InteractionSystem;
using System.Threading.Tasks;
using UnityEngine;

namespace Dao.SceneSystem
{
    public class Room1 : IScene
    {
        private GameObject m_root;

        public override void OnEnter()
        {
            m_root = FindUtility.Find("Room1");
            
            GameObject cameraSetting = FindUtility.Find("CameraSetting", m_root.transform);
            Bound2D bound = cameraSetting.transform.Find("Bound").GetComponent<Bound2D>();
            float screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            CameraController.Instance.MoveRange = new Vector2(bound.Rect.xMin + screenWidth / 2, bound.Rect.xMax - screenWidth / 2);
            CameraController.Instance.SetPosition(new Vector3(CameraController.Instance.MoveRange.x, 0, -10));
            CameraController.Instance.Enable = true;

            InteractiveItem[] items = FindUtility.FindAllOfType<InteractiveItem>(m_root.transform);
            InitItems(items);

            m_root.SetActive(true);
        }

        public override void OnExit()
        {
            m_root.SetActive(false);
        }

        private void InitItems(InteractiveItem[] items)
        {
            foreach (InteractiveItem item in items)
            {
                switch(item.itemName)
                {
                    case "Door":
                        InitItem_Door(item);
                        break;
                }
            }
        }

        private void InitItem_Door(InteractiveItem item)
        {
            GameObject background = FindUtility.Find("Background", m_root.transform);
            GameObject nearDoor = FindUtility.Find("NearDoor", m_root.transform);
            GameObject openDoor = FindUtility.Find("OpenDoor", m_root.transform);

            background.SetActive(true);
            nearDoor.SetActive(false);
            openDoor.SetActive(false);

            item.onMouseDown = () =>
            {
                background.SetActive(false);
                nearDoor.SetActive(true);
                CameraController.Instance.Enable = false;
            };
            var closeButton = nearDoor.transform.Find("CloseButton").GetComponent<InteractiveItem>();
            closeButton.onMouseDown = () =>
            {
                nearDoor.SetActive(false);
                background.SetActive(true);
            };

            InteractiveItem doorHandle = FindUtility.Find("DoorHandle", nearDoor.transform).GetComponent<InteractiveItem>();
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
                    SceneManager.Instance.LoadScene("Room2");
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
