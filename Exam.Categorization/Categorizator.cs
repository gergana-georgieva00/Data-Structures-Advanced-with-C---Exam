using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Categorization
{
    public class Categorizator : ICategorizator
    {
        public class Tree<T>
        {
            public T value;
            private Tree<T> parent;
            public readonly List<Tree<T>> children;

            public Tree(T value)
            {
                this.value = value;
                this.children = new List<Tree<T>>();
            }

            public Tree(T value, params Tree<T>[] children)
                : this(value)
            {
                foreach (var child in children)
                {
                    child.parent = this;
                    this.children.Add(child);
                }
            }

            //public List<Tree<T>> ChildrenOfNode()
            //{
            //    return this.children;
            //}


            public void AddChild(T parentKey, Tree<T> child)
            {
                var parentNode = this.FindNodeWithBfs(parentKey);
                if (parentNode != null)
                {
                    parentNode.children.Add(child);
                    child.parent = parentNode;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }

            public Tree<T> FindNodeWithBfs(T parentKey)
            {
                var queue = new Queue<Tree<T>>();
                queue.Enqueue(this);

                while (queue.Count > 0)
                {
                    var subtree = queue.Dequeue();

                    if (subtree.value.Equals(parentKey))
                    {
                        return subtree;
                    }

                    foreach (var child in subtree.children)
                    {
                        queue.Enqueue(child);
                    }
                }

                return null;
            }

            public IEnumerable<T> OrderBfs()
            {
                var result = new List<T>();

                var queue = new Queue<Tree<T>>();
                queue.Enqueue(this);

                while (queue.Count > 0)
                {
                    var subtree = queue.Dequeue();
                    result.Add(subtree.value);

                    foreach (var child in subtree.children)
                    {
                        queue.Enqueue(child);
                    }
                }

                return result;
            }

            public IEnumerable<T> OrderDfs()
            {
                var result = new Stack<T>();
                var stack = new Stack<Tree<T>>();
                stack.Push(this);

                while (stack.Count > 0)
                {
                    var node = stack.Pop();

                    foreach (var child in node.children)
                    {
                        stack.Push(child);
                    }

                    result.Push(node.value);
                }

                return result;
            }

            public void RemoveNode(T nodeKey)
            {
                var toBeDeleted = FindNodeWithBfs(nodeKey);

                if (toBeDeleted is null)
                {
                    throw new ArgumentNullException();
                }

                var parentNode = toBeDeleted.parent;

                if (parentNode is null)
                {
                    throw new ArgumentException();
                }

                parentNode.children.Remove(toBeDeleted);
            }

            public void Swap(T firstKey, T secondKey)
            {
                var firstNode = this.FindNodeWithBfs(firstKey);
                var secondNode = this.FindNodeWithBfs(secondKey);

                if (firstNode is null || secondNode is null)
                {
                    throw new ArgumentNullException();
                }

                var firstParent = firstNode.parent;
                var secondParent = secondNode.parent;

                if (firstParent is null || secondParent is null)
                {
                    throw new ArgumentException();
                }

                var indexOfFirstChild = firstParent.children.IndexOf(firstNode);
                var indexOfSecondChild = secondParent.children.IndexOf(secondNode);

                firstParent.children[indexOfFirstChild] = secondNode;
                secondNode.parent = firstParent;

                secondParent.children[indexOfSecondChild] = firstNode;
                firstNode.parent = secondParent;
            }
        }

        private Tree<Category> tree;
        private Dictionary<string, Tree<Category>> categoriesById = new Dictionary<string, Tree<Category>>();

        public void AddCategory(Category category)
        {
            if (categoriesById.ContainsKey(category.Id))
            {
                throw new ArgumentException();
            }

            categoriesById.Add(category.Id, new Tree<Category>(category));
        }

        public void AssignParent(string childCategoryId, string parentCategoryId)
        {
            if (!categoriesById.ContainsKey(parentCategoryId) || !categoriesById.ContainsKey(childCategoryId))
            {
                throw new ArgumentException();
            }

            if (categoriesById[parentCategoryId].ChildrenOfNode().Contains(categoriesById[childCategoryId]))
            {
                throw new ArgumentException();
            }

            tree.AddChild(categoriesById[parentCategoryId].value, (categoriesById[childCategoryId]));
        }

        public bool Contains(Category category)
            => categoriesById.ContainsKey(category.Id);

        public IEnumerable<Category> GetChildren(string categoryId)
        {
            if (!categoriesById.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            return (IEnumerable<Category>)categoriesById[categoryId].ChildrenOfNode();
        }

        public IEnumerable<Category> GetHierarchy(string categoryId)
        {
            if (!categoriesById.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            return tree.OrderDfs();
        }

        public IEnumerable<Category> GetTop3CategoriesOrderedByDepthOfChildrenThenByName()
        {
            throw new NotImplementedException();
        }

        public void RemoveCategory(string categoryId)
        {
            if (!categoriesById.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            tree.RemoveNode(categoriesById[categoryId].value);
            categoriesById.Remove(categoryId);
        }

        public int Size()
            => categoriesById.Count;
    }
}
