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

namespace KhachoWPFControls
{
	/// <summary>
	/// Логика взаимодействия для KhachoNumericUpDown.xaml
	/// </summary>
	public partial class NumericUpDown : UserControl
	{
		#region {DEPENDENCY_PROPERTIES}

		/// <summary>
		/// Видимость стрелок.
		/// </summary>
		public static DependencyProperty ArrowsVisibilityProperty;

		/// <summary>
		/// Текущее значение.
		/// </summary>
		public static DependencyProperty ValueProperty;

		#endregion


		#region {EVENTS}

		/// <summary>
		/// Происходит при изменении значения.
		/// </summary>
		public event RoutedEventHandler ValueChanged;

		#endregion


		#region {PROPERTIES}

		/// <summary>
		/// Задает или возвращает значени, которое указывает на видимость стрелок.
		/// </summary>
		public Visibility ArrowsVisibility
		{
			get { return (Visibility)base.GetValue(ArrowsVisibilityProperty); }
			set { base.SetValue(ArrowsVisibilityProperty, value); }
		}

		///// <summary>
		///// Текущее значение.
		///// </summary>
		//public Decimal Value
		//{
		//	get
		//	{
		//		decimal val = 0;
		//		tb_value.Dispatcher.Invoke(() => Decimal.TryParse(tb_value.Text, out val));
		//		return val;
		//	}
		//	set { tb_value.Dispatcher.Invoke(() => tb_value.Text = string.Format(stringFormat, value)); }
		//}

		/// <summary>
		/// Текущее значение.
		/// </summary>
		public Decimal Value
		{
			get { return (Decimal)base.GetValue(ValueProperty); }
			set { base.SetValue(ValueProperty, value); }
		}


		/// <summary>
		/// Указывает число отображаемых десятичных разрядов.
		/// </summary>
		public int DecimalPlaces
		{
			get { return decimalPlaces; }
			set
			{
				decimalPlaces = value;
				stringFormat = string.Concat("{0:F", decimalPlaces, "}");
				Value = Value;
			}
		}
		private int decimalPlaces;

		/// <summary>
		/// Указывает максимальное значение.
		/// </summary>
		public Decimal Maximum { get; set; }

		/// <summary>
		/// Указывает минимальное значение.
		/// </summary>
		public Decimal Minimum { get; set; }

		/// <summary>
		/// Определяет, отображается или скрыт данный элемент управления.
		/// </summary>
		public bool Visible
		{
			get { return grid_main.Visibility == System.Windows.Visibility.Visible; }
			set { grid_main.Visibility = (value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
		}

		#endregion


		#region {MEMBERS}

		/// <summary>
		/// Формат строки, представляющей текущее значение.
		/// </summary>
		private string stringFormat = "{0:F0}";

		#endregion


		#region {CONSTRUCTOR}

		/// <summary>
		/// Статический конструктор.
		/// </summary>
		static NumericUpDown()
		{
			// регистрируем свойства
			// видимость стрелок
			ArrowsVisibilityProperty = DependencyProperty.Register("ArrowsVisibility", typeof(Visibility), typeof(NumericUpDown),
				new UIPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(arrowsVisibilityChanged)));
			// текущее значение
			ValueProperty = DependencyProperty.Register("Value", typeof(Decimal), typeof(NumericUpDown),
				new UIPropertyMetadata(0m, new PropertyChangedCallback(valueChanged)));
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса MyNumericUpDown.
		/// </summary>
		public NumericUpDown()
		{
			InitializeComponent();
			Maximum = 100;
			Minimum = 0;
			Value = 0;
		}

		#endregion


		#region {PRIVATE_METHODS}

		/// <summary>
		/// Обработка события изменения значения видимости стрелок.
		/// </summary>
		/// <param name="depObj">Объект, сгенерировавший событие.</param>
		/// <param name="ea">Данные события.</param>
		private static void arrowsVisibilityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs ea)
		{
			// извлекаем текущий отображатель числа (нуд короче)
			var nud = depObj as NumericUpDown;

			// применяем значение свойства к кнопкам
			nud.b_increase.Visibility = (Visibility)ea.NewValue;
			nud.b_decrease.Visibility = (Visibility)ea.NewValue;

			// разворачиваем или сворачиваем поле со значением
			Grid.SetColumnSpan(nud.tb_value, ((Visibility)ea.NewValue == Visibility.Visible) ? 1 : 2);
		}

		/// <summary>
		/// Обработка события изменения текущего значения.
		/// </summary>
		/// <param name="depObj">Объект, сгенерировавший событие.</param>
		/// <param name="ea">Данные события.</param>
		private static void valueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs ea)
		{
			// извлекаем текущий отображатель числа
			var nud = depObj as NumericUpDown;

			// применяем изменение свойства
			nud.tb_value.Dispatcher.Invoke(() => nud.tb_value.Text = string.Format(nud.stringFormat, ea.NewValue));
		}

		#endregion


		#region {PUBLIC_METHODS}

		/// <summary>
		/// Выделяет все содержимое элемента.
		/// </summary>
		public void SelectAll()
		{
			// выделяем весь текст
			tb_value.SelectAll();
		}

		#endregion


		#region {EVENT_METHODS}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.Height = 22;
		}

		private void b_increase_Click(object sender, RoutedEventArgs e)
		{
			Value++;
		}

		private void b_decrease_Click(object sender, RoutedEventArgs e)
		{
			Value--;
		}

		private void tb_value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// выделяем весь текст
			tb_value.SelectAll();
		}

		private void tb_value_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Text))
			{
				e.Handled = true;
				return;
			}

			// определяем позицию запятой
			var separatorPosition = tb_value.Text.IndexOf(',');
			separatorPosition = (separatorPosition == -1) ? tb_value.Text.Length : separatorPosition;
			// отсекаем любые символы, кроме одной запятой...
			if (e.Text.Equals(",") && (tb_value.Text.Any(ch => ch.Equals(e.Text[0])) == false) ||
				// ...и цифр, если их количество после запятой не превышает допустимого...
				Char.IsDigit(e.Text, 0) /*&& tb_value.Text.Substring(separatorPosition).Length <= decimalPlaces*/ ||
				// ...и знака '-'
				e.Text.Equals("-"))
			{
				e.Handled = false;
			}
			else
			{
				e.Handled = true;
			}
		}

		private void tb_value_TextChanged(object sender, TextChangedEventArgs e)
		{
			// ограничиваем значение по максимуму и минимуму
			if (Value > Maximum) Value = Maximum;
			if (Value < Minimum) Value = Minimum;

			// формируем значение
			var value = 0m;
			tb_value.Dispatcher.Invoke(() => Decimal.TryParse(tb_value.Text, out value));
			Value = value;

			// провоцируем событие изменения значения
			if (ValueChanged != null) ValueChanged(this, new RoutedEventArgs());
		}

		#endregion
	}
}
