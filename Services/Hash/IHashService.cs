namespace ASP_202.Services.Hash
{
    public interface IHashService
    {
        /// <summary>
        /// Обчислення гексадецимального хеш-образу від рядкових даних
        /// </summary>
        /// <param name="text">Вхідні дані</param>
        /// <returns>Рядок з гексадецимальним хешем</returns>
        String Hash(String text);
    }
}
