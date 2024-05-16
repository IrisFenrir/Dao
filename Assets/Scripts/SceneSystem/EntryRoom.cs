using Dao.CameraSystem;
using Dao.InventorySystem;
using Dao.WordSystem;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Dao.SceneSystem
{
    public class EntryRoom : IScene
    {
        public bool enable;

        private GameObject m_root;
        private List<Responder> m_responders = new();

        private Transform m_mouseHandle;

        private bool m_canShowInteractive;
        private bool m_canShowNearDoorInteractive;

        private MoveTask m_moveTask = new();

        public EntryRoom()
        {
            m_root = FindUtility.Find("EntryRoom");
            InitItems();
        }

        public override void OnEnter()
        {
            //GameObject cameraSetting = FindUtility.Find("CameraSetting", m_root.transform);
            //Bound2D bound = cameraSetting.transform.Find("Bound").GetComponent<Bound2D>();
            //float screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            //CameraController.Instance.MoveRange = new Vector2(bound.Rect.xMin + screenWidth / 2, bound.Rect.xMax - screenWidth / 2);
            //CameraController.Instance.SetPosition(new Vector3(CameraController.Instance.MoveRange.x, 0, -10));
            //CameraController.Instance.Enable = true;

            CameraController.Instance.SetPosition(new Vector3(0, 0, -10));
            Camera.main.orthographicSize = 0.8f;
        }

        public override void OnUpdate(float deltaTime)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_mouseHandle.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            if (Input.GetMouseButtonDown(2))
            {
                if (m_canShowInteractive)
                {
                    FindUtility.Find("Environments/EntryRoom/Scene/InteractiveItems").SetActive(true);
                }
                else if (m_canShowNearDoorInteractive)
                {
                    FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/InteractiveItems").SetActive(true);
                }
            }
            if (Input.GetMouseButtonUp(2))
            {
                FindUtility.Find("Environments/EntryRoom/Scene/InteractiveItems").SetActive(false);
                FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/InteractiveItems").SetActive(false);
            }

            m_moveTask.Update(deltaTime);
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
            Background();

            Hand();

            Mather();

            Door();
        }

        private void Background()
        {
            FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders/Background").AddComponent<Responder>().onMouseDown = () =>
            {
                float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                m_moveTask.Start(x);
                m_moveTask.OnComplete = null;
            };
        }

        private void Hand()
        {
            var handTrigger = FindUtility.Find("Environments/EntryRoom/Scene/HandTrigger");

            var hand1 = FindUtility.Find("Environments/EntryRoom/Scene/Hand1");
            var hand2 = FindUtility.Find("Environments/EntryRoom/Scene/Hand2");
            var hand3 = FindUtility.Find("Environments/EntryRoom/Scene/Hand3");
            var hand4 = FindUtility.Find("Environments/EntryRoom/Scene/Hand4");
            var hand5 = FindUtility.Find("Environments/EntryRoom/Scene/Hand5");
            int handIndex = 0;

            var paper = FindUtility.Find("Environments/EntryRoom/Scene/Background/Paper");
            var responders = FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders");
            var colliders = FindUtility.Find("Environments/EntryRoom/Scene/Background").GetComponentsInChildren<Collider2D>().ToList();
            colliders.ForEach(c => c.enabled = false);

            handTrigger.AddComponent<Responder>().onMouseDown = async () =>
            {
                if (handIndex == 0)
                {
                    hand1.SetActive(false);
                    hand2.SetActive(true);
                }
                else if (handIndex == 1)
                {
                    hand2.SetActive(false);
                    hand3.SetActive(true);
                }
                else if (handIndex == 2)
                {
                    hand3.SetActive(false);
                    hand4.SetActive(true);
                }
                else if (handIndex == 3)
                {
                    hand4.SetActive(false);
                    hand5.SetActive(true);
                    paper.SetActive(true);
                }
                else if (handIndex == 4)
                {
                    handTrigger.SetActive(false);
                    //while (paper.transform.position.y > -3.1f)
                    //{
                    //    paper.transform.Translate(Vector3.down * 1f * Time.deltaTime);
                    //    var pos = new Vector3(0, paper.transform.position.y, -10);
                    //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, pos, 0.01f);
                    //    await Task.Yield();
                    //}
                    //hand5.SetActive(false);
                    //await Task.Delay(1000);
                    //Camera camera = Camera.main;
                    //var blackBackground = FindUtility.Find("Environments/EntryRoom/Scene/BlackBackground").GetComponent<SpriteRenderer>();
                    //float a = 1;
                    //while (camera.transform.position.y < 0)
                    //{
                    //    a -= (1f / 2.8f) * Time.deltaTime;
                    //    a = Mathf.Clamp01(a);
                    //    blackBackground.color = new Color(0, 0, 0, a);
                    //    camera.transform.Translate(Vector3.up * 1f * Time.deltaTime);
                    //    camera.orthographicSize += (4.2f / 2.8f) * Time.deltaTime;
                    //    await Task.Yield();
                    //}
                    //camera.transform.position = new Vector3(0, 0, -10);
                    //camera.orthographicSize = 5;

                    while (paper.transform.position.y > -1.5f)
                    {
                        paper.transform.Translate(Vector3.down * 1f * Time.deltaTime);
                        var pos = new Vector3(0, paper.transform.position.y, -10);
                        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, pos, 0.01f);
                        await Task.Yield();
                    }
                    hand5.SetActive(false);
                    var blackBackground = FindUtility.Find("Environments/EntryRoom/Scene/BlackBackground").GetComponent<SpriteRenderer>();
                    float a = 1;
                    while (paper.transform.position.y > -3.1f)
                    {
                        paper.transform.Translate(Vector3.down * 1f * Time.deltaTime);
                        var pos = new Vector3(0, paper.transform.position.y, -10);
                        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, pos, 0.01f);
                        
                        Camera.main.orthographicSize += 1f * Time.deltaTime;
                        a = Mathf.Clamp01(a - 1f * Time.deltaTime);
                        blackBackground.color = new Color(0, 0, 0, a);
                        await Task.Yield();
                    }
                    while (Camera.main.transform.position.y < -0.01f || Camera.main.orthographicSize < 4.99f)
                    {
                        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, 0, -10), 0.002f);
                        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5f, 0.002f);
                        await Task.Yield();
                    }
                    Camera.main.transform.position = new Vector3(0, 0, -10);
                    Camera.main.orthographicSize = 5f;
                    await Task.Delay(1000);

                    responders.SetActive(true);
                    m_canShowInteractive = true;
                    colliders.ForEach(c => c.enabled = true);
                }
                handIndex++;
            };

            responders.transform.Find("PaperTrigger").gameObject.AddComponent<Responder>().onMouseDown = () =>
            {
                float x = responders.transform.Find("PaperTrigger").position.x;
                m_moveTask.Start(x);
                m_moveTask.OnComplete = () =>
                {
                    paper.SetActive(false);
                    InventoryManager.Instance.AddItem(new Piece4());
                    Object.Destroy(FindUtility.Find("Environments/EntryRoom/Scene/InteractiveItems/Icon1"));
                    Object.Destroy(FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders/PaperTrigger"));
                };
            };
        }

        private void Mather()
        {
            bool isFirst = true;

            var responders = FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders");
            var mather = FindUtility.Find("MatherModel").transform;

            FindUtility.Find("Mather", responders.transform).AddComponent<Responder>().onMouseDown = async () =>
            {
                float x = mather.position.x;
                m_moveTask.Start(x);
                m_moveTask.OnComplete = async () =>
                {
                    responders.SetActive(false);
                    CameraController.Instance.Enable = false;
                    m_canShowInteractive = false;
                    var dialog1 = DialogUtility.GetDialog("EntryRoom-Mather-First");
                    var dialog2 = DialogUtility.GetDialog("EntryRoom-Mather-NotFirst");
                    var pos = FindUtility.Find("MatherModel").transform.position + new Vector3(-1.63f, 1.56f, 0);
                    FindUtility.Find("WorldCanvas/SecretSentence").transform.position = pos;
                    UIDialogManager.Instance.StartDialog(isFirst ? dialog1 : dialog2, false);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    if (isFirst)
                    {
                        InventoryManager.Instance.AddItem(new Key());
                        isFirst = false;
                    }
                    responders.SetActive(true);
                    CameraController.Instance.Enable = true;
                    m_canShowInteractive = true;
                };
            };
        }

        private void Door()
        {
            m_mouseHandle = FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/MouseHandle").transform;

            GameObject background = FindUtility.Find("Background", m_root.transform);
            GameObject nearDoor = FindUtility.Find("NearDoor", m_root.transform);
            GameObject openDoor = FindUtility.Find("OpenDoor", m_root.transform);

            var mather = FindUtility.Find("MatherModel");
            var player = FindUtility.Find("Player");
            var responders = FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders");

            background.SetActive(true);
            nearDoor.SetActive(false);
            openDoor.SetActive(false);

            //Word word = WordManager.Instance.GetWord("开");

            var door = FindUtility.Find("Environments/EntryRoom/Scene/Background/Responders/Door").AddComponent<Responder>();
            m_responders.Add(door);
            door.onMouseDown = () =>
            {
                float x = door.transform.position.x;
                m_moveTask.Start(x);
                m_moveTask.OnComplete = () =>
                {
                    m_canShowInteractive = false;
                    responders.SetActive(false);
                    mather.SetActive(false);
                    player.SetActive(false);
                    background.SetActive(false);
                    nearDoor.SetActive(true);
                    CameraController.Instance.Enable = false;
                    m_canShowNearDoorInteractive = true;

                    // 获得密文
                    UIDictionary.Instance.AddWord("Open");
                    UIDictionary.Instance.AddWord("Back");
                };
            };

            var closeButton = nearDoor.transform.Find("CloseButton").gameObject.AddComponent<Responder>();
            closeButton.onMouseDown = () =>
            {
                nearDoor.SetActive(false);
                background.SetActive(true);
                mather.SetActive(true);
                player.SetActive(true);
                responders.SetActive(true);
                m_canShowInteractive = true;
                m_canShowNearDoorInteractive = false;
            };
            GameObject uiword = FindUtility.Find("UIWord", nearDoor.transform);
            var wordGO = FindUtility.Find("WorldCanvas/UIWord");
            wordGO.transform.position = uiword.transform.position + new Vector3(0, 0, 0);
            UIWord uIWord = new UIWord(WordManager.Instance.GetWord("Open"), uiword, wordGO);

            bool isLocked = true;

            Responder doorHandle = FindUtility.Find("DoorHandle", nearDoor.transform).AddComponent<Responder>();
            Vector3 origin = doorHandle.transform.position;
            Vector3 startPos = Vector3.zero;
            bool isMouseDown = false;
            Vector2 angleRange = GameLoop.Instance.doorHandleAngleRange;
            doorHandle.onMouseDown = () =>
            {
                if (isLocked)
                {
                    return;
                }
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
                if (angle == angleRange.y && InventoryManager.Instance.Contains<Piece4>())
                {
                    nearDoor.SetActive(false);
                    openDoor.SetActive(true);
                    mather.SetActive(true);
                    player.SetActive(true);
                    var audio = openDoor.GetComponent<AudioSource>();
                    audio.Play();
                    while (audio.isPlaying)
                    {
                        await Task.Yield();
                    }
                    openDoor.SetActive(false);
                    SceneManager.Instance.LoadScene("LivingRoom");
                    player.transform.position = new Vector3(16.86f, player.transform.position.y, player.transform.position.z);
                }
            };
            doorHandle.onMouseUp = () =>
            {
                doorHandle.transform.eulerAngles = Vector3.zero;
                isMouseDown = false;
            };

            
            var doorLock = FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/Lock");
            doorLock.AddComponent<Responder>().onMouseDown = async () =>
            {
                var key = FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/MouseHandle/Key");
                if (key.activeInHierarchy)
                {
                    key.SetActive(false);
                    isLocked = false;
                    FindUtility.Find("Environments/EntryRoom/Scene/NearDoor/CloseButton").GetComponent<Responder>().enable = true;
                }
                else if (isLocked)
                {
                    float z = 0;
                    while (z > -10f)
                    {
                        z -= 30f * Time.deltaTime;
                        z = Mathf.Clamp(z, -10, 0);
                        doorHandle.transform.eulerAngles = new Vector3(0, 0, z);
                        await Task.Yield();
                    }
                    while (z < 0f)
                    {
                        z += 30f * Time.deltaTime;
                        z = Mathf.Clamp(z, -10, 0);
                        doorHandle.transform.eulerAngles = new Vector3(0, 0, z);
                        await Task.Yield();
                    }
                }
                else
                {
                    if (!InventoryManager.Instance.Contains<Piece4>())
                        return;
                    float z = 0;
                    while (z > -90f)
                    {
                        z -= 30f * Time.deltaTime;
                        z = Mathf.Clamp(z, -90, 0);
                        doorHandle.transform.eulerAngles = new Vector3(0, 0, z);
                        await Task.Yield();
                    }
                    nearDoor.SetActive(false);
                    openDoor.SetActive(true);
                    mather.SetActive(true);
                    player.SetActive(true);
                    var audio = openDoor.GetComponent<AudioSource>();
                    audio.Play();
                    while (audio.isPlaying)
                    {
                        await Task.Yield();
                    }
                    SceneManager.Instance.LoadScene("LivingRoom");
                    player.transform.position = new Vector3(16.86f, player.transform.position.y, player.transform.position.z);
                }
            };
        }
    }
}
