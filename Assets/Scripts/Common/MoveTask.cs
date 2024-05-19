using System;
using UnityEngine;

namespace Dao
{
    public class MoveTask
    {
        public bool isRunning { get; private set; }
        public Action OnComplete { get; set; }
        public Action OnInterrupt { get; set; }

        private float m_target;

        private GameObject m_player;

        public MoveTask()
        {
            m_player = FindUtility.Find("Player");
        }

        public void Start(float x)
        {
            if (Mathf.Abs(x - m_player.transform.position.x) < 0.01f)
                return;
            if (isRunning)
            {
                OnInterrupt?.Invoke();
            }
            isRunning = true;
            m_target = x;
            m_player.GetComponentInChildren<Animator>().Play("Player_WalkRight");
            if (x > m_player.transform.position.x)
            {
                m_player.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (x < m_player.transform.position.x)
            {
                m_player.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void Stop()
        {
            if (isRunning)
                OnInterrupt?.Invoke();
        }

        public void Update(float deltaTime)
        {
            if (isRunning)
            {
                m_player.transform.Translate(Vector3.right * (m_target - m_player.transform.position.x).Sign() * GlobalSetting.walkSpeed * deltaTime);
                if ((m_target - m_player.transform.position.x).Abs() < 0.01f)
                {
                    isRunning = false;
                    m_player.GetComponentInChildren<Animator>().Play("Player_Idle");
                    OnComplete?.Invoke();
                }
            }
        }
    }
}
