using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Hexmap
{
	/// <summary>
	/// Component that controls the singleton camera that navigates the hex map.
	/// </summary>
	public class HexMapCamera : MonoBehaviour
	{
		[SerializeField] private float stickMinZoom, stickMaxZoom;

		[SerializeField] private float swivelMinZoom, swivelMaxZoom;

		[SerializeField] private float moveSpeedMinZoom, moveSpeedMaxZoom;

		[SerializeField] private float rotationSpeed;

		[SerializeField] private HexGrid grid;

		private Transform swivel, stick;

		private float zoom = 1f;

		private float rotationAngle;

		private static HexMapCamera _instance;

		/// <summary>
		/// Whether the singleton camera controls are locked.
		/// </summary>
		public static bool Locked
		{
			set => _instance.enabled = !value;
		}

		/// <summary>
		/// Validate the position of the singleton camera.
		/// </summary>
		public static void ValidatePosition() => _instance.AdjustPosition(0f, 0f);

		private void Awake()
		{
			swivel = transform.GetChild(0);
			stick = swivel.GetChild(0);
		}

		private void OnEnable()
		{
			_instance = this;
			ValidatePosition();
		}

		private void Update()
		{
			float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
			if (zoomDelta != 0f)
			{
				AdjustZoom(zoomDelta);
			}

			float rotationDelta = Input.GetAxis("Rotation");
			if (rotationDelta != 0f)
			{
				AdjustRotation(rotationDelta);
			}

			float xDelta = Input.GetAxis("Horizontal");
			float zDelta = Input.GetAxis("Vertical");
			if (xDelta != 0f || zDelta != 0f)
			{
				AdjustPosition(xDelta, zDelta);
			}
		}

		private void AdjustZoom(float delta)
		{
			zoom = Mathf.Clamp01(zoom + delta);

			float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
			stick.localPosition = new Vector3(0f, 0f, distance);

			float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
			swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
		}

		private void AdjustRotation (float delta)
		{
			rotationAngle += delta * rotationSpeed * Time.deltaTime;
			if (rotationAngle < 0f)
			{
				rotationAngle += 360f;
			}
			else if (rotationAngle >= 360f)
			{
				rotationAngle -= 360f;
			}
			transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
		}

		private void AdjustPosition(float xDelta, float zDelta)
		{
			Vector3 direction =
				transform.localRotation *
				new Vector3(xDelta, 0f, zDelta).normalized;
			float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
			float distance =
				Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
				damping * Time.deltaTime;

			Vector3 position = transform.localPosition;
			position += direction * distance;
			transform.localPosition =
				grid.Wrapping ? WrapPosition(position) : ClampPosition(position);
		}

		private Vector3 ClampPosition(Vector3 position)
		{
			float xMax = (grid.CellCountX - 0.5f) * HexMetrics.InnerDiameter;
			position.x = Mathf.Clamp(position.x, 0f, xMax);

			float zMax = (grid.CellCountZ - 1) * (1.5f * HexMetrics.OuterRadius);
			position.z = Mathf.Clamp(position.z, 0f, zMax);

			return position;
		}

		private Vector3 WrapPosition(Vector3 position)
		{
			float width = grid.CellCountX * HexMetrics.InnerDiameter;
			while (position.x < 0f)
			{
				position.x += width;
			}
			while (position.x > width)
			{
				position.x -= width;
			}

			float zMax = (grid.CellCountZ - 1) * (1.5f * HexMetrics.OuterRadius);
			position.z = Mathf.Clamp(position.z, 0f, zMax);

			grid.CenterMap(position.x);
			return position;
		}
	}
}
