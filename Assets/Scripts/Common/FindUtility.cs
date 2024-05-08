using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dao
{
    public static class FindUtility
    {
        public static GameObject Find(string name, string sceneName = null)
        {
            Scene sceneToSearch = sceneName != null ? SceneManager.GetSceneByName(sceneName) : SceneManager.GetActiveScene();
            if (!sceneToSearch.isLoaded) return null;
            GameObject[] rootObjects = sceneToSearch.GetRootGameObjects();
            string[] names = name.Split('/');
            foreach (GameObject item in rootObjects)
            {
                if (names.Length > 1)
                {
                    if (names[0] == item.name)
                    {
                        GameObject result = FindWithPathRecursively(item, names, 1);
                        if (result != null) return result;
                        break;
                    }
                }
                else
                {
                    if (names[0] == item.name)
                        return item;
                    GameObject result = FindRecursively(item, name);
                    if (result != null) return result;
                }
            }
            return null;
        }

        private static GameObject FindWithPathRecursively(GameObject parent, string[] path, int index)
        {
            Transform child = parent.transform.Find(path[index]);
            if (child == null) return null;
            if (index == path.Length - 1)
                return child.gameObject;
            return FindWithPathRecursively(child.gameObject, path, index + 1);
        }

        private static GameObject FindRecursively(GameObject parent, string name)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);
                if (child.name == name)
                    return child.gameObject;
                GameObject result = FindRecursively(child.gameObject, name);
                if (result != null) return result;
            }
            return null;
        }

        public static GameObject Find(string name, Transform parent)
        {
            if (string.IsNullOrEmpty(name) || parent == null)
                return null;
            
            if (parent.name == name)
                return parent.gameObject;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject result = Find(name, parent.GetChild(i));
                if (result != null)
                    return result;
            }
            return null;
        }

        public static T[] FindAllOfType<T>(Transform parent) where T:MonoBehaviour
        {
            List<T> results = new List<T>();
            FindAllRecursively(parent, p => p.GetComponent<Responder>() != null, p => results.Add(p.GetComponent<T>()));
            return results.ToArray();
        }

        private static void FindAllRecursively(Transform parent, Predicate<Transform> condition, Action<Transform> action)
        {
            if (parent == null) return;
            if (condition(parent))
                action(parent);
            for (int i = 0; i < parent.childCount; i++)
            {
                FindAllRecursively(parent.GetChild(i), condition, action);
            }
        }
    }
}
