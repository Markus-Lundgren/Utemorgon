using System;
using RiverScenes;

namespace UtemorgonPOS.Terminal;

class Program
{
	static void Main(string[] args)
	{
		Console.Clear();
		SceneManager sm = new();

		sm.Initialize(new SMain());

		Console.WriteLine("End of main execution (success).");
	}
}
