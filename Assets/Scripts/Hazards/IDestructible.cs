// Answered by anything that can be destroyed, and the only way to destroy it. One call rather than
// a readable rule plus a separate Destroy, so no caller can skip the question.
public interface IDestructible
{
    // True when it was actually destroyed, which is how an axe tells a rock it bounced off from an
    // enemy it killed.
    bool TryDestroy(Destroyer by);
}
