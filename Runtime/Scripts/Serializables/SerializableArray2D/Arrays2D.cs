using System;

namespace CommonUtils.Serializables.SerializableArray2D {
	[Serializable]
	public class BoolArray2D : AbstractSerializableArray2D<bool, BoolArrayContainer> {
		public BoolArray2D() : base() { }
		public BoolArray2D(int     rows, int cols) : base(rows, cols) { }
		public BoolArray2D(bool[,] source) : base(source) { }
	}

	[Serializable]
	public class IntArray2D : AbstractSerializableArray2D<int, IntArrayContainer> {
		public IntArray2D() : base() { }
		public IntArray2D(int     rows, int cols) : base(rows, cols) { }
		public IntArray2D(int[,] source) : base(source) { }
	}

	[Serializable]
	public class FloatArray2D : AbstractSerializableArray2D<float, FloatArrayContainer> {
		public FloatArray2D() : base() { }
		public FloatArray2D(int     rows, int cols) : base(rows, cols) { }
		public FloatArray2D(float[,] source) : base(source) { }
	}

	[Serializable]
	public class StringArray2D : AbstractSerializableArray2D<string, StringArrayContainer> {
		public StringArray2D() : base() { }
		public StringArray2D(int     rows, int cols) : base(rows, cols) { }
		public StringArray2D(string[,] source) : base(source) { }
	}
}