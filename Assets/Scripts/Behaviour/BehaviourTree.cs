using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Behaviour
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        protected State rootState;

        /// <summary>
        /// Use this function as Start function
        /// </summary>
        protected abstract void Setup();

        void Start()
        {
            Setup();
            rootState.CallEnterState();
        }

        void FixedUpdate()
        {
            rootState.CallFixedUpdate();
        }

        void OnDestroy()
        {
            rootState.CallExitState();
        }

    }
}