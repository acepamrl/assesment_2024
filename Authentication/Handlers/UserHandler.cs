using Authentication.Entities;
using Authentication.Extensions;
using Authentication.MessageBroker.PublisherModels;
using Authentication.Models.Request;
using Authentication.Models.Result;
using Authentication.Persistences;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Authentication.Handlers
{
    public interface IUserHandler
    {
        Task<bool> Create(CreateUserRequest request, CancellationToken cancellationToken);
        Task<AuthResult> Login(AuthRequest request, CancellationToken cancellationToken);
    }

    public class UserHandler : IUserHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        public UserHandler(ApplicationDbContext dbContext, IConfiguration configuration, IBus bus)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _bus = bus;
        }

        public async Task<bool> Create(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var userQuery = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (userQuery != null)
                throw new Exception("User already registered!");

            // Password must be equal or more than 8 words
            if (request.Password.Length < 8)
                throw new Exception("Invalid Password : Password must be more than 8 words!");

            // ReGex to check if a string contains lowercase, uppercase, and numeric value
            var regex = "^(?=.*[a-z])" +
                        "(?=.*[A-Z])" +
                        "(?=.*\\d)";
            var regexValidation = new Regex(regex);
            var match = regexValidation.Match(request.Password);

            if (!match.Success)
                throw new Exception($"Invalid Password : Password must contains lowercase, uppercase, and numeric value!");

            var newUser = new User
            {
                Name = request.Name,
                Password = string.Concat(request.Password).ToSHA256(),
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                CreatedBy = "system",
                CreatedDate = DateTime.UtcNow,
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            #region Message Broker
            var eventMessage = new UserPublishEventModel
            {
                Id = newUser.Id,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedBy = "Admin"
            };

            var ticketMessage = new UserPublishTicketModel
            {
                Id = newUser.Id,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedBy = "Admin"
            };

            var paymentMessage = new UserPublishPaymentModel
            {
                Id = newUser.Id,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedBy = "Admin"
            };
            await _bus.Publish(eventMessage);
            await _bus.Publish(ticketMessage);
            await _bus.Publish(paymentMessage);
            #endregion

            return true;
        }

        public async Task<AuthResult> Login(AuthRequest request, CancellationToken cancellationToken)
        {
            var userQuery = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (userQuery == null)
                throw new Exception("User not found!");

            if (userQuery.Password != string.Concat(request.Password).ToSHA256())
                throw new Exception("User not found!");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                    new Claim("Id", userQuery.Id.ToString()),
                    new Claim("Name", userQuery.Name),
                    new Claim("Email", userQuery.Email)
                };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
