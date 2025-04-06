using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviour;
using UnityEngine.Events;

public class EnemyBehaviour : BehaviourTree
{

    private UnityAction<bool> SetAnxietyCondition;
    private UnityAction<int> SetCalmState;
    private UnityAction<int> SetScaredState;

    protected override void Setup()
    {
        BinaryState anxietyState = new BinaryState(false, RootFixedUpdate, OnRootEnter);
        SetAnxietyCondition = anxietyState.GetChangeStateAction();
        rootState = anxietyState;

        CompoundState calm = new CompoundState(0, CalmFixedUpdate);
        SetCalmState = calm.GetChangeStateAction();
        anxietyState.falseState = calm;

        CompoundState scared = new CompoundState(0, ScaredFixedUpdate, OnEnterScared);
        SetScaredState = scared.GetChangeStateAction();
        anxietyState.trueState = scared;

        AtomicState placeholderCalmState = new AtomicState();
        calm.AddChild(placeholderCalmState);
        AtomicState placeholderAngryState = new AtomicState();
        scared.AddChild(placeholderAngryState);



    }


    bool isPlayerNear = false; // TODO detect if player is near or add additional functionality

    private void OnRootEnter()
    {
        StartCoroutine(DebugChangeState());
    }

    private void RootFixedUpdate()
    {
        SetAnxietyCondition(isPlayerNear);
    }

    IEnumerator DebugChangeState()
    {
        yield return new WaitForSeconds(0.5f);
        isPlayerNear = true;
    }

    private void CalmFixedUpdate()
    {
        Debug.Log("IM CALMED");
        
    }
    private void ScaredFixedUpdate()
    {
        Debug.Log("IM SCARED");
    }

    private void OnEnterScared()
    {
        Debug.Log("I BECAME SCARED");
    }


}
