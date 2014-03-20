namespace System.Linq.Expressions
{
    using System.Reflection;

    public static class SystemLinqExpressionsExtensions
    {
        /// <summary>
        /// Retrieves an instance properties name from the specified property access expression.
        /// The function requires a non static property access expression, visibility may be public, protected or private.
        /// </summary>
        /// <typeparam name="T">type of the property</typeparam>
        /// <param name="propertyExpression">property access expression</param>
        /// <returns>name of the property, throws in any unsuccessful case</returns>
        public static string GetPropertyName<T>(this Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("propertyExpression must contain a member access");

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == (PropertyInfo)null)
                throw new ArgumentException("propertyExpression must contain a property access");

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("propertyExpression must contain an instance property access");

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Retrieves an instance properties value from the specified property access expression.
        /// The function requires a non static property access expression, visibility of the propery and get method 
        /// may be public, protected or private.
        /// </summary>
        /// <typeparam name="T">type of the property</typeparam>
        /// <param name="propertyExpression">property access expression</param>
        /// <returns>value of the property, throws in any unsuccessful case</returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> propertyExpression, object owner)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("propertyExpression must contain a member access");

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == (PropertyInfo)null)
                throw new ArgumentException("propertyExpression must contain a property access");

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("propertyExpression must contain an instance property access");

            return (T)propertyInfo.GetGetMethod(true).Invoke(owner, null);
        }
    }
}
