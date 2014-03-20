namespace Microsoft.Practices.Prism.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class ExtendedNotificationObject : NotificationObject
    {
        #region Set Property/field of Notification object and raise PropertyChangedEvent

        /// <summary>
        /// Sets the value of teh referenced field with the new value IF the value is different from the old value.
        /// Equality is compared with the default equality comparer for this type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            return this.Set(propertyExpression, ref field, newValue, null, null);
        }

        /// <summary>
        /// Sets the value of teh referenced field with the new value IF the value is different from the old value.
        /// Equality is compared with the default equality comparer for this type.
        /// When the value is not the same, beforeChange will be called before setting the new value and afterChange will be called after wetting the value.
        /// Those two actions will not be called if the old and the new values are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="beforeChange"></param>
        /// <param name="afterChange"></param>
        /// <returns></returns>
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, Action<T> beforeChange, Action<T> afterChange)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            if (beforeChange != null) beforeChange(field);

            field = newValue;

            if (afterChange != null) afterChange(field);

            this.RaisePropertyChanged(propertyExpression);
            return true;
        }

        #endregion Set Property/field of Notification object and raise PropertyChangedEvent

        /// <summary>
        /// Retrieves the priopery name from the specified property expression.
        /// not valid for static properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        protected string GetPropertyName<T>(Expression<Func<T>> fromPropertyExpression)
        {
            return SystemLinqExpressionsExtensions.GetPropertyName(fromPropertyExpression);
        }
    }
}
