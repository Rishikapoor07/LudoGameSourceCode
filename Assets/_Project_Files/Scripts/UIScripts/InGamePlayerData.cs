using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerData : MonoBehaviour
{
    public GameObject FirstMove;
    public GameObject SecondMove;
    public GameObject ThirdMove;
    public List<GameObject> lastThreeMoves = new List<GameObject>();
}
