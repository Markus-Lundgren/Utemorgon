using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomerByName : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Customers info > Search by name");
    Console.WriteLine();

    Console.Write("Enter customer name: ");
    string name = Console.ReadLine() ?? "";

    var customers = Data.Customers.GetCustomersByName(name);
    Console.WriteLine();

    bool hasCustomers = false;

    foreach (var customer in customers)
    {
      hasCustomers = true;
      Console.WriteLine($"Customer ID: {customer.Id}");
      Console.WriteLine($"Name: {customer.FullName}");
    }

    if (!hasCustomers)
      Console.WriteLine("No customers found.");

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}