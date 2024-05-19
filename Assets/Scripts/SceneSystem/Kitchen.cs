using Dao.CameraSystem;
using Dao.Common;
using Dao.InventorySystem;
using Dao.WordSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.SceneSystem
{
    public class Kitchen : IScene
    {
        private GameObject m_root;
        private bool m_isFireOpened;
        private GameObject m_mouseHandle;
        private bool m_isCookFinished;

        private bool m_canShowInteractive;

        private MoveTask m_moveTask = new();

        public Kitchen()
        {
            m_root = FindUtility.Find("Kitchen");
            m_mouseHandle = FindUtility.Find("Environments/Kitchen/Scene/Pot/MouseHandle");
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

        private void Init()
        {
            Background();

            // 点击炉灶
            Stove();
            // 点击锅
            Pot();
            
            // 点击鸟
            Bird();

            // 点击食物
            SetDialog("做好的饭", "Kitchen-Food");
            // 调查罐子
            SetDialog("罐子", "Kitchen-Can");
            // 调查小碗
            SetDialog("小碗", "Kitchen-SmallBowl");
            // 调查大碗
            SetDialog("大碗", "Kitchen-BigBowl");
            // 调查食物
            SetDialog("食物", "Kitchen-Food2");
            // 调查刀
            SetDialog("刀", "Kitchen-Knife");
            // 调查灯
            SetDialog("灯", "Kitchen-Lamp");
            // 调查枯死的植物
            SetDialog("枯死的植物", "Kitchen-DiedPlant");
            // 调查水壶
            SetDialog("水壶", "Kitchen-Kettle");
            // 调查盐罐
            SetDialog("盐罐", "Kitchen-Salt");
            // 调查糖罐
            SetDialog("糖罐", "Kitchen-Sugar");
            // 调查水龙头
            SetDialog("水龙头", "Kitchen-Faucet");
            // 调查小水杯
            SetDialog("小水杯", "Kitchen-SmallCup");
            // 调查红色苹果
            SetDialog("红色苹果", "Kitchen-RedApple");
            // 调查绿色苹果
            SetDialog("绿色苹果", "Kitchen-GreenApple");
            // 调查大锅
            SetDialog("大锅", "Kitchen-BigPot");
            // 调查小锅
            SetDialog("小锅", "Kitchen-SmallPot");
            // 调查大水杯
            BigCup();
            // 调查座椅
            Chair();

            // 调查绿茶
            Tea();
            // 调查墙上的纸条
            Paper();

            GoTo();
        }

        public override void OnUpdate(float deltaTime)
        {
            m_mouseHandle.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SceneManager.Instance.LoadScene("LivingRoom");
                SceneManager.Instance.GetScene<LivingRoom>("LivingRoom").OpenMedicalCase();
                if (m_isCookFinished)
                    ShowBird();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SceneManager.Instance.LoadScene("Bedroom");
                if (m_isCookFinished)
                    ShowBird();
            }

            if (Input.GetMouseButtonDown(2))
            {
                if (m_canShowInteractive)
                {
                    FindUtility.Find("Environments/Kitchen/Scene/Background/InteractiveItems").SetActive(true);
                }
            }
            else if (Input.GetMouseButtonUp(2))
            {
                FindUtility.Find("Environments/Kitchen/Scene/Background/InteractiveItems").SetActive(false);
            }

            m_moveTask.Update(deltaTime);
        }

        public void OpenFire()
        {
            m_isFireOpened = true;
            FindUtility.Find("Environments/Kitchen/Scene/Background/Base/灶火").SetActive(true);
        }

        public void CloseFire()
        {
            m_isFireOpened = false;
            FindUtility.Find("Environments/Kitchen/Scene/Background/Base/灶火").SetActive(false);
        }

        public void ShowBird()
        {
            FindUtility.Find("Environments/Kitchen/Scene/Background/Base/受伤的鸟").SetActive(true);
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/受伤的鸟").SetActive(true);
        }

        private void Background()
        {
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/背景").AddComponent<Responder>().onMouseDown = () =>
            {
                float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                m_moveTask.Start(x);
                m_moveTask.OnComplete = null;
            };
        }

        private void Stove()
        {
            var stove = FindUtility.Find("炉灶", m_root.transform);
            var stoveSwitch = FindUtility.Find("炉灶开关", m_root.transform);
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            bool firstClickClosed = false;
            bool firstClickOpened = false;

            stove.AddComponent<Responder>().onMouseDown = async () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(stove));
                m_moveTask.OnComplete = async () =>
                {
                    if (!firstClickClosed && !m_isFireOpened)
                    {
                        // 关闭响应
                        responders.SetActive(false);
                        m_canShowInteractive = false;
                        CameraController.Instance.Enable = false;
                        // 设置对话
                        var dialog = DialogUtility.GetDialog("Kitchen-Stove-Closed");
                        var select = dialog.Next[0].Next[0].Next[0].Next[0] as SelectDialog;
                        // 选择是
                        select.BindSelectAction(0, () =>
                        {
                            firstClickClosed = true;
                            OpenFire();
                        });
                        select.BindSelectAction(1, () =>
                        {
                            UIDialogManager.Instance.Close();
                        });
                        // 开启对话
                        CiphertextDialog.SetPosition(GameUtility.GetDialogPos(stove));
                        UIDialogManager.Instance.StartDialog(dialog);
                        while (UIDialogManager.Instance.Enable)
                            await Task.Yield();
                        CiphertextDialog.Reset();
                        // 开启响应
                        responders.SetActive(true);
                        m_canShowInteractive = true;
                        CameraController.Instance.Enable = true;
                    }
                    else if (!firstClickOpened && m_isFireOpened)
                    {
                        firstClickOpened = true;
                        // 关闭响应
                        responders.SetActive(false);
                        m_canShowInteractive = false;
                        CameraController.Instance.Enable = false;
                        // 开启对话
                        var dialog = DialogUtility.GetDialog("Kitchen-Fire-First");
                        var select = DialogUtility.SearchSelectDialog(dialog);
                        select.DisableOption(1);
                        CiphertextDialog.SetPosition(GameUtility.GetDialogPos(stove));
                        UIDialogManager.Instance.StartDialog(dialog);
                        while (UIDialogManager.Instance.Enable)
                            await Task.Yield();
                        CiphertextDialog.Reset();
                        // 妈妈向左走离开
                        var player = FindUtility.Find("Player");
                        var mather = FindUtility.Find("MatherModel");
                        mather.transform.position = new Vector3(player.transform.position.x, mather.transform.position.y, mather.transform.position.z);
                        mather.SetActive(true);
                        var matherAnim = mather.transform.Find("Model").GetComponent<Animator>();
                        matherAnim.Play("Mather_WalkRight");
                        mather.transform.localScale = new Vector3(-1, 1, 1);
                        float walkSpeed = 5f;
                        while (mather.transform.position.x > 46)
                        {
                            mather.transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                        // 客厅开启医疗箱
                        SceneManager.Instance.GetScene<LivingRoom>("LivingRoom").OpenMedicalCase();
                        // 主角移动到灶台前
                        var playerAnim = player.transform.Find("Model").GetComponent<Animator>();
                        if (player.transform.position.x > 61.2f)
                        {
                            playerAnim.Play("Player_WalkRight");
                            player.transform.localScale = new Vector3(-1, 1, 1);
                            while (player.transform.position.x > 61.2f)
                            {
                                player.transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
                                await Task.Yield();
                            }
                        }
                        else
                        {
                            playerAnim.Play("Player_WalkRight");
                            while (player.transform.position.x < 61.2f)
                            {
                                player.transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
                                await Task.Yield();
                            }
                        }
                        // 等待
                        playerAnim.Play("Player_Idle");
                        await Task.Delay(1000);
                        // 妈妈回来
                        matherAnim.Play("Mather_WalkRight");
                        mather.transform.localScale = new Vector3(1, 1, 1);
                        while (mather.transform.position.x < 59.2f)
                        {
                            mather.transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                        matherAnim.Play("Mather_Idle");
                        // 治疗过程
                        var healDialog = DialogUtility.GetDialog("Kitchen-Heal");
                        UIDialogManager.Instance.StartDialog(healDialog);
                        while (UIDialogManager.Instance.Enable)
                            await Task.Yield();
                        // 黑屏出现
                        var black = FindUtility.Find("Canvas/Black");
                        black.SetActive(true);
                        var blackImage = black.GetComponent<Image>();
                        blackImage.color = new Color(0, 0, 0, 0);
                        float a = 0;
                        while (a < 1)
                        {
                            a += 2f * Time.deltaTime;
                            a = Mathf.Clamp01(a);
                            blackImage.color = new Color(0, 0, 0, a);
                            await Task.Yield();
                        }
                        // 妈妈消失
                        mather.SetActive(false);
                        // 黑屏消失
                        a = 1;
                        while (a > 0)
                        {
                            a -= 2f * Time.deltaTime;
                            a = Mathf.Clamp01(a);
                            blackImage.color = new Color(0, 0, 0, a);
                            await Task.Yield();
                        }
                        black.SetActive(false);
                        // 恢复游戏
                        responders.SetActive(true);
                        m_canShowInteractive = true;
                        CameraController.Instance.Enable = true;
                    }
                    else if (m_isFireOpened)
                    {
                        // 关闭响应
                        responders.SetActive(false);
                        m_canShowInteractive = false;
                        CameraController.Instance.Enable = false;
                        // 开启对话
                        var dialog = DialogUtility.GetDialog("Kitchen-Fire-NotFirst");
                        CiphertextDialog.SetPosition(GameUtility.GetDialogPos(stove));
                        UIDialogManager.Instance.StartDialog(dialog);
                        while (UIDialogManager.Instance.Enable)
                            await Task.Yield();
                        CiphertextDialog.Reset();
                        // 开启响应
                        responders.SetActive(true);
                        m_canShowInteractive = true;
                        CameraController.Instance.Enable = true;
                    }
                };
            };

            stoveSwitch.AddComponent<Responder>().onMouseDown = async () =>
            {
                if (m_isFireOpened)
                {
                    CloseFire();
                }
                else
                {
                    // 关闭响应
                    responders.SetActive(false);
                    m_canShowInteractive = false;
                    CameraController.Instance.Enable = false;
                    // 设置对话
                    var dialog = DialogUtility.GetDialog("Kitchen-Stove-Closed");
                    var select = dialog.Next[0].Next[0].Next[0].Next[0] as SelectDialog;
                    // 选择是
                    select.BindSelectAction(0, () =>
                    {
                        OpenFire();
                    });
                    // 开启对话
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    // 开启响应
                    responders.SetActive(true);
                    m_canShowInteractive = true;
                    CameraController.Instance.Enable = true;
                }
            };
        }

        private void Pot()
        {
            var background = FindUtility.Find("Environments/Kitchen/Scene/Background");
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Pot", m_root.transform);

            // 在拖动食材过程中禁止关闭界面
            bool canClose = true;

            // 相关组件
            var fireSwitch = FindUtility.Find("Switch", root.transform);
            var fire = FindUtility.Find("Fire", root.transform);
            bool isFireOpened = false;
            var pot = root.transform.Find("Pot").gameObject;

            var salt = FindUtility.Find("Salt", root.transform);
            var sugar = FindUtility.Find("Sugar", root.transform);
            var water = FindUtility.Find("Water", root.transform);
            var red1 = FindUtility.Find("Red1", root.transform);
            var green1 = FindUtility.Find("Green1", root.transform);
            var blue1 = FindUtility.Find("Blue1", root.transform);
            var red2 = FindUtility.Find("Red2", root.transform);
            var green2 = FindUtility.Find("Green2", root.transform);
            var blue2 = FindUtility.Find("Blue2", root.transform);

            var saltPos = FindUtility.Find("SaltPos", root.transform);
            var sugarPos = FindUtility.Find("SugarPos", root.transform);
            var waterPos = FindUtility.Find("WaterPos", root.transform);
            var red1Pos = FindUtility.Find("Red1Pos", root.transform);
            var green1Pos = FindUtility.Find("Green1Pos", root.transform);
            var blue1Pos = FindUtility.Find("Blue1Pos", root.transform);
            var red2Pos = FindUtility.Find("Red2Pos", root.transform);
            var green2Pos = FindUtility.Find("Green2Pos", root.transform);
            var blue2Pos = FindUtility.Find("Blue2Pos", root.transform);

            var saltImage = FindUtility.Find("SaltImage", root.transform);
            var sugarImage = FindUtility.Find("SugarImage", root.transform);
            var waterImage = FindUtility.Find("WaterImage", root.transform);
            var red1Image = FindUtility.Find("Red1Image", root.transform);
            var gree1Image = FindUtility.Find("Green1Image", root.transform);
            var blue1Image = FindUtility.Find("Blue1Image", root.transform);
            var red2Image = FindUtility.Find("Red2Image", root.transform);
            var green2Image = FindUtility.Find("Green2Image", root.transform);
            var blue2Image = FindUtility.Find("Blue2Image", root.transform);

            var potSprite = FindUtility.Find("Environments/Kitchen/Scene/Background/Base/锅").GetComponent<SpriteRenderer>();

            Dictionary<GameObject, Vector3> itemPos = new()
            {
                { salt, salt.transform.localPosition },
                { sugar, sugar.transform.localPosition },
                { water, water.transform.localPosition },
                { red1, red1.transform.localPosition },
                { green1, green1.transform.localPosition },
                { blue1, blue1.transform.localPosition },
                { red2, red2.transform.localPosition },
                { green2, green2.transform.localPosition },
                { blue2, blue2.transform.localPosition }
            };

            // 定义操作顺序
            /* 
             * 开火 0 关火 1
             * 加盐 2 加糖 3 加水 4
             * 大红 5 大绿 6 大蓝 7
             * 小红 8 小绿 9 小蓝 10 
             */
            int[] order = new int[] { 0, 5, 4, 6, 2, 10, 1 };
            int currentStep = 0;
            List<int> realOrder = new();
            GameObject currentItem = null;

            // 烹饪失败
            Action cookFail = async () =>
            {
                // 关火
                fire.SetActive(false);
                isFireOpened = false;
                // 复位
                foreach (var item in itemPos)
                {
                    item.Key.SetActive(true);
                    item.Key.transform.localPosition = item.Value;
                    item.Key.GetComponent<BoxCollider2D>().enabled = true;
                }
                var position = FindUtility.Find("Position", root.transform).transform;
                for (int i = 0; i < position.childCount; i++)
                {
                    position.GetChild(i).gameObject.SetActive(false);
                }
                var image = FindUtility.Find("Image", root.transform).transform;
                for (int i = 0; i < image.childCount; i++)
                {
                    image.GetChild(i).gameObject.SetActive(false);
                }

                currentStep = 0;
                root.SetActive(false);
                background.SetActive(true);
                player.SetActive(true);
                responders.SetActive(false);
                m_canShowInteractive = false;
                int order = potSprite.sortingOrder;
                potSprite.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog("Kitchen-Cook-Fail");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                CiphertextDialog.Reset();
                potSprite.sortingOrder = order;
                responders.SetActive(true);
                m_canShowInteractive = true;
            };

            // 烹饪成功
            Action cookSuccess = async () =>
            {
                m_isCookFinished = true;
                // 关火
                fire.SetActive(false);
                isFireOpened = false;
                // 复位
                foreach (var item in itemPos)
                {
                    item.Key.transform.localPosition = item.Value;
                    item.Key.GetComponent<BoxCollider2D>().enabled = true;
                }
                var position = FindUtility.Find("Position").transform;
                for (int i = 0; i < position.childCount; i++)
                {
                    position.GetChild(i).gameObject.SetActive(false);
                }

                currentStep = 0;
                root.SetActive(false);
                background.SetActive(true);
                player.SetActive(true);
                responders.SetActive(false);
                m_canShowInteractive = false;
                int order = potSprite.sortingOrder;
                potSprite.sortingOrder = 31;
                var dialog = DialogUtility.GetDialog("Kitchen-Cook-Success");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                CiphertextDialog.Reset();
                potSprite.sortingOrder = order;

                // 食物出现
                FindUtility.Find("做好的饭", m_root.transform).SetActive(true);
                FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/做好的饭").SetActive(true);
                // 主角移动
                if (player.transform.position.x < 68.2f)
                {
                    player.GetComponentInChildren<Animator>().Play("Player_WalkRight");
                    while (player.transform.position.x < 68.2f)
                    {
                        player.transform.Translate(Vector3.right * 5f * Time.deltaTime);
                        await Task.Yield();
                    }
                }
                else if (player.transform.position.x > 70.2f)
                {
                    player.GetComponentInChildren<Animator>().Play("Player_WalkLeft");
                    while (player.transform.position.x > 70.2f)
                    {
                        player.transform.Translate(Vector3.left * 5f * Time.deltaTime);
                        await Task.Yield();
                    }
                }
                player.GetComponentInChildren<Animator>().Play("Player_Idle");
                responders.SetActive(true);
                m_canShowInteractive = true;
            };

            // 检查结果
            Action<int, Action, Action> check = (index, onSuccess, onFail) =>
            {
                realOrder.Add(index);
                if (realOrder.Count == order.Length)
                {
                    bool result = true;
                    for (int i = 0; i < order.Length; i++)
                    {
                        result = result && (realOrder[i] == order[i]);
                    }
                    realOrder.Clear();
                    if (result)
                    {
                        onSuccess?.Invoke();
                        cookSuccess();
                    }
                    else
                    {
                        onFail?.Invoke();
                        cookFail();
                    }  
                }
            };

            // 开火关火
            var fireSwitchSound = fireSwitch.GetComponent<AudioSource>();
            fireSwitch.AddComponent<Responder>().onMouseDown = () =>
            {
                if (isFireOpened)
                {
                    // 关火
                    fire.SetActive(false);
                    isFireOpened = false;
                    // 检查步骤
                    check(1, null, null);
                }
                else
                {
                    // 开火
                    fire.SetActive(true);
                    isFireOpened = true;
                    // 检查步骤
                    check(0, null, null);
                }
            };

            // 加盐
            salt.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                salt.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = salt;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                saltPos.SetActive(true);
            };
            saltPos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                salt.transform.SetParent(root.transform);
                salt.transform.localPosition = itemPos[salt];
                salt.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                saltPos.SetActive(false);
            };


            // 加糖
            sugar.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                sugar.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = sugar;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                sugarPos.SetActive(true);
            };
            sugarPos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                sugar.transform.SetParent(root.transform);
                sugar.transform.localPosition = itemPos[sugar];
                sugar.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                saltPos.SetActive(false);
            };

            // 加水
            water.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                water.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = water;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                waterPos.SetActive(true);
            };
            waterPos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                water.transform.SetParent(root.transform);
                water.transform.localPosition = itemPos[water];
                water.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                waterPos.SetActive(false);
            };

            // 加大红
            red1.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                red1.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = red1;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                red1Pos.SetActive(true);
            };
            red1Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                red1.transform.SetParent(root.transform);
                red1.transform.localPosition = itemPos[red1];
                red1.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                red1Pos.SetActive(false);
            };

            // 加大绿
            green1.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                green1.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = green1;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                green1Pos.SetActive(true);
            };
            green1Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                green1.transform.SetParent(root.transform);
                green1.transform.localPosition = itemPos[green1];
                green1.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                green1Pos.SetActive(false);
            };

            // 加大蓝
            blue1.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                blue1.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = blue1;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                blue1Pos.SetActive(true);
            };
            blue1Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                blue1.transform.SetParent(root.transform);
                blue1.transform.localPosition = itemPos[blue1];
                blue1.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                blue1Pos.SetActive(false);
            };

            // 加小红
            red2.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                red2.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = red2;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                red2Pos.SetActive(true);
            };
            red2Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                red2.transform.SetParent(root.transform);
                red2.transform.localPosition = itemPos[red2];
                red2.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                red2Pos.SetActive(false);
            };

            // 加小绿
            green2.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                green2.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = green2;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                green2Pos.SetActive(true);
            };
            green2Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                green2.transform.SetParent(root.transform);
                green2.transform.localPosition = itemPos[green2];
                green2.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                green2Pos.SetActive(false);
            };

            // 加小蓝
            blue2.AddComponent<Responder>().onMouseDown = () =>
            {
                // 关闭自身响应
                blue2.GetComponent<BoxCollider2D>().enabled = false;
                // 之前的食材复位
                if (currentItem != null)
                {
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    currentItem = null;
                    canClose = true;
                }
                // 选择当前食材
                currentItem = blue2;
                // 随鼠标移动
                currentItem.transform.SetParent(m_mouseHandle.transform);
                canClose = false;
                blue2Pos.SetActive(true);
            };
            blue2Pos.AddComponent<Responder>().onMouseDown = () =>
            {
                // 复位
                blue2.transform.SetParent(root.transform);
                blue2.transform.localPosition = itemPos[blue2];
                blue2.GetComponent<BoxCollider2D>().enabled = true;
                currentItem = null;
                canClose = true;
                blue2Pos.SetActive(false);
            };


            // 点击锅内
            pot.AddComponent<Responder>().onMouseDown = () =>
            {
                if (currentItem == null) return;
                // 盐、糖、水
                if (currentItem == salt || currentItem == sugar || currentItem == water)
                {
                    // 复位
                    currentItem.transform.SetParent(root.transform);
                    currentItem.transform.localPosition = itemPos[currentItem];
                    currentItem.GetComponent<BoxCollider2D>().enabled = true;
                    // 效果
                    if (currentItem == salt)
                    {
                        saltImage.SetActive(true);
                    }
                    else if (currentItem == sugar)
                    {
                        sugarImage.SetActive(true);
                    }
                    else if (currentItem == water)
                    {
                        waterImage.SetActive(true);
                    }
                    // 检查步骤
                    int cur = 0;
                    if (currentItem == salt) cur = 2;
                    else if (currentItem == sugar) cur = 3;
                    else if (currentItem == water) cur = 4;
                    currentItem = null;
                    check(cur, null, null);
                }
                // 其他食物
                else
                {
                    // 放下食物
                    currentItem.transform.SetParent(root.transform);
                    if (currentItem == red1) red1Pos.SetActive(false);
                    else if (currentItem == green1) green1Pos.SetActive(false);
                    else if (currentItem == blue1) blue1Pos.SetActive(false);
                    else if (currentItem == red2) red2Pos.SetActive(false);
                    else if (currentItem == green2) green2Pos.SetActive(false);
                    else if (currentItem == blue2) blue2Pos.SetActive(false);
                    
                    // 显示效果
                    currentItem.SetActive(false);
                    if(currentItem == red1)
                    {
                        red1Image.SetActive(true);
                    }
                    else if (currentItem == green1)
                    {
                        gree1Image.SetActive(true);
                    }
                    else if (currentItem == blue1)
                    {
                        blue1Image.SetActive(true);
                    }
                    else if(currentItem == red2)
                    {
                        red2Image.SetActive(true);
                    }
                    else if (currentItem == green2)
                    {
                        green2Image.SetActive(true);
                    }
                    else if (currentItem == blue2)
                    {
                        blue2Image.SetActive(true);
                    }
                    
                    // 检查步骤
                    int cur = 0;
                    if (currentItem == red1) cur = 5;
                    else if (currentItem == green1) cur = 6;
                    else if (currentItem == blue1) cur = 7;
                    else if (currentItem == red2) cur = 8;
                    else if (currentItem == green2) cur = 9;
                    else if (currentItem == blue2) cur = 10;
                    currentItem = null;
                    check(cur, () =>
                    {
                        cookSuccess();
                    },
                    () =>
                    {
                        currentItem.transform.SetParent(root.transform);
                        currentItem.transform.localPosition = itemPos[currentItem];
                        currentItem.GetComponent<BoxCollider2D>().enabled = true;
                        //canClose = true;
                        cookFail();
                    });
                }
            };

            var dialog = DialogUtility.GetDialog("Kitchen-Pot");
            var select = DialogUtility.SearchSelectDialog(dialog);
            select.Next[0].onStop = async () =>
            {
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                CiphertextDialog.Reset();
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                player.SetActive(false);
                root.SetActive(true);
                canClose = true;
            };
            select.BindSelectAction(1, () =>
            {
                UIDialogManager.Instance.Close();
            });
            

            // 显示界面
            var potResponder = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/锅");
            
            potResponder.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(potResponder));
                m_moveTask.OnComplete = async () =>
                {
                    responders.SetActive(false);
                    m_canShowInteractive = false;
                    int order = potSprite.sortingOrder;
                    potSprite.sortingOrder = 31;
                    CiphertextDialog.SetPosition(GameUtility.GetDialogPos(potResponder));
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    CiphertextDialog.Reset();
                    potSprite.sortingOrder = order;
                    responders.SetActive(true);
                    m_canShowInteractive = true;
                };
            };

            // 关闭界面
            FindUtility.Find("Close", root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                if (!canClose) return;
                background.SetActive(true);
                player.SetActive(true);
                root.SetActive(false);
                responders.SetActive(true);
                m_canShowInteractive = true;
            };
        }

        private void Food()
        {
            var responders = FindUtility.Find("Responders", m_root.transform);
            var player = FindUtility.Find("Player");

            var food = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/做好的饭");
            food.AddComponent<Responder>().onMouseDown = async () =>
            {
                responders.SetActive(false);
                m_canShowInteractive = false;
                var dialog = DialogUtility.GetDialog("Kitchen-Food");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                responders.SetActive(true);
                m_canShowInteractive = true;
            };
        }

        private void Bird()
        {
            var background = FindUtility.Find("Environments/Kitchen/Scene/Background");
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Bird", m_root.transform);
            var birdResponders = FindUtility.Find("Responders", root.transform);

            bool canClose = true;

            // 显示界面
            var bird = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/受伤的鸟");
            bird.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(bird));
                m_moveTask.OnComplete = () =>
                {
                    Rect screenRect = CameraController.Instance.GetScreenRect();
                    root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                    responders.SetActive(false);
                    m_canShowInteractive = false;
                    root.SetActive(true);
                };
            };

            // 关闭界面
            FindUtility.Find("Close", root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                if (!canClose) return;
                responders.SetActive(true);
                m_canShowInteractive = true;
                root.SetActive(false);
            };

            var piece = FindUtility.Find("Piece", root.transform);
            var food = FindUtility.Find("Food", root.transform);
            var body = FindUtility.Find("Body", root.transform);
            var hurt = FindUtility.Find("Hurt", root.transform);

            // 调查碎片
            piece.AddComponent<Responder>().onMouseDown = () =>
            {
                // 获得道具
                InventoryManager.Instance.AddItem(new Piece3());
                FindUtility.Find("信纸", root.transform).SetActive(false);
            };

            // 调查食物
            food.AddComponent<Responder>().onMouseDown = async () =>
            {
                canClose = false;
                birdResponders.SetActive(false);
                CameraController.Instance.Enable = false;
                var dialog = DialogUtility.GetDialog("Kitchen-Bird-Food");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(food));
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                birdResponders.SetActive(true);
                CameraController.Instance.Enable = false;
                canClose = true;
            };

            // 调查身体
            body.AddComponent<Responder>().onMouseDown = async () =>
            {
                canClose = false;
                birdResponders.SetActive(false);
                CameraController.Instance.Enable = false;
                var dialog = DialogUtility.GetDialog("Kitchen-Bird-Body");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(body));
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                birdResponders.SetActive(true);
                CameraController.Instance.Enable = false;
                canClose = true;
            };

            // 调查伤口
            hurt.AddComponent<Responder>().onMouseDown = async () =>
            {
                canClose = false;
                birdResponders.SetActive(false);
                CameraController.Instance.Enable = false;
                var dialog = DialogUtility.GetDialog("Kitchen-Bird-Hurt");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(hurt));
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                birdResponders.SetActive(true);
                CameraController.Instance.Enable = false;
                canClose = true;
            };
        }

        private void SetDialog(string itemName, string dialogID)
        {
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            var item = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/" + itemName);
            var image = FindUtility.Find("Environments/Kitchen/Scene/Background/Base/" + itemName).GetComponent<SpriteRenderer>();
            item.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(item));
                m_moveTask.OnComplete = async () =>
                {
                    responders.SetActive(false);
                    m_canShowInteractive = false;
                    CameraController.Instance.Enable = false;
                    var order = image.sortingOrder;
                    image.sortingOrder = 31;
                    var dialog = DialogUtility.GetDialog(dialogID);
                    CiphertextDialog.SetPosition(GameUtility.GetDialogPos(item));
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    image.sortingOrder = order;
                    responders.SetActive(true);
                    m_canShowInteractive = true;
                    CameraController.Instance.Enable = true;
                };
            };
        }

        private void BigCup()
        {
            var background = FindUtility.Find("Environments/Kitchen/Scene/Background");
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            var player = FindUtility.Find("Player");
            var root = FindUtility.Find("Environments/Kitchen/Scene/BigCup");
            var colliders = root.GetComponentsInChildren<Collider2D>().ToList();

            // 打开界面
            var bottle = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/大水杯");
            bottle.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(bottle));
                m_moveTask.OnComplete = () =>
                {
                    Rect screenRect = CameraController.Instance.GetScreenRect();
                    root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                    m_canShowInteractive = false;
                    responders.SetActive(false);
                    root.SetActive(true);
                };
            };

            // 关闭界面
            FindUtility.Find("Close", root.transform).AddComponent<Responder>().onMouseDown = () =>
            {
                m_canShowInteractive = true;
                responders.SetActive(true);
                root.SetActive(false);
            };

            // 点击水杯
            var body = FindUtility.Find("Body", root.transform);
            body.AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Kitchen-BigCup-Body");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(body));
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };

            // 点击红光
            var red = FindUtility.Find("Red", root.transform);
            red.AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Kitchen-BigCup-Red");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(red));
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };

            // 点击绿光
            var green = FindUtility.Find("Green", root.transform);
            green.AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Kitchen-BigCup-Green");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(green));
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };

            // 点击蓝光
            var blue = FindUtility.Find("Blue", root.transform);
            blue.AddComponent<Responder>().onMouseDown = async () =>
            {
                colliders.ForEach(c => c.enabled = false);
                var dialog = DialogUtility.GetDialog("Kitchen-BigCup-Blue");
                CiphertextDialog.SetPosition(GameUtility.GetDialogPos(blue));
                UIDialogManager.Instance.StartDialog(dialog, false);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                colliders.ForEach(c => c.enabled = true);
            };
        }

        private void Chair()
        {
            var player = FindUtility.Find("Player");
            var playerSit = FindUtility.Find("Environments/Kitchen/Scene/Background/Base/主角坐下");
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");

            var chair1 = FindUtility.Find("座椅1", responders.transform);
            var chair1Image = FindUtility.Find("Environments/Kitchen/Scene/Background/Base/座椅1").GetComponent<SpriteRenderer>();
            var chair2 = FindUtility.Find("座椅2", responders.transform);
            var chair2Image = FindUtility.Find("Environments/Kitchen/Scene/Background/Base/座椅2").GetComponent<SpriteRenderer>();

            var dialog = DialogUtility.GetDialog("Kitchen-Chair");
            var select = DialogUtility.SearchSelectDialog(dialog);
            select.BindSelectAction(0, async () =>
            {
                UIDialogManager.Instance.Close();
                player.SetActive(false);
                playerSit.SetActive(true);
                while (!Input.GetMouseButtonDown(0))
                    await Task.Yield();
                player.SetActive(true);
                playerSit.SetActive(false);
                responders.SetActive(true);
                m_canShowInteractive = true;
            });
            select.BindSelectAction(1, () =>
            {
                UIDialogManager.Instance.Close();
                responders.SetActive(true);
                m_canShowInteractive = true;
            });

            Action sit = () =>
            {
                m_moveTask.Start(GameUtility.GetPlayerPos(chair1));
                m_moveTask.OnComplete = async () =>
                {
                    responders.SetActive(false);
                    m_canShowInteractive = false;
                    var order1 = chair1Image.sortingOrder;
                    var order2 = chair2Image.sortingOrder;
                    chair1Image.sortingOrder = 31;
                    chair2Image.sortingOrder = 31;
                    CiphertextDialog.SetPosition(GameUtility.GetDialogPos(chair1));
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    chair1Image.sortingOrder = order1;
                    chair2Image.sortingOrder = order2;
                };
            };

            chair1.AddComponent<Responder>().onMouseDown = sit;
            chair2.AddComponent<Responder>().onMouseDown = sit;
        }

        private void Tea()
        {
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/绿茶").AddComponent<Responder>().onMouseDown = () =>
            {
                FindUtility.Find("Environments/Kitchen/Scene/Background/Base/绿茶").SetActive(false);
                // 添加道具
                InventoryManager.Instance.AddItem(new Tea());
            };
        }

        private void Paper()
        {
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/纸条").AddComponent<Responder>().onMouseDown = () =>
            {
                FindUtility.Find("Environments/Kitchen/Scene/Background/Base/纸条").SetActive(false);
                // 添加道具
                InventoryManager.Instance.AddItem(new Menu());
            };
        }

        private void GoTo()
        {
            var livingRoom = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/客厅");
            var bedRoom = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/卧室");
            var player = FindUtility.Find("Player");

            livingRoom.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(livingRoom.transform.position.x);
                m_moveTask.OnComplete = async () =>
                {
                    await GameUtility.Transition(() =>
                    {
                        SceneManager.Instance.LoadScene("LivingRoom");
                        player.transform.position = new Vector3(39.24f, -1.91f, 0);
                        if (m_isCookFinished)
                            ShowBird();
                    });
                };
            };

            bedRoom.AddComponent<Responder>().onMouseDown = () =>
            {
                m_moveTask.Start(bedRoom.transform.position.x);
                m_moveTask.OnComplete = async () =>
                {
                    await GameUtility.Transition(() =>
                    {
                        SceneManager.Instance.LoadScene("Bedroom");
                        player.transform.position = new Vector3(90.86f, -1.91f, 0);
                        if (m_isCookFinished)
                            ShowBird();
                    });
                };
            };
        }
    }
}
