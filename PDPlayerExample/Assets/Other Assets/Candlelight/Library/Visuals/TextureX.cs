// 
// TextureX.cs
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
// This file contains a class with extension methods for Texture and subclasses.

using UnityEngine;

namespace Candlelight
{
	/// <summary>
	/// Extension methods for Texture and subclasses.
	/// </summary>
	public static class TextureX : System.Object
	{
		/// <summary>
		/// A temporary render texture allocation.
		/// </summary>
		private static RenderTexture tempRenderTex;

		/// <summary>
		/// Gets a readable copy of the source texture.
		/// </summary>
		/// <returns>A readable copy of the source texture.</returns>
		/// <param name="source">Source.</param>
		public static Texture2D GetReadableCopy(this Texture2D source)
		{
			Texture2D result = new Texture2D(source.width, source.height, source.format, false);
			result.hideFlags = HideFlags.DontSave;
#if UNITY_PRO_LICENSE
			tempRenderTex = RenderTexture.GetTemporary(source.width, source.height);
			RenderTexture.active = tempRenderTex;
			Graphics.Blit(source, tempRenderTex);
			result.ReadPixels(new Rect(0f, 0f, result.width, result.height), 0 , 0);
			RenderTexture.ReleaseTemporary(tempRenderTex);
			RenderTexture.active = null;
#else
			try
			{
				result.SetPixels32(source.GetPixels32());
			}
			catch (UnityException)
			{
				result = null;
			}
#endif
			return result;
		}

		/// <summary>
		/// Flood fill the specified block on the specified texture using the specified color.
		/// </summary>
		/// <param name="tex">Tex.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="col">Col.</param>
		/// <param name="apply">If set to <c>true</c> apply.</param>
		public static void FloodFill(
			this Texture2D tex, int x, int y, int width, int height, Color col, bool apply=true
		)
		{
			int size = width*height;
			Color[] colors = new Color[size];
			for (int i=0; i<size; ++i)
			{
				colors[i] = col;
			}
			try
			{
				tex.SetPixels(x, y, width, height, colors);
				if (apply)
				{
					tex.Apply();
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
		}
	}
}