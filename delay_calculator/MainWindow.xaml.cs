using System;
using System.Windows;

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

        public string CalculateWithCoefficient(int bpm, double C) //где K это коэффициент
        {
            string result = ""; string delayTime = "";

            for (int i = 0; i < startArray.Length; i++)
            {
                if (bpm == 0) //Если ноль, то просто выводим заглушку
                 { delayTime = string.Format("{0}1/{1}=∞ ms", Environment.NewLine, Convert.ToString(startArray[i]));}
                else
                {
                 /*Например, у нас есть трек, с BPM = 120. В одной минуте 60000 ms, в одном такте 4 доли. получаем: 240000/120=2000ms.*/
                 delayTime = string.Format("{0}1/{1}={2} ms", Environment.NewLine, Convert.ToString(startArray[i]), Math.Round(240000/bpm/startArray[i]*C,2));
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
        }

        /// <summary>
        /// Метод для запусках расчета всех трех вариантов (обычные ноты, триолли, полторашки)
        /// </summary>
        /// <param name="bpm">BPM</param>
        private void NotesCalculate(int bpm)
        {
            //Создаем новый объект 
            var cs = new CalculateSupport();
            //Используем новый объект :)
            //Пишем и считаем для обычных нот
            NotesTextBlock.Text = cs.CalculateWithCoefficient(bpm, 1);
            //Пишем и считаем для триолей
            TriolliTextBlock.Text = cs.CalculateWithCoefficient(bpm, 0.667);
            //Пишем и считаем для полуторных нот
            DotesTextBlock.Text = cs.CalculateWithCoefficient(bpm, 1.5);
        }

        /// <summary>
        /// Метод запускаемый и при запуске программы и по нажатию кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        protected void Calculate(object sender, EventArgs ea)
        {
            try
            { 
                NotesCalculate(int.Parse(BpmTextInput.Text));
            }
            catch (System.FormatException) //Если кто-то ввел не число, покажем исключение
            {
                MessageBox.Show("Введите целое число!");
            }

        }
    }
}
