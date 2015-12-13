using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private int width;
        private int height;
        private const int dd = 20;
        private int cols;
        private int rows;
        private Color[,] field;
        private Color FIGURE_CELL = Colors.Red;
        private Color EMPTY_CELL = Colors.LightBlue;
        private Color BUSY_CELL = Colors.Green;
        private Color PREVIEW = Colors.Black;
        private Boolean NeedNewFigure = false;
        private Thread t;
        private Boolean stopped = false;
        private Boolean Paused = false;
        private int INITIAL_DELAY = 500;
        private int Delay = 500;

        private int FigureType = 0;
        private int NextFigureType = 0;

        private int LinesCollapsed = 0;

        Random rand = new Random();
        object lockObj = new object();

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);

            width = (int)Math.Floor(mainCanvas.Width);
            height = (int)Math.Floor(mainCanvas.Height);
            cols = width / dd;
            rows = height / dd;

            mainCanvas.Width = dd * cols;
            mainCanvas.Height = dd * rows;

            field = new Color[rows, cols];

            InitGame();

            SynchronizationContext uiContext = SynchronizationContext.Current;
            t = new Thread(Run);
            t.Start(uiContext);
        }

        private void InitGame()
        {
            Paused = true;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    field[i, j] = EMPTY_CELL;

            PutNewFigure();

            menuStart.IsEnabled = false;
            menuStop.IsEnabled = true;
            NeedNewFigure = false;
            Delay = INITIAL_DELAY;
            LinesCollapsed = 0;
            UpdateStat();

            Paused = false;
    }

        //========================================== MAIN MENU ================================================//

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Paused = false;
            menuStart.IsEnabled = false;
            menuStop.IsEnabled = true;
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            Paused = true;
            menuStart.IsEnabled = true;
            menuStop.IsEnabled = false;
        }

        private void OnRestartClick(object sender, RoutedEventArgs e)
        {
            Paused = true;
            InitGame();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            stopped = true;
            Application.Current.Shutdown();
        }

        private void OnWindowClose(object sender, EventArgs e)
        {
            stopped = true;
            Application.Current.Shutdown();
        }

        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("C# Tetris\nCopyright (c) r47717", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //========================================== PAINTING ================================================//

        private void PaintField()
        {
            CleanField();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    PaintCell(i, j);
        }

        private void CleanField()
        {
            mainCanvas.Children.Clear();
        }

        private int XCoord(int i)
        {
            return i * dd;
        }

        private int YCoord(int j)
        {
            return j * dd;
        }

        private void PaintCell(int x, int y)
        {
            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(field[x, y]);
            rect.Fill = new SolidColorBrush(field[x, y]);
            rect.Width = dd;
            rect.Height = dd;
            Canvas.SetLeft(rect, YCoord(y));
            Canvas.SetTop(rect, XCoord(x));
            mainCanvas.Children.Add(rect);
        }

        private void UpdateStat()
        {
            statLabel.Content = "Lines: " + LinesCollapsed.ToString();
        }

        private void CleanPreview()
        {
            previewCanvas.Children.Clear();
        }

        private void ShowPreview()
        {
            int width = (int) previewCanvas.Width;
            int height = (int) previewCanvas.Height;
            int dd;
            int margin = 10;


            CleanPreview();

            if (NextFigureType == 2) // 4x4
            {
                dd = (width - 2 * margin) / 4;
                int x = margin;
                int y = margin + dd + dd / 2;
                previewCanvas.Children.Add(PreviewRect(x, y, dd));
                previewCanvas.Children.Add(PreviewRect(x + dd, y, dd));
                previewCanvas.Children.Add(PreviewRect(x + dd * 2, y, dd));
                previewCanvas.Children.Add(PreviewRect(x + dd * 3, y, dd));
            }
            else // 3x3
            {
                margin = 20;
                dd = (width - 2 * margin) / 3;
                int x = margin;
                int y = margin + dd / 2;
                switch(NextFigureType)
                {
                    case 1:
                        previewCanvas.Children.Add(PreviewRect(x + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y + dd, dd));
                        break;
                    case 3:
                        previewCanvas.Children.Add(PreviewRect(x, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y + dd, dd));
                        break;
                    case 4:
                        previewCanvas.Children.Add(PreviewRect(x + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));
                        break;
                    case 5:
                        x += dd / 2;
                        previewCanvas.Children.Add(PreviewRect(x, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));

                        break;
                    case 6:
                        previewCanvas.Children.Add(PreviewRect(x, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y + dd, dd));
                        break;
                    case 7:
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y, dd));
                        previewCanvas.Children.Add(PreviewRect(x, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd, y + dd, dd));
                        previewCanvas.Children.Add(PreviewRect(x + dd + dd, y + dd, dd));
                        break;
                }
            }
        }

        private Rectangle PreviewRect(int x, int y, int width)
        {
            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(PREVIEW);
            rect.Fill = new SolidColorBrush(PREVIEW);
            rect.Width = width;
            rect.Height = width;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }

        //========================================== TIME CYCLE ================================================//

        private void Run(object ob)
        {
            SynchronizationContext ctx = ob as SynchronizationContext;

            while (!stopped)
            {
                if(!Paused)
                {
                    Thread.Sleep(Delay);
                    if(!Paused) {
                        ctx.Post(UpdateUI, null);
                    }
                }
            }

        }

        private void UpdateUI(object param)
        {
            lock(lockObj)
            {
                Move();
                PaintField();
            }
        }

        private void Move()
        {
            if (NeedNewFigure)
            {
                PutNewFigure();
                NeedNewFigure = false;
                if (!CanMoveDown())
                {
                    SettleFigure();
                    stopped = true;
                }
                return;
            }

            MoveDown();
        }

        private void MoveDown()
        {
            for (int j = 0; j < cols; j++)
            {
                int len = 0;
                for (int i = 0; i < rows; i++)
                {
                    if (field[i, j] == FIGURE_CELL)
                    {
                        if (len == 0)
                        {
                            field[i, j] = EMPTY_CELL;
                            len++;
                        }
                    }
                    else if (field[i, j] == EMPTY_CELL && len > 0)
                    {
                        field[i, j] = FIGURE_CELL;
                        len--;
                    }
                }
            }

            if (!CanMoveDown())
            {
                SettleFigure();
                NeedNewFigure = true;
                return;
            }
        }


        private Boolean CanMoveDown()
        {
            Boolean cannot = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (field[i, j] == FIGURE_CELL && (i == rows - 1 || field[i + 1, j] == BUSY_CELL))
                        cannot = true;
            return !cannot;
        }

        private void SettleFigure()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    if (field[i, j] == FIGURE_CELL)
                        field[i, j] = BUSY_CELL;
            }

            CollapseFullLines();
        }



        private void PutNewFigure()
        {
            int center = cols / 2;

            FigureType = (NextFigureType == 0) ? rand.Next(1, 8) : NextFigureType;
            NextFigureType = rand.Next(1, 8);

            switch (FigureType)
            {
                case 1:
                    field[0, center] = FIGURE_CELL;
                    field[1, center - 1] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center + 1] = FIGURE_CELL;
                    break;
                case 2:
                    field[0, center - 2] = FIGURE_CELL;
                    field[0, center - 1] = FIGURE_CELL;
                    field[0, center] = FIGURE_CELL;
                    field[0, center + 1] = FIGURE_CELL;
                    break;
                case 3:
                    field[0, center - 1] = FIGURE_CELL;
                    field[0, center] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center + 1] = FIGURE_CELL;
                    break;
                case 4:
                    field[0, center] = FIGURE_CELL;
                    field[0, center + 1] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center - 1] = FIGURE_CELL;
                    break;
                case 5:
                    field[0, center] = FIGURE_CELL;
                    field[0, center + 1] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center + 1] = FIGURE_CELL;
                    break;
                case 6:
                    field[0, center - 1] = FIGURE_CELL;
                    field[1, center - 1] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center + 1] = FIGURE_CELL;
                    break;
                case 7:
                    field[0, center + 1] = FIGURE_CELL;
                    field[1, center - 1] = FIGURE_CELL;
                    field[1, center] = FIGURE_CELL;
                    field[1, center + 1] = FIGURE_CELL;
                    break;
            }

            PaintField();
            ShowPreview();
        }


        //========================================== COLLAPSE LINES ================================================//

        private void CollapseFullLines()
        {
            for(int r = 0; r < rows; r++)
            {
                int c;
                for(c = 0; c < cols; c++)
                {
                    if (field[r, c] != BUSY_CELL)
                        break;
                }
                if(c == cols)
                {
                    RemoveLine(r);
                    LinesCollapsed++;
                    UpdateStat();
                }
                    
            }
        }

        private void RemoveLine(int r)
        {
            for(int c = 0; c < cols; c++)
            {
                for (int i = r; i > 0; i--)
                    field[i, c] = field[i - 1, c];
            }
            for (int c = 0; c < cols; c++)
                field[0, c] = EMPTY_CELL;
        }


        //========================================== MOVING FIGURE BY KBD ================================================//

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (Paused)
                return;

            lock (lockObj)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        if (CanMoveLeft())
                            MoveFigureLeft();
                        break;
                    case Key.Right:
                        if (CanMoveRight())
                            MoveFigureRight();
                        break;
                    case Key.Up:
                        RotateFigure();
                        break;
                    case Key.Down:
                        MoveDown();
                        break;
                    case Key.Space:
                        FastDown();
                        break;
                }
                PaintField();
            }
        }

        private Boolean CanMoveLeft()
        {
            Boolean cannot = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (field[i, j] == FIGURE_CELL && (j == 0 || field[i, j - 1] == BUSY_CELL))
                        cannot = true;
            return !cannot;
        }

        private void MoveFigureLeft()
        {
            for (int i = 0; i < rows; i++)
            {
                int len = 0;
                for (int j = cols; --j >= 0;)
                {
                    if (field[i, j] == FIGURE_CELL)
                    {
                        if (len == 0)
                        {
                            field[i, j] = EMPTY_CELL;
                            len++;
                        }
                    }
                    else if (field[i, j] == EMPTY_CELL && len > 0)
                    {
                        field[i, j] = FIGURE_CELL;
                        len--;
                    }
                }
            }
        }

        private Boolean CanMoveRight()
        {
            Boolean cannot = false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (field[i, j] == FIGURE_CELL && (j == cols - 1 || field[i, j + 1] == BUSY_CELL))
                        cannot = true;
            return !cannot;
        }

        private void MoveFigureRight()
        {
            for (int i = 0; i < rows; i++)
            {
                int len = 0;
                for (int j = 0; j < cols; j++)
                {
                    if (field[i, j] == FIGURE_CELL)
                    {
                        if (len == 0)
                        {
                            field[i, j] = EMPTY_CELL;
                            len++;
                        }
                    }
                    else if (field[i, j] == EMPTY_CELL && len > 0)
                    {
                        field[i, j] = FIGURE_CELL;
                        len--;
                    }
                }
            }
        }

        private void RotateFigure()
        {
            int max = 0;
            int row = 0;
            int col = 0;

            if (FigureType == 2)
            {
                for (int r = 0; r < rows - 3; r++)
                {
                    for (int c = 0; c < cols - 3; c++)
                    {
                        int tmp = Count4x4(r, c);
                        if (tmp > max)
                        {
                            row = r;
                            col = c;
                            max = tmp;
                        }
                    }
                }
                Rotate4x4(row, col);
            } else
            {
                for (int r = 0; r < rows - 2; r++)
                {
                    for (int c = 0; c < cols - 2; c++)
                    {
                        int tmp = Count3x3(r, c);
                        if (tmp > max)
                        {
                            row = r;
                            col = c;
                            max = tmp;
                        }
                    }
                }
                Rotate3x3(row, col);
            }
        }

        private int Count3x3(int r, int c)
        {
            int count = 0;
            if (field[r + 1, c + 1] == EMPTY_CELL)
                return 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    if (field[r + i, c + j] == FIGURE_CELL)
                        count++;
            }
                
            return count;
        }

        private void Rotate3x3(int r, int c)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    if (field[i, j] == BUSY_CELL)
                        return;
            }

            Color tmp;

            tmp = field[r, c];
            field[r, c] = field[r, c + 2];
            field[r, c + 2] = field[r + 2, c + 2];
            field[r + 2, c + 2] = field[r + 2, c];
            field[r + 2, c] = tmp;

            tmp = field[r, c + 1];
            field[r, c + 1] = field[r + 1, c + 2];
            field[r + 1, c + 2] = field[r + 2, c + 1];
            field[r + 2, c + 1] = field[r + 1, c];
            field[r + 1, c] = tmp;
        }

        private int Count4x4(int r, int c)
        {
            int count = 0;
            if (field[r + 1, c + 1] != FIGURE_CELL && field[r + 1, c + 2] != FIGURE_CELL &&
                field[r + 2, c + 1] != FIGURE_CELL && field[r + 2, c + 2] != FIGURE_CELL)
            {
                return 0;
            }
                
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    if (field[r + i, c + j] == FIGURE_CELL)
                        count++;
            }

            return count;
        }

        private void Rotate4x4(int r, int c)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    if (field[i, j] == BUSY_CELL)
                        return;
            }

            Color tmp;

            tmp = field[r, c];
            field[r, c] = field[r, c + 3];
            field[r, c + 3] = field[r + 3, c + 3];
            field[r + 3, c + 3] = field[r + 3, c];
            field[r + 3, c] = tmp;

            tmp = field[r, c + 1];
            field[r, c + 1] = field[r + 1, c + 3];
            field[r + 1, c + 3] = field[r + 3, c + 2];
            field[r + 3, c + 2] = field[r + 2, c];
            field[r + 2, c] = tmp;

            tmp = field[r, c + 2];
            field[r, c + 2] = field[r + 2, c + 3];
            field[r + 2, c + 3] = field[r + 3, c + 1];
            field[r + 3, c + 1] = field[r + 1, c];
            field[r + 1, c] = tmp;

            tmp = field[r + 1, c + 1];
            field[r + 1, c + 1] = field[r + 1, c + 2];
            field[r + 1, c + 2] = field[r + 2, c + 2];
            field[r + 2, c + 2] = field[r + 2, c + 1];
            field[r + 2, c + 1] = tmp;
        }

        private void FastDown()
        {
            while(!NeedNewFigure)
            {
                if(CanMoveDown())
                {
                    MoveDown();
                    PaintField();
                }
            }
        }

    }
}
