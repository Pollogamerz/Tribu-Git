using System;

public interface IStartable
{
    void StartMiniGame();
    void FinishMiniGame();
    event Action OnMiniGameFinished;
}

