// 
// StringX.cs
// 
// Author:
//       Adam Mechtley <adam@adammechtley.com>
//       http://adammechtley.com/donations
// 
// Copyright (c) 2011-2014, Adam Mechtley
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
// This file contains a static class with string-related extension methods.

using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Candlelight
{
	/// <summary>
	/// String-related extension methods.
	/// </summary>
	public static class StringX : System.Object
	{
		/// <summary>
		/// A reusable char list allocation.
		/// </summary>
		private static List<char> charList = new List<char>();
		/// <summary>
		/// The title case text info.
		/// </summary>
		private static TextInfo titleCaseTextInfo = new CultureInfo("en-US", false).TextInfo;

		/// <summary>
		/// Return a string which is the concatenation of the strings in the collection. The separator between elements
		/// is the string providing this method. Mimics str.join(iterable) in Python.
		/// </summary>
		/// <param name="separator">Separator.</param>
		/// <param name="collection">Collection.</param>
		public static string Join(this string separator, System.Collections.Generic.IEnumerable<string> collection)
		{
			return string.Join(separator, collection.ToArray());
		}

		/// <summary>
		/// Gets a range of the supplied string using the specified start index, end index, and skip. Mimics index
		/// operator in Python.
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="start">Start index.</param>
		/// <param name="end">End index.</param>
		/// <param name="skip">Number of indices to skip.</param>
		public static string Range(this string str, int start, int end = -1, int skip = 1)
		{
			end = end >= 0 ? Mathf.Min(end, str.Length) : str.Length + end;
			charList.Clear();
			for (int index = start; index < end; index += skip)
			{
				charList.Add(str[index]);
			}
			return new string(charList.ToArray());
		}

		/// <summary>
		/// Converts the camel/titlecase string to words.
		/// </summary>
		/// <returns>The supplied camel/titlecase string broken into individual, capitalized words.</returns>
		/// <param name='camelCase'>A string in camelCase or TitleCase.</param>
		public static string ToWords(this string titleCase)
		{
			string ret = Regex.Replace(titleCase, "(\\B[A-Z])", " $1");
			return string.Format("{0}{1}", ret[0].ToString().ToUpper(), ret.Substring(1));
		}
		
		/// <summary>
		/// Converts float meters value to feet and inches string.
		/// </summary>
		/// <returns>A feet/inches string representation of the supplied meters value.</returns>
		/// <param name='meters'>Meters.</param>
		public static string ToFeetInchesString(this float meters)
		{
			float inches = meters * 39.37007874015748f; // 100f / 2.54f
			int feet = (int)(inches * 0.083333333333333f); // 1f / 12f
			inches = inches - feet * 12;
			return string.Format("{0}' {1:0}\"", feet, inches);
		}
		
		/// <summary>
		/// Converts float to time string.
		/// </summary>
		/// <returns>The time string representation of the supplied time.</returns>
		/// <param name='time'>Time in seconds.</param>
		public static string ToTimeString(this float time)
		{
			return string.Format("{0}:{1:00.00}", (int)(time % 3600)/60, time % 60);
		}

		/// <summary>
		/// Converts the words to title case words
		/// </summary>
		/// <returns>The supplied words converted to title case.</returns>
		/// <param name="words">A string containing one or more words.</param>
		public static string ToTitleCase(this string words)
		{
			return titleCaseTextInfo.ToTitleCase(words);
		}
	}
}