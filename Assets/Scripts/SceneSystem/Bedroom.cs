using Dao.CameraSystem;
using Dao.InventorySystem;
using Dao.WordSystem;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Dao.SceneSystem
{
    public class Bedroom : IScene
    {
        private GameObject m_root;

        private Bounds m_upperBounds;
        private Bounds m_leftBounds;
        private Bounds m_rightBounds;
        private bool m_mouseDown;
        private Vector3 m_mouseDownPosition;
        private bool m_clickUpper;
        private bool m_clickLeft;
        private bool m_clickRight;

        private bool m_isBoxOpened;

        private Transform m_noteMouseHandle;
        private int m_notePieceIndex = -1;
        private bool m_notePiece1Down;
        private bool m_notePiece2Down;
        private bool m_notePiece3Down;
        private bool m_notePiece4Down;

        private bool m_isDrawerLocked = true;

        private bool m_canShowInteractive = true;

        public Bedroom()
        {
            m_root = FindUtility.Find("Bedroom");
            Init();
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
            m_canShowInteractive = true;
        }

        public override void OnUpdate(float deltaTime)
        {
            #region 大箱子
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!m_mouseDown && Input.GetMouseButtonDown(0))
            {
                if (m_upperBounds.Contain(mousePosition))
                {
                    m_clickUpper = true;
                    m_mouseDown = true;
                    m_mouseDownPosition = mousePosition;
                }
                if (m_leftBounds.Contain(mousePosition))
                {
                    m_clickLeft = true;
                    m_mouseDown = true;
                    m_mouseDownPosition = mousePosition;
                }
                if (m_rightBounds.Contain(mousePosition))
                {
                    m_clickRight = true;
                    m_mouseDown = true;
                    m_mouseDownPosition = mousePosition;
                }
            }


            if (m_mouseDown)
            {
                
                if (m_clickUpper &&
                    mousePosition.x > m_mouseDownPosition.x && 
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 2f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(1, 0, 0)) < 10f)
                {
                    Upper_Right();
                    m_mouseDown = false;
                }
                else if (m_clickUpper &&
                    mousePosition.x < m_mouseDownPosition.x &&
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 2f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(-1, 0, 0)) < 10f)
                {
                    Upper_Left();
                    m_mouseDown = false;
                }
                else if (m_clickLeft &&
                    mousePosition.y < m_mouseDownPosition.y &&
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 1.5f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(0, -1, 0)) < 10f)
                {
                    Left_Down();
                    m_mouseDown = false;
                }
                else if (m_clickLeft &&
                    mousePosition.y > m_mouseDownPosition.y &&
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 1.5f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(0, 1, 0)) < 10f)
                {
                    Left_Up();
                    m_mouseDown = false;
                }
                else if (m_clickRight &&
                    mousePosition.y < m_mouseDownPosition.y &&
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 1f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(0, -1, 0)) < 10f)
                {
                    Right_Down();
                    m_mouseDown = false;
                }
                else if (m_clickRight &&
                    mousePosition.y > m_mouseDownPosition.y &&
                    Vector3.Distance(mousePosition, m_mouseDownPosition) > 1f &&
                    Vector3.Angle(mousePosition - m_mouseDownPosition, new Vector3(0, 1, 0)) < 10f)
                {
                    Right_Up();
                    m_mouseDown = false;
                }
            }



            if (Input.GetMouseButtonUp(0))
            {
                m_mouseDown = false;
                m_clickUpper = false;
                m_clickLeft = false;
                m_clickRight = false;
            }
            #endregion

            #region 日记本 鼠标跟随
            m_noteMouseHandle.position = new Vector3(mousePosition.x, mousePosition.y, m_noteMouseHandle.position.z);
            if (Input.GetMouseButtonDown(0) && m_notePieceIndex >= 0)
            {
                if (m_notePieceIndex == 0 && FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece1").GetComponent<BoxCollider2D>().bounds.Contain(mousePosition))
                {
                    var piece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece1");
                    piece.SetActive(true);
                    piece.GetComponent<SpriteRenderer>().enabled = true;
                    var movePiece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/MouseHandle/MovePiece1");
                    movePiece.transform.SetParent(m_noteMouseHandle.parent);
                    movePiece.SetActive(false);
                    m_notePieceIndex = -1;
                    m_notePiece1Down = true;
                    if (m_notePiece1Down && m_notePiece2Down && m_notePiece3Down && m_notePiece4Down)
                    {
                        NextPage();
                    }
                }
                else if (m_notePieceIndex == 1 && FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece2").GetComponent<BoxCollider2D>().bounds.Contain(mousePosition))
                {
                    var piece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece2");
                    piece.SetActive(true);
                    piece.GetComponent<SpriteRenderer>().enabled = true;
                    var movePiece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/MouseHandle/MovePiece2");
                    movePiece.transform.SetParent(m_noteMouseHandle.parent);
                    movePiece.SetActive(false);
                    m_notePieceIndex = -1;
                    m_notePiece2Down = true;
                    if (m_notePiece1Down && m_notePiece2Down && m_notePiece3Down && m_notePiece4Down)
                    {
                        NextPage();
                    }
                }
                else if (m_notePieceIndex == 2 && FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece3").GetComponent<BoxCollider2D>().bounds.Contain(mousePosition))
                {
                    var piece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece3");
                    piece.SetActive(true);
                    piece.GetComponent<SpriteRenderer>().enabled = true;
                    var movePiece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/MouseHandle/MovePiece3");
                    movePiece.transform.SetParent(m_noteMouseHandle.parent);
                    movePiece.SetActive(false);
                    m_notePieceIndex = -1;
                    m_notePiece3Down = true;
                    if (m_notePiece1Down && m_notePiece2Down && m_notePiece3Down && m_notePiece4Down)
                    {
                        NextPage();
                    }
                }
                else if (m_notePieceIndex == 3 && FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece4").GetComponent<BoxCollider2D>().bounds.Contain(mousePosition))
                {
                    var piece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/Piece4");
                    piece.SetActive(true);
                    piece.GetComponent<SpriteRenderer>().enabled = true;
                    var movePiece = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1/MouseHandle/MovePiece4");
                    movePiece.transform.SetParent(m_noteMouseHandle.parent);
                    movePiece.SetActive(false);
                    m_notePieceIndex = -1;
                    m_notePiece4Down = true;
                    if (m_notePiece1Down && m_notePiece2Down && m_notePiece3Down && m_notePiece4Down)
                    {
                        NextPage();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                MoveNotePiece1();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                MoveNotePiece2();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                MoveNotePiece3();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                MoveNotePiece4();
            }
            #endregion

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SceneManager.Instance.LoadScene("Kitchen");
            }

            if (Input.GetMouseButtonDown(2))
            {
                if (m_canShowInteractive && FindUtility.Find("Environments/Bedroom/Scene/Background").activeInHierarchy &&
                    !UIDialogManager.Instance.Enable)
                {
                    FindUtility.Find("Environments/Bedroom/Scene/Background/InteractiveItems").SetActive(true);
                }
            }
            else if (Input.GetMouseButtonUp(2))
            {
                FindUtility.Find("Environments/Bedroom/Scene/Background/InteractiveItems").SetActive(false);
            }
        }

        private void Init()
        {
            // 点击大箱子
            Box();
            // 点击抽屉
            Drawer();
            // 点击日记本
            Note();

            // 调查枯死的植物
            SetDialog("枯死的植物", "Bedroom-DeadPlant");
            // 调查电风扇
            SetDialog("电风扇", "Bedroom-ElectricFan");
            // 调查健康的植物
            SetDialog("健康的植物", "Bedroom-HealthyPlant");
            // 调查照片
            Photo();
            // 调查窗外
            Window();
        }

        private void Box()
        {
            var background = FindUtility.Find("Environments/Bedroom/Scene/Background");
            var player = FindUtility.Find("Player");

            var root = FindUtility.Find("Environments/Bedroom/Scene/Box");


            // 箱子打开后
            var boxOpened = FindUtility.Find("Environments/Bedroom/Scene/BoxOpened");
            boxOpened.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                boxOpened.SetActive(false);
            };
            // 拿碎片
            var piece = FindUtility.Find("Piece", boxOpened.transform);
            piece.AddComponent<Responder>().onMouseDown = () =>
            {
                piece.SetActive(false);
                // 添加道具
                InventoryManager.Instance.AddItem(new Piece1());
            };
            // 拿钥匙
            var key = FindUtility.Find("Key", boxOpened.transform);
            key.AddComponent<Responder>().onMouseDown = () =>
            {
                key.SetActive(false);
                // 添加道具
                InventoryManager.Instance.AddItem(new Key());
            };

            // 显示界面
            FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/大箱子").AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                background.SetActive(false);
                player.SetActive(false);
                CameraController.Instance.Enable = false;
                if (m_isBoxOpened)
                {
                    boxOpened.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                    boxOpened.SetActive(true);
                }
                else
                {
                    m_mouseDown = false;
                    root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                    root.SetActive(true);
                    // 识别鼠标手势
                    var upper = FindUtility.Find("Upper", root.transform);
                    var left = FindUtility.Find("Left", root.transform);
                    var right = FindUtility.Find("Right", root.transform);

                    m_upperBounds = upper.GetComponent<BoxCollider2D>().bounds;
                    m_leftBounds = left.GetComponent<BoxCollider2D>().bounds;
                    m_rightBounds = right.GetComponent<BoxCollider2D>().bounds;
                }
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };
        }
        private void BoxCheck()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid1").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid2").GetComponent<SpriteRenderer>();
            var grid3 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid3").GetComponent<SpriteRenderer>();
            var grid4 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid4").GetComponent<SpriteRenderer>();
            var grid5 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid5").GetComponent<SpriteRenderer>();
            var grid6 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid6").GetComponent<SpriteRenderer>();
            var grid7 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid7").GetComponent<SpriteRenderer>();

            if (grid1.sprite == WordManager.Instance.GetWord("Wind").image &&
                grid2.sprite == WordManager.Instance.GetWord("Water").image &&
                grid3.sprite == WordManager.Instance.GetWord("Water").image &&
                grid4.sprite == WordManager.Instance.GetWord("Dirt").image &&
                grid5.sprite == WordManager.Instance.GetWord("Father").image &&
                grid6.sprite == WordManager.Instance.GetWord("Mather").image &&
                grid7.sprite == WordManager.Instance.GetWord("Sugar").image)
            {
                m_isBoxOpened = true;
                FindUtility.Find("Environments/Bedroom/Scene/Box").SetActive(false);
                Rect screenRect = CameraController.Instance.GetScreenRect();
                var boxOpened = FindUtility.Find("Environments/Bedroom/Scene/BoxOpened");
                boxOpened.SetActive(true);
                boxOpened.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                boxOpened.SetActive(true);
            }
        }
        private void Upper_Right()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid1").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid2").GetComponent<SpriteRenderer>();
            var grid3 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid3").GetComponent<SpriteRenderer>();
            var grid4 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid4").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;
            var sprite3 = grid3.sprite;
            var sprite4 = grid4.sprite;

            grid1.sprite = sprite4;
            grid2.sprite = sprite1;
            grid3.sprite = sprite2;
            grid4.sprite = sprite3;

            BoxCheck();

            Debug.Log(111);
        }
        private void Upper_Left()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid1").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid2").GetComponent<SpriteRenderer>();
            var grid3 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid3").GetComponent<SpriteRenderer>();
            var grid4 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid4").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;
            var sprite3 = grid3.sprite;
            var sprite4 = grid4.sprite;

            grid1.sprite = sprite2;
            grid2.sprite = sprite3;
            grid3.sprite = sprite4;
            grid4.sprite = sprite1;

            BoxCheck();
        }
        private void Left_Down()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid1").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid5").GetComponent<SpriteRenderer>();
            var grid3 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid6").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;
            var sprite3 = grid3.sprite;

            grid1.sprite = sprite3;
            grid2.sprite = sprite1;
            grid3.sprite = sprite2;

            BoxCheck();
        }
        private void Left_Up()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid1").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid5").GetComponent<SpriteRenderer>();
            var grid3 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid6").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;
            var sprite3 = grid3.sprite;

            grid1.sprite = sprite2;
            grid2.sprite = sprite3;
            grid3.sprite = sprite1;

            BoxCheck();
        }
        private void Right_Down()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid4").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid7").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;

            grid1.sprite = sprite2;
            grid2.sprite = sprite1;

            BoxCheck();
        }
        private void Right_Up()
        {
            var grid1 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid4").GetComponent<SpriteRenderer>();
            var grid2 = FindUtility.Find("Environments/Bedroom/Scene/Box/Grid7").GetComponent<SpriteRenderer>();

            var sprite1 = grid1.sprite;
            var sprite2 = grid2.sprite;

            grid1.sprite = sprite2;
            grid2.sprite = sprite1;

            BoxCheck();
        }

        private void Drawer()
        {
            var background = FindUtility.Find("Environments/Bedroom/Scene/Background");
            var player = FindUtility.Find("Player");

            var root = FindUtility.Find("Environments/Bedroom/Scene/Drawer");

            var responders = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders");
            var item = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/抽屉");
            var image = FindUtility.Find("Environments/Bedroom/Scene/Background/Base/抽屉").GetComponent<SpriteRenderer>();

            // 显示界面
            FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/抽屉").AddComponent<Responder>().onMouseDown = async () =>
            {
                if (m_isDrawerLocked)
                {
                    responders.SetActive(false);
                    CameraController.Instance.Enable = false;
                    var order = image.sortingOrder;
                    image.sortingOrder = 31;
                    var dialog = DialogUtility.GetDialog("Bedroom-Drawer");
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    image.sortingOrder = order;
                    responders.SetActive(true);
                    CameraController.Instance.Enable = true;
                }
                else
                {
                    Rect screenRect = CameraController.Instance.GetScreenRect();
                    root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                    background.SetActive(false);
                    player.SetActive(false);
                    CameraController.Instance.Enable = false;
                    root.SetActive(true);
                }
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };

            // 点击纸片
            var paper = FindUtility.Find("Paper", root.transform);
            paper.AddComponent<Responder>().onMouseDown = () =>
            {
                paper.SetActive(false);
                // 添加道具

            };
        }

        private void Note()
        {
            var background = FindUtility.Find("Environments/Bedroom/Scene/Background");
            var player = FindUtility.Find("Player");

            var root = FindUtility.Find("Environments/Bedroom/Scene/Note");

            // 显示界面
            FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/日记本").AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                player.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };

            // 设置鼠标跟随
            m_noteMouseHandle = FindUtility.Find("MouseHandle", root.transform).transform;
        }
        public void MoveNotePiece1()
        {
            var root = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1");
            var movePiece1 = FindUtility.Find("MovePiece1", root.transform);

            movePiece1.SetActive(true);
            movePiece1.transform.SetParent(m_noteMouseHandle);
            movePiece1.transform.localPosition = Vector3.zero;
            m_notePieceIndex = 0;
        }
        public void MoveNotePiece2()
        {
            var root = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1");
            var movePiece1 = FindUtility.Find("MovePiece2", root.transform);

            movePiece1.SetActive(true);
            movePiece1.transform.SetParent(m_noteMouseHandle);
            movePiece1.transform.localPosition = Vector3.zero;
            m_notePieceIndex = 1;
        }
        public void MoveNotePiece3()
        {
            var root = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1");
            var movePiece1 = FindUtility.Find("MovePiece3", root.transform);

            movePiece1.SetActive(true);
            movePiece1.transform.SetParent(m_noteMouseHandle);
            movePiece1.transform.localPosition = Vector3.zero;
            m_notePieceIndex = 2;
        }
        public void MoveNotePiece4()
        {
            var root = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1");
            var movePiece1 = FindUtility.Find("MovePiece4", root.transform);

            movePiece1.SetActive(true);
            movePiece1.transform.SetParent(m_noteMouseHandle);
            movePiece1.transform.localPosition = Vector3.zero;
            m_notePieceIndex = 3;
        }
        private async void NextPage()
        {
            await Task.Delay(500);
            var pageEffect = FindUtility.Find("Environments/Bedroom/Scene/Note/PageEffect");
            pageEffect.SetActive(true);
            var page1 = FindUtility.Find("Environments/Bedroom/Scene/Note/Page1");
            var page2 = FindUtility.Find("Environments/Bedroom/Scene/Note/Page2");
            page2.SetActive(true);
            page2.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            await Task.Delay(500);
            page1.SetActive(false);
            pageEffect.SetActive(false);
            page2.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }

        private void SetDialog(string itemName, string dialogID)
        {
            var responders = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders");
            var item = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/" + itemName);
            var image = FindUtility.Find("Environments/Bedroom/Scene/Background/Base/" + itemName).GetComponent<SpriteRenderer>();
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

        private void Photo()
        {
            var background = FindUtility.Find("Environments/Bedroom/Scene/Background");
            var player = FindUtility.Find("Player");

            var root = FindUtility.Find("Environments/Bedroom/Scene/Photo");
            var colliders = root.GetComponentsInChildren<Collider2D>().ToList();

            // 显示界面
            FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/照片").AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                player.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
            };

            // 调查 眼
            FindUtility.Find("Eye", root.transform).AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Bedroom-Photo-Eye");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };

            // 调查 耳
            FindUtility.Find("Ear", root.transform).AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Bedroom-Photo-Ear");
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };
        }

        private void Window()
        {
            var responders = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders");
            var item = FindUtility.Find("Environments/Bedroom/Scene/Background/Responders/窗外");
            var image = FindUtility.Find("Environments/Bedroom/Scene/Background/Base/窗外").GetComponent<SpriteRenderer>();
            item.AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                CameraController.Instance.Enable = false;
                var order = image.sortingOrder;
                image.sortingOrder = 31;
                // 播放音效

                var dialog = DialogUtility.GetDialog("Bedroom-Window");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                image.sortingOrder = order;
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
            };
        }
    }
}
