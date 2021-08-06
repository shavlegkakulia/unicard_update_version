// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MvvmCross.ViewModels;
using MvvmCross.Views;

namespace Kunicardus.Droid
{
    public class ViewsContainer : MvxViewsContainer
    {
        private readonly Dictionary<Type, Type> _bindingMap = new Dictionary<Type, Type>();
        private readonly List<IMvxViewFinder> _secondaryViewFinders;
        private IMvxViewFinder _lastResortViewFinder;

        public ViewsContainer()
        {
            _secondaryViewFinders = new List<IMvxViewFinder>();
        }

        public void AddAll(IDictionary<Type, Type> lookup)
        {
            foreach (var pair in lookup)
            {
                Add(pair.Key, pair.Value);
            }
            base.AddAll(lookup);
        }

        public void Add(Type viewModelType, Type viewType)
        {
            base.Add(viewModelType, viewType);
            _bindingMap[viewModelType] = viewType;
        }

        public void Add<TViewModel, TView>()
            where TViewModel : IMvxViewModel
            where TView : IMvxView
        {
            Add(typeof(TViewModel), typeof(TView));
        }

        public Type GetViewType(Type viewModelType)
        {
            base.GetViewType(viewModelType);

            Type binding;
            if (_bindingMap.TryGetValue(viewModelType, out binding))
            {
                return binding;
            }

            foreach (var viewFinder in _secondaryViewFinders)
            {
                binding = viewFinder.GetViewType(viewModelType);
                if (binding != null)
                {
                    return binding;
                }
            }

            if (_lastResortViewFinder != null)
            {
                binding = _lastResortViewFinder.GetViewType(viewModelType);
                if (binding != null)
                {
                    return binding;
                }
            }

            throw new KeyNotFoundException("Could not find view for " + viewModelType);
        }

        public void AddSecondary(IMvxViewFinder finder)
        {
            _secondaryViewFinders.Add(finder);
            base.AddSecondary(finder);
        }

        public void SetLastResort(IMvxViewFinder finder)
        {
            _lastResortViewFinder = finder;
            base.SetLastResort(finder);
        }
    }
}
