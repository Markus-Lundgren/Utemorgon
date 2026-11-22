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
    Console.WriteLine("Customers > Anonymize customer");
    Console.WriteLine();

    while (true)
    {
      Console.Write("Enter customer ID: ");

      if (!int.TryParse(Console.ReadLine(), out int id))
      {
        Console.WriteLine("Invalid customer ID.");
        Console.WriteLine("Please try again.");
        continue;
      }

      try
      {
        Customer? customer = Data.Customers.GetCustomer(id);

        if (customer == null)
        {
          Console.WriteLine("No customer found with this ID.");
          Console.WriteLine("Please try again.");
          continue;
        }

        Console.WriteLine();
        Console.WriteLine($"Customer: {customer.FullName}");
        Console.WriteLine($"Personal number: {customer.SSNFormated}");
        Console.WriteLine();
        Console.WriteLine("This will anonymize the customer.");
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
          Console.WriteLine("Anonymization cancelled.");
          Console.WriteLine("Returning to customer menu.");
          Console.WriteLine("Press any key to return.");
          Console.ReadKey();
          return new SceneTransition.Pop();
        }

        Console.WriteLine();
        Console.WriteLine("Anonymizing customer...");

        Data.Customers.DeleteCustomer(id);

        Customer? anonymizedCustomer = Data.Customers.GetCustomer(id);

        if (anonymizedCustomer == null)
        {
          Console.WriteLine("The customer could not be retrieved after anonymization.");
          Console.WriteLine("Please check the database.");
        }
        else
        {
          Console.WriteLine();
          Console.WriteLine("===== CUSTOMER ANONYMIZED =====");
          Console.WriteLine($"Customer ID: {anonymizedCustomer.Id}");
          Console.WriteLine($"Name: {anonymizedCustomer.FullName}");
          Console.WriteLine($"Personal number: {anonymizedCustomer.SocialNumber ?? "Not provided"}");
          Console.WriteLine($"Address: {anonymizedCustomer.Address}");
          Console.WriteLine($"Phone number: {anonymizedCustomer.PhoneNumber}");
          Console.WriteLine("===============================");
          Console.WriteLine();
          Console.WriteLine("Customer successfully anonymized.");
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
        Console.WriteLine("The anonymization could not be completed or verified.");
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