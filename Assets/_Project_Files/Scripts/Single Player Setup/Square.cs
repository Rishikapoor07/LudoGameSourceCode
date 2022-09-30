using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Token.TokenType activeDice = Token.TokenType.Blue;
    public PathManager pathManager;
    public bool isSafe = false;
    public bool isVertical;
    public bool Safe
    {
        get { return isSafe; }
        set { isSafe = value; }
    }

    [SerializeField] private List<Token> tokens;
    public List<Token> Tokens
    {
        get { return tokens; }
    }

    void Start()
    {
        tokens = new List<Token>();
        pathManager = GetComponentInParent<PathManager>();
    }

    public void AddToken(Token token)
    {
        tokens.Add(token);
        SetTheTokenScaleAndPosition();
    }

    public void RemoveToken(Token token)
    {
        tokens.Remove(token);
        SetTheTokenScaleAndPosition();
        token.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void SetTheTokenScaleAndPosition()
    {
        int playersCount = Tokens.Count;
        bool isOdd = (playersCount % 2) == 0 ? false : true;
        int sortingLayer = 5;
        int extent = playersCount / 2;
        int counter = 0;


        if (isOdd)
        {
            for (int i = -extent; i <= extent; i++)
            {
                Tokens[counter].transform.localScale = new Vector3(pathManager.scale[playersCount - 1], pathManager.scale[playersCount - 1], 1f);
                if (isVertical)
                    Tokens[counter].transform.position = new Vector3(transform.position.x + (i * pathManager.positions[playersCount - 1]), transform.position.y - 0.2f, 1f);
                else
                    Tokens[counter].transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y + (i * pathManager.positions[playersCount - 1]), 1f);
                counter++;
            }
        }
        else
        {
            for (int i = -extent; i < extent; i++)
            {
                Tokens[counter].transform.localScale = new Vector3(pathManager.scale[playersCount - 1], pathManager.scale[playersCount - 1], 1f);
                if (isVertical)
                    Tokens[counter].transform.position = new Vector3(transform.position.x + (i * pathManager.positions[playersCount - 1]), transform.position.y - 0.2f, 1f);
                else
                    Tokens[counter].transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y + (i * pathManager.positions[playersCount - 1]), 1f);
                counter++;
            }
        }

        for (int i = 0; i < Tokens.Count; i++)
        {
            Tokens[i].GetComponent<SpriteRenderer>().sortingOrder = sortingLayer;
            sortingLayer++;
        }

    }

    /*public float SetOffset(Token.TokenType type)
    {
        float offset = 0f;
        switch (type)
        {
            case Token.TokenType.Blue:

                break;
            case Token.TokenType.Red:

                break;
            case Token.TokenType.Yellow:
                offset = 0.2f;
                break;
        }
        return offset;
    }*/
}
