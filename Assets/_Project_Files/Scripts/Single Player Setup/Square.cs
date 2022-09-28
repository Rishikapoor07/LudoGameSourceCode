using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{

	public bool isSafe = false;
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
	}

	public void AddToken(Token token)
	{
		tokens.Add(token);
	}

	public void RemoveToken(Token token)
	{
		tokens.Remove(token);
		token.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}
