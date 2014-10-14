// 
// EditorPreferenceMenu.cs
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
// This file contains a class for the Candlelight editor preferences menu.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Candlelight
{
	/// <summary>
	/// Editor preference menu.
	/// </summary>
	public class EditorPreferenceMenu : Singleton<EditorPreferenceMenu>
	{
		/// <summary>
		/// The bug report email address.
		/// </summary>
		private const string bugReportEmailAddress = "bugs@adammechtley.com";
		/// <summary>
		/// The feature groups.
		/// </summary>
		private static List<string> featureGroups = new List<string>();
		/// <summary>
		/// The menu items.
		/// </summary>
		protected static Dictionary<string, List<System.Action>> menuItems = new Dictionary<string, List<System.Action>>();

		#region Backing Fields
		private static GUIStyle m_TabAreaStyle = null;
		#endregion
		/// <summary>
		/// Gets the tab area style.
		/// </summary>
		/// <value>The tab area style.</value>
		private static GUIStyle TabAreaStyle
		{
			get
			{
				if (m_TabAreaStyle == null)
				{
					m_TabAreaStyle = new GUIStyle();
					m_TabAreaStyle.padding = new RectOffset(3, 3, 0, 0); // otherwise tabs spill over edges of box
				}
				return m_TabAreaStyle;
			}
		}

		/// <summary>
		/// The current tab.
		/// </summary>
		[SerializeField]
		private int currentTab;
		/// <summary>
		/// The scroll position.
		/// </summary>
		[SerializeField]
		private Vector2 scrollPosition;
		
		/// <summary>
		/// Adds the preference menu item.
		/// </summary>
		/// <param name='preferenceMenu'>Preference menu.</param>
		/// <param name='method'>Method.</param>
		public static void AddPreferenceMenuItem(string featureGroup, System.Action method)
		{
			if (!menuItems.ContainsKey(featureGroup))
			{
				menuItems.Add(featureGroup, new List<System.Action>());
			}
			menuItems[featureGroup].Add(method);
			featureGroups.Clear();
			featureGroups.AddRange(menuItems.Keys);
			featureGroups.Sort();
		}

		/// <summary>
		/// Displays the preference GUI.
		/// </summary>
		[PreferenceItem("Candlelight")]
		public static void DisplayPreferenceGUI()
		{
			Dictionary<int, System.Action> tabPages = new Dictionary<int, System.Action>();
			for (int i=0; i<featureGroups.Count; ++i)
			{
				tabPages.Add(i, () => Instance.DisplayPreferences(featureGroups[Instance.currentTab]));
			}
			GUILayout.BeginArea(new Rect(134f, 39f, 352f, 352f)); // the rect in the preference window is bizarre...
			{
#if IS_CANDLELIGHT_SCENE_GUI_AVAILABLE
				EditorGUIX.DisplaySceneGUIToggle();
#endif
				EditorGUILayout.BeginVertical(TabAreaStyle, GUILayout.ExpandWidth(false));
				{
					Instance.currentTab =
						DisplayTabGroup(Instance.currentTab, featureGroups.ToArray(), tabPages);
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}
		
		/// <summary>
		/// Displays the bug report button.
		/// </summary>
		/// <param name='feature'>Feature.</param>
		/// <param name='emailAddress'>Email address to which bug report should be sent.</param>
		private static void DisplayBugReportButton(string feature, string emailAddress = bugReportEmailAddress)
		{
			// NOTE: button copied from EditorGUIX to reduce interdependencies
			Rect position = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight + 4);
			position = EditorGUI.IndentedRect(position);
			position.height -= 4;
			if (GUI.Button(position, string.Format("Report a Problem With {0} Tool", feature)))
			{
				try
				{
					System.Diagnostics.Process.Start(
						string.Format(
							"mailto:{0}?subject={1} Bug Report&body=1) What happened?\n\n2) How often does it " +
							"happen?\n\n3) How can I reproduce it using the example you attached?",
							bugReportEmailAddress, feature
						)
					);
				}
				catch
				{
					EditorUtility.DisplayDialog(
						"Error Creating Bug Report",
						"Please ensure an application is associated with email links.",
						"OK"
					);
				}
			}
		}
		
		/// <summary>
		/// Displays the preferences.
		/// </summary>
		private void DisplayPreferences(string featureName)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			{
				menuItems[featureName].Sort(
					(a, b) => string.Format(
						"{0}.{1}", a.Method.DeclaringType, a.Method.Name
					).CompareTo(
						string.Format("{0}.{1}", b.Method.DeclaringType, b.Method.Name)
					)
				);
				foreach (System.Action method in menuItems[featureName])
				{
					EditorGUILayout.LabelField(method.Method.DeclaringType.Name.ToWords(), EditorStyles.boldLabel);
					GUILayout.Box(GUIContent.none, GUILayout.Height(2f), GUILayout.ExpandWidth(true));
					EditorGUI.indentLevel += 1;
					method.Invoke();
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUILayout.EndScrollView();
			DisplayBugReportButton(featureName);
		}

		/// <summary>
		/// Displays the tab group.
		/// </summary>
		/// <remarks>
		/// This method is essentially copied from EditorGUIX to reduce interdependencies.
		/// </remarks>
		/// <returns>The tab group.</returns>
		/// <param name="currentTab">Current tab.</param>
		/// <param name="tabs">Tabs.</param>
		/// <param name="tabContents">Tab contents.</param>
		private static int DisplayTabGroup(int currentTab, string[] tabs, Dictionary<int, System.Action> tabContents)
		{
			currentTab = GUILayout.SelectionGrid(currentTab, tabs, tabs.Length, EditorStylesX.BrightTab);
			EditorGUILayout.BeginVertical(EditorStylesX.TabAreaBackground);
			{
				EditorGUILayout.Separator();
				try
				{
					tabContents[currentTab].Invoke();
				}
				catch (KeyNotFoundException)
				{
					throw new KeyNotFoundException(string.Format("No draw method supplied for tab {0}", currentTab));
				}
			}
			EditorGUILayout.EndVertical();
			return currentTab;
		}
	}
}