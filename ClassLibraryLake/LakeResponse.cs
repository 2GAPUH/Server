using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryLake
{
    /// <summary>
    /// Ответ от справочника
    /// </summary>
    public class LakeResponse
    {
        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Запрос успешен
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Информация о озере
        /// </summary>
        public Lake Lake { get; set; }
    }
}
