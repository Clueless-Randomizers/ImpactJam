using Eflatun.SceneReference;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


namespace _Scripts.RefactoredSampleScripts.Builder {
	public class ObjectiveManager : MonoBehaviour {
		[SerializeField] List<Building> _requiredBuildings = new();
		[SerializeField] Transform _objectivesPanel;
		[SerializeField] GameObject _objectivesTextPrefab;

		[Header("WinGame Config")]
		[SerializeField] SceneReference _winLevelReference;
		[SerializeField] float _winGameDelay = 15f;

		private Dictionary<Building, TMP_Text> _buildingObjectivesText = new();
		private Dictionary<string, bool> _objectivesMet = new();
		private float _wonGameTimer;

		void Start () {
			GameManager.BuildingManager.OnBuilt.AddListener((Building building) => CheckIfObjectiveMetBuilding(building) );
			
			foreach (Building building in _requiredBuildings ) {
				GameObject go = Instantiate(_objectivesTextPrefab, _objectivesPanel);
				if (go.TryGetComponent( out TMP_Text text )) {
					_buildingObjectivesText.Add(building, text);
					_objectivesMet.Add(building.buildingName, false);
					text.SetText( $"Build {building.buildingName}." );
				}
			}
		}

		private void Update () {
			if (_wonGameTimer < Time.realtimeSinceStartup) {
				SceneManager.LoadScene( _winLevelReference.Name );
			}
		}

		private void CheckIfObjectiveMetBuilding (Building building) {
			if (building != default && _requiredBuildings.Contains( building ) && _buildingObjectivesText.ContainsKey( building )) {
				RedrawBuildingObjectives(building);
				_objectivesMet[ building.buildingName ] = true;
				CheckIfLevelWon();
			}
		}

		private void RedrawBuildingObjectives ( Building building ) {
			_buildingObjectivesText[ building ].SetText($"<i><s>Build {building.buildingName}.</s></i>" );
		}

		private void CheckIfLevelWon () {
			foreach (string key in _objectivesMet.Keys) {
				if ( _objectivesMet[ key ] == false) {
					return;
				}
			}
			_wonGameTimer = Time.realtimeSinceStartup + 15;
		}
	}
}