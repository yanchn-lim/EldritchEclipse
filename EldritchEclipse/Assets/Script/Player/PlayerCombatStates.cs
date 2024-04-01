using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCombatState_Base : FSM_State
{
    protected PlayerCombatHandler combatHandler;

    public PlayerCombatState_Base(int id,FSM fsm, PlayerCombatHandler handler) : base (id,fsm)
    {
        combatHandler = handler;
    }
}

public class PlayerCombatState_Shooting : PlayerCombatState_Base
{
    Coroutine firing;
    public override void Enter()
    {
        //start shooting
        firing = combatHandler.StartCoroutine(Firing());
    }

    public override void Update()
    {
        
    }

    IEnumerator Firing()
    {
        while (InputHandler.FireHeld)
        {
            if (combatHandler.AmmoPercentage <= 0)
            {
                Debug.Log($"firing : switching to reload {combatHandler.AmmoPercentage}");
                _fsm.SwitchState((int)CombatStates.RELOAD);
                yield break;
            }

            combatHandler.SpawnBullet();
            yield return new WaitForSeconds(combatHandler.DelayBetweenShots);
        }

        _fsm.SwitchState((int)CombatStates.IDLE);
    }

    public override void Exit()
    {
        if(firing != null)
            combatHandler.StopCoroutine(firing);
    }

    public PlayerCombatState_Shooting(int id, FSM fsm, PlayerCombatHandler handler) : base(id,fsm,handler)
    {

    }
}

public class PlayerCombatState_Idle : PlayerCombatState_Base
{
    float reloadDelay;
    Coroutine timer;
    public override void Enter()
    {
        if (combatHandler.AmmoPercentage == 1)
            return;

        timer = combatHandler.StartCoroutine(StartReloadTimer());
    }

    IEnumerator StartReloadTimer()
    {
        float timer = 0;
        while(timer < reloadDelay)
        {
            yield return new WaitForSeconds(GameVariables.TimeTick);
            timer += GameVariables.TimeTick;
        }

        _fsm.SwitchState((int)CombatStates.RELOAD);
    }

    public override void Update()
    {
        if (InputHandler.FirePressed)
        {
            _fsm.SwitchState((int)CombatStates.SHOOTING);
        }
    }

    public override void Exit()
    {
        if(timer != null)
            combatHandler.StopCoroutine(timer);
    }

    public PlayerCombatState_Idle(int id, FSM fsm, PlayerCombatHandler handler,float ReloadDelay) : base(id,fsm, handler)
    {
        reloadDelay = ReloadDelay;
    }
}

public class PlayerCombatState_Reload : PlayerCombatState_Base
{
    Coroutine reload;
    bool _reloadComplete;

    public override void Enter()
    {
        reload = combatHandler.StartCoroutine(Reload());
        _reloadComplete = false;
        combatHandler.ToggleReloadBar();
    }

    IEnumerator Reload()
    {
        float progress = 0;
        while (progress < 100f)
        {
            yield return new WaitForSeconds(GameVariables.TimeTick);
            progress += combatHandler.ReloadSpeed * GameVariables.TimeTick;
            combatHandler.UpdateReloadBar(progress);
        }

        combatHandler.Reload();
        _reloadComplete = true;
    }

    public override void Update()
    {
        if(_reloadComplete && InputHandler.FireHeld)
        {
            _fsm.SwitchState((int)CombatStates.SHOOTING);
            return;
        }

        if (_reloadComplete)
        {
            _fsm.SwitchState((int)CombatStates.IDLE);
        }

        if((InputHandler.FireHeld || InputHandler.FirePressed) && combatHandler.AmmoPercentage > 0)
        {
            _fsm.SwitchState((int)CombatStates.SHOOTING);
        }
    }

    public override void Exit()
    {
        if(reload != null)
            combatHandler.StopCoroutine(reload);

        combatHandler.ToggleReloadBar();

    }

    public PlayerCombatState_Reload(int id, FSM fsm, PlayerCombatHandler handler) : base(id,fsm, handler)
    {

    }
}
public enum CombatStates
{
    IDLE = 0,
    SHOOTING,
    RELOAD
}