﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CommonUtils.Editor.AssetCleaner
{
	public class ShaderReferenceCollection
	{
		// shader name / shader file guid
		public Dictionary<string, string> shaderFileList = new();
		public Dictionary<string, List<string> > shaderReferenceList = new();

		public void Collection() {
			shaderFileList = new Dictionary<string, string>();
			shaderReferenceList = new Dictionary<string, List<string>>();
			collectionShaderFiles();
			checkReference();
		}

		private void collectionShaderFiles ()
		{
			var shaderFiles = Directory.GetFiles ("Assets", "*.shader", SearchOption.AllDirectories);
			foreach (var shaderFilePath in shaderFiles) {
				var code = File.ReadAllText (shaderFilePath);
				var match = Regex.Match (code, "Shader \"(?<name>.*)\"");
				if (match.Success) {
					var shaderName = match.Groups ["name"].ToString ();
					if (shaderFileList.ContainsKey (shaderName) == false) {
						shaderFileList.Add (shaderName, AssetDatabase.AssetPathToGUID(shaderFilePath));
					}
				}
			}

			var cgFiles = Directory.GetFiles ("Assets", "*.cg", SearchOption.AllDirectories);
			foreach (var cgFilePath in cgFiles) {
				var file = Path.GetFileName (cgFilePath);
				shaderFileList.Add (file, cgFilePath);
			}

			var cgincFiles = Directory.GetFiles ("Assets", "*.cginc", SearchOption.AllDirectories);
			foreach (var cgincPath in cgincFiles) {
				var file = Path.GetFileName (cgincPath);
				shaderFileList.Add (file, cgincPath);
			}
		}

		private void checkReference() {
			foreach (var shader in shaderFileList) {
				var shaderFilePath = AssetDatabase.GUIDToAssetPath(shader.Value);
				if (string.IsNullOrWhiteSpace(shaderFilePath)) {
					Debug.LogWarning($"Null or empty file path for shader {shader.Key}");
					continue;
				}

				var shaderName = shader.Key;

				var referenceList = new List<string>();
				shaderReferenceList.Add(shaderName, referenceList);

				var code = File.ReadAllText(shaderFilePath);

				foreach (var checkingShaderName in shaderFileList.Keys) {
					if (Regex.IsMatch(code, string.Format("{0}", checkingShaderName))) {
						var filePath = shaderFileList[checkingShaderName];
						referenceList.Add(filePath);
					}
				}
			}
		}
	}
}