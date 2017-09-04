using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyRateApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Словарь (список пар ключ-значение) валют.
        /// В качестве ключа выступает аббревиатура (междунардный код) валюты,
        /// в качестве значения - название валюты на русском языке.
        /// </summary>
        private List<KeyValuePair<string, string>> Сurrencies;

        /// <summary>
        /// Экземпляр класса CurrencyRate для работы с курсом валют с сайта Центрального банка РФ
        /// </summary>
        private CurrencyRate CurrencyRateFromCBR;

        /// <summary>
        /// Индекс выбранной пользователем валюты
        /// </summary>
        private int SelectedIndex = 0;

        /// <summary>
        /// Код выбранной пользователем валюты
        /// </summary>
        private string SelectedRateCode = "";

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Получаем список валют и устанавливаем названия валют в ComboBox
                InitializeForm();
            }
            catch // Если в процессе получения курса валют произошла ошибка,
            {
                // То отображаем пользователю сообщение об ошибке
                MessageBox.Show("Во время получения курса валют произошла ошибка. Программа будет закрыта", "Ошибка получения курса валют", MessageBoxButton.OK, MessageBoxImage.Error);

                // Закрываем программу
                this.Close();
            }
        }

        /// <summary>
        /// Инициализация формы - получение списка валют и установка названий валют в ComboBox
        /// </summary>
        private void InitializeForm()
        {
            // Создаём экземпляр класса CurrencyRate
            this.CurrencyRateFromCBR = new CurrencyRate();

            // Получаем словарь названий валют
            this.Сurrencies = this.CurrencyRateFromCBR.GetCurrencies();

            // Создаём список валют, который отобразятся пользователю в выпадающем меню
            List<string> comboBoxItems = new List<string>();

            // Для каждой пары ключ-значение из словаря валют
            foreach (KeyValuePair<string, string> currencyPair in this.Сurrencies)
            {
                // Добавляем название валюты в список отображаемых названий
                comboBoxItems.Add(currencyPair.Value);
            }

            // Устанавливаем полученный список валют как источник данных для ComboBox 
            this.comboBoxCurrency.ItemsSource = comboBoxItems;

            // Устанавливаем текущую валюту 
            this.comboBoxCurrency.SelectedIndex = this.SelectedIndex;
        }

        /// <summary>
        /// Метод, вызываемый при изменении текущей валюты в ComboBox
        /// </summary>
        private void comboBoxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Запоминаем индекс выбранной валюты
            this.SelectedIndex = this.comboBoxCurrency.SelectedIndex;

            // Получаем название выбранной пользователем валюты
            string selectedName = this.comboBoxCurrency.Items[this.SelectedIndex].ToString();

            this.SelectedRateCode = (from currencyPair in this.Сurrencies // Для каждой пары из словаря валют
                                    where currencyPair.Value == selectedName // где значение пары равно выбранному имени валюты
                                    select currencyPair.Key // выбираем ключ - код валюты
                                    ).First(); // Из полученной коллекции выбираем первый элемент

            // Устанавливаем актуальный курс выбранной валюты
            SetActualRate();
        }

        /// <summary>
        /// Метод, вызываемый при нажатии на кнопку "Обновить курс"
        /// Обновляет курс текущей валюты, а также обновляет список валют
        /// </summary>
        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Получаем список валют и устанавливаем названия валют в ComboBox
            InitializeForm();

            // Устанавливаем актуальный курс выбранной валюты
            SetActualRate();
        }

        /// <summary>
        /// Установить (показать пользователю) актуальный курс выбранной валюты
        /// </summary>
        private void SetActualRate()
        {
            // Получаем текущий курс согласно выбранному коду валюты
            string actualRate = this.CurrencyRateFromCBR.GetCurrencyRateByCode(this.SelectedRateCode);

            // Выводим пользователю актуальный курс выбранной валюты
            this.textBoxCurrentRate.Text = actualRate;
        }
    }
}
