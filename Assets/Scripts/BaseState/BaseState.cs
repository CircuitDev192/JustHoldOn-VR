
// The base state class that all state
public abstract class BaseState<ContextType>
{
    // Called when entering the state
    public abstract void EnterState(ContextType context);

    // Called on update
    public abstract BaseState<ContextType> UpdateState(ContextType context);

    // Called when exiting the state
    public abstract void ExitState(ContextType context);
}
