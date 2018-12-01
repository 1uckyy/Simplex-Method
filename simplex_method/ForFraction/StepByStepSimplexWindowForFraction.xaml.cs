using System;
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
    /// Логика для пошагового симплекс-метода.
    /// </summary>
    public partial class StepByStepSimplexWindow : Window
    {
        /// <summary>
        /// Матрица коэффициентов (обыкновенных дробей) системы ограничений-равенств.
        /// </summary>
        List<List<ordinary_fraction>> fractions = new List<List<ordinary_fraction>>();
        /// <summary>
        /// Коэффициенты (обыкновенных дробей целевой функции.
        /// </summary>
        ordinary_fraction[] target_function_fractions;
        /// <summary>
        /// Десятичные(true) или обыкновенные(false) дроби.
        /// </summary>
        bool decimals_or_simple = true;

        /// <summary>
        /// Конструктор для окна выполнения пошагового симплекс-метода.
        /// </summary>
        /// <param name="fractions">Матрица коэффициентов (обыкновенных дробей) системы ограничений-равенств.</param>
        /// <param name="selected_number_of_rows">Выбранное число строк.</param>
        /// <param name="selected_number_of_columns">Выбранное число столбцов.</param>
        /// <param name="variable_visualization">Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).</param>
        /// <param name="number_of_permutations">Количество базисных переменных.</param>
        /// <param name="target_function_elements">Коэффициенты целевой функции.</param>
        public StepByStepSimplexWindow(List<List<ordinary_fraction>> fractions, int selected_number_of_rows, int selected_number_of_columns, int[] variable_visualization, int number_of_permutations, ordinary_fraction[] target_function_fractions, int MinMax, bool? CornerDot)
        {
            InitializeComponent();
            //переносим заполненный массив с главного окна
            this.fractions = fractions;
            //изначально мы на нулевом шаге
            step = 0;
            //Количество базисных переменных.
            this.number_of_permutations = number_of_permutations;
            //вычисляем количество свободных переменных
            number_of_free_variables = variable_visualization.Length - number_of_permutations;
            //Вспомогательный массив для визуализации переменных(по типу x1, x2, x3 и т.д.).
            this.variable_visualization = variable_visualization;
            //Вспомогательная переменная, которая определяет была ли создана симплекс-таблица. True - была создана. False - не была создана.
            simplex_table_was_draw = false;
            //Коэффициенты целевой функции.
            this.target_function_fractions = target_function_fractions;
            //Задача на минимум или максимум(0 - минимум, 1 - максимум)
            this.MinMax = MinMax;
            //Была ли введена угловая точка(false - нет, true - да).
            this.CornerDot = CornerDot;
            //настройка количества строк и столбцов матрицы
            SimplexTable.SettingMatrix(selected_number_of_rows, selected_number_of_columns, gaussgrid);
            //для обыкновенных дробей
            decimals_or_simple = false;
            //отрисовка закруглённых скобок матрицы и вертикальной черты, отделяющей столбец свободных членов
            DrawExtendedMatrix();
            //добавляем лейблы с визуализацией переменных(по типу x1, x2, x3 и т.д.)
            VariableVisualization(variable_visualization);
            //заполнение таблицы числами
            FillingtheTableWithNumbers();
        }
    }
}
