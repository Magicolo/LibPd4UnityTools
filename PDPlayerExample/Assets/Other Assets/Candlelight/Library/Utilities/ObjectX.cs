// 
// ObjectX.cs
// 
// Author:
//       Adam Mechtley <adam@adammechtley.com>
//       http://adammechtley.com/donations
// 
// Copyright (c) 2012-2014, Adam Mechtley
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
// This file contains a class with extension methods for System.Object
// and UnityEngine.Object.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Candlelight
{
	/// <summary>
	/// Extension methods for System.Object and UnityEngine.Object.
	/// </summary>
	public static class ObjectX
	{
		/// <summary>
		/// The binding flags for instance fields and properties.
		/// </summary>
		public const BindingFlags instanceBindingFlags =
			BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		/// <summary>
		/// The binding flags for static fields and properties.
		/// </summary>
		public const BindingFlags staticBindingFlags =
			BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

		/// <summary>
		/// A regular expression to match an instance's prefab name
		/// </summary>
		private static readonly System.Text.RegularExpressions.Regex matchPrefabName =
			new System.Text.RegularExpressions.Regex(@".+(?=\s*\(Clone\))|.+");

		#region Backing Fields
		private static ReadOnlyCollection<System.Type> m_AllTypes = null;
		private static ReadOnlyCollection<Assembly> m_UnityRuntimeAssemblies = null;
		#endregion

		/// <summary>
		/// Gets all types in the current application.
		/// </summary>
		/// <value>All types in the current application.</value>
		public static ReadOnlyCollection<System.Type> AllTypes
		{
			get
			{
				if (m_AllTypes == null)
				{
					HashSet<System.Type> allTypes = new HashSet<System.Type>();
					foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
					{
						foreach (System.Type t in assembly.GetTypes())
						{
							allTypes.Add(t);
						}
					}
					m_AllTypes = new ReadOnlyCollection<System.Type>(allTypes.ToArray());
				}
				return m_AllTypes;
			}
		}

		/// <summary>
		/// Gets the Unity runtime assemblies.
		/// </summary>
		/// <value>The Unity runtime assemblies.</value>
		public static ReadOnlyCollection<Assembly> UnityRuntimeAssemblies
		{
			get
			{
				if (m_UnityRuntimeAssemblies == null)
				{
					m_UnityRuntimeAssemblies = new ReadOnlyCollection<Assembly>(
						(
							from assembly in System.AppDomain.CurrentDomain.GetAssemblies() select assembly
						).Where(
							assembly => assembly.GetName().Name.StartsWith("UnityEngine")
						).ToArray()
					);
				}
				return m_UnityRuntimeAssemblies;
			}
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <remarks>
		/// http://msdn.microsoft.com/en-us/library/dwc6ew1d(v=vs.90).aspx
		/// </remarks>
		/// <returns>The custom attributes on the provider.</returns>
		/// <param name='provider'>Provider.</param>
		/// <param name='inherit'>
		/// <c>true</c> if inherited attributes should be included; otherwise, <c>false</c>.
		/// </param>
		/// <typeparam name='T'>The type of the custom attributes desired.</typeparam>
		/// <exception cref='System.ArgumentNullException'>
		/// Is thrown if the provider is <see langword="null" />.
		/// </exception>
	    public static T[] GetCustomAttributes<T>(
			this ICustomAttributeProvider provider, bool inherit = false
		) where T : System.Attribute
	    {
	        if (provider == null)
			{
	            throw new System.ArgumentNullException("provider");
			}
	        T[] attributes = provider.GetCustomAttributes(typeof(T), inherit) as T[];
	        if (attributes == null)
	        {   // WORKAROUND: Due to a bug in the code for retrieving attributes from a dynamic generated parameter,
				// GetCustomAttributes can return an instance of an object[] instead of T[], and hence the cast above
	            // will return null.
	            return new T[0];
	        }
	        return attributes;
	    }
		
		/// <summary>
		/// Gets the field value on an instance.
		/// </summary>
		/// <returns>The field value on an instance.</returns>
		/// <param name="provider">Provider.</param>
		/// <param name="fieldName">Field name.</param>
		/// <param name="bindingAttr">Binding flags.</param>
		/// <typeparam name="T">The type of the field.</typeparam>
		public static T GetFieldValue<T>(
			this object provider, string fieldName, BindingFlags bindingAttr = instanceBindingFlags
		)
		{
			return GetFieldValue<T>(provider.GetType(), provider, fieldName, bindingAttr);
		}
		
		/// <summary>
		/// Gets the field value on a provider
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="provider">Provider.</param>
		/// <param name="fieldName">Field name.</param>
		/// <param name="bindingAttr">Binding attr.</param>
		/// <typeparam name="T">The type of the field.</typeparam>
		/// <exception cref="System.ArgumentNullException">
		/// Is thrown if the type or fieldName is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.ArgumentException">Is thrown if fieldName is empty.</exception>
		private static T GetFieldValue<T>(
			System.Type type, object provider, string fieldName, BindingFlags bindingAttr
		)
		{
			if (type == null)
			{
				throw new System.ArgumentNullException("type");
			}
			else if (fieldName == null)
			{
				throw new System.ArgumentNullException("fieldName");
			}
			else if (fieldName.Length == 0)
			{
				throw new System.ArgumentException("No field name specified.", "fieldName");
			}
			return (T)type.GetField(fieldName, bindingAttr).GetValue(provider);
		}

		/// <summary>
		/// Gets the name of the prefab associated with the supplied instance.
		/// </summary>
		/// <returns>The prefab name.</returns>
		/// <param name="instance">Instance.</param>
		public static string GetPrefabName(this UnityEngine.Object instance)
		{
			return matchPrefabName.Match(instance.name).Value;
		}

		/// <summary>
		/// Gets the property value on an instance.
		/// </summary>
		/// <returns>The property value on an instance.</returns>
		/// <param name="provider">Provider.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="bindingAttr">Binding flags.</param>
		/// <param name="index">Index.</param>
		/// <typeparam name="T">The type of the property.</typeparam>
		public static T GetPropertyValue<T>(
			this object provider,
			string propertyName,
			BindingFlags bindingAttr = instanceBindingFlags,
			object[] index = null
		)
		{
			return GetPropertyValue<T>(provider.GetType(), provider, propertyName, bindingAttr, index);
		}
		
		/// <summary>
		/// Gets the property value on a provider.
		/// </summary>
		/// <returns>The property value on an instance.</returns>
		/// <param name="provider">Provider.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="bindingAttr">Binding flags.</param>
		/// <param name="index">Index.</param>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <exception cref="System.ArgumentNullException">
		/// Is thrown if the provider or propertyName is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.ArgumentException">Is thrown if propertyName is empty.</exception>
		private static T GetPropertyValue<T>(
			System.Type type, object provider, string propertyName, BindingFlags bindingAttr, object[] index
		)
		{
			if (provider == null)
			{
				throw new System.ArgumentNullException("provider");
			}
			else if (propertyName == null)
			{
				throw new System.ArgumentNullException("propertyName");
			}
			else if (propertyName.Length == 0)
			{
				throw new System.ArgumentException("No property name specified.", "propertyName");
			}
			return (T)provider.GetType().GetProperty(propertyName, bindingAttr).GetValue(provider, index);
		}
		
		/// <summary>
		/// Gets the field value on a class.
		/// </summary>
		/// <returns>The field value on a class.</returns>
		/// <param name="type">Type.</param>
		/// <param name="fieldName">Field name.</param>
		/// <param name="bindingAttr">Binding flags.</param>
		/// <typeparam name="T">The type of the field.</typeparam>
		public static T GetStaticFieldValue<T>(
			this System.Type type, string fieldName, BindingFlags bindingAttr = staticBindingFlags
		)
		{
			return GetFieldValue<T>(type, null, fieldName, bindingAttr);
		}

		/// <summary>
		/// Gets the property value on a class.
		/// </summary>
		/// <returns>The property value on a class.</returns>
		/// <param name="provider">Provider.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="bindingAttr">Binding flags.</param>
		/// <param name="index">Index.</param>
		/// <typeparam name="T">The type of the property.</typeparam>
		public static T GetStaticPropertyValue<T>(
			this System.Type type,
			string propertyName,
			BindingFlags bindingAttr = instanceBindingFlags,
			object[] index = null
			)
		{
			return GetPropertyValue<T>(type, null, propertyName, bindingAttr, index);
		}
	}
}