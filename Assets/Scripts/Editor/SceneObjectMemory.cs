using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

// Keeps an editor window pointing at the same scene object across a full editor restart, by its
// path in the hierarchy. Unity stores an asset reference as a GUID, but a scene GameObject has
// only a per-session instance id, so a window's serialized layout has nothing durable to hold.
public static class SceneObjectMemory
{
    // A field cleared by hand fills itself back in on the next repaint rather than staying empty.
    // Nothing here has a reason to clear it, and healing back to a working tool beats leaving one
    // that looks broken.
    public static GameObject Resolve(GameObject current, ref string rememberedPath)
    {
        if (current != null)
        {
            rememberedPath = PathOf(current);
            return current;
        }

        if (string.IsNullOrEmpty(rememberedPath))
            return null;

        return Find(rememberedPath);
    }

    private static string PathOf(GameObject target)
    {
        StringBuilder path = new StringBuilder(target.name);

        for (Transform parent = target.transform.parent; parent != null; parent = parent.parent)
            path.Insert(0, parent.name + "/");

        return path.ToString();
    }

    // Walked down from the scene's roots rather than handed to GameObject.Find, which matches on
    // the leaf name alone and skips anything inactive - so with one level deactivated it would
    // answer with the other level's identically named child.
    private static GameObject Find(string path)
    {
        int firstSlash = path.IndexOf('/');
        string rootName = firstSlash < 0 ? path : path.Substring(0, firstSlash);

        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name != rootName)
                continue;

            if (firstSlash < 0)
                return root;

            Transform child = root.transform.Find(path.Substring(firstSlash + 1));
            if (child != null)
                return child.gameObject;
        }

        return null;
    }
}
