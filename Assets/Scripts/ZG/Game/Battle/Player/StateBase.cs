/*
 * 脚本名(ScriptName)：    StateBase.cs
 * 作者(Author):           小宝
 * 官网(Url):              http://www.youke.pro
 */
using UnityEngine;
using System.Collections;

public abstract class StateBase : IState 
{
    protected PlayerBase mPlayer;
    public StateBase(PlayerBase player)
    {
        mPlayer = player;
    }

    public abstract uint GetStateID();

    public abstract void OnEnter(StateMachine machine, IState prevState, object param1, object param2);

    public abstract void OnLeave(IState nextState, object param1, object param2);

    public abstract void OnUpate();

    public abstract void OnFixedUpdate();

    public abstract void OnLeteUpdate();

    public virtual void OnAnimationEventEnd(string clipName) { }
    public virtual void OnAnimationAttackOver(string clipName) { }
}
