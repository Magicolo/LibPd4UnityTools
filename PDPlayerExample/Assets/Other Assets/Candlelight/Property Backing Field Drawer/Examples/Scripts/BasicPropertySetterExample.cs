using UnityEngine;

namespace Candlelight.Examples
{
	public class BasicPropertySetterExample : MonoBehaviour
	{
		// Use Candlelight.PropertyBackingField and specify the provider type and property name.
		[SerializeField, Candlelight.PropertyBackingField(typeof(BasicPropertySetterExample), "Int")]
		private int m_Int = 0;

		public int Int
		{
			get { return m_Int; }
			set
			{
				if (m_Int != value)
				{
					m_Int = value;
					Debug.Log(string.Format("set Int: {0}", m_Int));
		 		}
			}
		}

		// You can also specify the attribute type or field type of a different drawer you wish to use.
		[SerializeField, Candlelight.PropertyBackingField(
			typeof(BasicPropertySetterExample), "Float",
			typeof(RangeAttribute), 0f, 1f // override attribute type and its constructor args
		)]
		private float m_Float;

		public float Float
		{
			get { return m_Float; }
			set
			{
				if (m_Float != value)
				{
					m_Float = value;
					Debug.Log(string.Format("set Float: {0}", m_Float));
				}
			}
		}
	}
}