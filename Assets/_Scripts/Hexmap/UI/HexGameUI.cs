using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Hexmap.UI
{
	/// <summary>
	/// Component that manages the game UI.
	/// </summary>
	public class HexGameUI : MonoBehaviour
	{
		[SerializeField] private HexGrid grid;

		private int currentCellIndex = -1;

		private HexUnit selectedUnit;

		/// <summary>
		/// Set whether map edit mode is active.
		/// </summary>
		/// <param name="toggle">Whether edit mode is enabled.</param>
		public void SetEditMode(bool toggle)
		{
			enabled = !toggle;
			grid.ShowUI(!toggle);
			grid.ClearPath();
			if (toggle)
			{
				Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
			}
			else
			{
				Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
			}
		}

		private void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0))
				{
					DoSelection();
				}
				else if (selectedUnit)
				{
					if (Input.GetMouseButtonDown(1))
					{
						DoMove();
					}
					else
					{
						DoPathfinding();
					}
				}
			}
		}

		private void DoSelection()
		{
			grid.ClearPath();
			UpdateCurrentCell();
			if (currentCellIndex >= 0)
			{
				selectedUnit = grid.GetCell(currentCellIndex).Unit;
			}
		}

		private void DoPathfinding()
		{
			if (UpdateCurrentCell())
			{
				if (currentCellIndex >= 0 &&
				    selectedUnit.IsValidDestination(grid.GetCell(currentCellIndex)))
				{
					grid.FindPath(
						selectedUnit.Location,
						grid.GetCell(currentCellIndex),
						selectedUnit);
				}
				else
				{
					grid.ClearPath();
				}
			}
		}

		private void DoMove()
		{
			if (grid.HasPath)
			{
				selectedUnit.Travel(grid.GetPath());
				grid.ClearPath();
			}
		}

		private bool UpdateCurrentCell()
		{
			HexCell cell = grid.GetCell(
				Camera.main.ScreenPointToRay(Input.mousePosition));
			int index = cell ? cell.Index : -1;
			if (index != currentCellIndex)
			{
				currentCellIndex = index;
				return true;
			}
			return false;
		}
	}
}
