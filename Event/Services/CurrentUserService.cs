using System.Security.Claims;

namespace Event.Services
{
    public interface ICurrentUserService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            Id = httpContextAccessor.HttpContext.User.FindFirstValue("Id");
            Name = httpContextAccessor.HttpContext.User.FindFirstValue("Name");
            PhoneNumber = httpContextAccessor.HttpContext.User.FindFirstValue("PhoneNumber");
            Email = httpContextAccessor.HttpContext.User.FindFirstValue("Email");
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
