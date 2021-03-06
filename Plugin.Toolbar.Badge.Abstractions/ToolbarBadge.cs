﻿using System;
using Xamarin.Forms;

namespace Plugin.Toolbar.Badge.Abstractions
{
	public class BadgeToolbarItem : ToolbarItem
	{
		public static readonly BindableProperty BadgeTextProperty =
			BindableProperty.Create("BadgeText", typeof(string), typeof(BadgeToolbarItem), default(string));

		public static readonly BindableProperty BadgeColorProperty =
			BindableProperty.Create("BadgeColor", typeof(Color), typeof(BadgeToolbarItem), Color.Red);

		public static readonly BindableProperty BadgeTextColorProperty =
			BindableProperty.Create("BadgeTextColor", typeof(Color), typeof(BadgeToolbarItem), Color.White);
        
		public string BadgeText
		{
			get => (string)GetValue(BadgeTextProperty);
			set => SetValue(BadgeTextProperty, value);
		}

		public Color BadgeColor
		{
			get => (Color)GetValue(BadgeColorProperty);
			set => SetValue(BadgeColorProperty, value);
		}

		public Color BadgeTextColor
		{
			get => (Color)GetValue(BadgeTextColorProperty);
			set => SetValue(BadgeTextColorProperty, value);
		}

		public BadgeToolbarItem(string name, string icon, Action activated, ToolbarItemOrder order = ToolbarItemOrder.Default, int priority = 0)
			:base(name, icon, activated, order, priority)
		{
		}

		public BadgeToolbarItem()
		{	
		}
        
	}

}
