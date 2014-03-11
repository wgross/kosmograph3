namespace KosmoGraph.Desktop.Dialog
{
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
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for DialogControl.xaml
    /// </summary>
    public partial class DialogControl : UserControl
    {
        public DialogControl()
        {
            this.InitializeComponent();
            this.CommandBindings.Add( new CommandBinding(DialogCommands.Ok, this.OkExecuted, this.OkCanExecute));
            this.CommandBindings.Add(new CommandBinding(DialogCommands.Cancel, this.CancelExecuted, this.CancelCanExecute));
        }

        public DialogViewModel ViewModel
        {
            get
            {
                return this.DataContext as DialogViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
        #region Show dialog modal

        public bool? DialogResult
        {
            get;set;
        }

        internal bool? WaitModal()
        {
            this.pendingDispatcher = new DispatcherFrame();
            
            Dispatcher.PushFrame(this.pendingDispatcher);

            return this.DialogResult;
        }

        internal void EndModal(bool? result)
        {
            this.DialogResult=result;
            this.pendingDispatcher.Continue=false;
        }

        private DispatcherFrame pendingDispatcher;

        #endregion 

        void dialogContent_Loaded(object sender, RoutedEventArgs e)
        {
            //Dispatcher.PushFrame(this.pendingDispatcher);
        }

        #region Handle Ok Command

        private void OkCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            if(args.Parameter!=null)
                args.CanExecute= (args.Parameter as DialogAction).ContentCommand.CanExecute(this.ViewModel.DialogContent);
        }

        private void OkExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            try
            {
                (args.Parameter as DialogAction).ContentCommand.Execute(this.ViewModel.DialogContent);
            }
            finally
            {
                this.EndModal(true);
            }
        }

        #endregion

        #region Handle Cancel Command

        private void CancelCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            if( args.Parameter!=null)
                args.CanExecute = (args.Parameter as DialogAction).ContentCommand.CanExecute(this.ViewModel.DialogContent);
        }

        private void CancelExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            try
            {
                (args.Parameter as DialogAction).ContentCommand.Execute(this.ViewModel.DialogContent);
            }
            finally
            {
                this.EndModal(false);
            }
        }

        #endregion

        #region Handle Escape Command

        private void EscapeExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            this.EndModal(null);
        }

        #endregion

        #region //Handle resize Grip events
        
        //void resizeGrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.resizeGrip.IsMouseCaptured)
        //    {
        //        this.resizeGrip.ReleaseMouseCapture();

        //        e.Handled = true;
        //    }
        //}

        //void resizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 1)
        //    {
        //        this.originalOrigin = this.resizeGrip.PointToScreen(Mouse.GetPosition(this.resizeGrip));
        //        this.originalWidth = this.contentPanel.ActualWidth;
        //        this.originalHeight = this.contentPanel.ActualHeight;
        //        this.resizeGrip.CaptureMouse();

        //        e.Handled = true;
        //    }
        //}

        //private Point originalOrigin;
        //private double originalWidth, originalHeight;
        
        //void resizeGrip_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (this.resizeGrip.IsMouseCaptured)
        //    {
        //        Point currentPosition = this.resizeGrip.PointToScreen(Mouse.GetPosition(this.resizeGrip));

        //        this.contentPanel.Width = Math.Max(0, this.originalWidth + (currentPosition.X - this.originalOrigin.X));
        //        this.contentPanel.Height = Math.Max(0, this.originalHeight + (currentPosition.Y - this.originalOrigin.Y));

        //        //Debug.WriteLine("_originalHeight=" + _originalHeight);
        //        //Debug.WriteLine("delta Y=" + (currentPosition.Y - _originalOrigin.Y));

        //        //Point currentResizerPosition = resizeGrip.TranslatePoint( new Point( 0, 0 ), resizeGrip );
        //        //Point absolutePosition = resizeGrip.PointToScreen( new Point( 0, 0 ) );

        //        // Get absolute location on screen of upper left corner of button

        //        //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
        //        //    (int)(System.Windows.Forms.Cursor.Position.X + (deltaWidth / 2)),
        //        //    System.Windows.Forms.Cursor.Position.Y
        //        //);
        //        //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
        //        //    (int)(System.Windows.Forms.Cursor.Position.Y - deltaWidth),
        //        //    (int)(System.Windows.Forms.Cursor.Position.Y)
        //        //);

        //        e.Handled = true;
        //    }
        //}
        #endregion
    }
}
