using TMPro;
using UnityEngine;

// Draws the fruit count as a number beside a fixed icon, in a warning colour when the next strike
// is close. Its own class rather than a base shared with StrikesView, which would be three types
// doing the work of two.
public class FruitView : MonoBehaviour, IFruitView
{
    [SerializeField] private TextMeshProUGUI count;

    // How few short of the next strike counts as close. A display choice rather than a game rule,
    // which is why the controller hands over a distance instead of a verdict.
    [SerializeField, Min(0)] private int warnWithin = 5;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;

    private void Awake()
    {
        if (count == null)
            GameLog.Warning(LogCategory.Game, "No count label assigned, the fruit count will not be drawn");
    }

    public void ShowFruit(int collected, int untilStrike)
    {
        if (count == null)
            return;

        count.text = collected.ToString("00");
        count.color = untilStrike <= warnWithin ? warningColor : normalColor;
    }
}
