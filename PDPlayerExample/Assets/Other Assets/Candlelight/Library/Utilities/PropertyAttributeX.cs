// 
// PropertyAttributeX.cs
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
// This file contains a class with extension methods for PropertyAttribute.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Candlelight
{
	/// <summary>
	/// Property attribute extension methods.
	/// </summary>
	public static class PropertyAttributeX : System.Object
	{
		/// <summary>
		/// Creates a new property attribute instance.
		/// </summary>
		/// <returns>An unbound property attribute.</returns>
		/// <param name="propertyAttributeType">Property attribute type.</param>
		/// <param name="constructorArguments">Constructor arguments.</param>
		public static PropertyAttribute CreateNew(
			System.Type propertyAttributeType, params object[] constructorArguments
		)
		{
			// try to find the matching constructor
			ConstructorInfo constructor = null;
			System.Type[] suppliedArgTypes =
				(from param in constructorArguments select param == null ? typeof(object) : param.GetType()).ToArray();
			foreach (ConstructorInfo ctor in propertyAttributeType.GetConstructors())
			{
				ParameterInfo[] ctorParams = ctor.GetParameters();
				System.Type paramArrayType = (
					ctorParams.LastOrDefault() != null &&
					ctorParams.Last().GetCustomAttributes(typeof(System.ParamArrayAttribute), false).Length > 0
				) ? ctorParams.Last().ParameterType : null;
				constructor = ctor;
				// first check explicitly supplied arguments
				for (int i=0; i<suppliedArgTypes.Length; ++i)
				{
					// if there are more supplied arguments than parameter types, see if they match param type
					if (
						i >= ctorParams.Length &&
						(paramArrayType == null || !paramArrayType.IsAssignableFrom(suppliedArgTypes[i]))
					)
					{
						constructor = null;
						continue;
					}
					// if supplied argument type is mismatch, then constructor is a bad match
					if (
						!ctorParams[i].ParameterType.IsAssignableFrom(suppliedArgTypes[i]) ||
						(suppliedArgTypes[i] == null && !ctorParams[i].ParameterType.IsClass)
					)
					{
						constructor = null;
					}
				}
				// if candidate is a match, see if it has any further parameters
				if (constructor != null && ctorParams.Length > suppliedArgTypes.Length)
				{
					List<object> newArgs = new List<object>(constructorArguments);
					for (int i=suppliedArgTypes.Length; i<ctorParams.Length; ++i)
					{
						// candidate is a mistmatch if the argument is not optional, or is not the params argument
						if (!ctorParams[i].IsOptional && !(paramArrayType != null && i == ctorParams.Length - 1))
						{
							constructor = null;
						}
						else if (ctorParams[i].IsOptional)
						{
							newArgs.Add(ctorParams[i].DefaultValue);
						}
					}
					// if candidate is still a match, append missing arguments
					if (constructor != null)
					{
						constructorArguments = newArgs.ToArray();
					}
				}
				// break out if the candidate is still valid
				if (constructor != null)
				{
					break;
				}
			}
			if (constructor == null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				foreach (System.Type type in suppliedArgTypes)
				{
					sb.AppendFormat(", {0}", type);
				}
				throw new System.ArgumentException(
					"constructorArguments",
					string.Format(
						"No constructor found for {0} matching method signature [{1}].",
						propertyAttributeType, sb.Length > 0 ? sb.ToString().Substring(2) : ""
					)
				);
			}
			return constructor.Invoke(constructorArguments) as PropertyAttribute;
		}
	}
}