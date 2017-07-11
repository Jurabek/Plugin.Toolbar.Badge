using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using AToolbar = Android.Support.V7.Widget.Toolbar;
using Plugin.Toolbar.Badge.Abstractions;
using AColor = Android.Graphics.Color;
using Com.Mikepenz.Actionitembadge.Library.Utils;
using Com.Mikepenz.Actionitembadge.Library;
using Android.Widget;

namespace Plugin.Toolbar.Badge.Droid
{
    public class ToolbarBadgeNavigationPageRenderer : NavigationPageRenderer
    {
        private AToolbar Toolbar
        {
            get
            {
                Type type = this.GetType().BaseType;
                if (type != null)
                {
                    FieldInfo field = type.GetField("_toolbar", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (field != null)
                        return field.GetValue(this) as AToolbar;
                }
                return null;
            }
        }

        ToolbarTracker _toolbarTracker;
        private bool _disposed;

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);
            //return;
            if (e.NewElement != null)
            {
                if (_toolbarTracker == null)
                {
                    _toolbarTracker = new ToolbarTracker();
                    _toolbarTracker.CollectionChanged += ToolbarTrackerOnCollectionChanged;
                }

                var parents = new List<Page>();
                Page root = Element;
                while (!IsApplicationOrNull(root.Parent))
                {
                    root = (Page)root.Parent;
                    parents.Add(root);
                }
                _toolbarTracker.Target = e.NewElement;
                _toolbarTracker.AdditionalTargets = parents;
                UpdateMenu();
            }
        }

        void UpdateMenu()
        {
            //return;
            if (_disposed)
                return;

            var activity = (FormsAppCompatActivity)Context;
            AToolbar bar = Toolbar;
            IMenu menu = bar.Menu;

            foreach (ToolbarItem item in _toolbarTracker.ToolbarItems)
                item.PropertyChanged -= HandleToolbarItemPropertyChanged;

            menu.Clear();

            foreach (ToolbarItem item in _toolbarTracker.ToolbarItems)
            {
                IMenuItemController controller = item;
                item.PropertyChanged += HandleToolbarItemPropertyChanged;
                if (item.Order == ToolbarItemOrder.Secondary)
                {
                    IMenuItem menuItem = menu.Add(item.Text);
                    menuItem.SetEnabled(controller.IsEnabled);
                    menuItem.SetOnMenuItemClickListener(new MenuClickListener(controller.Activate));
                }
                else
                {
                    IMenuItem menuItem = menu.Add(item.Text);
                    FileImageSource icon = item.Icon;
                    if (!string.IsNullOrEmpty(icon))
                    {
                        Drawable iconDrawable = GetFormsDrawable(Context.Resources, icon);
                        if (iconDrawable != null)
                            menuItem.SetIcon(iconDrawable);

                        if (item is BadgeToolbarItem badgeItem)
                        {
                            var color = badgeItem.BadgeColor.ToAndroid();
                            var colorPressed = AColor.ParseColor("#CC0000");
                            var textColor = badgeItem.BadgeTextColor.ToAndroid();

                            var badgeStyle = new BadgeStyle(BadgeStyle.Style.Default, 
                                                                Resource.Layout.menu_action_item_badge,
                                                                color, 
                                                                colorPressed, 
                                                                textColor);
                                                        

                            ActionItemBadge.Update(activity, menuItem, iconDrawable, badgeStyle, badgeItem.BadgeText);
                            menuItem.ActionView.Click += (_, __ ) => controller.Activate();
                            menuItem.SetEnabled(controller.IsEnabled);
                            menuItem.SetShowAsAction(ShowAsAction.Always);
                        }
                    }
                   
                }
            }
        }

        internal Drawable GetFormsDrawable(Resources resource, FileImageSource fileImageSource)
        {
            var file = fileImageSource.File;
            Drawable drawable = resource.GetDrawable(fileImageSource);
            if (drawable == null)
            {
                var bitmap = resource.GetBitmap(file) ?? BitmapFactory.DecodeFile(file);
                if (bitmap != null)
                    drawable = new BitmapDrawable(resource, bitmap);
            }
            return drawable;
        }

        private void HandleToolbarItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == BadgeToolbarItem.BadgeTextProperty.PropertyName 
                || e.PropertyName ==  BadgeToolbarItem.BadgeTextColorProperty.PropertyName
                || e.PropertyName == BadgeToolbarItem.BadgeColorProperty.PropertyName)
            {
                UpdateMenu();
            }
        }

        private void ToolbarTrackerOnCollectionChanged(object sender, EventArgs e)
        {
        }

        public bool IsApplicationOrNull(Element element)
        {
            return element == null || element is Application;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }

    public class MenuClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
    {
        readonly Action _callback;

        public MenuClickListener(Action callback)
        {
            _callback = callback;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            _callback();
            return true;
        }
    }
}