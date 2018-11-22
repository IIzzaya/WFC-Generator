using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntVector3 {
	public int X;
	public int Y;
	public int Z;

	public IntVector3(int x, int y, int z) {
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public IntVector3(Vector3 vector) {
		this.X = (int) vector.x;
		this.Y = (int) vector.y;
		this.Z = (int) vector.z;
	}

	public static IntVector3 operator +(IntVector3 a, IntVector3 b) {
		return new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static IntVector3 operator -(IntVector3 a, IntVector3 b) {
		return new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public static IntVector3 operator -(IntVector3 v) {
		return new IntVector3(-v.X, -v.Y, -v.Z);
	}

	public static IntVector3 operator *(int f, IntVector3 b) {
		return new IntVector3(f * b.X, f * b.Y, f * b.Z);
	}
	public static IntVector3 operator *(IntVector3 b, int f) {
		return new IntVector3(f * b.X, f * b.Y, f * b.Z);
	}

	public static IntVector3 operator /(IntVector3 b, int d) {
		return new IntVector3(b.X / d, b.Y / d, b.Z / d);
	}

	public static bool operator ==(IntVector3 a, IntVector3 b) {
		return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
	}

	public static bool operator !=(IntVector3 a, IntVector3 b) {
		return !(a == b);
	}

	public override bool Equals(object obj) {
		if (!(obj is IntVector3)) {
			return false;
		}
		return this == (IntVector3) obj;
	}

	public override int GetHashCode() {
		return this.X * 1111111111 + this.Y * 2222 + this.Z * 3333333;
	}

	public Vector3 ToVector3() {
		return new Vector3(this.X, this.Y, this.Z);
	}

	public override string ToString() {
		return "(" + this.X + ", " + this.Y + ", " + this.Z + ")";
	}

	public float Magnitude {
		get {
			return Mathf.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
		}
	}

	// end Excluded;
	public bool Inside(IntVector3 start, IntVector3 end) {
		if (start.X > X || X >= end.X) return false;
		if (start.Y > Y || Y >= end.Y) return false;
		if (start.Z > Z || Z >= end.Z) return false;
		return true;
	}

	public static IntVector3 zero {
		get { return new IntVector3(0, 0, 0); }
	}

	public static IntVector3 up {
		get { return new IntVector3(0, 1, 0); }
	}

	public static IntVector3 down {
		get { return new IntVector3(0, -1, 0); }
	}

	public static IntVector3 right {
		get { return new IntVector3(1, 0, 0); }
	}

	public static IntVector3 left {
		get { return new IntVector3(-1, 0, 0); }
	}

	public static IntVector3 front {
		get { return new IntVector3(0, 0, 1); }
	}

	public static IntVector3 back {
		get { return new IntVector3(0, 0, -1); }
	}
}