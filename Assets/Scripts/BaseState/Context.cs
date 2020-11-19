using UnityEngine;

// Enforces methods that must exist in a context. Prevents rewriting this code in every context.
public abstract class Context<ContextType> : MonoBehaviour
{
    // A context must track the current state
    protected BaseState<ContextType> currentState;

    // A context must initialize itself to a starting state (and call enter on that state)
    public abstract void InitializeContext();

    // A starting state must delegate update to a state and if the state changes trigger exit on the old and enter on the new
    public void ManageState(ContextType context)
    {
        BaseState<ContextType> cacheState = currentState;

        currentState = currentState.UpdateState(context);

        if(currentState != cacheState)
        {
            cacheState.ExitState(context);
            currentState.EnterState(context);
        }
    }
}
