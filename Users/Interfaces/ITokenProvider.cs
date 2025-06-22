namespace Users.Interfaces
{
    public interface ITokenProvider
    {
        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// <returns>The authentication token as a string.</returns>
        Task<string?> GetTokenAsync();
        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The authentication token to set.</param>
        void SetToken(string token);
    }
}
