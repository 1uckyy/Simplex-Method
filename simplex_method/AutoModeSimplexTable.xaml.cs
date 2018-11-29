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

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for AutoModeSimplexTable.xaml
    /// </summary>
    public partial class AutoModeSimplexTable : Window
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
        /// Была ли введена угловая точка(false - нет, true - да).
        /// </summary>
        bool? CornerDot;
        /// <summary>
        /// Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).
        /// </summary>
        int[] variable_visualization;
        /// <summary>
        /// Ищем минимум или максимум(0-минимум,1-максимум).
        /// </summary>
        int MinMax;
        /// <summary>
        /// Коэффициенты целевой функции.
        /// </summary>
        double[] target_function_elements;
        /// <summary>
        /// Симплекс-таблица.
        /// </summary>
        SimplexTable simplextable;
        /// <summary>
        /// Угловая точка соответствующая решению.
        /// </summary>
        Grid corner_dot;


        public AutoModeSimplexTable(List<List<double>> elements, bool? CornerDot, int[] variable_visualization, int number_of_basix, double[] target_function_elements, int MinMax)
        {
            InitializeComponent();
            //переносим заполненный массив с главного окна
            this.elements = elements;
            //Была ли введена угловая точка(false - нет, true - да).
            this.CornerDot = CornerDot;
            //Вспомогательный массив для визуализации переменных(по типу x1, x2, x3 и т.д.).
            this.variable_visualization = variable_visualization;
            //Количество базисных переменных.
            this.number_of_basix = number_of_basix;
            //вычисляем количество свободных переменных
            number_of_free_variables = variable_visualization.Length - number_of_basix;
            //Коэффициенты целевой функции.
            this.target_function_elements = target_function_elements;
            //Ищем минимум или максимум(0-минимум,1-максимум).
            this.MinMax = MinMax;

            //Процесс выполнения.
            Implementation();
        }

        /// <summary>
        /// Выполнение.
        /// </summary>
        private void Implementation()
        {
            //Прямой ход метода Гаусса для приведения к треугольному виду.
            MatrixTransformation.Gauss(elements, CornerDot, variable_visualization);

            //Выражение базисных переменных.
            MatrixTransformation.HoistingMatrix(elements, number_of_basix);

            //создаём сиплекс-таблицу
            simplextable = new SimplexTable(number_of_basix, number_of_free_variables, variable_visualization, elements, target_function_elements, true);
            MainGrid.Children.Add(simplextable);

            int responce;
            int step = 1;

            while(true)
            {
                if((responce=simplextable.ResponseCheck())==0)
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
                else if(responce==1)
                {
                    if (MinMax == 0)
                        labelanswer.Content = "Ответ :" + simplextable.Response() * (-1);
                    else labelanswer.Content = "Ответ :" + simplextable.Response();
                    //добавляем точку
                    corner_dot = simplextable.ResponseCornerDot(step-1);
                    MainGrid.Children.Add(corner_dot);
                    buttonToMainWindow.Visibility = Visibility.Visible;
                    break;
                }
                else if(responce ==-1)
                {
                    labelanswer.Content = "Задача не разрешима!";
                    buttonToMainWindow.Visibility = Visibility.Visible;
                    break;
                }
            }
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