using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    private string itemId;
    public string ItemId
	{
		get => itemId;
		set => itemId = value;
	}

	private float itemcost;
	public float ItemCost
	{
		get => itemcost;
		set => itemcost = value;
	}

	private int itemAmount;
	public int ItemAmount
	{
		get => itemAmount;
		set => itemAmount = value;
	}
}
