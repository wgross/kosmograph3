namespace KosmoGraph.Desktop.View
{
    using Microsoft.Practices.Prism.Commands;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shapes;

    public partial class KosmoGraphWindow
    {
        protected void KosmoGraphWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                this.Cursor = Cursors.Arrow;
        }
        
        #region Handle click events

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void restoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Handle mouse down in movw rectangle

        private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        #endregion 

        protected void resizeRectangle_MouseMove(Object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
                }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        protected void resizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    this.Cursor = Cursors.SizeNS;
                    this.ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    this.Cursor = Cursors.SizeNS;
                    this.ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    this.Cursor = Cursors.SizeWE;
                    this.ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    this.Cursor = Cursors.SizeWE;
                    this.ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    this.Cursor = Cursors.SizeNWSE;
                    this.ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    this.Cursor = Cursors.SizeNESW;
                    this.ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    this.Cursor = Cursors.SizeNESW;
                    this.ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    this.Cursor = Cursors.SizeNWSE;
                    this.ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
    }
}
