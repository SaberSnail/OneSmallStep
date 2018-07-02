using GoldenAnvil.Utility;

namespace OneSmallStep
{
	public abstract class ViewModelBase : NotifyPropertyChangedBase
	{
		public AppModel AppModel => AppModel.Current;
	}
}
