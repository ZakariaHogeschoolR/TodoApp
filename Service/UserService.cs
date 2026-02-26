// using System.Security.Cryptography.X509Certificates;

// public class UserService
// {
//     private Users[] _users;
//     private int _count; 

//     public int Count
//     {
//         get { return _count; }
//         set { _count = value; }
//     }
//     private Users _currentUser;

//     public void Login(int userId)
//     {
//         var user = FindBy(userId, (u, id) => u.Id == id);

//         _currentUser = user;
//     }

//     public Users CurrentUser => _currentUser;

//     public void Register()
//     {

//         var register = FindBy(user.Id, (u, id) => u.Id == id);
//         if(register == null)
//         {
//             AddUser(user);
//         }
//     }

//     public void AddUser(Users user)
//     {
//         Users[] users = new Users[Count + 1];
        
//         for(int i = 0; i < Count; i++)
//         {
//             users[i] = _users[i];
//         }

//         users[Count + 1] = user;
//         Count++;
//     }

//     public Users FindBy<K>(K Key, Func<Users, K, bool> comparer)
//     {
//         for(int i = 0; i < Count; i++)
//         {
//             if(_users[i] == null)
//             {
//                 continue;
//             }
//             if(comparer(_users[i], Key))
//             {
//                 return _users[i];
//             }
//         }
//         return null;
//     }
// }