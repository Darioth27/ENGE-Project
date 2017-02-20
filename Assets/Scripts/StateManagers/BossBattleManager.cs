using UnityEngine;
using System.Collections;

public class BossBattleManager : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public GameObject bossObject;

	private Character p1;
	private Character p2;
	private Enemy boss;

	public AudioClip[] sounds;
	private AudioSource audioSource;

	//General Images/Textures
	public Texture healthBar;
	public Texture manaBar;
	
	//Player1 Textures and Properties
	public Texture p1BarBackground;
	public Texture p1Profile;
	public Texture p2Profile;
	
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
	
	public GameObject cursor;
	private AnimationOffset selector;
	
	private GameObject[] turnOrder;
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
		
		//Sets Rectangle Sizes/Locations for Player 1's GUI
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
		
		if (turn == Turn.P1Choice)
		{
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
				boss.Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (boss.gameObject);
				selector.setOffset (0, boss.gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
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
				selectedSlot = 0;
				boss.Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (boss.gameObject);
				selector.setOffset (0, boss.gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
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
			} 
			//Flee Button
			GUI.SetNextControlName ("Flee");
			if ((menuIndex == 3 && Input.GetKeyDown (KeyCode.Z)) || 
			    GUI.Button (new Rect (Screen.width / 2 + Screen.width / 10.6f, Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Flee")) 
			{ 
				playSound (0);
				GameManager.Instance.setLevel("Overworld Map"); 
				Application.LoadLevel("Overworld"); 
				print ("You flee the battle");
			} 
			
			GUI.FocusControl (menuOptions[menuIndex]);
			
		}
		
		if (turn == Turn.P2Choice)
		{
			//PlayerProfile
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
				boss.Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (boss.gameObject);
				selector.setOffset (0, boss.gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
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
				selectedSlot = 0;
				boss.Selected = true;
				selector.gameObject.GetComponent<Renderer>().enabled = true;
				selector.setParent (boss.gameObject);
				selector.setOffset (0, boss.gameObject.GetComponent<Renderer>().bounds.size.y / 2, 0);
				playSound (0);
			}
			//Items Button
			GUI.SetNextControlName ("Items");
			if (menuIndex == 2 && Input.GetKeyDown (KeyCode.Z) ||
			    GUI.Button (new Rect (Screen.width / 2, Screen.height * 0.88f, 
			                      Screen.width / 10.6f, Screen.height / 16f), "Items")) 
			{ 
				timer = 5;
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
				playSound (0);
				GameManager.Instance.setLevel("Overworld Map"); 
				Application.LoadLevel("Overworld"); 
				print ("You flee the battle");
			} 
			
			GUI.FocusControl (menuOptions[menuIndex]);
			
		}
		
		if (turn == Turn.GameOver)
		{
			GUI.Label(new Rect (Screen.width/2 - 50, Screen.height/2 + 50, 120, 100), "You Died.\n\n Game Over");
			
			GUI.SetNextControlName ("StartOver");
			if (Input.GetKeyDown (KeyCode.Z) ||
			    GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 - 50, 120, 40), "Start Over")) 
			{
				GameManager.Instance.setLevel ("Start Screen");
				Application.LoadLevel ("StartScreen");
			}
			GUI.FocusControl ("StartOver");
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
			if (!p1.Target.isDown ())
			{
				p1.PerformAction ();
				turn = Turn.DelayPhaseP1;
			}
			else
			{
				p1.HasAttacked = true;
				turn = Turn.CalculationPhase;
			}
			break;
		case Turn.DelayPhaseP1:
			if (p1.State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.P2Attack:
			if (!p2.Target.isDown ())
			{
				p2.PerformAction ();
				turn = Turn.DelayPhaseP2;
			}
			else
			{
				p2.HasAttacked = true;
				turn = Turn.CalculationPhase;
			}
			break;
		case Turn.DelayPhaseP2:
			if (p2.State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
			break;
		case Turn.E1Attack:
			if (Random.Range (0, 2) == 0 && !p1.isDown ())
			{
				boss.setTarget(p1);
			}
			else if (!p2.isDown ())
			{
				boss.setTarget(p2);
			}
			else
			{
				boss.setTarget (p1);
			}
			boss.bossPerformAction();
			turn = Turn.DelayPhaseE1;
			break;
		case Turn.DelayPhaseE1:
			if (boss.State != CharacterState.ATTACK) { turn = Turn.CalculationPhase;}
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
	
	private void calculatePhase()
	{
		if (p1.HasAttacked == false && !p1.isDown ()) {turn = Turn.P1Attack;}
		else if (p2.HasAttacked == false && !p2.isDown ()) {turn = Turn.P2Attack;}
		else if (boss.HasAttacked == false && !boss.isDown()) 
		{turn = Turn.E1Attack;}
		else {
			boss.HasAttacked = false;
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
		if (boss.isDown())
		{
			totalExpReward += boss.data ().getExpYield();
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
	
	private void selectAction()
	{
		if (Input.GetButtonDown ("Horizontal"))
		{
			if (Input.GetAxis ("Horizontal") > 0 )
			{
				if (menuIndex == menuOptions.Length - 1)
				{
					menuIndex = 0;
				}
				else
				{
					menuIndex++;
				}
			}
			else
			{
				if (menuIndex == 0)
				{
					menuIndex = menuOptions.Length - 1;
				}
				else
				{
					menuIndex--;
				}
			}
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.X))
		{
			if (turn == Turn.P2Choice)
			{
				turn = Turn.P1Choice;
				playSound (0);
			}
		}
	}
	
	private void selectTarget()
	{
		if (Input.GetKeyDown (KeyCode.Z))
		{
			if (turn == Turn.P1Target)
			{
				p1.setTarget (boss);
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
				p2.setTarget (boss);
				nextTurn = Turn.CalculationPhase;
				turn = Turn.Transition;
			}
			boss.Selected = false;
			selector.gameObject.GetComponent<Renderer>().enabled = false;
			playSound (0);
		}
		else if (Input.GetKeyDown (KeyCode.X))
		{
			boss.Selected = false;
			selector.gameObject.GetComponent<Renderer>().enabled = false;
			if (turn == Turn.P1Target)
			{
				turn = Turn.P1Choice;
			}
			else if (turn == Turn.P2Target)
			{
				turn = Turn.P2Choice;
			}
			playSound (0);
		}
	}
	
	public void playSound(int index)
	{
		audioSource.clip = sounds[index];
		audioSource.Play();
	}
	
	private void setUp()
	{
		//Sets up the players and enemies
		GameObject character1 = Instantiate (player1) as GameObject;
		GameObject character2 = Instantiate (player2) as GameObject;
		p1 = character1.GetComponent<Character> ();
		p2 = character2.GetComponent<Character> ();
		GameObject boss1 = Instantiate (bossObject) as GameObject;
		boss = boss1.GetComponent<Enemy>();
		boss.Slot = 1;
		boss.setSlot ();
		boss.data ().setMaxHealth (400);
		boss.data ().setAttack(50);
		turnOrder = new GameObject[3];
		
		//Manages enemy selection
		selectedSlot = 0;
		GameObject theCursor = Instantiate (cursor) as GameObject;
		theCursor.GetComponent<Renderer>().enabled = false;
		selector = theCursor.GetComponent<AnimationOffset>();
		selector.setParent (boss.gameObject);
		
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
}
