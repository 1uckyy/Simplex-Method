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

namespace DrawMatricesAndTables
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        //отрисовка закруглённых скобок матрицы и вертикальной черты, отделяющей столбец свободных членов
        public void draw_extended_matrix(Grid gaussgrid)
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
                myLine.Name = "line" + i + (gaussgrid.ColumnDefinitions.Count - 1);
                RegisterName(myLine.Name, myLine);
                Grid.SetColumn(myLine, gaussgrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(myLine, i);
                gaussgrid.Children.Add(myLine);
            }



            //отрисовка левой скобки матрицы (без закруглений)
            for (int i = 2; i < gaussgrid.RowDefinitions.Count - 1; i++) //НЕ УДАЛЯЮ, НУЖНО ДОБАВИТЬ УДАЛЕНИЕ
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.X1 = 6;
                myLine.X2 = 6;
                myLine.Y1 = 0;
                myLine.Y2 = 30;
                myLine.Name = "line" + i + 0;
                RegisterName(myLine.Name, myLine);
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
            for (int i = 2; i < gaussgrid.RowDefinitions.Count - 1; i++) //НЕ УДАЛЯЮ, НУЖНО ДОБАВИТЬ УДАЛЕНИЕ
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.X1 = 31;
                myLine.X2 = 31;
                myLine.Y1 = 30;
                myLine.Y2 = 0;
                myLine.Name = "secondline" + i + (gaussgrid.ColumnDefinitions.Count - 1);
                RegisterName(myLine.Name, myLine);
                Grid.SetColumn(myLine, gaussgrid.ColumnDefinitions.Count - 1);
                Grid.SetRow(myLine, i);
                gaussgrid.Children.Add(myLine);
            }

            //отрисовка закругления в правом верхнем углу
            PathGeometry pathGeometry2 = new PathGeometry();
            PathFigure figure2 = new PathFigure();
            figure2.StartPoint = new Point(24, 0);
            figure2.Segments.Add(new ArcSegment(new Point(31, 30), new Size(55, 45), -45, false, SweepDirection.Clockwise, true));
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
            figure3.StartPoint = new Point(31, 0);
            figure3.Segments.Add(new ArcSegment(new Point(24, 30), new Size(55, 45), -45, false, SweepDirection.Clockwise, true));
            pathGeometry3.Figures.Add(figure3);
            Path path3 = new Path();
            path3.Data = pathGeometry3;
            path3.Stroke = Brushes.Black;
            Grid.SetColumn(path3, gaussgrid.ColumnDefinitions.Count - 1);
            Grid.SetRow(path3, gaussgrid.RowDefinitions.Count - 1);
            gaussgrid.Children.Add(path3);
        }
    }
}
