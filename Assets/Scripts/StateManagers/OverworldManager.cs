using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour {

	public GameObject player;
	public AudioClip[] sounds;
	private AudioSource audioSource;

	private string[] menuOptions;
	private int menuIndex;

	private Rect windowSize;
	private Rect statArea;
	private Rect statArea2;

	private bool enteringEncounter;
	private bool showStatWindow;
	private bool isPlayer2;

	void Awake () 
	{
		player = GameObject.Find ("Character");
		audioSource = GetComponent<AudioSource> ();
		menuOptions = new string[6];
		menuOptions[0] = "Switch";
		menuOptions[1] = "LavaZone";
		menuOptions[2] = "BossBattle";
		menuOptions[3] = "ForestBattle";
		menuOptions[4] = "DefaultBattle";
		menuOptions[5] = "Exit";
		menuIndex = 0;

		enteringEncounter = false;
		showStatWindow = false;
		isPlayer2 = false;

		windowSize = new Rect (40, 40, Screen.width - 80, Screen.height - 80);
		statArea = new Rect (windowSize.x + 10, windowSize.y + 10, windowSize.width - 40, windowSize.height - 40);
		statArea2 = new Rect (statArea.x + 200, statArea.y + 10, statArea.width / 2, statArea.height);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M) )
		{
			showStatWindow = !showStatWindow;
			if (!showStatWindow)
			{
				playSound (1);
				player.GetComponent<PlayerAnimation>().enabled = true;
			}
			else
			{
				playSound(0);
				player.GetComponent<PlayerAnimation>().stopMovement ();
				player.GetComponent<PlayerAnimation>().enabled = false;
			}
		}
		if (showStatWindow)
		{
			selectAction ();
		}

		if (Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			randomEncounter (350);
		}
		else if (Input.GetKey (KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			randomEncounter (350);
		}
		else if (Input.GetKey (KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
			randomEncounter (350);
		}
		else if (Input.GetKey (KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
			randomEncounter (350);
		}
	}

	void OnGUI () {

		if (showStatWindow)
		{
			windowSize = GUI.Window (0, windowSize, DisplayStatWindow, data ().Name + "'s Stats");
			GUI.Label (new Rect(Screen.width - 160, Screen.height - 40, 150, 30),
			           "Press 'M' again to close");
		}
		else
		{
			GUI.Label (new Rect(Screen.width - 160, Screen.height - 40, 150, 30),
				           "Press 'M' to open menu");
		}

		if (enteringEncounter)
		{
			if (Input.GetKeyDown (KeyCode.Z) || GUI.Button (new Rect (Screen.width / 2 - 70, Screen.height / 2 + 20, 140, 40), "Random Encounter!")) 
			{ 
				GameManager.Instance.setLevel("Forest Battle Scene"); 
				Application.LoadLevel("BattleForest"); 
			}
		}

	}

	void DisplayStatWindow (int windowID)
	{
		if (!isPlayer2)
		{
		GUI.Label (statArea, "Level: " + data ().Level + "\n" +
						"EXP Till Next Level: " + data ().CurrentEXP + "/" + data ().ExpNeeded + "\n" +
						"Total EXP: " + data ().TotalEXP + "\n" +
						"\n" +
						"HP: " + data ().getCurrentHealth () + "/" + data ().getMaxHealth () + "\n" +
						"MP: " + data ().getCurrentMana () + "/" + data ().getMaxMana () + "\n" +
						"\n" +
						"Attributes-------------------------\n" +
						"Constitution: " + data ().getAttribute (AttributeName.CON).Amount + "\n" +
						"Strength: " + data ().getAttribute (AttributeName.STR).Amount + "\n" +
						"Dexterity: " + data ().getAttribute (AttributeName.DEX).Amount + "\n" +
						"Intelligence: " + data ().getAttribute (AttributeName.INT).Amount + "\n" +
						"Mind: " + data ().getAttribute (AttributeName.MND).Amount + "\n" +
						"-----------------------------------");
		GUI.Label (statArea2, "Stats-------------------------\n" +
		           "Attack: " + data ().Attack.BaseValue + "\n" +
		           "Magic Attack: " + data ().MagicAttack.BaseValue + "\n" +
		           "\n" +
		           "Defense: " + data ().Defense.BaseValue + "\n" +
		           "Magic Defense: " + data ().MagicDefense.BaseValue + "\n" +
		           "\n" +
		           "Accuracy: " + data ().Accuracy.BaseValue + "\n" +
		           "Critical Chance: " + data ().Critical.BaseValue + "\n" +
		           "Dodge Chance: " + data ().Dodge.BaseValue + "\n" +
		           "Speed: " + data ().Speed.BaseValue + "\n" +
		           "--------------------------------");

			GUI.SetNextControlName ("Switch");
			if ((menuIndex == 0 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (statArea.width - 150, statArea.height - 220, 150, 30), "Virginia's Stats")) 
			{
				isPlayer2 = true;
			}
		}
		else
		{
			GUI.Label (statArea, "Level: " + data ().Level + "\n" +
			           "EXP Till Next Level: " + data ().CurrentEXP + "/" + data ().ExpNeeded + "\n" +
			           "Total EXP: " + data ().TotalEXP + "\n" +
			           "\n" +
			           "HP: " + data ().getCurrentHealth () + "/" + data ().getMaxHealth () + "\n" +
			           "MP: " + data ().getCurrentMana () + "/" + data ().getMaxMana () + "\n" +
			           "\n" +
			           "Attributes-------------------------\n" +
			           "Constitution: " + data ().getAttribute (AttributeName.CON).Amount + "\n" +
			           "Strength: " + data ().getAttribute (AttributeName.STR).Amount + "\n" +
			           "Dexterity: " + data ().getAttribute (AttributeName.DEX).Amount + "\n" +
			           "Intelligence: " + data ().getAttribute (AttributeName.INT).Amount + "\n" +
			           "Mind: " + data ().getAttribute (AttributeName.MND).Amount + "\n" +
			           "-----------------------------------");
			GUI.Label (statArea2, "Stats-------------------------\n" +
			           "Attack: " + data ().Attack.BaseValue + "\n" +
			           "Magic Attack: " + data ().MagicAttack.BaseValue + "\n" +
			           "\n" +
			           "Defense: " + data ().Defense.BaseValue + "\n" +
			           "Magic Defense: " + data ().MagicDefense.BaseValue + "\n" +
			           "\n" +
			           "Accuracy: " + data ().Accuracy.BaseValue + "\n" +
			           "Critical Chance: " + data ().Critical.BaseValue + "\n" +
			           "Dodge Chance: " + data ().Dodge.BaseValue + "\n" +
			           "Speed: " + data ().Speed.BaseValue + "\n" +
			           "--------------------------------");

			GUI.SetNextControlName ("Switch");
			if ((menuIndex == 0 && Input.GetKeyDown (KeyCode.Z)) ||
			    GUI.Button (new Rect (statArea.width - 150, statArea.height - 220, 150, 30), "Tech's Stats")) 
			{
				isPlayer2 = false;
			}
		}

		GUI.SetNextControlName ("LavaZone");
		if ((menuIndex == 1 && Input.GetKeyDown (KeyCode.Z)) ||
		    GUI.Button (new Rect (statArea.width - 150, statArea.height - 180, 150, 30), "Go to Lava Zone")) 
		{
			GameManager.Instance.setLevel("BridgeOverworld"); 
			Application.LoadLevel("BridgeOverworld"); 
		}

		GUI.SetNextControlName ("BossBattle");
		if ((menuIndex == 2 && Input.GetKeyDown (KeyCode.Z)) ||
		    GUI.Button (new Rect (statArea.width - 150, statArea.height - 140, 150, 30), "Enter Boss Battle")) 
		{
			GameManager.Instance.setLevel("BossBattle"); 
			Application.LoadLevel("BattleBoss");  
		}

		GUI.SetNextControlName ("ForestBattle");
		if ((menuIndex == 3 && Input.GetKeyDown (KeyCode.Z)) ||
		    GUI.Button (new Rect (statArea.width - 150, statArea.height - 100, 150, 30), "Enter Forest Battle")) 
		{
			GameManager.Instance.setLevel("Forest Battle Scene"); 
			Application.LoadLevel("BattleForest"); 
		}

		GUI.SetNextControlName ("DefaultBattle");
		if ((menuIndex == 4 && Input.GetKeyDown (KeyCode.Z)) ||
		    GUI.Button (new Rect (statArea.width - 150, statArea.height - 60, 150, 30), "Enter Default Battle")) 
		{ 
			GameManager.Instance.setLevel("Battle Scene"); 
			Application.LoadLevel("BattleCommon"); 
		}

		GUI.SetNextControlName ("Exit");
		if ((menuIndex == 5 && Input.GetKeyDown (KeyCode.Z)) ||
		    GUI.Button (new Rect (statArea.width - 150, statArea.height - 20, 150, 30), "Exit Game")) 
		{
			Application.Quit (); 
		}

		GUI.FocusControl (menuOptions[menuIndex]);
	}

	private void randomEncounter(int rate)
	{
		if (!showStatWindow)
		{
			if (Random.Range (0, rate) == 1)
			{
				enteringEncounter = true;
				player.GetComponent<PlayerAnimation>().stopMovement ();
				player.GetComponent<PlayerAnimation>().enabled = false;
			}
		}
	}
	
	public CharacterData data()
	{
		if (!isPlayer2)
		{
			return GameManager.Instance.P1Data;
		}
		return GameManager.Instance.P2Data;
	}

	private void selectAction()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow))
		{
			if (menuIndex == 0)
			{
				menuIndex = menuOptions.Length - 1;
			}
			else
			{
				menuIndex--;
			}
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow))
		{
			if (menuIndex == menuOptions.Length - 1)
			{
				menuIndex = 0;
			}
			else
			{
				menuIndex++;
			}
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.Z))
		{
			playSound (2);
		}
	}

	public void playSound(int index)
	{
		audioSource.clip = sounds[index];
		audioSource.Play();
	}
}
