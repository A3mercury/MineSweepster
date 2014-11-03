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

        private int numberOfCells = 16;
        private int numberOfBombs = 40;
        
        private bool[,] isBomb;
        private int[,] Cells;

        private Random rand;
        private int randX, randY;

        private int rectSize;

        private bool isGameInSession = false;

        private bool isFlag = false;
        private bool isUnsure = false;

        public MainWindow()
        {
            InitializeComponent();

            rand = new Random();

            mainWindow.Height = mainWindow.Width;

            boardCanvas.Width = mainWindow.Width;
            boardCanvas.Height = boardCanvas.Width;

            rectSize = (int)boardCanvas.Width / numberOfCells;

            CreateBoard();
            //GetBombPlacement();
            ConsoleDrawGrid();
            DrawGrid();
        }

        /// <summary>
        /// Generate gameboard.
        /// Called when starting new game.
        /// </summary>
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
                    //rect.Fill = new ImageBrush { 
                    //    ImageSource = new BitmapImage(new Uri("Images/0.png", UriKind.RelativeOrAbsolute))
                    //};
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

        /// <summary>
        /// Gets random bomb placements with random ints for 
        /// randX & randY
        /// </summary>
        private void GetBombPlacement(int row, int col)
        {
            for (int i = 0; i < numberOfBombs; i++)
            {
                randX = rand.Next(numberOfCells);
                randY = rand.Next(numberOfCells);

                while (Cells[randX, randY] == BOMB && (randX == row && randY == col))
                {
                    randX = rand.Next(numberOfCells);
                    randY = rand.Next(numberOfCells);
                }
                Cells[randX, randY] = BOMB;
            }

            SetAdjacentToBombValues();
        }

        /// <summary>
        /// Find what the values of each cell touching bombs are
        /// </summary>
        private void SetAdjacentToBombValues()
        {
            // For every cell
            for (int row = 0; row < numberOfCells; row++)
            {
                for (int col = 0; col < numberOfCells; col++)
                {
                    int bombCount = 0;
                    // Check surrounding cells
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int NewRow = row + i;
                            int NewCol = col + j;
                            if(NewRow >= 0 && NewCol >= 0 &&
                               NewRow < numberOfCells && NewCol < numberOfCells)
                            {
                                if(Cells[NewRow, NewCol] == BOMB)
                                    bombCount++;
                            }
                        }
                    }
                    if(Cells[row, col] != BOMB)
                        Cells[row, col] = bombCount;
                }
            }
        }

        /// <summary>
        /// Reveals all of the grid cells that are true
        /// except for the ones marked as 'flags'
        /// </summary>
        private void DrawGrid()
        {
            // Set colors of each rectangle based on grid values
            for (int row = 0; row < numberOfCells; row++)
            {
                for (int col = 0; col < numberOfCells; col++)
                {
                    int index = row * numberOfCells + col;
                    Rectangle rect = boardCanvas.Children[index] as Rectangle;

                    if (isBomb[row, col] && rect.Fill != Brushes.Blue)
                    {                        
                        rect.Fill = GetImageBrush(rect, row, col);
                    }
                }
            }
        }

        /// <summary>
        /// Image handling of cells background
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private ImageBrush GetImageBrush(Rectangle rect, int row, int col)
        {
            ImageBrush result = new ImageBrush();

            if (Cells[row, col] == 1) result.ImageSource = new BitmapImage(new Uri("Images/1.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 2) result.ImageSource = new BitmapImage(new Uri("Images/2.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 3) result.ImageSource = new BitmapImage(new Uri("Images/3.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 4) result.ImageSource = new BitmapImage(new Uri("Images/4.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 5) result.ImageSource = new BitmapImage(new Uri("Images/5.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 6) result.ImageSource = new BitmapImage(new Uri("Images/6.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 7) result.ImageSource = new BitmapImage(new Uri("Images/7.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == 8) result.ImageSource = new BitmapImage(new Uri("Images/8.png", UriKind.RelativeOrAbsolute));
            else if (Cells[row, col] == BOMB) result.ImageSource = new BitmapImage(new Uri("Images/bomb.png", UriKind.RelativeOrAbsolute));
            else result.ImageSource = new BitmapImage(new Uri("Images/0.png", UriKind.RelativeOrAbsolute));

            return result;
        }

        /// <summary>
        /// Draw grid values to Console.
        /// </summary>
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

        /// <summary>
        /// If Left clicked, reveal the cell.
        /// If bomb, end game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Find row, col of mouse press
            Point mousePosition = e.GetPosition(boardCanvas);
            int row = (int)(mousePosition.Y) / rectSize;
            int col = (int)(mousePosition.X) / rectSize;
            Console.WriteLine("row: " + row + " col: " + col);

            if(!isGameInSession)
            {
                GetBombPlacement(row, col);
                isGameInSession = true;
            }

            int index = row * numberOfCells + col;
            Rectangle rect = boardCanvas.Children[index] as Rectangle;

            if(Cells[row, col] == 0)
                FillOutZero(row, col);

            if (!isBomb[row, col])
            {
<<<<<<< HEAD
                if (Cells[row, col] == BOMB)
                {
                    rect.Fill = new ImageBrush { 
                        ImageSource = new BitmapImage(new Uri("Images/bomb.png", UriKind.RelativeOrAbsolute))
                    };
                }
                else
                {
                    rect.Fill = GetImageBrush(rect, row, col);
                }

=======
>>>>>>> a1b76fd35c034df9fac576e30e0b0cfd6430d1be
                isBomb[row, col] = true;
            }

            // find if game is over
            if(IsGameOver())
            {
                mainWindow.Close();
            }

            ConsoleDrawGrid();
            DrawGrid();
        }

<<<<<<< HEAD
=======
        /// <summary>
        /// If a bomb was left-clicked on the game is over and player loses
        /// Else if there is no false cells in isBomb[,]
        /// Else the game is not over
        /// </summary>
        /// <returns></returns>
        private bool IsGameOver()
        {
            bool result = false;
            int counter = 0;

            for (int i = 0; i < numberOfCells; i++)
            {
                for (int j = 0; j < numberOfCells; j++)
                {
                    int index = i * numberOfCells + j;
                    Rectangle rect = boardCanvas.Children[index] as Rectangle;

                    if(isBomb[i, j])
                    {
                        counter++;
                        if (counter == numberOfCells * numberOfCells)
                            result = true;
                    }

                    if((Cells[i, j] == 9 && isBomb[i, j]) && rect.Fill != Brushes.Blue)
                        result = true;
                }
            }

            return result;
        }

>>>>>>> a1b76fd35c034df9fac576e30e0b0cfd6430d1be
        /// <summary>
        /// DFS of all cells with value of 0
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void FillOutZero(int row, int col)
        {
            // If the selected spot is a zero,
            // starting from that cell do a DFS until we hit numbers

            // Mark spot we are at
            isBomb[row, col] = true;

            // for all nodes adjacent to node: [row, col]
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int NewRow = row + i;
                    int NewCol = col + j;
                    if ((NewRow >= 0 && NewCol >= 0 && NewRow < numberOfCells && NewCol < numberOfCells) && !isBomb[NewRow, NewCol])
                    {
                        if (Cells[NewRow, NewCol] == 0)
                        {
                            FillOutZero(NewRow, NewCol);
                        }
                        else
                        {
                            isBomb[NewRow, NewCol] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the cell hasn't been revealed, right click will set a flag.
        /// If flag is set and right clicked again, set to unsure.
        /// If unsure, allow left click, if right click again, resets the cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(boardCanvas);
            int row = (int)(mousePosition.Y) / rectSize;
            int col = (int)(mousePosition.X) / rectSize;
            Console.WriteLine("row: " + row + " col: " + col);

            int index = row * numberOfCells + col;
            Console.WriteLine(index);
            Rectangle rect = boardCanvas.Children[index] as Rectangle;

            // Does the unsure if bomb graphic
            if (isFlag)
            {
                rect.Fill = new ImageBrush {
                    ImageSource = new BitmapImage(new Uri("Images/unsure.png", UriKind.RelativeOrAbsolute))
                };
                isFlag = false;
                isUnsure = true;
                isBomb[row, col] = false;
            }
            else if (isUnsure)
            {
                rect.Fill = Brushes.Violet;
                isUnsure = false;
                isBomb[row, col] = false;
            }
            else
            {
                if (!isBomb[row, col])
                {
                    rect.Fill = new ImageBrush { 
                        ImageSource = new BitmapImage(new Uri("Images/flag.png", UriKind.RelativeOrAbsolute))
                    };
                    isFlag = true;
                    isBomb[row, col] = true;
                }
            }

            ConsoleDrawGrid();
        }

        private ImageBrush GetImageBrush(Rectangle rect, int row, int col)
        {
            ImageBrush result = new ImageBrush();

            if (Cells[row, col] == 1) result.ImageSource = new BitmapImage(new Uri("Images/1.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 2) result.ImageSource = new BitmapImage(new Uri("Images/2.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 3) result.ImageSource = new BitmapImage(new Uri("Images/3.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 4) result.ImageSource = new BitmapImage(new Uri("Images/4.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 5) result.ImageSource = new BitmapImage(new Uri("Images/5.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 6) result.ImageSource = new BitmapImage(new Uri("Images/6.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 7) result.ImageSource = new BitmapImage(new Uri("Images/7.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 8) result.ImageSource = new BitmapImage(new Uri("Images/8.png", UriKind.RelativeOrAbsolute));
            if (Cells[row, col] == 9) result.ImageSource = new BitmapImage(new Uri("Images/9.png", UriKind.RelativeOrAbsolute));

            return result;
        }
    }
}
