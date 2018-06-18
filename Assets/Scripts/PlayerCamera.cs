using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
	[SerializeField] Vector3 center;
	[SerializeField] Vector3 blackCameraPos;

	[SerializeField] float zoomSpeed;
	[SerializeField] float rotationSpeedX;
	[SerializeField] float rotationSpeedY;

	[Header("Zoom")]
	[SerializeField] int mouseButton = 3;
	[SerializeField] float maxInZoom;
	[SerializeField] float maxOutZoom;

	float curZoom = 0;
	Vector3 prevMousePos;

	Vector3 initialPos;
	Vector3 initialRot;

	void Start()
	{
		initialPos = this.transform.position;
		initialRot = this.transform.rotation.eulerAngles;
	}

	public void SetDefaultPos(bool pointToWhite)
	{
		transform.position = pointToWhite ? initialPos : blackCameraPos;
		transform.rotation = Quaternion.Euler(initialRot + (pointToWhite ? Vector3.zero : Vector3.up * 180));
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
			transform.RotateAround(center, Vector3.up, delta.x * Time.deltaTime * rotationSpeedX);
			transform.RotateAround(center, transform.right, -delta.y * Time.deltaTime * rotationSpeedY);
		}
		prevMousePos = curMousePos;
	}
}
