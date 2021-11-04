using System.Collections.Generic;
using CommonUtils.UnityComponents;

namespace CommonUtils.Input.ButtonExternalControllers.SequentialKeyboardNavigation
{
    public class FocusableButtonFromKeyboardComparer : IComparer<IFocusableButtonFromKeyboard> {
        public int Compare(IFocusableButtonFromKeyboard x, IFocusableButtonFromKeyboard y) {
            if (!x.IsValid() && !y.IsValid()) return 0;
            if (!x.IsValid()) return 1;
            if (!y.IsValid()) return -1;
            var result = x.TabIndex.CompareTo(y.TabIndex);
            
            // If the TabIndex is the same in both items, treat the first one as smaller; otherwise returning zero
            // will provoke the second item not to be included in the SortedSet.
            return result == 0 ? -1 : x.TabIndex.CompareTo(y.TabIndex);
        }
    }
}