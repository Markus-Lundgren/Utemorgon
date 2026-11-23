using System;
using RiverScenes;
using UtemorgonPOS.Core;

namespace UtemorgonPOS.Terminal;

class Program
{
	static void Main(string[] _)
	{

		Console.Clear();
		SceneManager sm = new();

		sm.Initialize(SceneRepository.Main);

		Console.WriteLine("End of main execution (success).");
	}
}