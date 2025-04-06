using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Behaviour
{
    /// <summary>
    /// Base for other states.
    /// </summary>
    public abstract class State
    {
        public UnityAction FixedUpdate;

        public UnityAction EnterState;
        public UnityAction ExitState;

        public abstract void CallFixedUpdate();
        public abstract void CallEnterState();
        public abstract void CallExitState();
    }

    /// <summary>
    /// Does not have children.
    /// </summary>
    public class AtomicState : State
    {
        public override void CallFixedUpdate()
        {
            if (FixedUpdate != null) FixedUpdate();
        }

        public override void CallEnterState()
        {
            if (EnterState != null) EnterState();
        }

        public override void CallExitState()
        {
            if (ExitState != null) ExitState();
        }

        public AtomicState(UnityAction fixedUpdate = null, UnityAction enterState = null, UnityAction exitState = null)
        {
            FixedUpdate = fixedUpdate;
            EnterState = enterState;
            ExitState = exitState;
        }

    }

    /// <summary>
    /// All child states are active.
    /// </summary>
    public class ParallelState : State
    {
        public List<State> children = new List<State>();
        public override void CallFixedUpdate()
        {
            if (FixedUpdate != null) FixedUpdate();
            foreach (var child in children)
            {
                child.CallFixedUpdate();
            }
        }

        public override void CallEnterState()
        {
            if (EnterState != null) EnterState();
            foreach (var child in children)
            {
                child.CallEnterState();
            }
        }

        public override void CallExitState()
        {
            if (ExitState != null) ExitState();
            foreach (var child in children)
            {
                child.CallExitState();
            }
        }

        public void AddChild(State child)
        {
            children.Add(child);
        }

        public ParallelState(UnityAction fixedUpdate = null, UnityAction enterState = null, UnityAction exitState = null)
        {
            FixedUpdate = fixedUpdate;
            EnterState = enterState;
            ExitState = exitState;
        }
    }

    /// <summary>
    /// One child state is active.
    /// You supply a function to pick that state based on number.
    /// </summary>
    public class CompoundState : State
    {
        public List<State> children = new List<State>();

        private int currentState;

        public override void CallFixedUpdate()
        {
            if (FixedUpdate != null) FixedUpdate();
            children.ElementAt(currentState).CallFixedUpdate();
        }

        public override void CallEnterState()
        {
            if (EnterState != null) EnterState();
            children.ElementAt(currentState).CallEnterState();
        }

        public override void CallExitState()
        {
            if (ExitState != null) ExitState();
            children.ElementAt(currentState).CallExitState();
        }

        public void AddChild(State child)
        {
            children.Add(child);
        }

        private void ChangeState(int stateIndex)
        {
            if (currentState == stateIndex) { return; }
            children.ElementAt(currentState).CallExitState();
            currentState = stateIndex;
            children.ElementAt(currentState).CallEnterState();
        }

        public UnityAction<int> GetChangeStateAction() {
            return ChangeState;
        }

        public CompoundState(int StartingState, UnityAction fixedUpdate = null, UnityAction enterState = null, UnityAction exitState = null)
        {
            FixedUpdate = fixedUpdate;
            EnterState = enterState;
            ExitState = exitState;
        }
    }

    /// <summary>
    /// One child state is active.
    /// You supply a function to pick that state based on if its true or false.
    /// </summary>
    public class BinaryState : State
    {
        public State trueState;
        public State falseState;

        private bool currentCondition;

        private State GetCurrentState() { return currentCondition ? trueState : falseState; }

        public override void CallFixedUpdate()
        {
            if (FixedUpdate != null) FixedUpdate();
            GetCurrentState().CallFixedUpdate();
        }

        public override void CallEnterState()
        {
            if (EnterState != null) EnterState();
            GetCurrentState().CallEnterState();
        }

        public override void CallExitState()
        {
            if (ExitState != null) ExitState();
            GetCurrentState().CallExitState();
        }

        private void ChangeState(bool condition)
        {
            if (currentCondition == condition) { return; }
            GetCurrentState().CallExitState();
            currentCondition = condition;
            GetCurrentState().CallEnterState();
        }

        public UnityAction<bool> GetChangeStateAction()
        {
            return ChangeState;
        }

        public BinaryState(bool StartingState, UnityAction fixedUpdate = null, UnityAction enterState = null, UnityAction exitState = null)
        {
            FixedUpdate = fixedUpdate;
            EnterState = enterState;
            ExitState = exitState;
        }
    }
}