﻿using System.Collections.Generic;

namespace Dao.SceneSystem
{
    public class SceneManager : Singleton<SceneManager>
    {
        private Dictionary<string, IScene> m_scenes = new();

        public IScene Current { get; private set; }

        public void AddScene(string name, IScene scene)
        {
            if (string.IsNullOrEmpty(name) || scene == null) return;
            scene.name = name;
            m_scenes.TryAdd(name, scene);
        }

        public void RemoveScene(string name)
        {
            m_scenes.Remove(name);
        }

        public T GetScene<T>(string sceneName) where T:IScene
        {
            return m_scenes[sceneName] as T;
        }

        public void LoadScene(string name)
        {
            if (m_scenes.TryGetValue(name, out IScene nextScene))
            {
                Current?.Disable();
                Current?.Hide();
                nextScene.OnEnter();
                nextScene.Enable();
                nextScene.Show();
                Current = nextScene;
            }
        }

        public void Update(float deltaTime)
        {
            Current?.OnUpdate(deltaTime);
        }
    }
}
