using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Collections.Generic;

namespace DelayCalculator
{
    public class CalculateSupport
    {
        //Объявляем глобальный массив дробей для расчета
        int[] startArray = {1, 2, 3, 4, 8, 16, 32, 64, 128};

        /// <summary>
        /// Метод для рассчета задержки и формирования строки для вывода в форму (переносы строки включены)
        /// </summary>
        /// <param name="bpm">BPM</param>
        /// <param name="C">Коэффициент рассчета для триоллей и полуторых нот</param>
        /// <param name="result">Результат работы метода</param>

        public string CalculateWithCoefficient(int bpm, double C) //где C это коэффициент
        {
            string result = ""; string delayTime = "";

            for (int i = 0; i < startArray.Length; i++)
            {
                if (bpm == 0) //Если ноль, то просто выводим заглушку
                 { delayTime = string.Format("{0}1/{1}=∞ {2}", Environment.NewLine, Convert.ToString(startArray[i]), Application.Current.FindResource("ms"));}
                else
                {
                 /*Например, у нас есть трек, с BPM = 120. В одной минуте 60000 ms, в одном такте 4 доли. получаем: 240000/120=2000ms.*/
                 delayTime = string.Format("{0}1/{1}={2} {3}", Environment.NewLine, Convert.ToString(startArray[i]), Math.Round(240000/bpm/startArray[i]*C,2), Application.Current.FindResource("ms"));
                }
                result = result + delayTime;
            }
            return result;
        }
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Инициализируем перехватчик события для отмечания нужного пункта языка в меню
            App.LanguageChanged += LanguageChanged; 

            var currLang = App.Language;

            //Заполняем меню смены языка теми региональными данными что получили при инициализации App
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }

           BpmTextInput.Text = Properties.Settings.Default.DefaultBpm; //Получим введенный при прошлом запуске BPM
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            var currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        /// <summary>
        /// Метод для смены языка по клику из формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void ChangeLanguageClick(object sender, EventArgs ea)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
            Calculate(sender, ea); //Обновляем формы с результатами
        }

        /// <summary>
        /// Метод для проверки корректности ввода данных в поле BPM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BpmTextInputPreview(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        /// <summary>
        /// Метод для запусках расчета всех трех вариантов (обычные ноты, триолли, полторашки)
        /// </summary>
        /// <param name="bpm">BPM</param>
        protected void NotesCalculate(int bpm)
        {
            //Создаем новый объект 
            var cs = new CalculateSupport();

            notesTextBlock.Text = cs.CalculateWithCoefficient(bpm, 1);       //Пишем и считаем для обычных нот
            triolliTextBlock.Text = cs.CalculateWithCoefficient(bpm, 0.667); //Пишем и считаем для триолей
            dotesTextBlock.Text = cs.CalculateWithCoefficient(bpm, 1.5);     //Пишем и считаем для полуторных нот

            //Закэшируем введенный BPM
            Properties.Settings.Default.DefaultBpm = BpmTextInput.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Метод запускаемый и при запуске программы и по нажатию кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        protected void Calculate(object sender, EventArgs ea)
        {
                NotesCalculate(int.Parse(BpmTextInput.Text));
        }

        private void AboutAppButton(object sender, RoutedEventArgs e)
        {
            //  about_window.Show();
            //MessageBox.Show("Утилита для расчета времени задержки исходя из BPM. Исходный код и разработчик - https://github.com/treepoint/Micro_Delay_Calculator");

            about winAbout = new about();
            winAbout.Show();
        }
    }
}
