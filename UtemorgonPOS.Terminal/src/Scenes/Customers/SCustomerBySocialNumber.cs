using System;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomerBySocialNumber : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Customers info > Search by personal number");
    Console.WriteLine();

    string socialNumber = "";
    bool isValid = false;

    while (!isValid)
    {
      Console.Write("Enter personal number: ");
      socialNumber = Console.ReadLine() ?? "";

      bool onlyDigits = true;

      for (int i = 0; i < socialNumber.Length; i++)
      {
        if (socialNumber[i] < '0' || socialNumber[i] > '9')
        {
          onlyDigits = false;
          break;
        }
      }

      isValid = (socialNumber.Length == 12) && onlyDigits;

      if (!isValid)
      {
        Console.WriteLine("Invalid socialnumber. Try again: ");
      }
    }

    var customer = Data.Customers.GetCustomerBySocialNumber(socialNumber);
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