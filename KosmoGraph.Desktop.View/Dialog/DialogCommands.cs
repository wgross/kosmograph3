﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KosmoGraph.Desktop.Dialog
{
    public static class DialogCommands
    {
        public static RoutedCommand Ok = new RoutedCommand();
        public static RoutedCommand Cancel = new RoutedCommand();
        public static RoutedCommand Escape = new RoutedCommand();
    }
}