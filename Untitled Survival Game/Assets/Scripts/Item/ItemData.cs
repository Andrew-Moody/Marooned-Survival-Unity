
public struct ItemNetData
{
	// Can't contain any non serializable members (SO's, Sprite, Mesh, Texture, etc)
    public int ItemID;
    public int Quantity;

	public static ItemNetData Empty;

	public ItemNetData(int itemID, int quantity)
	{
		ItemID = itemID;
		Quantity = quantity;
	}


	static ItemNetData()
	{
		Empty = new ItemNetData(0, 0);
	}
}
