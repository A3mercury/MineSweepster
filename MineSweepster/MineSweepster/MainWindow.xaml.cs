using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MineSweepster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constants
        private const int BOMB = 9;

        private int numberOfCells = 9;
        private int numberOfBombs = 10;
        
        private bool[,] isBomb;
        private int[,] Cells;

        private Random rand;
        private int randX, randY;

        int rectSize;

        public MainWindow()
        {
            InitializeComponent();

            rand = new Random();

            rectSize = (int)boardCanvas.Width / numberOfCells;

            CreateBoard();
            GetBombPlacement();
            ConsoleDrawGrid();
            DrawGrid();
        }

        private void CreateBoard()
        {
            boardCanvas.Children.Clear();

            isBomb = new bool[numberOfCells, numberOfCells];
            Cells = new int[numberOfCells, numberOfCells];

            int rectSize = (int)boardCanvas.Width / numberOfCells;

            // Turn entire grid on and create rectangles to represent it
            for (int row = 0; row < numberOfCells; row++)
            {
                for (int col = 0; col < numberOfCells; col++)
                {
                    isBomb[row, col] = false;

                    Rectangle rect = new Rectangle();
                    rect.Fill = Brushes.Violet;
                    rect.Width = rectSize + 1;
                    rect.Height = rect.Width + 1;
                    rect.Stroke = Brushes.Black;

                    int x = col * rectSize;
                    int y = row * rectSize;

                    Canvas.SetTop(rect, y);
                    Canvas.SetLeft(rect, x);

                    // Add the new rectangle to the canvas' children
                    boardCanvas.Children.Add(rect);
                }
            }
        }

        private void GetBombPlacement()
        {
            for (int i = 0; i < numberOfBombs; i++)
            {
                randX = rand.Next(numberOfCells);
                randY = rand.Next(numberOfCells);

                while (Cells[randX, randY] == BOMB)
                {
                    randX = rand.Next(numberOfCells);
                    randY = rand.Next(numberOfCells);
                }
                Cells[randX, randY] = BOMB;
            }
        }

        private void DrawGrid()
        {
            int index = 0;

            // Set colors of each rectangle based on grid values
            for (int row = 0; row < numberOfCells; row++)
            {
                for (int col = 0; col < numberOfCells; col++)
                {
                    Rectangle rect = boardCanvas.Children[index] as Rectangle;
                    index++;

                    if (isBomb[row, col])
                    {
                        if (Cells[row, col] == BOMB)
                        {
                            rect.Fill = Brushes.Red;
                            rect.Stroke = Brushes.Black;
                        }
                        else
                        {
                            rect.Fill = Brushes.White;
                            rect.Stroke = Brushes.Black;
                        }
                    }
                }
            }
        }

        private void ConsoleDrawGrid()
        {
            for (int r = 0; r < numberOfCells; r++)
            {
                for (int c = 0; c < numberOfCells; c++)
                {
                    Console.Write(Cells[r, c]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private void boardCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Find row, col of mouse press
            Point mousePosition = e.GetPosition(boardCanvas);
            int row = (int)(mousePosition.Y) / rectSize;
            int col = (int)(mousePosition.X) / rectSize;
            Console.WriteLine("row: " + row + " col: " + col);

            isBomb[row, col] = true;
            ConsoleDrawGrid();
            DrawGrid();
        }

        private void boardCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(boardCanvas);
            int row = (int)(mousePosition.Y) / rectSize;
            int col = (int)(mousePosition.X) / rectSize;
            Console.WriteLine("row: " + row + " col: " + col);

            isBomb[row, col] = true;

            Rectangle rect = sender as Rectangle;
            rect.Fill = Brushes.Green;
            rect.Stroke = Brushes.Green;

            ConsoleDrawGrid();
            DrawGrid();
        }
    }
}
