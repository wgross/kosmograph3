namespace KosmoGraph.Desktop.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Shapes;
    
    public partial class KosmoGraphWindow : Window
    {
        #region Construction and initialization of this instance

        static KosmoGraphWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KosmoGraphWindow), new FrameworkPropertyMetadata(typeof(KosmoGraphWindow)));
        }

        public KosmoGraphWindow()
        {
            this.PreviewMouseMove+=this.KosmoGraphWindow_PreviewMouseMove;
            this.SourceInitialized+=this.KosmoGraphWindow_SourceInitialized;
        }
 
        private HwndSource hwndSource;  

        private void KosmoGraphWindow_SourceInitialized(object sender, EventArgs e)
        {
            this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }

        public override void OnApplyTemplate()
        {
            var minimizeButton = this.GetTemplateChild("minimizeButton") as Button;
            if (minimizeButton != null)
                minimizeButton.Click += this.minimizeButton_Click;

            var restoreButton = this.GetTemplateChild("restoreButton") as Button;
            if (restoreButton != null)
                restoreButton.Click += this.restoreButton_Click;

            var closeButton = this.GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
                closeButton.Click += this.closeButton_Click;

            var moveRectangle = this.GetTemplateChild("moveWindowRectangle") as Rectangle;
            if (moveRectangle != null)
                moveRectangle.PreviewMouseDown += this.moveRectangle_PreviewMouseDown;

            var resizeGrid = this.GetTemplateChild("resizeWindowGrid") as Grid;
            if (resizeGrid != null)
            {
                foreach (UIElement resizeRectangle in resizeGrid.Children.OfType<Rectangle>())
                {
                    resizeRectangle.PreviewMouseDown += this.resizeRectangle_PreviewMouseDown;
                    resizeRectangle.MouseMove += this.resizeRectangle_MouseMove;
                }
            }
 
            base.OnApplyTemplate();
        }

        #endregion 

    }
}
