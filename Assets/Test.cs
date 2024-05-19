using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    async void Start()
    {
        // ���������첽����
        Task task1 = Task.Run(() => LongRunningOperation1());
        Task task2 = Task.Run(() => LongRunningOperation2());

        // ʹ��Task.WhenAll���ȴ������������
        await Task.WhenAll(task1, task2);

        // ����������������ɣ�����ִ�к�������
        Debug.Log("Both tasks have completed.");
        // ����ִ�к��������
    }

    // ģ��һ����ʱ�����еĲ���
    void LongRunningOperation1()
    {
        // �������������ҪһЩʱ��
        Thread.Sleep(3000);
        Debug.Log("Task 1 completed.");
    }

    // ģ����һ����ʱ�����еĲ���
    void LongRunningOperation2()
    {
        // �������������ҪһЩʱ��
        Thread.Sleep(2000);
        Debug.Log("Task 2 completed.");
    }
}
