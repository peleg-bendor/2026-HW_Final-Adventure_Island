using UnityEngine;

// Keeps an editor window pointing at the same scene object across a full editor restart, by name.
// Unity stores an asset reference as a GUID, but a scene GameObject has only a per-session
// instance id, so a window's serialized layout has nothing durable to hold. Not GlobalObjectId:
// the only thing this points at is a root object with a fixed name the whole project relies on.
public static class SceneObjectMemory
{
    // A field cleared by hand fills itself back in on the next repaint rather than staying empty.
    // Nothing here has a reason to clear it, and healing back to a working tool beats leaving one
    // that looks broken.
    public static GameObject Resolve(GameObject current, ref string rememberedName)
    {
        if (current != null)
        {
            rememberedName = current.name;
            return current;
        }

        if (string.IsNullOrEmpty(rememberedName))
            return null;

        return GameObject.Find(rememberedName);
    }
}
