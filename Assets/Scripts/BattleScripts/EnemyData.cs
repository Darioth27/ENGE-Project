using UnityEngine;
using System.Collections;

public class EnemyData {

	private int level;		//Current character level
	private int expYield;	//Experience points
	private int maxHealth;		//Represents life remaining
	private int currentHealth;

	private int strength;	//Affects ATK/DEF
	private int dexterity;	//Affects Accuracy, Crit. & Dodge Chance, Speed
	private int intelligence;	//Affects Magic ATK/DEF
	
	private BaseStat attack;
	private BaseStat defense;
	private BaseStat magicAttack;
	private BaseStat magicDefense;
	private BaseStat acc;
	private BaseStat crit;
	private BaseStat dodge;
	private BaseStat speed;


	public EnemyData()
	{
		this.CreateInitialStats();
	}

	public void CreateInitialStats()
	{
		level = 1;

		maxHealth = 50;
		strength = 4;
		dexterity = 4;
		intelligence = 4;
		
		attack = new BaseStat ();
		magicAttack = new BaseStat ();
		defense = new BaseStat ();
		magicDefense = new BaseStat ();
		speed = new BaseStat ();
		acc = new BaseStat ();
		crit = new BaseStat ();
		dodge = new BaseStat ();

		currentHealth = maxHealth;
		
		expYield = 50;

		updateStats();
	}

	public void setMaxHealth(int m)
	{
		maxHealth = m;
		currentHealth = m;
	}

	public void setAttack(int a)
	{
		attack.BaseValue = a;
	}

	public int CurrentHealth
	{
		get { return currentHealth; }
	}

	public void restoreHealth(int amount)
	{
		if (currentHealth + amount > maxHealth)
		{
			currentHealth = maxHealth;
		}
		else
		{
			currentHealth += amount;
		}
	}
	
	public void takeDamage(int amount)
	{
		if (currentHealth - amount < 0)
		{
			currentHealth = 0;
		}
		else
		{
			currentHealth -= amount;
		}
	}

	public int getExpYield()
	{
		return expYield;
	}
	
	public int getMaxHealth()
	{
		return maxHealth;
	}
	
	public void setAttack()
	{
		attack.BaseValue = 25;
	}
	
	public void setMagicAttack()
	{
		magicAttack.BaseValue = 14;
	}
	
	public void setDefense()
	{
		defense.BaseValue = 5;
	}
	
	public void setMagicDefense()
	{
		magicDefense.BaseValue = 4;
	}
	
	public void setSpeed()
	{
		speed.BaseValue = 5;
	}
	
	public void setAccuracy()
	{
		acc.BaseValue = 100;
	}
	
	public void setCritical()
	{
		crit.BaseValue = 5;
	}
	
	public void setDodge()
	{
		dodge.BaseValue = 0;
	}
	
	public BaseStat Attack { get {return attack; } }
	public BaseStat Defense { get { return defense; }}
	public BaseStat MagicAttack { get { return magicAttack; }}
	public BaseStat MagicDefense { get { return magicDefense; }}
	public BaseStat Speed { get { return speed; }}
	public BaseStat Accuracy { get { return acc; }}
	public BaseStat Critical { get { return crit; }}
	public BaseStat Dodge{ get  { return dodge; }}
	
	public void updateStats ()
	{
		setAttack ();
		setDefense ();
		setMagicAttack ();
		setMagicDefense ();
		setSpeed ();
		setAccuracy ();
		setCritical ();
		setDodge ();
	}
}
