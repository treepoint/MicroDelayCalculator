using System;
using System.Windows;

namespace delay_calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Объявялем глобальный массив дробей для расчета
        string[] start_array = {"1","2","3","4","8","16","32","64","128"};
        
        /// <summary>
        /// Метод для рассчета задержки и формирования строки для вывода в форму (переносы строки включены)
        /// </summary>
        /// <param name="bpm">BPM</param>
        /// <param name="K">Коэффициент рассчета для триоллей и полуторых нот</param>
        /// <param name="result">Результат работы метода</param>
        public void calculate_with_K(int bpm, double K, out string result) //где K это коэффициент
        {
            result = "";  string delay_time = "";

            for (int i = 0; i < start_array.Length; i++)
            {
                if (bpm == 0) //Если ноль, то просто выводим заглушку
                { delay_time = "\r\n" + start_array[i] + "=∞ ms"; }
                else
                {
                    /*Например, у нас есть трек, с BPM = 120. В одной минуте 60000 ms, в одном такте 4 доли. получаем:
                      240000/120=2000ms.*/
                    delay_time = "\r\n" + start_array[i] + "= " + Math.Round(240000 / bpm / Convert.ToDecimal(start_array[i]) * Convert.ToDecimal(K), 2) + " ms";
                }
                result = result + delay_time;
            }
        }

        /// <summary>
        /// Метод для запусках расчета всех трех вариантов (обычные ноты, триолли, полторашки)
        /// </summary>
        /// <param name="bpm">BPM</param>
        public void notes_calculate(int bpm)
        {
            string result = "";
            //Пишем и считаем для обычных нот
            calculate_with_K(bpm, 1, out result);
            notes_text_block.Text = result;
            //Пишем и считаем для триолей
            calculate_with_K(bpm, 0.667, out result);
            triolli_text_block.Text = result;
            //Пишем и считаем для полуторных нот
            calculate_with_K(bpm, 1.5, out result);
            dotes_text_block.Text = result;
        }

        /// <summary>
        /// Метод запускаемый и при запуске программы и по нажатию кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void calculate(object sender, EventArgs ea)
        {
            try
            {
                notes_calculate(int.Parse(BPM_text_input.Text));
            }
            catch (System.FormatException) //Если кто-то ввел не число, покажем исключение
            {
                MessageBox.Show("Введите целое число!");
            }
            
        }
    }
}
