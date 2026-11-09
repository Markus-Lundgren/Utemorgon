using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace UtemorgonPOS.Core;

public static class EasyEnv
{
	static Dictionary<string, string>? _envs = null;
	public static Dictionary<string, string> Variables => _envs ??= InitializeEnv();

	/// <summary>Fetches an env variable as a certain type (as long as that type is IParseable).</summary>
	/// <remarks>Note: This code is not robust and breaks easily. Its only purpose is to get the debug environment working.</remarks>
	public static T Get<T>(string variableName) where T : IParsable<T>
		=> T.Parse(Variables[variableName], CultureInfo.InvariantCulture);

	// Note: This is not a safe method, or even class.
	// It's a quick and dirty way to just get this working for the debug environment, it's not critical code.
	static Dictionary<string, string> InitializeEnv()
	{
		Dictionary<string, string> kvps = new(StringComparer.InvariantCultureIgnoreCase);
		var lines = File.ReadAllLines(GetEnvFilePath());

		foreach (var l in lines)
		{
			if (string.IsNullOrWhiteSpace(l) || l.StartsWith('#'))
				continue;

			var sl = l.Split('=', 2);

			if (sl.Length < 2)
				throw new TypeLoadException("Not all entries in the .env file contains a value!");

			kvps.Add(sl[0].Trim(), sl[1].Trim());
		}

		return kvps;
	}

	static string GetEnvFilePath()
	{
		// Traverse up from the current directory until an .env file is found.
		DirectoryInfo? currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
		while (currentDirectory != null && !File.Exists(Path.Combine(currentDirectory.FullName, ".env")))
			currentDirectory = currentDirectory.Parent;

		// If we never hit the .env file, throw the program.
		if (currentDirectory == null)
			throw new FileNotFoundException("Could not find a .env file in any parent directory.");

		// If we hit it, return the path
		return Path.Combine(currentDirectory.FullName, ".env");
	}
}