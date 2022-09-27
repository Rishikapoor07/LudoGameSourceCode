using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomTextIF : InputField
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.clickCount == 2)
        {
            SelectAll();
            ForceLabelUpdate();
        }
    }
}
