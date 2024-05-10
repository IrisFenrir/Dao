﻿using Dao.CameraSystem;
using Dao.WordSystem;
using System;
using System.Collections.Generic;
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
        }

        private void Init()
        {
            // 点击炉灶
            Stove();
            // 点击锅
            Pot();
            // 点击食物
            Food();
            // 点击鸟
            Bird();
        }

        public override void OnUpdate(float deltaTime)
        {
            m_mouseHandle.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

        private void Stove()
        {
            var stove = FindUtility.Find("炉灶", m_root.transform);
            var stoveSwitch = FindUtility.Find("炉灶开关", m_root.transform);
            var responders = FindUtility.Find("Environments/Kitchen/Scene/Background/Responders");
            bool firstClickClosed = false;
            bool firstClickOpened = false;

            stove.AddComponent<Responder>().onMouseDown = async () =>
            {
                if (!firstClickClosed && !m_isFireOpened)
                {
                    // 关闭响应
                    responders.SetActive(false);
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
                    // 开启对话
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    // 开启响应
                    responders.SetActive(true);
                    CameraController.Instance.Enable = true;
                }
                else if (!firstClickOpened && m_isFireOpened)
                {
                    firstClickOpened = true;
                    // 关闭响应
                    responders.SetActive(false);
                    CameraController.Instance.Enable = false;
                    // 开启对话
                    var dialog = DialogUtility.GetDialog("Kitchen-Fire-First");
                    var select = DialogUtility.SearchSelectDialog(dialog);
                    select.DisableOption(1);
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    // 妈妈向左走离开
                    var player = FindUtility.Find("Player");
                    var mather = FindUtility.Find("MatherModel");
                    mather.transform.position = new Vector3(player.transform.position.x, mather.transform.position.y, mather.transform.position.z);
                    mather.SetActive(true);
                    var matherAnim = mather.transform.Find("Model").GetComponent<Animator>();
                    matherAnim.Play("Mather_WalkLeft");
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
                        while (player.transform.position.x > 61.2f)
                        {
                            player.transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                    }
                    else
                    {
                        while (player.transform.position.x < 61.2f)
                        {
                            player.transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                    }
                    // 等待
                    await Task.Delay(1000);
                    // 妈妈回来
                    matherAnim.Play("Mather_WalkRight");
                    while (mather.transform.position.x < 59.2f)
                    {
                        mather.transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
                        await Task.Yield();
                    }
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
                    CameraController.Instance.Enable = true;
                }
                else if (m_isFireOpened)
                {
                    // 关闭响应
                    responders.SetActive(false);
                    CameraController.Instance.Enable = false;
                    // 开启对话
                    var dialog = DialogUtility.GetDialog("Kitchen-Fire-NotFirst");
                    UIDialogManager.Instance.StartDialog(dialog);
                    while (UIDialogManager.Instance.Enable)
                        await Task.Yield();
                    // 开启响应
                    responders.SetActive(true);
                    CameraController.Instance.Enable = true;
                }
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
             * 开 火 源头
             * 大 红色 食物
             * 大 蓝色 食物
             * 小 绿色 食物
             * 前 水
             * 前 盐
             * 大 绿色 食物
             * 小 蓝色 食物
             * 前 水
             * 关 火 源头
             * 前 糖
             * 小 红色 食物 
             */
            int[] order = new int[] { 0, 5, 7, 9, 4, 2, 6, 10, 4, 1, 3, 8 };
            int currentStep = 0;
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
                var dialog = DialogUtility.GetDialog("Kitchen-Cook-Fail");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                responders.SetActive(true);
                CameraController.Instance.Enable = true;
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
                var dialog = DialogUtility.GetDialog("Kitchen-Cook-Success");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();

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
            };

            // 开火关火
            fireSwitch.AddComponent<Responder>().onMouseDown = () =>
            {
                if (isFireOpened)
                {
                    // 关火
                    fire.SetActive(false);
                    isFireOpened = false;
                    // 检查步骤
                    if (order[currentStep] == 1)
                    {
                        currentStep++;
                    }
                    else
                    {
                        cookFail();
                    }
                }
                else
                {
                    // 开火
                    fire.SetActive(true);
                    isFireOpened = true;
                    // 检查步骤
                    if (order[currentStep] == 0)
                    {
                        currentStep++;
                    }
                    else
                    {
                        cookFail();
                    }
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
                    canClose = true;
                    // 检查步骤
                    if ((currentItem == salt && order[currentStep] == 2) || 
                        (currentItem == sugar && order[currentStep] == 3) ||
                        (currentItem == water && order[currentStep] == 4))
                    {
                        currentStep++;
                        currentItem = null;
                        if (currentStep == 12)
                            cookSuccess();
                    }
                    else
                    {
                        currentItem = null;
                        cookFail();
                    }
                }
                // 其他食物
                else
                {
                    // 检查步骤
                    if ((currentItem == red1 && order[currentStep] == 5) ||
                        (currentItem == green1 && order[currentStep] == 6) ||
                        (currentItem == blue1 && order[currentStep] == 7) ||
                        (currentItem == red2 && order[currentStep] == 8) ||
                        (currentItem == green2 && order[currentStep] == 9) ||
                        (currentItem == blue2 && order[currentStep] == 10))
                    {
                        currentStep++;
                        // 放入锅中
                        currentItem.transform.SetParent(root.transform);
                        if (currentItem == red1) red1Pos.SetActive(false);
                        else if (currentItem == green1) green1Pos.SetActive(false);
                        else if (currentItem == blue1) blue1Pos.SetActive(false);
                        else if (currentItem == red2) red2Pos.SetActive(false);
                        else if (currentItem == green2) green2Pos.SetActive(false);
                        else if (currentItem == blue2) blue2Pos.SetActive(false);
                        currentItem = null;
                        if (currentStep == 12)
                            cookSuccess();
                    }
                    else
                    {
                        currentItem.transform.SetParent(root.transform);
                        currentItem.transform.localPosition = itemPos[currentItem];
                        currentItem.GetComponent<BoxCollider2D>().enabled = true;
                        currentItem = null;
                        canClose = true;
                        cookFail();
                    }
                }
            };

            // 显示界面
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/锅").AddComponent<Responder>().onMouseDown = async () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                player.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
                canClose = true;
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                if (!canClose) return;
                background.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
                root.SetActive(false);
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
                var dialog = DialogUtility.GetDialog("Kitchen-Food");
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                responders.SetActive(true);
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
            FindUtility.Find("Environments/Kitchen/Scene/Background/Responders/受伤的鸟").AddComponent<Responder>().onMouseDown = () =>
            {
                Rect screenRect = CameraController.Instance.GetScreenRect();
                root.transform.position = new Vector3(screenRect.x + (screenRect.width) / 2, 0, 0);
                background.SetActive(false);
                responders.SetActive(false);
                player.SetActive(false);
                CameraController.Instance.Enable = false;
                root.SetActive(true);
            };

            // 关闭界面
            root.AddComponent<Responder>().onMouseDown = () =>
            {
                if (!canClose) return;
                background.SetActive(true);
                responders.SetActive(true);
                player.SetActive(true);
                CameraController.Instance.Enable = true;
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
            };

            // 调查食物
            food.AddComponent<Responder>().onMouseDown = async () =>
            {
                canClose = false;
                birdResponders.SetActive(false);
                CameraController.Instance.Enable = false;
                var dialog = DialogUtility.GetDialog("Kitchen-Bird-Food");
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
                UIDialogManager.Instance.StartDialog(dialog);
                while (UIDialogManager.Instance.Enable)
                    await Task.Yield();
                birdResponders.SetActive(true);
                CameraController.Instance.Enable = false;
                canClose = true;
            };
        }
    }
}
