
namespace KosmoGraph.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ExceptionAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
                throw new AssertFailedException(string.Format("Expected exception of type '{0}' but none was thrown", typeof(TException)));
            }
            catch (TException)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(string.Format("Expected exception of type '{0}' but type '{1}' was thrown", typeof(TException), ex.GetType()));
            }
        }
    }
}
