using System;

// The power count, clamped between empty and the bar's capacity. Knows nothing about what reaching
// zero means, because that is a game rule and it lives in the controller.
public class PowerModel : IPowerModel
{
    public PowerModel(int capacity)
    {
        Capacity = capacity;
        Current = capacity;
    }

    public int Current { get; private set; }
    public int Capacity { get; private set; }

    public int Add(int amount)
    {
        if (amount <= 0)
            return 0;

        int before = Current;
        Current = Math.Min(Current + amount, Capacity);
        return Current - before;
    }

    public int Remove(int amount)
    {
        if (amount <= 0)
            return 0;

        int before = Current;
        Current = Math.Max(Current - amount, 0);
        return before - Current;
    }

    public void SetTo(int amount)
    {
        Current = Math.Min(Math.Max(amount, 0), Capacity);
    }
}
