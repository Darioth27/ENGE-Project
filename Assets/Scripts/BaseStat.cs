
public class BaseStat {

	private int baseValue;
	private int buffValue;

	public BaseStat() 
	{
		baseValue = 0;
		buffValue = 0;
	}

	//Setters and Getters
	public int BaseValue
	{
		get{ return baseValue; }
		set{ baseValue = value; }
	}

	public int BuffValue
	{
		get{ return buffValue; }
		set{ buffValue = value; }
	}

	//Other Methods
	public int AdjustedValue ()
	{
		return baseValue + buffValue;
	}

	public void increaseValue (int v)
	{
		buffValue = buffValue + v;
		if (buffValue > (baseValue / 2))
		    buffValue = baseValue / 2;

	}

	public void decreaseValue (int v)
	{
		buffValue = buffValue - v;
		if (buffValue < (-1 * baseValue / 2))
			buffValue = -1 * baseValue / 2;
	}

}
