using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace simplex_method
{
    public class DoubleToFraction
    {
        /// <summary>
        /// Конвертация дестичной дроби в обыкновенную.
        /// </summary>
        /// <param name="elem">Дестичная дробь.</param>
        /// <returns>Обыкновенная дробь.</returns>
        public static string Convert(double elem) //не проверено для отрицательного
        {
            elem = Math.Round(elem, 2);

            //сплит по запятой
            string[] str = elem.ToString().Split(',');

            //если целое число
            if (str.Length == 1)
            {
                return elem.ToString();
            }
            else
            {

                //определяем знак
                sbyte sign = 1;
                if (elem < 0)
                    sign = -1;

                //определяем знаменатель
                string factor = new string('0', str[1].Length);
                factor = "1" + factor;

                //числитель
                int top_number = Math.Abs(Int32.Parse(str[1]));
                //знаменатель
                int bottom_number = Int32.Parse(factor);

                //НОД
                int nod = GreatestCommonDivisor(top_number, bottom_number);

                //сокращаем согласно НОД
                top_number /= nod;
                bottom_number /= nod;

                return sign * top_number + "/" + bottom_number;
            }
        }

        /// <summary>
        /// Наибольший общий делитель.
        /// </summary>
        /// <returns>Наибольший общий делитель.</returns>
        private static int GreatestCommonDivisor(int a, int b)
        {
            a = Math.Abs(a);
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a = a % b;
                else
                    b = b % a;
            }
            return Math.Abs(a + b);
        }
    }
}
