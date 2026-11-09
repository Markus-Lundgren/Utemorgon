using System;
using UtemorgonPOS.Core;
namespace UtemorgonPOS.CustomerDisplay;

class Program
{
	static void Main(string[] args)
	{
		var name = EasyEnv.Get<string>("kalle");
		var age = EasyEnv.Get<int>("age") + 10;
		Console.WriteLine($"{name}: {age} år.");
	}
}