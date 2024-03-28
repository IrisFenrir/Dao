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
                    GameObject result = FindRecursizely(item, name);
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

        private static GameObject FindRecursizely(GameObject parent, string name)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Transform child = parent.transform.GetChild(i);
                if (child.name == name)
                    return child.gameObject;
                GameObject result = FindRecursizely(child.gameObject, name);
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
    }
}
