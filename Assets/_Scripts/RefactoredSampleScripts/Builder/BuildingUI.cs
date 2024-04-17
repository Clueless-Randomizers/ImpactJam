using System;
using System.Collections.Generic;
using _Scripts.RefactoredSampleScripts.Agent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.RefactoredSampleScripts.Builder
{
	public class BuildingUI : MonoBehaviour
	{
		CanvasGroup canvasGroup;
		bool isPlacing = false;
		int currentIndex = 0;
		
		Mesh buildingPreviewMesh;
		[SerializeField] Material buildingPreviewMat;
		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		void Start()
		{
			Button[] buttons = GetComponentsInChildren<Button>();
			for (int i = 0; i < buttons.Length; i++)
			{
				int index = i;
				buttons[index].onClick.AddListener(() => SelectBuilding(index));

				Building b = BuildingManager.instance.buildingPrefabs[index];
				buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = GetButtonText(b);
			}
		}

		private void Update()
		{
			if (isPlacing)
			{
				Vector3 position = Utility.MouseToTerrainPosition();
				Graphics.DrawMesh(buildingPreviewMesh, position, Quaternion.identity, buildingPreviewMat, 0);

				if (Input.GetMouseButtonDown(0))
				{
					BuildingManager.instance.SpawnBuilding(currentIndex, position);
					canvasGroup.alpha = 1;
					isPlacing = false;
				}
			}
		}

		void SelectBuilding(int index)
		{
			currentIndex = index;
			ActorManager.instance.DeselectActors();
			canvasGroup.alpha = 0;
			isPlacing = true;
			buildingPreviewMesh = BuildingManager.instance.GetPrefab(index).GetBuildingPreviewMesh;
		}

		string GetButtonText(Building building)
		{
			PurchasePrice[] _purchasePrices = building.PurchasePrices;

			string _buildingName = building.buildingName;
			
			List<string> _resourceString = new();

			foreach (PurchasePrice purchasePrice in _purchasePrices)
				_resourceString.Add($"{purchasePrice.Currency.PresentableName} ({purchasePrice.Value})");

			return "<size=23><b>" + _buildingName + "</b></size><br>" + String.Join("\n", _resourceString.ToArray() );
		}
	}
}
