using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListAPI.Controllers;
using TodoListAPI.Data;
using TodoListAPI.Models;
using Xunit;

namespace TodoListAPI.Tests
{
    public class TodosControllerTest
    {
        private readonly Mock<DbSet<TodoItem>> _mockSet;
        private readonly Mock<TodoContext> _mockContext;
        private readonly TodosController _controller;

        public TodosControllerTest()
        {
            _mockSet = new Mock<DbSet<TodoItem>>();
            _mockContext = new Mock<TodoContext>();
            _mockContext.Setup(m => m.TodoItems).Returns(_mockSet.Object);

            _controller = new TodosController(_mockContext.Object);
        }

        [Fact]
        public async Task GetTodoItems_ReturnsAllItems()
        {
            // Arrange
            var data = new List<TodoItem>
                {
                    new TodoItem { Id = 1, Title = "Test 1"},
                    new TodoItem { Id = 2, Title = "Test 2"},
                }.AsQueryable();

            //_mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(data.Provider);
            //_mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
            //_mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            // Act
            var result = await _controller.GetTodoItems();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TodoItem>>>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(actionResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetTodoItem_ReturnsItem_WhenItemExists()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test" };
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _controller.GetTodoItem(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
            var model = Assert.IsAssignableFrom<TodoItem>(actionResult.Value);
            Assert.Equal(todoItem.Id, model.Id);
        }

        [Fact]
        public async Task PostTodoItem_ReturnsCreatedAtAction_WhenItemIsAdded()
        {
            // Arrange
            var todoItem = new TodoItem { Title = "Test" };
            _mockSet.Setup(m => m.AddAsync(It.IsAny<TodoItem>(), default)).Returns(new ValueTask<EntityEntry<TodoItem>>(Task.FromResult((EntityEntry<TodoItem>)null)));

            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.PostTodoItem(todoItem);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TodoItem>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetTodoItem", createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task PutTodoItem_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Updated Title" };
            _mockContext.Setup(m => m.TodoItems.FindAsync(todoItem.Id)).ReturnsAsync(todoItem);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.PutTodoItem(todoItem.Id, todoItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test" };

            // Act
            var result = await _controller.PutTodoItem(2, todoItem); // ID mismatch

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_ReturnsBadRequest_OnException()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test" };
            _mockContext.Setup(m => m.TodoItems.FindAsync(todoItem.Id)).ReturnsAsync(todoItem);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.PutTodoItem(todoItem.Id, todoItem);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItem_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test" };
            _mockContext.Setup(m => m.TodoItems.FindAsync(todoItem.Id)).ReturnsAsync(todoItem);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteTodoItem(todoItem.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockContext.Setup(m => m.TodoItems.FindAsync(1)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _controller.DeleteTodoItem(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItem_ReturnsBadRequest_OnException()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test" };
            _mockContext.Setup(m => m.TodoItems.FindAsync(todoItem.Id)).ReturnsAsync(todoItem);
            _mockContext.Setup(m => m.TodoItems.Remove(todoItem)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteTodoItem(todoItem.Id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }


    }
}
