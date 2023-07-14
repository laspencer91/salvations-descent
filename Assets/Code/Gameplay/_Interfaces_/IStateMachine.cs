
// summary: 
//   Make any object a state machine by implementing this interface. Its generic type specifies the values which will be used to identify states.
public interface IStateMachine<StateTypes>
{
    void TransitionToState(StateTypes state);
}