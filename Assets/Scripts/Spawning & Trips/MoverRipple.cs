using UnityEngine;
using System.Collections.Generic;

public class MoverRipple : MonoBehaviour
{
	public float rippleWidth = 10;
	public float speed = 1;
	public Vector3 rippleDirection = Vector3.up;

	List<Transform> transforms = null;
	List<Vector3> originalPositions = null;
	List<float> phaseOffsets = null;
	float phase = 0;
	
	void Start ()	{
		if (transforms != null)
			return;
		transforms = new List<Transform> ();
		originalPositions = new List<Vector3> ();
		phaseOffsets = new List<float> ();
		for (int i = 0; i<transform.childCount;i++){
			Transform child = transform.GetChild(i);
			transforms.Add(child);
			originalPositions.Add (child.localPosition);
			phaseOffsets.Add(child.localPosition.magnitude);
		}
	}
	
	void Update (){
		phase += Time.deltaTime * speed;
		for (int i = 0; i < transforms.Count; i++){
			float phaseOffset = phaseOffsets[i] / rippleWidth;
			float p = phase + phaseOffset;
			
			Transform t = transforms[i];
			t.localPosition = originalPositions[i] + rippleDirection *Mathf.Sin (2 * Mathf.PI * p);
		}
	}
}