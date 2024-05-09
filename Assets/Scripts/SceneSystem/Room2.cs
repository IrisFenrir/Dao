﻿using Dao.CameraSystem;
using UnityEngine;

namespace Dao.SceneSystem
{
    public class Room2 : IScene
    {
        public override void OnEnter()
        {
            GameObject root = FindUtility.Find("Room2");

            GameObject cameraSetting = FindUtility.Find("CameraSetting", root.transform);
            Bound2D bound = cameraSetting.transform.Find("Bound").GetComponent<Bound2D>();
            float screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            CameraController.Instance.MoveRange = new Vector2(bound.Rect.xMin + screenWidth / 2, bound.Rect.xMax - screenWidth / 2);
            CameraController.Instance.SetPosition(new Vector3(CameraController.Instance.MoveRange.x, 0, -10));
            CameraController.Instance.Enable = true;

            root.SetActive(true);
        }
    }
}