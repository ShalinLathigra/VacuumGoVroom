using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimeController : MonoBehaviour
{
	/*
		This handles updating the texture which is used in the Grime Layer
		This includes:
			Clearing Grime according to game rules
			Adding more Grime according to game rules

			Last thing that needs to happen is actual generation of the initial Grime Map, which is further affected by the player & other objects

			//Also want to be able to track all objects in the original scene so that it can calculate where exactly the initial map is set up.
	*/
	[SerializeField]
	GameObject[] solidObjects;
	[SerializeField]
	private int resolution;
	private Texture2D texture;
	Color[] colors;

	[SerializeField]
	private string textureReference;
	[SerializeField]
	private Renderer grimeRenderer;

	int maxSquares;
	int clearedSquares;
	Transform pTransform;

	void Start()
	{	
		if (!grimeRenderer)
			grimeRenderer = GameObject.FindGameObjectWithTag("GrimeLayer").GetComponent<Renderer>();
		pTransform = this.GetComponentInParent<Transform>();
		texture = new Texture2D(resolution * (int)pTransform.localScale.x, resolution * (int)pTransform.localScale.z);
		grimeRenderer.material.SetTexture(textureReference, texture);

		InitTexture();

		maxSquares = texture.width * texture.height - (texture.width + texture.height - 1);
		clearedSquares = 0;

		CullSolid();
	}

	private void InitTexture()
	{
		colors = new Color[2];
		colors[0] = new Color(0, 0, 0, 0);
        colors[1] = Color.white;
        int mipCount = Mathf.Min(3, texture.mipmapCount);

        Color[] cols = texture.GetPixels(0);
        for (int i = 0; i < cols.Length; ++i)
        {
            cols[i] = Color.Lerp(cols[i], colors[0], 0.33f);
        }
        texture.SetPixels(cols, 0);

        texture.Apply(false);
	}

	public Vector2Int ConvertPosToCoords (Vector3 input)
	{

		//translated coords (Position relative to center of pTransform)
		Vector3 translatedCoords = pTransform.position - input;
		//converted Coords accounting for scale & natural dimensions of the 5x5 plane
		Vector2 convertedCoords = new Vector2(translatedCoords.x / pTransform.localScale.x, translatedCoords.z / pTransform.localScale.z) / 5.0f;
		//This goes from -1 to 1
		convertedCoords = convertedCoords*0.5f + new Vector2(0.5f, 0.5f);
		convertedCoords.x *= texture.width;
		convertedCoords.y *= texture.height;
		return new Vector2Int((int)convertedCoords.x, (int)convertedCoords.y);
	}

	public void ModifyCircle(Vector3 position, int radius, int mode)
	{
		//Actually, can I Use a few squares to approximate this well enough? If this is too bad, do it this way instead
		/*
			First has side lengths 1.4 * rad 
			Second Set is 1.7 * rad  by rad & vice versa
			Third set is 2 * rad by .6 * rad & vice versa
		*/
		Vector2Int origin = ConvertPosToCoords(position);
		for (int i = -radius; i < radius; i++)
		{
			for (int j = -radius; j < radius; j++)
			{
				if (origin.x + i > 0 && origin.x + i < texture.width)
				{
					if (origin.y + j > 0 && origin.y + j < texture.height)
					{
						if (texture.GetPixel(origin.x + i, origin.y + j).a > 0)
						{
							if (i * i + j * j < radius * radius)
							{
								texture.SetPixel(origin.x + i, origin.y + j, colors[mode]);
								clearedSquares++;
							}
						}
					}
				}
			}
		}
        texture.Apply(false);
	}

	public void ModifySquare(Vector3 start, Vector3 end, int mode)
	{	
		//Generated from Collider.bounds in the call
		Vector2Int convertedStart = ConvertPosToCoords(start);
		Vector2Int convertedEnd = ConvertPosToCoords(end);

		for (int i = Mathf.Max(Mathf.Min(convertedStart.x, convertedEnd.x), 0); i<Mathf.Min(Mathf.Max(convertedStart.x, convertedEnd.x), texture.width); i++)
		{
			for (int j = Mathf.Max(Mathf.Min(convertedStart.y, convertedEnd.y), 0); j<Mathf.Min(Mathf.Max(convertedStart.y, convertedEnd.y), texture.height); j++)
			{
				texture.SetPixel(i, j, colors[mode]);
			}	
		}
		texture.Apply(false);
	}

	public void CullSolid()
	{
		foreach(GameObject g in solidObjects)
		{
			Vector3 start = g.transform.position;
			Bounds bounds = g.GetComponent<Collider>().bounds;
			ModifySquare(bounds.min, bounds.max, 0);
		}
	}
}
