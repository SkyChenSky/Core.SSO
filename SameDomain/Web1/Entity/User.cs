using System.Collections.Generic;
using System.Linq;

namespace Web1.Entity
{
    public class User
    {
        private static readonly List<User> List = new List<User>
        {
            new User
            {
                Password = "123",
                RealName = "陈珙",
                UserId = "1EEDE8D3-3012-496E-9641-8594C61CDF94",
                UserName = "chengong"
            }
        };

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RealName { get; set; }

        public bool Vaild(string userName, string password)
        {
            var r = List.FirstOrDefault(a => a.UserName == userName && a.Password == password);
            if (r != null)
            {
                UserId = r.UserId;
                UserName = r.UserName;
                Password = r.Password;
                RealName = r.RealName;

                return true;
            }

            return false;
        }
    }
}
