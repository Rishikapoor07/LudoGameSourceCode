using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{

	public delegate void OnDiceRolled(int diceNum);

	public event OnDiceRolled onDiceRolled;

	[SerializeField] private int minDiceNumber = 1;
	[SerializeField] private int maxDiceNumber = 6;
	[SerializeField] private int currentDiceNumber;

	private Animator animator;
	[SerializeField] private bool rolling;
	public bool Rolling
	{
		get => rolling;
		set => rolling = value;
	}
	[SerializeField] private bool enableUserInteraction;
	public bool EnableUserInteraction
	{
		set { enableUserInteraction = value; }
	}

	void Start()
	{
		animator = GetComponent<Animator>();
		Rolling = false;
		enableUserInteraction = true;
	}

	public void RollTheDice()
	{
		if (!enableUserInteraction || Rolling)
		{
			return;
		}
		StartCoroutine(RollDice());
	}

	public IEnumerator RollDice()
	{
		
		if (!Rolling)
		{
			Rolling = true;
			animator.SetTrigger("RollDice");
			animator.SetInteger("DiceNum", 0);
			yield return new WaitForSeconds(1f);

			//rolling = false;
			int num = Random.Range(minDiceNumber, maxDiceNumber + 1);
			animator.SetInteger("DiceNum", num);
			currentDiceNumber = num;

			if (onDiceRolled != null)
			{
				onDiceRolled(num);
			}
		}

		yield return null;
	}
}
