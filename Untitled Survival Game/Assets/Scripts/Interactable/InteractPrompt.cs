using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
	[SerializeField]
	private LayerMask _viewMask;

	[SerializeField]
	private TextMeshProUGUI _prompt;

	private Transform _camera;

	private const float _viewRange = 2f;


	void FixedUpdate()
	{
		if (_camera == null)
		{
			_camera = Camera.main.transform;
		}


		if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hitInfo, _viewRange, _viewMask.value))
		{
			Interactable interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();

			if (interactable != null)
			{
				_prompt.text = interactable.InteractPrompt;
				_prompt.enabled = true;
			}
		}
		else
		{
			_prompt.enabled = false;
		}

	}
}
