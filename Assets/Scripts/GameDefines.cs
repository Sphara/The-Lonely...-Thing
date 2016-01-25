using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A file with some useful structs/enums (First created to avoid duplicating the Coord struct)
/// </summary>

public struct Coord {
	public int x;
	public int y;

	public class EqualityComparer : IEqualityComparer<Coord>
	{
		public bool Equals(Coord x, Coord y)
		{
			return (x.x == y.x && x.y == y.y);
		}

		public int GetHashCode(Coord x)
		{
			return x.x.GetHashCode() ^ x.y.GetHashCode();
		}
	}

	public Coord(int _x, int _y) {
		x = _x;
		y = _y;
	}
};

public struct CollisionsInfo {
	public bool above, below, left, right;
	public bool climbingASlope;
	public bool descendingASlope;
	public float slopeAngle, slopeAngleOld;
	public Vector3 oldVelocity;

	public void Reset () {
		above = below = left = right = false;
		climbingASlope = descendingASlope = false;
		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
};
