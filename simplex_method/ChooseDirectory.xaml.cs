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
using System.IO;
using System.Threading.Tasks;
using OrdinaryFractionLibrary;

namespace simplex_method
{
    /// <summary>
    /// Окно для открытия или сохранения файла.
    /// </summary>
    public partial class ChooseDirectory : Window
    {
        /// <summary>
        /// Путь каталога.
        /// </summary>
        public string path = "не установлено";
        /// <summary>
        /// Буффер выбранных каталогов для возврата назад.
        /// </summary>
        List<string> buffer_path = new List<string>();
        /// <summary>
        /// Количество ограничений.
        /// </summary>
        int number_of_restrictions;
        /// <summary>
        /// Количество переменных.
        /// </summary>
        int number_of_variables;
        /// <summary>
        /// Коэффициенты целевой функции.
        /// </summary>
        double[] target_function_elements;
        /// <summary>
        /// Коэффициенты системы ограничений-равенств.
        /// </summary>
        List<List<double>> elements;
        /// <summary>
        /// Базисные переменные.
        /// </summary>
        double[] basix_vars;
        /// <summary>
        /// Ищется минимум или максимум.(0-min,1-max)
        /// </summary>
        int MinMax;

        /// <summary>
        /// Коэффициенты (обык.дроби) целевой функции.
        /// </summary>
        ordinary_fraction[] target_function_fractions;
        /// <summary>
        /// Коэффициенты (обык.дроби) системы ограничений-равенств.
        /// </summary>
        List<List<ordinary_fraction>> fractions;

        bool? decimals_or_simple;

        /// <summary>
        /// Конструктор окна с выбором файла.
        /// </summary>
        public ChooseDirectory()
        {
            //инициализация окна
            InitializeComponent();

            //получаем массив дисков, которые есть на компьютере
            DriveInfo[] drives = DriveInfo.GetDrives();

            //добавляем все диски в листбокс
            foreach (DriveInfo drive in drives)
            {
                listBoxDirectories.Items.Add(drive.Name);
            }

            //Меняем название окна на "Выберите диск..."
            this.Title = "Выберите диск...";

            //кнопка возврата в предыдущюю папку по началу(при выборе диска) недоступна
            buttonBac.IsEnabled = false;

            //кнопка показа файлов не доступна, так как нужно выбрать сначала диск
            ShowFiles.IsEnabled = false;
        }

        /// <summary>
        /// Конструктор окна с сохранением файла.
        /// </summary>
        /// <param name="number_of_restrictions">Количество ограничений.</param>
        /// <param name="number_of_variables">Количество переменных.</param>
        /// <param name="target_function_elements">Коэффициенты целевой функции.</param>
        /// <param name="elements">Коэффициенты системы ограничений-равенств.</param>
        /// <param name="basix_vars">Базисные переменные.</param>
        /// <param name="MinMax">Ищется минимум или максимум.(0-min,1-max).</param>
        public ChooseDirectory(int number_of_restrictions, int number_of_variables, double[] target_function_elements, List<List<double>> elements, double[] basix_vars, int MinMax, bool? decimals_or_simple)
        {
            //инициализация окна
            InitializeComponent();
            //Количество ограничений.
            this.number_of_restrictions = number_of_restrictions;
            //Количество переменных.
            this.number_of_variables = number_of_variables;
            //Коэффициенты целевой функции.
            this.target_function_elements = target_function_elements;
            //Коэффициенты системы ограничений-равенств.
            this.elements = elements;
            //Базисные переменные.
            this.basix_vars = basix_vars;
            //Ищется минимум или максимум.(0-min,1-max).
            this.MinMax = MinMax;
            this.decimals_or_simple = decimals_or_simple;


            //получаем массив дисков, которые есть на компьютере
            DriveInfo[] drives = DriveInfo.GetDrives();

            //добавляем все диски в листбокс
            foreach (DriveInfo drive in drives)
            {
                listBoxDirectories.Items.Add(drive.Name);
            }

            //Меняем название окна на "Выберите диск..."
            this.Title = "Выберите диск...";

            //кнопка возврата в предыдущюю папку по началу(при выборе диска) недоступна
            buttonBac.IsEnabled = false;

            //удаляем событие показа файлов формата .txt
            ShowFiles.Click -= new RoutedEventHandler(ShowFiles_Click);
            //добавляем событие сохранения файла
            ShowFiles.Click += new RoutedEventHandler(ShowFiles_Click_2);

            //удаляем событие показа окна помощи по открытию файла
            help.Click -= new RoutedEventHandler(MenuItem_Click);
            //добавляем событие показа окна помощи по сохранению файла
            help.Click += new RoutedEventHandler(MenuItem_Click_2);

            //меняем картинку menuitem
            ImageShowFiles.Source = new BitmapImage(new Uri(@"save.ico", UriKind.RelativeOrAbsolute));
            //меняем текст menuitem
            ShowFiles.Header = "Сохранить";

            //label "Имя файла:" теперь виден
            labelFileName.Visibility = Visibility.Visible;
            //поле ввода для имени файла теперь видно
            textBoxFileName.Visibility = Visibility.Visible;

            //увеличиваем высоту окна
            this.Height = 380;
        }

