﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotebookWPF.View.UserControls
{
    /// <summary>
    /// Interaction logic for NotesUserControl.xaml
    /// </summary>
    public partial class NotesUserControl : UserControl
    {
        #region Constructor

        public NotesUserControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Events

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmDeletePanel.Visibility == Visibility.Collapsed)
                ConfirmDeletePanel.Visibility = Visibility.Visible;
            else
                ConfirmDeletePanel.Visibility = Visibility.Collapsed;
        }

        private void CancelDeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeletePanel.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
