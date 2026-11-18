using System;
using System.Text;
using RiverScenes;
using UtemorgonPOS.Core;

class SProductInfo : IScene
{
	StringBuilder _accumulativeMenu = new();

	public void Enter()
	{
		_accumulativeMenu = new("=====~* UTEMORGON *~=====\nProducts > Product information\n");
	}

	public void Exit()
	{
	}

	public SceneTransition Run()
	{
		Console.Clear();
		Console.Write(_accumulativeMenu.ToString());




		return new SceneTransition.Pop();
	}
}