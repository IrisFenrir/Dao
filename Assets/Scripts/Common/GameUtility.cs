using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.Common
{
    public class GameUtility
    {
        public static float GetPlayerPos(GameObject responder)
        {
            var player = FindUtility.Find("Player").transform.position.x;
            var pos1 = FindUtility.Find("PlayerPos1", responder.transform).transform.position.x;
            var pos2 = FindUtility.Find("PlayerPos2", responder.transform).transform.position.x;
            var dis1 = (player - pos1).Abs();
            var dis2 = (player - pos2).Abs();
            return dis1 < dis2 ? pos1 : pos2;
        }

        public static Vector3 GetDialogPos(GameObject responder)
        {
            return FindUtility.Find("DialogPos", responder.transform).transform.position;
        }

        public static async Task FadeOut(SpriteRenderer spriteRenderer, float time)
        {
            float a = 1;
            Color color = spriteRenderer.color;
            float speed = 1f / time;
            while (a > 0)
            {
                a = Mathf.Clamp01(a - speed * Time.deltaTime);
                spriteRenderer.color = new Color(color.r, color.g, color.b, a);
                await Task.Yield();
            }
            spriteRenderer.color = new Color(color.r, color.g, color.b, 0);
        }

        public static async Task FadeIn(SpriteRenderer spriteRenderer, float time)
        {
            float a = 0;
            Color color = spriteRenderer.color;
            float speed = 1f / time;
            while (a < 1)
            {
                a = Mathf.Clamp01(a + speed * Time.deltaTime);
                spriteRenderer.color = new Color(color.r, color.g, color.b, a);
                await Task.Yield();
            }
            spriteRenderer.color = new Color(color.r, color.g, color.b, 1);
        }

        public static async Task TransitionLeft2Right(Action action = null)
        {
            var transitionRoot = FindUtility.Find("Canvas/Transition");
            var panel = FindUtility.Find("Panel", transitionRoot.transform);

            // 参数
            float transitionLerpSpeed = GameLoop.Instance.transitionLerpSpeed;

            // 设置初始状态
            transitionRoot.SetActive(true);
            var image = panel.GetComponent<Image>();
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 0;

            // 从左到右出现
            float value = 0;
            while (value < 0.995f)
            {
                value = Mathf.Lerp(value, 1, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            // 等待
            await Task.Delay(1000);
            action?.Invoke();

            // 从左到右消失
            image.fillOrigin = (int)Image.OriginHorizontal.Right;
            value = image.fillAmount;
            while (value > 0.005f)
            {
                value = Mathf.Lerp(value, 0, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            transitionRoot.SetActive(false);
        }

        public static async Task TransitionRight2Left(Action action = null)
        {
            var transitionRoot = FindUtility.Find("Canvas/Transition");
            var panel = FindUtility.Find("Panel", transitionRoot.transform);

            // 参数
            float transitionLerpSpeed = GameLoop.Instance.transitionLerpSpeed;

            // 设置初始状态
            transitionRoot.SetActive(true);
            var image = panel.GetComponent<Image>();
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Right;
            image.fillAmount = 0;

            // 从左到右出现
            float value = 0;
            while (value < 0.995f)
            {
                value = Mathf.Lerp(value, 1, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            // 等待
            await Task.Delay(1000);
            action?.Invoke();

            // 从左到右消失
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            value = image.fillAmount;
            while (value > 0.005f)
            {
                value = Mathf.Lerp(value, 0, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            transitionRoot.SetActive(false);
        }

        public static async Task TransitionTop2Bottom(Action action = null)
        {
            var transitionRoot = FindUtility.Find("Canvas/Transition");
            var panel = FindUtility.Find("Panel", transitionRoot.transform);

            // 参数
            float transitionLerpSpeed = GameLoop.Instance.transitionLerpSpeed;

            // 设置初始状态
            transitionRoot.SetActive(true);
            var image = panel.GetComponent<Image>();
            image.fillMethod = Image.FillMethod.Vertical;
            image.fillOrigin = (int)Image.OriginVertical.Top;
            image.fillAmount = 0;

            // 从左到右出现
            float value = 0;
            while (value < 0.995f)
            {
                value = Mathf.Lerp(value, 1, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            // 等待
            await Task.Delay(1000);
            action?.Invoke();

            // 从左到右消失
            image.fillOrigin = (int)Image.OriginVertical.Bottom;
            value = image.fillAmount;
            while (value > 0.005f)
            {
                value = Mathf.Lerp(value, 0, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            transitionRoot.SetActive(false);
        }

        public static async Task TransitionBottom2Top(Action action = null)
        {
            var transitionRoot = FindUtility.Find("Canvas/Transition");
            var panel = FindUtility.Find("Panel", transitionRoot.transform);

            // 参数
            float transitionLerpSpeed = GameLoop.Instance.transitionLerpSpeed;

            // 设置初始状态
            transitionRoot.SetActive(true);
            var image = panel.GetComponent<Image>();
            image.fillMethod = Image.FillMethod.Vertical;
            image.fillOrigin = (int)Image.OriginVertical.Bottom;
            image.fillAmount = 0;

            // 从左到右出现
            float value = 0;
            while (value < 0.995f)
            {
                value = Mathf.Lerp(value, 1, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            // 等待
            await Task.Delay(1000);
            action?.Invoke();

            // 从左到右消失
            image.fillOrigin = (int)Image.OriginVertical.Top;
            value = image.fillAmount;
            while (value > 0.005f)
            {
                value = Mathf.Lerp(value, 0, transitionLerpSpeed);
                image.fillAmount = value;
                await Task.Yield();
            }

            transitionRoot.SetActive(false);
        }

        public enum TransitionDirection
        {
            LeftToRight, RightToLeft, TopToBottom, BottomToTop
        }

        public static async Task Transition(Action action = null)
        {
            switch(GameLoop.Instance.transitionDirection)
            {
                case TransitionDirection.LeftToRight:
                    await TransitionLeft2Right(action);
                    break;
                case TransitionDirection.RightToLeft:
                    await TransitionRight2Left(action);
                    break;
                case TransitionDirection.TopToBottom:
                    await TransitionTop2Bottom(action);
                    break;
                case TransitionDirection.BottomToTop:
                    await TransitionBottom2Top(action);
                    break;
            }
        }
    }
}
