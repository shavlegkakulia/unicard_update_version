using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.ViewModels;
using MvvmCross;

namespace Kuni.Core.ViewModels
{
	public class ImageSliderViewModel:BaseViewModel
	{
		
		public void Init ()
		{
			_imageUrls = new List<string> () { 
				"http://api.unicard.ge/upload/7475dc89587b120a9c3b853f9d20c7e4.png/220x90.jpg",
				"http://www.vdcapital.ge/uploads/news/th_316x243_Untitled-2.jpg",
				"http://jeweleryretailers.com/wp-content/uploads/2012/01/ring_jewellery.jpg",
				"http://www.teddybeartimes.com/img/p2imag2.jpg"
			};
			_currentImageUrl = _imageUrls [0];
		}

		private List<string> _imageUrls;

		public List<string> ImageUrls {
			get{ return _imageUrls; }
			set { _imageUrls = value; }
		}

		private int _index;

		public int Index {
			get { return _index; }
			set { _index = value; }
		}

		private string _currentImageUrl;

		public string CurrentImageUrl {
			get{ return _currentImageUrl; }
			set {
				_currentImageUrl = value;
				RaisePropertyChanged (() => CurrentImageUrl);
			}
		}

		private ICommand _nextCommand;

		public ICommand NextCommand {
			get { 
				_nextCommand = _nextCommand ?? new MvvmCross.Commands.MvxCommand (Next);
				return _nextCommand;
			}
		}



		private ICommand _previousCommand;

		public ICommand PreviousCommand {
			get { 
				_previousCommand = _previousCommand ?? new MvvmCross.Commands.MvxCommand (Previous);
				return _previousCommand;
			}
		}

		private void Previous ()
		{
			if (_index > 0) {
				_index--;
				CurrentImageUrl = _imageUrls [_index];
			}
		}

		private void Next ()
		{
			if (_index == _imageUrls.Count - 1) {
			} else if (_index < _imageUrls.Count) {
				_index++;
				CurrentImageUrl = _imageUrls [_index];
			}
		}
	}
}

