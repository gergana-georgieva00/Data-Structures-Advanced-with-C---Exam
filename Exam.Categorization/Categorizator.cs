using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Categorization
{
    public class Categorizator : ICategorizator
    {
        Dictionary<string, Category> categoriesById = new Dictionary<string, Category>();
        Dictionary<string, Dictionary<string, Category>> categoriesByParentId = new Dictionary<string, Dictionary<string, Category>>();
        Dictionary<string, string> parentsByCategoryId = new Dictionary<string, string>();
        Dictionary<string, int> categoriesDepths = new Dictionary<string, int>();

        public void AddCategory(Category category)
        {
            if (categoriesById.ContainsKey(category.Id))
            {
                throw new ArgumentException();
            }

            categoriesById.Add(category.Id, category);
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
