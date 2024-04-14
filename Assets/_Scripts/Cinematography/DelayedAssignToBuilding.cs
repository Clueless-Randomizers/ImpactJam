using UnityEngine;

/// <summary>
/// This script will assign the unit in question to a building at a specified time.
/// </summary>
public class DelayedAssignToBuilding : MonoBehaviour
{
	[SerializeField] private GatheringBuilding _targetGatheringBuilding;
	[SerializeField] private GatheringAI _targetGatheringAI;
	[SerializeField] private float _assignBuildingDelay = 5f;
	private float _whenToAssignGatheringBuilding;
	private bool finishedAssigningBuilding = false;

	// Start is called before the first frame update
	void Start()
	{
		_whenToAssignGatheringBuilding = Time.realtimeSinceStartup + _assignBuildingDelay;

	}

	// Update is called once per frame
	void Update()
	{
		if (finishedAssigningBuilding || _targetGatheringAI == default || _targetGatheringBuilding == default ) {
			return;
		}

		if (_whenToAssignGatheringBuilding < Time.realtimeSinceStartup) {
			_targetGatheringAI.SetGatheringBuilding(_targetGatheringBuilding);
			finishedAssigningBuilding = true;
		}
	}
}
