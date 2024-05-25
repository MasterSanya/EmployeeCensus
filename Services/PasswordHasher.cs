using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace EmployeeCensus.Services
{
    public class PasswordHasher
    {
        // Метод для создания хеша и соли пароля
        public static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            // Генерация случайной соли
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Конвертация соли в строку Base64
            passwordSalt = Convert.ToBase64String(salt);

            // Хеширование пароля с использованием соли и алгоритма PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Установка значений хеша и соли
            passwordHash = hashed;
        }

        // Метод для проверки хеша пароля
        public static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            // Преобразование соли из строки Base64 в байты
            byte[] salt = Convert.FromBase64String(storedSalt);

            // Хеширование введенного пароля с использованием сохраненной соли
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Сравнение хеша введенного пароля с сохраненным хешем
            return hashed == storedHash;
        }
    }
}
