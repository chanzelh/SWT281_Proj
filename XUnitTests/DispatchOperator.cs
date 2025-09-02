using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Milestone_2
{
    // Represents a dispatch operator responsible for handling emergency calls
    public class DispatchOperator
    {
        // The first name of the operator
        public string OperatorName { get; set; }
        // The surname of the operator
        public string OperatorSurname { get; set; }
        // The username used for operator login
        public string Username { get; set; }
        // The password for operator authentication (kept private)
        private string Password { get; set; }

        // Authenticates the operator using the provided username and password
        // Returns true if login is successful, false otherwise
        public bool Login(string username, string password)
        {
            // This Dictonary string is used to store the username and password pairs
            Dictionary<string, string> users = new Dictionary<string, string>
            {
                { "admin", "admin@123" },
                { "operator1", "password@1" },
                { "operator2", "password@2" }
        };

            // This checks to see if the username and password are in the dictionary
            if (users.ContainsKey(username) && users[username] == password)
            {
                Username = username;
                Password = password; // Store the password securely
                return true; // Login successful
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
                return false; // Login failed
            }
        }
    }
}
