
namespace Auth.Types
{
    public class InMemoryTokenStore : ITokenStore
    {
        private readonly Dictionary<string, DateTime> tokenStore = [];
        public bool IsValid(string jti)
        {
            return tokenStore.TryGetValue(jti, out var expires) && expires > DateTime.UtcNow;
        }

        public void Revoke(string jti)
        {
            tokenStore.Remove(jti);
        }

        public void Store(string jti, DateTime expires)
        {
            tokenStore[jti] = expires;
        }
    }
}
