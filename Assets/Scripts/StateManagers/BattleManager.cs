using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {
	
	public GameObject player1;
	public GameObject player2;
	public GameObject enemy;

	private Character p1;
	private Character p2;
	private Enemy[] enemies;
	
	public AudioClip[] sounds;
	private AudioSource audioSource;

	//General Images/Textures
	public Texture healthBar;
	public Texture manaBar;

	//Player1 Textures and Properties
	public Texture p1BarBackground;
	public Texture p1Profile;
	public Texture p2Profile;

	#region GUI Rectangle Creation
	private Rect p1BackSize;
	private Rect p1HealthSize;
	private Rect p1HealthLabel;
	private Rect p1ManaSize;
	private Rect p1ManaLabel;
	private Rect p1ProfileSize;
	private Rect p1Name;
	
	private Rect p2BackSize;
	private Rect p2HealthSize;
	private Rect p2HealthLabel;
	private Rect p2ManaSize;
	private Rect p2ManaLabel;
	private Rect p2ProfileSize;
	private Rect p2Name;
	#endregion

	public GameObject cursor;
	private AnimationOffset selector;

	//private GameObject[] turnOrder;
	private Turn turn;
	private Turn nextTurn;
	private int selectedSlot;
	private int timer;

	private string[] menuOptions;
	private int menuIndex;

	private int totalExpReward;

	void Awake()
	{
		setUp ();
		audioSource = GetComponent<AudioSource>();

		#region Create Rectangles for GUI Images
		p1BackSize = new Rect (Screen.width / 12.4f - 20, Screen.height / 21, 
		                       Screen.width / 6.9f, Screen.height / 11.2f);
		p1HealthSize = new Rect (Screen.width / 10 - 20, Screen.height / 20, 
		                         Screen.width / 8, Screen.height / 30);
		p1ManaSize = new Rect (Screen.width / 10 - 20, Screen.height * 0.083f, 
		                       Screen.width / 8, Screen.height / 30);
		p1ProfileSize = new Rect (Screen.width / 22 - 20, Screen.height / 50, 
		                          Screen.width / 13.2f, Screen.width / 13.2f);
		p1Name = new Rect (p1ProfileSize.x - 10, p1ProfileSize.yMax - 10, 100, 20);
		p1HealthLabel = new Rect (p1BackSize.xMax + 5, p1HealthSize.y, Screen.width / 10, p1HealthSize.height*2);
		p1ManaLabel = new Rect (p1BackSize.xMax + 5, p1ManaSize.y, Screen.width / 10, p1ManaSize.height*2);

		p2BackSize = new Rect(p1BackSize.x +  2 * Screen.width / 6.9f, p1BackSize.y, 
		                      Screen.width / 6.9f, Screen.height / 11.2f);
		p2HealthSize = new Rect (p1HealthSize.x + 2 * Screen.width/6.9f, p1HealthSize.y, 
		                         Screen.width / 8, Screen.height / 30);
		p2ManaSize = new Rect (p1ManaSize.x + 2 * Screen.width/6.9f, p1ManaSize.y, 
		                       Screen.width / 8, Screen.height / 30);
		p2ProfileSize = new Rect (p1ProfileSize.x + 2 * Screen.width / 6.9f, p1ProfileSize.y, 
		                          Screen.width / 13.2f, Screen.width / 13.2f);
		p2HealthLabel = new Rect (p2BackSize.xMax + 5, p2HealthSize.y, Screen.width / 10, p2HealthSize.height*2);
		p2ManaLabel = new Rect (p2BackSize.xMax + 5, p2ManaSize.y, Screen.width / 10, p2ManaSize.height*2);
		#endregion
	}

	void Start()
	{
		print("Loaded: " + GameManager.Instance.getLevel());
	}

	void Update()
	{
		manageTurns ();

		if (turn == Turn.P1Choice || turn == Turn.P2Choice)
		{
			selectAction ();
		}

		if (turn == Turn.P1Target || turn == Turn.P2Target)
		{
			selectTarget();
		}

	}

		//GUI properties
	void OnGUI() 
	{
		#region Draw Static GUI objects
		//Player1 GUI
		GUI.DrawTexture(p1BackSize, p1BarBackground, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect (p1HealthSize.x, p1HealthSize.y, 
		                          (Screen.width / 8) * percentRemaining (GameManager.Instance.P1Data, 'h'),
		                          Screen.height / 30), healthBar, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect (p1ManaSize.x, p1ManaSize.y, 
		                          (Screen.width / 8) * percentRemaining (GameManager.Instance.P1Data, 'm'), 
		                          Screen.height / 30), manaBar, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(p1ProfileSize, p1Profile, ScaleMode.StretchToFill, true, 0);
		//GUI.Label (p1Name, GameManager.Instance.P1Data.Name);

		GUI.Label (p1HealthLabel, GameManager.Instance.P1Data.getCurrentHealth () 
		           + "/" + GameManager.Instance.P1Data.getMaxHealth ());
		GUI.Label (p1ManaLabel, GameManager.Instance.P1Data.getCurrentMana () 
		           + "/" + GameManager.Instance.P1Data.getMaxMana ());
		//Player2 GUI
		GUI.DrawTexture(p2BackSize, p1BarBackground, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect (p2HealthSize.x, p2HealthSize.y, 
		                          (Screen.width / 8) * percentRemaining (GameManager.Instance.P2Data, 'h'),
		                          Screen.height / 30), healthBar, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(new Rect (p2ManaSize.x, p2ManaSize.y, 
		                          (Screen.width / 8) * percentRemaining (GameManager.Instance.P2Data, 'm'), 
		                          Screen.height / 30), manaBar, ScaleMode.StretchToFill, true, 0);
		GUI.DrawTexture(p2ProfileSize, p2Profile, ScaleMode.StretchToFill, true, 0);
		GUI.Label (p2HealthLabel, GameManager.Instance.P2Data.getCurrentHealth () 
		           + "/" + GameManager.Instance.P2Data.getMaxHealth ());
		GUI.Label (p2ManaLabel, GameManager.Instance.P2Data.getCurrentMana () 
		           + "/" + GameManager.Instance.P2Data.getMaxMana ());
		#endregion

		#region Player 1 Selection Menu
		if (turn == Turn.P1Choice)
		{
			//Player 1 Profile
			GUI.DrawTexture(new Rect(Screen.width / 2 - (2.88f * Screen.width / 10.6f), Screen.height * 0.84f, 
			                         Screen.width / 10.6f, Screen.width / 10.6f), 
			                p1Profile, ScaleMode.StretchToFill, true, 0);
			//Attack Button
			GUI.SetNextControlName ("Attack");
			if ((menuIndex == 0 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (Screen.width / 2 - (2 * Screen.width / 10.6f), Screen.height * 0.88f, 
			                          Screen.width / 10.6f, Screen.height / 16f), "Attack")) 
			{
				timer = 1;
				p1.setNextAction(1);
				nextTurn = Turn.P1Target;
				turn = Turn.Transition;
				selectedSlot = 0;
				if (enemies[0].isDown())
				{
					selectedSlot = 1;
					if (enemies[1].isDown())
					{
						selectedSlot = 2;
					}
				}
				enemies[selectedSlot].Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (enemies[selectedSlot].gameObject);
				selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
				playSound (0);
			} 
			//Magic Button
			GUI.SetNextControlName ("Magic");
			if ((menuIndex == 1 && Input.GetKeyDown (KeyCode.Z)) ||
			    GUI.Button (new Rect (Screen.width / 2 - Screen.width / 10.6f, Screen.height * 0.88f, 
		                          	  Screen.width / 10.6f, Screen.height / 16f), "Magic")) 
			{ 
				timer = 1;
				p1.setNextAction(2);
				nextTurn = Turn.P1Target;
				turn = Turn.Transition;
				selectedSlot = 0;	if (enemies[0].isDown())
				{
					selectedSlot = 1;
					if (enemies[1].isDown())
					{
						selectedSlot = 2;
					}
				}
				enemies[selectedSlot].Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (enemies[selectedSlot].gameObject);
				selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
				playSound (0);
			}
			//Items Button
			GUI.SetNextControlName ("Items");
			if (menuIndex == 2 && Input.GetKeyDown (KeyCode.Z) ||
				GUI.Button (new Rect (Screen.width / 2, Screen.height * 0.88f, 
		 	                    	  Screen.width / 10.6f, Screen.height / 16f), "Items")) 
			{ 
				timer = 5;
				p1.setNextAction(3);
				if (!p2.isDown ())
				{
					nextTurn = Turn.P2Choice;
					turn = Turn.Transition;
				}
				else
				{
					nextTurn = Turn.CalculationPhase;
					turn = Turn.Transition;
				}
				playSound (0);
			} 
			//Flee Button
			GUI.SetNextControlName ("Flee");
			if ((menuIndex == 3 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (Screen.width / 2 + Screen.width / 10.6f, Screen.height * 0.88f, 
		                        	  Screen.width / 10.6f, Screen.height / 16f), "Flee")) 
			{ 
				playSound (2);
				GameManager.Instance.setLevel("Overworld Map"); 
				Application.LoadLevel("Overworld"); 
				print ("You flee the battle");
			} 

			GUI.FocusControl (menuOptions[menuIndex]);

		}
		#endregion

		#region Player 2 Selection Menu
		if (turn == Turn.P2Choice)
		{
			//Player2 Profile
			GUI.DrawTexture(new Rect(Screen.width / 2 - (2.88f * Screen.width / 10.6f), Screen.height * 0.84f, 
			                         Screen.width / 10.6f, Screen.width / 10.6f), 
			                p2Profile, ScaleMode.StretchToFill, true, 0);
			//Attack Button
			GUI.SetNextControlName ("Attack");
			if ((menuIndex == 0 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (Screen.width / 2 - (2 * Screen.width / 10.6f), Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Attack")) 
			{
				timer = 1;
				p2.setNextAction(1);
				nextTurn = Turn.P2Target;
				turn = Turn.Transition;
				selectedSlot = 0;
				if (enemies[0].isDown())
				{
					selectedSlot = 1;
					if (enemies[1].isDown())
					{
						selectedSlot = 2;
					}
				}
				enemies[selectedSlot].Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (enemies[selectedSlot].gameObject);
				selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
				playSound (0);
			} 
			//Magic Button
			GUI.SetNextControlName ("Magic");
			if ((menuIndex == 1 && Input.GetKeyDown (KeyCode.Z)) ||
			    GUI.Button (new Rect (Screen.width / 2 - Screen.width / 10.6f, Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Magic")) 
			{ 
				timer = 1;
				p2.setNextAction(2);
				nextTurn = Turn.P2Target;
				turn = Turn.Transition;
				selectedSlot = 0;	if (enemies[0].isDown())
				{
					selectedSlot = 1;
					if (enemies[1].isDown())
					{
						selectedSlot = 2;
					}
				}
				enemies[selectedSlot].Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (enemies[selectedSlot].gameObject);
				selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
				playSound (0);
			}
			//Items Button
			GUI.SetNextControlName ("Items");
			if (menuIndex == 2 && Input.GetKeyDown (KeyCode.Z) ||
			    GUI.Button (new Rect (Screen.width / 2, Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Items")) 
			{ 
				timer = 5;
				playSound (0);
				p2.setNextAction(3);
				nextTurn = Turn.CalculationPhase;
				turn = Turn.Transition;
			} 
			//Flee Button
			GUI.SetNextControlName ("Flee");
			if ((menuIndex == 3 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (Screen.width / 2 + Screen.width / 10.6f, Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Flee")) 
			{ 
				playSound (2);
				GameManager.Instance.setLevel("Overworld Map"); 
				Application.LoadLevel("Overworld"); 
				print ("You flee the battle");
			} 
			
			GUI.FocusControl (menuOptions[menuIndex]);
			
		}
		#endregion

		if (turn == Turn.GameOver)
		{
			GUI.Label(new Rect (Screen.width/2 - 50, Screen.height/2 + 50, 120, 100), "You Died.\n\n Game Over");

			GUI.SetNextControlName ("StartOver");
			GUI.FocusControl ("StartOver");
			if (Input.GetKeyDown (KeyCode.Z) ||
			    GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 40), "Start Over")) 
			{
				GameManager.Instance.setLevel ("Start Screen");
				Application.LoadLevel ("StartScreen");
			}

		}

		if (turn == Turn.Victory)
		{
			GUI.Label(new Rect (Screen.width/2 - 50, Screen.height/2 + 50, 120, 100), "You won!");
			
			GUI.SetNextControlName ("ReturnToMap");
			if (Input.GetKeyDown (KeyCode.Z) ||
			    GUI.Button (new Rect (Screen.width / 2 - 80, Screen.height / 2 - 50, 160, 40), "Back to Overworld")) 
			{
				GameManager.Instance.setLevel ("Overworld Map");
				Application.LoadLevel ("Overworld");
			}
			GUI.FocusControl ("ReturnToMap");
		}
	}

	#region Turn Management
	private void manageTurns()
	{
		switch (turn)
		{
		case Turn.P1Choice:
			break;
		case Turn.P1Target:
			break;
		case Turn.P2Choice:
			break;
		case Turn.P2Target:
			break;
		case Turn.CalculationPhase:
			calculatePhase();
			break;
		case Turn.P1Attack:
			p1.PerformAction ();
			turn = Turn.DelayPhaseP1;
			break;
		case Turn.DelayPhaseP1:
			if (p1.State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.P2Attack:
			p2.PerformAction ();
			turn = Turn.DelayPhaseP2;
			break;
		case Turn.DelayPhaseP2:
			if (p2.State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.E1Attack:
			if (Random.Range (0, 2) == 0 && !p1.isDown ())
			{
				enemies[0].setTarget(p1);
			}
			else if (!p2.isDown ())
			{
				enemies[0].setTarget(p2);
			}
			else
			{
				enemies[0].setTarget (p1);
			}
			enemies[0].PerformAction();
			turn = Turn.DelayPhaseE1;
			break;
		case Turn.DelayPhaseE1:
			if (enemies[0].State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.E2Attack:
			if (Random.Range (0, 2) == 0 && !p1.isDown ())
			{
				enemies[1].setTarget(p1);
			}
			else if (!p2.isDown ())
			{
				enemies[1].setTarget(p2);
			}
			else
			{
				enemies[1].setTarget (p1);
			}
			enemies[1].PerformAction();
			turn = Turn.DelayPhaseE2;
			break;
		case Turn.DelayPhaseE2:
			if (enemies[1].State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.E3Attack:
			if (Random.Range (0, 2) == 0 && !p1.isDown ())
			{
				enemies[2].setTarget(p1);
			}
			else if (!p2.isDown ())
			{
				enemies[2].setTarget(p2);
			}
			else
			{
				enemies[2].setTarget (p1);
			}
			enemies[2].PerformAction();
			turn = Turn.DelayPhaseE3;
			break;
		case Turn.DelayPhaseE3:
			if (enemies[2].State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.Transition:
			if (timer > 0)
				timer -= 1;
			else
				turn = nextTurn;
			break;
		case Turn.GameOver:
			break;
		default:
			break;
		}
	}
	#endregion

	private void calculatePhase()
	{
		if (p1.HasAttacked == false && !p1.isDown ()) {turn = Turn.P1Attack;}
		else if (p2.HasAttacked == false && !p2.isDown ()) {turn = Turn.P2Attack;}
		else if (enemies[0].HasAttacked == false && !enemies[0].isDown()) 
			{turn = Turn.E1Attack;}
		else if (enemies[1].HasAttacked == false && !enemies[1].isDown()) 
			{turn = Turn.E2Attack;}
		else if (enemies[2].HasAttacked == false && !enemies[2].isDown()) 
			{turn = Turn.E3Attack;}
		else {
			enemies[0].HasAttacked = false;
			enemies[1].HasAttacked = false;
			enemies[2].HasAttacked = false;
			p1.HasAttacked = false;
			p2.HasAttacked = false;
			if (!p1.isDown ())
			{
				turn = Turn.P1Choice;
			}
			else
			{
				turn = Turn.P2Choice;
			}
		}
		if (enemies[0].isDown() && enemies[1].isDown() && enemies[2].isDown())
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				totalExpReward += enemies[i].data ().getExpYield();
			}
			GameManager.Instance.P1Data.updateEXP(totalExpReward);
			GameManager.Instance.P1Data.updateLevel ();
			GameManager.Instance.P2Data.updateEXP(totalExpReward);
			GameManager.Instance.P2Data.updateLevel ();
			nextTurn = Turn.Victory;
			timer = 100;
			turn = Turn.Transition;
		}
		if (p1.isDown () && p2.isDown())
		{
			turn = Turn.GameOver;
		}
	}

	#region Actionbar Navigation
	private void selectAction()
	{
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow))
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
		else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow))
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
		else if (Input.GetKeyDown (KeyCode.X))
		{
			if (turn == Turn.P2Choice)
			{
				turn = Turn.P1Choice;
				playSound (1);
			}
		}
	}
	#endregion

	#region SelectTarget
	private void selectTarget()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.LeftArrow))
		{
			enemies[selectedSlot].Selected = false;
			if (selectedSlot == 0) {
				selectedSlot = enemies.Length - 1;
			} else {
				selectedSlot -= 1;
			}
			enemies[selectedSlot].Selected = true;
			selector.setParent (enemies[selectedSlot].transform);
			selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
			if (enemies[selectedSlot].isDown())
			{
				selectTarget ();
			}
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.RightArrow))
		{
			enemies[selectedSlot].Selected = false;
			if (selectedSlot == enemies.Length -1) {
				selectedSlot = 0;
			} else {
				selectedSlot += 1;
			}
			enemies[selectedSlot].Selected = true;
			selector.setParent (enemies[selectedSlot].transform);
			selector.setOffset (0, enemies[selectedSlot].gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
			if (enemies[selectedSlot].isDown())
			{
				selectTarget ();
			}
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.Z))
		{
			if (turn == Turn.P1Target)
			{
				p1.setTarget (enemies[selectedSlot]);
				if (!p2.isDown ())
				{
					nextTurn = Turn.P2Choice;
					turn = Turn.Transition;
				}
				else
				{
					nextTurn = Turn.CalculationPhase;
					turn = Turn.Transition;
				}
			}
			else if (turn == Turn.P2Target)
			{
				p2.setTarget (enemies[selectedSlot]);
				nextTurn = Turn.CalculationPhase;
				turn = Turn.Transition;
			}
			enemies[selectedSlot].Selected = false;
			selector.gameObject.GetComponent<Renderer>().enabled = false;
			playSound (2);
		}
		else if (Input.GetKeyDown (KeyCode.X))
		{
			enemies[selectedSlot].Selected = false;
			selector.gameObject.GetComponent<Renderer>().enabled = false;
			if (turn == Turn.P1Target)
			{
				turn = Turn.P1Choice;
			}
			else if (turn == Turn.P2Target)
			{
				turn = Turn.P2Choice;
			}
			playSound (1);
		}
	}
	#endregion

	public void playSound(int index)
	{
		audioSource.clip = sounds[index];
		audioSource.Play();
	}

	#region Set Up
	private void setUp()
	{
		//Sets up the players and enemies
		GameObject character1 = Instantiate (player1) as GameObject;
		GameObject character2 = Instantiate (player2) as GameObject;
		p1 = character1.GetComponent<Character> ();
		p2 = character2.GetComponent<Character> ();
		enemies = new Enemy[3];
		GameObject e1 = Instantiate (enemy) as GameObject;
		GameObject e2 = Instantiate (enemy) as GameObject;
		GameObject e3 = Instantiate (enemy) as GameObject;
		enemies [0] = e1.GetComponent<Enemy>();
		enemies [1] = e2.GetComponent<Enemy>();
		enemies [2] = e3.GetComponent<Enemy>();
		enemies [0].Slot = 1;
		enemies [1].Slot = 2;
		enemies [2].Slot = 3;
		enemies [0].setSlot ();
		enemies [1].setSlot ();
		enemies [2].setSlot ();
		//turnOrder = new GameObject[4];
		
		//Manages enemy selection
		selectedSlot = 0;
		GameObject theCursor = Instantiate (cursor) as GameObject;
		theCursor.GetComponent<Renderer>().enabled = false;
		selector = theCursor.GetComponent<AnimationOffset>();
		selector.setParent (enemies[selectedSlot].gameObject);

		menuOptions = new string[4];
		menuOptions[0] = "Attack";
		menuOptions[1] = "Magic";
		menuOptions[2] = "Items";
		menuOptions[3] = "Flee";
		menuIndex = 0;

		turn = Turn.P1Choice;
		nextTurn = Turn.P1Choice;

		totalExpReward = 0;
	}
	#endregion

	private float percentRemaining(CharacterData data, char stat)
	{
		if (stat == 'h')
		{
			return (float)data.getCurrentHealth () / data.getMaxHealth ();
		}
		else if (stat == 'm')
		{
			return (float)data.getCurrentMana ()/data.getMaxMana ();
		}
		else
		{
			print ("Invalid char input.  Please use 'h' for health or 'm' for mana.");
		}
		return 0.0f;
	}

	public void setTurn(Turn t, float time)
	{
		turn = Turn.Transition;
		nextTurn = t;

		Invoke("completeTransition", time);
	}

	private void completeTransition()
	{
		turn = nextTurn;
	}

}

public enum Turn {
	DelayPhaseP1,
	DelayPhaseP2,
	DelayPhaseE1,
	DelayPhaseE2,
	DelayPhaseE3,
	P1Choice,
	P1Target,
	P1Attack,
	P2Choice,
	P2Target,
	P2Attack,
	E1Attack,
	E2Attack,
	E3Attack,
	CalculationPhase,
	Transition,
	Victory,
	GameOver
}