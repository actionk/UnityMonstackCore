using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Plugins.UnityMonstackCore.Utils.Trees
{
    public class KdTree2d<T> : IEnumerable<T> where T : class, IKdTree2dEntry
    {
        protected KdNode _root;
        protected KdNode _last;
        protected int _count;
        protected float _LastUpdate = -1f;
        protected KdNode[] _open;

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public float AverageSearchLength { protected set; get; }
        public float AverageSearchDeep { protected set; get; }

        public IKdTree2dEntry this[int key]
        {
            get
            {
                if (key >= _count)
                    throw new ArgumentOutOfRangeException();
                var current = _root;
                for (var i = 0; i < key; i++)
                    current = current.next;
                return current.entry;
            }
        }

        /// <summary>
        /// add item
        /// </summary>
        /// <param name="item">item</param>
        public void Add(T item)
        {
            _add(new KdNode() {entry = item});
        }

        /// <summary>
        /// batch add items
        /// </summary>
        /// <param name="items">items</param>
        public void AddAll(List<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// find all objects that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        public KdTree2d<T> FindAll(Predicate<T> match)
        {
            var list = new KdTree2d<T>();
            foreach (var node in this)
                if (match(node))
                    list.Add(node);
            return list;
        }

        /// <summary>
        /// find first object that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        public T Find(Predicate<T> match)
        {
            var current = _root;
            while (current != null)
            {
                if (match(current.entry))
                    return current.entry;
                current = current.next;
            }

            return null;
        }

        /// <summary>
        /// Remove at position i (position in list or loop)
        /// </summary>
        public void RemoveAt(int i)
        {
            var list = new List<KdNode>(_getNodes());
            list.RemoveAt(i);
            RestructTree(list);
        }

        /// <summary>
        /// remove all objects that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        public void RemoveAll(Predicate<T> match)
        {
            var list = new List<KdNode>(_getNodes());
            list.RemoveAll(n => match(n.entry));
            RestructTree(list);
        }

        /// <summary>
        /// remove an object that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        public bool Remove(Predicate<T> match)
        {
            var list = new List<KdNode>(_getNodes());
            var kdNodeToRemove = list.FindIndex(n => match(n.entry));
            if (kdNodeToRemove == -1)
                return false;
            list.RemoveAt(kdNodeToRemove);

            RestructTree(list);
            return true;
        }

        private void RestructTree(List<KdNode> list)
        {
            Clear();
            foreach (var node in list)
            {
                node._oldRef = null;
                node.next = null;
            }

            foreach (var node in list)
                _add(node);
        }

        /// <summary>
        /// count all objects that matches the given predicate
        /// </summary>
        /// <param name="match">lamda expression</param>
        /// <returns>matching object count</returns>
        public int CountAll(Predicate<T> match)
        {
            int count = 0;
            foreach (var node in this)
                if (match(node))
                    count++;
            return count;
        }

        /// <summary>
        /// clear tree
        /// </summary>
        public void Clear()
        {
            //rest for the garbage collection
            _root = null;
            _last = null;
            _count = 0;
        }

        /// <summary>
        /// Update positions (if objects moved)
        /// </summary>
        /// <param name="rate">Updates per second</param>
        public void UpdatePositions(float rate)
        {
            if (Time.timeSinceLevelLoad - _LastUpdate < 1f / rate)
                return;

            _LastUpdate = Time.timeSinceLevelLoad;

            UpdatePositions();
        }

        /// <summary>
        /// Update positions (if objects moved)
        /// </summary>
        public void UpdatePositions()
        {
            //save old traverse
            var current = _root;
            while (current != null)
            {
                current._oldRef = current.next;
                current = current.next;
            }

            //save root
            current = _root;

            //reset values
            Clear();

            //readd
            while (current != null)
            {
                _add(current);
                current = current._oldRef;
            }
        }

        /// <summary>
        /// Method to enable foreach-loops
        /// </summary>
        /// <returns>Enumberator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = _root;
            while (current != null)
            {
                yield return current.entry;
                current = current.next;
            }
        }

        /// <summary>
        /// Convert to list
        /// </summary>
        /// <returns>list</returns>
        public List<T> ToList()
        {
            var list = new List<T>();
            foreach (var node in this)
                list.Add(node);
            return list;
        }

        /// <summary>
        /// Method to enable foreach-loops
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected float _getSplitValue(int level, int2 position)
        {
            return (level % 2 == 0) ? position.x : position.y;
        }

        private void _add(KdNode newNode)
        {
            _count++;
            newNode.left = null;
            newNode.right = null;
            newNode.level = 0;
            var parent = _findParent(newNode.entry.Position);

            //set last
            if (_last != null)
                _last.next = newNode;
            _last = newNode;

            //set root
            if (parent == null)
            {
                _root = newNode;
                return;
            }

            var splitParent = _getSplitValue(parent);
            var splitNew = _getSplitValue(parent.level, newNode.entry.Position);

            newNode.level = parent.level + 1;

            if (splitNew < splitParent)
                parent.left = newNode; //go left
            else
                parent.right = newNode; //go right
        }

        private KdNode _findParent(int2 position)
        {
            //travers from root to bottom and check every node
            var current = _root;
            var parent = _root;
            while (current != null)
            {
                var splitCurrent = _getSplitValue(current);
                var splitSearch = _getSplitValue(current.level, position);

                parent = current;
                if (splitSearch < splitCurrent)
                    current = current.left; //go left
                else
                    current = current.right; //go right
            }

            return parent;
        }

        /// <summary>
        /// Find closest object to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>closest object</returns>
        public T FindClosest(int2 position)
        {
            return _findClosest(position);
        }

        /// <summary>
        /// Find close objects to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <returns>close object</returns>
        public List<T> FindClose(int2 position)
        {
            var output = new List<T>();
            _findClosest(position, output);
            return output;
        }

        /// <summary>
        /// Find close objects to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="maxDistance">maxDistance</param>
        /// <returns>close object</returns>
        public List<T> FindClose(int2 position, float maxDistance)
        {
            var output = new List<T>();
            _findClosest(position, output, maxDistance);
            return output;
        }

        /// <summary>
        /// Find close objects to given position
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="maxDistance">maxDistance</param>
        /// <returns>close object</returns>
        public List<T> FindClose(int2 position, float maxDistance, Predicate<T> selector)
        {
            var output = new List<T>();
            _findClosest(position, output, maxDistance, selector);
            return output;
        }

        public T FindClosest(int2 position, Predicate<T> selector)
        {
            return _findClosest(position, null, -1, selector);
        }

        protected T _findClosest(int2 position, List<T> traversed = null, float maxDistance = -1, Predicate<T> selector = null)
        {
            if (_root == null)
                return null;

            var nearestDist = float.MaxValue;
            KdNode nearest = null;

            if (_open == null || _open.Length < Count)
                _open = new KdNode[Count];
            for (int i = 0; i < _open.Length; i++)
                _open[i] = null;

            var openAdd = 0;
            var openCur = 0;

            if (_root != null)
                _open[openAdd++] = _root;

            while (openCur < _open.Length && _open[openCur] != null)
            {
                var current = _open[openCur++];
                var distanceToNode = math.distance(position, current.entry.Position);

                var isCloseEnough = maxDistance < 0 || distanceToNode <= maxDistance;
                var isSelected = selector == null || selector.Invoke(current.entry);

                if (isCloseEnough && isSelected)
                {
                    traversed?.Add(current.entry);

                    if (distanceToNode < nearestDist)
                    {
                        nearestDist = distanceToNode;
                        nearest = current;
                    }
                }

                var splitCurrent = _getSplitValue(current);
                var splitSearch = _getSplitValue(current.level, position);

                if (splitSearch < splitCurrent)
                {
                    if (current.left != null)
                        _open[openAdd++] = current.left; //go left
                    if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.right != null)
                        _open[openAdd++] = current.right; //go right
                }
                else
                {
                    if (current.right != null)
                        _open[openAdd++] = current.right; //go right
                    if (Mathf.Abs(splitCurrent - splitSearch) * Mathf.Abs(splitCurrent - splitSearch) < nearestDist && current.left != null)
                        _open[openAdd++] = current.left; //go left
                }
            }

            if (nearest == null)
                return null;

            AverageSearchLength = (99f * AverageSearchLength + openCur) / 100f;
            AverageSearchDeep = (99f * AverageSearchDeep + nearest.level) / 100f;

            return nearest.entry;
        }

        private float _getSplitValue(KdNode node)
        {
            return _getSplitValue(node.level, node.entry.Position);
        }

        private IEnumerable<KdNode> _getNodes()
        {
            var current = _root;
            while (current != null)
            {
                yield return current;
                current = current.next;
            }
        }

        protected class KdNode
        {
            internal T entry;
            internal int level;
            internal KdNode left;
            internal KdNode right;
            internal KdNode next;
            internal KdNode _oldRef;
        }
    }
}