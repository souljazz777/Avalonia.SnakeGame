using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System.Collections.Generic;
using Rectangle = System.Drawing.Rectangle;
using Avalonia.Input;
using HarfBuzzSharp;
using System.Numerics;
using Avalonia.Threading;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int CellSize = 20; // 單位格子大小
        private Ellipse food; // 食物 使用橢圓形來表示
        private readonly Random random = new Random(); // 隨機數生成器
        private List<Border> snakeBody = new List<Border>(); // 儲存蛇身體各部分的清單
        private Vector2 direction = new Vector2(1, 0); // 蛇的初始移動方向（向右）
        private DispatcherTimer timer; // 定期更新蛇的位置的計時器

        // MainWindow的建構函式
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => InitializeGame(); // 視窗載入時的事件處理程序
            this.KeyDown += OnKeyDown; // 按鍵事件處理程序

            // 設置並啟動更新蛇位置的計時器
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (s, e) => UpdateSnake();
            timer.Start();
        }

        // 初始化遊戲的方法
        public void InitializeGame()
        {
            food = CreateEllipse(Brushes.Red); // 創建一個紅色橢圓形代表食物
            snakegame.Children.Add(food); // 將食物添加到遊戲畫布

            // 初始化蛇身體，包含5個部分
            for (int i = 0; i < 5; i++)
            {
                var part = CreateRectangle(Brushes.Green); // 創建一個綠色矩形代表蛇的身體部分
                snakeBody.Add(part); // 將身體部分添加到蛇身體清單
                snakegame.Children.Add(part); // 將身體部分添加到遊戲畫布

                // 將蛇身體部分初始化在畫布的中心位置
                double centerX = (snakegame.Bounds.Width / 2) - (CellSize * (snakeBody.Count / 2)) + (CellSize * i);
                double centerY = snakegame.Bounds.Height / 2;
                Canvas.SetLeft(part, centerX);
                Canvas.SetTop(part, centerY);
            }

            GenerateFoodLocation(); // 生成食物的初始位置
        }

        // 創建具有指定填充刷的矩形的方法
        private Border CreateRectangle(IBrush fillBrush)
        {
            return new Border
            {
                Width = CellSize,
                Height = CellSize,
                Background = fillBrush
            };
        }

        // 創建具有指定填充刷的橢圓形的方法
        private Ellipse CreateEllipse(IBrush fillBrush)
        {
            return new Ellipse
            {
                Width = CellSize,
                Height = CellSize,
                Fill = fillBrush
            };
        }

        // 生成食物在遊戲網格內的隨機位置的方法
        private void GenerateFoodLocation()
        {
            int maxX = (int)(snakegame.Bounds.Width / CellSize);
            int maxY = (int)(snakegame.Bounds.Height / CellSize);

            double foodX = random.Next(0, maxX) * CellSize;
            double foodY = random.Next(0, maxY) * CellSize;

            Canvas.SetLeft(food, foodX);
            Canvas.SetTop(food, foodY);
        }

        // 鍵盤按鍵事件處理程序，用於改變蛇的移動方向
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    direction = new Vector2(0, -1);
                    break;
                case Key.Down:
                    direction = new Vector2(0, 1);
                    break;
                case Key.Left:
                    direction = new Vector2(-1, 0);
                    break;
                case Key.Right:
                    direction = new Vector2(1, 0);
                    break;
            }
        }

        // 更新蛇位置的方法
        private void UpdateSnake()
        {
            // 逆序更新每個身體部分的位置，從尾部開始
            for (int i = snakeBody.Count - 1; i > 0; i--)
            {
                var prevPart = snakeBody[i - 1];
                var part = snakeBody[i];
                Canvas.SetLeft(part, Canvas.GetLeft(prevPart));
                Canvas.SetTop(part, Canvas.GetTop(prevPart));
            }

            var head = snakeBody[0]; // 獲取蛇頭
            double nextX = Canvas.GetLeft(head) + direction.X * CellSize; // 計算下一個X位置
            double nextY = Canvas.GetTop(head) + direction.Y * CellSize; // 計算下一個Y位置

            // 檢查蛇是否碰到遊戲畫布的邊界
            if (nextX < 0 || nextY < 0 || nextX >= snakegame.Bounds.Width || nextY >= snakegame.Bounds.Height)
            {
                timer.Stop(); // 停止計時器
                var dialog = new GameOverDialog(); // 創建遊戲結束的對話框
                dialog.ShowDialog(this); // 顯示遊戲結束的對話框
                return;
            }

            // 檢查蛇頭是否與食物重疊
            if (Math.Abs(nextX - Canvas.GetLeft(food)) < CellSize && Math.Abs(nextY - Canvas.GetTop(food)) < CellSize)
            {
                // 如果頭部與食物重疊，則在尾部添加一個新的身體部分
                var part = CreateRectangle(Brushes.Green);
                snakeBody.Add(part);
                snakegame.Children.Add(part);
                Canvas.SetLeft(part, Canvas.GetLeft(snakeBody[snakeBody.Count - 2]));
                Canvas.SetTop(part, Canvas.GetTop(snakeBody[snakeBody.Count - 2]));

                // 減少計時器的間隔以增加蛇的速度
                timer.Interval -= TimeSpan.FromMilliseconds(10);
                GenerateFoodLocation(); // 生成新的食物位置
            }

            Canvas.SetLeft(head, nextX); // 更新蛇頭的X位置
            Canvas.SetTop(head, nextY); // 更新蛇頭的Y位置
        }
    }
}
