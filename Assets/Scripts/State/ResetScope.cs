// How much of a level a reset restores. Losing a פסילה leaves dead enemies dead where starting a
// level does not, and each object decides which of those applies to it.
public enum ResetScope
{
    Full,
    AfterStrike
}
