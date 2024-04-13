using System.Collections.Generic;
using UnityEngine;

public class GatheringBuilding : MonoBehaviour
{
	[SerializeField] private float _timeBetweenGathering;
	[SerializeField] private float _timeSpentGathering;
	[SerializeField] private SO_Currency _buildingCurrency;
	[SerializeField] private int _maxGatheringPeople = 5;
	List<GatheringAI> _gatheringPeople = new();

	private void Start () {
		_buildingCurrency = GameManager.CurrencyManager.GetCurrency( _buildingCurrency.name );
	}

	public float GatheringTime { get { return _timeBetweenGathering; } }

	public float TimeToGather { get { return _timeSpentGathering; } }

	public SO_Currency GetCurrency {  get { return _buildingCurrency; } }

	public bool AddGatheringPerson ( GatheringAI gatheringAI ) {
		if ( _gatheringPeople.Count < _maxGatheringPeople && !_gatheringPeople.Contains( gatheringAI ) ) {
			_gatheringPeople.Add( gatheringAI );
			return true;
		} else {
			return false;
		};
	}

	public void AddCurrency (int currency) {
		_buildingCurrency.Value = currency;
	}
}
