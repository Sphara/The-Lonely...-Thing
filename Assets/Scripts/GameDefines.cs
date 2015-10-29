using UnityEngine;
using System.Collections;

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