﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;



namespace AnalogClock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
        
        Ticks ticks = new Ticks();
        Time time = new Time() ;


		//bool? DateAtBottom;
		public MainWindow()
		{
            time.Ticks = ticks;
			InitializeComponent();
            
   
			this.DataContext = time;
			  
			var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
			timer.Tick += async (s, e) => {
				var now = DateTime.Now;

				var date = string.Format("{0} {1}", now.Day, now.ToString("MMM", CultureInfo.InvariantCulture));
				txtDate.Text = date;
				txtDay.Text = DateTime.Now.DayOfWeek.ToString();

                time.Second = now.Second;
                time.Minute = now.Minute;
				time.Hour = now.Hour;
				time.MilliSecond = now.Millisecond;
				if (time.Minute == 0 && time.Second == 0 && (string)ClockGrid.Tag !="Chime")
				{   
					ClockGrid.Tag = "Chime";
				}
				else if(time.Minute == 0 && time.Second>0 && (string)ClockGrid.Tag =="Chime")
				{
					ClockGrid.Tag = "";
				}
				if ((time.Minute == 29) && time.Second == 58 && (string)ClockGrid.Tag != "Ding")
				{
					ClockGrid.Tag = "Ding";
				}
				else if ((time.Minute == 29 ) && time.Second >58 && (string)ClockGrid.Tag == "Ding")
				{
					ClockGrid.Tag = "";
				}
				if ((time.Minute == 14 || time.Minute == 44) && time.Second == 58 && (string)ClockGrid.Tag != "DingSingle")
				{
					ClockGrid.Tag = "DingSingle";
				}
				else if ((time.Minute == 14 || time.Minute == 44) && time.Second > 58 && (string)ClockGrid.Tag == "DingSingle")
				{
					ClockGrid.Tag = "";
				}

				if ((time.Second == 59 && ticks.TickTypeValue == TickType.Smooth) || (time.Second == 0 && ticks.TickTypeValue == TickType.Second))
					await StartAnimationAsync(FindResource("Storyboard12") as Storyboard);
				else if ((time.Second == 14 && ticks.TickTypeValue == TickType.Smooth) || (time.Second == 15 && ticks.TickTypeValue == TickType.Second))
					await StartAnimationAsync(FindResource("Animate_3_VerticalTransform") as Storyboard);
				else if ((time.Second == 29 && ticks.TickTypeValue == TickType.Smooth) || (time.Second == 30 && ticks.TickTypeValue == TickType.Second))
					await StartAnimationAsync(FindResource("Animate_6_1_Grow") as Storyboard);
				else if ((time.Second == 44 && ticks.TickTypeValue == TickType.Smooth) || (time.Second == 45 && ticks.TickTypeValue == TickType.Second))
					await StartAnimationAsync(FindResource("Animate_9_VerticalTransform") as Storyboard);
				else if (time.Second == 5)
					await StartAnimationAsync(FindResource("Animate1_1") as Storyboard);
				else if (time.Second == 10)
					await StartAnimationAsync(FindResource("Animate2_1") as Storyboard);
				else if (time.Second ==20)
					await StartAnimationAsync(FindResource("Animate4_1") as Storyboard);
				else if (time.Second == 25)
					await StartAnimationAsync(FindResource("Animate5_1") as Storyboard);
				else if (time.Second == 35)
					await StartAnimationAsync(FindResource("Animate7_1") as Storyboard);
				else if (time.Second == 40)
					await StartAnimationAsync(FindResource("Animate8_1") as Storyboard);
				 else if (time.Second == 50)
					await StartAnimationAsync(FindResource("Animate10_1") as Storyboard);
				else if (time.Second == 55)
					await StartAnimationAsync(FindResource("Animate11_1") as Storyboard);

				var hourAngle = time.HourAngle > 360 ? time.HourAngle - ((int)time.HourAngle/ 360)*360 : time.HourAngle;
			   

				//// Position of Date
				//if(((hourAngle>270 || hourAngle<90) ) && (DateAtBottom==false || DateAtBottom==null))
				//{
				//    DateAtBottom = true;
				//    await StartAnimationAsync(FindResource("DateToBottom") as Storyboard);
				//}
				//    // Pointers at the botton half
				//else if((( hourAngle>90 && hourAngle<270 ) ) && (DateAtBottom==true || DateAtBottom==null))
				//{
				//   // MessageBox.Show("WTF");
				//    DateAtBottom = false;
				//    await StartAnimationAsync(FindResource("DateToTop") as Storyboard);
				//}

			};
			timer.Start();
		}

		bool sb12Completed = true;

		private Task StartAnimationAsync(Storyboard sb)
		{
			var tcs = new TaskCompletionSource<object>();
			
			if (sb == null) 
				tcs.SetException(new ArgumentNullException("Storyboard cannot be null"));
			else
				{
					if (sb12Completed == false) 
						tcs.SetResult(null);
					else
					{
						EventHandler handler = null;
						handler = (s, e) =>
						{
							sb.Completed -= handler;
							sb12Completed = true;
							tcs.SetResult(null);
						};
						sb.Completed += handler;
						sb12Completed = false;
						sb.Begin();
					}
				}
			return tcs.Task;
		}

   

		//private void btnChime_Click(object sender, RoutedEventArgs e)
		//{
		//    //ClockGrid.Tag = (string)ClockGrid.Tag == "Chime"? "":"Chime";
		//    ClockGrid.Tag = "DingSingle";
		//    ClockGrid.Tag = "";
		//}

		#region Window Events
		private void window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void window1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.Close();
		}

		private void window1_Loaded(object sender, RoutedEventArgs e)
		{
			this.Top = Properties.Settings.Default.settingsTop;
			this.Left = Properties.Settings.Default.settingsLeft;
            ticks.TickTypeValue = Properties.Settings.Default.settingsTickSmooth ? TickType.Smooth : TickType.Second;
		}

		private void window1_Closing(object sender, CancelEventArgs e)
		{
			Properties.Settings.Default.settingsLeft = this.Left;
			Properties.Settings.Default.settingsTop = this.Top;
            Properties.Settings.Default.settingsTickSmooth = ticks.TickTypeValue == TickType.Smooth;
			Properties.Settings.Default.Save();
		}
        #endregion

        private void ellCenter_ToolTipOpening(object sender, System.Windows.Controls.ToolTipEventArgs e)
        {
            var tt = (ToolTip)(sender as System.Windows.Shapes.Ellipse).ToolTip;
            ((tt.Content as Border).Child as TextBlock).Text = DateTime.Now.ToShortTimeString() + (DateTime.Now.Hour > 11 ?" PM":" AM");
            
        }

        private void window1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.T)
            {
                ticks.TickTypeValue = ticks.TickTypeValue == TickType.Smooth ? TickType.Second : TickType.Smooth;
            }
        }
    }

    public class Time:INotifyPropertyChanged
	{
        public Ticks Ticks { get; set; }

        public bool Chime{get;set;}
		private double hourAngle;
		public double HourAngle
		{
			get { return hourAngle; }
			set
			{
				hourAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HourAngle"));
            }
		}

		
		private int hour;
		public int Hour
		{
			get { return hour; }
			set
			{
				hour = value;
				HourAngle = (hour + ((double)Minute / 60)) * 30;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hour"));
            }
		}


		private double minuteAngle;
		public double MinuteAngle
		{
			get { return minuteAngle; }
			set
			{
				minuteAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinuteAngle"));
            }
		}

		private int m_minute;
		public int Minute
		{
			get { return m_minute; }
			set
			{
				m_minute = value;
				MinuteAngle = (value+((double)Second/60))  * 6;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Minute"));
            }
		}

		double secondAngle;
		public double SecondAngle {
			get{return secondAngle;}
			private set 
			{
				secondAngle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondAngle"));
            }
		}
 
		private int m_second;
		public int Second
		{
			get { return m_second; }
			set
			{
				m_second = value;
				SecondAngle = value * 6;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Second"));

            }
		}

		private double milliSecond;
        public double MilliSecond
        {
            get { return milliSecond; }
            set
            {
                milliSecond = value;
                if(this.Ticks.TickTypeValue == TickType.Smooth )
                    SecondAngle = (Second + (milliSecond / 1000)) * 6;
                else
                    SecondAngle = Second * 6;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MilliSecond"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
	}

}
