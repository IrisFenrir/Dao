using Dao.CameraSystem;
using Dao.WordSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.SceneSystem
{
    public class LivingRoom : IScene
    {
        private GameObject m_root;

        public LivingRoom()
        {
            m_root = FindUtility.Find("LivingRoom");
            InitItems();
        }

        public override void Enable()
        {
            GameObject cameraSetting = FindUtility.Find("CameraSetting", m_root.transform);
            Bound2D bound = cameraSetting.transform.Find("Bound").GetComponent<Bound2D>();
            float screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            CameraController.Instance.MoveRange = new Vector2(bound.Rect.xMin + screenWidth / 2, bound.Rect.xMax - screenWidth / 2);
            CameraController.Instance.SetPosition(new Vector3(CameraController.Instance.MoveRange.x, 0, -10));
            CameraController.Instance.Enable = true;

            m_root.SetActive(true);
        }

        private void InitItems()
        {
            // 点击门
            Door();
            // 点击小木匣
            Box();
            // 点击小木箱
            Case();
            // 点击卡片
            Card();
            // 点击奖状
            Certificate();
        }

        private void Box()
        {
            bool[] answer = new bool[5] { true, true, false, true, false };
            bool[] state = new bool[5];
            GameObject puzzle_box = FindUtility.Find("Canvas/Puzzle_Box");
            GameObject outerBox = FindUtility.Find("OuterBox", puzzle_box.transform);
            GameObject innerBox = FindUtility.Find("InnerBox", puzzle_box.transform);
            bool isOpened = false;

            GameObject responders = FindUtility.Find("Responders", m_root.transform);
            Responder box = FindUtility.Find("小木匣", responders.transform).AddComponent<Responder>();

            puzzle_box.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 关闭UI
                puzzle_box.SetActive(false);
                // 开启响应
                responders.SetActive(true);
            });

            Transform switches = FindUtility.Find("Switch", puzzle_box.transform).transform;
            Transform images = FindUtility.Find("Images", puzzle_box.transform).transform;
            Sprite switchOn = GameLoop.Instance.boxSwitchOn;
            Sprite switchOff = GameLoop.Instance.boxSwitchOff;
            for (int i = 0; i < 5; i++)
            {
                Image image = images.GetChild(i).GetComponent<Image>();
                Button button = switches.GetChild(i).GetComponent<Button>();
                image.sprite = switchOff;
                int j = i;
                button.onClick.AddListener(() =>
                {
                    state[j] = !state[j];
                    if (state[j])
                        image.sprite = switchOn;
                    else
                        image.sprite = switchOff;
                });
            }

            
            Button openButton = FindUtility.Find("OpenButton", puzzle_box.transform).GetComponent<Button>();
            openButton.onClick.AddListener(() =>
            {
                bool result = true;
                for (int i = 0; i < 5; i++)
                {
                    result = result && (state[i] == answer[i]);
                }
                if (result)
                {
                    outerBox.SetActive(false);
                    innerBox.SetActive(true);
                    isOpened = true;
                    Debug.Log(isOpened);
                }
            });

            
            box.onMouseDown = () =>
            {
                // 禁用相机
                CameraController.Instance.Enable = false;
                // 打开UI
                puzzle_box.SetActive(true);
                if (isOpened)
                {
                    outerBox.SetActive(false);
                    innerBox.SetActive(true);
                }
                else
                {
                    outerBox.SetActive(true);
                    innerBox.SetActive(false);
                }
                // 禁用响应
                responders.SetActive(false);
            };
        }
    
        private void Door()
        {
            GameObject background = FindUtility.Find("Background", m_root.transform);

            GameObject responders = FindUtility.Find("Responders", m_root.transform);
            GameObject door = FindUtility.Find("门", responders.transform);

            GameObject nearDoor = FindUtility.Find("NearDoor", m_root.transform);
            GameObject doorHandle = FindUtility.Find("DoorHandle", nearDoor.transform);
            GameObject closeButton = FindUtility.Find("CloseButton", nearDoor.transform);

            nearDoor.SetActive(false);

            // 点击空白处关闭
            closeButton.AddComponent<Responder>().onMouseDown = () =>
            {
                // 隐藏近景，显示背景
                nearDoor.SetActive(false);
                background.SetActive(true);
                // 恢复响应
                responders.SetActive(true);
            };

            // 给门添加响应
            GameObject nearDoorResponders = FindUtility.Find("Responders", nearDoor.transform);
            Responder doorButton = door.AddComponent<Responder>();
            doorButton.onMouseDown = () =>
            {
                // 显示门的近景画面
                Rect screenRect = CameraController.Instance.GetScreenRect();
                Vector3 nearDoorSize = nearDoor.GetComponent<SpriteRenderer>().bounds.size;
                nearDoor.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                nearDoor.SetActive(true);
                background.SetActive(false);
                responders.SetActive(false);
                //DialogManager.Instance.StartDialog("客厅/门锁/0");
            };
            // 近景门
            UIWord uIWord = new UIWord(WordManager.Instance.GetWord("Open"), FindUtility.Find("UIWord", nearDoor.transform), FindUtility.Find("WorldCanvas/LivingRoom_Door_UIWord_Open"));

            // 给近景门锁添加响应
            doorHandle.AddComponent<Responder>().onMouseDown = async () =>
            {
                // 关闭近景响应
                nearDoorResponders.SetActive(false);
                // 显示对话
                UIDialogManager.Instance.StartDialog(DialogLoader.Instance.GetDialog("LivingRoom-NearDoor"));
                // 等待对话结束
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                // 打开近景响应，关闭近景门，显示背景
                nearDoorResponders.SetActive(true);
                nearDoor.SetActive(false);
                background.SetActive(true);
                responders.SetActive(true);
            };
        }

        private void Case()
        {
            var root = FindUtility.Find("Canvas/Puzzle_Case");
            var before = FindUtility.Find("Before", root.transform);
            var after = FindUtility.Find("After", root.transform);

            // 获取 Button 和 Image
            var button1 = FindUtility.Find("Button1", root.transform).GetComponent<Button>();
            var button2 = FindUtility.Find("Button2", root.transform).GetComponent<Button>();
            var button3 = FindUtility.Find("Button3", root.transform).GetComponent<Button>();
            var button4 = FindUtility.Find("Button4", root.transform).GetComponent<Button>();
            var image1 = FindUtility.Find("Image1", root.transform).GetComponent<Image>();
            var image2 = FindUtility.Find("Image2", root.transform).GetComponent<Image>();
            var image3 = FindUtility.Find("Image3", root.transform).GetComponent<Image>();
            var image4 = FindUtility.Find("Image4", root.transform).GetComponent<Image>();

            // 获取四张图
            var wind = WordManager.Instance.GetWord("Wind").image;
            var fire = WordManager.Instance.GetWord("Fire").image;
            var water = WordManager.Instance.GetWord("Water").image;
            var dirt = WordManager.Instance.GetWord("Dirt").image;
            Sprite[] sprites = new Sprite[4] { wind, fire, water, dirt };
            int[] spriteIndex = new int[4] { 0, 1, 2, 3 };

            // 初始化
            image1.sprite = wind;
            image2.sprite = fire;
            image3.sprite = water;
            image4.sprite = dirt;

            // 正确状态
            Action checkAction = () =>
            {
                if (spriteIndex[0] == 1 && spriteIndex[1] == 0 && spriteIndex[2] == 2 && spriteIndex[3] == 3)
                {
                    before.SetActive(false);
                    after.SetActive(true);
                }
            };

            // Button 事件
            button1.onClick.AddListener(() =>
            {
                spriteIndex[0] = (spriteIndex[0] + 1) % 3;
                image1.sprite = sprites[spriteIndex[0]];
                checkAction();
            });
            button2.onClick.AddListener(() =>
            {
                spriteIndex[1] = (spriteIndex[1] + 1) % 3;
                image2.sprite = sprites[spriteIndex[1]];
                checkAction();
            });
            button3.onClick.AddListener(() =>
            {
                spriteIndex[2] = (spriteIndex[2] + 1) % 3;
                image3.sprite = sprites[spriteIndex[2]];
                checkAction();
            });
            button4.onClick.AddListener(() =>
            {
                spriteIndex[3] = (spriteIndex[3] + 1) % 3;
                image4.sprite = sprites[spriteIndex[3]];
                checkAction();
            });

            // 打开箱子后
            var close = FindUtility.Find("Close", after.transform);
            var tea = FindUtility.Find("Tea", after.transform);
            close.GetComponent<Button>().onClick.AddListener(() =>
            {
                root.SetActive(false);
                m_root.SetActive(true);
                CameraController.Instance.Enable = true;
            });
            tea.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 绿茶添加到道具栏
                // to do
                tea.SetActive(false);
                root.SetActive(false);
                m_root.SetActive(true);
                CameraController.Instance.Enable = true;
            });

            // 点击场景 小木箱
            FindUtility.Find("箱子", m_root.transform).AddComponent<Responder>().onMouseDown = async () =>
            {
                await Task.Delay(100);
                m_root.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
            };
        }

        private void Card()
        {
            var root = FindUtility.Find("NearCard", m_root.transform);
            var background = FindUtility.Find("Background", m_root.transform);

            // 显示近景
            FindUtility.Find("卡片", m_root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                root.SetActive(true);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
            };

            // 关闭近景
            FindUtility.Find("Background", root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                root.SetActive(false);
                background.SetActive(true);
                CameraController.Instance.Enable = true;
            };

            // 点击卡片
            var cardBackground = FindUtility.Find("Background", root.transform);
            var image1 = FindUtility.Find("Image1", root.transform);
            var image2 = FindUtility.Find("Image2", root.transform);
            var image3 = FindUtility.Find("Image3", root.transform);
            var image4 = FindUtility.Find("Image4", root.transform);
            var card1 = FindUtility.Find("Card1", root.transform);
            var card2 = FindUtility.Find("Card2", root.transform);
            var card3 = FindUtility.Find("Card3", root.transform);
            var card4 = FindUtility.Find("Card4", root.transform);
            image1.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(false);
                card1.SetActive(true);
            };
            card1.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(true);
                card1.SetActive(false);
            };
            image2.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(false);
                card2.SetActive(true);
            };
            card2.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(true);
                card2.SetActive(false);
            };
            image3.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(false);
                card3.SetActive(true);
            };
            card3.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(true);
                card3.SetActive(false);
            };
            image4.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(false);
                card4.SetActive(true);
            };
            card4.AddComponent<Responder>().onMouseDown = () =>
            {
                cardBackground.SetActive(true);
                card4.SetActive(false);
            };
        }

        private void Certificate()
        {
            var background = FindUtility.Find("Background", m_root.transform);
            var root = FindUtility.Find("Stage", m_root.transform);

            // 显示舞台
            FindUtility.Find("奖状", m_root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
            };

            // 关闭舞台
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };

            var people1 = FindUtility.Find("People1", root.transform);
            var people2 = FindUtility.Find("People2", root.transform);

            int people1Index = 0;
            FindUtility.Find("Mather", people1.transform).transform.Translate(Vector3.down * 5f);
            FindUtility.Find("Kid", people1.transform).transform.Translate(Vector3.down * 5f);
            people1.AddComponent<Responder>().onMouseDown = async () =>
            {
                var father = FindUtility.Find("Father", people1.transform);
                var mather = FindUtility.Find("Mather", people1.transform);
                var kid = FindUtility.Find("Kid", people1.transform);
                Transform[] people = new Transform[3] { father.transform, mather.transform, kid.transform };
                float[] height = new float[3] { father.transform.position.y, mather.transform.position.y + 5, kid.transform.position.y + 5 };

                while (height[people1Index] - people[people1Index].transform.position.y < 5f)
                {
                    people[people1Index].transform.Translate(Vector3.down * 5f * Time.deltaTime);
                    await Task.Yield();
                }
                people1Index = (people1Index + 1) % 3;
                while (height[people1Index] > people[people1Index].transform.position.y)
                {
                    people[people1Index].transform.Translate(Vector3.up * 5f * Time.deltaTime);
                    await Task.Yield();
                }
            };
        }

        // 打开医疗箱柜子
        public void OpenMedicalCase()
        {
            var medicalCase = FindUtility.Find("MedicalCase", m_root.transform);
            var close = FindUtility.Find("Close", medicalCase.transform);
            var open = FindUtility.Find("Open", medicalCase.transform);
            close.SetActive(false);
            open.SetActive(true);
            var paper = FindUtility.Find("Paper", open.transform);
            FindUtility.Find("纸条", open.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                paper.SetActive(false);
                // 纸条 放入道具栏

            };
        }

        private void InitItem_OnlyWithDialog(string itemName, string dialogID)
        {
            GameObject sceneBase = FindUtility.Find("Base", m_root.transform);
            GameObject black = FindUtility.Find("Background_Black", sceneBase.transform);
            GameObject highlight = FindUtility.Find(itemName, sceneBase.transform);
            GameObject background = FindUtility.Find("Background", sceneBase.transform);

            GameObject responders = sceneBase.transform.Find("Responders").gameObject;
            GameObject target = FindUtility.Find(itemName, responders.transform);
            Responder button = target.AddComponent<Responder>();
            button.onMouseDown = async () =>
            {
                // 隐藏背景
                background.SetActive(false);
                // 显示亮图标
                highlight.SetActive(true);
                // 显示暗背景
                black.SetActive(true);
                // 关闭所有响应
                responders.SetActive(false);
                // 开启对话
                //DialogManager.Instance.StartDialog(dialogID);
                //// 等待对话结束
                //while (DialogManager.Instance.enable)
                //    await Task.Yield();
                // 恢复场景
                background.SetActive(true);
                highlight.SetActive(false);
                black.SetActive(false);
                responders.SetActive(true);
            };
        }
    }
}
