
public class Attribute {

	private AttributeName type;
	private int attributeValue;

	public Attribute(AttributeName t, int v)
	{
		type = t;
		attributeValue = v;
	}

	public int Amount
	{
		get {return attributeValue; }
		set{ attributeValue = value; }
	}

	public AttributeName Type
	{
		get {return type; }
		set { type =  value; }
	}
}

public enum AttributeName {
	CON,
	STR,
	DEX,
	INT,
	MND,
	NULL
}
