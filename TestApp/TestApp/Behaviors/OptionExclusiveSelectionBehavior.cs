using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace TestApp.Behaviors
{
    public class OptionExclusiveSelectionBehavior : Behavior<View>
    {


        private TapGestureRecognizer _tapRecognizer;

        private static List<OptionExclusiveSelectionBehavior> _defaultGroup = new List<OptionExclusiveSelectionBehavior>();

        private static Dictionary<string, List<OptionExclusiveSelectionBehavior>> _radioGroups =
                            new Dictionary<string, List<OptionExclusiveSelectionBehavior>>();


        public static readonly BindableProperty IsCheckedProperty = BindableProperty.CreateAttached(
            propertyName: nameof(IsChecked),
            returnType: typeof(bool),
            declaringType: typeof(OptionExclusiveSelectionBehavior),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnIsCheckedChanged);

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }


        public static readonly BindableProperty GroupNameProperty = BindableProperty.CreateAttached(
            propertyName: nameof(GroupName),
            returnType: typeof(string),
            declaringType: typeof(OptionExclusiveSelectionBehavior),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnGroupNameChanged);

        public string GroupName
        {
            set { SetValue(GroupNameProperty, value); }
            get { return (string)GetValue(GroupNameProperty); }
        }


        public OptionExclusiveSelectionBehavior()
        {
            _defaultGroup.Add(this);
        }


        protected override void OnAttachedTo(View owner)
        {
            base.OnAttachedTo(owner);

            if (owner is Button myButton)
                myButton.Clicked += OnTapped;
            else
            {
                _tapRecognizer = new TapGestureRecognizer();
                _tapRecognizer.Tapped += OnTapped;
                owner.GestureRecognizers.Add(_tapRecognizer);
            }
        }


        protected override void OnDetachingFrom(View owner)
        {
            base.OnDetachingFrom(owner);

            TapGestureRecognizer gestureRec = owner.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;

            if (gestureRec != null)
            {
                owner.GestureRecognizers.Remove(_tapRecognizer);
                gestureRec.Tapped -= OnTapped;
            }
        }


        void OnTapped(object sender, EventArgs args)
        {
            IsChecked = true;
        }


        static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OptionExclusiveSelectionBehavior behavior = (OptionExclusiveSelectionBehavior)bindable;

            if ((bool)newValue)
            {
                string groupName = behavior.GroupName;
                List<OptionExclusiveSelectionBehavior> behaviors;

                if (string.IsNullOrEmpty(groupName))
                    behaviors = _defaultGroup;
                else
                    behaviors = _radioGroups[groupName];

                foreach (OptionExclusiveSelectionBehavior otherBehavior in behaviors)
                {
                    if (otherBehavior != behavior)
                        otherBehavior.IsChecked = false;
                }
            }
        }

        static void OnGroupNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OptionExclusiveSelectionBehavior behavior = (OptionExclusiveSelectionBehavior)bindable;
            string oldGroupName = (string)oldValue;
            string newGroupName = (string)newValue;

            if (string.IsNullOrEmpty(oldGroupName))
                // Remove the Behavior from the default group.
                _defaultGroup.Remove(behavior);

            else
            {
                // Remove the OptionExclusiveSelectionBehavior from the _radioGroups collection.
                List<OptionExclusiveSelectionBehavior> behaviors = _radioGroups[oldGroupName];
                behaviors.Remove(behavior);

                // Get rid of the collection if it's empty.
                if (behaviors.Count == 0)
                    _radioGroups.Remove(oldGroupName);
            }

            if (string.IsNullOrEmpty(newGroupName))
                // Add the new Behavior to the default group.
                _defaultGroup.Add(behavior);

            else
            {
                List<OptionExclusiveSelectionBehavior> behaviors = null;

                if (_radioGroups.ContainsKey(newGroupName))
                    // Get the named group.
                    behaviors = _radioGroups[newGroupName];

                else
                {
                    // If that group doesn't exist, CreateAttached it.
                    behaviors = new List<OptionExclusiveSelectionBehavior>();
                    _radioGroups.Add(newGroupName, behaviors);
                }

                // Add the Behavior to the group.
                behaviors.Add(behavior);
            }
        }
    }
}
