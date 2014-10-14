// 
// PropertyBackingFieldAttribute.cs
// 
// Author:
//       Adam Mechtley <adam@adammechtley.com>
//       http://adammechtley.com/donations
// 
// Copyright (c) 2014, Adam Mechtley
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
// 
// This file contains a custom PropertyAttribute to indicate that a serialized
// field is a backing field for a getter/setter. In order to use it, the
// property to which the field corresponds must implement both get and set
// methods (any access modifiers okay). These methods can be implemented either
// as a property or as methods (e.g., public int SomeInt { get; set; } or
// public int GetSomeInt() / public void SetSomeInt(int)).

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Candlelight
{
	/// <summary>
	/// A custom attribute for specifying that a field is a backing field for a property.
	/// </summary>
	public class PropertyBackingFieldAttribute : PropertyAttribute
	{
		/// <summary>
		/// The warning format string.
		/// </summary>
		private const string warningFormatString = "CA1819: Properties should not return arrays.\n\nConsider " +
			"implementing methods {0} Get{1}() and void Set{1}({0}) in class {2}.";

		/// <summary>
		/// Gets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public System.Func<object, object> Getter { get; private set; }

		/// <summary>
		/// Gets the override attribute.
		/// </summary>
		/// <value>The override attribute.</value>
		public PropertyAttribute OverrideAttribute { get; private set; }

		/// <summary>
		/// Gets the type of the property.
		/// </summary>
		/// <value>The type of the property.</value>
		public System.Type PropertyType { get; private set; }

		/// <summary>
		/// Gets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public System.Action<object, object> Setter { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.PropertySetterBackingFieldAttribute"/> class that
		/// uses the default property drawer for the field type.
		/// </summary>
		/// <param name="providerType">Type of the provider for the property.</param>
		/// <param name="propertyName">
		/// Name of the getter/setter property corresponding to the backing field, or name of getter/setter methods. For
		/// example, "Character" could refer to either a property Character Character { get; set; } or two methods
		/// Character GetCharacter() / SetCharacters(Character[] characters).
		/// </param>
		public PropertyBackingFieldAttribute(System.Type providerType, string propertyName)
		{
			PropertyInfo property = providerType.GetProperty(propertyName, ObjectX.instanceBindingFlags);
			if (property != null)
			{
				Getter = (provider) => property.GetValue(provider, null);
				if (property.GetSetMethod(true) != null)
				{
					Setter = (provider, value) => property.SetValue(
						provider, value == null || !PropertyType.IsAssignableFrom(value.GetType()) ? null : value, null
					);
				}
				PropertyType = property.PropertyType;
				if (typeof(System.Collections.IList).IsAssignableFrom(PropertyType))
				{
					Debug.LogWarning(string.Format(warningFormatString, PropertyType, propertyName, providerType));
				}
			}
			else
			{
				MethodInfo getter =
					providerType.GetMethod(string.Format("Get{0}", propertyName), ObjectX.instanceBindingFlags);
				Getter = (provider) => getter.Invoke(provider, null);
				PropertyType = getter.ReturnType;
				MethodInfo setter =
					providerType.GetMethod(string.Format("Set{0}", propertyName), ObjectX.instanceBindingFlags);
				if (setter != null)
				{
					Setter = (provider, value) => setter.Invoke(provider, new object[] { value });
				}
			}
			OverrideAttribute = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.PropertyBackingFieldAttribute"/> class that
		/// uses a specifically indicated property drawer to display the field in the inspector.
		/// </summary>
		/// <param name="providerType">Type of the provider for the property.</param>
		/// <param name="propertyName">
		/// Name of the getter/setter property corresponding to the backing field, or name of getter/setter methods. For
		/// example, "Characters" could refer to either a property Character[] Characters { get; set; } or to methods
		/// Character[] GetCharacters() / SetCharacters(Character[] characters).
		/// </param>
		/// <param name="propertyAttributeOverride">Type to specify what drawer should be used.</param>
		/// <param name="overrideParams">Parameters for the override drawer type.</param>
		public PropertyBackingFieldAttribute(
			System.Type providerType, string propertyName,
			System.Type propertyAttributeOverride, params object[] overrideParams
		) : this(providerType, propertyName)
		{
			OverrideAttribute = PropertyAttributeX.CreateNew(propertyAttributeOverride, overrideParams);
		}
	}
}