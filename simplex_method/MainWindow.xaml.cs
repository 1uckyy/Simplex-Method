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
using System.IO;
using System.Text.RegularExpressions;

namespace simplex_method
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        List<List<double>> elements = new List<List<double>>();
        int num_variable;
        public static TextBox textBoxDirectory = new TextBox();

        public MainWindow()
        {
            InitializeComponent();
            textBoxDirectory.Text = "не установлен";
            textBoxDirectory.SelectionChanged += new RoutedEventHandler(Selection_Changed);
        }

        //после изменения значения в combobox со строками
        private void dimension1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //отображаем в combobox'е выбранное значение
            //dimension1.Text = (Int32.Parse(dimension1.SelectedIndex.ToString()) + 1).ToString();

            //отлавливаем проблему, когда при запуске программы выполняется метод dimension1_SelectionChanged(так как по умолчанию выбрано 3 строки), но при этом grid ещё не создан и мы не можем на него ссылаться
            try
            {
                //удаляем все textbox'ы
                del_textBoxs();

                //узнаём количество строк в grid'е
                int count_of_rows = entergrid.RowDefinitions.Count;

                //удаляем все строки в grid'е
                for (int i = count_of_rows - 1; i > -1; i--)
                    entergrid.RowDefinitions.RemoveAt(i);

                //добавляем в grid выбранное число строк
                for (int i = 0; i < Int32.Parse(dimension1.SelectedIndex.ToString()) + 1; i++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    entergrid.RowDefinitions.Add(row);
                }

                //добавляем textbox'ы
                add_textBoxs();

                //изменяем высоту grid'a (для scrollviewer)
                entergrid.Height = entergrid.RowDefinitions.Count * 30;
            }
            catch
            {
            }
        }

        //после изменения значения в combobox со столбцами
        private void dimension2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //отлавливаем проблему, когда при запуске программы выполняется метод dimension2_SelectionChanged(так как по умолчанию выбрано 3 столбца), но при этом grid ещё не создан и мы не можем на него ссылаться
            try
            {
                //удаляем все textbox'ы
                del_textBoxs();

                //узнаём количество столбцов в grid'е
                int count_of_columns = entergrid.ColumnDefinitions.Count;

                //удаляем все столбцы в grid'е
                for (int i = count_of_columns - 1; i > -1; i--)
                    entergrid.ColumnDefinitions.RemoveAt(i);

                //добавляем в grid выбранное число столбцов
                for (int i = 0; i < (Int32.Parse(dimension2.SelectedIndex.ToString()) + 1) * 2 + 1; i++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    entergrid.ColumnDefinitions.Add(column);
                }

                //добавляем textbox'ы
                add_textBoxs();

                //для базисных переменных (работа с grid with name "basix")
                if (method.SelectedIndex == 0)
                {
                    del_for_basix();

                    //узнаём количество столбцов в grid'е basix
                    int count_of_columns_basix = basix_variables.ColumnDefinitions.Count;

                    //удаляем все столбцы в grid'е basix
                    for (int i = count_of_columns_basix - 1; i > -1; i--)
                        basix_variables.ColumnDefinitions.RemoveAt(i);

                    //добавляем в grid basix выбранное число столбцов
                    for (int i = 0; i < (Int32.Parse(dimension2.SelectedIndex.ToString()) + 1) * 2 + 1; i++)
                    {
                        ColumnDefinition column = new ColumnDefinition();
                        column.Width = GridLength.Auto;
                        basix_variables.ColumnDefinitions.Add(column);
                    }

                    add_for_basix();
                }

                //для целевой функции
                del_for_targetfunction();

                //узнаём количество столбцов в grid'е targetfunction
                int count_of_columns_targetfunction = targetfunction.ColumnDefinitions.Count;

                //удаляем все столбцы в grid'е targetfunction
                for (int i = count_of_columns_targetfunction - 1; i > -1; i--)
                    targetfunction.ColumnDefinitions.RemoveAt(i);

                //добавляем в grid targetfunction выбранное число столбцов
                for (int i = 0; i < (Int32.Parse(dimension2.SelectedIndex.ToString()) + 1) * 2; i++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    targetfunction.ColumnDefinitions.Add(column);
                }

                add_for_targetfunction();
            }
            catch
            {
            }

        }

        //после загрузки grid'а добавляем текстбоксы
        private void entergrid_Loaded(object sender, RoutedEventArgs e)
        {
            //добавляем текстбоксы
            add_textBoxs();
        }

        //метод добавления textbox'ов и label'ов
        private void add_textBoxs()
        {
            int width; //измеряем ширину для grid'a
            int num_variable;
            for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
            {
                num_variable = 1;
                width = 0;
                for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                {
                    if (j % 2 == 0)
                    {
                        //создаём новый textbox
                        System.Windows.Controls.TextBox txt = new System.Windows.Controls.TextBox();
                        //присваеваем ему имя, соответствующее определённой ячейке grid'а
                        txt.Name = "textBox" + i + "_" + j;
                        txt.Text = j.ToString(); //УДАЛИТЬ ЭТУ СТРОКУ ДЛЯ АВТОЗАПОЛНЕНИЯ
                        txt.Height = 20;
                        txt.Width = 20;
                        //регистрируем имя
                        RegisterName(txt.Name, txt);
                        //устанавливаем столбец
                        Grid.SetColumn(txt, j);
                        //устанавливаем строку
                        Grid.SetRow(txt, i);
                        //добавляем в grid
                        entergrid.Children.Add(txt);
                        width += 20;
                    }
                    else
                    {
                        if (j == entergrid.ColumnDefinitions.Count - 2)
                        {
                            Label variable = new Label();
                            variable.Content = "*x" + num_variable + "=";
                            if (num_variable / 10 != 0)
                            {
                                variable.Width = 40;
                                width += 40;
                            }
                            else
                            {
                                variable.Width = 35;
                                width += 35;
                            }
                            variable.Height = 30;
                            variable.Name = "label" + i + "_" + j;
                            //регистрируем имя
                            RegisterName(variable.Name, variable);
                            Grid.SetColumn(variable, j);
                            Grid.SetRow(variable, i);
                            entergrid.Children.Add(variable);
                        }
                        else
                        {
                            Label variable = new Label();
                            variable.Content = "*x" + num_variable + "+";
                            if (num_variable / 10 != 0)
                            {
                                variable.Width = 40;
                                width += 40;
                            }
                            else
                            {
                                variable.Width = 35;
                                width += 35;
                            }
                            variable.Height = 30;
                            variable.Name = "label" + i + "_" + j;
                            //регистрируем имя
                            RegisterName(variable.Name, variable);
                            Grid.SetColumn(variable, j);
                            Grid.SetRow(variable, i);
                            entergrid.Children.Add(variable);
                            num_variable++;
                        }
                    }
                }
                entergrid.Width = width;
            }
        }

        //метод удаления textbox'ов и label'ов
        private void del_textBoxs()
        {
            for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                {
                    if (j % 2 == 0)
                    {
                        //находим textbox
                        TextBox txt = (TextBox)entergrid.FindName("textBox" + i + "_" + j);
                        //разрегистрируем
                        UnregisterName(txt.Name);
                        //удаляем из grid'а
                        entergrid.Children.Remove(txt);
                    }
                    else
                    {
                        Label variable = (Label)entergrid.FindName("label" + i + "_" + j);
                        UnregisterName(variable.Name);
                        entergrid.Children.Remove(variable);
                    }
                }
            }
        }

        //кнопка "решить"
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //для обыкновенных дробей дополнительная проверка
                if(radioButtonDecimals.IsChecked == false)
                {
                    //шаблон
                    string pattern = @"[0-9]+/[0-9]+";

                    string pattern1 = @"[0-9]+.[0-9]+";

                    //проверяем по шаблону
                    for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
                    {
                        for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                        {
                            if (j % 2 == 0)
                            {
                                //находим textbox
                                TextBox txt = (TextBox)entergrid.FindName("textBox" + i + "_" + j);
                                //если это дробь, то обрабатываем её
                                if (Regex.IsMatch(txt.Text, pattern) == true)
                                {
                                    string[] str = txt.Text.Split('/');

                                    if(Int32.Parse(str[1])==0)
                                    {
                                        throw new Exception("Деление на ноль: "+txt.Text);
                                    }

                                    txt.Text = ((double)Int32.Parse(str[0]) / Int32.Parse(str[1])).ToString();

                                    Regex.Replace(txt.Text, ",", ".");
                                }

                                if (Regex.IsMatch(txt.Text, pattern1) == true)
                                    throw new Exception("Десятичная дробь: " + txt.Text);
                            }
                        }
                    }
                }

                elements = new List<List<double>>();

                //заполняем массив элементами(коэффициентами), введёнными в текстбоксы
                for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
                {
                    elements.Add(new List<double>());
                    for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                    {
                        if (j % 2 == 0)
                        {
                            //находим textbox
                            TextBox txt = (TextBox)entergrid.FindName("textBox" + i + "_" + j);
                            //добавляем в массив число
                            elements[i].Add(double.Parse(txt.Text));
                        }
                    }
                }

                //вспомогательный массив для подсчёта ранга
                List<List<double>> copy_elements = new List<List<double>>();
                for (int i = 0; i < elements.Count; i++)
                {
                    copy_elements.Add(new List<double>());
                    for (int j = 0; j < elements[0].Count; j++)
                    {
                        copy_elements[i].Add(elements[i][j]);
                    }
                }

                //ранг матрицы
                int rang = RangOfMatrix(copy_elements);

                //если выбраны пошаговый режим, симплекс-метод и задание начальной угловой точки
                if ((stepbystepmode.IsChecked == true) && simplex.IsSelected && (checkBoxCornerDot.IsChecked == true))
                {
                    int index = 0;//индекс переменной, которая является базисной
                    int count_basix_var = 0;//число базисных переменных

                    //проверяем на возможность выражения базисных переменных
                    for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
                    {
                        if (j % 2 != 0)
                        {
                            //находим textbox
                            TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                            //проверяем базисная ли переменная
                            if (Int32.Parse(txt.Text) != 0)
                            {
                                CheckCanBeBasix(index);
                                count_basix_var++;
                            }
                            index++;
                        }
                    }

                    //проверяем совпадает ли число базисных переменных с рангом матрицы
                    if (rang != count_basix_var)
                        throw new Exception("Ранг матрицы (ранг=" + rang + ") не равен числу заданных базисных переменных (кол-во=" + count_basix_var + ").");


                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //если решаем продолжать
                        if (MessageBox.Show("Число ограничений-равенств (" + (dimension1.SelectedIndex + 1) + ") больше ранга матрицы (" + rang + "). Следовательно есть линейно зависимые строки. Убрать \"ненужные\" строки матрицы и продолжить?", "Вопрос!?!?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки


                            //вспомогательный массив для дальнейшего отображения переменных
                            int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                            //меняем местами столбцы для прямого хода метода Гаусса
                            int count = ChangeColumnsForGauss(copy_elements, variable_visualization);


                            //массив для коэффициентов целевой функции
                            double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                            //заполняем массив коэффициентов целевой функции
                            FillArrayWithCoefOfGoalFunc(target_function_elements);


                            //создаём экземпляр окна для пошагового режима
                            StepByStepSimplexWindow SBSSW = new StepByStepSimplexWindow(copy_elements, rang + 1, (Int32.Parse(dimension2.SelectedIndex.ToString()) + 2), variable_visualization, count, target_function_elements, comboBoxMinMax.SelectedIndex, checkBoxCornerDot.IsChecked, radioButtonDecimals.IsChecked);
                            //открываем
                            SBSSW.Show();
                            //закрываем основной
                            this.Close();
                        }
                    }
                    //если никакие строки убирать не надо, тогда идём обычным путём
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //меняем местами столбцы для прямого хода метода Гаусса
                        int count = ChangeColumnsForGauss(elements, variable_visualization);

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна
                        StepByStepSimplexWindow SBSSW = new StepByStepSimplexWindow(elements, (Int32.Parse(dimension1.SelectedIndex.ToString()) + 2), (Int32.Parse(dimension2.SelectedIndex.ToString()) + 2), variable_visualization, count, target_function_elements, comboBoxMinMax.SelectedIndex, checkBoxCornerDot.IsChecked, radioButtonDecimals.IsChecked);
                        //открываем
                        SBSSW.Show();
                        //закрываем основной
                        this.Close();
                    }
                }
                //если выбраны пошаговый режим и симплекс-метод без задания начальной угловой точки
                else if ((stepbystepmode.IsChecked == true) && simplex.IsSelected && (checkBoxCornerDot.IsChecked == false))
                {
                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //если решаем продолжать
                        if (MessageBox.Show("Число ограничений-равенств (" + (dimension1.SelectedIndex + 1) + ") больше ранга матрицы (" + rang + "). Следовательно есть линейно зависимые строки. Убрать \"ненужные\" строки матрицы и продолжить?", "Вопрос!?!?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки

                            //вспомогательный массив для дальнейшего отображения переменных
                            int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                            //Заполняем массив визуализации переменных. Например, для x1,x2,x3,x4 заполним [1,2,3,4].
                            for (int i = 0; i < variable_visualization.Count(); i++)
                                variable_visualization[i] = i + 1;

                            //массив для коэффициентов целевой функции
                            double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                            //заполняем массив коэффициентов целевой функции
                            FillArrayWithCoefOfGoalFunc(target_function_elements);


                            //создаём экземпляр окна
                            StepByStepSimplexWindow SBSSW = new StepByStepSimplexWindow(copy_elements, rang + 1, (Int32.Parse(dimension2.SelectedIndex.ToString()) + 2), variable_visualization, rang, target_function_elements, comboBoxMinMax.SelectedIndex, checkBoxCornerDot.IsChecked, radioButtonDecimals.IsChecked);
                            //открываем
                            SBSSW.Show();
                            //закрываем основной
                            this.Close();
                        }
                    }
                    //если никакие строки убирать не надо, тогда идём обычным путём
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //Заполняем массив визуализации переменных. Например, для x1,x2,x3,x4 заполним [1,2,3,4].
                        for (int i = 0; i < variable_visualization.Count(); i++)
                            variable_visualization[i] = i + 1;

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна
                        StepByStepSimplexWindow SBSSW = new StepByStepSimplexWindow(elements, (Int32.Parse(dimension1.SelectedIndex.ToString()) + 2), (Int32.Parse(dimension2.SelectedIndex.ToString()) + 2), variable_visualization, rang, target_function_elements, comboBoxMinMax.SelectedIndex, checkBoxCornerDot.IsChecked, radioButtonDecimals.IsChecked);
                        //открываем
                        SBSSW.Show();
                        //закрываем основной
                        this.Close();
                    }
                }
                //если выбраны автоматический режим, симплекс-метод и задание начальной угловой точки
                else if ((automode.IsChecked == true) && simplex.IsSelected && (checkBoxCornerDot.IsChecked == true))
                {
                    int index = 0;//индекс переменной, которая является базисной
                    int count_basix_var = 0;//число базисных переменных

                    //проверяем на возможность выражения базисных переменных
                    for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
                    {
                        if (j % 2 != 0)
                        {
                            //находим textbox
                            TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                            //проверяем базисная ли переменная
                            if (Int32.Parse(txt.Text) != 0)
                            {
                                CheckCanBeBasix(index);
                                count_basix_var++;
                            }
                            index++;
                        }
                    }

                    //проверяем совпадает ли число базисных переменных с рангом матрицы
                    if (rang != count_basix_var)
                        throw new Exception("Ранг матрицы (ранг=" + rang + ") не равен числу заданных базисных переменных (кол-во=" + count_basix_var + ").");


                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки

                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //меняем местами столбцы для прямого хода метода Гаусса
                        int count = ChangeColumnsForGauss(copy_elements, variable_visualization);


                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна для автоматического режима
                        AutoModeSimplexTable AMST = new AutoModeSimplexTable(copy_elements, checkBoxCornerDot.IsChecked, variable_visualization, count_basix_var, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        //открываем
                        AMST.Show();
                        //закрываем основной
                        this.Close();
                    }
                    //если никакие строки убирать не надо, тогда идём обычным путём
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //меняем местами столбцы для прямого хода метода Гаусса
                        int count = ChangeColumnsForGauss(elements, variable_visualization);

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна для автоматического режима
                        AutoModeSimplexTable AMST = new AutoModeSimplexTable(elements, checkBoxCornerDot.IsChecked, variable_visualization, count_basix_var, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        //открываем
                        AMST.Show();
                        //закрываем основной
                        this.Close();
                    }
                }
                //если выбраны автоматический режим и симплекс-метод без задания начальной угловой точки
                else if ((automode.IsChecked == true) && simplex.IsSelected && (checkBoxCornerDot.IsChecked == false))
                {
                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки

                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //Заполняем массив визуализации переменных. Например, для x1,x2,x3,x4 заполним [1,2,3,4].
                        for (int i = 0; i < variable_visualization.Count(); i++)
                            variable_visualization[i] = i + 1;

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна для автоматического режима
                        AutoModeSimplexTable AMST = new AutoModeSimplexTable(copy_elements, checkBoxCornerDot.IsChecked, variable_visualization, rang, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        //открываем
                        AMST.Show();
                        //закрываем основной
                        this.Close();
                    }
                    //если никакие строки убирать не надо, тогда идём обычным путём
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text)];
                        //Заполняем массив визуализации переменных. Например, для x1,x2,x3,x4 заполним [1,2,3,4].
                        for (int i = 0; i < variable_visualization.Count(); i++)
                            variable_visualization[i] = i + 1;

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна для автоматического режима
                        AutoModeSimplexTable AMST = new AutoModeSimplexTable(elements, checkBoxCornerDot.IsChecked, variable_visualization, rang, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        //открываем
                        AMST.Show();
                        //закрываем основной
                        this.Close();
                    }
                }
                //если выбраны пошаговый режим и метод искусственного базиса
                else if ((stepbystepmode.IsChecked == true) && (artificialbasismethod.IsSelected == true))
                {
                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //если решаем продолжать
                        if (MessageBox.Show("Число ограничений-равенств (" + (dimension1.SelectedIndex + 1) + ") больше ранга матрицы (" + rang + "). Следовательно есть линейно зависимые строки. Убрать \"ненужные\" строки матрицы и продолжить?", "Вопрос!?!?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки

                            //вспомогательный массив для дальнейшего отображения переменных
                            int[] variable_visualization = new int[Int32.Parse(dimension2.Text) + rang];
                            for (int i = 0; i < rang; i++)
                                variable_visualization[i] = Int32.Parse(dimension2.Text) + i + 1;
                            //Заполняем массив визуализации переменных..
                            for (int i = rang; i < variable_visualization.Count(); i++)
                                variable_visualization[i] = i - rang + 1;

                            //массив для коэффициентов целевой функции
                            double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                            //заполняем массив коэффициентов целевой функции
                            FillArrayWithCoefOfGoalFunc(target_function_elements);


                            //создаём экземпляр окна
                            StepByStepArtificialBasisWindow SBABW = new StepByStepArtificialBasisWindow(copy_elements, rang, variable_visualization, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                            //открываем
                            SBABW.Show();
                            //закрываем основной
                            this.Close();
                        }
                    }
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text) + rang];
                        for (int i = 0; i < rang; i++)
                            variable_visualization[i] = Int32.Parse(dimension2.Text) + i + 1;
                        //Заполняем массив визуализации переменных..
                        for (int i = rang; i < variable_visualization.Count(); i++)
                            variable_visualization[i] = i - rang + 1;

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        StepByStepArtificialBasisWindow SBABW = new StepByStepArtificialBasisWindow(elements, rang, variable_visualization, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        SBABW.Show();
                        this.Close();
                    }
                }
                //если выбраны автоматический режим и метод искусственного базиса
                else if ((automode.IsChecked == true) && (artificialbasismethod.IsSelected == true))
                {
                    //если число ограничений-равенств больше ранга матрицы
                    if ((dimension1.SelectedIndex + 1) > rang)
                    {
                        //если решаем продолжать
                        if (MessageBox.Show("Число ограничений-равенств (" + (dimension1.SelectedIndex + 1) + ") больше ранга матрицы (" + rang + "). Следовательно есть линейно зависимые строки. Убрать \"ненужные\" строки матрицы и продолжить?", "Вопрос!?!?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //тогда работаем с массивом copy_elements, в котором уже удалены "ненужные" строки

                            //вспомогательный массив для дальнейшего отображения переменных
                            int[] variable_visualization = new int[Int32.Parse(dimension2.Text) + rang];
                            for (int i = 0; i < rang; i++)
                                variable_visualization[i] = Int32.Parse(dimension2.Text) + i + 1;
                            //Заполняем массив визуализации переменных..
                            for (int i = rang; i < variable_visualization.Count(); i++)
                                variable_visualization[i] = i - rang + 1;

                            //массив для коэффициентов целевой функции
                            double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                            //заполняем массив коэффициентов целевой функции
                            FillArrayWithCoefOfGoalFunc(target_function_elements);

                            //создаём экземпляр окна для автоматического режима
                            AutoModeArtificialBasis AMAB = new AutoModeArtificialBasis(copy_elements, rang, variable_visualization, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                            //открываем
                            AMAB.Show();
                            //закрываем основной
                            this.Close();
                        }
                    }
                    else
                    {
                        //вспомогательный массив для дальнейшего отображения переменных
                        int[] variable_visualization = new int[Int32.Parse(dimension2.Text) + rang];
                        for (int i = 0; i < rang; i++)
                            variable_visualization[i] = Int32.Parse(dimension2.Text) + i + 1;
                        //Заполняем массив визуализации переменных..
                        for (int i = rang; i < variable_visualization.Count(); i++)
                            variable_visualization[i] = i - rang + 1;

                        //массив для коэффициентов целевой функции
                        double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
                        //заполняем массив коэффициентов целевой функции
                        FillArrayWithCoefOfGoalFunc(target_function_elements);

                        //создаём экземпляр окна для автоматического режима
                        AutoModeArtificialBasis AMAB = new AutoModeArtificialBasis(elements, rang, variable_visualization, target_function_elements, comboBoxMinMax.SelectedIndex, radioButtonDecimals.IsChecked);
                        //открываем
                        AMAB.Show();
                        //закрываем основной
                        this.Close();
                    }
                }
            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Заполняем массив коэффициентов целевой функции.
        /// </summary>
        /// <param name="target_function_elements">Массив для коэффициентов целевой функции.</param>
        private void FillArrayWithCoefOfGoalFunc(double[] target_function_elements)
        {
            //индекс для массива коэффициентов целевой функции
            int number_element = 0;
            //заполняем массив коэффициентов целевой функции
            for (int j = 0; j < targetfunction.ColumnDefinitions.Count; j++)
            {
                if (j % 2 == 0)
                {
                    //находим textbox
                    TextBox txt = (TextBox)targetfunction.FindName("targetfunctiontextBox" + j);
                    if (comboBoxMinMax.SelectedIndex == 0)
                        target_function_elements[number_element] = Int32.Parse(txt.Text);
                    else target_function_elements[number_element] = Int32.Parse(txt.Text) * (-1);
                    number_element++;
                }
            }
        }

        /// <summary>
        /// Меняем местами столбцы для прямого хода метода Гаусса.
        /// </summary>
        /// <param name="elements_1">Матрица коэффициентов системы ограничений-равенств.</param>
        /// <param name="variable_visualization">Вспомогательный массив для дальнейшего отображения переменных, который хранит индексы переменных в нужном порядке(x1,x3,x2... и другие различные комбинации).</param>
        /// <returns>Возвращаем количество перестановок и по совместительнству число базисных переменных.</returns>
        private int ChangeColumnsForGauss(List<List<double>> elements_1, int[] variable_visualization)
        {
            int count = 0; //счётчик перестановок и по совместительнству число базисных переменных
            int number_variable_basix = 0;//индекс переменной
            //double[] temp_column = new double[Int32.Parse(dimension1.Text)]; //вспомогательная колонка
            double temp_for_elements;
            int temp_for_visual; //вспомогательная переменная для перестановки индексов переменных в массиве визуализации переменных

            //Заполняем массив визуализации переменных. Например, для x1,x2,x3,x4 заполним [1,2,3,4].
            for (int i = 0; i < variable_visualization.Count(); i++)
                variable_visualization[i] = i + 1;

            //перестановки в матрице и массиве визуализации
            for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
            {
                if (j % 2 != 0)
                {
                    //находим textbox
                    TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                    //проверяем базисная ли переменная
                    if (Int32.Parse(txt.Text) != 0)
                    {
                        //меняемся столбцами
                        for (int i = 0; i < elements_1.Count; i++)
                        {
                            temp_for_elements = elements_1[i][number_variable_basix];
                            elements_1[i][number_variable_basix] = elements_1[i][count];
                            elements_1[i][count] = temp_for_elements;
                        }
                        //меняем индексы для последующей визуализации
                        temp_for_visual = variable_visualization[number_variable_basix];
                        variable_visualization[number_variable_basix] = variable_visualization[count];
                        variable_visualization[count] = temp_for_visual;

                        count++;//плюс ещё одна базисная переменная
                    }
                    number_variable_basix++; //меняем индекс переменной
                }
            }

            //Возвращаем количество перестановок и по совместительнству число базисных переменных
            return count;
        }

        /// <summary>
        /// Ищем ранг матрицы. Вместе с этим в функции уже удаляются "ненужные" строки.
        /// </summary>
        /// <returns>Возвращает ранг матрицы.</returns>
        private int RangOfMatrix(List<List<double>> elements_1)
        {
            double first_elem = 0;
            for (int i = 0; i < elements_1.Count; i++)
            {
                int j = 0;
                first_elem = elements_1[i][j];
                //находим ненулевой элемент в строке. если такого нет, то удаляем строку из нулей
                if (first_elem == 0)
                {
                    j = 1;
                    while (first_elem == 0)
                    {
                        first_elem = elements_1[i][j];
                        j++;
                        //если не нашли не нулевого, то удаляем строку из нулей
                        if (j == elements_1[0].Count)
                        {
                            elements_1.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    j--;
                }

                //удалось найти не нулевой
                if (first_elem != 0)
                {
                    for (int p = 0; p < elements_1[0].Count; p++)
                        elements_1[i][p] /= first_elem;
                    for (int k = i + 1; k < elements_1.Count; k++)
                    {
                        if (elements_1[k][j] != 0)
                        {
                            double first_elem_1 = elements_1[k][j];
                            for (int m = 0; m < elements_1[0].Count; m++)
                            {
                                elements_1[k][m] = elements_1[k][m] - elements_1[i][m] * first_elem_1;
                            }
                        }
                    }
                }
            }

            return elements_1.Count;
        }

        /// <summary>
        /// Проверка возможности выражения базисной переменной.
        /// </summary>
        private void CheckCanBeBasix(int index)
        {
            bool isnull = true;
            for (int i = 0; i < elements.Count; i++)
                if (elements[i][index] != 0)
                    isnull = false;
            if (isnull)
                throw new Exception("Невозможно выразить базисную переменную x" + (index + 1) + ", так как в соответствующем столбце №" + (index + 1) + " имеются только нули!");
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (method.SelectedIndex == 0)
                {
                    basix_variables.Visibility = Visibility.Visible;
                    Scrollbasix.Visibility = Visibility.Visible;
                    checkBoxCornerDot.Visibility = Visibility.Visible;
                }
                else
                {
                    basix_variables.Visibility = Visibility.Hidden;
                    Scrollbasix.Visibility = Visibility.Hidden;
                    checkBoxCornerDot.Visibility = Visibility.Hidden;
                }
            }
            catch { }
        }

        private void basix_variables_Loaded(object sender, RoutedEventArgs e)
        {
            add_for_basix();
        }

        private void add_for_basix()
        {
            int width = 0; //измеряем ширину для grid'a
            for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
            {
                if (j == 0)
                {
                    //надстрочный символ
                    Label variable2 = new Label();
                    variable2.Content = "(0)";
                    variable2.FontSize = 7.5;
                    variable2.Width = 20;
                    variable2.Height = 20;
                    variable2.Margin = new Thickness(4, -4, 5, 9);
                    variable2.Name = "label" + j + "_";
                    //регистрируем имя
                    RegisterName(variable2.Name, variable2);
                    Grid.SetColumn(variable2, j);
                    Grid.SetRow(variable2, 0);
                    basix_variables.Children.Add(variable2);


                    Label variable = new Label();
                    variable.Content = "X =(";
                    width += 35;
                    variable.Width = 35;
                    variable.Height = 30;
                    variable.Name = "label" + j;
                    //регистрируем имя
                    RegisterName(variable.Name, variable);
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, 0);
                    basix_variables.Children.Add(variable);
                }
                else if (j % 2 != 0)
                {
                    //создаём новый textbox
                    System.Windows.Controls.TextBox txt = new System.Windows.Controls.TextBox();
                    //присваеваем ему имя, соответствующее определённой ячейке grid'а
                    txt.Name = "textBox" + j;
                    txt.Text = "0";
                    width += 20;
                    txt.Height = 20;
                    txt.Width = 20;
                    //регистрируем имя
                    RegisterName(txt.Name, txt);
                    //устанавливаем столбец
                    Grid.SetColumn(txt, j);
                    //устанавливаем строку
                    Grid.SetRow(txt, 0);
                    //добавляем в grid
                    basix_variables.Children.Add(txt);
                }
                else
                {
                    Label variable = new Label();
                    if (j == basix_variables.ColumnDefinitions.Count - 1)
                        variable.Content = ")";
                    else
                        variable.Content = ",";
                    width += 15;
                    variable.Width = 15;
                    variable.Height = 30;
                    variable.Name = "label" + j;
                    //регистрируем имя
                    RegisterName(variable.Name, variable);
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, 0);
                    basix_variables.Children.Add(variable);
                }
            }
            basix_variables.Width = width;
        }

        private void del_for_basix()
        {
            for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
            {
                if (j % 2 == 0)
                {
                    if (j == 0)
                    {
                        Label variable1 = (Label)entergrid.FindName("label" + j + "_");
                        UnregisterName(variable1.Name);
                        basix_variables.Children.Remove(variable1);
                    }
                    Label variable = (Label)entergrid.FindName("label" + j);
                    UnregisterName(variable.Name);
                    basix_variables.Children.Remove(variable);
                }
                else
                {
                    //находим textbox
                    TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                    //разрегистрируем
                    UnregisterName(txt.Name);
                    //удаляем из grid'а
                    basix_variables.Children.Remove(txt);
                }
            }
        }

        private void targetfunction_Loaded(object sender, RoutedEventArgs e)
        {
            add_for_targetfunction();
        }

        private void add_for_targetfunction()
        {
            int width = 0; //измеряем ширину для grid'a
            num_variable = 1;
            for (int j = 0; j < targetfunction.ColumnDefinitions.Count - 1; j++)
            {
                if (j % 2 == 0)
                {
                    //создаём новый textbox
                    System.Windows.Controls.TextBox txt = new System.Windows.Controls.TextBox();
                    //присваеваем ему имя, соответствующее определённой ячейке grid'а
                    txt.Name = "targetfunctiontextBox" + j;
                    txt.Text = j.ToString(); //УДАЛИТЬ ЭТУ СТРОКУ ДЛЯ АВТОЗАПОЛНЕНИЯ
                    txt.Height = 20;
                    txt.Width = 20;
                    //регистрируем имя
                    RegisterName(txt.Name, txt);
                    //устанавливаем столбец
                    Grid.SetColumn(txt, j);
                    //устанавливаем строку
                    Grid.SetRow(txt, 0);
                    //добавляем в grid
                    targetfunction.Children.Add(txt);
                    width += 20;
                }
                else
                {
                    Label variable = new Label();
                    variable.Content = "*x" + num_variable + "+";
                    variable.HorizontalContentAlignment = HorizontalAlignment.Center;
                    variable.VerticalAlignment = VerticalAlignment.Center;
                    variable.Width = 40;
                    width += 40;
                    variable.Height = 30;
                    variable.Name = "targetfunctionlabel" + j;
                    //регистрируем имя
                    RegisterName(variable.Name, variable);
                    Grid.SetColumn(variable, j);
                    Grid.SetRow(variable, 0);
                    targetfunction.Children.Add(variable);
                    num_variable++;
                }
            }

            Label variable1 = new Label();
            variable1.Content = "*x" + num_variable + " -> min";
            variable1.HorizontalContentAlignment = HorizontalAlignment.Center;
            variable1.VerticalAlignment = VerticalAlignment.Center;
            variable1.Width = 80;
            width += 80;
            variable1.Height = 30;
            variable1.Name = "targetfunctionlabel" + (targetfunction.ColumnDefinitions.Count - 1);
            //регистрируем имя
            RegisterName(variable1.Name, variable1);
            Grid.SetColumn(variable1, targetfunction.ColumnDefinitions.Count - 1);
            Grid.SetRow(variable1, 0);
            targetfunction.Children.Add(variable1);

            targetfunction.Width = width;
        }

        private void del_for_targetfunction()
        {
            for (int j = 0; j < targetfunction.ColumnDefinitions.Count; j++)
            {
                if (j % 2 == 0)
                {
                    //находим textbox
                    TextBox txt = (TextBox)targetfunction.FindName("targetfunctiontextBox" + j);
                    //разрегистрируем
                    UnregisterName(txt.Name);
                    //удаляем из grid'а
                    targetfunction.Children.Remove(txt);
                }
                else
                {
                    Label variable = (Label)targetfunction.FindName("targetfunctionlabel" + j);
                    UnregisterName(variable.Name);
                    targetfunction.Children.Remove(variable);
                }
            }
        }

        private void comboBoxMinMax_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Label variable = (Label)targetfunction.FindName("targetfunctionlabel" + (targetfunction.ColumnDefinitions.Count - 1));
                UnregisterName(variable.Name);
                targetfunction.Children.Remove(variable);

                Label variable1 = new Label();
                if (comboBoxMinMax.SelectedIndex == 0)
                    variable1.Content = "*x" + num_variable + " -> min";
                else variable1.Content = "*x" + num_variable + " -> max";
                variable1.HorizontalContentAlignment = HorizontalAlignment.Center;
                variable1.VerticalAlignment = VerticalAlignment.Center;
                variable1.Width = 80;
                //width += 80;
                variable1.Height = 30;
                variable1.Name = "targetfunctionlabel" + (targetfunction.ColumnDefinitions.Count - 1);
                //регистрируем имя
                RegisterName(variable1.Name, variable1);
                Grid.SetColumn(variable1, targetfunction.ColumnDefinitions.Count - 1);
                Grid.SetRow(variable1, 0);
                targetfunction.Children.Add(variable1);
            }
            catch
            {
            }
        }

        private void checkBoxCornerDot_Unchecked(object sender, RoutedEventArgs e)
        {
            Scrollbasix.Visibility = Visibility.Hidden;
        }

        private void checkBoxCornerDot_Checked_1(object sender, RoutedEventArgs e)
        {
            Scrollbasix.Visibility = Visibility.Visible;
        }

        private void Selection_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxDirectory.Text != "Нет файлов формата .txt")
                {
                    string path = textBoxDirectory.Text;
                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        string line;//считываемая линия
                        string[] elems;//массив элементов

                        //задание размерности
                        line = sr.ReadLine();
                        elems = line.Split();
                        dimension1.SelectedIndex = Int32.Parse(elems[0]) - 1;
                        dimension2.SelectedIndex = Int32.Parse(elems[1]) - 1;


                        //целевая функция
                        int count = 0;//индекс
                        line = sr.ReadLine();
                        elems = line.Split();
                        for (int j = 0; j < targetfunction.ColumnDefinitions.Count; j++)
                        {
                            if (j % 2 == 0)
                            {
                                //находим textbox
                                TextBox txt = (TextBox)targetfunction.FindName("targetfunctiontextBox" + j);
                                txt.Text = elems[count];
                                count++;
                            }
                        }

                        //коэффициенты ограничений-равенств
                        for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
                        {
                            line = sr.ReadLine();
                            elems = line.Split();
                            count = 0;
                            for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                            {
                                if (j % 2 == 0)
                                {
                                    TextBox txt = (TextBox)entergrid.FindName("textBox" + i + "_" + j);
                                    txt.Text = elems[count];
                                    count++;
                                }
                            }
                        }

                        //если выбран симплекс-метод с заданной начальной угловой точкой
                        if (simplex.IsSelected && (checkBoxCornerDot.IsChecked == true))
                        {
                            //коэффициенты угловой точки
                            line = sr.ReadLine();
                            elems = line.Split();
                            count = 0;
                            for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
                            {
                                if (j % 2 != 0)
                                {
                                    TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                                    txt.Text = elems[count];
                                    count++;
                                }
                            }
                        }

                        //min или max
                        line = sr.ReadLine();
                        if (line == "min")
                        {
                            comboBoxMinMax.SelectedIndex = 0;
                        }
                        else comboBoxMinMax.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception a)
            {
                if (textBoxDirectory.Text != "не установлен")
                    MessageBox.Show(a.Message);
            }
            //отображаем открытый файл
            labelLastFile.Content = textBoxDirectory.Text;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            ChooseDirectory CD = new ChooseDirectory();
            CD.Owner = this;
            CD.ShowDialog();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            elements = new List<List<double>>();

            //заполняем массив элементами(коэффициентами), введёнными в текстбоксы
            for (int i = 0; i < entergrid.RowDefinitions.Count; i++)
            {
                elements.Add(new List<double>());
                for (int j = 0; j < entergrid.ColumnDefinitions.Count; j++)
                {
                    if (j % 2 == 0)
                    {
                        //находим textbox
                        TextBox txt = (TextBox)entergrid.FindName("textBox" + i + "_" + j);
                        //добавляем в массив число
                        elements[i].Add(Int32.Parse(txt.Text));
                    }
                }
            }

            //массив для коэффициентов целевой функции
            double[] target_function_elements = new double[Int32.Parse(dimension2.Text)];
            //заполняем массив коэффициентов целевой функции
            FillArrayWithCoefOfGoalFunc(target_function_elements);

            double[] basix_vars = new double[Int32.Parse(dimension2.Text)];
            int index = 0;
            //перестановки в матрице и массиве визуализации
            for (int j = 0; j < basix_variables.ColumnDefinitions.Count; j++)
            {
                if (j % 2 != 0)
                {
                    //находим textbox
                    TextBox txt = (TextBox)entergrid.FindName("textBox" + j);
                    basix_vars[index] = double.Parse(txt.Text);
                    index++;
                }
            }

            ChooseDirectory CD = new ChooseDirectory(Int32.Parse(dimension1.Text), Int32.Parse(dimension2.Text), target_function_elements, elements, basix_vars, comboBoxMinMax.SelectedIndex);
            CD.Owner = this;
            CD.ShowDialog();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InformationWindow IW = new InformationWindow();
            IW.Owner = this;
            IW.ShowDialog();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow HW = new simplex_method.HelpWindow();
            HW.Owner = this;
            HW.Show();
        }

        /// <summary>
        /// При закрытии главного окна закрываются все дочерние.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            //foreach (Window w in App.Current.Windows)
            //    w.Close();
        }
    }
}
