namespace Auth.Types
{
    public interface ITokenStore
    {
        void Store(string jti, DateTime expires);
        bool IsValid(string jti);
        void Revoke(string jti);
    }
}
