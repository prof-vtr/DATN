using System;
using System.Collections.Generic;
using System.Linq;
using DoAnNguyenVanTruong.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAnNguyenVanTruong.Service
{
    public class ProductRecommendationService
    {
        private readonly BaseAppDbContext _dbContext;

        public ProductRecommendationService(BaseAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Product> GetRecommendedProducts(int userId, int numberOfRecommendations = 4)
        {
            // 1. Get the target user's purchased products
            var targetUserProducts = _dbContext.Orders
                .Where(o => o.UserID == userId)
                .SelectMany(o => o.OrderDetails)
                .Select(od => od.Product_Size.ProductID)
                .Distinct()
                .ToList();

            // If user has no purchase history, return popular products
            if (!targetUserProducts.Any())
            {
                return _dbContext.Products
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(numberOfRecommendations)
                    .ToList();
            }

            // 2. Find users who purchased at least one of the same products
            var similarUserIds = _dbContext.Orders
                .Where(o => o.UserID != userId && o.OrderDetails.Any(od => targetUserProducts.Contains(od.Product_Size.ProductID)))
                .Select(o => o.UserID)
                .Distinct()
                .ToList();

            // 3. Calculate similarity scores between target user and other users
            var userSimilarities = new Dictionary<int, double>();

            foreach (var similarUserId in similarUserIds)
            {
                var similarUserProducts = _dbContext.Orders
                    .Where(o => o.UserID == similarUserId)
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => od.Product_Size.ProductID)
                    .Distinct()
                    .ToList();

                // Calculate Jaccard similarity (intersection over union)
                var intersection = targetUserProducts.Intersect(similarUserProducts).Count();
                var union = targetUserProducts.Union(similarUserProducts).Count();

                var similarity = (double)intersection / union;
                userSimilarities[(int)similarUserId] = similarity;
            }

            // 4. Get products purchased by similar users that the target user hasn't purchased
            var candidateProducts = new Dictionary<int, double>();

            foreach (var entry in userSimilarities.OrderByDescending(x => x.Value).Take(10)) // Top 10 similar users
            {
                var similarUserId = entry.Key;
                var similarityScore = entry.Value;

                var productsFromSimilarUser = _dbContext.Orders
                    .Where(o => o.UserID == similarUserId)
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => od.Product_Size.ProductID)
                    .Distinct()
                    .Where(pid => !targetUserProducts.Contains(pid))
                    .ToList();

                foreach (var productId in productsFromSimilarUser)
                {
                    if (!candidateProducts.ContainsKey((int)productId))
                        candidateProducts[(int)productId] = 0;

                    // We're not adding scores, just counting how many similar users bought this product
                    candidateProducts[(int)productId] += 1;
                }
            }

            // 5. Get the recommended products
            var recommendedProductIds = candidateProducts
                .OrderByDescending(x => x.Value) // Sort by how many similar users purchased this product
                .Select(x => x.Key)
                .Take(numberOfRecommendations)
                .ToList();

            var recommendedProducts = _dbContext.Products
                .Where(p => recommendedProductIds.Contains(p.ProductID))
                .ToList();

            // 6. If we don't have enough recommendations, add products from categories the user has purchased
            if (recommendedProducts.Count < numberOfRecommendations)
            {
                // Get categories the user has purchased
                var userBrands = _dbContext.Products
                    .Where(p => targetUserProducts.Contains(p.ProductID))
                    .Select(p => p.CategoryID)
                    .Distinct()
                    .ToList();

                // Get popular products from those brands that the user hasn't purchased
                var additionalProducts = _dbContext.Products
                    .Where(p => userBrands.Contains(p.CategoryID) && !targetUserProducts.Contains(p.ProductID) && !recommendedProductIds.Contains(p.ProductID))
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(numberOfRecommendations - recommendedProducts.Count)
                    .ToList();

                recommendedProducts.AddRange(additionalProducts);
            }

            // 7. If we still don't have enough, add overall popular products
            if (recommendedProducts.Count < numberOfRecommendations)
            {
                var existingIds = recommendedProducts.Select(p => p.ProductID)
                    .ToList() // Materialize the first collection
                    .Union(targetUserProducts.Cast<int>()) // Cast the second collection to match
                    .ToList();

                var popularProducts = _dbContext.Products
                    .Where(p => !existingIds.Contains(p.ProductID))
                    .OrderByDescending(p => p.TotalRevenue)
                    .Take(numberOfRecommendations - recommendedProducts.Count)
                    .ToList();

                recommendedProducts.AddRange(popularProducts);
            }

            return recommendedProducts;
        }

        public static List<List<T>> GetCombinations<T>(List<T> list, int length)
        {
            var result = new List<List<T>>();
            GetCombinationsRecursive(list, new List<T>(), length, 0, result);
            return result;
        }

        private static void GetCombinationsRecursive<T>(
            List<T> input,
            List<T> current,
            int length,
            int index,
            List<List<T>> result)
        {
            if (current.Count == length)
            {
                result.Add(new List<T>(current));
                return;
            }

            for (int i = index; i < input.Count; i++)
            {
                current.Add(input[i]);
                GetCombinationsRecursive(input, current, length, i + 1, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}