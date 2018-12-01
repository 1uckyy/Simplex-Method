using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrdinaryFractionLibrary
{
    public class ordinary_fraction
    {
        /// <summary>
        /// Дробь.
        /// </summary>
        string fraction { get; set; }
        /// <summary>
        /// Число над чертой.
        /// </summary>
        public int top_number { get; set; }
        /// <summary>
        /// Число под чертой.
        /// </summary>
        public int bottom_number { get; set; }
        ///// <summary>
        ///// Само значение.
        ///// </summary>
        //public double value { get; set; }

        //некоторое описание для типа данных обыкновенных дробей (ordinary_fraction)
        //если дробь отрицательная, то минус должен храниться в числителе (переменной top_number)
        //если число целое, то в числителе (top_number) хранится это целое число, а в знаменателе 1 (bottom_number=1)

        /// <summary>
        /// Оператор присваивания(string to ordinary_fraction).
        /// </summary>
        /// <param name="str">Строка.</param>
        public static implicit operator ordinary_fraction(string str)
        {
            //вспомогательный массив(определяем верхнее число и нижнее)
            string[] temp = str.Split('/');

            //если это дробное число
            if (temp.Length == 2)
            {
                //новые числитель и знаменатель
                int top_new_number = Int32.Parse(temp[0]);
                int bottom_new_number = Int32.Parse(temp[1]);

                //наибольший общий делить
                int nod = GreatestCommonDivisor(top_new_number, bottom_new_number);

                //сокращаем и числитель, и знаменатель
                top_new_number /= nod;
                bottom_new_number /= nod;

                return new ordinary_fraction { fraction = top_new_number+"/"+ bottom_new_number, top_number = top_new_number, bottom_number = bottom_new_number };
            }
            //иначе это целое число
            else
                return new ordinary_fraction { fraction = temp[0] + "/1", top_number = Int32.Parse(temp[0]), bottom_number = 1 };
        }

        /// <summary>
        /// Деление.
        /// </summary>
        public static ordinary_fraction operator /(ordinary_fraction of1, ordinary_fraction of2)//не обрабатывается деление на ноль
        {
            //конечный знак результата плюс
            sbyte sign=1;
            //если оба числа отрицательны, то теперь они положительны
            if (of1.top_number < 0 && of2.top_number < 0)
            {
                //теперь они положительны
                of1.top_number *= (-1);
                of2.top_number *= (-1);
            }
            //если первое число отрицательно, а второе положительно
            else if(of1.top_number<0 && of2.top_number>0)
            {
                //теперь первое число положительно(необходимо для вычисления)
                of1.top_number *= (-1);
                //конечный знак результата минус
                sign = -1;
            }
            //если первое число положительно, а второе отрицательно
            else if (of1.top_number >= 0 && of2.top_number < 0)
            {
                //теперь второе число положительно(необходимо для вычисления)
                of2.top_number *= (-1);
                //конечный знак результата минус
                sign = -1;
            }

            //вычисления согласно оператору
            int top_new_number = of1.top_number * of2.bottom_number;
            int bottom_new_number = of1.bottom_number * of2.top_number;

            //наибольший общий делить
            int nod = GreatestCommonDivisor(top_new_number, bottom_new_number);

            //сокращаем и числитель, и знаменатель
            top_new_number /= nod;
            bottom_new_number /= nod;

            return new ordinary_fraction { fraction = sign*top_new_number + "/" + bottom_new_number, top_number = sign*top_new_number, bottom_number = bottom_new_number};
        }

        /// <summary>
        /// Умножение.
        /// </summary>
        public static ordinary_fraction operator *(ordinary_fraction of1, ordinary_fraction of2)
        {
            sbyte sign = 1;
            //если оба числа отрицательны, то теперь они положительны
            if (of1.top_number < 0 && of2.top_number < 0)
            {
                //теперь они положительны
                of1.top_number *= (-1);
                of2.top_number *= (-1);
            }
            //если первое число отрицательно, а второе положительно
            else if (of1.top_number < 0 && of2.top_number >= 0)
            {
                //теперь первое число положительно(необходимо для вычисления)
                of1.top_number *= (-1);
                //конечный знак результата минус
                sign = -1;
            }
            //если первое число положительно, а второе отрицательно
            else if (of1.top_number >= 0 && of2.top_number < 0)
            {
                //теперь второе число положительно(необходимо для вычисления)
                of2.top_number *= (-1);
                //конечный знак результата минус
                sign = -1;
            }

            //вычисления согласно оператору
            int top_new_number = of1.top_number * of2.top_number;
            int bottom_new_number = of1.bottom_number * of2.bottom_number;

            //наибольший общий делить
            int nod = GreatestCommonDivisor(top_new_number, bottom_new_number);

            //сокращаем и числитель, и знаменатель
            top_new_number /= nod;
            bottom_new_number /= nod;

            return new ordinary_fraction { fraction = sign*top_new_number + "/" + bottom_new_number, top_number = sign*top_new_number, bottom_number = bottom_new_number};
        }

        /// <summary>
        /// Вычитание.
        /// </summary>
        /// <param name="of1"></param>
        /// <param name="of2"></param>
        /// <returns></returns>
        public static ordinary_fraction operator -(ordinary_fraction of1, ordinary_fraction of2)
        {
            //вычисления согласно оператору
            int top_new_number = (of1.top_number * of2.bottom_number) - (of2.top_number * of1.bottom_number);
            int bottom_new_number = of1.bottom_number * of2.bottom_number;

            //наибольший общий делить
            int nod = GreatestCommonDivisor(top_new_number, bottom_new_number);

            //сокращаем и числитель, и знаменатель
            top_new_number /= nod;
            bottom_new_number /= nod;

            return new ordinary_fraction { fraction = top_new_number + "/" + bottom_new_number, top_number = top_new_number, bottom_number = bottom_new_number};
        }

        /// <summary>
        /// Наибольший общий делитель.
        /// </summary>
        /// <returns>Наибольший общий делитель.</returns>
        public static int GreatestCommonDivisor(int a, int b)
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

        /// <summary>
        /// Сравнение двух обыкновенных дробей.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;

            ordinary_fraction other = (ordinary_fraction)obj;

            if ((this.top_number==other.top_number) && (this.bottom_number==other.bottom_number))
                return true;
            return false;
        }

        /// <summary>
        /// Возврат значения без вызова метода tostring().
        /// </summary>
        /// <returns>Возвращает значение.</returns>
        public override string ToString()
        {
            if (this.bottom_number != 1)
                return fraction;
            else
                return top_number.ToString();
        }
    }
}
