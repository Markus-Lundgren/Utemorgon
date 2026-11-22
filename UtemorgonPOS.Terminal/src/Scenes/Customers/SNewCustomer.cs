using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SNewCustomer : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > New customer");
    Console.WriteLine();

    Customer customer = new();

    try
    {
      while (true)
      {
        Console.Write("Personal number (12 digits, leave blank if unavailable): ");
        string? socialNumber = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(socialNumber))
        {
          customer.SocialNumber = null;
          break;
        }

        bool validNumber = socialNumber.Length == 12;

        foreach (char digit in socialNumber)
        {
          if (digit < '0' || digit > '9')
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

        if (Data.Customers.GetCustomerBySocialNumber(socialNumber) != null)
        {
          Console.WriteLine("A customer with this personal number already exists.");
          continue;
        }

        customer.SocialNumber = socialNumber;
        break;
      }

      while (true)
      {
        Console.Write("First name: ");
        string firstName = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(firstName))
        {
          Console.WriteLine("First name is required. Please try again.");
          continue;
        }

        customer.FName = firstName;
        break;
      }

      while (true)
      {
        Console.Write("Last name: ");
        string lastName = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(lastName))
        {
          Console.WriteLine("Last name is required. Please try again.");
          continue;
        }

        customer.LName = lastName;
        break;
      }

      Console.Write("Address with both street and city: ");
      customer.Address = Console.ReadLine()?.Trim() ?? "";

      Console.Write("Phone number: ");
      customer.PhoneNumber = Console.ReadLine()?.Trim() ?? "";

      Data.Customers.AddCustomer(customer);

    }
    catch (DbException)
    {
      Console.Clear();
      Console.WriteLine("===== CUSTOMER not ADDED =====");
      Console.WriteLine("A database error occurred.");
      Console.WriteLine("The customer could not be saved.");
      Console.WriteLine();
      Console.WriteLine("Please try again later.");
      Console.WriteLine();
      Console.WriteLine("Press any key to return.");
      Console.ReadKey();

      return new SceneTransition.Pop();
    }

    Console.Clear();
    Console.WriteLine("===== CUSTOMER ADDED =====");
    Console.WriteLine($"Name: {customer.FullName}");
    Console.WriteLine($"Personal number: {customer.SocialNumber ?? "Not provided"}");
    Console.WriteLine($"Address: {customer.Address}");
    Console.WriteLine($"Phone number: {customer.PhoneNumber}");
    Console.WriteLine("==========================");
    Console.WriteLine();
    Console.WriteLine("Customer successfully added.");

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}