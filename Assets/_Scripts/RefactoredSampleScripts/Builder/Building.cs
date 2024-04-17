using System.Collections.Generic;
using System.Linq;
using _Scripts.ScriptableObjects;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.RefactoredSampleScripts.Builder
{
	[RequireComponent(typeof(Damageable))]
	public class Building : MonoBehaviour
	{
		public string buildingName;
		
		[SerializeField] float height;
		public float radius = 5;
		float originalHeight;
		[SerializeField] int totalWorkToComplete = 100;
		int currentWork;
		Transform buildingTransform;
		[HideInInspector] public Damageable attackable;
		public bool isHover = false;
		private bool done;
		MeshRenderer buildingRender;

		Cinemachine.CinemachineImpulseSource impulse;
		[ColorUsage(true, true)] [SerializeField] private Color[] stateColors;
		private Tween buildingTween;
		[Header( "References" )]
		[SerializeField] private MeshFilter _buildingPreviewMeshFilter;

		[Header("Building Prices")]
		[SerializeField] private PurchasePrice[] _purchasePrice;

		private void Awake()
		{
			attackable = GetComponent<Damageable>();

			// If the previewmeshfilter does not exist, find one.
			if (_buildingPreviewMeshFilter == default) {
				_buildingPreviewMeshFilter = GetComponentInChildren<MeshFilter>(); ;
			}
		}

		void Start()
		{
			buildingTransform = transform.GetChild(0);
			buildingRender = buildingTransform.GetComponent<MeshRenderer>();
			impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
			currentWork = 0;
			originalHeight = buildingTransform.localPosition.y;
			buildingTransform.localPosition = Vector3.down * height;
			buildingTween = buildingTransform.DOLocalMoveY(originalHeight, (float)totalWorkToComplete / totalWorkToComplete).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause();

		}
		public void Build(int work)
		{
			currentWork += work;
			buildingTransform.localPosition = Vector3.Lerp(Vector3.down * height, new Vector3(0,originalHeight,0), (float)currentWork / totalWorkToComplete);

			buildingTween.Play();
			//visual
			buildingTransform.DOComplete();
			buildingTransform.DOShakeScale(.5f, .2f, 10, 90, true);
			BuildingManager.instance.PlayParticle(transform.position);
		}
		public bool IsFinished()
		{
			if (currentWork >= totalWorkToComplete && !done && buildingRender)
			{
				done = true;
				buildingRender.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => buildingRender.material.DOColor(stateColors[0], "_EmissionColor", .5f));
				if (impulse)
					impulse.GenerateImpulse();
			}
			return currentWork >= totalWorkToComplete;
		}
		/// <summary>
		/// Returns PurchasePrices[]
		/// </summary>
		public PurchasePrice[] PurchasePrices { get { return _purchasePrice;} }

		/// <summary>
		/// Returns mesh used for previewing building.
		/// </summary>
		public Mesh GetBuildingPreviewMesh { get {
				if (_buildingPreviewMeshFilter == default) {
					return default;
				}

				return _buildingPreviewMeshFilter.sharedMesh; 
			} 
		}

		private void OnMouseEnter()
		{
			isHover = true;
		}
		private void OnMouseExit()
		{
			isHover = false;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}
