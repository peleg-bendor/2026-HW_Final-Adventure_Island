using System;

// A destroyed enemy's wait before it comes back, which can be abandoned. One per enemy, so no countdown
// can cancel or finish another enemy's.
public interface IRespawnCountdown
{
    // Replaces any countdown already running. The seconds it waited are handed to the callback.
    void Begin(string owner, Action<float> finished);

    void Cancel();
}
