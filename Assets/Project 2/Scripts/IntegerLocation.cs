using System;
using UnityEngine;

public class IntegerLocation
{
	public int x;
	public int y;

	public IntegerLocation (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public IntegerLocation(double x, double y)
	{	
		this.x = (int)Math.Round(x);
		this.y = (int)Math.Round(y);
	}
	public IntegerLocation(Vector2 v)
	{
		this.x = (int)Math.Round(v.x);
		this.y = (int)Math.Round(v.y);
	}

	public static bool operator ==(IntegerLocation lhs, IntegerLocation rhs)
	{
		return ((lhs.x == rhs.x) && (lhs.y == rhs.y));
	}

	public static bool operator !=(IntegerLocation lhs, IntegerLocation rhs)
	{
		return !((lhs.x == rhs.x) && (lhs.y == rhs.y));
	}

	public override string ToString()
	{
		return "(" + x + "," + y + ")";
	}

	public override bool Equals (object obj)
	{
		IntegerLocation other = obj as IntegerLocation;
		return (this == other);
	}
	public override int GetHashCode ()
	{
		return (x + y) * (x + y + 1) / 2 + x;
	}

	public static int Distance(IntegerLocation a, IntegerLocation b)
	{
		return (int)Math.Round(Math.Sqrt ((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y)));
	}

}

