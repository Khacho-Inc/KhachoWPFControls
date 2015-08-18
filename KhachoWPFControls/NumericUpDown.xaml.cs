using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

		/// <summary>
		/// Указывает число отображаемых десятичных разрядов.
		/// </summary>
		public static DependencyProperty DecimalPlacesProperty;

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

		/// <summary>
		/// Текущее значение.
		/// </summary>
		public decimal Value
		{
			get { return (decimal)base.GetValue(ValueProperty); }
			set { base.SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Указывает число отображаемых десятичных разрядов.
		/// </summary>
		public int DecimalPlaces
		{
			get { return (int)base.GetValue(DecimalPlacesProperty); }
			set { base.SetValue(DecimalPlacesProperty, value); }
		}

		/// <summary>
		/// Указывает максимальное значение.
		/// </summary>
		public decimal Maximum { get; set; }

		/// <summary>
		/// Указывает минимальное значение.
		/// </summary>
		public decimal Minimum { get; set; }

		/// <summary>
		/// Определяет, отображается или скрыт данный элемент управления.
		/// </summary>
		public bool Visible
		{
			get { return grid_main.Visibility == System.Windows.Visibility.Visible; }
			set { grid_main.Visibility = (value) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
		}

		/// <summary>
		/// Строковое представление значения.
		/// </summary>
		public string ValueString
		{
			get { return string.Format(stringFormat, Value); }
			set
			{
				// проводим валидацию
				Value = validationValue(value);
			}
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
			ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(NumericUpDown),
				new UIPropertyMetadata(0m, new PropertyChangedCallback(valueChanged)));
			// количество знаков после запятой
			DecimalPlacesProperty = DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumericUpDown),
				new UIPropertyMetadata(0, new PropertyChangedCallback(decimalPlacesChanged)));
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса MyNumericUpDown.
		/// </summary>
		public NumericUpDown()
		{
			// инициализируем визуальные компоненты
			InitializeComponent();
			// задаем значения по-умолчанию
			Maximum = 100;
			Minimum = 0;
			Value = 0;
			// задаем контекст данных редактору текста
			tb_value.DataContext = this;
		}

		#endregion


		#region {DEPENDENCY_PROPERTIES_METHODS}

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
			
			// провоцируем событие изменения значения
			if (nud.ValueChanged != null) nud.ValueChanged(nud, new RoutedEventArgs());
		}

		/// <summary>
		/// Обработка события изменения значения количества знаков после запятой.
		/// </summary>
		/// <param name="depObj">Объект, сгенерировавший событие.</param>
		/// <param name="ea">Данные события.</param>
		private static void decimalPlacesChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs ea)
		{
			// извлекаем текущий отображатель числа
			var nud = depObj as NumericUpDown;

			// формируем новую строку формата данных
			nud.stringFormat = string.Concat("{0:F", (int)ea.NewValue, "}");
			// применяем изменение свойства
			nud.tb_value.Dispatcher.Invoke(() => nud.tb_value.Text = string.Format(nud.stringFormat, nud.Value));
		}

		#endregion


		#region {PRIVATE_METHODS}

		/// <summary>
		/// Проводит валидацию переданного значения <paramref name="value"/> и в случае успешной валидации возвращает его числовое представление. В противном случае возвращает текущее значение элемента управления.
		/// </summary>
		/// <param name="value">Проверяемое строковое значение.</param>
		/// <returns>Числовое представление строкового значения в случаем успешной валидации. В противном случае текущее значение элемента управления.</returns>
		private decimal validationValue(string value)
		{
			// готовим значение
			decimal newValue;

			// пытаемся разобрать значение
			if (decimal.TryParse(value, out newValue) == false)
			{
				// в случае ошибки вернем текущее значение
				newValue = Value;
			}
			else
			{
				// в случае успеха проверяем на выход за допустимый диапазон
				if (newValue < Minimum) newValue = Minimum;
				if (newValue > Maximum) newValue = Maximum;
			}

			// возвращаем получившееся значение
			return newValue;
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

		private void tb_value_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			// выделяем весь текст
			tb_value.SelectAll();
		}

		private void tb_value_GotMouseCapture(object sender, MouseEventArgs e)
		{
			// выделяем весь текст
			tb_value.SelectAll();
			e.Handled = false;
		}

		private void tb_value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			// выделяем весь текст
			tb_value.SelectAll();
		}

		private void tb_value_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// опрелеям, нажат ли Enter
			if (e.Key == Key.Enter)
			{
				// перемещаем фокус на следующий элемент
				tb_value.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				// останавливаем трассировку события, что бы следующий элемент не обрабатывал его
				e.Handled = true;
			}
		}

		private void tb_value_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// если строка пустая, прерываем обработку
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
				// ...и цифр /*, если их количество после запятой не превышает допустимого... */
				char.IsDigit(e.Text, 0) /*&& tb_value.Text.Substring(separatorPosition).Length <= decimalPlaces*/ ||
				// ...и одного знака '-'
				e.Text.Equals("-") && (tb_value.Text.Any(ch => ch.Equals(e.Text[0])) == false))
			{
				e.Handled = false;
			}
			else
			{
				e.Handled = true;
			}
		}

		#endregion
	}
}
