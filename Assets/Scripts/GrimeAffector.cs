using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimeAffector : MonoBehaviour
{
/*
 *	This requires:
 *		GrimeController to be created
 *		Maybe create a GrimeAffector which contains a radius, add/remove bool, reference to GrimeController, reference to Parent Transform, active bool
 *		Player needs a GrimeAffector set to Remove, other creatures need one set to Add
 */
	[SerializeField]
	private GrimeController grimeController;
	
	public enum Mode {Subtract, Add, Unassigned};

	[SerializeField]
	private Mode mode;
	private bool active;

	[SerializeField]
	private float effectRadius;
	
    // Start is called before the first frame update
    void Start()
    {
		active = false;
		if (!grimeController)
			grimeController = GameObject.FindGameObjectWithTag("GrimeLayer").GetComponent<GrimeController>();
    }

	void Update()
	{
		if (active)
			grimeController.ModifyCircle(transform.position, (int)effectRadius, (int)mode);
	}

	public void Init(Mode _mode, bool _active, int _effectRadius)
	{
		mode = _mode;
		active = _active;
		effectRadius = _effectRadius;
	}

	public void Toggle()
	{
		active = !active;
	}

	public float GetRadius()
	{
		return effectRadius;
	}

	public void SetRadius(float _effectRadius)
	{
		effectRadius = _effectRadius;
	}
}
