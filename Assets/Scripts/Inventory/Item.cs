using UnityEngine;
using System.Collections;

public abstract class Item {

	private string name;
	private string description;
	private ItemType itemType;
	private ItemCatagory catagory;
	private bool stackable;

	public string Name
	{
		get {return name; }
		set {name = value; }
	}

	public string Description
	{
		get {return description; }
		set {description = value; }
	}

	public ItemCatagory Catagory
	{
		get {return catagory; }
		set {catagory = value; }
	}

	public ItemType Type
	{
		get {return itemType; }
		set {itemType = value; }
	}

	public bool isStackable
	{
		get {return stackable; }
		set {stackable = value; }
	}

}

public enum ItemCatagory {
	Equipment,
	Consumable,
	Key
}

public enum ItemType {
	Sword,
	Staff,
	Head,
	Armor,
	Gloves,
	Boots,
	Accessory,
	Potion,
	Food
}