using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public delegate void OnNextTurn(Token.TokenType tokenType);
    public delegate void OnSixThreeTimes(Token.TokenType tokenType);

    public event OnNextTurn onNextTurn;
    public event OnSixThreeTimes onSixThreeTimes;

    //	private Dictionary<Token.TokenType, GameObject[]> tokens;
    //	public Dictionary<Token.TokenType, GameObject[]> Tokens {
    //		get { return tokens; }
    //		set { tokens = value; }
    //	}
    //
    [SerializeField] private TokenManager tokenManager;
    [SerializeField] private Dice dice;
    [SerializeField] private DiceManager diceManager;
    [SerializeField] private OpponentController opponentCtrl;
    [SerializeField] private BoardHighlighter highlighter;
    [SerializeField] private Token.TokenType[] selectedTypes;
    [SerializeField] private int currentToken = 0;
    [SerializeField] private int lastDiceNum = 0;
    [SerializeField] private int diceSixCount = 0;
    [SerializeField] private InGamePlayerData BluePlayerData;
    [SerializeField] private InGamePlayerData RedPlayerData;

    public void Init(TokenManager tokenManager, DiceManager diceManager,
        OpponentController oppCtrl, BoardHighlighter highlighter)
    {
        this.diceManager = diceManager;
        this.opponentCtrl = oppCtrl;
        this.highlighter = highlighter;
        this.tokenManager = tokenManager;

        opponentCtrl.onTokenSelected += TokenSelected;
        opponentCtrl.TokenManager = tokenManager;
        diceManager.onDiceRolled += DiceRolled;
        dice = diceManager.GetCurrentActiveDice();

        tokenManager.onTokenAnimationsDone += TokenAnimationsEnd;

        selectedTypes = new Token.TokenType[tokenManager.Tokens.Keys.Count];
        tokenManager.Tokens.Keys.CopyTo(selectedTypes, 0);

        foreach (KeyValuePair<Token.TokenType, Token[]> entry in tokenManager.Tokens)
        {
            for (int i = 0; i < entry.Value.Length; i++)
            {
                Token t = entry.Value[i].GetComponent<Token>();
                t.onTokenSelected += TokenSelected;
            }
        }

        highlighter.OnTurnTimerComplete = () =>
        {
            NextTurn();
            StartCoroutine(StartTurn());
        };
    }

    /*
	 * memulai giliran pertama
	 */
    public void StartFirstTurn()
    {
        currentToken = 0;
        StartCoroutine(StartTurn());
    }

    /*
	 * memulai giliran
	 */
    public IEnumerator StartTurn()
    {
        /*
		 * delay beberapa ms sebelum player dapat mengocok dadu
		 */
        yield return new WaitForSeconds(0.7f);
        Token.TokenPlayer tokenPlayer = GetCurrentTokenPlayer();
        highlighter.StopHighlight();
        highlighter.Highlight(GetCurrentTokenType());

        diceManager.ShowDice(GetCurrentTokenType());
        dice = diceManager.GetCurrentActiveDice();

        if (tokenPlayer == Token.TokenPlayer.Human)
        {
            /*
			 * aktifkan dadu agar user dapat menyentuh/berinteraksi dengan dadu tsb
			 */
            dice.EnableUserInteraction = true;
            dice.Rolling = false;
        }
        else if (tokenPlayer == Token.TokenPlayer.Computer)
        {

            /*
			 * delay beberapa ms sebelum AI mengocok dadu
			 */
            yield return new WaitForSeconds(0.7f);
            dice.EnableUserInteraction = false;

            opponentCtrl.StartTurn(GetCurrentTokenType());
        }
    }

    /*
	 * men-increment 'currentToken' ke giliran selanjutnya 
	 */
    public void NextTurn()
    {
        diceSixCount = 0;
        int count = selectedTypes.Length;
        currentToken++;
        if (currentToken >= count)
        {
            currentToken = 0;
        }

        if (onNextTurn != null)
        {
            onNextTurn(GetCurrentTokenType());
        }
    }

    /*
	 * method callback. akan dipanggil ketika dadu selesai dikocok
	 * oleh DiceManager
	 */
    public void DiceRolled(int diceNum, Token.TokenType type)
    {
        if (diceNum == 6)
        {
            diceSixCount++;

            if (diceSixCount >= 3)
            {
                dice.EnableUserInteraction = false;
                diceSixCount = 0;

                if (onSixThreeTimes != null)
                {
                    onSixThreeTimes(GetCurrentTokenType());
                }

                NextTurn();
                StartCoroutine(StartTurn());
                return;
            }
        }

        SetLastThreeMoves(type, BluePlayerData, RedPlayerData, diceNum);
        lastDiceNum = diceNum;
        Token.TokenPlayer player = GetCurrentTokenPlayer();
        List<Token> movableTokens = tokenManager.GetMovableTokens(GetCurrentTokenType(), diceNum);

        if (diceNum != 6 && movableTokens.Count <= 0)
        {
            NextTurn();
            StartCoroutine(StartTurn());
            return;
        }



        if (player == Token.TokenPlayer.Human)
        {
            dice.EnableUserInteraction = false;

            if (movableTokens.Count == 1)
            {
                if (!movableTokens[0].MoveToken(diceNum))
                {
                    NextTurn();
                    StartCoroutine(StartTurn());
                }
            }
            else if (movableTokens.Count > 1)
            {
                for (int i = 0; i < movableTokens.Count; i++)
                    movableTokens[i].SelectionMode = true;
            }
            else if (movableTokens.Count <= 0)
            {
                Debug.Log("Re roll the dice!!!");
                NextTurn();
                StartCoroutine(StartTurn());
            }
        }
        else if (player == Token.TokenPlayer.Computer)
        {
            opponentCtrl.Play(diceNum);
        }

    }

    /*
	 * callback untuk event onTokenSelected pada object Token
	 */
    public void TokenSelected(Token selectedToken)
    {
        highlighter.StopHighlight();

        if (selectedToken == null)
        {
            NextTurn();
            StartCoroutine(StartTurn());
            return;
        }

        if (selectedToken.State == Token.TokenState.Locked)
        {
            if (lastDiceNum == 6)
            {
                selectedToken.Unlock();
            }
        }
        else
        {
            if (!selectedToken.MoveToken(lastDiceNum))
            {
                NextTurn();
                StartCoroutine(StartTurn());
            }
        }
    }

    /*
	 * callback untuk event onTokenAnimationEnd pada object TokenManger
	 */
    public void TokenAnimationsEnd()
    {
        if (lastDiceNum != 6)
        {
            NextTurn();
        }

        StartCoroutine(StartTurn());
    }

    // Rishi Code

    public void SetLastThreeMoves(Token.TokenType type, InGamePlayerData blueData = null, InGamePlayerData reddata = null, int dicenum = 0)
    {
        if (type == Token.TokenType.Blue)
        {
            if (blueData.lastThreeMoves.Count >= 3)
                blueData.lastThreeMoves.RemoveAt(0);

            blueData.lastThreeMoves.Add(diceManager.GetCurrentDiceTexture(dicenum));
            for (int i = 0; i < blueData.lastThreeMoves.Count; i++)
            {
                blueData.moves[i].texture = blueData.lastThreeMoves[i];
                blueData.moves[i].gameObject.SetActive(true);
            }
        }

        if (type == Token.TokenType.Red)
        {
            if (reddata.lastThreeMoves.Count >= 3)
                reddata.lastThreeMoves.RemoveAt(0);

            reddata.lastThreeMoves.Add(diceManager.GetCurrentDiceTexture(dicenum));
            for (int i = 0; i < reddata.lastThreeMoves.Count; i++)
            {
                reddata.moves[i].texture = reddata.lastThreeMoves[i];
                reddata.moves[i].gameObject.SetActive(true);
            }
        }
    }

    Token.TokenType GetCurrentTokenType()
    {
        return selectedTypes[currentToken];
    }

    Token.TokenPlayer GetCurrentTokenPlayer()
    {
        return tokenManager.GetTokensOfType(GetCurrentTokenType())[0].GetComponent<Token>().Player;
    }
}