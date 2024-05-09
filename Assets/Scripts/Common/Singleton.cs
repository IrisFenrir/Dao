using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    private static T m_instance;
    private static readonly object m_lock = new object();

    public static T Instance
    {
        get
        {
            if(m_instance == null)
            {
                lock(m_lock)
                {
                    m_instance ??= new T();
                }
            }
            return m_instance;
        }
    }
}

