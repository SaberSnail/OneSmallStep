using System;
using System.Collections.Generic;
using GoldenAnvil.Utility;

namespace OneSmallStep.ECS
{
	public abstract class ComponentBase
	{
		public int Version { get; private set; }

		public abstract ComponentBase Clone();

		protected ComponentBase()
		{
		}

		protected ComponentBase(ComponentBase that)
		{
			Version = that.Version;
		}

		protected void SetChanged()
		{
			Version++;
		}

		protected IDisposable ScopedPropertyChange()
		{
			return Scope.Create( () => Version++);
		}

		protected bool SetPropertyField<T>(T newValue, ref T field)
		{
			return SetPropertyField(newValue, ref field, EqualityComparer<T>.Default);
		}

		protected bool SetPropertyField<T>(T newValue, ref T field, IEqualityComparer<T> comparer)
		{
			if (comparer.Equals(field, newValue))
				return false;

			using (ScopedPropertyChange())
				field = newValue;
			return true;
		}
	}
}
