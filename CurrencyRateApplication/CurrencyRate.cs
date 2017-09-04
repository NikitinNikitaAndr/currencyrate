using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CurrencyRateApplication
{
    /// <summary>
    /// Класс для получения информации о текущем курсе рубля.
    /// Данные берутся с сайта Центрального банка РФ
    /// </summary>
    class CurrencyRate
    {
        /// <summary>
        /// Список валют, считанный с сайта Центрального банка РФ
        /// </summary>
        private List<Currency> DataFromCBR;

        /// <summary>
        /// Инициализация нового экземпляра класса CurrencyRate
        /// </summary>
        public CurrencyRate()
        {
            // Получаем (считываем) данные с сайта Центрального банка РФ
            GetCurrencyRateFromCBR();
        }

        /// <summary>
        /// Получить (считать) данные с сайта Центрального банка РФ
        /// Результат записывается в поле DataFromCBR класса CurrencyRate
        /// </summary>
        public void GetCurrencyRateFromCBR()
        {
            // Создаём объект класса XmlDocument
            XmlDocument xmlDoc = new XmlDocument();

            // Загружаем XML документ с сайта Центрального банка РФ
            xmlDoc.Load("http://www.cbr.ru/scripts/XML_daily.asp");

            // Получаем корневой элемент документа
            XmlElement root = xmlDoc.DocumentElement;

            // Получаем список тэгов Valute
            XmlNodeList nodes = root.SelectNodes("/ValCurs/Valute");

            // Создаём новый список валют (объектов класса Currency)
            this.DataFromCBR = new List<Currency>();

            // Для каждого тэга Valute
            foreach (XmlNode node in nodes)
            {
                // Получаем текст тэга Name - название валюты
                string name = node["Name"].InnerText;

                // Получаем текст тэга Nominal - номинал валюты
                string nominal = node["Nominal"].InnerText;

                // Получаем текст тэга CharCode - аббревиатура валюты (код)
                string code = node["CharCode"].InnerText;

                // Получаем текст тэга Value - курс (значение) валюты
                double value = Convert.ToDouble(node["Value"].InnerText);

                // Создаём объект класса Currency, передавая в качестве аргументов название, код, номинал и значение курса валюты
                // и добавляем полученный экземпляр в список
                this.DataFromCBR.Add(new Currency(name, code, nominal, value));
            }
        }

        /// <summary>
        /// Получить список названий валют
        /// </summary>
        /// <returns>
        /// Возвращает список пар ключ-значение, где ключом является аббериватура валюты (EUR, USD и т.д.), 
        /// а значением - описание валюты на русском языке (например, 'Доллар США')
        /// </returns>
        public List<KeyValuePair<string, string>> GetCurrencies()
        {
            // Создаём список пар ключ-значение
            var currencies = new List<KeyValuePair<string, string>>();
            
            // Для каждой валюты из списка валют, считанный с сайта Центрального банка РФ
            foreach(Currency currency in this.DataFromCBR)
            {
                // Создаём пару ключ-значение, где , где ключом является аббериватура валюты (EUR, USD и т.д.), 
                // а значением - описание валюты на русском языке
                var currencyPair = new KeyValuePair<string, string>(currency.CharCode, currency.Name);

                // Добавляем созданную пару в список валют
                currencies.Add(currencyPair);
            }

            // Возрващаем полученный список
            return currencies;
        }

        /// <summary>
        /// Получить курс (значение) валюты по коду (аббревиатуре)
        /// </summary>
        /// <param name="CharCode">Международный код валюты</param>
        /// <returns>Строка в следующем формате: 'Номинал Название_валюты = Значение_валюты руб.'</returns>
        public string GetCurrencyRateByCode(string CharCode)
        {
            string rate = (from currency in this.DataFromCBR // для каждой валюты из DataFromCBR
                           where currency.CharCode == CharCode // где код текущей валюты равен полученному коду 
                           select currency.Nominal + " " + currency.Name + " = " + currency.Value + " руб." // Выбираем строку согласно шаблону
                           ).First(); // Из полученных результатов выбираем первый

            // Возрващаем полученный курс
            return rate;
        }
    }

    /// <summary>
    /// Класс для хранения и представления валюты
    /// </summary>
    class Currency
    {
        /// <summary>
        /// Название валюты
        /// </summary>
        public string Name;

        /// <summary>
        /// Аббревиатура валюты (международный код)
        /// </summary>
        public string CharCode;

        /// <summary>
        /// Номинал валюты
        /// </summary>
        public string Nominal;

        /// <summary>
        /// Текущий курс
        /// </summary>
        public double Value;

        /// <summary>
        /// Инициализация нового экземпляра класса Currency
        /// </summary>
        /// <param name="Name_">Название валюты</param>
        /// <param name="CharCode_">Международный код</param>
        /// <param name="Nominal_">Номинал валюты</param>
        /// <param name="Value_">Текущий курс</param>
        public Currency(string Name_, string CharCode_, string Nominal_, double Value_)
        {
            this.Name = Name_;

            this.CharCode = CharCode_;

            this.Nominal = Nominal_;

            this.Value = Value_;
        }
    }
}
