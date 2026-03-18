using BCrypt.Net;

// 生成BCrypt加密后的密码
string password = "admin";
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine($"密码: {password}");
Console.WriteLine($"BCrypt加密后: {hashedPassword}");
