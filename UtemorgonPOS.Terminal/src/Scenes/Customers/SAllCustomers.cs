using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SAllCustomers : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Customers info > All customers");
    Console.WriteLine();

    bool hasCustomers = false;

    foreach (var customer in Data.Customers.GetAll())
    {
      hasCustomers = true;
      Console.WriteLine($"{customer.Id}: {customer.FullName}");
    }

    if (!hasCustomers) Console.WriteLine("No customers found.");

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}