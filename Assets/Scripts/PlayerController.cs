using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(GrimeAffector))]
public class PlayerController : MonoBehaviour
{
	private GrimeAffector grimeAffector;
	private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
		playerMovement = GetComponent<PlayerMovement>();
        grimeAffector = GetComponent<GrimeAffector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			grimeAffector.Toggle();
		}
    }
}
