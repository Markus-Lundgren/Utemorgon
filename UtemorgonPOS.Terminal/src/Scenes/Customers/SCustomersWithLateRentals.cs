using System;
using System.Data.Common;
using RiverScenes;
using UtemorgonPOS.Core;

class SCustomersWithLateRentals : IScene
{
  public void Enter()
  { }

  public void Exit()
  { }

  public SceneTransition Run()
  {
    Console.Clear();
    Console.WriteLine("=====~* UTEMORGON *~=====");
    Console.WriteLine("Customers > Customers info > Late rentals");
    Console.WriteLine();

    try
    {
      var lateRentals = Data.Customers.GetCustomersWithLateRentals();

      if (lateRentals.Length == 0)
      {
        Console.WriteLine("No late rentals found.");
      }
      else
      {
        foreach (var rental in lateRentals)
        {
          Console.WriteLine("===== LATE RENTALS  =====");
          Console.WriteLine($"Customer: {rental.FullName}");
          Console.WriteLine($"PhoneNumber: {rental.PhoneNumber}");
          Console.WriteLine($"Movie: {rental.MovieName}");
          Console.WriteLine($"Expected return: {rental.ExpectedReturnDate?.ToString("yyyy-MM-dd")}");
          Console.WriteLine("============================");
        }
      }
    }
    catch (DbException)
    {
      Console.WriteLine();
      Console.WriteLine("A database error occurred.");
      Console.WriteLine("Late rentals could not be retrieved.");
    }

    Console.WriteLine();
    Console.WriteLine("Press any key to return.");
    Console.ReadKey();

    return new SceneTransition.Pop();
  }
}