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
            categoriesByParentId.Add(category.Id, new Dictionary<string, Category>());
            categoriesDepths.Add(category.Id, 0);
        }

        public void AssignParent(string childCategoryId, string parentCategoryId)
        {
            if (!categoriesById.ContainsKey(childCategoryId) || !categoriesById.ContainsKey(parentCategoryId))
            {
                throw new ArgumentException();
            }
            if (categoriesByParentId[parentCategoryId].ContainsKey(childCategoryId))
            {
                throw new ArgumentException();
            }

            var child = categoriesById[childCategoryId];
            categoriesByParentId[parentCategoryId].Add(childCategoryId, child);
            parentsByCategoryId.Add(childCategoryId, parentCategoryId);

            string currentId = childCategoryId;
            while (parentsByCategoryId.ContainsKey(currentId))
            {
                string parentId = parentsByCategoryId[currentId];
                int currentDepth = this.categoriesDepths[currentId] + 1;
                int parentDepth = this.categoriesDepths[parentId];
                this.categoriesDepths[parentId] = Math.Max(currentDepth, parentDepth);
                currentId = parentId;
            }
        }

        public bool Contains(Category category)
            => categoriesById.ContainsKey(category.Id);

        public IEnumerable<Category> GetChildren(string categoryId)
        {
            if (!categoriesById.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            LinkedList<Category> children = new LinkedList<Category>();

            Queue<Category> currentCategories = new Queue<Category>();
            currentCategories.Enqueue(this.categoriesById[categoryId]);

            while (currentCategories.Count > 0)
            {
                Category current = currentCategories.Dequeue();

                foreach (Category category in this.categoriesByParentId[current.Id].Values)
                {
                    children.AddLast(category);
                    currentCategories.Enqueue(category);
                }
            }

            return children;
        }

        public IEnumerable<Category> GetHierarchy(string categoryId)
        {
            if (!categoriesById.ContainsKey(categoryId))
            {
                throw new ArgumentException();
            }

            LinkedList<Category> hierarchy = new LinkedList<Category>();
            hierarchy.AddLast(this.categoriesById[categoryId]);
            string currentId = categoryId;

            while (this.parentsByCategoryId.ContainsKey(currentId))
            {
                hierarchy.AddFirst(this.categoriesById[this.parentsByCategoryId[currentId]]);
                currentId = this.parentsByCategoryId[currentId];
            }

            return hierarchy;
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

            this.categoriesById.Remove(categoryId);
            if (this.parentsByCategoryId.ContainsKey(categoryId)) this.categoriesByParentId[this.parentsByCategoryId[categoryId]].Remove(categoryId);
            this.parentsByCategoryId.Remove(categoryId);
            List<string> categoryChildrenIds = this.categoriesByParentId[categoryId].Keys.ToList();
            categoryChildrenIds.ForEach(this.RemoveCategory);
            this.categoriesByParentId.Remove(categoryId);
            this.categoriesDepths.Remove(categoryId);
        }

        public int Size()
            => categoriesById.Count;
    }
}
