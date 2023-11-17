using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace SnakeGame
{
    public class GameOverDialog : Window
    {
        public GameOverDialog()
        {
            // 創建一個 TextBlock 來顯示消息
            var textBlock = new TextBlock
            {
                Text = "遊戲結束",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // 將 TextBlock 添加到對話框的內容中
            this.Content = textBlock;

            this.Width = 100;
            this.Height = 100;
        }
    }
}
