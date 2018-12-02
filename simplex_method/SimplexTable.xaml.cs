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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for SimplexTable.xaml
    /// </summary>
    public partial class SimplexTable : UserControl
    {
        int number_of_permutations;
        int number_of_free_variables;
        int[] variable_visualization;
        List<List<int>> buffer_variable_visualization = new List<List<int>>();
        /// <summary>
        /// Матрица коэффициентов системы ограничений-равенств.
        /// </summary>
        List<List<double>> elements = new List<List<double>>();
        /// <summary>
        /// Коэффициенты симплекс-таблицы.
        /// </summary>
        List<List<double>> simplex_elements = new List<List<double>>();
        /// <summary>
        /// Коэффициенты целевой функции.
        /// </summary>
        double[] target_function_elements;
        /// <summary>
        /// Буфер для коэффициентов симплекс-таблицы.
        /// </summary>
        List<List<List<double>>> buffer_simplex_elements = new List<List<List<double>>>();
        /// <summary>
        /// Кнопки выбора опорного элемента
        /// </summary>
        List<Button> buttons;
        /// <summary>
        /// Опорный элемент.
        /// </summary>
        double supporting_member;
        /// <summary>
        /// Возможные координаты опорного элемента.
        /// </summary>
        List<List<int>> the_coordinates_of_the_support_element;
        /// <summary>
        /// Разрешающая строка.
        /// </summary>
        int row_of_the_support_element;
        /// <summary>
        /// Разрешающий столбец.
        /// </summary>
        int column_of_the_support_element;
        /// <summary>
        /// Симплекс-метод(true) или метод искусственного базиса(false).
        /// </summary>
        bool? simplex_or_artificial;
        /// <summary>
        /// Десятичные(true) или обыкновенные(false) дроби.
        /// </summary>
        bool? decimal_or_simple=false;//передавать значение после конца тестирования


        public SimplexTable(int number_of_permutations, int number_of_free_variables, int[] variable_visualization, List<List<double>> elements, double[] target_function_elements, bool? simplex_or_artificial)
        {
            InitializeComponent();
            this.number_of_permutations = number_of_permutations;
            this.number_of_free_variables = number_of_free_variables;
            this.variable_visualization = variable_visualization;
            this.elements = elements;
            this.target_function_elements = target_function_elements;
            this.simplex_or_artificial = simplex_or_artificial;
            DrawSimplexTable();
        }

        /// <summary>
        /// Настройка количества строк и столбцов матрицы.
        /// </summary>
        /// <param name="selected_number_of_rows">Нужное количество строк.</param>
        /// <param name="selected_number_of_columns">Нужное количество столбцов.</param>
        /// <param name="grid">Преобразуемый grid.</param>
        public static void SettingMatrix(int selected_number_of_rows, int selected_number_of_columns, Grid grid)
        {
            //узнаём количество строк в изменяемом grid'е
            int this_count_of_rows = grid.RowDefinitions.Count;

            //удаляем все строки в изменяемом grid'е
            for (int i = this_count_of_rows - 1; i > -1; i--)
                grid.RowDefinitions.RemoveAt(i);

            //узнаём количество столбцов в изменяемом grid'е
            int this_count_of_columns = grid.ColumnDefinitions.Count;

            //удаляем все столбцы в изменяемом grid'е
            for (int i = this_count_of_columns - 1; i > -1; i--)
                grid.ColumnDefinitions.RemoveAt(i);

            //добавляем в изменяемый grid нужное число столбцов
            for (int i = 0; i < selected_number_of_columns; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(column);
            }

            //добавляем в изменяемый grid нужное число строк
            for (int i = 0; i < selected_number_of_rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid.RowDefinitions.Add(row);
            }
        }

        /// <summary>
        /// Смена номера угловой точки.
        /// </summary>
        /// <param name="number">Номер угловой точки.</param>
        public void CornerPoint(int number)
        {
            //возвращаем видимость label
            Label lbl = (Label)simplextablegrid.FindName("simplexlabel0_0");
            lbl.Content = "(" + number + ")";
        }

        /// <summary>
        /// Удаление ненужного столбца.(отказ от метода)
        /// </summary>
        public void DeleteColumn()
        {
            //удаляем столбец
            for (int i = number_of_permutations; i < variable_visualization.Length; i++)
            {
                if (variable_visualization[i] > number_of_free_variables)
                {
                    //удаляем в массиве
                    for (int k = 0; k < simplex_elements.Count; k++)
                    {
                        simplex_elements[k].RemoveAt(0);
                    }

                    //удаляем визуализацию
                    int[] temp = new int[variable_visualization.Length - 1]; //временный массив
                    for (int j = 0; j < variable_visualization.Length; j++)
                    {
                        if (j < i)
                        {
                            temp[j] = variable_visualization[j];
                        }
                        else if (j > i)
                        {
                            temp[j - 1] = variable_visualization[j];
                        }
                    }
                    variable_visualization = new int[temp.Length];
                    for (int j = 0; j < temp.Length; j++)
                        variable_visualization[j] = temp[j];
                    //simplextablegrid.ColumnDefinitions.RemoveAt(i-number_of_permutations);
                    //свободных переменных теперь меньше
                    number_of_free_variables--;
                    //выход
                    break;
                }
            }
        }

        /// <summary>
        /// Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
        /// </summary>
        public void ChangeOfVisualizationVariables()
        {
            int temp;//вспомогательная переменная

            //меняем в столбце номер переменной
            Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + (row_of_the_support_element + 1) + "_0");
            lbl.Content = "x" + variable_visualization[number_of_permutations + column_of_the_support_element];

            //меняем в строке номер переменной
            Label lbl1 = (Label)simplextablegrid.FindName("simplexlabel0_" + (column_of_the_support_element + 1));
            lbl1.Content = "x" + variable_visualization[row_of_the_support_element];

            //буферизация массива визуализации до изменения
            buffer_variable_visualization.Add(new List<int>());
            for (int i = 0; i < variable_visualization.Length; i++)
                buffer_variable_visualization[buffer_variable_visualization.Count - 1].Add(variable_visualization[i]);

            //в массиве также меняем
            temp = variable_visualization[row_of_the_support_element];
            variable_visualization[row_of_the_support_element] = variable_visualization[number_of_permutations + column_of_the_support_element];
            variable_visualization[number_of_permutations + column_of_the_support_element] = temp;
        }

        /// <summary>
        /// Смена местами визуализаций переменных(после выбора опорного элемента), без буферизации.
        /// </summary>
        public void ChangeOfVisualWithoutBuffer()
        {
            int temp;//вспомогательная переменная

            //меняем в столбце номер переменной
            Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + (row_of_the_support_element + 1) + "_0");
            lbl.Content = "x" + variable_visualization[number_of_permutations + column_of_the_support_element];

            //меняем в строке номер переменной
            Label lbl1 = (Label)simplextablegrid.FindName("simplexlabel0_" + (column_of_the_support_element + 1));
            lbl1.Content = "x" + variable_visualization[row_of_the_support_element];

            //в массиве также меняем
            temp = variable_visualization[row_of_the_support_element];
            variable_visualization[row_of_the_support_element] = variable_visualization[number_of_permutations + column_of_the_support_element];
            variable_visualization[number_of_permutations + column_of_the_support_element] = temp;
        }

        /// <summary>
        /// Возврат элементов визуализации из буфера.
        /// </summary>
        public void GetOutOfTheBufferVisualizationVariables(int step)
        {
            for (int j = 0; j < buffer_variable_visualization[step - 5].Count; j++)
                variable_visualization[j] = buffer_variable_visualization[step - 5][j];
            buffer_variable_visualization.RemoveAt(step - 5);

            //возвращаем в столбце номер переменной
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                lbl.Content = "x" + variable_visualization[i - 1];
            }

            //возвращаем в строке номер переменной
            for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count - 1; j++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel0_" + j);
                lbl.Content = "x" + variable_visualization[(number_of_permutations - 1) + j];
            }
        }

        /// <summary>
        /// Возврат элементов визуализации из буфера.
        /// </summary>
        public void GetOutOfTheBufferVisualizationVariablesTest()
        {
            variable_visualization = new int[buffer_variable_visualization[0].Count];
            for (int j = 0; j < buffer_variable_visualization[buffer_variable_visualization.Count - 1].Count; j++)
                variable_visualization[j] = buffer_variable_visualization[buffer_variable_visualization.Count - 1][j];
            buffer_variable_visualization.RemoveAt(buffer_variable_visualization.Count - 1);

            //возвращаем в столбце номер переменной
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                lbl.Content = "x" + variable_visualization[i - 1];
            }

            //возвращаем в строке номер переменной
            for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count - 1; j++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel0_" + j);
                lbl.Content = "x" + variable_visualization[(number_of_permutations - 1) + j];
            }
        }

        /// <summary>
        /// Отрисовка сиплекс-таблицы.
        /// </summary>
        private void DrawSimplexTable()
        {
            DrawTable();

            //Заполнение сиплекс-таблицы числами.
            FillingtheSimplexTableWithNumbers();
        }

        /// <summary>
        /// Отрисовка 2.
        /// </summary>
        private void DrawTable()
        {
            //настройка количества строк и столбцов матрицы
            SettingMatrix(number_of_permutations + 2, number_of_free_variables + 2, simplextablegrid);

            double width = 0; //пересчёт ширины
            double height = 0; //пересчёт высоты

            //надстрочный символ
            Label variable2 = new Label();
            variable2.Content = "(0)";
            variable2.FontSize = 7.5;
            variable2.Width = 20;
            variable2.Height = 20;
            variable2.Margin = new Thickness(4, -4, 5, 9);
            variable2.Name = "simplexlabel" + 0 + "_" + 0;
            //регистрируем имя
            RegisterName(variable2.Name, variable2);
            Grid.SetColumn(variable2, 0);
            Grid.SetRow(variable2, 0);
            simplextablegrid.Children.Add(variable2);

            //X
            Label variable = new Label();
            variable.Content = "X";
            width += 55;
            variable.Width = 35;
            height += 30;
            variable.Height = 30;
            Grid.SetColumn(variable, 0);
            Grid.SetRow(variable, 0);
            simplextablegrid.Children.Add(variable);

            //добавляем лейблы с визуализацией СВОБОДНЫХ переменных(по типу x1, x2, x3 и т.д.)
            for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count - 1; j++)
            {
                Label variable1 = new Label();
                variable1.Width = 55;
                width += 55;
                variable1.Height = 30;
                variable1.Content = "x" + variable_visualization[(number_of_permutations - 1) + j];
                variable1.HorizontalContentAlignment = HorizontalAlignment.Center;
                variable1.Name = "simplexlabel0_" + j;
                //регистрируем имя
                RegisterName(variable1.Name, variable1);
                Grid.SetColumn(variable1, j);
                Grid.SetRow(variable1, 0);
                simplextablegrid.Children.Add(variable1);
            }
            height += 30;

            //добавляем лейблы с визуализацией БАЗИСНЫХ переменных(по типу x1, x2, x3 и т.д.)
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label variable1 = new Label();
                variable1.Width = 55;
                variable1.Height = 30;
                height += 30;
                variable1.Content = "x" + variable_visualization[i - 1];
                variable1.Name = "simplexlabel" + i + "_0";
                //регистрируем имя
                RegisterName(variable1.Name, variable1);
                Grid.SetColumn(variable1, 0);
                Grid.SetRow(variable1, i);
                simplextablegrid.Children.Add(variable1);
            }
            width += 55;

            simplextablegrid.Width = width;
            simplextablegrid.Height = height;

            //отрисовка вертикальных черт
            for (int i = 0; i < simplextablegrid.RowDefinitions.Count; i++)
            {
                for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    Line myLine = new Line();
                    myLine.Stroke = Brushes.Black;
                    myLine.X1 = 0;
                    myLine.X2 = 0;
                    myLine.Y1 = 0;
                    myLine.Y2 = 30;
                    Grid.SetColumn(myLine, j);
                    Grid.SetRow(myLine, i);
                    simplextablegrid.Children.Add(myLine);
                }
            }

            //отрисовка горизонтальных черт
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    Line myLine = new Line();
                    myLine.Stroke = Brushes.Black;
                    myLine.X1 = 0;
                    myLine.X2 = 55;
                    myLine.Y1 = 0;
                    myLine.Y2 = 0;
                    Grid.SetColumn(myLine, j);
                    Grid.SetRow(myLine, i);
                    simplextablegrid.Children.Add(myLine);
                }
            }
        }

        /// <summary>
        /// Добавление знака тильды.
        /// </summary>
        public void AddTilde()
        {
            Label variable2 = new Label();
            variable2.Content = "~";
            //variable2.FontSize = 7.5;
            variable2.Width = 20;
            variable2.Height = 20;
            variable2.Margin = new Thickness(-11, -16, 5, 9);
            Grid.SetColumn(variable2, 0);
            Grid.SetRow(variable2, 0);
            simplextablegrid.Children.Add(variable2);
        }

        /// <summary>
        /// Заполнение сиплекс-таблицы числами.
        /// </summary>
        private void FillingtheSimplexTableWithNumbers()
        {
            //заполняем основное поле
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                //новая строка рабочего массива
                simplex_elements.Add(new List<double>());
                for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    Label variable = new Label();
                    //для десятичных
                    if (decimal_or_simple == true)
                    {
                        if (simplex_or_artificial == true)
                        {
                            variable.Content = Math.Round(elements[i - 1][(number_of_permutations - 1) + j], 2);
                            //добавляем число в рабочий массив
                            simplex_elements[i - 1].Add(elements[i - 1][(number_of_permutations - 1) + j]);
                        }
                        else
                        {
                            variable.Content = Math.Round(elements[i - 1][j - 1], 2);
                            //добавляем число в рабочий массив
                            simplex_elements[i - 1].Add(elements[i - 1][j - 1]);
                        }
                    }
                    //для обыкновенных
                    else
                    {
                        if (simplex_or_artificial == true)
                        {
                            variable.Content = DoubleToFraction.Convert(elements[i - 1][(number_of_permutations - 1) + j]);
                            //добавляем число в рабочий массив
                            simplex_elements[i - 1].Add(elements[i - 1][(number_of_permutations - 1) + j]);
                        }
                        else
                        {
                            variable.Content = DoubleToFraction.Convert(elements[i - 1][j - 1]);
                            //добавляем число в рабочий массив
                            simplex_elements[i - 1].Add(elements[i - 1][j - 1]);
                        }
                    }
                    variable.Width = 55;
                    variable.Height = 30;
                    variable.HorizontalContentAlignment = HorizontalAlignment.Center;
                    variable.Name = "simplexlabel" + i + "_" + j;
                    //регистрируем имя
                    RegisterName(variable.Name, variable);
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, i);
                    simplextablegrid.Children.Add(variable);
                }
            }

            //для сиплекс метода
            if (simplex_or_artificial == true)
            {
                //новая строка рабочего массива
                simplex_elements.Add(new List<double>());
                //коэффициенты последней строки
                double a;
                //счёт столбца
                int column_index = 1;
                for (int j = number_of_permutations; j < elements[0].Count - 1; j++)
                {
                    //логика
                    a = 0;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        a += elements[i][j] * (-1) * target_function_elements[variable_visualization[i] - 1];
                    }
                    a += target_function_elements[variable_visualization[j] - 1];

                    //отображение
                    Label variable1 = new Label();
                    //для десятичных
                    if (decimal_or_simple == true)
                        variable1.Content = Math.Round(a, 2);
                    //для обыкновенных
                    else
                        variable1.Content = DoubleToFraction.Convert(a);
                    //добавляем число в рабочий массив
                    simplex_elements[simplex_elements.Count - 1].Add(a);
                    variable1.Width = 55;
                    variable1.Height = 30;
                    variable1.HorizontalContentAlignment = HorizontalAlignment.Center;
                    variable1.Name = "simplexlabel" + (simplextablegrid.RowDefinitions.Count - 1) + "_" + column_index;
                    //регистрируем имя
                    RegisterName(variable1.Name, variable1);
                    Grid.SetColumn(variable1, column_index);
                    Grid.SetRow(variable1, simplextablegrid.RowDefinitions.Count - 1);
                    simplextablegrid.Children.Add(variable1);
                    column_index++;
                }


                //коэффициент в нижнем правом углу симплекс таблицы
                a = 0;
                for (int i = 0; i < elements.Count; i++)
                    a += elements[i][elements[0].Count - 1] * target_function_elements[variable_visualization[i] - 1];
                Label variable2 = new Label();
                //для десятичных
                if (decimal_or_simple == true)
                    variable2.Content = Math.Round(a, 2) * (-1);
                //для обыкновенных
                else
                    variable2.Content = DoubleToFraction.Convert(a * (-1));
                //добавляем число в рабочий массив
                simplex_elements[simplex_elements.Count - 1].Add(a*(-1));
                variable2.Width = 55;
                variable2.Height = 30;
                variable2.HorizontalContentAlignment = HorizontalAlignment.Center;
                variable2.Name = "simplexlabel" + (simplextablegrid.RowDefinitions.Count - 1) + "_" + (simplextablegrid.ColumnDefinitions.Count - 1);
                //регистрируем имя
                RegisterName(variable2.Name, variable2);
                Grid.SetColumn(variable2, simplextablegrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(variable2, simplextablegrid.RowDefinitions.Count - 1);
                simplextablegrid.Children.Add(variable2);

                ////заполняем рабочий массив
                //for (int i = 1; i < simplextablegrid.RowDefinitions.Count; i++)
                //{
                //    simplex_elements.Add(new List<double>());
                //    for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                //    {
                //        //находим label
                //        Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + j);
                //        //добавляем в массив число
                //        simplex_elements[i - 1].Add(double.Parse(lbl.Content.ToString()));
                //    }
                //}
            }
            //для искусственного базиса
            else
            {
                //новая строка рабочего массива
                simplex_elements.Add(new List<double>());
                //коэффициенты последней строки
                double a;
                for (int j = 0; j < elements[0].Count; j++)
                {
                    //логика
                    a = 0;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        a += elements[i][j];
                    }

                    //отображение
                    Label variable1 = new Label();
                    //для десятичных
                    if (decimal_or_simple == true)
                        variable1.Content = Math.Round(a * (-1), 2);
                    //для обыкновенных
                    else
                        variable1.Content = DoubleToFraction.Convert(a * (-1));
                    //добавляем число в рабочий массив
                    simplex_elements[simplex_elements.Count - 1].Add(a*(-1));
                    variable1.Width = 55;
                    variable1.Height = 30;
                    variable1.HorizontalContentAlignment = HorizontalAlignment.Center;
                    variable1.Name = "simplexlabel" + (simplextablegrid.RowDefinitions.Count - 1) + "_" + (j + 1);
                    //регистрируем имя
                    RegisterName(variable1.Name, variable1);
                    Grid.SetColumn(variable1, j + 1);
                    Grid.SetRow(variable1, simplextablegrid.RowDefinitions.Count - 1);
                    simplextablegrid.Children.Add(variable1);
                }

                ////заполняем рабочий массив
                //for (int i = 1; i < simplextablegrid.RowDefinitions.Count; i++)
                //{
                //    simplex_elements.Add(new List<double>());
                //    for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                //    {
                //        //находим label
                //        Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + j);
                //        //добавляем в массив число
                //        simplex_elements[i - 1].Add(double.Parse(lbl.Content.ToString()));
                //    }
                //}
            }
        }

        /// <summary>
        /// Буферизация элементов симплекс-таблицы.
        /// </summary>
        public void BufferingSimplexTableValues(int step)
        {
            buffer_simplex_elements.Add(new List<List<double>>());
            for (int i = 0; i < simplex_elements.Count; i++)
            {
                buffer_simplex_elements[step - 4].Add(new List<double>());
                for (int j = 0; j < simplex_elements[0].Count; j++)
                    buffer_simplex_elements[step - 4][i].Add(simplex_elements[i][j]);
            }
        }

        /// <summary>
        /// Буферизация элементов симплекс-таблицы.
        /// </summary>
        public void BufferingSimplexTableValuesTest()
        {
            buffer_simplex_elements.Add(new List<List<double>>());
            for (int i = 0; i < simplex_elements.Count; i++)
            {
                buffer_simplex_elements[buffer_simplex_elements.Count - 1].Add(new List<double>());
                for (int j = 0; j < simplex_elements[0].Count; j++)
                    buffer_simplex_elements[buffer_simplex_elements.Count - 1][i].Add(simplex_elements[i][j]);
            }
        }

        /// <summary>
        /// Выбор опорного элемента.
        /// </summary>
        public void SelectionOfTheSupportElement()
        {
            buttons = new List<Button>();
            the_coordinates_of_the_support_element = new List<List<int>>();
            int index = 0; //для счёта координат

            //координаты минимума в столбце
            int[] minimum = new int[2];

            //ищем отрицательный элемент в последней строке
            for (int j = 0; j < simplex_elements[0].Count - 1; j++)
            {
                if (simplex_elements[simplex_elements.Count - 1][j] < 0)
                {
                    minimum[0] = -1;
                    minimum[1] = -1;
                    //ищем подходящий не отрицательный элемент в столбце
                    for (int i = 0; i < simplex_elements.Count - 1; i++)
                    {
                        if (simplex_elements[i][j] > 0)
                        {
                            if ((minimum[0] == -1) && (minimum[1] == -1))
                            {
                                minimum[0] = i;
                                minimum[1] = j;
                            }
                            else if ((simplex_elements[minimum[0]][simplex_elements[0].Count - 1] / simplex_elements[minimum[0]][minimum[1]]) > (simplex_elements[i][simplex_elements[0].Count - 1] / simplex_elements[i][j]))
                            {
                                minimum[0] = i;
                                minimum[1] = j;
                            }
                        }
                    }
                    //если есть минимальный, то делаем его кнопкой
                    if ((minimum[0] != -1) && (minimum[1] != -1))
                    {
                        //находим label
                        Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + (minimum[0] + 1) + "_" + (minimum[1] + 1));
                        lbl.Visibility = Visibility.Hidden;

                        Button btn = new Button();
                        btn.Content = lbl.Content;
                        btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                        btn.Background = Brushes.Transparent;
                        btn.BorderThickness = new Thickness(3, 3, 3, 3);
                        btn.BorderBrush = Brushes.Red;
                        //click
                        btn.Click += new RoutedEventHandler(btn_Click);
                        btn.Name = "simplexbutton" + minimum[0] + "_" + minimum[1];
                        //регистрируем имя
                        RegisterName(btn.Name, btn);
                        Grid.SetColumn(btn, minimum[1] + 1);
                        Grid.SetRow(btn, minimum[0] + 1);
                        simplextablegrid.Children.Add(btn);
                        buttons.Add(btn);

                        //координаты возможного опорного элемента
                        the_coordinates_of_the_support_element.Add(new List<int>());
                        the_coordinates_of_the_support_element[index].Add(minimum[0]);
                        the_coordinates_of_the_support_element[index].Add(minimum[1]);
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Выбор рандомного опорного элемента.
        /// </summary>
        public void SelectionRandomSupportElement()
        {
            //координаты минимума в столбце
            int[] minimum = new int[2];

            //ищем отрицательный элемент в последней строке
            for (int j = 0; j < simplex_elements[0].Count - 1; j++)
            {
                if (simplex_elements[/*simplextablegrid.RowDefinitions.Count - 2*/simplex_elements.Count - 1][j] < 0)
                {
                    minimum[0] = -1;
                    minimum[1] = -1;
                    //ищем подходящий не отрицательный элемент в столбце
                    for (int i = 0; i < /*simplextablegrid.RowDefinitions.Count - 1*/simplex_elements.Count - 1; i++)
                    {
                        if (simplex_elements[i][j] > 0)
                        {
                            if ((minimum[0] == -1) && (minimum[1] == -1))
                            {
                                minimum[0] = i;
                                minimum[1] = j;
                            }
                            else if ((simplex_elements[minimum[0]][simplex_elements[0].Count - 1] / simplex_elements[minimum[0]][minimum[1]]) > (simplex_elements[i][simplex_elements[0].Count - 1] / simplex_elements[i][j]))
                            {
                                minimum[0] = i;
                                minimum[1] = j;
                            }
                        }
                    }
                    //если есть минимальный элемент, то делаем его опорным
                    if ((minimum[0] != -1) && (minimum[1] != -1))
                    {
                        row_of_the_support_element = minimum[0];
                        column_of_the_support_element = minimum[1];
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Обновление данных симплекс-таблицы.
        /// </summary>
        public void UpdateSimplexTableValues()
        {
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count; i++)
            {
                for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    //находим label
                    Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + j);
                    //для десятичных
                    if (decimal_or_simple == true)
                        lbl.Content = Math.Round(simplex_elements[i - 1][j - 1], 2);
                    else
                        lbl.Content = DoubleToFraction.Convert(simplex_elements[i - 1][j - 1]);
                }
            }
        }

        /// <summary>
        /// Изменение данных при переходе к основной части. (нужен в методе искусственного базиса) + изменение визуализации переменных.
        /// </summary>
        public void UpdateValuesNewStage()
        {
            //заполняем основное поле
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    //находим label
                    Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + j);
                    lbl.Content = Math.Round(elements[i - 1][(number_of_permutations - 1) + j], 2);
                }
            }

            //коэффициенты последней строки
            double a;
            //счёт столбца
            int column_index = 1;
            for (int j = number_of_permutations; j < elements[0].Count - 1; j++)
            {
                //логика
                a = 0;
                for (int i = 0; i < elements.Count; i++)
                {
                    a += elements[i][j] * (-1) * target_function_elements[variable_visualization[i] - 1];
                }
                a += target_function_elements[variable_visualization[j] - 1];

                //находим label
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + (simplextablegrid.RowDefinitions.Count - 1) + "_" + column_index);
                lbl.Content = Math.Round(a, 2);
                column_index++;
            }


            //коэффициент в нижнем правом углу симплекс таблицы
            a = 0;
            for (int i = 0; i < elements.Count; i++)
                a += elements[i][elements[0].Count - 1] * target_function_elements[variable_visualization[i] - 1];
            //находим label
            Label variable2 = (Label)simplextablegrid.FindName("simplexlabel" + (simplextablegrid.RowDefinitions.Count - 1) + "_" + (simplextablegrid.ColumnDefinitions.Count - 1));
            variable2.Content = Math.Round(a, 2) * (-1);

            simplex_elements = new List<List<double>>();

            //заполняем рабочий массив
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count; i++)
            {
                simplex_elements.Add(new List<double>());
                for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    //находим label
                    Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + j);
                    //добавляем в массив число
                    simplex_elements[i - 1].Add(double.Parse(lbl.Content.ToString()));
                }
            }


            //изменение визуализации переменных
            //изменяем лейблы с визуализацией СВОБОДНЫХ переменных(по типу x1, x2, x3 и т.д.)
            for (int j = 1; j < simplextablegrid.ColumnDefinitions.Count - 1; j++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel0_" + j);
                lbl.Content = "x" + variable_visualization[(number_of_permutations - 1) + j];
            }

            //изменяем лейблы с визуализацией БАЗИСНЫХ переменных(по типу x1, x2, x3 и т.д.)
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                lbl.Content = "x" + variable_visualization[i - 1];
            }
        }

        /// <summary>
        /// Возврат элементов таблицы из буфера.
        /// </summary>
        public void GetOutOfTheBufferSimplex(int step)
        {
            for (int i = 0; i < buffer_simplex_elements[step - 5].Count; i++)
                for (int j = 0; j < buffer_simplex_elements[step - 5][0].Count; j++)
                    simplex_elements[i][j] = buffer_simplex_elements[step - 5][i][j];
            buffer_simplex_elements.RemoveAt(step - 5);
        }

        /// <summary>
        /// Возврат элементов таблицы из буфера.
        /// </summary>
        public void GetOutOfTheBufferSimplexTest()
        {
            //for (int i = 0; i < buffer_simplex_elements[buffer_simplex_elements.Count-1].Count; i++)
            //    for (int j = 0; j < buffer_simplex_elements[buffer_simplex_elements.Count - 1][0].Count; j++)
            simplex_elements = buffer_simplex_elements[buffer_simplex_elements.Count - 1];
            buffer_simplex_elements.RemoveAt(buffer_simplex_elements.Count - 1);
        }

        /// <summary>
        /// Удаление кнопок с симплекс-таблицы.
        /// </summary>
        public void DeleteButtons()
        {
            for (int i = 0; i < simplextablegrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < simplextablegrid.ColumnDefinitions.Count; j++)
                {
                    //находим button
                    Button btn = (Button)simplextablegrid.FindName("simplexbutton" + i + "_" + j);
                    if (btn != null)
                    {
                        //разрегистрируем
                        UnregisterName(btn.Name);
                        //удаляем из grid'а
                        simplextablegrid.Children.Remove(btn);

                        //возвращаем видимость label
                        Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + (i + 1) + "_" + (j + 1));
                        lbl.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Логика кнопок.
        /// </summary>
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //у всех красные борты
            for (int i = 0; i < buttons.Count; i++)
                buttons[i].BorderBrush = Brushes.Red;

            Button button = (Button)sender;

            //у нажатой кнопки голубые борты
            button.BorderBrush = Brushes.LightSkyBlue;
        }

        /// <summary>
        /// Проверка выбора опорного элемента.
        /// </summary>
        public void ButtonPressedOrNot()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].BorderBrush == Brushes.LightSkyBlue)
                {
                    row_of_the_support_element = the_coordinates_of_the_support_element[i][0];
                    column_of_the_support_element = the_coordinates_of_the_support_element[i][1];
                    return;
                }
            }
            throw new Exception("Не выбран опорный элемент");
        }

        /// <summary>
        /// Вычисление согласно выбранному опорному элементу.
        /// </summary>
        public void CalculateSimplexTable()
        {
            //записываем значение опорного элемента
            supporting_member = simplex_elements[row_of_the_support_element][column_of_the_support_element];

            //вычисление на месте опорного
            simplex_elements[row_of_the_support_element][column_of_the_support_element] = 1 / supporting_member;

            //вычисление разрешающей строки
            for (int j = 0; j < simplex_elements[0].Count; j++)
            {
                if (j != column_of_the_support_element)
                {
                    simplex_elements[row_of_the_support_element][j] /= supporting_member;
                }
            }

            //вычисление остальных строк сиплекс-таблицы
            for (int i = 0; i < simplex_elements.Count; i++)
            {
                if (i != row_of_the_support_element)
                {
                    for (int j = 0; j < simplex_elements[0].Count; j++)
                    {
                        if (j != column_of_the_support_element)
                        {
                            simplex_elements[i][j] = simplex_elements[i][j] - simplex_elements[row_of_the_support_element][j] * simplex_elements[i][column_of_the_support_element];
                        }
                    }
                }
            }

            //вычисление разрешающего столбца
            for (int i = 0; i < simplex_elements.Count; i++)
            {
                if (i != row_of_the_support_element)
                {
                    simplex_elements[i][column_of_the_support_element] /= supporting_member * (-1);
                }
            }
        }

        /// <summary>
        /// Холостой шаг для метода искусственного базиса.
        /// </summary>
        public void IdleStep()
        {
            buttons = new List<Button>();
            the_coordinates_of_the_support_element = new List<List<int>>();
            string temp; //вспомогательныа строка
            int index = 0; //для счёта координат

            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                temp = lbl.Content.ToString().Trim('x');
                //если это переменная которую ещё нужно выразить
                if (Int32.Parse(temp) > number_of_free_variables)
                {
                    for (int j = 0; j < simplex_elements[0].Count - 1; j++)
                    {
                        if (simplex_elements[i - 1][j] != 0)
                        {
                            //находим label
                            Label lbl2 = (Label)simplextablegrid.FindName("simplexlabel" + i + "_" + (j + 1));
                            lbl2.Visibility = Visibility.Hidden;

                            Button btn = new Button();
                            btn.Content = lbl2.Content;
                            btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                            btn.Background = Brushes.Transparent;
                            btn.BorderThickness = new Thickness(3, 3, 3, 3);
                            btn.BorderBrush = Brushes.Red;
                            //click
                            btn.Click += new RoutedEventHandler(btn_Click);
                            btn.Name = "simplexbutton" + (i - 1) + "_" + j;
                            //регистрируем имя
                            RegisterName(btn.Name, btn);
                            Grid.SetColumn(btn, j + 1);
                            Grid.SetRow(btn, i);
                            simplextablegrid.Children.Add(btn);
                            buttons.Add(btn);

                            //координаты возможного опорного элемента
                            the_coordinates_of_the_support_element.Add(new List<int>());
                            the_coordinates_of_the_support_element[index].Add(i - 1);
                            the_coordinates_of_the_support_element[index].Add(j);
                            index++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// В холостом шаге выбирается любой элемент в качестве опорного.
        /// </summary>
        public void RandomIdleStep()
        {
            string temp; //вспомогательныа строка

            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                temp = lbl.Content.ToString().Trim('x');
                //если это переменная которую ещё нужно выразить
                if (Int32.Parse(temp) > number_of_free_variables)
                {
                    for (int j = 0; j < simplex_elements[0].Count - 1; j++)
                    {
                        if (simplex_elements[i - 1][j] != 0)
                        {
                            //координаты опорного элемента
                            row_of_the_support_element = i - 1;
                            column_of_the_support_element = j;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Проверка решения/не разрешимости.
        /// </summary>
        public int ResponseCheck()
        {
            //неразрешимо?
            bool insoluble = false;
            for (int j = 0; j < simplex_elements[0].Count - 1; j++)
            {
                if (simplex_elements[simplex_elements.Count - 1][j] < 0)
                {
                    insoluble = true;
                    for (int i = 0; i < simplex_elements.Count - 1; i++)
                    {
                        if (simplex_elements[i][j] > 0)
                        {
                            insoluble = false;
                            break;
                        }
                    }
                }
                //неразрешима
                if (insoluble)
                    return -1;
            }

            //предполагаем, что нет отрицательных элементов в последней строке
            insoluble = true;
            //проверяем
            for (int j = 0; j < simplex_elements[0].Count - 1; j++)
                if (simplex_elements[simplex_elements.Count - 1][j] < 0)
                {
                    //если есть отрицательный элемент, то ищем решение дальше
                    insoluble = false;
                    break;
                }
            //есть ответ
            if (insoluble)
                return 1;

            //продолжение поиска решения
            return 0;
        }

        /// <summary>
        /// Проверка выражения искусственного базиса.
        /// </summary>
        /// <returns>True - решение готово, false - продолжаем ешать.</returns>
        public bool ArtificialResponseCheck()
        {
            int count = 0;//счётчик того, сколько переменных мы выразили
            for (int i = number_of_permutations; i < variable_visualization.Length; i++)
            {
                if (variable_visualization[i] > number_of_free_variables)
                {
                    count++;
                }
            }
            if (count == number_of_permutations)
                return true;
            else return false;
        }

        /// <summary>
        /// Ответ задачи.
        /// </summary>
        public double Response()
        {
            return simplex_elements[simplex_elements.Count - 1][simplex_elements[0].Count - 1];
        }

        /// <summary>
        /// Угловая точка соответствующая решению.
        /// </summary>
        /// <returns>Возвращает угловою точку.</returns>
        public Grid ResponseCornerDot(int step)
        {
            Grid corner_dot = new Grid();
            corner_dot.HorizontalAlignment = HorizontalAlignment.Left;
            corner_dot.Margin = new Thickness(25, 60, 0, 0);
            SettingMatrix(1, variable_visualization.Length * 2 + 1, corner_dot);
            //угловая точка соответствующая решению
            double[] finish_corner_dot = ResponseDot();

            int width = 0; //измеряем ширину для grid'a
            int index_basix = 0;//вспомогательный индекс
            for (int j = 0; j < corner_dot.ColumnDefinitions.Count; j++)
            {
                if (j == 0)
                {
                    //надстрочный символ
                    Label variable2 = new Label();
                    variable2.Content = "(" + step + ")";
                    variable2.FontSize = 7.5;
                    variable2.Width = 20;
                    variable2.Height = 20;
                    variable2.Margin = new Thickness(4, -4, 5, 9);
                    Grid.SetColumn(variable2, j);
                    Grid.SetRow(variable2, 0);
                    corner_dot.Children.Add(variable2);


                    Label variable = new Label();
                    variable.Content = "X =(";
                    width += 35;
                    variable.Width = 35;
                    variable.Height = 30;
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, 0);
                    corner_dot.Children.Add(variable);
                }
                else if (j % 2 != 0)
                {
                    //создаём новый label
                    Label txt = new Label();
                    txt.Content = Math.Round(finish_corner_dot[index_basix], 2).ToString();
                    width += 33;
                    txt.Height = 30;
                    txt.Width = 33;
                    //устанавливаем столбец
                    Grid.SetColumn(txt, j);
                    //устанавливаем строку
                    Grid.SetRow(txt, 0);
                    //добавляем в grid
                    corner_dot.Children.Add(txt);
                    index_basix++;
                }
                else
                {
                    Label variable = new Label();
                    if (j == corner_dot.ColumnDefinitions.Count - 1)
                        variable.Content = ")";
                    else
                        variable.Content = ",";
                    width += 15;
                    variable.Width = 15;
                    variable.Height = 30;
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, 0);
                    corner_dot.Children.Add(variable);
                }
            }
            corner_dot.Width = width;

            return corner_dot;
        }

        /// <summary>
        /// Коэффициенты угловой точки переносятся в массив.
        /// </summary>
        /// <returns>Массив коэффициентов угловой точки.</returns>
        private double[] ResponseDot()
        {
            //угловая точка соответствующая решению
            double[] finish_corner_dot = new double[variable_visualization.Length];

            //вспомогательная переменная
            int temp;

            //заполняем коэффициентами
            for (int i = 1; i < simplextablegrid.RowDefinitions.Count - 1; i++)
            {
                Label lbl = (Label)simplextablegrid.FindName("simplexlabel" + i + "_0");
                temp = Int32.Parse(lbl.Content.ToString().Trim('x'));
                finish_corner_dot[temp - 1] = simplex_elements[i - 1][simplex_elements[0].Count - 1];
            }

            return finish_corner_dot;
        }

        /// <summary>
        /// Возвращение массива визуализации.
        /// </summary>
        public int[] ReturnVariableVisualization()
        {
            //удаляем столбец
            for (int i = number_of_permutations; i < variable_visualization.Length; i++)
            {
                if (variable_visualization[i] > number_of_free_variables)
                {
                    //удаляем в массиве
                    for (int k = 0; k < simplex_elements.Count; k++)
                    {
                        simplex_elements[k].RemoveAt(i - number_of_permutations);
                    }

                    //удаляем визуализацию
                    int[] temp = new int[variable_visualization.Length - 1]; //временный массив
                    for (int j = 0; j < variable_visualization.Length; j++)
                    {
                        if (j < i)
                        {
                            temp[j] = variable_visualization[j];
                        }
                        else if (j > i)
                        {
                            temp[j - 1] = variable_visualization[j];
                        }
                    }
                    variable_visualization = new int[temp.Length];
                    for (int j = 0; j < temp.Length; j++)
                        variable_visualization[j] = temp[j];
                    i--;
                }
            }
            return variable_visualization;
        }

        /// <summary>
        /// Возвращение системы коэффициентов.
        /// </summary>
        public List<List<double>> ReturnElements()
        {
            //simplex_elements.RemoveAt(simplex_elements.Count - 1);
            return simplex_elements;
        }

        /// <summary>
        /// Скрыть симплекс таблицу.
        /// </summary>
        public void HideSimplexTable()
        {
            simplextablegrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Показать симплекс таблицу.
        /// </summary>
        public void VisibleSimplexTable()
        {
            simplextablegrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Задать массив визуализации.
        /// </summary>
        /// <param name="variable_visualization"></param>
        public void SetVariableVisualization(int[] variable_visualization)
        {
            this.variable_visualization = variable_visualization;
        }

        /// <summary>
        /// Задать систему коэффициентов.
        /// </summary>
        public void SetElements(List<List<double>> elements)
        {
            this.elements = elements;
        }
    }
}
