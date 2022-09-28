using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePlayerData : MonoBehaviour
{
    public RawImage FirstMove;
    public RawImage SecondMove;
    public RawImage ThirdMove;
    public List<RawImage> moves = new List<RawImage>();
    public List<Texture> lastThreeMoves = new List<Texture>();
}
