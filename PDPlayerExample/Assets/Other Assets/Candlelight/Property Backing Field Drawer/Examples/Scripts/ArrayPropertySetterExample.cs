using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Candlelight.Examples
{
	public class ArrayPropertySetterExample : MonoBehaviour
	{
		// Serializable IList properties X should implement GetX and SetX methods.
		[SerializeField, PropertyBackingField(typeof(ArrayPropertySetterExample), "ArrayProperty")]
		private int[] m_ArrayProperty = new int[1];

		public int[] GetArrayProperty()
		{
			return (int[])m_ArrayProperty.Clone();
		}

		public void SetArrayProperty(int[] value)
		{
			value = value ?? new int[0];
			if (m_ArrayProperty == null || !m_ArrayProperty.SequenceEqual(value))
			{
				m_ArrayProperty = (int[])value.Clone();
				Debug.Log(
					string.Format(
						"SetArrayProperty: [{0}]",
						string.Join(", ", (from element in m_ArrayProperty select element.ToString()).ToArray())
					)
				);
			}
		}

		// List<T> backing fields can work with Get/Set methods that take corresponding array types (or vice versa).
		[SerializeField, PropertyBackingField(typeof(ArrayPropertySetterExample), "ListProperty")]
		private List<int> m_ListProperty = new List<int>(new int[1]);
		
		public int[] GetListProperty()
		{
			return m_ListProperty.ToArray();
		}

		public void SetListProperty(int[] value)
		{
			value = value ?? new int[0];
			if (m_ListProperty == null || !m_ListProperty.SequenceEqual(value))
			{
				m_ListProperty = new List<int>(value);
				Debug.Log(
					string.Format(
						"SetListProperty: [{0}]",
						string.Join(", ", (from element in m_ListProperty select element.ToString()).ToArray())
					)
				);
			}
		}
	}
}