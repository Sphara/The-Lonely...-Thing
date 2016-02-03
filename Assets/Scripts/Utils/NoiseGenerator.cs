using UnityEngine;
using System.Collections;

/// <summary>
/// A noise generator. Right now, it only uses Perlin noise.
/// 
/// Offsets the positions asked for with randomly generated (or user-defined) values to allow multiple generations with a single instance
/// </summary>

public class NoiseGenerator {

	int xPos;
	int yPos;

	public NoiseGenerator (int x, int y) 
	{
		xPos = x;
		yPos = y;
	}

	public NoiseGenerator () 
	{
		Reset ();
	}

	public void Reset () 
	{
		xPos = (int)Random.Range (0, 100000);
		yPos = (int)Random.Range (0, 100000);
	}

	/// <summary>
	/// Perlins the noise. Well, not really but monodevelop told me that and i find it funny, so i'll let that here.
	/// </summary>
	/// <returns>The noise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="scale">How far the values are from each other</param>
	/// <param name="mag">Kind of the same thing, but not really</param>
	/// <param name="exp">The result of the Mathf.PerlinNoise is at this exponent</param>

	public float PerlinNoise (int x, int y, float scale, float mag, float exp)
	{
		return (Mathf.Pow ((Mathf.PerlinNoise((xPos + x) / scale, (y + yPos) / scale) * mag), (exp))); 
	}
}
