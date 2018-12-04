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

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for AutoModeArtificialBasis.xaml
    /// </summary>
    public partial class AutoModeArtificialBasis : Window
    {
        /// <summary>
        /// Матрица коэффициентов системы ограничений-равенств.
        /// </summary>
        List<List<double>> elements = new List<List<double>>();
        /// <summary>
        /// Количество базисных переменных.
        /// </summary>
        int number_of_basix;
        /// <summary>
        /// Количество свободных переменных.
        /// </summary>
        int number_of_free_variables;
        /// <summary>
        /// Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).
        /// </summary>
        int[] variable_visualization;
        /// <summary>
        /// Коэффициенты целевой функции.
        /// </summary>
        double[] target_function_elements;
        /// <summary>
        /// Симплекс-таблица для искусственного базиса.
        /// </summary>
        SimplexTable simplextable;
        /// <summary>
        /// Симплекс-таблица для обчных проходок.
        /// </summary>
        SimplexTable simplextable1;
        public static int step = 1;
        /// <summary>
        /// Ищем минимум или максимум(0-минимум,1-максимум).
        /// </summary>
        int MinMax;
        /// <summary>
        /// Угловая точка соответствующая решению.
        /// </summary>
        Grid corner_dot = new Grid();
        public AutoModeArtificialBasis(List<List<double>> elements, int number_of_basix, int[] variable_visualization, double[] target_function_elements, int MinMax)
        {
            InitializeComponent();

            //Матрица коэффициентов системы ограничений-равенств.
            this.elements = elements;
            //Количество базисных переменных.
            this.number_of_basix = number_of_basix;
            //Количество свободных переменных.
            this.number_of_free_variables = variable_visualization.Length - number_of_basix;
            //Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).
            this.variable_visualization = variable_visualization;
            //Коэффициенты целевой функции.
            this.target_function_elements = target_function_elements;
            this.MinMax = MinMax;

            //Процесс выполнения.
            Implementation();
        }

        private void Implementation()
        {
            //создаём сиплекс-таблицу
            simplextable = new SimplexTable(number_of_basix, number_of_free_variables, variable_visualization, elements, target_function_elements, false);
            MainGrid.Children.Add(simplextable);
            //добавляем тильду
            simplextable.AddTilde();

            while(simplextable.ArtificialResponseCheck()!=true)
            {
                if (simplextable.ResponseCheck() == 1)
                {
                    //холостой шаг
                    simplextable.RandomIdleStep();
                    this.Width = 720;
                    //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                    simplextable.ChangeOfVisualizationVariables();
                    //вычисление согласно выбранному опорному элементу
                    simplextable.CalculateSimplexTable();
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    simplextable.CornerPoint(step);
                }
                else
                {
                    //выбор опорного
                    simplextable.SelectionRandomSupportElement();
                    this.Width = 651;
                    //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                    simplextable.ChangeOfVisualizationVariables();
                    //вычисление согласно выбранному опорному элементу
                    simplextable.CalculateSimplexTable();
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    simplextable.CornerPoint(step);
                }
            }
    
            variable_visualization = simplextable.ReturnVariableVisualization();
            elements = simplextable.ReturnElements();
            elements.RemoveAt(elements.Count - 1);

            //организация массива для симплекс-метода
            List<List<double>> temp_elements = new List<List<double>>();
            for (int i = 0; i < number_of_basix; i++)
            {
                temp_elements.Add(new List<double>());
                for (int j = 0; j < number_of_basix; j++)
                {
                    if (temp_elements[i].Count == i)
                        temp_elements[i].Add(1);
                    else temp_elements[i].Add(0);
                }
            }
            for (int i = 0; i < number_of_basix; i++)
            {
                temp_elements[i].AddRange(elements[i]);
            }

            elements = temp_elements;

            simplextable.HideSimplexTable();
            simplextable1 = new SimplexTable(number_of_basix, variable_visualization.Length - number_of_basix, variable_visualization, elements, target_function_elements, true);
            MainGrid.Children.Add(simplextable1);

            step = 0;

            while(simplextable1.ResponseCheck()!=1 && simplextable1.ResponseCheck() != -1)
            {
                //выбор опорного
                simplextable1.SelectionRandomSupportElement();
                //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                simplextable1.ChangeOfVisualizationVariables();
                //вычисление согласно выбранному опорному элементу
                simplextable1.CalculateSimplexTable();
                //обновление данных сиплекс-таблицы
                simplextable1.UpdateSimplexTableValues();
            }

            if (simplextable1.ResponseCheck() == 1)
            {
                if (MinMax == 0)
                    labelanswer.Content = "Ответ :-" + simplextable1.Response();
                else labelanswer.Content = "Ответ :" + simplextable1.Response();
                //добавляем точку
                corner_dot = simplextable1.ResponseCornerDot(step);
                MainGrid.Children.Add(corner_dot);
                buttonToMainWindow.Visibility = Visibility.Visible;
            }
            else
            {
                labelanswer.Content = "Задача не разрешима!";
                buttonToMainWindow.Visibility = Visibility.Visible;
            }

            step++;
        }

        private void buttonToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            //создаём экземпляр главного окна
            MainWindow MW = new MainWindow();
            //открываем главное окно
            MW.Show();
            //закрываем это окно
            this.Close();
        }
    }
}
