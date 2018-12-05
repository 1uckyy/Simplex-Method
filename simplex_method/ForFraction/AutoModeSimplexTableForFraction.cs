﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MatrixLogicLibrary;
using OrdinaryFractionLibrary;

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for AutoModeSimplexTable.xaml
    /// </summary>
    public partial class AutoModeSimplexTable : Window
    {
        /// <summary>
        /// Матрица коэффициентов (обыкновенных дробей) системы ограничений-равенств.
        /// </summary>
        List<List<ordinary_fraction>> fractions = new List<List<ordinary_fraction>>();
        /// <summary>
        /// Коэффициенты (обыкновенных дробей) целевой функции.
        /// </summary>
        ordinary_fraction[] target_function_fractions;

        public AutoModeSimplexTable(List<List<ordinary_fraction>> fractions, bool? CornerDot, int[] variable_visualization, int number_of_basix, ordinary_fraction[] target_function_fractions, int MinMax)
        {
            InitializeComponent();
            //переносим заполненный массив с главного окна
            this.fractions = fractions;
            //Была ли введена угловая точка(false - нет, true - да).
            this.CornerDot = CornerDot;
            //Вспомогательный массив для визуализации переменных(по типу x1, x2, x3 и т.д.).
            this.variable_visualization = variable_visualization;
            //Количество базисных переменных.
            this.number_of_basix = number_of_basix;
            //вычисляем количество свободных переменных
            number_of_free_variables = variable_visualization.Length - number_of_basix;
            //Коэффициенты целевой функции.
            this.target_function_fractions = target_function_fractions;
            //Ищем минимум или максимум(0-минимум,1-максимум).
            this.MinMax = MinMax;

            //Процесс выполнения.
            ImplementationForFractions();
        }

        /// <summary>
        /// Выполнение (для обыкновенных дробей).
        /// </summary>
        private void ImplementationForFractions()
        {
            //Прямой ход метода Гаусса для приведения к треугольному виду.
            MatrixTransformation.Gauss(fractions, CornerDot, variable_visualization);

            //Выражение базисных переменных.
            MatrixTransformation.HoistingMatrix(fractions, number_of_basix);

            //создаём сиплекс-таблицу
            simplextable = new SimplexTable(number_of_basix, number_of_free_variables, variable_visualization, fractions, target_function_fractions, true);
            MainGrid.Children.Add(simplextable);

            int responce;
            int step = 1;

            while (true)
            {
                if ((responce = simplextable.ResponseCheck()) == 0)
                {
                    //выбор любого опорного
                    simplextable.SelectionRandomSupportElement();
                    //смена визуализации переменных в симплек-таблице
                    simplextable.ChangeOfVisualWithoutBuffer();
                    //вычисление согласно выбранному опорному элементу
                    simplextable.CalculateSimplexTable();
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    //номер угловой точки
                    simplextable.CornerPoint(step);
                    step++;
                }
                else if (responce == 1)
                {
                    if (MinMax == 0)
                        labelanswer.Content = "Ответ :-" + simplextable.Response();
                    else labelanswer.Content = "Ответ :" + simplextable.Response();
                    //добавляем точку
                    corner_dot = simplextable.ResponseCornerDot(step - 1);
                    MainGrid.Children.Add(corner_dot);
                    buttonToMainWindow.Visibility = Visibility.Visible;
                    break;
                }
                else if (responce == -1)
                {
                    labelanswer.Content = "Задача не разрешима!";
                    buttonToMainWindow.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
    }
}
