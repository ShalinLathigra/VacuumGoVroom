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
	*/

	[SerializeField]
	private int resolution;
	private Texture2D texture;
	Color[] colors;

	[SerializeField]
	private string textureReference;

	void Start()
	{	
		texture = new Texture2D(resolution * (int)transform.localScale.x, resolution * (int)transform.localScale.z);
		GetComponent<Renderer>().material.SetTexture(textureReference, texture);

		InitTexture();
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
		float deltaX = transform.position.x - input.x;
		float deltaZ = transform.position.z - input.z;

		int xPos = (int)(deltaX * resolution / 10) + (int)(texture.width / 2);
		int zPos = (int)(deltaZ * resolution / 10) + (int)(texture.height / 2);
		return new Vector2Int(xPos, zPos);
	}

	public void ModifyCircle(Vector3 position, int radius, int mode)
	{
		//Actually, can I Use a few squares to approximate this well enough?
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
							}
						}
					}
				}
			}
		}
        texture.Apply(false);
	}
}
