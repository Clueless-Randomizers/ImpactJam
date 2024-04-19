using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace _Scripts.RefactoredSampleScripts.Builder {
	public class ObjectiveManager : MonoBehaviour {
		[SerializeField] List<Building> _requiredBuildings = new();
		[SerializeField] Transform _objectivesPanel;
		[SerializeField] GameObject _objectivesTextPrefab;

		private Dictionary<Building, TMP_Text> _buildingObjectivesText = new();

		void Start () {
			GameManager.BuildingManager.OnBuilt.AddListener((Building building) => CheckIfObjectiveMetBuilding(building) );
			
			foreach (Building building in _requiredBuildings ) {
				GameObject go = Instantiate(_objectivesTextPrefab, _objectivesPanel);
				if (go.TryGetComponent( out TMP_Text text )) {
					_buildingObjectivesText.Add(building, text);
					text.SetText( $"Build {building.buildingName}." );
				}
			}
		}

		private void CheckIfObjectiveMetBuilding (Building building) {
			if (building != default && _requiredBuildings.Contains( building ) && _buildingObjectivesText.ContainsKey( building )) {
				RedrawBuildingObjectives(building);
			}
		}

		private void RedrawBuildingObjectives ( Building building ) {
			_buildingObjectivesText[ building ].SetText($"<i><s>Build {building.buildingName}.</s></i>" );
		}
	}
}