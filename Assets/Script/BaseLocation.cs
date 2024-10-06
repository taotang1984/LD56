using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseLocation : Location
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        Debug.Log("Base Location Clicked");

    }

    public override void UpdateUI()
    {
        Debug.Log("Update Base Location UI");
    }

    private void TransferResource()
    {
        Debug.Log("Transfer Resource");
    }
}
