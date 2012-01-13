using System;
using System.Collections.Generic;

namespace TernarySearchTree
{
	internal class Unsubscriber<TS> : IDisposable
	{
		private readonly IList<IObserver<IEnumerable<TS>>> _observers;
		private readonly IObserver<IEnumerable<TS>> _observer;

		public Unsubscriber(IList<IObserver<IEnumerable<TS>>> observers, IObserver<IEnumerable<TS>> observer)
		{
			_observers = observers;
			_observer = observer;
		}

		public void Dispose()
		{
			if (_observer != null && _observers.Contains(_observer))
				_observers.Remove(_observer);
		}
	}
}