using System.Collections;
using System.Collections.Generic;

namespace ExaGames.RestSdk {
	/// <summary>
	/// Wrapper para colecciones de DTOs obtenidas desde un servicio REST.
	/// </summary>
	/// <remarks>
	/// Por convención de Unity (y limitaciones de JsonUtility) se debe wrappear las colecciones en DTOs de esta manera
	/// Referencia: https://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/
	/// </remarks>
	public class DtoCollection<TDto> : IEnumerable<TDto> {
		/// <summary>
		/// Colección interna de items.
		/// </summary>
		public TDto[] Items;

		/// <summary>
		/// Obtiene o establece el item en el índice especificado.
		/// </summary>
		/// <param name="index">Índice de la colección.</param>
		/// <exception cref="KeyNotFoundException">Cuando el índice especificado no existe en la colección.</exception>
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

		public int Count() {
			return Items != null ? Items.Length : 0;
		}

		public override string ToString() {
			return string.Format("DtoCollection: {0} -> {1}", typeof(TDto), Count());
		}
	}
}