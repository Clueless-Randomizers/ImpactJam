using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class GatheringAI : MonoBehaviour
{
	[SerializeField] private GatheringBuilding _gatheringBuilding = default;
	[SerializeField] private int _maxFoundResources = 5;
	[SerializeField] NavMeshAgent _agent = default;
	[Header("Hat Configuration")]
	[SerializeField] private Renderer[] _hatRenderers;
	[SerializeField] private Material _hatMaterial;
	[Header( "References" )]
	[SerializeField] private TMP_Text _resourceCounter;
	[SerializeField] private Image _resourceImage;

	float _nextGatheringRun = 0f;
	float _gatheringTimeout = 20f;
	float _timeSpentGathering = 30f;
	float _nextReturnTime = 0f;

	bool _isGathering = false;
	bool _hasResources = false;
	
	int _carryingResources = 0;


	private void Start () {
		if ( _agent == default ) {
			_agent = transform.GetComponent<NavMeshAgent>();
		}

		if ( _gatheringBuilding != default) {
			SetGatheringBuilding( _gatheringBuilding );
		}

		_nextGatheringRun = GetNextTime( _gatheringTimeout, 1.5f );
	}

	void Update()
	{
		if ( _hasResources ) {
			return;
		}

		if ( _isGathering ) {
			CheckIfDoneGathering();
		} else {
			if (_nextGatheringRun < Time.realtimeSinceStartup)
			{
				if (_gatheringBuilding != default)
				{
					StartGathering();
				}
			} 
		}
	}

	/// <summary>
	/// Checks if the character is done gathering:
	///		If Yes: redirects the character to the resource building, adjusts state-booleans, assigns how many resources the character managed to find;
	///		If No: Does nothing;
	/// </summary>
	private void CheckIfDoneGathering () {
		if ( _nextReturnTime < Time.realtimeSinceStartup) {
			GoToDestination(_gatheringBuilding.transform.position);
			_hasResources = true;
			_isGathering = false;
			_carryingResources = ( int ) 1 * Random.Range( 1 , _maxFoundResources );
			_resourceCounter.text = $"{_carryingResources}";
		}
	}

	/// <summary>
	/// Checks if the unit is doing anything, acts on this if it is not busy.
	/// </summary>
	/// <param name="targetPosition"></param>
	public void GoToDestinationIfNotGathering (Vector3 targetPosition) {
		if (_isGathering || _hasResources) return;

		GoToDestination(targetPosition);
	}

	/// <summary>
	/// Tells NavMeshAgent SetDestination and isStopped = false.
	/// </summary>
	/// <param name="position"></param>
	private void GoToDestination ( Vector3 position ) {
		_agent.SetDestination(position);
		
		_agent.isStopped = false;
	}

	/// <summary>
	/// Starts the gathering procedures.
	/// </summary>
	private void StartGathering () {
		_isGathering = true;

		_resourceImage.sprite = _gatheringBuilding.GetCurrency.Icon;
		_resourceImage.enabled = true;

		float _generatedTimeOut = GetNextTime( _timeSpentGathering, 1.5f );
		_nextReturnTime = _generatedTimeOut;
		
		Debug.Log($"Next Return time: {_generatedTimeOut - Time.realtimeSinceStartup}s from now.");
		Vector3 _randomPosition = GetRandomPosition();
		
		GoToDestination( _randomPosition );
	}

	/// <summary>
	/// Assigns this character's GatheringBuilding, updates _gatheringTimeout, updates _timeSpentGathering
	/// </summary>
	/// <param name="gatheringBuilding"></param>
	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		// If gatheringBuilding is default, reset unit's gathering building to nothing.
		if ( gatheringBuilding == default) {
			ResetGatheringBuilding();
			return;
		}
		// If gatheringbuilding is not default and we added this unit's GatheringAI-script to the gathering-building...
		if ( gatheringBuilding.AddGatheringPerson( this ) ) {
			if (_gatheringBuilding != gatheringBuilding && _gatheringBuilding != default) {
				_gatheringBuilding.RemoveGatheringPerson( this );
			}
			_gatheringBuilding = gatheringBuilding;

			_gatheringTimeout = _gatheringBuilding.WorkerRestTime;
			
			_resourceImage.sprite = _gatheringBuilding.GetCurrency.Icon;
			
			// Divided by 2 to make the variable account for the whole time the character is spending gathering.
			_timeSpentGathering = _gatheringBuilding.TimeSpentGathering / 2;

			Material gatheringBuildingHatMaterial = _gatheringBuilding.GetHatMaterial();
			if ( gatheringBuildingHatMaterial != default) { 
				foreach ( Renderer hatPart in _hatRenderers ) {
					hatPart.material = gatheringBuildingHatMaterial;
				}
			}

			ResetGatheringStatus();

			GoToDestination( gatheringBuilding.transform.position );
		}
	}

	/// <summary>
	/// Resets current gathering Building.
	/// </summary>
	private void ResetGatheringBuilding () {
		_gatheringBuilding = default;
		_gatheringTimeout = default;
		_timeSpentGathering = default;

		foreach (Renderer hatPart in _hatRenderers) {
			hatPart.material = _hatMaterial;
		}

		_resourceImage.enabled = false;
	}

	/// <summary>
	/// Resets current GatheringStatus (_carryingResources, _hasResources, _isGathering, _resourceCounter)
	/// </summary>
	private void ResetGatheringStatus () {
		_nextGatheringRun = _nextGatheringRun = GetNextTime( _gatheringTimeout, 1.75f );
		_carryingResources = 0;
		_hasResources = false;
		_isGathering = false;
		_resourceCounter.text = "";
	}

	/// <summary>
	/// Creates a time in the future based on timeout and randomMultiplicator.
	/// </summary>
	/// <param name="timeout"></param>
	/// <param name="randomMultiplicator"></param>
	/// <returns></returns>
	private float GetNextTime ( float timeout, float randomMultiplicator = 1 ) { // 8*1.75 = 14.-- seconds
		return Time.realtimeSinceStartup + (timeout * Random.Range(1f, randomMultiplicator ) );
	}

	// GetRandomPosition Author: Clamum; Source: https://forum.unity.com/threads/generating-a-random-position-on-navmesh.873364/#post-5796748
	// Modified by Tor-Arne Sandstrak to recursively find something outside _gatheringBuilding's range of influence
	/// <summary>
	/// Gets a random position on the navmesh, returns a Vector3.
	/// </summary>
	/// <returns></returns>
	private Vector3 GetRandomPosition () {
		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

		int maxIndices = navMeshData.indices.Length - 3;

		// pick the first indices of a random triangle in the nav mesh
		int firstVertexSelected = Random.Range( 0, maxIndices );
		int secondVertexSelected = Random.Range( 0, maxIndices );

		// spawn on verticies
		Vector3 point = navMeshData.vertices[ navMeshData.indices[ firstVertexSelected ] ];

		Vector3 firstVertexPosition = navMeshData.vertices[ navMeshData.indices[ firstVertexSelected ] ];
		Vector3 secondVertexPosition = navMeshData.vertices[ navMeshData.indices[ secondVertexSelected ] ];

		// eliminate points that share a similar X or Z position to stop spawining in square grid line formations
		if ( ( int )firstVertexPosition.x == ( int )secondVertexPosition.x || ( int )firstVertexPosition.z == ( int )secondVertexPosition.z ) {
			point = GetRandomPosition(); // re-roll a position - I'm not happy with this recursion it could be better
		} else {
			// select a random point on it
			point = Vector3.Lerp( firstVertexPosition, secondVertexPosition, Random.Range( 0.05f, 0.95f ) );
		}

		if ( Vector3.Distance( _gatheringBuilding.transform.position, point ) < _gatheringBuilding.GetComponent<Collider>().bounds.extents.y ) {
			point = GetRandomPosition();
		}

		return point;
	}

	/// <summary>
	/// Checks if you have entered the range of your _gatheringBuilding: deposits the resources, resets the gathering state, and assigns the next time to gather.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter ( Collider other ) {
		if ( _hasResources && other.transform.TryGetComponent( out GatheringBuilding gatheringBuilding ) && gatheringBuilding == _gatheringBuilding ) {
			DeliverCurrencyToCurrencyBuilding();
		}
	}

	/// <summary>
	/// Allow currency delivery if already inside the trigger.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerStay ( Collider other ) {
		if ( _hasResources && other.transform.TryGetComponent( out GatheringBuilding gatheringBuilding ) && gatheringBuilding == _gatheringBuilding ) {
			DeliverCurrencyToCurrencyBuilding();
		}
	}

	/// <summary>
	/// Extracted to allow delivery of resources when stuck inside the trigger.
	/// </summary>
	private void DeliverCurrencyToCurrencyBuilding () {
		_gatheringBuilding.AddCurrency( _carryingResources );
		_agent.isStopped = true;

		ResetGatheringStatus();

		_nextGatheringRun = GetNextTime( _gatheringTimeout, 1.75f );
	}
	/// <summary>
	/// Cancels whatever gathering is required and moves to indicated destination.
	/// </summary>
	/// <param name="targetDestination"></param>
	public void CancelGatheringAndGoHere ( Vector3 targetDestination ) {
		ResetGatheringStatus();
		ResetGatheringBuilding();
		GoToDestination( targetDestination );
	}
}
