using UnityEngine;
using UnityEngine.AI;

[RequireComponent( typeof( NavMeshAgent ) )]
public class GatheringAI : MonoBehaviour
{
	[SerializeField] GatheringBuilding _gatheringBuilding = default;
	[SerializeField] private int _maxFoundResources = 5;
	[SerializeField] private static float _mapSize = 400f;

	float _nextGatheringRun = 0f;
	float _gatheringTimeout = 20f;
	float _timeSpentGathering = 30f;
	float _nextReturnTime = 0f;

	bool _isGathering = false;
	bool _hasResources = false;
	
	int _carryingResources = 0;

	[SerializeField] NavMeshAgent _agent = default;

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
			if ( _nextGatheringRun < Time.realtimeSinceStartup ) {
				StartGathering();
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
		}
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
		float _generatedTimeOut = GetNextTime( _timeSpentGathering, 1.5f );
		_nextReturnTime = _generatedTimeOut;
		Debug.Log($"Next Return time: {_generatedTimeOut - Time.realtimeSinceStartup}s from now.");
		Vector3 _randomPosition = GetRandomPosition();
		//float dist=agent.remainingDistance; if (dist!=Mathf.infinite && agent.pathStatus==NavMeshPathStatus.completed && agent.remainingDistance==0)
		GoToDestination( _randomPosition );
	}
	/// <summary>
	/// Access point for a random location.
	/// </summary>
	/// <returns></returns>
	//private static Vector3 GetRandomPosition () {
	//	return new Vector3( Random.Range( -1 * _mapSize, _mapSize ), 0f, Random.Range( -1 * _mapSize, _mapSize ) );
	//}

	/// <summary>
	/// Assigns this character's GatheringBuilding, updates _gatheringTimeout, updates _timeSpentGathering
	/// </summary>
	/// <param name="gatheringBuilding"></param>
	public void SetGatheringBuilding(GatheringBuilding gatheringBuilding) {
		if ( gatheringBuilding.AddGatheringPerson( this ) ) {
			_gatheringBuilding = gatheringBuilding;

			_gatheringTimeout = _gatheringBuilding.GatheringTime;

			// Divided by 2 to make the variable account for the whole time the character is spending gathering.
			_timeSpentGathering = _gatheringBuilding.TimeSpentGathering / 2; 
		}
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
	private Vector3 GetRandomPosition () {
		NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

		int maxIndices = navMeshData.indices.Length - 3;

		// pick the first indice of a random triangle in the nav mesh
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

		Debug.Log( $"Random point {point} is {Vector3.Distance( _gatheringBuilding.transform.position , point)}m from GatheringBuilding." );
		
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
		_carryingResources = 0;

		_agent.isStopped = true;

		//Vector3 _awayFromGatheringBuilding = ;
		//_agent.SetDestination(_awayFromGatheringBuilding );
		_hasResources = false;

		_nextGatheringRun = GetNextTime( _gatheringTimeout, 1.75f );
	}
}
