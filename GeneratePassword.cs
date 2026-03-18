using BCrypt.Net;

namespace GeneratePassword
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "admin";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine($"密码: {password}");
            Console.WriteLine($"BCrypt加密后: {hashedPassword}");
        }
    }
}