using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Globalization; //Пространство имен, ака библиотека для локализации

namespace DelayCalculator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
     public partial class App : Application
    {
        //Создаем копию объекта для хранения региональных значений, таких как настройки языка и т.п
        private static List<CultureInfo> LanguagesList = new List<CultureInfo>();

        /// <summary>
        /// Метод для возвращения списка поддерживаемых языков
        /// </summary>
        public static List<CultureInfo> Languages
        { get
            { return LanguagesList; } //Получаем все занесенные региональные значения
        } 

        public App()
        {
            InitializeComponent();
            LanguageChanged += AppLanguageChanged; //Инициализируем перехватчик события методом смены языка и сохранения состояния

            //Очищаем лист с языками и добавляем 2 новых при инициализации приложения
            LanguagesList.Clear();
            LanguagesList.Add(new CultureInfo("en-US")); 
            LanguagesList.Add(new CultureInfo("ru-RU")); //Дефолт для этого проекта

            //Устанавливаем сохраненный язык для приложения и создаем нужный словарь
            Language = DelayCalculator.Properties.Settings.Default.DefaultLanguage;
        }

        /// <summary>
        /// Перехватчик события смены языка из приложения
        /// </summary>
        public static event EventHandler LanguageChanged;

        /// <summary>
        /// Метод-обертка для смены и сохранения выбранного языка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppLanguageChanged(Object sender, EventArgs e)
        {
            DelayCalculator.Properties.Settings.Default.DefaultLanguage = Language; 
            DelayCalculator.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Метод в котором происходит смена языка, создание нового словаря и вызывается event для обновления форм
        /// </summary>
        public static CultureInfo Language
        {
         get { return System.Threading.Thread.CurrentThread.CurrentUICulture; } //Получаем текущий язык интерфейса

         set { if (value == null) throw new ArgumentNullException("value"); //Если язык не передан, обработаем исключение
               if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return; //Если выбран тот же язык, то ничего не делаем.

               //Если переданный язык и текущий не сопадает, то меняем язык приложения:
               System.Threading.Thread.CurrentThread.CurrentUICulture = value;

               //Создаём объект "словарь" для новой культуры
               var dict = new ResourceDictionary();

               //Заполняем словарь из нужного файла с локализацией исходя из того какой язык нужен
               switch (value.Name)
                 { case "en-US": dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break; //выходим из switch
                   default: dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break; //выходим из switch
                 }

               //Находим старый словарь, удаляем его и добавляем новый
               ResourceDictionary oldDict = (from d in App.Current.Resources.MergedDictionaries
                                            where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                           select d).First();

               App.Current.Resources.MergedDictionaries.Clear();
               App.Current.Resources.MergedDictionaries.Add(dict);

               /*Вызываем евент для оповещения всех окон и смены языка везде*/
               LanguageChanged(App.Current, new EventArgs());
            }
        }
    }
}
