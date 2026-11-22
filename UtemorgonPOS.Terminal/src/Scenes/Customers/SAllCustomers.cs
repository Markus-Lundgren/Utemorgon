using System;
using System.Data.Common;
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

    try
    {
      foreach (var customer in Data.Customers.GetAll())
      {
        hasCustomers = true;

        Console.WriteLine("===== CUSTOMER DETAILS =====");
        Console.WriteLine($"Customer ID: {customer.Id}");
        Console.WriteLine($"Name: {customer.FullName}");
        Console.WriteLine($"Personal number: {customer.SSNFormated}");
        Console.WriteLine($"Address: {customer.Address}");
        Console.WriteLine($"Phone number: {customer.PhoneNumber}");
        Console.WriteLine("============================");
        Console.WriteLine();
      }

      if (!hasCustomers) Console.WriteLine("No customers found.");
    }
    catch (DbException)
    {
      Console.WriteLine("A database error occurred.");
      Console.WriteLine("Customers could not be retrieved.");
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}