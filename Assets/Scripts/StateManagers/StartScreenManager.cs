using UnityEngine;
using System.Collections;

public class StartScreenManager : MonoBehaviour {

	string stringToEdit = "Sword Guy";

	public AudioClip[] sounds;
	private AudioSource audioSource;

	private string[] menuOptions;
	private int menuIndex;

	void Awake() {
		Application.targetFrameRate = 60;
		audioSource = GetComponent<AudioSource>();

		menuOptions = new string[2];
		menuOptions[0] = "New";
		menuOptions[1] = "Exit";
		menuIndex = 0;
	}

	void Start () {
		print ("Game Loaded");
	}

	void Update() {
		selectAction ();
	}

	void OnGUI () {

		GUI.SetNextControlName ("New");
		if ((menuIndex == 0 && Input.GetKeyDown (KeyCode.Z)) || 
			GUI.Button (new Rect (Screen.width/2 - 60, Screen.height/2 - 30, 120, 40), "New Game")) 
		{ 
			GameManager.Instance.startState ();
			GameManager.Instance.setLevel("Overworld Map"); 
			GameManager.Instance.P1Data.Name = "Tech";
			GameManager.Instance.P2Data.Name = "Virginia";
			Application.LoadLevel("Overworld"); 
			print ("You start a new game");
		}

		GUI.SetNextControlName ("Exit");
		if ((menuIndex == 1 && Input.GetKeyDown (KeyCode.Z)) || 
			GUI.Button (new Rect (Screen.width/2 - 60, Screen.height/2 + 10, 120, 40), "Exit")) 
		{
			Application.Quit ();
		}

		GUI.Label (new Rect (Screen.width/2 - 150, Screen.height/2 + 60, 300, 200), 
		           "Controls:\n Movement: ASWD/ArrowKeys\nMenu: M\n" + 
		           "\nBattle Selection:ArrowKeys\n\nconfirm: z\ncancel: x");

		GUI.FocusControl (menuOptions[menuIndex]);

		//stringToEdit = GUI.TextField (new Rect (Screen.width/2 + 100, Screen.height/2 - 80, 120, 30), stringToEdit, 25);
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
