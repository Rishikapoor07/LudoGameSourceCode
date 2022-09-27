using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokensRadioGroup : MonoBehaviour {

	public delegate void OnTokenTypeSelected(Token.TokenType type);
	public event OnTokenTypeSelected onTokenTypeSelected;

	[SerializeField] private Toggle blueToggle;
	[SerializeField] private Toggle redToggle;
	[SerializeField] private Toggle greenToggle;
	[SerializeField] private Toggle yellowToggle;

	void Start ()
	{
		CallTokenSelected((Token.TokenType)Random.Range(0, 3));
	}


	void CallTokenSelected (Token.TokenType type)
	{
		if (onTokenTypeSelected != null) {
			onTokenTypeSelected (type);
		}
	}
}
