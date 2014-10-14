// 
// EditorStylesX.cs
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
// This file contains an extension class for accessing built-in editor styles.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Candlelight
{
	/// <summary>
	/// Editor styles extensions.
	/// </summary>
	public static class EditorStylesX : System.Object
	{
		#region Styles
		#region Backing Fields
		private static GUIStyle m_BoldFoldout;
		private static GUIStyle m_BoldLabel;
		private static GUIStyle m_BoldTitleBar;
		private static GUIStyle m_Box;
		private static GUIStyle m_BrightTab;
		private static GUIStyle m_EmptyInspectorArea;
		private static GUIStyle m_EmptySceneBox;
		private static GUIStyle m_EvenBackground;
		private static GUIStyle m_Foldout;
		private static GUIStyle m_Label;
		private static GUIStyle m_LabelRight;
		private static GUIStyle m_ListItem;
		private static GUIStyle m_MiniButtonLeft;
		private static GUIStyle m_MiniButtonRight;
		private static GUIStyle m_DarkTab;
		private static GUIStyle m_OddBackground;
		private static GUIStyle m_PadlockToggle;
		private static GUIStyle m_PropertyFieldHorizontalLayoutBlock;
		private static GUIStyle m_PropertyFieldHorizontalLayoutBlockDefault;
		private static GUIStyle m_SceneBox;
		private static GUIStyle m_SceneGUIInspectorBackground;
		private static GUIStyle m_SceneNotification;
		private static GUIStyle m_SelectedListItem;
		private static GUIStyle m_StatusIconStyle;
		private static GUIStyle m_TabBackground;
		#endregion

		/// <summary>
		/// Gets the bold foldout.
		/// </summary>
		/// <value>The bold foldout.</value>
		public static GUIStyle BoldFoldout
		{
			get
			{
				if (m_BoldFoldout == null)
				{
					try
					{
						m_BoldFoldout = new GUIStyle(Foldout);
						m_BoldFoldout.fontStyle = FontStyle.Bold;
					}
					catch { m_BoldFoldout = GetErrorGUIStyle(); }
				}
				return m_BoldFoldout;
			}
		}

		/// <summary>
		/// Gets the bold label.
		/// </summary>
		/// <value>The bold label.</value>
		public static GUIStyle BoldLabel
		{
			get
			{
				if (m_BoldLabel == null)
				{
					// NOTE: EditorStyles.boldLabel cannot be called in ctor, but this approach can be
					try { m_BoldLabel = GUI.skin.GetStyle("BoldLabel"); }
					catch { m_BoldLabel = GetErrorGUIStyle(); }
				}
				return m_BoldLabel;
			}
		}

		/// <summary>
		/// Gets the bold title bar.
		/// </summary>
		/// <value>The bold title bar.</value>
		public static GUIStyle BoldTitleBar
		{
			get
			{
				if (m_BoldTitleBar == null)
				{
					try { m_BoldTitleBar = GUI.skin.GetStyle("OL Title"); }
					catch { m_BoldTitleBar = GetErrorGUIStyle(); }
				}
				return m_BoldTitleBar;
			}
		}

		/// <summary>
		/// Gets the box.
		/// </summary>
		/// <value>The box.</value>
		public static GUIStyle Box
		{
			get
			{
				if (m_Box == null)
				{
					try { m_Box = new GUIStyle(GUI.skin.GetStyle("Box")); }
					catch { m_Box = GetErrorGUIStyle(); }
				}
				return m_Box;
			}
		}

		/// <summary>
		/// Gets the bright tab.
		/// </summary>
		/// <value>The bright tab.</value>
		public static GUIStyle BrightTab
		{
			get
			{
				if (m_BrightTab == null)
				{
					try
					{
						m_BrightTab = new GUIStyle(GUI.skin.GetStyle("dragtab"));
						m_BrightTab.normal.background = m_BrightTab.onNormal.background;
						if (EditorGUIUtility.isProSkin)
						{
							// create brighter texture for on normal
							Texture2D tx = m_BrightTab.onNormal.background.GetReadableCopy();
							if (tx != null)
							{
								Color[] pixels = tx.GetPixels();
								for (int i=0; i<pixels.Length; ++i)
								{
									float a = pixels[i].a;
									pixels[i] = pixels[i] * 1.2f;
									pixels[i].a = a;
								}
								tx.SetPixels(pixels);
								tx.Apply();
								m_BrightTab.onNormal.background = tx;
							}
							else
							{
								m_BrightTab.normal.textColor =
									m_BrightTab.normal.textColor * new Color(1f, 1f, 1f, 0.5f);
								m_BrightTab.onNormal.background =
									GUI.skin.GetStyle("dragtabbright").onNormal.background;
							}
						}
						else
						{
							m_BrightTab.normal.textColor = m_BrightTab.normal.textColor * new Color(1f, 1f, 1f, 0.5f);
							m_BrightTab.onNormal.background = GUI.skin.GetStyle("dragtabbright").onNormal.background;
						}
						m_BrightTab.margin = new RectOffset(0, 0, 2, 0);
						m_BrightTab.clipping = TextClipping.Clip;
					}
					catch { m_BrightTab = GetErrorGUIStyle(); }
				}
				return m_BrightTab;
			}
		}
		
		/// <summary>
		/// Gets the dark tab style.
		/// </summary>
		/// <value>The dark tab style.</value>
		public static GUIStyle DarkTab
		{
			get
			{
				if (m_DarkTab == null)
				{
					try
					{
						if (EditorGUIUtility.isProSkin)
						{
							m_DarkTab = new GUIStyle(GUI.skin.GetStyle("dragtab"));
							Texture2D tx = m_DarkTab.onNormal.background.GetReadableCopy();
							if (tx != null)
							{
								Color[] pixels = tx.GetPixels();
								for (int i=0; i<pixels.Length; ++i)
								{
									float a = pixels[i].a;
									pixels[i] = pixels[i] * 0.85f;
									pixels[i].a = a;
								}
								tx.SetPixels(pixels);
								tx.Apply();
								m_DarkTab.normal.background = tx;
							}
							else
							{
								m_DarkTab = new GUIStyle(BrightTab);
							}
						}
						else
						{
							m_DarkTab = new GUIStyle(BrightTab);
						}
						m_DarkTab.margin = new RectOffset(0, 0, 2, 0);
						m_DarkTab.clipping = TextClipping.Clip;
					}
					catch { m_DarkTab = GetErrorGUIStyle(); }
				}
				return m_DarkTab;
			}
		}

		/// <summary>
		/// Gets the empty inspector area.
		/// </summary>
		/// <remarks>
		/// This style is necessary at the top level of the inspector GUI to get a PropertyField to line up.
		/// </remarks>
		/// <value>The empty inspector area.</value>
		public static GUIStyle EmptyInspectorArea
		{
			get
			{
				if (m_EmptyInspectorArea == null)
				{
					m_EmptyInspectorArea = new GUIStyle();
					m_EmptyInspectorArea.padding = Box.padding;
				}
				return m_EmptyInspectorArea;
			}
		}

		/// <summary>
		/// Gets the empty scene box.
		/// </summary>
		/// <value>The empty scene box.</value>
		public static GUIStyle EmptySceneBox
		{
			get
			{
				if (m_EmptySceneBox == null)
				{
					m_EmptySceneBox = new GUIStyle(SceneBox);
					m_EmptySceneBox.normal.background = null;
				}
				return m_EmptySceneBox;
			}
		}

		/// <summary>
		/// Gets the even background.
		/// </summary>
		/// <value>The even background.</value>
		public static GUIStyle EvenBackground
		{
			get
			{
				if (m_EvenBackground == null)
				{
					m_EvenBackground = new GUIStyle();
					m_EvenBackground.normal.background = EvenBackgroundTexture;
				}
				return m_EvenBackground;
			}
		}

		/// <summary>
		/// Gets the foldout.
		/// </summary>
		/// <value>The foldout.</value>
		public static GUIStyle Foldout
		{
			get
			{
				if (m_Foldout == null)
				{
					try { m_Foldout = GUI.skin.GetStyle("Foldout"); }
					catch { m_Foldout = GetErrorGUIStyle(); }
				}
				return m_Foldout;
			}
		}

		/// <summary>
		/// Gets the label.
		/// </summary>
		/// <value>The label.</value>
		public static GUIStyle Label
		{
			get
			{
				if (m_Label == null)
				{
					// NOTE: EditorStyles.label cannot be called in ctor, but this approach can be
					try { m_Label = GUI.skin.GetStyle("Label"); }
					catch { m_Label = GetErrorGUIStyle(); }
				}
				return m_Label;
			}
		}

		/// <summary>
		/// Gets the right-aligned label.
		/// </summary>
		/// <value>The right-aligned label.</value>
		public static GUIStyle LabelRight
		{
			get
			{
				if (m_LabelRight == null)
				{
					try
					{
						m_LabelRight = new GUIStyle(Label);
						m_LabelRight.alignment = TextAnchor.UpperRight;
					}
					catch { m_LabelRight = GetErrorGUIStyle(); }
				}
				return m_LabelRight;
			}
		}

		/// <summary>
		/// Gets the list item.
		/// </summary>
		/// <value>The list item.</value>
		public static GUIStyle ListItem
		{
			get
			{
				if (m_ListItem == null)
				{
					m_ListItem = new GUIStyle(GUI.skin.GetStyle("IN SelectedLine"));
				}
				return m_ListItem;
			}
		}

		/// <summary>
		/// Gets the left mini button.
		/// </summary>
		/// <value>The left mini button.</value>
		public static GUIStyle MiniButtonLeft
		{
			get
			{
				if (m_MiniButtonLeft == null)
				{
					try { m_MiniButtonLeft = GUI.skin.GetStyle("minibuttonleft"); }
					catch { m_MiniButtonLeft = GetErrorGUIStyle(); }
				}
				return m_MiniButtonLeft;
			}
		}

		/// <summary>
		/// Gets the right mini button.
		/// </summary>
		/// <value>The right mini button.</value>
		public static GUIStyle MiniButtonRight
		{
			get
			{
				if (m_MiniButtonRight == null)
				{
					try { m_MiniButtonRight = GUI.skin.GetStyle("minibuttonright"); }
					catch { m_MiniButtonRight = GetErrorGUIStyle(); }
				}
				return m_MiniButtonRight;
			}
		}

		/// <summary>
		/// Gets the odd background.
		/// </summary>
		/// <value>The odd background.</value>
		public static GUIStyle OddBackground
		{
			get
			{
				if (m_OddBackground == null)
				{
					m_OddBackground = new GUIStyle();
					m_OddBackground.normal.background = OddBackgroundTexture;
				}
				return m_OddBackground;
			}
		}

		/// <summary>
		/// Gets the padlock toggle.
		/// </summary>
		/// <value>The padlock toggle.</value>
		public static GUIStyle PadlockToggle
		{
			get
			{
				if (m_PadlockToggle == null)
				{
					m_PadlockToggle = new GUIStyle();
					string suffix = EditorGUIUtility.isProSkin ? ".png" : "";
					m_PadlockToggle.active.background = GetBuiltinTexture("IN LockButton act" + suffix);
					m_PadlockToggle.onActive.background = GetBuiltinTexture("IN LockButton on act" + suffix);
					m_PadlockToggle.normal.background = GetBuiltinTexture("IN LockButton" + suffix);
					m_PadlockToggle.onNormal.background = GetBuiltinTexture("IN LockButton on" + suffix);
					int height = m_PadlockToggle.normal.background.height;
					int width = m_PadlockToggle.normal.background.width;
					m_PadlockToggle.border = new RectOffset(width, 0, height, 0);
					m_PadlockToggle.margin = new RectOffset(4, 4, 2, 2);
					m_PadlockToggle.padding = new RectOffset(width + 3, 3, 1, 2);
					m_PadlockToggle.fixedHeight = height;
					m_PadlockToggle.fixedWidth = width;
				}
				return m_PadlockToggle;
			}
		}

		/// <summary>
		/// Gets the property field horizontal layout block style.
		/// </summary>
		/// <remarks>
		/// Use this when laying out PropertyFields inside of HorizontalLayout blocks along with other GUI in order to
		/// prevent large margins.
		/// </remarks>
		/// <value>The property field horizontal layout block style.</value>
		public static GUIStyle PropertyFieldHorizontalLayoutBlock
		{
			get
			{
				if (m_PropertyFieldHorizontalLayoutBlock == null)
				{
					m_PropertyFieldHorizontalLayoutBlock = new GUIStyle();
					m_PropertyFieldHorizontalLayoutBlock.margin = new RectOffset(0, 0, -4, -4);
					m_PropertyFieldHorizontalLayoutBlock.padding = new RectOffset();
				}
				return m_PropertyFieldHorizontalLayoutBlock;
			}
		}

		/// <summary>
		/// Gets the property field horizontal layout block default.
		/// </summary>
		/// <remarks>
		/// Use this when laying out PropertyFields inside of HorizontalLayout
		/// blocks in order to prevent large margins.
		/// </remarks>
		/// <value>The property field horizontal layout block default.</value>
		public static GUIStyle PropertyFieldHorizontalLayoutBlockDefault
		{
			get
			{
				if (m_PropertyFieldHorizontalLayoutBlockDefault == null)
				{
					m_PropertyFieldHorizontalLayoutBlockDefault = new GUIStyle();
					m_PropertyFieldHorizontalLayoutBlockDefault.margin = new RectOffset(0, 0, -2, -2);
				}
				return m_PropertyFieldHorizontalLayoutBlockDefault;
			}
		}

		/// <summary>
		/// Gets the scene box.
		/// </summary>
		/// <value>The scene box.</value>
		public static GUIStyle SceneBox
		{
			get
			{
				if (m_SceneBox == null)
				{
					try { m_SceneBox = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game).GetStyle("Box")); }
					catch { m_SceneBox = GetErrorGUIStyle(); }
				}
				return m_SceneBox;
			}
		}

		/// <summary>
		/// Gets the scene GUI inspector background.
		/// </summary>
		/// <value>The scene GUI inspector background.</value>
		public static GUIStyle SceneGUIInspectorBackground
		{
			get
			{
				if (m_SceneGUIInspectorBackground == null)
				{
					try
					{
						m_SceneGUIInspectorBackground = new GUIStyle(GUI.skin.GetStyle("Box"));
						m_SceneGUIInspectorBackground.margin.left = 1;
						m_SceneGUIInspectorBackground.margin.right = 0;
					}
					catch { m_SceneGUIInspectorBackground = GetErrorGUIStyle(); }
				}
				return m_SceneGUIInspectorBackground;
			}
		}

		/// <summary>
		/// Gets the scene notification style.
		/// </summary>
		/// <value>The scene notification style.</value>
		public static GUIStyle SceneNotification
		{
			get
			{
				if (m_SceneNotification == null)
				{
					m_SceneNotification = new GUIStyle(GUI.skin.GetStyle("NotificationBackground"));
				}
				return m_SceneNotification;
			}
		}

		/// <summary>
		/// Gets the selected list item.
		/// </summary>
		/// <value>The selected list item.</value>
		public static GUIStyle SelectedListItem
		{
			get
			{
				if (m_SelectedListItem == null)
				{
					m_SelectedListItem = new GUIStyle(GUI.skin.GetStyle("IN SelectedLine"));
					m_SelectedListItem.normal.background = m_SelectedListItem.onNormal.background;
				}
				return m_SelectedListItem;
			}
		}

		/// <summary>
		/// Gets the status icon style.
		/// </summary>
		/// <remarks>
		/// Used to make boxes of a fixed size for status icons.
		/// </remarks>
		/// <value>The status icon style.</value>
		public static GUIStyle StatusIconStyle
		{
			get
			{
				if (m_StatusIconStyle == null)
				{
					m_StatusIconStyle = new GUIStyle();
					m_StatusIconStyle.normal.textColor = Label.normal.textColor;
					m_StatusIconStyle.alignment = TextAnchor.MiddleCenter;
					// account for empty space baked into icons
					m_StatusIconStyle.fixedHeight = 22f; // GUIHelpers.defaultLineHeight
					m_StatusIconStyle.fixedWidth = 22f; // GUIHelpers.defaultLineHeight;
					m_StatusIconStyle.contentOffset = -2f * Vector2.one;
				}
				return m_StatusIconStyle;
			}
		}

		/// <summary>
		/// Gets the tab background.
		/// </summary>
		/// <value>The tab background.</value>
		public static GUIStyle TabAreaBackground
		{
			get
			{
				if (m_TabBackground == null)
				{
					try
					{
						m_TabBackground = new GUIStyle(GUI.skin.GetStyle("Box"));
						m_TabBackground.margin.top = 0;
					}
					catch { m_TabBackground = GetErrorGUIStyle(); }
				}
				return m_TabBackground;
			}
		}
		
		/// <summary>
		/// Gets the error GUI style.
		/// </summary>
		/// <remarks>
		/// Used when loading a built-in style fails
		/// </remarks>
		/// <returns>The error GUI style.</returns>
		private static GUIStyle GetErrorGUIStyle()
		{
			GUIStyle s = new GUIStyle();
			s.normal.textColor = Color.magenta;
			return s;
		}
		#endregion

		#region Textures
		#region Backing Fields
		private static Texture2D m_ErrorIcon;
		private static Texture2D m_EvenBackgroundTexture;
		private static Texture2D m_InfoIcon;
		private static Texture2D m_LockedIcon;
		private static Texture2D m_OddBackgroundTexture;
		private static Texture2D m_OkayIcon;
		private static Texture2D m_UnlockedIcon;
		private static Texture2D m_WarningIcon;
		#endregion
		
		/// <summary>
		/// Gets the error icon.
		/// </summary>
		/// <value>The error icon.</value>
		public static Texture2D ErrorIcon
		{
			get
			{
				if (m_ErrorIcon == null)
				{
					m_ErrorIcon = EditorGUIUtility.FindTexture("console.erroricon");
				}
				return m_ErrorIcon;
			}
		}
		
		/// <summary>
		/// Gets the even background texture.
		/// </summary>
		/// <value>The even background texture.</value>
		public static Texture2D EvenBackgroundTexture
		{
			get
			{
				if (m_EvenBackgroundTexture == null)
				{
					try { m_EvenBackgroundTexture = GUI.skin.GetStyle("CN EntryBackEven").normal.background; }
					catch { m_EvenBackgroundTexture = EditorGUIUtility.whiteTexture; }
				}
				return m_EvenBackgroundTexture;
			}
		}
		
		/// <summary>
		/// Gets the info icon.
		/// </summary>
		/// <value>The info icon.</value>
		public static Texture2D InfoIcon
		{
			get
			{
				if (m_InfoIcon == null)
				{
					m_InfoIcon = EditorGUIUtility.FindTexture("console.infoicon");
				}
				return m_InfoIcon;
			}
		}
		
		/// <summary>
		/// Gets the locked icon.
		/// </summary>
		/// <value>The locked icon.</value>
		public static Texture2D LockedIcon
		{
			get
			{
				if (m_LockedIcon == null)
				{
					m_LockedIcon = GetBuiltinTexture("IN LockButton on");
				}
				return m_LockedIcon;
			}
		}
		
		/// <summary>
		/// Gets the odd background texture.
		/// </summary>
		/// <value>The odd background texture.</value>
		public static Texture2D OddBackgroundTexture
		{
			get
			{
				if (m_OddBackgroundTexture == null)
				{
					try { m_OddBackgroundTexture = GUI.skin.GetStyle("CN EntryBackOdd").normal.background; }
					catch { m_OddBackgroundTexture = EditorGUIUtility.whiteTexture; }
				}
				return m_OddBackgroundTexture;
			}
		}
		
		/// <summary>
		/// Gets the okay icon.
		/// </summary>
		/// <value>The okay icon.</value>
		public static Texture2D OkayIcon
		{
			get
			{
				if (m_OkayIcon == null)
				{
					try { m_OkayIcon = GUI.skin.GetStyle("MenuItem").onNormal.background; }
					catch { m_OkayIcon = EditorGUIUtility.whiteTexture; }
				}
				return m_OkayIcon;
			}
		}
		
		/// <summary>
		/// Gets the unlocked icon.
		/// </summary>
		/// <remarks>
		/// Light version.
		/// </remarks>
		/// <value>The unlocked icon.</value>
		public static Texture2D UnlockedIcon
		{
			get
			{
				if (m_UnlockedIcon == null)
				{
					m_UnlockedIcon = GetBuiltinTexture("IN LockButton");
				}
				return m_UnlockedIcon;
			}
		}
		
		/// <summary>
		/// Gets the warning icon.
		/// </summary>
		/// <value>The warning icon.</value>
		public static Texture2D WarningIcon
		{
			get
			{
				if (m_WarningIcon == null)
				{
					m_WarningIcon = EditorGUIUtility.FindTexture("console.warnicon");
				}
				return m_WarningIcon;
			}
		}
		
		/// <summary>
		/// Gets the builtin texture with the specified name.
		/// </summary>
		/// <returns>The builtin texture with the specified name.</returns>
		/// <param name="textureName">Texture name.</param>
		public static Texture2D GetBuiltinTexture(string textureName)
		{
			Texture2D[] result = GetBuiltinTextures(textureName);
			return result == null || result.Length == 0 ? null : result[0];
		}
		
		/// <summary>
		/// Gets the builtin textures.
		/// </summary>
		/// <returns>The builtin textures with the specified name.</returns>
		/// <param name="textureName">Texture name.</param>
		public static Texture2D[] GetBuiltinTextures(string textureName)
		{
			string withExt = textureName + ".png";
			return new List<Texture2D>(
				Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[]
			).FindAll(item => item.name == textureName || item.name == withExt).ToArray();
		}
		#endregion
	}
}