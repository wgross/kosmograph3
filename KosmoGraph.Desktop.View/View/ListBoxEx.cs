using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KosmoGraph.Desktop.View
{
    //public class PublishDesiredContentSizeControl : ContentControl
    //{
    //    protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
    //    {
    //        var desiredSize = base.MeasureOverride(constraint);
    //        this.DesiredContentSize = desiredSize;
    //        return desiredSize;
    //    }

    //    public Size DesiredContentSize
    //    {
    //        get
    //        {
    //            return (Size)(this.GetValue(DesiredContentSizeProperty));
    //        }
    //        set
    //        {
    //            this.SetValue(DesiredContentSizeProperty, value);
    //        }
    //    }

    //    public static readonly DependencyProperty DesiredContentSizeProperty = DependencyProperty.Register("DesiredContentSize", typeof(Size), typeof(PublishDesiredContentSizeControl));
    //}

    public class PublishDesiredSizeListBox : ListBox
    {
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            var desiredSize = base.MeasureOverride(constraint);
            this.DesiredContentSize = desiredSize;
            return desiredSize;
        }

        public Size DesiredContentSize
        {
            get
            {
                return (Size)(this.GetValue(DesiredContentSizeProperty));
            }
            set
            {
                this.SetValue(DesiredContentSizeProperty, value);
            }
        }

        public static readonly DependencyProperty DesiredContentSizeProperty = DependencyProperty.Register("DesiredContentSize", typeof(Size), typeof(PublishDesiredSizeListBox));
    }
}
