using UnityEngine;
using System.Collections;

/// <summary>
/// A file with some useful structs/enums (First created to avoid duplicating the Coord struct)
/// </summary>

public struct Coord {
	public int x;
	public int y;
	
	public Coord(int _x, int _y) {
		x = _x;
		y = _y;
	}
};

public struct CollisionsInfo {
	public bool above, below, left, right;
	public bool climbingASlope;
	public float slopeAngle, slopeAngleOld;
	
	public void Reset () {
		above = below = left = right = false;
		climbingASlope = false;
		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
};