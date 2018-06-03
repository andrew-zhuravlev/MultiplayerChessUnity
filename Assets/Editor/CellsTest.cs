using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class CellsTest {

	[Test]
	public void Cells_Set_To_King_On_00()
	{
		Board.Instance.Cells[0, 0] = Cell.King;

		Assert.IsTrue(Board.Instance.Cells[0, 0] == Cell.King);
	}
}
