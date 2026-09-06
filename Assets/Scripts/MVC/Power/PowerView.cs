using UnityEngine;
using UnityEngine.UI;

// Draws the bar as one line per unit of capacity. The lines are built from the number it is given
// rather than placed by hand, which could not fall out of step with a changed capacity.
public class PowerView : MonoBehaviour, IPowerView
{
    [SerializeField] private Image linePrefab;

    private Image[] lines;

    public void ShowPower(int current, int capacity)
    {
        if (lines == null)
            Build(capacity);

        if (lines == null)
            return;

        // The renderer is switched off rather than the object, so an empty slot keeps its place in
        // the row and the bar stays the same width however full it is.
        for (int i = 0; i < lines.Length; i++)
            lines[i].enabled = i < current;
    }

    private void Build(int capacity)
    {
        if (linePrefab == null)
        {
            GameLog.Warning(LogCategory.Game, "No line prefab assigned, the power bar will not be drawn");
            return;
        }

        lines = new Image[capacity];

        for (int i = 0; i < capacity; i++)
            lines[i] = Instantiate(linePrefab, transform, false);
    }
}
