using Dao.CameraSystem;
using Dao.WordSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dao.SceneSystem
{
    public class Kitchen : IScene
    {
        private GameObject m_root;
        private bool m_isFireOpened;

        public Kitchen()
        {
            m_root = FindUtility.Find("Kitchen");
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
                    while (mather.transform.position.x < 63.9f)
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
    }
}
