﻿#pragma checksum "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D8DAD05E8753ABA9D2AE6A38070BB6AECD81FA17"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UHub.Database.UI.Designer.DBNavigator.Views;
using UHub.Database.UI.Designer.Main.Models;


namespace UHub.Database.UI.Designer.DBNavigator.Views {
    
    
    /// <summary>
    /// DBNavigatorPage
    /// </summary>
    public partial class DBNavigatorPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_GenViews;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_GenRevisionViews;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UHub.Database.UI.Designer;component/dbnavigator/views/dbnavigatorpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btn_GenViews = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml"
            this.btn_GenViews.Click += new System.Windows.RoutedEventHandler(this.btn_GenViews_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btn_GenRevisionViews = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\..\DBNavigator\Views\DBNavigatorPage.xaml"
            this.btn_GenRevisionViews.Click += new System.Windows.RoutedEventHandler(this.btn_GenRevisionViews_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
