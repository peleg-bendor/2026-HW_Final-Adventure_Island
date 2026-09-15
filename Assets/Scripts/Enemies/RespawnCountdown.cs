using System;
using System.Threading;
using System.Threading.Tasks;

// Waits a random time from the game's respawn rule, then reports back. A Task and not a coroutine: the
// enemy it belongs to is switched off for the whole wait, and Unity stops coroutines on a disabled object.
public class RespawnCountdown : IRespawnCountdown
{
    private readonly RespawnDelay delay;

    private CancellationTokenSource running;

    public RespawnCountdown(RespawnDelay delay)
    {
        this.delay = delay;
    }

    // Started rather than awaited, since nothing that destroys an enemy can await.
    public async void Begin(string owner, Action<float> finished)
    {
        Cancel();

        if (delay == null)
        {
            GameLog.Warning(LogCategory.Enemy, "No RespawnDelay injected, " + owner + " stays destroyed");
            return;
        }

        CancellationTokenSource countdown = new CancellationTokenSource();
        running = countdown;

        float seconds = UnityEngine.Random.Range(delay.minSeconds, delay.maxSeconds);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds), countdown.Token);
        }
        catch (OperationCanceledException)
        {
            // Abandoned by a level start or by the enemy being destroyed, neither of which is a fault.
            return;
        }
        catch (Exception error)
        {
            // async void rather than an unobserved Task, so a fault surfaces here instead of vanishing.
            GameLog.Error(LogCategory.Enemy, owner + " respawn failed - " + error.Message);
            return;
        }

        // The delay can end a frame before this line runs. A cancel or a new countdown in that gap has
        // replaced this one, and the enemy is already where that left it.
        if (running != countdown)
            return;

        running = null;
        countdown.Dispose();
        finished(seconds);
    }

    public void Cancel()
    {
        if (running == null)
            return;

        running.Cancel();
        running.Dispose();
        running = null;
    }
}
