using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlighter : MonoBehaviour
{

	[SerializeField] private GameObject blueHighlight;
	[SerializeField] private GameObject redHighlight;
	[SerializeField] private GameObject yellowHighlight;
	private HomeBaseManager homeBaseManager;

	public void Init(HomeBaseManager homeBaseManager)
	{
		this.homeBaseManager = homeBaseManager;
	}
	public void Highlight(Token.TokenType section)
	{
		switch (section)
		{
			case Token.TokenType.Blue:
				blueHighlight.SetActive(true);
				//HighLightCurrentSelectedTokens(homeBaseManager.BlueHomeBase);
				CurrentPlayerNumber = 1;
				break;
			case Token.TokenType.Red:
				redHighlight.SetActive(true);
                //HighLightCurrentSelectedTokens(homeBaseManager.RedHomeBase);
                currentPlayerNumber = 2;
				break;
			case Token.TokenType.Yellow:
				yellowHighlight.SetActive(true);
                //HighLightCurrentSelectedTokens(homeBaseManager.YellowHomeBase);
                currentPlayerNumber = 3;
				break;
		}
		StartPlayerTurn();
	}

	public void StopHighlight()
	{
		blueHighlight.SetActive(false);
		redHighlight.SetActive(false);
		yellowHighlight.SetActive(false);
		StopTurn();
	}

	public void HighLightCurrentSelectedTokens(Transform[] currentSelectedToken)
	{
		foreach (var token in currentSelectedToken)
		{
			token.localScale = new Vector3(1.5f, 1.5f, 1.5f);
		}
	}
    #region Turn related

    private void Awake()
    {
		StopTurn();
    }

    public UnityEngine.UI.Image[] playersTimerImage;
	[SerializeField] private int currentPlayerNumber = 1;
	public int CurrentPlayerNumber
	{
		get => currentPlayerNumber;
		set => currentPlayerNumber = value;
	}
	[SerializeField] private bool startTurnTimer;
	private float turnTimer;
	public float TurnTimer
	{
		get => turnTimer;
		set
		{
			turnTimer = value;
			playersTimerImage[CurrentPlayerNumber - 1].fillAmount = turnTimer / 60;
		}
	}
	private Coroutine turnTimerCoroutine;

	public delegate void OnTurnTimerFinished();
	public OnTurnTimerFinished OnTurnTimerComplete;

	private void StartPlayerTurn()
	{
		if (turnTimerCoroutine != null)
		{
			StopCoroutine(turnTimerCoroutine);
			turnTimerCoroutine = null;
		}
		turnTimerCoroutine = StartCoroutine(StartTurnTimer());
	}

	private IEnumerator StartTurnTimer()
	{
		startTurnTimer = true;
		TurnTimer = 60;
		float timerValue = TurnTimer;

		while (startTurnTimer)
		{
			if (TurnTimer > 0.2f)
			{
				timerValue -= Time.deltaTime;
				TurnTimer = timerValue;
			}
			else
			{
				TurnTimer = 0;
				startTurnTimer = false;
				OnTurnTimerComplete?.Invoke();
				yield return null;
			}
			yield return new WaitForSeconds(0.001f);
		}
	}

	private void StopTurn()
	{
		if (turnTimerCoroutine != null)
		{
			StopCoroutine(turnTimerCoroutine);
			turnTimerCoroutine = null;
		}
		TurnTimer = 0;
		foreach (UnityEngine.UI.Image timerFillImage in playersTimerImage) timerFillImage.fillAmount = 0;
	}

	#endregion Turn Related
}