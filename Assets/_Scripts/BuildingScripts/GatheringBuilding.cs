using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringBuilding : MonoBehaviour
{
	[SerializeField] private float _timeBetweenGathering;
	[SerializeField] private float _timeSpentGathering;
	List<GatheringAI> _gatheringPeople = new();

	public float GatheringTime { get { return _timeBetweenGathering; } }

	public float TimeToGather { get { return _timeSpentGathering; } }

	public void AddGatheringPerson ( GatheringAI gatheringAI ) {
		_gatheringPeople.Add(gatheringAI);
	}
	
}
