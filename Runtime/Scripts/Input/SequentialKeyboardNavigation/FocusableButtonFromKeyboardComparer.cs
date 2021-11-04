using System.Collections.Generic;
using CommonUtils.UnityComponents;

namespace CommonUtils.Input.SequentialKeyboardNavigation
{
    public class FocusableButtonFromKeyboardComparer : IComparer<IFocusableButtonFromKeyboard> {
        public int Compare(IFocusableButtonFromKeyboard x, IFocusableButtonFromKeyboard y) {
            if (!x.IsValid() && !y.IsValid()) return 0;
            if (!x.IsValid()) return 1;
            if (!y.IsValid()) return -1;
            return x.TabIndex.CompareTo(y.TabIndex);
        }
    }
}