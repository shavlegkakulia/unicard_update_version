using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Fragments;
using Android.Support.V4.View;
using Java.Util;

public class ViewPagerAdapter : FragmentStatePagerAdapter
{
    private class ViewPagerItem
    {
        public Type Type { get; set; }
        public Android.Support.V4.App.Fragment CachedFragment { get; set; }
    }

    private readonly Context _context;
    private readonly ViewPager _viewPager;
    private readonly Dictionary<int, ViewPagerItem> _fragments = new Dictionary<int, ViewPagerItem>();

    public ViewPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer) { }

    public ViewPagerAdapter(Context context, ViewPager pager, Android.Support.V4.App.FragmentManager fm)
        : base(fm)
    {
        _context = context;
        _viewPager = pager;
    }

    public override Android.Support.V4.App.Fragment GetItem(int position)
    {
        if (!_fragments.ContainsKey(position)) return null;

        var bundle = new Bundle();
        bundle.PutInt("number", position);
        _fragments[position].CachedFragment = Android.Support.V4.App.Fragment.Instantiate(_context,
            FragmentJavaName(_fragments[position].Type), bundle);
        return _fragments[position].CachedFragment;
    }

    public override int Count
    {
        get { return _fragments.Count; }
    }

    public void AddFragment(Type fragType, int position = -1)
    {
        if (position < 0 && _fragments.Count == 0)
            position = 0;
        else if (position < 0 && _fragments.Count > 0)
            position = _fragments.Count;

        _fragments.Add(position, new ViewPagerItem
        {
            Type = fragType
        });

        NotifyDataSetChanged();
    }

    public void RemoveFragment(int position)
    {
        DestroyItem(null, position, _fragments[position].CachedFragment);
        _fragments.Remove(position);
        NotifyDataSetChanged();
        _viewPager.SetCurrentItem(position - 1, false);
    }

    protected virtual string FragmentJavaName(Type fragmentType)
    {
        var namespaceText = fragmentType.Namespace ?? "";
        if (namespaceText.Length > 0)
            namespaceText = namespaceText.ToLowerInvariant() + ".";
        return namespaceText + fragmentType.Name;
    }
}