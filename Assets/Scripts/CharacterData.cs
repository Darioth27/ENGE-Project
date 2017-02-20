using System;

public class CharacterData {

	//Character Pic

	//Attributes that need to be saved
	private string name;	//Character Name

	private int level;		//Character Level
	private int totalEXP;	//Experience points
	private int currentEXP;	//Current exp for the level
	private int expTillNextLevel;
	private float levelModifier =  1.5f;	//Adjusts EXP requird to level up depending on current level

	private int maxHealth;	//Total hp
	private int maxMana;		//Total mana
	private int currentHealth;	//Represents life remaining
	private int currentMana;		//Represents mana remaining

	private Attribute constitution;	//Affects MaxHealth
	private Attribute strength;		//Affects ATK/DEF
	private Attribute dexterity;		//Affects Accuracy, Crit. & Dodge Chance, Speed
	private Attribute intelligence;	//Affects Magic ATK/DEF
	private Attribute mind;			//Affects MaxMana

	private BaseStat attack;
	private BaseStat magicAttack;
	private BaseStat defense;
	private BaseStat magicDefense;
	private BaseStat speed;
	private BaseStat acc;
	private BaseStat crit;
	private BaseStat dodge;

	private Item[] inventory;
	private Skill[] skillList;	//Skills that the Character has

	private int slot;	//Order in party

	public CharacterData(int c, int s, int d, int i, int m, int st)
	{
		CreateInitialStats(c, s, d, i, m, st);
	}

	public void CreateInitialStats(int con, int str, int dex, int intel, int mnd, int slt)
	{
		level = 1;
		totalEXP = 0;
		currentEXP = totalEXP;
		expTillNextLevel = 100;
		constitution = new Attribute (AttributeName.CON, con);
		strength = new Attribute (AttributeName.STR, str);
		dexterity = new Attribute (AttributeName.DEX, dex);
		intelligence = new Attribute (AttributeName.INT, intel);
		mind = new Attribute (AttributeName.MND, mnd);
		attack = new BaseStat ();
		magicAttack = new BaseStat ();
		defense = new BaseStat ();
		magicDefense = new BaseStat ();
		speed = new BaseStat ();
		acc = new BaseStat ();
		crit = new BaseStat ();
		dodge = new BaseStat ();
		setMaxHealth ();
		setMaxMana ();
		currentHealth = maxHealth;
		currentMana = maxMana;
		updateStats ();
		
		skillList = new Skill[Enum.GetValues(typeof(SkillName)).Length];
		setupSkills ();
		
		slot = slt;
	}

	public void updateLevel()
	{
		if (currentEXP > expTillNextLevel)
		{
			currentEXP = currentEXP - expTillNextLevel;
			level++;
			expTillNextLevel = (int)(expTillNextLevel * levelModifier);
			constitution.Amount++;
			strength.Amount++;
			dexterity.Amount++;
			intelligence.Amount++;
			mind.Amount++;
			setMaxHealth ();
			currentHealth = maxHealth;
			setMaxMana ();
			currentMana = maxMana;
			updateStats ();
			updateLevel ();
		}
	}

	public String Name
	{
		get { return name; }
		set { name = value; }
	}

	public int Level
	{
		get { return level; }
		set { level = value; }
	}

	public int TotalEXP
	{
		get { return totalEXP; }
	}

	public int CurrentEXP
	{
		get { return currentEXP; }
	}

	public int ExpNeeded
	{
		get { return expTillNextLevel; }
	}

	public void updateEXP(int amount)
	{
		totalEXP += amount;
		currentEXP += amount;
	}

	public void setMaxHealth()
	{
		maxHealth = constitution.Amount * 8 + 40;
	}

	public int getMaxHealth()
	{
		return maxHealth;
	}

	public void setMaxMana()
	{
		maxMana = mind.Amount * 6 + 30;
	}

	public int getMaxMana()
	{
		return maxMana;
	}

	public int getCurrentHealth()
	{
		return currentHealth;
	}

	public int getCurrentMana()
	{
		return currentMana;
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

	public void restoreMana(int amount)
	{
		if (currentMana + amount > maxMana)
		{
			currentMana = maxMana;
		}
		else
		{
			currentMana += amount;
		}
	}
	
	public void consumeMana(int amount)
	{
		if (currentMana - amount < 0)
		{
			currentMana = 0;
		}
		else
		{
			currentMana -= amount;
		}
	}

	public void unlockSkill(SkillName name)
	{
		skillList [(int)name].Unlocked = true;
	}
	
	public void lockSkill(SkillName name)
	{
		skillList[(int)name].Unlocked = false;
	}

	public void setAttack()
	{
		attack.BaseValue = strength.Amount * 4;
	}

	public void setMagicAttack()
	{
		magicAttack.BaseValue = intelligence.Amount * 4;
	}

	public void setDefense()
	{
		defense.BaseValue = strength.Amount * 2;
	}

	public void setMagicDefense()
	{
		magicDefense.BaseValue = intelligence.Amount * 2;
	}

	public void setSpeed()
	{
		speed.BaseValue = dexterity.Amount * 2;
	}

	public void setAccuracy()
	{
		acc.BaseValue = 100;
	}

	public void setCritical()
	{
		crit.BaseValue = strength.Amount * 2;
	}

	public void setDodge()
	{
		dodge.BaseValue = strength.Amount * 2;
	}

	public BaseStat Attack { get { return attack; } }
	public BaseStat Defense { get { return defense; } }
	public BaseStat MagicAttack { get { return magicAttack; } }
	public BaseStat MagicDefense { get { return magicDefense; } }
	public BaseStat Speed { get { return speed; } }
	public BaseStat Accuracy { get { return acc; } }
	public BaseStat Critical { get { return crit; } }
	public BaseStat Dodge { get { return dodge; } }

	public Attribute getAttribute(AttributeName type)
	{
		switch(type)
		{
		case AttributeName.CON:
			return constitution;
		case AttributeName.DEX:
			return dexterity;
		case AttributeName.INT:
			return intelligence;
		case AttributeName.MND:
			return mind;
		case AttributeName.STR:
			return strength;
		default:
			break;
		}
		return new Attribute (AttributeName.NULL, 0);
	}

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

	public void setupSkills()
	{
		for (int i = 0; i < skillList.Length; i++)
		{
			skillList[i] = new Skill((SkillName)i, false);
		}
	}

	public int Slot
	{
		get { return slot; }
		set { slot = value;}
	}
}
