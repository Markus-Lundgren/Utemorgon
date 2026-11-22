using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SDeleteCustomer : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Delete customer");
    Console.WriteLine();

    while (true)
    {
      Console.Write("Enter customer ID: ");

      if (!int.TryParse(Console.ReadLine(), out int id))
      {
        Console.WriteLine("Invalid customer ID.");
        Console.WriteLine("Please try again.");
        Console.WriteLine();
        continue;
      }

      try
      {
        Customer? customer = Data.Customers.GetCustomer(id);

        if (customer == null)
        {
          Console.WriteLine("No customer found with this ID.");
          Console.WriteLine("Please try again.");
          Console.WriteLine();
          continue;
        }

        Console.WriteLine();
        Console.WriteLine($"Customer: {customer.FullName}");
        Console.WriteLine($"Personal number: {customer.SSNFormated}");
        Console.WriteLine();
        Console.WriteLine("This will delete the customer.");
        Console.WriteLine("The customer record and ID will remain in the database.");
        Console.WriteLine();

        ConsoleKey confirmation;

        while (true)
        {
          Console.Write("Are you sure? (y/n): ");
          confirmation = Console.ReadKey(intercept: true).Key;
          Console.WriteLine();

          if (confirmation == ConsoleKey.Y || confirmation == ConsoleKey.N)
            break;

          Console.WriteLine("Please enter Y or N");
        }

        if (confirmation == ConsoleKey.N)
        {
          Console.WriteLine("Deletion cancelled.");
          Console.WriteLine("Returning to customer menu.");
          Console.WriteLine("Press any key to return.");
          Console.ReadKey();
          return new SceneTransition.Pop();
        }

        Console.WriteLine();
        Console.WriteLine("Deleting customer...");

        Data.Customers.DeleteCustomer(id);

        Customer? deletedCustomer = Data.Customers.GetCustomer(id);

        if (deletedCustomer == null)
        {
          Console.WriteLine("The customer could not be retrieved after deletion.");
          Console.WriteLine("Please check the database.");
        }
        else
        {
          Console.WriteLine();
          Console.WriteLine("===== DELETE CUSTOMER  =====");
          Console.WriteLine($"Customer ID: {deletedCustomer.Id}");
          Console.WriteLine($"Name: {deletedCustomer.FullName}");
          Console.WriteLine($"Personal number: {deletedCustomer.SocialNumber ?? "Not provided"}");
          Console.WriteLine($"Address: {deletedCustomer.Address}");
          Console.WriteLine($"Phone number: {deletedCustomer.PhoneNumber}");
          Console.WriteLine("===============================");
          Console.WriteLine();
          Console.WriteLine("Customer successfully deleted.");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to return.");
        Console.ReadKey();

        return new SceneTransition.Pop();
      }
      catch (DbException)
      {
        Console.WriteLine();
        Console.WriteLine("A database error occurred.");
        Console.WriteLine("The deletion could not be completed or verified.");
        Console.WriteLine();
        Console.Write("Press R to retry or any other key to return: ");

        ConsoleKey retry = Console.ReadKey(intercept: true).Key;
        Console.WriteLine();

        if (retry != ConsoleKey.R)
          return new SceneTransition.Pop();

        Console.WriteLine();
      }
    }
  }
}