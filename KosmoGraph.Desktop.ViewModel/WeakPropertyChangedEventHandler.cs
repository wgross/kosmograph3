namespace KosmoGraph.Desktop.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public sealed class WeakPropertyChangedEventHandler
    {
        public WeakPropertyChangedEventHandler(PropertyChangedEventHandler strongPropertyChangedEventHandler)
        {
            this.method = strongPropertyChangedEventHandler.Method;
            this.targetReference = new WeakReference(strongPropertyChangedEventHandler.Target, true);
        }

        private readonly WeakReference targetReference;        
        private readonly MethodInfo method;

        public void Handler(object sender, PropertyChangedEventArgs e)
        {
            object target = null;
            if (this.targetReference.IsAlive)
                target = this.targetReference.Target;
            if (target == null)
                return;

            var callback = (Action<object, PropertyChangedEventArgs>)Delegate.CreateDelegate(typeof(Action<object, PropertyChangedEventArgs>), target, this.method, true);
            if (callback != null)
                callback(sender, e);
        }
    }
}
