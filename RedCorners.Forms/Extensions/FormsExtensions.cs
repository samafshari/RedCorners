using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RedCorners.Forms
{
    public static class FormsExtensions
    {
        public static ContentPage GetPage(this Element view)
        {
            var el = view;
            while (true)
            {
                if (el == null || el.Parent == null) return null;
                if (el.Parent is ContentPage page) return page;
                el = el.Parent;
            }
        }
    }
}
