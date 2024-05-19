using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    async void Start()
    {
        // 定义两个异步任务
        Task task1 = Task.Run(() => LongRunningOperation1());
        Task task2 = Task.Run(() => LongRunningOperation2());

        // 使用Task.WhenAll来等待两个任务都完成
        await Task.WhenAll(task1, task2);

        // 这里两个任务都已完成，可以执行后续操作
        Debug.Log("Both tasks have completed.");
        // 继续执行后面的内容
    }

    // 模拟一个长时间运行的操作
    void LongRunningOperation1()
    {
        // 假设这个操作需要一些时间
        Thread.Sleep(3000);
        Debug.Log("Task 1 completed.");
    }

    // 模拟另一个长时间运行的操作
    void LongRunningOperation2()
    {
        // 假设这个操作需要一些时间
        Thread.Sleep(2000);
        Debug.Log("Task 2 completed.");
    }
}
