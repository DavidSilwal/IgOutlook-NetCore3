﻿using IgOutlook.Core;

namespace IgOutlook.Modules.Mail.TabItems
{
    /// <summary>
    /// Interaction logic for HomeTab.xaml
    /// </summary>
    public partial class HomeTab : ISupportDataContext
    {
        public HomeTab()
        {
            InitializeComponent();
            SetResourceReference(StyleProperty, typeof(Infragistics.Windows.Ribbon.RibbonTabItem));
        }
    }
}
