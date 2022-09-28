using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{

    /*
	 * onDiceRolled event dipanggil ketika dadu sudah selesai dikocok
	 */
    public delegate void OnDiceRolled(int diceNum, Token.TokenType type);
    public event OnDiceRolled onDiceRolled;

    [SerializeField] private GameObject[] placeHolders;
    [SerializeField] private GameObject[] dices;
    [SerializeField] private Token.TokenType activeDice = Token.TokenType.Blue;  //need to make this random
    [SerializeField] private List<Texture> diceTextures;
    Dictionary<int, Texture> DiceImageWithValues = new Dictionary<int, Texture>();


    /*
	 * memilih dadu yang akan digunakan, 
	 * dadu yg tidak digunakan tidak ditampilkan pada scene
	 * bersamaan dgn placeholder-nya
	 */
    public void Init(Token.TokenType[] selected)
    {
        DeactivateAllDice();

        int count = Mathf.Min(selected.Length, 4);
        for (int i = 0; i < count; i++)
        {
            int idx = (int)selected[i];
            placeHolders[idx].SetActive(true);
        }

        for (int i = 0; i < diceTextures.Count; i++)
        {
            DiceImageWithValues.Add(i + 1, diceTextures[i]);
        }
    }

    /*
	 * menampilkan kembali dadu pada placeholder yg aktif
	 * (dadu tdk akan tampil jika placeholder tidak aktif/tampil)
	 */
    public void ShowDice(Token.TokenType type)
    {
        dices[(int)activeDice].SetActive(false); //todo
        dices[(int)type].SetActive(true);
        activeDice = type;
    }

    public Dice GetCurrentActiveDice()
    {
        return dices[(int)activeDice].GetComponent<Dice>();
    }

    public Texture GetCurrentDiceTexture(int dicenum)
    {
        return DiceImageWithValues[dicenum];
    }

    void Start()
    {
        placeHolders = new GameObject[3];
        placeHolders[0] = GameObject.FindWithTag("BlueDicePlaceHolder");
        placeHolders[1] = GameObject.FindWithTag("RedDicePlaceHolder");
        placeHolders[2] = GameObject.FindWithTag("YellowDicePlaceHolder");

        dices = new GameObject[placeHolders.Length];
        dices[0] = FindDiceFrom(placeHolders[0], "DiceBlue");
        dices[1] = FindDiceFrom(placeHolders[1], "DiceRed");
        dices[2] = FindDiceFrom(placeHolders[2], "DiceYellow");

        for (int i = 0; i < dices.Length; i++)
        {
            Dice dice = dices[i].GetComponent<Dice>();
            if (dice != null)
            {
                dice.onDiceRolled += DiceRolled;
            }
        }

        DeactivateAllDice();
    }




    GameObject FindDiceFrom(GameObject parent, string diceName)
    {
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.name == diceName)
            {
                return t.GetChild(i).gameObject;
            }
        }

        return null;
    }

    void DiceRolled(int diceNum)
    {
        if (onDiceRolled != null)
        {
            onDiceRolled(diceNum, activeDice);
        }
    }

    void DeactivateAllDice()
    {
        for (int i = 0; i < placeHolders.Length; i++)
        {
            placeHolders[i].SetActive(false);
            dices[i].SetActive(false);
        }
    }
}
