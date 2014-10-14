// 
// BackingFieldCompatibleObject.cs
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
// This file contains a base class for custom serializable objects that need
// to be compatible with Candlelight.PropertyBackingFieldAttribute. You can
// implement Candlelight.IPropertyBackingFieldCompatible on your own objects
// if you do not wish to inherit from this one. It is merely provided as a
// convenience.

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// Base class for a backing field compatible object. Make sure your subclass adds System.Serializable.
	/// </summary>
	public abstract class BackingFieldCompatibleObject : System.Object, IPropertyBackingFieldCompatible
	{
		/// <summary>
		/// Clone this instance.
		/// </summary>
		public abstract object Clone();
		/// <summary>
		/// Gets a hash value that is based on the values of the serialized properties of this instance.
		/// </summary>
		/// <remarks>
		/// Note that any reference type fields should implement and test with this interface; IList fields should
		/// generate a value-based hash.
		/// </remarks>
		/// <returns>A hash value based on the values of the serialized properties on this instance.</returns>
		public abstract int GetSerializedPropertiesHash();
	}
}