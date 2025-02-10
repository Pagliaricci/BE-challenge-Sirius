

// using EmailService.Modules.Users.Models;

// namespace EmailService.Modules.Users.Repositories
// {
//     public class TestRepository: EmailService.Modules.Users.Repositories.IUserRepository
//     {
//          private readonly List<EmailService.Modules.Users.Models.User> _users =
//          [
//              new() {
//                  Id = 1,
//                  Username = "admin",
//                  PasswordHash = "admin",
//                  Role = "admin",
//                  Email = "admin@gmail.com",
//                     EmailsSentToday = 0,
//                     LastEmailReset = DateTime.UtcNow
//                 },
//                 new User
//                 {
//                     Id = 2,
//                     Username = "user",
//                     PasswordHash = "user",
//                     Role = "user",
//                     Email = "user@gmail.com",
//                     EmailsSentToday = 0,
//                     LastEmailReset = DateTime.UtcNow
//                 }
//             ];

//         public async Task<User> GetUserByIdAsync(int id)
//         {
//             return await Task.Run(() =>
//             {
//                 var user = _users.FirstOrDefault(u => u.Id == id);
//                 if (user == null)
//                 {
//                     throw new KeyNotFoundException($"User with id {id} not found.");
//                 }
//                 return user;
//             });
//         }

//         public async Task<List<User>> GetAllUsersAsync()
//         {
//             return await Task.Run(() => _users.ToList());
//         }

//         public async Task<User> CreateUserAsync(CreateUser user)
//         {
//             return await Task.Run(() =>
//             {
//                 var newUser = new User
//                 {
//                     Id = _users.Max(u => u.Id) + 1,
//                     Username = user.Username,
//                     PasswordHash = user.Password,
//                     Role = user.Role,
//                     Email = user.Email,
//                     EmailsSentToday = 0,
//                     LastEmailReset = DateTime.UtcNow
//                 };
//                 _users.Add(newUser);
//                 return newUser;
//             });
//         }

//         public async Task<bool> UpdateUserAsync(User user)
//         {
//             return await Task.Run(() =>
//             {
//                 var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
//                 if (existingUser == null)
//                     return false;

//                 existingUser.Username = user.Username;
//                 existingUser.PasswordHash = user.PasswordHash;
//                 existingUser.Role = user.Role;
//                 existingUser.Email = user.Email;
//                 return true;
//             });
//         }

//         public async Task<bool> DeleteUserAsync(int id)
//         {
//             return await Task.Run(() =>
//             {
//                 var user = _users.FirstOrDefault(u => u.Id == id);
//                 if (user == null)
//                     return false;

//                 _users.Remove(user);
//                 return true;
//             });
//         }

//         public async Task<User?> FindUserByUsernameAndPassword(string username, string passwordHash)
//         {
//             return await Task.Run(() => _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash));
//         }

//     }

// }