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
using WeatherInfoCustomControl.Support;

namespace ButtonCustomControl
{
    /// <summary>
    /// Pro použití tohoto vlastního ovládacího prvku v souboru XAML postupujte podle kroků 1a nebo 1b a pak 2.
    ///
    /// Krok 1a) Použití tohoto vlastního ovládacího prvku v XAML souboru, který už je v aktuálním projektu.
    /// Přidejte tento XmlNamespace atribut do kořenového elementu označovacího souboru, kde 
    /// bude použit:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ButtonCustomControl"
    ///
    ///
    /// Krok 1b) Použití tohoto vlastního ovládacího prvku v souboru XAML, který je v jiném projektu.
    /// Přidejte tento XmlNamespace atribut do kořenového elementu označovacího souboru, kde 
    /// bude použit:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ButtonCustomControl;assembly=ButtonCustomControl"
    ///
    /// Budete taky muset přidat odkaz na projekt z projektu, kde se nachází XAML soubor,
    /// ve kterém se soubor XAML nachází, a pro vyloučení chyb kompilace projekt znovu sestavit:
    ///
    ///     V Průzkumníku řešení klikněte pravým tlačítkem na cílový projekt a
    ///     v nabídce "Přidat odkaz"->"Projekty"->[Najděte a vyberte tento projekt]
    ///
    ///
    /// Krok 2)
    /// Pokračujte dále a použijte svůj ovládací prvek v souboru XAML.
    ///
    ///     <MyNamespace:SimpleRoundButton/>
    ///
    /// </summary>
    public class SimpleRoundButton : Button
    {
        static SimpleRoundButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleRoundButton), new FrameworkPropertyMetadata(typeof(SimpleRoundButton)));
        }

        public bool ButtonMode
        {
            get { return (bool)GetValue(ButtonModeProperty); }
            set { SetValue(ButtonModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonModeProperty =
            DependencyProperty.Register(nameof(ButtonMode), typeof(bool), typeof(SimpleRoundButton), new PropertyMetadata(false));

        public RelayCommand Add
        {
            get { return (RelayCommand)GetValue(AddProperty); }
            set { SetValue(AddProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Add.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddProperty =
            DependencyProperty.Register(nameof(Add), typeof(RelayCommand), typeof(RelayCommand));

        public RelayCommand Remove
        {
            get { return (RelayCommand)GetValue(RemoveProperty); }
            set { SetValue(RemoveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Add.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveProperty =
            DependencyProperty.Register(nameof(Remove), typeof(RelayCommand), typeof(RelayCommand));


    }
}
