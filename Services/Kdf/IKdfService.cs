namespace ASP_202.Services.Kdf
{
    /// <summary>
    /// Key Derivation Function Service (RFC 8018)
    /// </summary>
    public interface IKdfService
    {
        /// <summary>
        /// Make Derived Key from password and salt
        /// </summary>
        /// <param name="password">Password string</param>
        /// <param name="salt">Salt string</param>
        /// <returns>Derived Key string</returns>
        String GetDerivedKey(String password, String salt);
    }
}
