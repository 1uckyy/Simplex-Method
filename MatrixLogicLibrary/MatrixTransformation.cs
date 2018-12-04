using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrdinaryFractionLibrary;

namespace MatrixLogicLibrary
{
    public static class MatrixTransformation
    {
        /// <summary>
        /// Прямой ход метода Гаусса для приведения к треугольному виду.
        /// </summary>
        /// <param name="elements">Двумерный массив коэффициентов системы линейных ограничений-равенств.</param>
        /// <param name="CornerDot">Была ли введена угловая точка(false - нет, true - да).</param>
        /// <param name="variable_visualization">Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).</param>
        public static void Gauss(List<List<double>> elements, bool? CornerDot, int[] variable_visualization)
        {
            for (int global = 0; global < elements.Count; global++)
            {
                for (int i = global; i < elements.Count; i++)
                {
                    if (i == global)
                    {
                        //проверяем возможность выражения переменной
                        double first_elem = elements[i][global];
                        bool responce = true; //можно ли вообще выразить
                        if (first_elem == 0)
                        {
                            responce = false;
                            for (int k = i + 1; k < elements.Count; k++)
                                if (elements[k][global] != 0)
                                {
                                    responce = true;
                                    first_elem = elements[k][global];
                                    double temp;
                                    //смена строк
                                    for (int j = 0; j < elements[0].Count; j++)
                                    {
                                        temp = elements[i][j];
                                        elements[i][j] = elements[k][j];
                                        elements[k][j] = temp;
                                    }
                                    break;
                                }
                        }

                        //если не получилось выразить переменную и была задана начальная угловая точка
                        if ((responce == false) && (CornerDot == true))
                            throw new Exception("Невозможно выразить одну или несколько базисных переменных. Возможно неверно введены коэффициенты.");
                        //если не получилось выразить переменную и НЕ была задана начальная угловая точка
                        else if ((responce == false) && (CornerDot == false))
                        {
                            //то ищем в других столбцах
                            bool check = false;
                            for (int column = global + 1; column < elements[0].Count; column++)
                            {
                                for (int row = i; row < elements.Count; row++)
                                {
                                    if (elements[row][column] != 0)
                                    {
                                        check = true;
                                        first_elem = elements[row][column];
                                        double temp; //вспомогательная переменная

                                        //смена строк
                                        for (int j = 0; j < elements[0].Count; j++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[i][j];
                                            elements[i][j] = elements[row][j];
                                            elements[row][j] = temp;
                                        }

                                        //смена столбцов
                                        for(int k = 0; k < elements.Count; k++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[k][global];
                                            elements[k][global] = elements[k][column];
                                            elements[k][column] = temp;
                                        }

                                        //для массива визуализации
                                        temp = variable_visualization[global];
                                        variable_visualization[global] = variable_visualization[column];
                                        variable_visualization[column] = (int)temp;
                                    }

                                    if (check)
                                        break;
                                }
                                if (check)
                                    break;
                            }

                            //Такого случая возможно не может быть. Поэтому это излишне.
                            if (check == false)
                                throw new Exception("Невозможно выразить переменные. Возможно неверно введены коэффициенты.");
                        }



                        for (int j = 0; j < elements[0].Count; j++)
                            elements[i][j] /= first_elem;
                    }
                    else
                    {
                        double first_elem = elements[i][global];
                        for (int j = 0; j < elements[0].Count; j++)
                        {
                            elements[i][j] -= elements[global][j] * first_elem;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Прямой ход метода Гаусса для приведения к треугольному виду.
        /// </summary>
        /// <param name="elements">Двумерный массив коэффициентов системы линейных ограничений-равенств.</param>
        /// <param name="CornerDot">Была ли введена угловая точка(false - нет, true - да).</param>
        /// <param name="variable_visualization">Вспомогательный массив для визуализации переменных(по типу x1,x2,x3 и т.д.).</param>
        public static void Gauss(List<List<ordinary_fraction>> elements, bool? CornerDot, int[] variable_visualization)
        {
            for (int global = 0; global < elements.Count; global++)
            {
                for (int i = global; i < elements.Count; i++)
                {
                    if (i == global)
                    {
                        //проверяем возможность выражения переменной
                        ordinary_fraction first_elem = elements[i][global];
                        bool responce = true; //можно ли вообще выразить
                        if (first_elem.top_number == 0)
                        {
                            responce = false;
                            for (int k = i + 1; k < elements.Count; k++)
                                if (elements[k][global].top_number != 0)
                                {
                                    responce = true;
                                    first_elem = elements[k][global];
                                    ordinary_fraction temp;
                                    //смена строк
                                    for (int j = 0; j < elements[0].Count; j++)
                                    {
                                        temp = elements[i][j];
                                        elements[i][j] = elements[k][j];
                                        elements[k][j] = temp;
                                    }
                                    break;
                                }
                        }

                        //если не получилось выразить переменную и была задана начальная угловая точка
                        if ((responce == false) && (CornerDot == true))
                            throw new Exception("Невозможно выразить одну или несколько базисных переменных. Возможно неверно введены коэффициенты.");
                        //если не получилось выразить переменную и НЕ была задана начальная угловая точка
                        else if ((responce == false) && (CornerDot == false))
                        {
                            //то ищем в других столбцах
                            bool check = false;
                            for (int column = global + 1; column < elements[0].Count; column++)
                            {
                                for (int row = i; row < elements.Count; row++)
                                {
                                    if (elements[row][column].top_number != 0)
                                    {
                                        check = true;
                                        first_elem = elements[row][column];
                                        ordinary_fraction temp; //вспомогательная переменная
                                        int temp1; //вспомогательная переменная

                                        //смена строк
                                        for (int j = 0; j < elements[0].Count; j++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[i][j];
                                            elements[i][j] = elements[row][j];
                                            elements[row][j] = temp;
                                        }

                                        //смена столбцов
                                        for (int k = 0; k < elements.Count; k++)
                                        {
                                            //для элементов матрицы
                                            temp = elements[k][global];
                                            elements[k][global] = elements[k][column];
                                            elements[k][column] = temp;
                                        }

                                        //для массива визуализации
                                        temp1 = variable_visualization[global];
                                        variable_visualization[global] = variable_visualization[column];
                                        variable_visualization[column] = temp1;
                                    }

                                    if (check)
                                        break;
                                }
                                if (check)
                                    break;
                            }

                            //Такого случая возможно не может быть. Поэтому это излишне.
                            if (check == false)
                                throw new Exception("Невозможно выразить переменные. Возможно неверно введены коэффициенты.");
                        }



                        for (int j = 0; j < elements[0].Count; j++)
                            elements[i][j] /= first_elem;
                    }
                    else
                    {
                        ordinary_fraction first_elem = elements[i][global];
                        for (int j = 0; j < elements[0].Count; j++)
                        {
                            elements[i][j] -= elements[global][j] * first_elem;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Выражение базисных переменных.
        /// </summary>
        /// <param name="elements">Двумерный массив коэффициентов системы линейных ограничений-равенств.</param>
        /// <param name="number">Количество базисных переменных.</param>
        public static void HoistingMatrix(List<List<double>> elements, int number)
        {
            for (int global = 1; global < number; global++)
            {
                for (int i = global - 1; i >= 0; i--)
                {
                    double first_elem = elements[i][global];
                    for (int j = global; j < elements[0].Count; j++)
                    {
                        elements[i][j] -= elements[global][j] * first_elem;
                    }
                }
            }
        }

        /// <summary>
        /// Выражение базисных переменных.
        /// </summary>
        /// <param name="elements">Двумерный массив коэффициентов системы линейных ограничений-равенств.</param>
        /// <param name="number">Количество базисных переменных.</param>
        public static void HoistingMatrix(List<List<ordinary_fraction>> elements, int number)
        {
            for (int global = 1; global < number; global++)
            {
                for (int i = global - 1; i >= 0; i--)
                {
                    ordinary_fraction first_elem = elements[i][global];
                    for (int j = global; j < elements[0].Count; j++)
                    {
                        elements[i][j] -= elements[global][j] * first_elem;
                    }
                }
            }
        }
    }
}
