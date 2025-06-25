using UnityEngine;

public class StateMachine
{
    private IState current;

    public void ChangeState(IState next)
    {
        current?.Exit();
        current = next;
        current.Enter();
    }

    public void Update()
    {
        current?.Update();
    }
}
