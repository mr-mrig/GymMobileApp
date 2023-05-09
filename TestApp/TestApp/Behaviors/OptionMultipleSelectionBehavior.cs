//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Xamarin.Forms;

//namespace TestApp.Behaviors
//{
//    public class OptionMultipleSelectionBehavior : Behavior<View>
//    {


//        private TapGestureRecognizer _tapRecognizer;

//        private static List<OptionMultipleSelectionBehavior> _defaultGroup = new List<OptionMultipleSelectionBehavior>();

//        private static Dictionary<string, List<OptionMultipleSelectionBehavior>> _radioGroups =
//                            new Dictionary<string, List<OptionMultipleSelectionBehavior>>();


//        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
//            propertyName: nameof(IsSelected),
//            returnType: typeof(bool),
//            declaringType: typeof(OptionMultipleSelectionBehavior),
//            defaultValue: false,
//            defaultBindingMode: BindingMode.TwoWay);

//        public bool IsSelected
//        {
//            get => (bool)GetValue(IsSelectedProperty);
//            set => SetValue(IsSelectedProperty, value);
//        }


//        public static readonly BindableProperty GroupNameProperty = BindableProperty.Create(
//            propertyName: nameof(GroupName),
//            returnType: typeof(string),
//            declaringType: typeof(OptionMultipleSelectionBehavior),
//            defaultValue: null,
//            defaultBindingMode: BindingMode.TwoWay,
//            propertyChanged: OnGroupNameChanged);

//        public string GroupName
//        {
//            set { SetValue(GroupNameProperty, value); }
//            get { return (string)GetValue(GroupNameProperty); }
//        }


//        public OptionMultipleSelectionBehavior()
//        {
//            _defaultGroup.Add(this);
//        }


//        protected override void OnAttachedTo(View owner)
//        {
//            base.OnAttachedTo(owner);

//            if (owner is Button myButton)
//                myButton.Clicked += OnTapRecognizerTapped;
//            else
//            {
//                _tapRecognizer = new TapGestureRecognizer();
//                _tapRecognizer.Tapped += OnTapRecognizerTapped;
//                owner.GestureRecognizers.Add(_tapRecognizer);
//            }
//        }


//        protected override void OnDetachingFrom(View owner)
//        {
//            base.OnDetachingFrom(owner);

//            TapGestureRecognizer gestureRec = owner.GestureRecognizers.FirstOrDefault() as TapGestureRecognizer;

//            if (gestureRec != null)
//            {
//                owner.GestureRecognizers.Remove(_tapRecognizer);
//                gestureRec.Tapped -= OnTapRecognizerTapped;
//            }
//        }


//        void OnTapRecognizerTapped(object sender, EventArgs args)
//        {
//            IsSelected = !IsSelected;
//        }


//        static void OnGroupNameChanged(BindableObject bindable, object oldValue, object newValue)
//        {
//            OptionMultipleSelectionBehavior behavior = (OptionMultipleSelectionBehavior)bindable;
//            string oldGroupName = (string)oldValue;
//            string newGroupName = (string)newValue;

//            if (string.IsNullOrEmpty(oldGroupName))
//                // Remove the Behavior from the default group.
//                _defaultGroup.Remove(behavior);

//            else
//            {
//                // Remove the OptionMultipleSelectionBehavior from the _radioGroups collection.
//                List<OptionMultipleSelectionBehavior> behaviors = _radioGroups[oldGroupName];
//                behaviors.Remove(behavior);

//                // Get rid of the collection if it's empty.
//                if (behaviors.Count == 0)
//                    _radioGroups.Remove(oldGroupName);
//            }

//            if (string.IsNullOrEmpty(newGroupName))
//                // Add the new Behavior to the default group.
//                _defaultGroup.Add(behavior);

//            else
//            {
//                List<OptionMultipleSelectionBehavior> behaviors;

//                if (_radioGroups.ContainsKey(newGroupName))
//                    behaviors = _radioGroups[newGroupName];

//                else
//                {
//                    behaviors = new List<OptionMultipleSelectionBehavior>();
//                    _radioGroups.Add(newGroupName, behaviors);
//                }

//                behaviors.Add(behavior);
//            }
//        }
//    }
//}
