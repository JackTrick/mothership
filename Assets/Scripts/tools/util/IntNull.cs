using System;

public class IntNull
{
	private int value_;
	public int Value { get { return value_; } set { value_ = value; defined_ = true; } }

	private bool defined_ = false;
	public bool Defined { get { return defined_; } }

	public IntNull()
	{
		value_ = 0;
	}

	public IntNull (int value)
	{
		value_ = value;
		defined_ = true;
	}

	public IntNull (int value, bool defined)
	{
		value_ = value;
		defined_ = defined;
	}

	/*
	public static int operator+ (int b, IntNull c) {
		int a;
		a = b + c.Value;
		return a;
	}

	public static int operator+ (IntNull b, int c) {
		int a;
		a = b.Value + c;
		return a;
	}

	public static int operator- (int b, IntNull c) {
		int a;
		a = b - c.Value;
		return a;
	}

	public static int operator- (IntNull b, IntNull c) {
		int a;
		a = b.Value - c;
		return a;
	}

	public static int operator* (int b, IntNull c) {
		int a;
		a = b * c.Value;
		return a;
	}

	public static int operator* (IntNull b, IntNull c) {
		int a;
		a = b.Value * c;
		return a;
	}

	public static int operator/ (int b, IntNull c) {
		int a;
		a = b / c.Value;
		return a;
	}

	public static int operator/ (IntNull b, IntNull c) {
		int a;
		a = b.Value / c;
		return a;
	}

	public static bool operator< (int b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator< (IntNull b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator<= (int b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator<= (IntNull b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator> (int b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator> (IntNull b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator>= (int b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}

	public static bool operator>= (IntNull b, IntNull c) {
		bool a;
		a = b < c.Value;
		return a;
	}
	*/
}


