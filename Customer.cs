namespace Utemorgon;

public class Customer(int id, string firstName, string lastName, string phoneNumber, string address, string ssn)
{
	int Id { get; set; } = id;
	string FirstName { get; set; } = firstName;
	string LastName { get; set; } = lastName;
	string PhoneNumber { get; set; } = phoneNumber;
	string Address { get; set; } = address;
	string SSN { get; set; } = ssn;
}
