// What the rest of the game does to power: fruit gains it, a hazard spends it. Its own verbs rather
// than the model's Add and Remove, so which layer a call is at is visible at the call site.
public interface IPower
{
    // Answers with how much actually landed, so fruit taken at full power is distinguishable from
    // a real gain.
    int Gain(int amount);

    void Spend(int amount);
}
