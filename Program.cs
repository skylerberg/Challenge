using System;
using System.Linq;
using System.Collections.Generic;
using PhoneNumbers;

namespace Challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new UserServiceClient();
            IEnumerable<User> users = client.All();

            IEnumerable<User> selectedUsers = users
                .Where(user => IsValidUSNumber(user.Number))
                .OrderBy(user => user.Age)
                .Take(5)
                .OrderBy(user => user.Name);

            foreach (User user in selectedUsers)
            {
                Console.WriteLine($"{user.Name} - {user.Age} - {user.Number}");
            }
        }

        static bool IsValidUSNumber(string number)
        {
            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
            PhoneNumber parsedNumber;

            try
            {
                parsedNumber = phoneNumberUtil.Parse(number, "US");
            }
            catch (NumberParseException)
            {
                return false;
            }

            // Checking if a number is a possible US number is a fast and reasonably accurate way to determine validity
            return phoneNumberUtil.IsPossibleNumber(parsedNumber);
        }
    }
}
