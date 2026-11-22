using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SUpdateCustomer : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Update customer");
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
        Customer? originalCustomer = Data.Customers.GetCustomer(id);

        if (originalCustomer == null)
        {
          Console.WriteLine("No customer found with this ID.");
          Console.WriteLine("Please try again.");
          Console.WriteLine();
          continue;
        }

        Console.WriteLine();
        Console.WriteLine("===== CURRENT CUSTOMER =====");
        Console.WriteLine($"Customer ID: {originalCustomer.Id}");
        Console.WriteLine($"Personal number: {originalCustomer.SSNFormated}");
        Console.WriteLine($"Name: {originalCustomer.FullName}");
        Console.WriteLine($"Adress: {originalCustomer.Address}");
        Console.WriteLine($"Phonenumber: {originalCustomer.PhoneNumber}");
        Console.WriteLine("============================");
        Console.WriteLine();
        Console.WriteLine("Leave empty to keep the current value, by pressing Enter.");
        Console.WriteLine("To clear a field with a value, type a hyphen (-) and press Enter.");
        Console.WriteLine();

        Customer updatedCustomer = new Customer
        {
          Id = originalCustomer.Id,
          SocialNumber = originalCustomer.SocialNumber,
          FName = originalCustomer.FName,
          LName = originalCustomer.LName,
          Address = originalCustomer.Address,
          PhoneNumber = originalCustomer.PhoneNumber
        };

        while (true)
        {
          Console.Write($"First name ({originalCustomer.FName}): ");
          string firstName = Console.ReadLine()?.Trim() ?? "";

          if (firstName == "")
            break;

          updatedCustomer.FName = firstName;
          break;
        }

        while (true)
        {
          Console.Write($"Last name ({originalCustomer.LName}): ");
          string lastName = Console.ReadLine()?.Trim() ?? "";

          if (lastName == "")
            break;

          updatedCustomer.LName = lastName;
          break;
        }

        while (true)
        {
          Console.Write($"Personal number ({originalCustomer.SocialNumber ?? "Not provided"}): ");
          string socialNumber = Console.ReadLine()?.Trim() ?? "";

          if (socialNumber == "")
            break;

          if (socialNumber == "-")
          {
            updatedCustomer.SocialNumber = null;
            break;
          }

          bool validNumber = socialNumber.Length == 12;

          foreach (char character in socialNumber)
          {
            if (character < '0' || character > '9')
            {
              validNumber = false;
              break;
            }
          }

          if (!validNumber)
          {
            Console.WriteLine("Invalid personal number. Enter exactly 12 digits.");
            continue;
          }

          Customer? existingCustomer = Data.Customers.GetCustomerBySocialNumber(socialNumber);

          if (existingCustomer != null && existingCustomer.Id != id)
          {
            Console.WriteLine("This personal number already belongs to another customer.");
            continue;
          }

          updatedCustomer.SocialNumber = socialNumber;
          break;
        }

        Console.Write($"Address ({originalCustomer.Address}): ");
        string address = Console.ReadLine()?.Trim() ?? "";

        if (address == "-")
          updatedCustomer.Address = "";
        else if (address != "")
          updatedCustomer.Address = address;

        Console.Write($"Phonenumber ({originalCustomer.PhoneNumber}): ");
        string phoneNumber = Console.ReadLine()?.Trim() ?? "";

        if (phoneNumber == "-")
          updatedCustomer.PhoneNumber = "";
        else if (phoneNumber != "")
          updatedCustomer.PhoneNumber = phoneNumber;

        bool hasChanges =
          originalCustomer.SocialNumber != updatedCustomer.SocialNumber ||
          originalCustomer.FName != updatedCustomer.FName ||
          originalCustomer.LName != updatedCustomer.LName ||
          originalCustomer.Address != updatedCustomer.Address ||
          originalCustomer.PhoneNumber != updatedCustomer.PhoneNumber
        ;

        if (!hasChanges)
        {
          Console.WriteLine();
          Console.WriteLine("No changes were made.");
          Console.WriteLine("Press any key to return.");
          Console.ReadKey();
          return new SceneTransition.Pop();
        }

        Console.WriteLine();
        Console.WriteLine("Updating customer...");

        Data.Customers.UpdateCustomer(originalCustomer, updatedCustomer);

        Customer? finalizedCustomer = Data.Customers.GetCustomer(id);
        if (finalizedCustomer == null)
        {
          Console.WriteLine("The customer could not be retrieved after anonymization.");
          Console.WriteLine("Please check the database.");
        }
        else
        {
          Console.WriteLine();
          Console.WriteLine("===== UPDATED CUSTOMER =====");
          Console.WriteLine($"Customer ID: {finalizedCustomer.Id}");
          Console.WriteLine($"Name: {finalizedCustomer.FullName}");
          Console.WriteLine($"Personal number: {finalizedCustomer.SocialNumber ?? "Not provided"}");
          Console.WriteLine($"Address: {finalizedCustomer.Address}");
          Console.WriteLine($"Phone number: {finalizedCustomer.PhoneNumber}");
          Console.WriteLine("===============================");
          Console.WriteLine();
          Console.WriteLine("Customer successfully updated.");
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
        Console.WriteLine("The customer could not be updated or verified.");
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