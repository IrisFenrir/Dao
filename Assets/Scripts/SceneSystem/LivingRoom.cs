using Dao.CameraSystem;
using Dao.InventorySystem;
using Dao.WordSystem;
using System;
using System.Security.Cryptography;
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

            // 调查喜阳植物
            SetDialog("植物1", "LivingRoom-LightPlant");
            // 调查喜阴植物
            SetDialog("植物2", "LivingRoom-DarkPlant");
            // 调查光
            SetDialog("光1", "LivingRoom-Light");
            // 调查灯
            SetDialog("光2", "LivingRoom-Lamp");
            // 调查泥土
            Dirt();
            // 调查照片1
            Photo1();
            // 调查照片3
            Photo3();
            // 调查照片4
            Photo4();
            // 调查照片5
            Photo5();
            // 调查大植物
            SetDialog("植物3", "LivingRoom-BigPlant");
            // 调查沙发
            Sofa();
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
                CameraController.Instance.Enable = true;
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
            openButton.onClick.AddListener(async () =>
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
                }
                else
                {
                    var dialog = DialogUtility.GetDialog("LivingRoom-Box-Fail");
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                }
            });

            
            box.onMouseDown = async () =>
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
                    var dialog = DialogUtility.GetDialog("LivingRoom-Box");
                    UIDialogManager.Instance.StartDialog(dialog, false);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                }
                // 禁用响应
                responders.SetActive(false);
            };

            var tea = FindUtility.Find("Tea", innerBox.transform);
            var paper = FindUtility.Find("Paper", innerBox.transform);
            tea.AddComponent<UIResponder>().onMouseClick = () =>
            {
                tea.SetActive(false);
                // 添加道具 绿茶
                InventoryManager.Instance.AddItem(new Tea());
            };
            paper.AddComponent<UIResponder>().onMouseClick = () =>
            {
                paper.SetActive(false);
                // 添加道具
                InventoryManager.Instance.AddItem(new Piece2());
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
                UIDialogManager.Instance.StartDialog(DialogUtility.GetDialog("LivingRoom-NearDoor"));
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
                InventoryManager.Instance.AddItem(new Tea());
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
            bool isWinned = false;

            // 显示舞台
            FindUtility.Find("奖状", m_root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
                // 如果已经完成，只保留关闭界面的响应
                if (isWinned)
                {
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    root.GetComponent<Responder>().enable = true;
                }
            };

            // 关闭舞台
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };

            // 更换人物
            var people1 = FindUtility.Find("People1", root.transform).transform.GetChild(0).gameObject;
            var people2 = FindUtility.Find("People2", root.transform).transform.GetChild(0).gameObject;
            int people1Index = 0;
            int people2Index = 0;

            // 更换卡片
            var card1 = FindUtility.Find("Card1", root.transform);
            var card2 = FindUtility.Find("Card2", root.transform);
            var card3 = FindUtility.Find("Card3", root.transform);
            int card1Index = 0;
            int card2Index = 0;
            int card3Index = 0;

            // 中间人物
            var people1father = FindUtility.Find("Father", people1.transform);
            var people1mather = FindUtility.Find("Mather", people1.transform);
            var people1kid = FindUtility.Find("Kid", people1.transform);
            float[] people1height = new float[3] { people1father.transform.position.y, people1mather.transform.position.y, people1kid.transform.position.y};
            FindUtility.Find("Mather", people1.transform).transform.Translate(Vector3.down * 5f);
            FindUtility.Find("Kid", people1.transform).transform.Translate(Vector3.down * 5f);
            people1.AddComponent<Responder>().onMouseDown = async () =>
            {
                Transform[] people = new Transform[3] { people1father.transform, people1mather.transform, people1kid.transform };
                while (people1height[people1Index] - people[people1Index].transform.position.y < 5f)
                {
                    people[people1Index].transform.Translate(Vector3.down * 5f * Time.deltaTime);
                    await Task.Yield();
                }
                people1Index = (people1Index + 1) % 3;
                while (people1height[people1Index] > people[people1Index].transform.position.y)
                {
                    people[people1Index].transform.Translate(Vector3.up * 5f * Time.deltaTime);
                    await Task.Yield();
                }
                if (!isWinned && people1Index == 2 && people2Index == 1 && card1Index == 1 && card2Index == 0 && card3Index == 3)
                {
                    isWinned = true;
                    // 禁用操作
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    // 播放动画
                    var anim = root.GetComponent<Animation>();
                    anim.Play();
                    while (anim.isPlaying)
                        await Task.Yield();
                    // 掉落碎片
                    var piece = FindUtility.Find("Piece", root.transform).transform;
                    while (piece.position.y - (-1) > 0.001f)
                    {
                        piece.position = new Vector3(piece.position.x, Mathf.Lerp(piece.position.y, -1, 0.1f), piece.position.z);
                        await Task.Yield();
                    }
                    // 点击碎片
                    piece.gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        // 碎片添加到道具栏
                        InventoryManager.Instance.AddItem(new Piece1());
                        // 关闭碎片
                        piece.gameObject.SetActive(false);
                        // 开启响应
                        root.GetComponent<Responder>().enable = true;
                    };
                }
            };

            // 右侧人物
            var people2father = FindUtility.Find("Father", people2.transform);
            var people2mather = FindUtility.Find("Mather", people2.transform);
            var people2kid = FindUtility.Find("Kid", people2.transform);
            float[] people2height = new float[3] { people2father.transform.position.y, people2mather.transform.position.y, people2kid.transform.position.y };
            FindUtility.Find("Mather", people2.transform).transform.Translate(Vector3.down * 5f);
            FindUtility.Find("Kid", people2.transform).transform.Translate(Vector3.down * 5f);
            people2.AddComponent<Responder>().onMouseDown = async () =>
            {
                Transform[] people = new Transform[3] { people2father.transform, people2mather.transform, people2kid.transform };
                while (people2height[people2Index] - people[people2Index].transform.position.y < 5f)
                {
                    people[people2Index].transform.Translate(Vector3.down * 5f * Time.deltaTime);
                    await Task.Yield();
                }
                people2Index = (people2Index + 1) % 3;
                while (people2height[people2Index] > people[people2Index].transform.position.y)
                {
                    people[people2Index].transform.Translate(Vector3.up * 5f * Time.deltaTime);
                    await Task.Yield();
                }
                if (!isWinned && people1Index == 2 && people2Index == 1 && card1Index == 1 && card2Index == 0 && card3Index == 3)
                {
                    isWinned = true;
                    // 禁用操作
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    // 播放动画
                    var anim = root.GetComponent<Animation>();
                    anim.Play();
                    while (anim.isPlaying)
                        await Task.Yield();
                    // 掉落碎片
                    var piece = FindUtility.Find("Piece", root.transform).transform;
                    while (piece.position.y - (-1) > 0.001f)
                    {
                        piece.position = new Vector3(piece.position.x, Mathf.Lerp(piece.position.y, -1, 0.1f), piece.position.z);
                        await Task.Yield();
                    }
                    // 点击碎片
                    piece.gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        // 碎片添加到道具栏
                        InventoryManager.Instance.AddItem(new Piece1());
                        // 关闭碎片
                        piece.gameObject.SetActive(false);
                        // 开启响应
                        root.GetComponent<Responder>().enable = true;
                    };
                }
            };

            // 左侧卡片
            card1.AddComponent<Responder>().onMouseDown = async () =>
            {
                FindUtility.Find("Item" + (card1Index + 1).ToString(), card1.transform).SetActive(false);
                card1Index = (card1Index + 1) % 5;
                FindUtility.Find("Item" + (card1Index + 1).ToString(), card1.transform).SetActive(true);
                if (!isWinned && people1Index == 2 && people2Index == 1 && card1Index == 1 && card2Index == 0 && card3Index == 3)
                {
                    // 禁用操作
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    // 播放动画
                    var anim = root.GetComponent<Animation>();
                    anim.Play();
                    while (anim.isPlaying)
                        await Task.Yield();
                    // 掉落碎片
                    var piece = FindUtility.Find("Piece", root.transform).transform;
                    while (piece.position.y - (-1) > 0.001f)
                    {
                        piece.position = new Vector3(piece.position.x, Mathf.Lerp(piece.position.y, -1, 0.1f), piece.position.z);
                        await Task.Yield();
                    }
                    // 点击碎片
                    piece.gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        // 碎片添加到道具栏
                        InventoryManager.Instance.AddItem(new Piece1());
                        // 关闭碎片
                        piece.gameObject.SetActive(false);
                        // 开启响应
                        root.GetComponent<Responder>().enable = true;
                    };
                    isWinned = true;
                }
            };

            // 中间卡片
            card2.AddComponent<Responder>().onMouseDown = async () =>
            {
                FindUtility.Find("Item" + (card2Index + 1).ToString(), card2.transform).SetActive(false);
                card2Index = (card2Index + 1) % 5;
                FindUtility.Find("Item" + (card2Index + 1).ToString(), card2.transform).SetActive(true);
                if (!isWinned && people1Index == 2 && people2Index == 1 && card1Index == 1 && card2Index == 0 && card3Index == 3)
                {
                    // 禁用操作
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    // 播放动画
                    var anim = root.GetComponent<Animation>();
                    anim.Play();
                    while (anim.isPlaying)
                        await Task.Yield();
                    // 掉落碎片
                    var piece = FindUtility.Find("Piece", root.transform).transform;
                    while (piece.position.y - (-1) > 0.001f)
                    {
                        piece.position = new Vector3(piece.position.x, Mathf.Lerp(piece.position.y, -1, 0.1f), piece.position.z);
                        await Task.Yield();
                    }
                    // 点击碎片
                    piece.gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        // 碎片添加到道具栏
                        InventoryManager.Instance.AddItem(new Piece1());
                        // 关闭碎片
                        piece.gameObject.SetActive(false);
                        // 开启响应
                        root.GetComponent<Responder>().enable = true;
                    };
                    isWinned = true;
                }
            };

            // 右侧卡片
            card3.AddComponent<Responder>().onMouseDown = async () =>
            {
                FindUtility.Find("Item" + (card3Index + 1).ToString(), card3.transform).SetActive(false);
                card3Index = (card3Index + 1) % 5;
                FindUtility.Find("Item" + (card3Index + 1).ToString(), card3.transform).SetActive(true);
                if (!isWinned && people1Index == 2 && people2Index == 1 && card1Index == 1 && card2Index == 0 && card3Index == 3)
                {
                    // 禁用操作
                    Array.ForEach(root.GetComponentsInChildren<Responder>(), res => res.enable = false);
                    // 播放动画
                    var anim = root.GetComponent<Animation>();
                    anim.Play();
                    while (anim.isPlaying)
                        await Task.Yield();
                    // 掉落碎片
                    var piece = FindUtility.Find("Piece", root.transform).transform;
                    while (piece.position.y - (-1) > 0.001f)
                    {
                        piece.position = new Vector3(piece.position.x, Mathf.Lerp(piece.position.y, -1, 0.1f), piece.position.z);
                        await Task.Yield();
                    }
                    // 点击碎片
                    piece.gameObject.AddComponent<Responder>().onMouseDown = () =>
                    {
                        // 碎片添加到道具栏
                        InventoryManager.Instance.AddItem(new Piece1());
                        // 关闭碎片
                        piece.gameObject.SetActive(false);
                        // 开启响应
                        root.GetComponent<Responder>().enable = true;
                    };
                    isWinned = true;
                }
            };
        }

        // 打开医疗箱柜子
        public void OpenMedicalCase()
        {
            var responders = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders");
            var medicalCase = FindUtility.Find("MedicalCase", m_root.transform);
            var close = FindUtility.Find("Close", medicalCase.transform);
            var open = FindUtility.Find("Open", medicalCase.transform);
            close.SetActive(false);
            open.SetActive(true);

            var cases = FindUtility.Find("Case", open.transform);
            var image = cases.GetComponent<SpriteRenderer>();
            FindUtility.Find("医疗箱", open.transform).AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order = image.sortingOrder;
                image.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog("LivingRoom-MedicalCase");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image.sortingOrder = order;
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            };

            var paper = FindUtility.Find("Paper", open.transform);
            FindUtility.Find("纸条", open.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                paper.SetActive(false);
                // 纸条 放入道具栏
                InventoryManager.Instance.AddItem(new LivingRoomPaper());
            };
        }

        private void SetDialog(string itemName, string dialogID)
        {
            var responders = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders");
            var item = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/" + itemName);
            var image = FindUtility.Find("Environments/LivingRoom/Scene/Background/Base/" + itemName).GetComponent<SpriteRenderer>();
            item.AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order = image.sortingOrder;
                image.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog(dialogID);
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image.sortingOrder = order;
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            };
        }

        private void Dirt()
        {
            var responders = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders");
            var item1 = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/泥土1");
            var item2 = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/泥土2");
            var image1 = FindUtility.Find("Environments/LivingRoom/Scene/Background/Base/泥土1").GetComponent<SpriteRenderer>();
            var image2 = FindUtility.Find("Environments/LivingRoom/Scene/Background/Base/泥土2").GetComponent<SpriteRenderer>();
            item1.AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order1 = image1.sortingOrder;
                var order2 = image2.sortingOrder;
                image1.sortingOrder = 31;
                image2.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog("LivingRoom-Dirt");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image1.sortingOrder = order1;
                image2.sortingOrder = order2;
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            };
            item2.AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order1 = image1.sortingOrder;
                var order2 = image2.sortingOrder;
                image1.sortingOrder = 31;
                image2.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog("LivingRoom-Dirt");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image1.sortingOrder = order1;
                image2.sortingOrder = order2;
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            };
        }

        private void Photo1()
        {
            var background = FindUtility.Find("Environments/LivingRoom/Scene/Background");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Environments/LivingRoom/Scene/NearPhoto1");

            // 显示界面
            FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/照片1").AddComponent<Responder>().onMouseDown = async () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                player.SetActive(false);
                root.SetActive(true);
                root.GetComponent<Responder>().enable = false;
                var dialog = DialogUtility.GetDialog("LivingRoom-Photo1");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                root.GetComponent<Responder>().enable = true;
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                player.SetActive(true);
                root.SetActive(false);
            };
        }

        private void Photo3()
        {
            var background = FindUtility.Find("Environments/LivingRoom/Scene/Background");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Environments/LivingRoom/Scene/NearPhoto3");

            // 显示界面
            FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/照片3").AddComponent<Responder>().onMouseDown = async () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                player.SetActive(false);
                root.SetActive(true);
                root.GetComponent<Responder>().enable = false;
                var dialog = DialogUtility.GetDialog("LivingRoom-Photo3");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                root.GetComponent<Responder>().enable = true;
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                player.SetActive(true);
                root.SetActive(false);
            };
        }

        private void Photo4()
        {
            var background = FindUtility.Find("Environments/LivingRoom/Scene/Background");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Environments/LivingRoom/Scene/NearPhoto4");

            // 显示界面
            FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/照片4").AddComponent<Responder>().onMouseDown = async () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                player.SetActive(false);
                root.SetActive(true);
                root.GetComponent<Responder>().enable = false;
                var dialog = DialogUtility.GetDialog("LivingRoom-Photo4");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                root.GetComponent<Responder>().enable = true;
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                player.SetActive(true);
                root.SetActive(false);
            };
        }

        private void Photo5()
        {
            var background = FindUtility.Find("Environments/LivingRoom/Scene/Background");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Environments/LivingRoom/Scene/NearPhoto5");

            // 显示界面
            FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/照片5").AddComponent<Responder>().onMouseDown = async () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                CameraController.Instance.Enable = false;
                player.SetActive(false);
                root.SetActive(true);
                root.GetComponent<Responder>().enable = false;
                var dialog = DialogUtility.GetDialog("LivingRoom-Photo5");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                root.GetComponent<Responder>().enable = true;
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                CameraController.Instance.Enable = true;
                player.SetActive(true);
                root.SetActive(false);
            };

            // 泪水
            var tear = FindUtility.Find("Tear", root.transform);
            tear.AddComponent<Responder>().onMouseDown = async () =>
            {
                root.GetComponent<Responder>().enable = false;
                var dialog = DialogUtility.GetDialog("LivingRoom-Photo5-Tear");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                root.GetComponent<Responder>().enable = true;
            };
        }

        private void Sofa()
        {
            var player = FindUtility.Find("Player");
            var playerSit = FindUtility.Find("Environments/LivingRoom/Scene/Background/Base/主角坐下");
            var responders = FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders");
            var image = FindUtility.Find("Environments/LivingRoom/Scene/Background/Base/沙发").GetComponent<SpriteRenderer>();

            var dialog = DialogUtility.GetDialog("LivingRoom-Sofa");
            var select = DialogUtility.SearchSelectDialog(dialog);
            select.BindSelectAction(0, async () =>
            {
                player.SetActive(false);
                playerSit.SetActive(true);
                while (Input.GetAxis("Horizontal") == 0)
                    await Task.Yield();
                player.SetActive(true);
                playerSit.SetActive(false);
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            });
            select.BindSelectAction(1, () =>
            {
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            });

            FindUtility.Find("Environments/LivingRoom/Scene/Background/Responders/沙发").AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order = image.sortingOrder;
                image.sortingOrder = 31;
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image.sortingOrder = order;
            };
        }
    }
}
