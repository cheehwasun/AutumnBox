/* =============================================================================*\
*
* Filename: CstmButton.xaml.cs
* Description: 
*
* Version: 1.0
* Created: 10/2/2017 23:08:35(UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
using AutumnBox.Helper;
using System;
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
using AutumnBox.Basic.Devices;

namespace AutumnBox.UI.Cstm
{
    /// <summary>
    /// CstmButton.xaml 的交互逻辑
    /// </summary>
    public partial class CstmButton : Button
    {
        public static readonly DependencyProperty ContorlNameProperty =
            DependencyProperty.Register("ContorlName", typeof(string), typeof(CstmButton), new PropertyMetadata());
        public CstmButton()
        {
            InitializeComponent();
        }

        public void ChangeByStatus(DeviceStatus nowStatus)
        {
            throw new NotImplementedException();
        }
    }
}