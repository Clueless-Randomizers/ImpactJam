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

		[SerializeField] GameObject _buttonPrefab;

		private Dictionary<Button, Building> _buttonRegister = new();
		private Building _currentBuilding;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		void Start()
		{
			foreach (Building building in BuildingManager.instance.buildingPrefabs) {
				GameObject _newButtonGameObject = Instantiate( _buttonPrefab, transform );

				if (_newButtonGameObject.TryGetComponent( out Button button )) {
					_buttonRegister.Add(button, building);

					button.onClick.AddListener( () => SelectBuilding(button) ) ;

					button.GetComponentInChildren<TMP_Text>().text = GetButtonText( building );

				}
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
					BuildingManager.instance.SpawnBuilding( _currentBuilding, position);

					canvasGroup.alpha = 1;
					isPlacing = false;
					_currentBuilding = default;
				}
			}
		}
		void SelectBuilding ( Button button  ) {
			_currentBuilding = _buttonRegister[ button ];

			ActorManager.instance.DeselectActors();
			canvasGroup.alpha = 0;
			isPlacing = true;
			buildingPreviewMesh = _currentBuilding.GetBuildingPreviewMesh;
		}

		string GetButtonText(Building building)
		{
			PurchasePrice[] _purchasePrices = building.PurchasePrices;

			string _buildingName = building.buildingName;
			
			List<string> _resourceString = new();

			foreach (PurchasePrice purchasePrice in _purchasePrices) { 
				_resourceString.Add($"{purchasePrice.Currency.PresentableName} ({purchasePrice.Value})");
			}

			return "<size=23><b>" + _buildingName + "</b></size><br>" + String.Join("\n", _resourceString.ToArray() );
		}
	}
}
