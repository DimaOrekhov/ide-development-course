using System;

namespace Expressions
{
    public static class Utils
    {
        public static void RequireNotNull(object o, string paramName = null)
        {
            if (o == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}