using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
namespace FlightRecorderPlugin
{
	[CustomEditor(typeof(FlightRecorderPlayback), true)]
	public class FlightRecorderPlaybackEditor : Editor
	{

		FlightRecorderPlayback myScript;
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			myScript = target as FlightRecorderPlayback;

			if(myScript.Playbacking == false)
			{
				if (GUILayout.Button("Load flight"))
				{
					string flightPath = GetRecordedFilePath();
					flightPath = flightPath.Replace('/', '\\');
					myScript.LoadFlight(flightPath);
				}
			}
			else
			{
				if (GUILayout.Button("Unload flight"))
				{
					myScript.UnloadFlight();
				}
			}		
		}

		/// <summary>
		/// Gets the path for recorded flight
		/// </summary>
		string GetRecordedFilePath()
		{
			string[] filesInFolder = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
			string path = EditorUtility.OpenFilePanel("Select recorded flight", filesInFolder[0], "txt");
			return path;
		}	

	}
}