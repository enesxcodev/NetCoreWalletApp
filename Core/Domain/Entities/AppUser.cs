using Domain.Common;

namespace Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string UserName { get; set; }

        public AppUser() { }

        public AppUser(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public AppUser(Guid id, string firstName, string lastName, string email, string userName)
        {
            this.Id = id; // 🚀 Artık burası hatasız çalışacak ve ezilecek!
            FirstName = firstName ?? "";
            LastName = lastName ?? "";
            Email = email;
            UserName = userName;
        }
    }
}