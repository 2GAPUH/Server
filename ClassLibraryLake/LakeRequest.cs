using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryLake
{
    /// <summary>
    /// Запрос к справочнику озёр
    /// </summary>
    public class LakeRequest
    {
        /// <summary>
        /// Тип зароса
        /// </summary>
        public LakeRequestType RequestType { get; set; }
        
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Информация о озёре
        /// </summary>
        public Lake Lake { get; set; }
    }
}
