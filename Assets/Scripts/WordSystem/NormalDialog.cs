using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Dao.WordSystem
{
    public class NormalDialog : IDialog
    {
        private Text m_text;
        private string m_content;

        public NormalDialog(string content)
        {
            Next = new IDialog[1];
            m_text = FindUtility.Find("Canvas/DialogPanel/NormalSentence/Text").GetComponent<Text>();
            m_content = content;
        }

        public override async Task Show()
        {
            m_text.transform.parent.gameObject.SetActive(true);
            m_text.text = "";
            StringBuilder builder = new StringBuilder();
            foreach (var item in m_content)
            {
                builder.Append(item);
                m_text.text = builder.ToString();
                await Task.Delay(50);
            }
        }

        public override void Close()
        {
            m_text.transform.parent.gameObject.SetActive(false);
        }
    }
}
