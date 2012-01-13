using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TernarySearchTree
{
	public class TernarySearchTree<TS> : IObservable<IEnumerable<TS>>, IEquatable<TernarySearchTree<TS>.Node>
        where TS : IComparable<TS>, IEquatable<TS>
    {
        public TernarySearchTree(IList<IEnumerable<TS>> searchObjects, IObservable<TS> streamToMatchAgainstTree)
        {
            if (searchObjects.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException("Nothing to build tree from!");
            if (streamToMatchAgainstTree == null)
                throw new ArgumentOutOfRangeException("Nothing to match against!");
            CreateTree(searchObjects);
            Node currentNode = _root;
            streamToMatchAgainstTree.Subscribe(ts =>
                {
                    if (_root == null)
                        return;
                    MatchResult mr = currentNode.FindMatch(ts, out currentNode);
                    if (mr.DeadEnd)
                    {
                        currentNode = _root;
                        return;
                    }
                    if (mr.Hit)
                    {
                        _observers.OnNext(mr.Match);
                    }
                });
        }

        Node _root;
        private readonly IList<IObserver<IEnumerable<TS>>> _observers = new List<IObserver<IEnumerable<TS>>>();
        private int _branchId;

        public class MatchResult
        {
            public bool DeadEnd { get; set; }

            public bool Hit { get; set; }

            public IEnumerable<TS> Match { get; set; }
        }

        public class Node : IComparable<TS>
        {
            private readonly IList<IObserver<IEnumerable<TS>>> _observers;
            private IEnumerable<TS> _match;
            TS _wrappee;
            public Node(TS wrappee, int branchId, IEnumerable<TS> match = null, IList<IObserver<IEnumerable<TS>>> observers = null)
            {
                _wrappee = wrappee;
                _match = match;
                _observers = observers;
                _left = null;
                _right = null;
                _middle = null;
                _branchId = branchId;
            }

            public Node()
                : this(default(TS), -1)
            {
            }


            protected Node _left;
            protected Node _right;
            protected Node _middle;
            private int _branchId = -1;

			public int CompareTo(Node other)
            {
                return _wrappee.CompareTo(other._wrappee);
            }

            public int CompareTo(TS other)
            {
                return _wrappee.CompareTo(other);
            }

            public bool AppendNode(Node node)
            {
                if (node.Equals(_wrappee) && _branchId != node._branchId)
                {
                    return false;
                }
                if (_middle == null)
                {
                    _middle = node;
                    return true;
                }
                if (node.CompareTo(this) < 0)
                {
                    if (_middle != null && node.CompareTo(_middle) <= 0)//Also less than middle?
                        return _middle.AppendNode(node);
                    if (_left != null)
                        _left.AppendNode(node);
                    else
                        _left = node;
                    return true;
                }
                if (node.CompareTo(this) > 0)
                {
                    if (node.CompareTo(_middle) >= 0)//Also greater than middle?
                        return _middle.AppendNode(node);
                    else
                    {
                        if (_right != null)
                            _right.AppendNode(node);
                        else
                            _right = node;
                        return true;
                    }
                }
                return false;
            }

            internal void AppendBranch(IEnumerable<TS> branch, int branchId)
            {
                Contract.Requires(branch != null); 
                var lastAddedNode = this;
                int index = 0;
                foreach (var item in branch.Take(branch.Count() - 1))
                {
                    var newNode = new Node(item, branchId);
                    if (lastAddedNode.AppendNode(newNode))
                        lastAddedNode = newNode;
                    index++;
                }
                lastAddedNode.AppendNode(new Node(branch.Last(), branchId));
            }

            public MatchResult FindMatch(TS ts, out Node nextNode)
            {
                if (_wrappee.CompareTo(ts) == 0)
                {
                    if (_match != null)
                    {
                        nextNode = null;
                        return new MatchResult(){Hit = true, DeadEnd = false, Match = _match};
                    }
                    nextNode = _middle;
                    return new MatchResult() { Hit = false, DeadEnd = false, Match = null };
                }
                if (_wrappee.CompareTo(ts) < 0 && _right != null)
                {
                    return _right.FindMatch(ts, out nextNode);
                }
            	if (_left != null)
            	{
            		return _left.FindMatch(ts, out nextNode);
            	}
            	nextNode = null;
                return new MatchResult() { Hit = false, DeadEnd = true, Match = null };
            }

            public bool Equals(Node other)
            {
                return  other != null &&
                        other.Equals(_wrappee) &&

                        (other._right != null && 
                        other._right.Equals(_right) || (other._right == _right))
                        
                        &&
                        (other._left != null &&
                        other._left.Equals(_left) || (other._left == _left)) 
                        
                        &&
                        (other._middle != null &&
                        other._middle.Equals(_middle) || (other._middle == _middle));
            }


            public bool Equals(TS otherWrappee)
            {
                return (otherWrappee != null ? _wrappee.Equals(otherWrappee) : _wrappee == null);
            }
        };

        private void CreateTree(IEnumerable<IEnumerable<TS>> searchObjects)
        {
            foreach (var searchObject in searchObjects)
            {
                CreateMatchableBranch(searchObject);
            }
        }

        protected void CreateMatchableBranch(IEnumerable<TS> obj)
        {
            _branchId++;
            if (_root == null)
            {
                _root = _root ?? new Node(obj.First(), _branchId);
                obj = obj.Skip(1);
            }
            _root.AppendBranch(obj, _branchId);
        }

        public IDisposable Subscribe(IObserver<IEnumerable<TS>> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<TS>(_observers, observer);
        }

        public bool Equals(TernarySearchTree<TS>.Node other)
        {
            return _root.Equals(other);    
        }
    }
}