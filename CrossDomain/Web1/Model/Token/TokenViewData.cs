using System.Collections.Generic;

namespace Web1.Model.Token
{
    public class TokenViewData
    {
        public string Token { get; set; }

        public List<string> HostAuthorization { get; set; }
    }
}
