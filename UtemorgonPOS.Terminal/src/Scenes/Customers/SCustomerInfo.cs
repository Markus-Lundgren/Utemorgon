using System;
using RiverScenes;

class SCustomerInfo : IScene
{
  static SceneTransition? _deferredTransition;

  Menu _menu = new([
    new(ConsoleKey.A, "Show all customer", () => {_deferredTransition = new SceneTransition.Push(SceneRepository.Customers.All); }),
    new(ConsoleKey.I, "Search customer by ID", () => {}),
    new(ConsoleKey.P, "Search customer by personal number", () => {}),
    new(ConsoleKey.N, "Search customer by name", () => {}),
    new(ConsoleKey.S, "Customers with late rentals", () => {}),
    new(ConsoleKey.B, "Back", () => {_deferredTransition = new SceneTransition.Pop(); }),
  ]);

  public void Enter()
  {
    _deferredTransition = null;
  }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    while (true)
    {
      Console.Clear();
      Console.WriteLine("=====~* UTEMORGON *~=====");
      Console.WriteLine("Customer > Customer info");
      Console.WriteLine(_menu.MenuRows);

      var a = _menu.GetAction(Console.ReadKey(intercept: true).Key);

      if (a is null)
        Console.WriteLine("Not a valid chocie!");
      else
        a?.Action();

      if (_deferredTransition != null)
        return _deferredTransition;

      Console.WriteLine("Press any key to continue...");
      Console.ReadKey();
    }
  }

}