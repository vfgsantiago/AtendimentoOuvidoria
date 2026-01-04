namespace AtendimentoOuvidoria.Helpers
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Retorna o primeiro dia da semana baseado no dia informado.
        /// </summary>
        /// <param name="dt">Data de referência</param>
        /// <param name="startOfWeek">Dia da semana que representa o início (ex: Monday)</param>
        /// <returns>Data do primeiro dia da semana</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Retorna o último dia da semana baseado no dia informado.
        /// </summary>
        /// <param name="dt">Data de referência</param>
        /// <param name="startOfWeek">Dia da semana que representa o início (ex: Monday)</param>
        /// <returns>Data do último dia da semana</returns>
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(6).Date;
        }
    }
}
