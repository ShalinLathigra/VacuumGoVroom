using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyController : MonoBehaviour
{

	[SerializeField]
	Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerTransform)
			playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		GetComponent<Renderer>().material.SetVector("Vector3_52467815", playerTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
		GetComponent<Renderer>().material.SetVector("Vector3_52467815", playerTransform.position);
    }
}
