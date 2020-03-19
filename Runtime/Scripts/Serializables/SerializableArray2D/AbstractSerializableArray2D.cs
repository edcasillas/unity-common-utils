using System;
using UnityEngine;

namespace CommonUtils.Serializables.SerializableArray2D {
    [Serializable]
    public class AbstractSerializableArray2D<T,TArrayContainer> where TArrayContainer : AbstractArrayContainer<T>, new() {
        [SerializeField] private TArrayContainer[] rows;

        public T this[int index0, int index1] {
            get => rows[index0][index1];
            set => rows[index0][index1] = value;
        }

        public int RowCount => rows?.Length ?? 0;
        public int ColCount => RowCount > 0 ? rows[0].Length : 0;
        public int TotalSize => RowCount * ColCount;

        public AbstractSerializableArray2D(){}

        public AbstractSerializableArray2D(int rows, int cols) : this() {
            if (rows < 0) throw new ArgumentException("Value of rows cannot be less than zero.", nameof(rows));
            if (cols < 0) throw new ArgumentException("Value of cols cannot be less than zero.", nameof(cols));
            initArrays(rows, cols);
        }

        public AbstractSerializableArray2D(T[,] source) {
            if(source == null) throw new ArgumentNullException(nameof(source));
            initArrays(source.GetLength(0), source.GetLength(1));

            for (var i = 0; i < source.GetLength(0); i++) {
                for (var j = 0; j < source.GetLength(1); j++) {
                    this[i, j] = source[i, j];
                }
            }
        }

        private void initArrays(int rows, int cols) {
            this.rows = new TArrayContainer[rows];
            for (int i = 0; i < rows; i++) {
                this.rows[i] = new TArrayContainer();
                this.rows[i].SetSize(cols);
            }
        }
    }
}