        /// <summary>
        /// Конструктор окна с сохранением файла.
        /// </summary>
        /// <param name="number_of_restrictions">Количество ограничений.</param>
        /// <param name="number_of_variables">Количество переменных.</param>
        /// <param name="target_function_elements">Коэффициенты целевой функции.</param>
        /// <param name="elements">Коэффициенты системы ограничений-равенств.</param>
        /// <param name="basix_vars">Базисные переменные.</param>
        /// <param name="MinMax">Ищется минимум или максимум.(0-min,1-max).</param>
        public ChooseDirectory(int number_of_restrictions, int number_of_variables, ordinary_fraction[] target_function_elements, List<List<ordinary_fraction>> fractions, double[] basix_vars, int MinMax, bool? decimals_or_simple)
        {
            //инициализация окна
            InitializeComponent();
            //Количество ограничений.
            this.number_of_restrictions = number_of_restrictions;
            //Количество переменных.
            this.number_of_variables = number_of_variables;
            //Коэффициенты целевой функции.
            this.target_function_fractions = target_function_elements;
            //Коэффициенты системы ограничений-равенств.
            this.fractions = fractions;
            //Базисные переменные.
            this.basix_vars = basix_vars;
            //Ищется минимум или максимум.(0-min,1-max).
            this.MinMax = MinMax;
            this.decimals_or_simple = decimals_or_simple;


            //получаем массив дисков, которые есть на компьютере
            DriveInfo[] drives = DriveInfo.GetDrives();

            //добавляем все диски в листбокс
            foreach (DriveInfo drive in drives)
            {
                listBoxDirectories.Items.Add(drive.Name);
            }

            //Меняем название окна на "Выберите диск..."
            this.Title = "Выберите диск...";

            //кнопка возврата в предыдущюю папку по началу(при выборе диска) недоступна
            buttonBac.IsEnabled = false;

            //удаляем событие показа файлов формата .txt
            ShowFiles.Click -= new RoutedEventHandler(ShowFiles_Click);
            //добавляем событие сохранения файла
            ShowFiles.Click += new RoutedEventHandler(ShowFiles_Click_2);

            //удаляем событие показа окна помощи по открытию файла
            help.Click -= new RoutedEventHandler(MenuItem_Click);
            //добавляем событие показа окна помощи по сохранению файла
            help.Click += new RoutedEventHandler(MenuItem_Click_2);

            //меняем картинку menuitem
            ImageShowFiles.Source = new BitmapImage(new Uri(@"save.ico", UriKind.RelativeOrAbsolute));
            //меняем текст menuitem
            ShowFiles.Header = "Сохранить";

            //label "Имя файла:" теперь виден
            labelFileName.Visibility = Visibility.Visible;
            //поле ввода для имени файла теперь видно
            textBoxFileName.Visibility = Visibility.Visible;

            //увеличиваем высоту окна
            this.Height = 380;
        }

        /// <summary>
        /// Нажатие на элемент листбокса при выборе папки.
        /// </summary>
        private void listBoxDirectories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try catch используется для отлавливания ошибки, когда метод listBoxDirectories.Items.Clear() инициирует вызов события listBoxDirectories_SelectionChanged(так как меняется SelectionItem), но теперь SelectionItem не ссылается на объект и возникает ошибка
            try
            {
                //ловим ошибку с свойством SelectedItem(описано выше)
                listBoxDirectories.SelectedItem.ToString();

                //начальная страница с выбором диска в буфере соответствует значению "не установлено"
                if (buffer_path.Count == 0)
                    buffer_path.Add("не установлено");
                else
                {
                    //буферизируем путь (при возникновении ошибки путь сохранится)
                    buffer_path.Add(path);
                }

                //присваиваем выбранный путь переменной path
                path = listBoxDirectories.SelectedItem.ToString();

                //очищаем листбокс
                listBoxDirectories.Items.Clear();

                //Меняем название окна на "Выберите каталог..."
                this.Title = "Выберите каталог...";

                //получаем массив директорий по выбранному пути
                string[] dirs = Directory.GetDirectories(path);
                //заполняем листбокс
                foreach (string s in dirs)
                {
                    listBoxDirectories.Items.Add(s);
                }

                //даём доступ к кнопке возвращания к предыдущему каталогу
                buttonBac.IsEnabled = true;
                //кнопка показа файлов доступна
                ShowFiles.IsEnabled = true;
            }
            //в отлавливателе ничего не делаем
            catch (Exception d)
            {
            }
        }
        
