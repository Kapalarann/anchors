using UnityEngine;

public abstract class GameState
{
    protected GameStateManager manager;

    public GameState(GameStateManager manager)
    {
        this.manager = manager;
    }

    public virtual void Enter() { } // Called when the state is entered
    public virtual void Exit() { }  // Called when the state is exited
    public virtual void Update() { } // Called every frame
}
