using System.Collections.Generic;
using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation
{
    public class SequentialKeyboardNavigationItemComparer : IComparer<ISequentialKeyboardNavigationItem> {
        public int Compare(ISequentialKeyboardNavigationItem x, ISequentialKeyboardNavigationItem y) {
            if (!x.IsValid() && !y.IsValid()) return 0;
            if (!x.IsValid()) return 1;
            if (!y.IsValid()) return -1;
            return x.TabIndex.CompareTo(y.TabIndex);
        }
    }
}