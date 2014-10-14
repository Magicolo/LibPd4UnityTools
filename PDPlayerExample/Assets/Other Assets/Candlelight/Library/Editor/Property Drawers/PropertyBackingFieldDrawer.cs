// 
// PropertyBackingFieldDrawer.cs
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
// This file contains a custom PropertyDrawer for a serialized backing field of
// a getter/setter.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Candlelight
{
	/// <summary>
	/// A custom property drawer for a property backing field that will use the corresponding setter to validate data.
	/// </summary>
	[InitializeOnLoad, CustomPropertyDrawer(typeof(PropertyBackingFieldAttribute))]
	public class PropertyBackingFieldDrawer : PropertyDrawer
	{
		/// <summary>
		/// A hashable structure for storing a serialized property.
		/// </summary>
		private struct HashableSerializedProperty
		{
			#region Backing Fields
			private int m_Hash;
			private string m_PropertyPath;
			private Object m_TargetObject;
			#endregion

			/// <summary>
			/// Gets the property path.
			/// </summary>
			/// <value>The property path.</value>
			public string PropertyPath { get { return m_PropertyPath; } }

			/// <summary>
			/// Gets the serialized property.
			/// </summary>
			/// <value>The serialized property.</value>
			public SerializedProperty SerializedProperty
			{
				get
				{
					if (m_TargetObject == null)
					{
						return null;
					}
					SerializedObject so = new SerializedObject(m_TargetObject);
					if (so == null)
					{
						return null;
					}
					return so.FindProperty(m_PropertyPath);
				}
			}
			
			/// <summary>
			/// Gets the target object.
			/// </summary>
			/// <value>The target object.</value>
			public Object TargetObject { get { return m_TargetObject; } }

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="Candlelight.PropertyBackingFieldDrawer+HashableSerializedProperty"/> struct.
			/// </summary>
			/// <param name="path">Path.</param>
			/// <param name="target">Target.</param>
			public HashableSerializedProperty(string path, Object target) : this()
			{
				m_PropertyPath = path;
				m_TargetObject = target;
				// store the hash upon construction, in case the object is killed later
				m_Hash = m_PropertyPath.GetHashCode() ^ m_TargetObject.GetHashCode();
			}

			/// <summary>
			/// Initializes a new instance of the
			/// <see cref="Candlelight.PropertyBackingFieldDrawer+HashableSerializedProperty"/> struct.
			/// </summary>
			/// <param name="mod">Property modification.</param>
			public HashableSerializedProperty(PropertyModification mod) : this(mod.propertyPath, mod.target)
			{

			}

			/// <summary>
			/// Serves as a hash function for a <see cref="Candlelight.PropertyBackingFieldDrawer+ArrayProperty"/>
			/// object.
			/// </summary>
			/// <returns>
			/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as
			/// a hash table.
			/// </returns>
			public override int GetHashCode()
			{
				return m_Hash;
			}

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents the current
			/// <see cref="Candlelight.PropertyBackingFieldDrawer+HashableSerializedProperty"/>.
			/// </summary>
			/// <returns>
			/// A <see cref="System.String"/> that represents the current
			/// <see cref="Candlelight.PropertyBackingFieldDrawer+HashableSerializedProperty"/>.
			/// </returns>
			public override string ToString()
			{
				return string.Format(
					"[HashableSerializedProperty: TargetObject={0}, PropertyPath={1}]", m_TargetObject, m_PropertyPath
				);
			}
		}

		/// <summary>
		/// The current selection.
		/// </summary>
		private static HashSet<Object> currentSelection = new HashSet<Object>();
		/// <summary>
		/// The property setter callback for each hashable property.
		/// </summary>
		private static Dictionary<HashableSerializedProperty, System.Action> propertySetterCallbacks =
			new Dictionary<HashableSerializedProperty, System.Action>();
		/// <summary>
		/// For each registered property, a cached backing field value. Used to flush the backing field right before the
		/// setter is invoked.
		/// </summary>
		private static Dictionary<HashableSerializedProperty, object> valueCache =
			new Dictionary<HashableSerializedProperty, object>();

		/// <summary>
		/// Initializes the <see cref="Candlelight.PropertyBackingFieldDrawer"/> class.
		/// </summary>
		static PropertyBackingFieldDrawer()
		{
			// add preference menu item
			EditorPreferenceMenu.AddPreferenceMenuItem(
				"Property Backing Field", () => EditorGUILayout.LabelField("Thanks for your bug reports!")
			);
			//register callbacks with UnityEditor.Undo
			Undo.postprocessModifications += OnPerformUndoableAction;
			Undo.undoRedoPerformed += OnUndoRedo;
			// get all types incompatible with this feature
			HashSet<System.Type> typesThatCannotBeBackingFields = new HashSet<System.Type>(
				ObjectX.AllTypes.Where(
					t =>
						(t.IsClass || (t.IsValueType && !t.IsEnum && !t.IsPrimitive)) &&
						t.GetCustomAttributes<System.SerializableAttribute>().Count() > 0 &&
						!typeof(IPropertyBackingFieldCompatible).IsAssignableFrom(t)
				)
			);
			typesThatCannotBeBackingFields.Remove(typeof(string));
			// collect T[] and List<T> for each incompatible type
			HashSet<System.Type> sequenceTypesThatCannotBeBackingFields = new HashSet<System.Type>();
			foreach (System.Type incompatibleType in typesThatCannotBeBackingFields)
			{
				sequenceTypesThatCannotBeBackingFields.Add(incompatibleType.MakeArrayType());
				sequenceTypesThatCannotBeBackingFields.Add(
					typeof(List<>).MakeGenericType(new System.Type[] { incompatibleType })
				);
			}
			// collect any fields that will cause problems with types that cannot be marked as backing fields
			Dictionary<System.Type, List<FieldInfo>> problematicFields = new Dictionary<System.Type, List<FieldInfo>>();
			// examine all fields on the scripted types to find any problematic usages
			foreach (System.Type providerType in ObjectX.AllTypes)
			{
				foreach (FieldInfo field in providerType.GetFields(ObjectX.instanceBindingFlags))
				{
					System.Type typeToValidate = field.FieldType;
					// skip the field if it is known to be compatible
					if (
						!typesThatCannotBeBackingFields.Contains(typeToValidate) &&
						!sequenceTypesThatCannotBeBackingFields.Contains(typeToValidate)
					)
					{
						continue;
					}
					// skip the field if it is a built-in Unity type
					if (ObjectX.UnityRuntimeAssemblies.Contains(typeToValidate.Assembly))
					{
						continue;
					}
					// skip the field if it is not serialized
					if (field.IsPrivate && field.GetCustomAttributes<SerializeField>().Count() == 0)
					{
						continue;
					}
					// skip the field if it is not designated as a backing field
					if (field.GetCustomAttributes<PropertyBackingFieldAttribute>().Count() == 0)
					{
						continue;
					}
					// add the type to the problem table
					if (typeToValidate.IsArray)
					{
						typeToValidate = typeToValidate.GetElementType();
					}
					else if (typeToValidate.IsGenericType)
					{
						typeToValidate = typeToValidate.GetGenericArguments()[0];
					}
					if (!problematicFields.ContainsKey(typeToValidate))
					{
						problematicFields.Add(typeToValidate, new List<FieldInfo>());
					}
					// add the field to the type's list of problematic usages
					problematicFields[typeToValidate].Add(field);
				}
			}
			// display messages for any problems
			foreach (KeyValuePair<System.Type, List<FieldInfo>> problematicType in problematicFields)
			{
				Debug.LogError(
					string.Format(
						"<b>{0}</b> must implement <b>{1}<{0}></b>, because it is marked with <b>{2}</b> on the following fields:\n{3}",
						problematicType.Key,
						typeof(IPropertyBackingFieldCompatible).FullName,
						typeof(PropertyBackingFieldAttribute).FullName,
						string.Join(
							"\n",
							(
								from field in problematicType.Value
								select string.Format(
									"    - <i>{0}.{1}</i>",
									field.DeclaringType, field.Name
								)
							).ToArray()
						)
					)
				);
			}
		}
		
		/// <summary>
		/// Gets any properties upstream of the supplied property. This method is used to determine when a parent setter
		/// might need to be invoked.
		/// </summary>
		/// <returns>The upstream properties.</returns>
		/// <param name="property">Property.</param>
		private static List<HashableSerializedProperty> GetUpstreamProperties(HashableSerializedProperty property)
		{
			List<HashableSerializedProperty> upstreamParents = new List<HashableSerializedProperty>();
			SerializedProperty sp = property.SerializedProperty;
			if (sp != null)
			{
				while (sp.GetParentProperty() != null)
				{
					sp = sp.GetParentProperty();
					upstreamParents.Add(new HashableSerializedProperty(sp.propertyPath, property.TargetObject));
				}
			}
			return upstreamParents;
		}

		/// <summary>
		/// Invokes the supplied property setter for the supplied hashable serialized property and update its value
		/// cache.
		/// </summary>
		/// <param name="hashableProperty">A hashable representation of the property's serialized backing field.</param>
		/// <param name="getter">A getter method that takes the provider as its argument.</param>
		/// <param name="setter">A setter method that takes the provider and new value as its arguments.</param> 
		/// <param name="propertyType">The type returned by the getter and specified in the setter signature.</param>
		private static void InvokeSetter(
			HashableSerializedProperty hashableProperty,
			System.Func<object, object> getter,
			System.Action<object, object> setter,
			System.Type propertyType
		)
		{
			SerializedProperty sp = hashableProperty.SerializedProperty;
			// mark for undo
			HashSet<Object> objectsToUndo = new HashSet<Object>();
			objectsToUndo.Add(hashableProperty.TargetObject);
			string undoName = string.Format("Change {0}", sp.GetDisplayName());
			// if it's on a monobehaviour, it may affect other objects in the hierarchy
			if (hashableProperty.TargetObject is MonoBehaviour)
			{
				MonoBehaviour monoBehaviour = hashableProperty.TargetObject as MonoBehaviour;
				foreach (Transform t in monoBehaviour.transform.root.GetComponentsInChildren<Transform>(true))
				{
					foreach (Component c in t.GetComponents<Component>())
					{
						objectsToUndo.Add(c);
					}
				}
			}
			Undo.RecordObjects(objectsToUndo.ToArray(), undoName);
			// get the providers
			FieldInfo fieldInfo;
			object provider = sp.GetProvider(out fieldInfo);
			// get the element index of the property being set, if any
			int elementIndex = hashableProperty.SerializedProperty.GetElementIndex();
			// flush inspector changes and store pending values
			sp.serializedObject.ApplyModifiedProperties();
			object pendingValue = null;
			// ensure IList backing field values are converted to the type expected by the setter
			if (elementIndex < 0 && typeof(IList).IsAssignableFrom(propertyType) && propertyType != fieldInfo.FieldType)
			{
				pendingValue = sp.GetConvertedIListValues(propertyType);
			}
			else
			{
				pendingValue = sp.GetValue();
			}
			// reset backing field to old values
			if (elementIndex >= 0)
			{
				(fieldInfo.GetValue(provider) as IList)[elementIndex] = valueCache[hashableProperty];
			}
			else
			{
				fieldInfo.SetValue(provider, valueCache[hashableProperty]);
			}
			// invoke the setter
			if (elementIndex >= 0)
			{
				// clone the result of the getter
				IList arrayValues = (IList)getter.Invoke(provider);
				if (typeof(System.Array).IsAssignableFrom(propertyType))
				{
					arrayValues = (IList)((System.ICloneable)arrayValues).Clone();
				}
				else
				{
					IList srcValues = arrayValues;
					arrayValues = (IList)System.Activator.CreateInstance(propertyType);
					for (int idx = 0; idx < srcValues.Count; ++idx)
					{
						arrayValues.Add(srcValues[idx]);
					}
				}
				// assign the pending element value
				arrayValues[elementIndex] = pendingValue;
				// invoke setter
				setter.Invoke(provider, arrayValues);
			}
			else
			{
				setter.Invoke(provider, pendingValue);
			}
			// set dirty
			EditorUtilityX.SetDirty(objectsToUndo.ToArray());
			// update cache
			valueCache[hashableProperty] = sp.GetValue();
		}

		/// <summary>
		/// Raises the perform undoable action event. Invoked when inspector is modified or context menu item is
		/// selected (e.g., Revert Value to Prefab, Set to Value of).
		/// </summary>
		/// <param name="modifications">Information about what properties were modified by the action.</param>
		private static UndoPropertyModification[] OnPerformUndoableAction(UndoPropertyModification[] modifications)
		{
			HashSet<HashableSerializedProperty> propertyModifications = new HashSet<HashableSerializedProperty>(
				(from mod in modifications select new HashableSerializedProperty(mod.propertyModification)).Where(
					mod => mod.SerializedProperty != null
				)
			);
			// add all upstream properties so parent setters are called
			foreach (HashableSerializedProperty modification in propertyModifications.ToArray())
			{
				foreach (HashableSerializedProperty upstreamProperty in GetUpstreamProperties(modification))
				{
					propertyModifications.Add(upstreamProperty);
				}
			}
			// trigger setters for all modified properties
			TriggerSettersForKnownModifications(propertyModifications);
			return modifications;
		}

		/// <summary>
		/// Raises the undo/redo event. This event contains no information about what was undone, so each hashed
		/// property must test its current value against its cached value.
		/// </summary>
		private static void OnUndoRedo()
		{
			// trigger setters for all modified properties
			TriggerSettersForKnownModifications(
				propertySetterCallbacks.Keys.Where(
					p => p.SerializedProperty != null && !p.SerializedProperty.IsValueEqualTo(valueCache[p])
				)
			);
		}
		
		/// <summary>
		/// Raises the trigger property setter event.
		/// </summary>
		/// <param name="hashableProperty">Hashable property.</param>
		/// <param name="getter">Getter.</param>
		/// <param name="setter">Setter.</param>
		/// <param name="propertyType">Property type.</param>
		/// <param name="oldValue">Old value.</param>
		private static void OnTriggerPropertySetter(
			HashableSerializedProperty hashableProperty,
			System.Func<object, object> getter,
			System.Action<object, object> setter,
			System.Type propertyType
		)
		{
			// clean up lookup tables and early out if property is dead
			SerializedProperty sp = hashableProperty.SerializedProperty;
			if (sp == null)
			{
				propertySetterCallbacks.Remove(hashableProperty);
				if (valueCache.ContainsKey(hashableProperty))
				{
					valueCache.Remove(hashableProperty);
				}
				return;
			}
			// invoke the setter
			InvokeSetter(hashableProperty, getter, setter, propertyType);
			// when any part of an array changes, ensure array, elements, and size are all up to date
			bool doElementsNeedUpdate = false;
			bool doesArrayNeedUpdate = false;
			bool doesSizeNeedUpdate = false;
			if (sp.isArray && sp.propertyType != SerializedPropertyType.String)
			{
				doElementsNeedUpdate = true;
				doesSizeNeedUpdate = true;
			}
			else if (sp.IsArrayElement())
			{
				doesArrayNeedUpdate = true;
				doesSizeNeedUpdate = true;
				sp = sp.GetParentProperty();
			}
			else if (sp.IsArraySize())
			{
				doesArrayNeedUpdate = true;
				doElementsNeedUpdate = true;
				sp = sp.GetParentProperty();
			}
			if (doElementsNeedUpdate)
			{
				for (int elementIndex = 0; elementIndex < sp.arraySize; ++elementIndex)
				{
					HashableSerializedProperty hashableElement = new HashableSerializedProperty(
						string.Format("{0}.Array.data[{1}]", sp.propertyPath, elementIndex),
						hashableProperty.TargetObject
					);
					if (valueCache.ContainsKey(hashableElement))
					{
						valueCache[hashableElement] = hashableElement.SerializedProperty.GetValue();
					}
					else // for added elements when list grows
					{
						valueCache.Add(hashableElement, hashableElement.SerializedProperty.GetValue());
					}
				}
			}
			if (doesArrayNeedUpdate)
			{
				HashableSerializedProperty hashableArray =
					new HashableSerializedProperty(sp.propertyPath, hashableProperty.TargetObject);
				valueCache[hashableArray] = hashableArray.SerializedProperty.GetValue();
			}
			if (doesSizeNeedUpdate)
			{
				HashableSerializedProperty hashableSize = new HashableSerializedProperty(
					string.Format("{0}.Array.size", sp.propertyPath), hashableProperty.TargetObject
				);
				valueCache[hashableSize] = hashableSize.SerializedProperty.GetValue();
			}
		}

		/// <summary>
		/// Registers the array property and its size property.
		/// </summary>
		/// <param name="arrayProperty">Array property.</param>
		/// <param name="getter">Getter.</param>
		/// <param name="setter">Setter.</param>
		/// <param name="propertyType">Property type.</param>
		private static void RegisterArrayProperty(
			SerializedProperty arrayProperty,
			System.Func<object, object> getter,
			System.Action<object, object> setter,
			System.Type propertyType
		)
		{
			HashableSerializedProperty hashableArrayProperty =
				new HashableSerializedProperty(arrayProperty.propertyPath, arrayProperty.serializedObject.targetObject);
			HashableSerializedProperty hashableArraySizeProperty = new HashableSerializedProperty(
				arrayProperty.propertyPath + ".Array.size", arrayProperty.serializedObject.targetObject
			);
			RegisterPropertyIfNeeded(hashableArrayProperty, hashableArrayProperty, getter, setter, propertyType);
			RegisterPropertyIfNeeded(hashableArraySizeProperty, hashableArrayProperty, getter, setter, propertyType);
			FieldInfo field;
			arrayProperty.GetProvider(out field);
			valueCache[hashableArrayProperty] = field.FieldType.IsArray ?
				System.Array.CreateInstance(field.FieldType.GetElementType(), 0) :
				System.Activator.CreateInstance(field.FieldType);
			valueCache[hashableArraySizeProperty] = 0;
		}

		/// <summary>
		/// Registers the supplied hashable property's initial backing field value and setter callback.
		/// </summary>
		/// <param name="trigger">Hashable property whose value changes will trigger the callback.</param>
		/// <param name="propertyToInvoke">
		/// Hashable property whose setter to invoke. Differs from trigger only when trigger is an array size property
		/// that triggers the setter in the array property.
		/// </param>
		/// <param name="getter">Getter.</param>
		/// <param name="setter">Setter.</param>
		/// <param name="propertyType">Property type.</param>
		private static void RegisterPropertyIfNeeded(
			HashableSerializedProperty trigger,
			HashableSerializedProperty propertyToInvoke,
			System.Func<object, object> getter,
			System.Action<object, object> setter,
			System.Type propertyType
		)
		{
			// initialize value cache
			if (!valueCache.ContainsKey(trigger))
			{
				valueCache.Add(trigger, trigger.SerializedProperty.GetValue());
				// if it is an array element, ensure the array and the size properties are registered
				SerializedProperty sp = trigger.SerializedProperty;
				if (sp.IsArrayElement())
				{
					RegisterArrayProperty(sp.GetParentProperty(), getter, setter, propertyType);
				}
			}
			if (!valueCache.ContainsKey(propertyToInvoke))
			{
				valueCache.Add(propertyToInvoke, propertyToInvoke.SerializedProperty.GetValue());
			}
			// add callbacks associated with the trigger
			if (!propertySetterCallbacks.ContainsKey(trigger))
			{
				propertySetterCallbacks.Add(
					trigger, () => OnTriggerPropertySetter(propertyToInvoke, getter, setter, propertyType)
				);
			}
		}

		/// <summary>
		/// Triggers the setter when the supplied property is known to have been modified. Properties are triggered
		/// according to depth, so that all children properties (e.g., elements, members) are triggered before their
		/// parents (e.g., arrays, classes, structs).
		/// </summary>
		/// <param name="modifiedProperties">Properties known to be modified.</param>
		private static void TriggerSettersForKnownModifications(
			IEnumerable<HashableSerializedProperty> modifiedProperties
		)
		{
			HashSet<HashableSerializedProperty> modifications =
				new HashSet<HashableSerializedProperty>(modifiedProperties);
			// sort by depth, so children setters are invoked first
			List<HashableSerializedProperty> sorted = modifications.ToList();
			sorted.Sort((a, b) => a.SerializedProperty.depth.CompareTo(b.SerializedProperty.depth));
			sorted.Reverse();
			// invoke setters
			foreach (HashableSerializedProperty hashableProperty in sorted)
			{
				if (propertySetterCallbacks.ContainsKey(hashableProperty))
				{
					propertySetterCallbacks[hashableProperty]();
				}
			}
		}
		
		#region Backing Fields
		private PropertyDrawer m_DrawerToUse = null;
		#endregion
		
		/// <summary>
		/// Gets the drawer to use.
		/// </summary>
		/// <value>
		/// The drawer to use if an override is specified, otherwise <c>null</c> if default drawer should be used.
		/// </value>
		private PropertyDrawer DrawerToUse
		{
			get
			{
				if (m_DrawerToUse == null)
				{
					m_DrawerToUse = fieldInfo.GetPropertyDrawer(Attribute.OverrideAttribute);
				}
				return m_DrawerToUse;
			}
		}
		
		/// <summary>
		/// Gets the backing field attribute.
		/// </summary>
		/// <value>The backing field attribute.</value>
		private PropertyBackingFieldAttribute Attribute { get { return attribute as PropertyBackingFieldAttribute; } }
		
		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <returns>The property height.</returns>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (DrawerToUse != null)
			{
				return DrawerToUse.GetPropertyHeight(property, label);
			}
			else
			{
				return property.GetDrawerHeight(label);
			}
		}
		
		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// TODO: this can be removed when Unity fixes bug 601339
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				return;
			}
			// clear all callbacks and cached values if the selection has changed
			if (!currentSelection.SetEquals(Selection.objects))
			{
				propertySetterCallbacks.Clear();
				valueCache.Clear();
				currentSelection = new HashSet<Object>(Selection.objects);
			}
			// ensure all properties are registered
			foreach (Object target in property.serializedObject.targetObjects)
			{
				HashableSerializedProperty hashableProperty =
					new HashableSerializedProperty(property.propertyPath, target);
				// register property if needed
				RegisterPropertyIfNeeded(
					hashableProperty, hashableProperty, Attribute.Getter, Attribute.Setter, Attribute.PropertyType
				);
			}
			// display field
			EditorGUI.BeginDisabledGroup(Attribute.Setter == null);
			{
				if (DrawerToUse == null)
				{
					EditorGUI.PropertyField(position, property, true);
				}
				else
				{
					DrawerToUse.OnGUI(position, property, label);
				}
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}