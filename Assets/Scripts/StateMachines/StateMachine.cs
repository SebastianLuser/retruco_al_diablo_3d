using UnityEngine;

public class StateMachine
{
    private IState current;

    public void ChangeState(IState next)
    {
        if (next == null)
        {
            Debug.LogError("Attempting to change to null state");
            return;
        }
        current?.Exit();
        current = next;
        current.Enter();
    }

    public void Update()
    {
        current?.Update();
    }
}
