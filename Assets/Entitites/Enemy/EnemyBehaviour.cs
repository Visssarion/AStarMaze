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

        AtomicState stand = new AtomicState(); // SetCalmState(0);
        AtomicState sleep = new AtomicState(SleepFixed); // SetCalmState(1);
        AtomicState patrol = new AtomicState(); // SetCalmState(2);
        AtomicState stroll = new AtomicState(); // SetCalmState(3);
        AtomicState talk = new AtomicState(); // SetCalmState(4);
        calm.AddChild(stand);
        calm.AddChild(sleep);
        calm.AddChild(patrol);
        calm.AddChild(stroll);
        calm.AddChild(talk);


        CompoundState scared = new CompoundState(0, ScaredFixedUpdate, OnEnterScared);
        SetScaredState = scared.GetChangeStateAction();
        anxietyState.trueState = scared;

        AtomicState screaming = new AtomicState(); // SetScaredState(0);
        AtomicState runningAway = new AtomicState(); // SetScaredState(1);
        scared.AddChild(screaming);
        scared.AddChild(runningAway);


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

    private void SleepFixed()
    {

    }
    private void OnEnterScared()
    {
        Debug.Log("I BECAME SCARED");
    }


}
