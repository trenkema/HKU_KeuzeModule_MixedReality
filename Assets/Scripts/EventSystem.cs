using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Event_Type
{
    SHAPE_COLLIDING,
    TOGGLE_CURSOR,
    IS_SCALING,
    IS_ROTATING,
    TOGGLE_INPUT,
    LOAD_BUTTON_SELECT
}

public static class EventSystem
{
    private static Dictionary<Event_Type, System.Action> eventRegister = new Dictionary<Event_Type, System.Action>();

    public static void Subscribe(Event_Type _evt, System.Action _func)
    {
        if (!eventRegister.ContainsKey(_evt))
        {
            eventRegister.Add(_evt, null);
        }

        eventRegister[_evt] += _func;
    }

    public static void Unsubscribe(Event_Type _evt, System.Action _func)
    {
        if (eventRegister.ContainsKey(_evt))
        {
            eventRegister[_evt] -= _func;
        }
    }

    public static void RaiseEvent(Event_Type _evt)
    {
        eventRegister[_evt]?.Invoke();
    }
}

public static class EventSystem<T>
{
    private static Dictionary<Event_Type, System.Action<T>> eventRegister = new Dictionary<Event_Type, System.Action<T>>();

    public static void Subscribe(Event_Type _evt, System.Action<T> _func)
    {
        if (!eventRegister.ContainsKey(_evt))
        {
            eventRegister.Add(_evt, null);
        }

        eventRegister[_evt] += _func;
    }

    public static void Unsubscribe(Event_Type _evt, System.Action<T> _func)
    {
        if (eventRegister.ContainsKey(_evt))
        {
            eventRegister[_evt] -= _func;
        }
    }

    public static void RaiseEvent(Event_Type _evt, T _arg)
    {
        eventRegister[_evt]?.Invoke(_arg);
    }
}

public static class EventSystem<A, T>
{
    private static Dictionary<Event_Type, System.Action<A, T>> eventRegister = new Dictionary<Event_Type, System.Action<A, T>>();

    public static void Subscribe(Event_Type _evt, System.Action<A, T> _func)
    {
        if (!eventRegister.ContainsKey(_evt))
        {
            eventRegister.Add(_evt, null);
        }

        eventRegister[_evt] += _func;
    }

    public static void Unsubscribe(Event_Type _evt, System.Action<A, T> _func)
    {
        if (eventRegister.ContainsKey(_evt))
        {
            eventRegister[_evt] -= _func;
        }
    }

    public static void RaiseEvent(Event_Type _evt, A _arg,  T _arg2)
    {
        eventRegister[_evt]?.Invoke(_arg, _arg2);
    }
}
