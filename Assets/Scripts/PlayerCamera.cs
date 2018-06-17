using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
	[SerializeField] Vector3 center;
	[SerializeField] Vector3 blackCameraPos;

	[SerializeField] float zoomSpeed;
	[SerializeField] float rotationSpeed;

	[Header("Zoom")]
	[SerializeField] int mouseButton = 3;
	[SerializeField] float maxInZoom;
	[SerializeField] float maxOutZoom;

	float curZoom = 0;
	Vector3 prevMousePos;

	public void PointToBlack()
	{
		transform.position = blackCameraPos;
		transform.Rotate(Vector3.up * 180, Space.World);
	}

	void Update()
	{
		Zoom();
		Rotate();
	}

	void Zoom()
	{
		float rot = Input.GetAxis("Mouse ScrollWheel");

		rot = Mathf.Clamp(rot, maxOutZoom - curZoom, maxInZoom - curZoom);
		curZoom += rot;

		transform.Translate(rot * Vector3.forward * Time.deltaTime * zoomSpeed, Space.Self);
	}

	void Rotate()
	{
		Vector3 curMousePos = Input.mousePosition;

		if (Input.GetMouseButton(mouseButton))
		{
			Vector3 delta = (curMousePos - prevMousePos).normalized;
			transform.RotateAround(center, Vector3.up, delta.x * Time.deltaTime * rotationSpeed);
			transform.RotateAround(center, transform.right, -delta.y * Time.deltaTime * rotationSpeed);
		}
		prevMousePos = curMousePos;
	}
}
