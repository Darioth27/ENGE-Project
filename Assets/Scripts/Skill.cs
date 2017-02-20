
public class Skill {
	
	private SkillName skillName;
	private SkillType type;
	private float power;
	private bool unlocked;

	public Skill(SkillName name, bool b)
	{
		skillName = name;
		unlocked = b;
	}

	public bool Unlocked
	{
		get {return unlocked; }
		set{ unlocked = value; }
	}

	public SkillName Name
	{
		get {return skillName; }
		set {skillName = value; }
	}

	public int skillID(SkillName skill)
	{
		return (int)skill;
	}
}

public enum SkillName {
	FIRE = 0,
	HEAL = 1
}

public enum SkillType {
	MELEE,
	MAGIC,
	BUFF,
	DEBUFF
}