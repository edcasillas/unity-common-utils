using System.Collections;
using System.Collections.Generic;

namespace CommonUtils.RestSdk {
	/// <summary>
	/// Wrapper for collections of DTOs retrieved from a REST service.
	/// </summary>
	/// <remarks>
	/// By convention (and because of limitations in the JsonUtility), in Unity DTO collections must be wrapped inside an
	/// object. This class provides a generic object to wrap them.
	/// See: https://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/
	/// </remarks>
	public class DtoCollection<TDto> : IEnumerable<TDto> {
		/// <summary>
		/// Internal collection of items.
		/// </summary>
		public TDto[] Items;

		/// <summary>
		/// Gets or sets the item in the specified index of the collection.
		/// </summary>
		/// <param name="index">Index in the internal collection of items.</param>
		/// <exception cref="KeyNotFoundException">When the specified index doesn't exist in the internal collection.</exception>
		public TDto this[int index] {
			get {
				if(Items != null && index >= 0 && index < Items.Length) {
					return Items[index];
				}
				throw new KeyNotFoundException();
			}
			set {
				if(Items != null && index >= 0 && index < Items.Length) {
					Items[index] = value;
				}
				throw new KeyNotFoundException();
			}
		}

		public IEnumerator<TDto> GetEnumerator() {
			return ((IEnumerable<TDto>)Items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Items.GetEnumerator();
		}

		public int Count() => Items?.Length ?? 0;

		public override string ToString() => $"DtoCollection: {typeof(TDto)} -> {Count()}";
	}
}