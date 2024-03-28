using System.Collections.Generic;

namespace Dao.SceneSystem
{
    public class SceneManager : Singleton<SceneManager>
    {
        private Dictionary<string, IScene> m_scenes = new();

        public IScene Current { get; private set; }

        public void AddScene(string name, IScene scene)
        {
            if (string.IsNullOrEmpty(name) || scene == null) return;
            m_scenes.TryAdd(name, scene);
        }

        public void RemoveScene(string name)
        {
            m_scenes.Remove(name);
        }

        public void LoadScene(string name)
        {
            if (m_scenes.TryGetValue(name, out IScene nextScene))
            {
                Current?.OnExit();
                nextScene.OnEnter();
                Current = nextScene;
            }
        }

        public void Update(float deltaTime)
        {
            Current?.OnUpdate(deltaTime);
        }
    }
}
