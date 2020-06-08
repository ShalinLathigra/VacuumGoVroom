using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(GrimeAffector))]
public class PlayerController : MonoBehaviour
{
	private GrimeAffector grimeAffector;
	private PlayerMovement playerMovement;

	[SerializeField]
	private float startRadius;
	[SerializeField]
	private float endRadius;
	[SerializeField]
	private float radiusIncreaseRate;

    // Start is called before the first frame update
    void Start()
    {
		playerMovement = GetComponent<PlayerMovement>();
        grimeAffector = GetComponent<GrimeAffector>();

		if (endRadius == 0)
			endRadius = grimeAffector.GetRadius();
		grimeAffector.SetRadius(startRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			grimeAffector.Toggle();
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			grimeAffector.Toggle();
			grimeAffector.SetRadius(startRadius);
		}
		if (Input.GetKey(KeyCode.Space))
		{
			grimeAffector.SetRadius(Mathf.Min(grimeAffector.GetRadius() + radiusIncreaseRate * Time.deltaTime, endRadius));
		}
    }
}
