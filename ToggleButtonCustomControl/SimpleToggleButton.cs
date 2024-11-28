using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToggleButtonCustomControl
{
    /// <summary>
    /// Pro použití tohoto vlastního ovládacího prvku v souboru XAML postupujte podle kroků 1a nebo 1b a pak 2.
    ///
    /// Krok 1a) Použití tohoto vlastního ovládacího prvku v XAML souboru, který už je v aktuálním projektu.
    /// Přidejte tento XmlNamespace atribut do kořenového elementu označovacího souboru, kde 
    /// bude použit:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ToggleButtonCustomControl"
    ///
    ///
    /// Krok 1b) Použití tohoto vlastního ovládacího prvku v souboru XAML, který je v jiném projektu.
    /// Přidejte tento XmlNamespace atribut do kořenového elementu označovacího souboru, kde 
    /// bude použit:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ToggleButtonCustomControl;assembly=ToggleButtonCustomControl"
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
    ///     <MyNamespace:SimpleToggleButton/>
    ///
    /// </summary>
    public class SimpleToggleButton : ToggleButton
    {
        static SimpleToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleToggleButton), new FrameworkPropertyMetadata(typeof(SimpleToggleButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public SimpleToggleButton() { }

        private static CornerRadius _defaultCornerRadius = new CornerRadius(0.0);
        private static Brush _defaultOnColor = Brushes.MediumSpringGreen;
        private static Brush _defaultOffColor = Brushes.IndianRed;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SimpleToggleButton), new PropertyMetadata(_defaultCornerRadius));

        public Brush ColorOn
        {
            get { return (Brush)GetValue(ColorOnProperty); }
            set { SetValue(ColorOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorOnProperty =
            DependencyProperty.Register(nameof(ColorOn), typeof(Brush), typeof(SimpleToggleButton), new PropertyMetadata(_defaultOnColor));

        public Brush ColorOff
        {
            get { return (Brush)GetValue(ColorOffProperty); }
            set { SetValue(ColorOffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorOff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorOffProperty =
            DependencyProperty.Register(nameof(ColorOff), typeof(Brush), typeof(SimpleToggleButton), new PropertyMetadata(_defaultOffColor));

        public string LabelOn
        {
            get { return (string)GetValue(LabelOnProperty); }
            set { SetValue(LabelOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelOnProperty =
            DependencyProperty.Register(nameof(LabelOn), typeof(string), typeof(SimpleToggleButton), new PropertyMetadata("ON"));

        public string LabelOff
        {
            get { return (string)GetValue(LabelOffProperty); }
            set { SetValue(LabelOffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelOff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelOffProperty =
            DependencyProperty.Register(nameof(LabelOff), typeof(string), typeof(SimpleToggleButton), new PropertyMetadata("OFF"));

        public double SwitchWidth
        {
            get { return (double)GetValue(SwitchWidthProperty); }
            set { SetValue(SwitchWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchWidthProperty =
            DependencyProperty.Register(nameof(SwitchWidth), typeof(double), typeof(SimpleToggleButton), new PropertyMetadata(45.0));

    }
}
