using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomersByName : IScene
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

    try
    {
      var customers = Data.Customers.GetCustomersByName(name);
      Console.WriteLine();

      bool hasCustomers = false;

      foreach (var customer in customers)
      {
        hasCustomers = true;
        Console.WriteLine("===== CUSTOMER BY NAME =====");
        Console.WriteLine($"Customer ID: {customer.Id}");
        Console.WriteLine($"Name: {customer.FullName}");
        Console.WriteLine("============================");
      }

      if (!hasCustomers) Console.WriteLine("No customers found.");
    }
    catch (DbException)
    {
      Console.WriteLine();
      Console.WriteLine("A database error occurred.");
      Console.WriteLine("Customers could not be retrieved.");
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}