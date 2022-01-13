using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadButton : MonoBehaviour, ISelectHandler
{
    [SerializeField] bool useParent = false;
    [SerializeField] Transform parentTransform;

    public void OnSelect(BaseEventData eventData)
    {
        if (useParent)
        {
            EventSystem<Transform>.RaiseEvent(EventType.LOAD_BUTTON_SELECT, parentTransform);
        }
        else
        {
            EventSystem<RectTransform>.RaiseEvent(EventType.LOAD_BUTTON_SELECT, GetComponent<RectTransform>());
        }
    }
}
