using System;

public interface INetworkCountdown
{
    public enum CountdownState
    {
        Idle,
        Running,
        Ended
    }
    
    // Event that is triggered when the countdown finishes
    Action OnCountdownFinished { get; set; }

    // Event that is triggered every frame with the current time remaining and state
    Action<CountdownState, int> OnCountdownTick { get; set; }

    // Countdown state (Idle, Running, Ended)
    CountdownState State { get; }

    // Time remaining in the countdown (in milliseconds)
    int TimeRemaining { get; }

    // Boolean indicating if the countdown is running
    bool IsRunning { get; }

    // Starts the countdown (only for master client in a networked game)
    void StartCountdown(int countdownDuration = 10000);

    // // Initializes the countdown state from room properties (for all clients)
    // void TryLoadCountdown();
}
