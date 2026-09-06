using TMPro;
using UnityEngine;

// Draws the strikes left as a number beside a fixed icon. Only the label is held here, since the
// icon never changes.
public class StrikesView : MonoBehaviour, IStrikesView
{
    [SerializeField] private TextMeshProUGUI count;

    private void Awake()
    {
        if (count == null)
            GameLog.Warning(LogCategory.Game, "No count label assigned, the strikes will not be drawn");
    }

    public void ShowStrikes(int remaining)
    {
        if (count != null)
            count.text = remaining.ToString("00");
    }
}
