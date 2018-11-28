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

namespace simplex_method
{
    /// <summary>
    /// Interaction logic for StepByStepArtificialBasisWindow.xaml
    /// </summary>
    public partial class StepByStepArtificialBasisWindow : Window
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
        /// <summary>
        /// Текущий шаг.
        /// </summary>
        public static int step = 1;
        /// <summary>
        /// Тип шага. True - обычный, false - холостой.
        /// </summary>
        List<bool> type_of_step = new List<bool>();
        bool memory;
        bool first;
        bool simplex_table_was_draw;
        /// <summary>
        /// Ищем минимум или максимум(0-минимум,1-максимум).
        /// </summary>
        int MinMax;
        int step_1;

        public StepByStepArtificialBasisWindow(List<List<double>> elements, int number_of_basix, int[] variable_visualization, double[] target_function_elements, int MinMax)
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
            step = 1;
            step_1 = 0;
            memory = false;
            simplex_table_was_draw = false;
            this.MinMax = MinMax;

            //Процесс выполнения.
            Implementation();
        }

        /// <summary>
        /// Выполнение.
        /// </summary>
        private void Implementation()
        {
            //создаём сиплекс-таблицу
            simplextable = new SimplexTable(number_of_basix, number_of_free_variables, variable_visualization, elements, target_function_elements, false);
            MainGrid.Children.Add(simplextable);
            //добавляем тильду
            simplextable.AddTilde();
            if (simplextable.ResponseCheck() == 1)
            {
                //type_of_step.Add(false);
                first = false;
                labelsteps.Content = "Холостой шаг: Метод искусственного базиса. Выбор опорного элемента.";
                //холостой шаг
                simplextable.IdleStep();
                this.Width = 720;
            }
            else
            {
                //type_of_step.Add(true);
                first = true;
                labelsteps.Content = "Шаг " + step + ": Метод искусственного базиса. Выбор опорного элемента.";
                //выбор опорного
                simplextable.SelectionOfTheSupportElement();
                this.Width = 651;
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

        /// <summary>
        /// Кнопка "Вперёд".
        /// </summary>
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //выбран ли опорный элемент
                simplextable.ButtonPressedOrNot();
                memory = false;
                //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                simplextable.ChangeOfVisualizationVariables();
                //буферизация данных
                //simplextable.BufferingSimplexTableValues(step + 3);
                simplextable.BufferingSimplexTableValuesTest();
                //удаляем кнопки
                simplextable.DeleteButtons();
                //вычисление согласно выбранному опорному элементу
                simplextable.CalculateSimplexTable();
                //обновление данных сиплекс-таблицы
                simplextable.UpdateSimplexTableValues();
                simplextable.CornerPoint(step);
                if (step == 1)
                {
                    type_of_step.Add(first);
                }
                //проверка решения
                switch (simplextable.ArtificialResponseCheck())
                {
                    case true:
                        variable_visualization = simplextable.ReturnVariableVisualization();
                        elements = simplextable.ReturnElements();

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

                        buttonBack.Visibility = Visibility.Hidden;
                        buttonNext.Visibility = Visibility.Hidden;
                        buttonBack1.Visibility = Visibility.Visible;
                        buttonNext1.Visibility = Visibility.Visible;
                        simplextable.HideSimplexTable();
                        step++;
                        if (simplex_table_was_draw == false)
                        {
                            simplextable1 = new SimplexTable(number_of_basix, variable_visualization.Length - number_of_basix, variable_visualization, elements, target_function_elements, true);
                            MainGrid.Children.Add(simplextable1);
                            //Симплекс-таблица была создана
                            simplex_table_was_draw = true;
                        }
                        else
                        {
                            simplextable1.DeleteButtons();
                            simplextable.HideSimplexTable();
                            simplextable1.VisibleSimplexTable();
                            MainGrid.Children.Add(simplextable1);
                        }
                        //проверка решения
                        switch (simplextable1.ResponseCheck())
                        {
                            case 0:
                                //выбор опорного
                                simplextable1.SelectionOfTheSupportElement();
                                labelsteps.Content = "Шаг " + step + ": Симплекс-таблица.";
                                break;
                            case 1:
                                if (MinMax == 0)
                                    labelsteps.Content = "Ответ :" + simplextable1.Response() * (-1);
                                else labelsteps.Content = "Ответ :" + simplextable1.Response();
                                buttonToMainWindow.Visibility = Visibility.Visible;
                                buttonNext1.Visibility = Visibility.Hidden;
                                break;
                            case -1:
                                labelsteps.Content = "Задача не разрешима!";
                                buttonToMainWindow.Visibility = Visibility.Visible;
                                buttonNext1.Visibility = Visibility.Hidden;
                                break;
                        }
                        break;
                    case false:
                        if (simplextable.ResponseCheck() == 1)
                        {
                            type_of_step.Add(false);
                            step++;
                            labelsteps.Content = "Холостой шаг: Метод искусственного базиса. Выбор опорного элемента.";
                            //холостой шаг
                            simplextable.IdleStep();
                            this.Width = 720;
                        }
                        else
                        {
                            type_of_step.Add(true);
                            step++;
                            labelsteps.Content = "Шаг " + step + ": Метод искусственного базиса. Выбор опорного элемента.";
                            //выбор опорного
                            simplextable.SelectionOfTheSupportElement();
                            this.Width = 651;
                        }
                        break;
                }
            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        /// <summary>
        /// Кнопка "Назад".
        /// </summary>
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            if (step == 1)
            {
                //создаём экземпляр главного окна
                MainWindow MW = new MainWindow();
                //открываем главное окно
                MW.Show();
                //закрываем это окно
                this.Close();
            }
            else
            {
                //убираем кнопки
                simplextable.DeleteButtons();
                //возвращение данных из буфера
                simplextable.GetOutOfTheBufferSimplexTest();
                simplextable.GetOutOfTheBufferVisualizationVariablesTest();
                //simplextable.GetOutOfTheBufferSimplex(step + 3);
                //simplextable.GetOutOfTheBufferVisualizationVariables(step + 3);
                //обновление данных сиплекс-таблицы
                simplextable.UpdateSimplexTableValues();
                if (memory == false)
                {
                    if (type_of_step[type_of_step.Count - 2] == true)
                    {
                        //выбор опорного
                        simplextable.SelectionOfTheSupportElement();
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                    }
                    else
                    {
                        //холостой шаг
                        simplextable.IdleStep();
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                    }
                    memory = true;
                }
                else
                {
                    if (type_of_step[type_of_step.Count - 1] == true)
                    {
                        //выбор опорного
                        simplextable.SelectionOfTheSupportElement();
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                    }
                    else
                    {
                        //холостой шаг
                        simplextable.IdleStep();
                        type_of_step.RemoveAt(type_of_step.Count - 1);
                    }
                }
                step--;
                labelsteps.Content = "Шаг " + step + ": Симплекс-таблица. Выбор опорного элемента.";
                buttonToMainWindow.Visibility = Visibility.Hidden;
                buttonNext1.Visibility = Visibility.Visible;
                simplextable.CornerPoint(step - 1);
            }
        }

        private void buttonNext1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //выбран ли опорный элемент
                simplextable1.ButtonPressedOrNot();
                //Смена местами визуализаций переменных(после выбора опорного элемента) + буферизация.
                simplextable1.ChangeOfVisualizationVariables();
                //буферизация данных
                //simplextable1.BufferingSimplexTableValues(step_1 + 3);
                simplextable1.BufferingSimplexTableValuesTest();
                //удаляем кнопки
                simplextable1.DeleteButtons();
                //вычисление согласно выбранному опорному элементу
                simplextable1.CalculateSimplexTable();
                //обновление данных сиплекс-таблицы
                simplextable1.UpdateSimplexTableValues();
                switch (simplextable1.ResponseCheck())
                {
                    case 0:
                        step_1++;
                        labelsteps.Content = "Шаг " + step + ": Симплекс-таблица. Выбор опорного элемента.";
                        //выбор опорного
                        simplextable1.SelectionOfTheSupportElement();
                        break;
                    case 1:
                        step_1++;
                        if (MinMax == 0)
                            labelsteps.Content = "Ответ :" + simplextable1.Response() * (-1);
                        else labelsteps.Content = "Ответ :" + simplextable1.Response();
                        buttonToMainWindow.Visibility = Visibility.Visible;
                        buttonNext1.Visibility = Visibility.Hidden;
                        break;
                    case -1:
                        step_1++;
                        labelsteps.Content = "Задача не разрешима!";
                        buttonToMainWindow.Visibility = Visibility.Visible;
                        buttonNext1.Visibility = Visibility.Hidden;
                        break;
                }
                simplextable1.CornerPoint(step_1);
            }
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void buttonBack1_Click(object sender, RoutedEventArgs e)
        {
            if (step_1 != 0)
            {
                //убираем кнопки
                simplextable1.DeleteButtons();
                //возвращение данных из буфера
                simplextable1.GetOutOfTheBufferSimplexTest();
                //simplextable.GetOutOfTheBufferSimplex(step_1 + step);
                //simplextable.GetOutOfTheBufferVisualizationVariables(step_1 + step);
                simplextable1.GetOutOfTheBufferVisualizationVariablesTest();
                //обновление данных сиплекс-таблицы
                simplextable1.UpdateSimplexTableValues();
                //выбор опорного
                simplextable1.SelectionOfTheSupportElement();
                step_1--;
                labelsteps.Content = "Шаг " + step_1 + ": Симплекс-таблица. Выбор опорного элемента.";
                buttonToMainWindow.Visibility = Visibility.Hidden;
                buttonNext1.Visibility = Visibility.Visible;
                simplextable1.CornerPoint(step_1);
            }
            else
            {
                simplextable1.HideSimplexTable();
                MainGrid.Children.Remove(simplextable1);
                simplextable.VisibleSimplexTable();
                memory = true;
                //simplextable.GetOutOfTheBufferSimplexTest();
                buttonBack1.Visibility = Visibility.Hidden;
                buttonNext1.Visibility = Visibility.Hidden;
                buttonBack.Visibility = Visibility.Visible;
                buttonNext.Visibility = Visibility.Visible;
            }
        }
    }
}
