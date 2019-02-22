using System;

namespace QuickFind {
    public static class Extensions {
        public static bool Contains (this string str, string contains, StringComparison comparison) {
            return str?.IndexOf (contains, comparison) >= 0;
        }
    }
}