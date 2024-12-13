namespace ClassLibraryLake
{
    /// <summary>
    /// Озеро
    /// </summary>
    public class Lake
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Глубина
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Кол-во смертей
        /// </summary>
        public int MortalityCount { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
    }
}
