public struct Move
{
	public int y;
	public int x;
	public bool isKill;
	public bool isCastle;

	public Move(int y, int x, bool isKill, bool isCastle)
	{
		this.y = y;
		this.x = x;
		this.isKill = isKill;
		this.isCastle = isCastle;
	}
}
