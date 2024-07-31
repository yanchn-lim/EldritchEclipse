using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventSystem
{
    public static EventManager<GameEvents> Game = new();
    public static EventManager<PlayerEvents> Player = new();

}

//put your event types here
public enum GameEvents
{

}

public enum PlayerEvents
{
    PLAYER_XP_GAIN
}