# FPS Prototype

----

![alt text](Recordings/screenshot_1.jpg)


This document is to serve as a technical documentation for the project.

### Features

- Modular AI System
- Smooth Quake like movement with strafe jumping
- Two weapons, swappable, with ammo
- Trigger System Where Events Open Gates
- Bullet Spread Visualizer Inspector
- Crunchy Particle Effects
- Camera Bob on Walk, Take Damage, and Landing
- Level Complete Screen With Accuracy Calculation

### Modular AI System
I had tried a few AI assets from the store and found them to have various bugs that caused my enemies to act with strange behaviours, especially during state transitions. I decided to take inspiration from them and create my own.

The high level overview of the system is as follows:

1. Implement Behaviors For Each State
   - We define an enum that contains possible states for an AI type.
   - For each behavior we create a class that derives from `AIStateBehavior<PossibleStatesEnum>` where possible states is the enum defined in step 1. This essentially makes our class useable by a wrapping StateMachine.
   - In this class we program the behavior of our AI. `Awake` is replaced with `AwakeStateController`, but functions the same. This is because the parent class uses Awake for some initialization into its state machine.
   - Each behavior controls transitions to other states, calling `stateMachine.TransitionToState(THE_STATE)`
  
2. Implement a `BehaviorStateManager`. In this project the example of this class is the `MeleeEnemyController`. It also needs a Type argument which should match the Enum that the behaviors were defined with. This allows the Controller to be compatible with the AIStateBehaviors.
   - In here, you override the abstract `TransitionToState()` method that is called by children StateBehaviors, and define what happens during transition. In the example `MeleeEnemyController` class, we call...
      - **ExitState()** - This method is part of the abstract `AIStateBehavior` which was overriden by the implementing class. This is so the behavior can do something on exit of its own State.
      - Update the currentState of the `MeleeEnemyController`.
      - **EnterState()** - Call on the new state.
      - Disable the old StateBehavior.
      - Enable the new StateBehavior.
    - This class also defines other non-state related behaviors such as executing footstep audio and handling damage effects.
