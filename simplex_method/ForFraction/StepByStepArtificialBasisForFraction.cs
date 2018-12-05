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
using OrdinaryFractionLibrary;

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for StepByStepArtificialBasisWindow.xaml
    /// </summary>
    public partial class StepByStepArtificialBasisWindow : Window
    {
        /// <summary>
        /// Матрица коэффициентов (обык. дробей) системы ограничений-равенств.
        /// </summary>
        List<List<ordinary_fraction>> fractions = new List<List<ordinary_fraction>>();
        /// <summary>
        /// Коэффициенты (обык. дробей) целевой функции.
        /// </summary>
        ordinary_fraction[] target_function_fractions;

        public StepByStepArtificialBasisWindow(List<List<ordinary_fraction>> fractions, int number_of_basix, int[] variable_visualization, ordinary_fraction[] target_function_fractions, int MinMax)
        {
            InitializeComponent();
            //Матрица коэффициентов системы ограничений-равенств.
            this.fractions = fractions;
            //Количество базисных переменных.
            this.number_of_basix = number_of_basix;
            //Количество свободных переменных.
            this.number_of_free_variables = variable_visualization.Length - number_of_basix;
            //Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).
            this.variable_visualization = variable_visualization;
            //Коэффициенты целевой функции.
            this.target_function_fractions = target_function_fractions;
            step = 1;
            step_1 = 0;
            memory = false;
            simplex_table_was_draw = false;
            this.MinMax = MinMax;

            //Процесс выполнения.
            Implementation();
        }


    }
}
