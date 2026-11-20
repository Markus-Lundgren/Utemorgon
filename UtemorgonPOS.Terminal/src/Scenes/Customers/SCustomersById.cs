using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomersById : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Customers info > Search by ID");
    Console.WriteLine();

    Console.Write("Enter customer ID: ");

    int id;
    while (!int.TryParse(Console.ReadLine(), out id))
    {
      Console.Write("Invalid ID. Enter a number: ");
    }

    var customer = Data.Customers.GetCustomer(id);
    Console.WriteLine();

    if (customer == null)
    {
      Console.WriteLine("No customers found.");
    }
    else
    {
      Console.WriteLine($"Customer ID: {customer.Id}");
      Console.WriteLine($"Name: {customer.FullName}");
      Console.WriteLine($"Personal number: {customer.SocialNumber}");
      Console.WriteLine($"Address: {customer.Address}");
      Console.WriteLine($"PhoneNumber: {customer.PhoneNumber}");
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}