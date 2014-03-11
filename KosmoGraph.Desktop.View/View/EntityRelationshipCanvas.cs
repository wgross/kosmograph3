namespace KosmoGraph.Desktop.View
{
    using KosmoGraph.Desktop.ViewModel;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public partial class EntityRelationshipCanvas : Canvas
    {
        public EntityRelationshipCanvas()
        {
            this.AddHandler(DragEntityThumb.EntityDraggedEvent,new RoutedEventHandler(this.EntityDragged));
        }

        private EntityRelationshipViewModel Model
        {
            get
            {
                return (EntityRelationshipViewModel)this.DataContext;
            }
        }

        #region Clip empty canvas areas 

        protected override Size MeasureOverride(Size constraint)
        {
            var measuredSize = base.Children
                .OfType<UIElement>()
                .Aggregate(new Size(), (size, uie) =>
                {
                    double left = Canvas.GetLeft(uie);
                    double top = Canvas.GetTop(uie);
                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    //measure desired size for each child
                    uie.Measure(constraint);

                    Size desiredSize = uie.DesiredSize;
                    if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                    {
                        size.Width = Math.Max(size.Width, left + desiredSize.Width);
                        size.Height = Math.Max(size.Height, top + desiredSize.Height);
                    }

                    return size;
                });
            
            //for aesthetic reasons add extra points
            measuredSize.Width += 10;
            measuredSize.Height += 10;
            
            return measuredSize;
        }

        #endregion

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
        }

        private void EntityDragged(object sender, RoutedEventArgs e)
        {
            this.InvalidateMeasure();
        }
    }
}
