namespace KosmoGraph.Desktop.ViewModel
{
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class EntityConnectorViewModel : NotificationObject
    {
        public EntityConnectorViewModel(EntityViewModel connectedEntity)
        {
            this.entity = connectedEntity;
        }

        public EntityViewModel Entity
        {
            get
            {
                return this.entity;
            }
        }

        private readonly EntityViewModel entity;

        public System.Windows.Point GetConnectionPoint()
        {
            return new Point(this.Entity.Left+this.Entity.ActualWidth/2, this.Entity.Top+this.Entity.ActualHeight/2);
        }

        public void MoveConnectionPoint(double deltaHeight, double deltaWidth)
        {
            this.Entity.Left += deltaWidth;
            this.Entity.Top += deltaHeight;
        }
    }
}
