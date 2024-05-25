using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using EmployeeCensus.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCensus.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataContext _context;

        // Конструктор, принимающий необходимые зависимости через DI
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DataContext context)
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        // Асинхронный метод для аутентификации
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Проверка наличия заголовка Authorization в запросе
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // Проверка схемы аутентификации и наличие параметра
                if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    // Декодирование и разбор параметра авторизации
                    var credentials = Encoding.UTF8
                        .GetString(Convert.FromBase64String(authHeaderVal.Parameter))
                        .Split(':', 2);
                    var username = credentials[0];
                    var password = credentials[1];

                    // Поиск пользователя по имени
                    var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

                    // Проверка хеша пароля
                    if (user != null && PasswordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    {
                        var claims = new[] {
                            new Claim(ClaimTypes.NameIdentifier, username),
                            new Claim(ClaimTypes.Name, username),
                        };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        // Возврат успешного результата аутентификации
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        // Возврат результата неудачной аутентификации
                        return AuthenticateResult.Fail("Invalid Authorization Header");
                    }
                }
                else
                {
                    // Возврат результата неудачной аутентификации
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            }
            catch
            {
                // Возврат результата неудачной аутентификации в случае ошибки
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