        /// <summary>
        /// Кнопка возврата назад к предыдущему каталогу.
        /// </summary>
        private void buttonBac_Click(object sender, RoutedEventArgs e)
        {
            //возвращаем из буфера директорию
            path = buffer_path.Last();
            //удаляем эту запись в буфере
            buffer_path.RemoveAt(buffer_path.Count - 1);

            //обновляем листбокс
            //очищаем листбокс
            listBoxDirectories.Items.Clear();
            //если предыдущий это выбор диска
            if (path == "не установлено")
            {
                //кнопка возврата назад теперь не доступна
                buttonBac.IsEnabled = false;

                //кнопка показа файлов не доступна, так как нужно выбрать сначала диск
                ShowFiles.IsEnabled = false;

                //Меняем название окна на "Выберите диск..."
                this.Title = "Выберите диск...";

                //получаем массив дисков, которые есть на компьютере
                DriveInfo[] drives = DriveInfo.GetDrives();
                //добавляем диски в листбокс
                foreach (DriveInfo drive in drives)
                {
                    listBoxDirectories.Items.Add(drive.Name);
                }
            }
            else
            {
                //получаем список(массив) папок в каталоге
                string[] dirs = Directory.GetDirectories(path);
                //добавляем каталоги в листбокс
                foreach (string s in dirs)
                {
                    listBoxDirectories.Items.Add(s);
                }

                //Меняем название окна на "Выберите каталог..."
                this.Title = "Выберите каталог...";

                //кнопка показа файлов доступна
                ShowFiles.IsEnabled = true;
            }

            //листбокс с выбором директории виден
            listBoxDirectories.Visibility = Visibility.Visible;
            //листбокс с выбором файла скрыт
            listBoxFiles.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Событие выбора файла в листбоксе с файлами.
        /// </summary>
        private void listBoxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //меняем вывод текущей директории в текстбоксе главного окна
                MainWindow.textBoxDirectory.Text = listBoxFiles.SelectedItem.ToString();
            }
            catch (Exception d)
            {

            }
        }

        /// <summary>
        /// Кнопка выбора диска.
        /// </summary>
        private void ChooseDisc_Click(object sender, RoutedEventArgs e)
        {
            //очищаем буфер
            buffer_path.Clear();

            //обновляем листбокс
            //очищаем листбокс
            listBoxDirectories.Items.Clear();
            //получаем массив дисков, которые есть на компьютере
            DriveInfo[] drives = DriveInfo.GetDrives();
            //заполняем дисками листбокс
            foreach (DriveInfo drive in drives)
            {
                listBoxDirectories.Items.Add(drive.Name);
            }

            //Меняем название окна на "Выберите диск..."
            this.Title = "Выберите диск...";

            //кнопка возврата назад недоступна
            buttonBac.IsEnabled = false;
            //кнопка показа файлов не доступна, так как нужно выбрать сначала диск
            ShowFiles.IsEnabled = false;
            //листбокс с выбором директории виден
            listBoxDirectories.Visibility = Visibility.Visible;
            //листбокс с выбором файла скрыт
            listBoxFiles.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Кнопка показа файлов директории.
        /// </summary>
        private void ShowFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //получаем список(массив) файлов в папке
                string[] files = Directory.GetFiles(path);//здесь отлавливается ошибка(при помощи try catch), когда неверно указан путь(path), а он неверно указан только в случае, когда пользователь должен выбрать диск


                //добавляем путь(path) для возврата назад
                buffer_path.Add(path);

                //обновляем листбокс
                //очищаем листбокс
                listBoxFiles.Items.Clear();
                //добавляем список файлов в листбокс
                foreach (string s in files)
                {
                    if (s.EndsWith("txt"))
                        listBoxFiles.Items.Add(s);
                }

                //если нет файлов в директории
                if (listBoxFiles.Items.Count == 0)
                    listBoxFiles.Items.Add("Нет файлов формата .txt");

                //кнопка показа файлов теперь доступна
                ShowFiles.IsEnabled = false;

                //Меняем название окна на "Выберите файл..."
                this.Title = "Выберите файл...";

                //листбокс с выбором директории теперь скрыт
                listBoxDirectories.Visibility = Visibility.Hidden;
                //листбокс с выбором файла теперь виден
                listBoxFiles.Visibility = Visibility.Visible;
            }
            //отлавливаем ошибка, когда не выбрана директория
            catch (DirectoryNotFoundException d)
            {
                MessageBox.Show("Не выбран диск!");
            }
            catch (UnauthorizedAccessException c)
            {
                MessageBox.Show("Нет прав доступа к каталогу " + path, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                //если нет доступа, то назад к выбору директории
                buttonBac_Click(new object(), new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Кнопка сохранения файла.
        /// </summary>
        private void ShowFiles_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (decimals_or_simple==true)
                {
                    //директория для сохранения файла
                    string path_path = path + "\\" + textBoxFileName.Text;
                    FileInfo filesave = new FileInfo(path_path);

                    //создаём новый файл
                    using (FileStream fs = filesave.Create()) { }

                    //рабочая строка
                    string str = "";

                    //открываем поток для записи
                    using (StreamWriter sw = new StreamWriter(path_path, false, System.Text.Encoding.Default))
                    {
                        //количество ограничений и количество переменных
                        str = number_of_restrictions + " " + number_of_variables;
                        //записываем
                        sw.WriteLine(str);

                        str = "";

                        //элементы целевой функции
                        for (int i = 0; i < target_function_elements.Length; i++)
                        {
                            str += target_function_elements[i].ToString() + " ";
                        }
                        //записываем
                        sw.WriteLine(str);

                        //коэффициенты системы ограничений-равенств
                        for (int i = 0; i < elements.Count; i++)
                        {
                            str = "";
                            for (int j = 0; j < elements[0].Count; j++)
                            {
                                str += elements[i][j].ToString() + " ";
                            }
                            //записываем строку коэффициентов
                            sw.WriteLine(str);
                        }

                        str = "";

                        //элементы начальной угловой точки
                        for (int i = 0; i < basix_vars.Length; i++)
                        {
                            str += basix_vars[i].ToString() + " ";
                        }
                        //записываем строку коэффициентов
                        sw.WriteLine(str);

                        //min или max
                        if (MinMax == 0)
                            str = "min";
                        else str = "max";
                        //записываем
                        sw.WriteLine(str);

                        MessageBox.Show("Сохранено!");
                    }
                }
                else
                {
                    //директория для сохранения файла
                    string path_path = path + "\\" + textBoxFileName.Text;
                    FileInfo filesave = new FileInfo(path_path);

                    //создаём новый файл
                    using (FileStream fs = filesave.Create()) { }

                    //рабочая строка
                    string str = "";

                    //открываем поток для записи
                    using (StreamWriter sw = new StreamWriter(path_path, false, System.Text.Encoding.Default))
                    {
                        //количество ограничений и количество переменных
                        str = number_of_restrictions + " " + number_of_variables;
                        //записываем
                        sw.WriteLine(str);

                        str = "";

                        //элементы целевой функции
                        for (int i = 0; i < target_function_fractions.Length; i++)
                        {
                            str += target_function_fractions[i].ToString() + " ";
                        }
                        //записываем
                        sw.WriteLine(str);

                        //коэффициенты системы ограничений-равенств
                        for (int i = 0; i < fractions.Count; i++)
                        {
                            str = "";
                            for (int j = 0; j < fractions[0].Count; j++)
                            {
                                str += fractions[i][j].ToString() + " ";
                            }
                            //записываем строку коэффициентов
                            sw.WriteLine(str);
                        }

                        str = "";

                        //элементы начальной угловой точки
                        for (int i = 0; i < basix_vars.Length; i++)
                        {
                            str += basix_vars[i].ToString() + " ";
                        }
                        //записываем строку коэффициентов
                        sw.WriteLine(str);

                        //min или max
                        if (MinMax == 0)
                            str = "min";
                        else str = "max";
                        //записываем
                        sw.WriteLine(str);

                        MessageBox.Show("Сохранено!");
                    }
                }
            }
            catch(ArgumentException)
            {
                if (path == "не установлено")
                    MessageBox.Show("Не выбран каталог для сохранения!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                else MessageBox.Show("Недопустимые знаки в имени файла \""+ textBoxFileName.Text + "\" !", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Кнопка выхода.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Открываем окно помощи по открытию файла.
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow1 HW = new HelpWindow1();
            HW.Owner = this;
            HW.Show();
        }

        /// <summary>
        /// Открываем окно помощи по сохранению файла.
        /// </summary>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            HelpWindow2 HW = new HelpWindow2();
            HW.Owner = this;
            HW.Show();
        }
    }
}
