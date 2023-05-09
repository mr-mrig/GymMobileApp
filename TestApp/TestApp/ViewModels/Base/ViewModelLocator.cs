using System;
using System.Globalization;
using System.Reflection;
using TestApp.Services.DomainPresenters;
using TestApp.Services.Navigation;
using TestApp.Services.TrainingDomain;
using TinyIoC;
using Xamarin.Forms;

namespace TestApp.ViewModels.Base
{
    public static class ViewModelLocator
    {
        private static TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }
        
        //public static BaseViewModel GetViewModel(BindableObject bindable)
        //{
        //    if (bindable is Element view)
        //    {
        //        Type viewType = view.GetType();
        //        string viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
        //        string viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
        //        string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

        //        Type viewModelType = Type.GetType(viewModelName);

        //        if (viewModelType == null)
        //            return null;

        //        return _container.Resolve(viewModelType) as BaseViewModel;
        //    }
        //    return null;
        //}

        public static bool UseMockService { get; set; }

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();

            // View models - by default, TinyIoC will register concrete classes as multi-instance.
            _container.Register<TrainingPlansSummariesViewModel>();

            // Services - by default, TinyIoC will register interface registrations as singletons.
            _container.Register<INavigationService, NavigationService>();
            _container.Register<ITrainingPlanService, TrainingPlanService>();
            _container.Register<ITrainingPresenter, WorkUnitPresenter>();
        }

        public static void UpdateDependencies(bool useMockServices)
        {
            // Change injected dependencies
            if (useMockServices)
            {
                _container.Register<ITrainingPlanService, TrainingPlanMockService>();
                _container.Register<ITrainingTagService, TrainingTagMockService>();

                UseMockService = true;
            }
            else
            {
                _container.Register<ITrainingPlanService, TrainingPlanService>();
                _container.Register<ITrainingTagService, TrainingTagService>();

                UseMockService = false;
            }
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>().AsSingleton();
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Element view)
            {
                Type viewType = view.GetType();
                string viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                string viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

                Type viewModelType = Type.GetType(viewModelName);

                if (viewModelType == null)
                    return;

                var viewModel = _container.Resolve(viewModelType);
                view.BindingContext = viewModel;
            }
        }
    }
}