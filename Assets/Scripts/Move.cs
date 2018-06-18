// TODO: Not quite sure for isKill and isCastle
public struct Move
{
	public int z;
	public int x;
	public bool isKill;
	public bool isCastle;

	public Move(int y, int x, bool isKill, bool isCastle)
	{
		this.z = y;
		this.x = x;
		this.isKill = isKill;
		this.isCastle = isCastle;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Move))
		{
			return false;
		}

		var move = (Move)obj;
		return z == move.z &&
			   x == move.x &&
			   isKill == move.isKill &&
			   isCastle == move.isCastle;
	}

	public override int GetHashCode()
	{
		var hashCode = 776008758;
		hashCode = hashCode * -1521134295 + base.GetHashCode();
		hashCode = hashCode * -1521134295 + z.GetHashCode();
		hashCode = hashCode * -1521134295 + x.GetHashCode();
		hashCode = hashCode * -1521134295 + isKill.GetHashCode();
		hashCode = hashCode * -1521134295 + isCastle.GetHashCode();
		return hashCode;
	}

	public static bool operator ==(Move first, Move second)
	{
		return first.x == second.x && first.z == second.z;
	}

	public static bool operator !=(Move first, Move second)
	{
		return !(first == second);
	}
}
