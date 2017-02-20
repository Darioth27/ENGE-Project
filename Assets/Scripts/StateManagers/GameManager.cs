using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private static GameManager instance;

	private string activeLevel;
	private int partyMembers;	//The amount of party members in party.

	private CharacterData player1Data;
	private CharacterData player2Data;
	
	public static GameManager Instance 
	{
		get 
		{
			if(instance==null) 
			{
				instance = new GameObject().AddComponent<GameManager>();
				DontDestroyOnLoad(instance);
				instance.startState ();
			}
			return instance;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad (instance);
	}

	public void startState()
	{
		player1Data = new CharacterData (8, 6, 4, 5, 3, 1);	//Sword Guy
		player2Data = new CharacterData (5, 3, 5, 7, 8, 2);	//Mage Girl
		partyMembers = 2;
		activeLevel = "Overworld";
	}

	public void OnApplicationQuit() 
	{ 
		instance = null; 
	}

	public string getLevel()
	{
		return activeLevel;
	}

	public void setLevel(string lvl)
	{
		activeLevel = lvl;
	}

	public CharacterData P1Data
	{
		get { return player1Data; }
		set { player1Data = value; }
	}

	public CharacterData P2Data
	{
		get { return player2Data; }
		set { player2Data = value; }
	}
}
