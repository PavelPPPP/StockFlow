using StockFlow.Domain.Entities;

namespace StockFlow.Domain.UnitTests.Entities
{
    public class CategoryTests
    {
        [Fact]
        public void Create_WithValidRootCategory_ShouldSucceed()
        {
            // Arrange
            var name = "Electronics";

            // Act
            var category = Category.Create(name, parentCategoryId: null);

            // Assert
            Assert.Equal(name, category.Name);
            Assert.Null(category.ParentCategoryId);
        }

        [Fact]
        public void Create_WithValidParentCategoryId_ShouldSucceed()
        {
            // Arrage
            var name = "Laptops";
            var parentId = Guid.NewGuid();

            // Act
            var category = Category.Create(name, parentCategoryId: parentId);

            // Assert
            Assert.Equal(name, category.Name);
            Assert.Equal(parentId, category.ParentCategoryId);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Category.Create(string.Empty, null));
        }
    }
}