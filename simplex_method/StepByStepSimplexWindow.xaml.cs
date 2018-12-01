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
    /// Логика для пошагового симплекс-метода.
    /// </summary>
    public partial class StepByStepSimplexWindow : Window
    {
        /// <summary>
        /// Матрица коэффициентов системы ограничений-равенств.
        /// </summary>
        List<List<double>> elements = new List<List<double>>();
        /// <summary>
        /// Буфер для матрицы коэффициентов системы ограничений-равенств.
        /// </summary>
        List<List<List<double>>> buffer_elements = new List<List<List<double>>>();
        /// <summary>
        /// Текущий шаг.
        /// </summary>
        public static int step = 0;
        /// <summary>
        /// Количество базисных переменных.
        /// </summary>
        int number_of_permutations;
        /// <summary>
        /// Количество свободных переменных.
        /// </summary>
        int number_of_free_variables;
        /// <summary>
        /// Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).
        /// </summary>
        int[] variable_visualization;
        /// <summary>
        /// Буфер вспомогательного массива для визуализации переменных(по типу x1,x2,x3 и т.д.).
        /// </summary>
        int[] buffer_variable_visualization;
        /// <summary>
        /// Вспомогательная переменная, которая определяет была ли создана симплекс-таблица. True - была создана. False - не была создана.
        /// </summary>
        public static bool simplex_table_was_draw;
        /// <summary>
        /// Коэффициенты целевой функции.
        /// </summary>
        double[] target_function_elements;
        /// <summary>
        /// Симплекс-таблица.
        /// </summary>
        SimplexTable simplextable;
        /// <summary>
        /// Ищем минимум или максимум(0-минимум,1-максимум).
        /// </summary>
        int MinMax;
        /// <summary>
        /// Была ли введена угловая точка(false - нет, true - да).
        /// </summary>
        bool? CornerDot;
        /// <summary>
        /// Угловая точка соответствующая решению.
        /// </summary>
        Grid corner_dot = new Grid();
        /// <summary>
        /// Угловая точка соответствующая решению была уже нарисована.
        /// </summary>
        bool corner_dot_was_added = false;
        /// <summary>
        /// Десятичные(true) или обыкновенные(false) дроби.
        /// </summary>
        bool? decimal_or_simple;

        /// <summary>
        /// Конструктор для окна выполнения пошагового симплекс-метода.
        /// </summary>
        /// <param name="elements">Матрица коэффициентов системы ограничений-равенств.</param>
        /// <param name="selected_number_of_rows">Выбранное число строк.</param>
        /// <param name="selected_number_of_columns">Выбранное число столбцов.</param>
        /// <param name="variable_visualization">Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).</param>
        /// <param name="number_of_permutations">Количество базисных переменных.</param>
        /// <param name="target_function_elements">Коэффициенты целевой функции.</param>
        public StepByStepSimplexWindow(List<List<double>> elements, int selected_number_of_rows, int selected_number_of_columns, int[] variable_visualization, int number_of_permutations, double[] target_function_elements, int MinMax, bool? CornerDot, bool? decimal_or_simple)
        {
            InitializeComponent();
            //переносим заполненный массив с главного окна
            this.elements = elements;
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
            this.target_function_elements = target_function_elements;
            //Задача на минимум или максимум(0 - минимум, 1 - максимум)
            this.MinMax = MinMax;
            //Была ли введена угловая точка(false - нет, true - да).
            this.CornerDot = CornerDot;
            //десятичные или обыкновенные
            this.decimal_or_simple = decimal_or_simple;
            //настройка количества строк и столбцов матрицы
            SimplexTable.SettingMatrix(selected_number_of_rows, selected_number_of_columns, gaussgrid);
            //отрисовка закруглённых скобок матрицы и вертикальной черты, отделяющей столбец свободных членов
            DrawExtendedMatrix();
            //добавляем лейблы с визуализацией переменных(по типу x1, x2, x3 и т.д.)
            VariableVisualization(variable_visualization);
            //заполнение таблицы числами
            FillingtheTableWithNumbers();
        }

        /// <summary>
        /// Отрисовка закруглённых скобок матрицы и вертикальной черты, отделяющей столбец свободных членов.
        /// </summary>
        private void DrawExtendedMatrix()
        {
            //отрисовка вертикальной черты, отделяющей столбец свободных членов
            for (int i = 1; i < gaussgrid.RowDefinitions.Count; i++)
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.X1 = 0;
                myLine.X2 = 0;
                myLine.Y1 = 0;
                myLine.Y2 = 30;
                //myLine.Name = "line" + i + (gaussgrid.ColumnDefinitions.Count - 1);
                //RegisterName(myLine.Name, myLine);
                Grid.SetColumn(myLine, gaussgrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(myLine, i);
                gaussgrid.Children.Add(myLine);
            }



            //отрисовка левой скобки матрицы (без закруглений)
            for (int i = 2; i < gaussgrid.RowDefinitions.Count - 1; i++)
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.X1 = 6;
                myLine.X2 = 6;
                myLine.Y1 = 0;
                myLine.Y2 = 30;
                Grid.SetColumn(myLine, 0);
                Grid.SetRow(myLine, i);
                gaussgrid.Children.Add(myLine);
            }

            //отрисовка закругления в левом верхнем углу
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(13, 0);
            figure.Segments.Add(new ArcSegment(new Point(6, 30), new Size(55, 45), 45, false, SweepDirection.Counterclockwise, true));
            pathGeometry.Figures.Add(figure);
            Path path = new Path();
            path.Data = pathGeometry;
            path.Stroke = Brushes.Black;
            Grid.SetColumn(path, 0);
            Grid.SetRow(path, 1);
            gaussgrid.Children.Add(path);

            //отрисовка закругления в левом нижнем углу
            PathGeometry pathGeometry1 = new PathGeometry();
            PathFigure figure1 = new PathFigure();
            figure1.StartPoint = new Point(6, 0);
            figure1.Segments.Add(new ArcSegment(new Point(13, 30), new Size(55, 45), 45, false, SweepDirection.Counterclockwise, true));
            pathGeometry1.Figures.Add(figure1);
            Path path1 = new Path();
            path1.Data = pathGeometry1;
            path1.Stroke = Brushes.Black;
            Grid.SetColumn(path1, 0);
            Grid.SetRow(path1, gaussgrid.RowDefinitions.Count - 1);
            gaussgrid.Children.Add(path1);



            //отрисовка правой скобки матрицы (без закруглений)
            for (int i = 2; i < gaussgrid.RowDefinitions.Count - 1; i++)
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.X1 = 50;
                myLine.X2 = 50;
                myLine.Y1 = 30;
                myLine.Y2 = 0;
                //myLine.Name = "secondline" + i + (gaussgrid.ColumnDefinitions.Count - 1);
                //RegisterName(myLine.Name, myLine);
                Grid.SetColumn(myLine, gaussgrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(myLine, i);
                gaussgrid.Children.Add(myLine);
            }

            //отрисовка закругления в правом верхнем углу
            PathGeometry pathGeometry2 = new PathGeometry();
            PathFigure figure2 = new PathFigure();
            figure2.StartPoint = new Point(43, 0);
            figure2.Segments.Add(new ArcSegment(new Point(50, 30), new Size(55, 45), -45, false, SweepDirection.Clockwise, true));
            pathGeometry2.Figures.Add(figure2);
            Path path2 = new Path();
            path2.Data = pathGeometry2;
            path2.Stroke = Brushes.Black;
            Grid.SetColumn(path2, gaussgrid.ColumnDefinitions.Count - 1);
            Grid.SetRow(path2, 1);
            gaussgrid.Children.Add(path2);

            //отрисовка закругления в правом нижнем углу
            PathGeometry pathGeometry3 = new PathGeometry();
            PathFigure figure3 = new PathFigure();
            figure3.StartPoint = new Point(50, 0);
            figure3.Segments.Add(new ArcSegment(new Point(43, 30), new Size(55, 45), -45, false, SweepDirection.Clockwise, true));
            pathGeometry3.Figures.Add(figure3);
            Path path3 = new Path();
            path3.Data = pathGeometry3;
            path3.Stroke = Brushes.Black;
            Grid.SetColumn(path3, gaussgrid.ColumnDefinitions.Count - 1);
            Grid.SetRow(path3, gaussgrid.RowDefinitions.Count - 1);
            gaussgrid.Children.Add(path3);
        }

        /// <summary>
        /// Добавляем лейблы с визуализацией переменных(по типу x1, x2, x3 и т.д.).
        /// </summary>
        /// <param name="variable_visualization">Вспомогательный массив.</param>
        private void VariableVisualization(int[] variable_visualization)
        {
            int width = 0; //для пересчёта ширины
            for (int j = 0; j < gaussgrid.ColumnDefinitions.Count; j++)
            {
                Label variable = new Label();
                if (j != gaussgrid.ColumnDefinitions.Count - 1)
                    variable.Content = "x" + variable_visualization[j];
                variable.Width = 35;
                width += 35;
                variable.Height = 30;
                variable.HorizontalContentAlignment = HorizontalAlignment.Center;
                variable.Name = "gaussvisuallabel" + j;
                //регистрируем имя
                RegisterName(variable.Name, variable);
                Grid.SetColumn(variable, j);
                Grid.SetRow(variable, 0);
                gaussgrid.Children.Add(variable);
            }
            gaussgrid.Width = width;
        }

        /// <summary>
        /// Заполнение таблицы числами.
        /// </summary>
        private void FillingtheTableWithNumbers()
        {
            double width = gaussgrid.Width; //для пересчёта ширины
            double height = 30; //для пересчёта высоты
            for (int i = 1; i < gaussgrid.RowDefinitions.Count; i++)
            {
                width = 0;
                for (int j = 0; j < gaussgrid.ColumnDefinitions.Count; j++)
                {
                    Label variable = new Label();
                    if (decimal_or_simple == true)
                        variable.Content = elements[i - 1][j];
                    else
                        variable.Content = DoubleToFraction.Convert(elements[i - 1][j]);
                    variable.Width = 55;
                    width += 55;
                    variable.Height = 30;
                    variable.HorizontalContentAlignment = HorizontalAlignment.Center;
                    variable.Name = "gausslabel" + i + "_" + j;
                    //регистрируем имя
                    RegisterName(variable.Name, variable);
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, i);
                    gaussgrid.Children.Add(variable);
                }
                height += 30;
            }
            gaussgrid.Width = width;
            gaussgrid.Height = height;
        }

        /// <summary>
        /// Обновление данных таблицы.
        /// </summary>
        private void UpdateTableValues()
        {
            for (int i = 1; i < gaussgrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < gaussgrid.ColumnDefinitions.Count; j++)
                {
                    //находим label
                    Label lbl = (Label)gaussgrid.FindName("gausslabel" + i + "_" + j);
                    if (decimal_or_simple == true)
                        lbl.Content = elements[i - 1][j];
                    else
                        lbl.Content = DoubleToFraction.Convert(elements[i - 1][j]);
                }
            }
        }

        /// <summary>
        /// Обновление визуализации переменных.
        /// </summary>
        private void UpdateVisualVariables()
        {
            ////буферизируем
            //buffer_variable_visualization = variable_visualization;

            for (int j = 0; j < gaussgrid.ColumnDefinitions.Count - 1; j++)
            {
                //находим label
                Label lbl = (Label)gaussgrid.FindName("gaussvisuallabel" + j);
                lbl.Content = "x" + variable_visualization[j];
            }
        }

        /// <summary>
        /// Возвращение из буфера массива визуализации.
        /// </summary>
        private void GetOutOfBufferVariableVisual()
        {
            for (int i = 0; i < buffer_variable_visualization.Length; i++)
                variable_visualization[i] = buffer_variable_visualization[i];

            for (int j = 0; j < gaussgrid.ColumnDefinitions.Count - 1; j++)
            {
                //находим label
                Label lbl = (Label)gaussgrid.FindName("gaussvisuallabel" + j);
                lbl.Content = "x" + variable_visualization[j];
            }
        }

        /// <summary>
        /// Буферизация элементов таблицы.
        /// </summary>
        private void BufferingTableValues()
        {
            buffer_elements.Add(new List<List<double>>());
            for (int i = 0; i < elements.Count; i++)
            {
                buffer_elements[step].Add(new List<double>());
                for (int j = 0; j < elements[0].Count; j++)
                    buffer_elements[step][i].Add(elements[i][j]);
            }
        }

        /// <summary>
        /// Возврат элементов таблицы из буфера.
        /// </summary>
        private void GetOutOfTheBuffer()
        {
            for (int i = 0; i < buffer_elements[step - 1].Count; i++)
                for (int j = 0; j < buffer_elements[step - 1][0].Count; j++)
                    elements[i][j] = buffer_elements[step - 1][i][j];
            buffer_elements.RemoveAt(step - 1);
        }

        /// <summary>
        /// Кнопка "вперёд".
        /// </summary>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            switch (step)
            {
                case 0:
                    //буферизация данных
                    BufferingTableValues();
                    try
                    {
                        buffer_variable_visualization = new int[variable_visualization.Length];
                        for (int i = 0; i < variable_visualization.Length; i++)
                            buffer_variable_visualization[i] = variable_visualization[i];
                        //прямой ход Гаусса
                        MatrixTransformation.Gauss(elements, CornerDot, variable_visualization);
                    }
                    catch (Exception d)
                    {
                        MessageBox.Show(d.Message);
                        buttonToMainWindow.Visibility = Visibility.Visible;
                    }
                    //Обновление визуализации переменных.
                    UpdateVisualVariables();
                    //обновление таблицы
                    UpdateTableValues();
                    step++;
                    labelsteps.Content = "Шаг 1: Прямой ход метода Гаусса.";
                    break;
                case 1:
                    //буферизация данных
                    BufferingTableValues();
                    //Выражение базисных переменных.
                    MatrixTransformation.HoistingMatrix(elements, number_of_permutations);
                    //обновление таблицы
                    UpdateTableValues();
                    step++;
                    labelsteps.Content = "Шаг 2: Выражение базисных переменных.";
                    break;
                case 2:
                    //скрываем матрицу
                    scrollgaussgrid.Visibility = Visibility.Hidden;
                    if (simplex_table_was_draw == false)
                    {
                        simplextable = new SimplexTable(number_of_permutations, number_of_free_variables, variable_visualization, elements, target_function_elements, true);
                        MainGrid.Children.Add(simplextable);
                        //Симплекс-таблица была создана
                        simplex_table_was_draw = true;
                    }
                    //показываем симплекс-таблицу
                    simplextable.Visibility = Visibility.Visible;
                    switch (simplextable.ResponseCheck())
                    {
                        case 0:
                            step++;
                            labelsteps.Content = "Шаг 3: Симплекс-таблица.";
                            break;
                        case 1:
                            step++;
                            if (MinMax == 0)
                                labelsteps.Content = "Ответ :" + simplextable.Response() * (-1);
                            else labelsteps.Content = "Ответ :" + simplextable.Response();
                            if (corner_dot_was_added == false)
                            {
                                //добавляем точку
                                corner_dot = simplextable.ResponseCornerDot(step);
                                MainGrid.Children.Add(corner_dot);
                                corner_dot_was_added = true;
                            }
                            //показываем угловую точку решения
                            corner_dot.Visibility = Visibility.Hidden;
                            buttonToMainWindow.Visibility = Visibility.Visible;
                            break;
                        case -1:
                            step++;
                            labelsteps.Content = "Задача не разрешима!";
                            buttonToMainWindow.Visibility = Visibility.Visible;
                            break;
                    }
                    break;
                case 3:
                    //выбор опорного
                    simplextable.SelectionOfTheSupportElement();
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    step++;
                    labelsteps.Content = "Шаг " + step + ": Симплекс-таблица. Выбор опорного элемента.";
                    break;
                default:
                    try
                    {
                        //выбран ли опорный элемент
                        simplextable.ButtonPressedOrNot();
                        //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                        simplextable.ChangeOfVisualizationVariables();
                        //буферизация данных
                        simplextable.BufferingSimplexTableValues(step);
                        //удаляем кнопки
                        simplextable.DeleteButtons();
                        //вычисление согласно выбранному опорному элементу
                        simplextable.CalculateSimplexTable();
                        //обновление данных сиплекс-таблицы
                        simplextable.UpdateSimplexTableValues();
                        switch (simplextable.ResponseCheck())
                        {
                            case 0:
                                step++;
                                labelsteps.Content = "Шаг " + step + ": Симплекс-таблица. Выбор опорного элемента.";
                                //выбор опорного
                                simplextable.SelectionOfTheSupportElement();
                                break;
                            case 1:
                                step++;
                                if (MinMax == 0)
                                    labelsteps.Content = "Ответ :" + simplextable.Response() * (-1);
                                else labelsteps.Content = "Ответ :" + simplextable.Response();
                                //если не была добавлена, то добавляем
                                if (corner_dot_was_added == false)
                                {
                                    //добавляем точку
                                    corner_dot = simplextable.ResponseCornerDot(step - 4);
                                    MainGrid.Children.Add(corner_dot);
                                    corner_dot_was_added = true;
                                }
                                //показываем угловую точку решения
                                corner_dot.Visibility = Visibility.Visible;
                                buttonToMainWindow.Visibility = Visibility.Visible;
                                break;
                            case -1:
                                step++;
                                labelsteps.Content = "Задача не разрешима!";
                                buttonToMainWindow.Visibility = Visibility.Visible;
                                break;
                        }
                        simplextable.CornerPoint(step - 4);
                    }
                    catch (Exception d)
                    {
                        MessageBox.Show(d.Message);
                    }
                    break;
            }
        }

        /// <summary>
        /// Кнопка "Назад".
        /// </summary>
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            switch (step)
            {
                case 0:
                    //создаём экземпляр главного окна
                    MainWindow MW = new MainWindow();
                    //открываем главное окно
                    MW.Show();
                    //закрываем это окно
                    this.Close();
                    break;
                case 1:
                    //возвращение визуализации из буфера
                    GetOutOfBufferVariableVisual();
                    //возвращение данных из буфера
                    GetOutOfTheBuffer();
                    //обновление таблицы
                    UpdateTableValues();
                    step--;
                    labelsteps.Content = "Матрица коэффициентов системы ограничений равенств.";
                    buttonToMainWindow.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    //возвращение данных из буфера
                    GetOutOfTheBuffer();
                    //обновление таблицы
                    UpdateTableValues();
                    step--;
                    labelsteps.Content = "Шаг 1: Прямой ход метода Гаусса.";
                    break;
                case 3:
                    //показываем матрицу
                    scrollgaussgrid.Visibility = Visibility.Visible;
                    //скрываем симплекс-таблицу
                    simplextable.Visibility = Visibility.Hidden;
                    //скрываем угловую точку решения
                    corner_dot.Visibility = Visibility.Hidden;
                    step--;
                    labelsteps.Content = "Шаг 2: Выражение базисных переменных.";
                    buttonToMainWindow.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    //убираем кнопки
                    simplextable.DeleteButtons();
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    step--;
                    labelsteps.Content = "Шаг 3: Симплекс-таблица.";
                    buttonToMainWindow.Visibility = Visibility.Hidden;
                    //скрываем угловую точку решения
                    corner_dot.Visibility = Visibility.Hidden;
                    break;
                default:
                    //убираем кнопки
                    simplextable.DeleteButtons();
                    //возвращение данных из буфера
                    simplextable.GetOutOfTheBufferSimplex(step);
                    simplextable.GetOutOfTheBufferVisualizationVariables(step);
                    //обновление данных сиплекс-таблицы
                    simplextable.UpdateSimplexTableValues();
                    //выбор опорного
                    simplextable.SelectionOfTheSupportElement();
                    step--;
                    labelsteps.Content = "Шаг " + step + ": Симплекс-таблица. Выбор опорного элемента.";
                    buttonToMainWindow.Visibility = Visibility.Hidden;
                    //скрываем угловую точку решения
                    corner_dot.Visibility = Visibility.Hidden;
                    simplextable.CornerPoint(step - 4);
                    break;
            }
        }

        /// <summary>
        /// Кнопка "На главную".
        /// </summary>
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

