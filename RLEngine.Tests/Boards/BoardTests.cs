using RLEngine.Boards;
using RLEngine.Entities;
using RLEngine.Utils;
using RLEngine.Tests.Utils;

using NUnit.Framework;

namespace RLEngine.Tests.Boards
{
    [TestFixture]
    public class BoardsTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 4)]
        public void BoardCreatedPasses(int width, int height)
        {
            var f = new ContentFixture();

            var size = new Size(width, height);
            var board = new Board(size, f.FloorTileType);

            Assert.That(board.Size, Is.EqualTo(size));
            for (var i = 0; i < board.Size.Y; i++)
            {
                for (var j = 0; j < board.Size.X; j++)
                {
                    var tileType = board.GetTileType(new Coords(j, i)).FailIfNull();
                    Assert.That(tileType, Is.SameAs(f.FloorTileType));
                }
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(2, 2)]
        public void AddPasses(int x, int y)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var position = new Coords(x, y);
            var added = board.Add(entity, position);
            Assert.That(added, Is.True);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.EqualTo(position));

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.Member(entity));
        }

        [Test]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        [TestCase(3, 0)]
        public void AddFailsOutOfBounds(int x, int y)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var position = new Coords(x, y);
            var added = board.Add(entity, position);
            Assert.That(added, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.Null);

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.No.Member(entity));
        }

        [Test]
        public void AddPassesWithCompatibleEntity()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entityA = new Entity(f.GroundEntityType);
            var entityB = new Entity(f.GhostAgentType);

            var position = new Coords(1, 1);
            board.Add(entityA, position);
            var added = board.Add(entityB, position);
            Assert.That(added, Is.True);

            var entityAPosition = board.GetCoords(entityA);
            Assert.That(entityAPosition, Is.EqualTo(position));

            var entityBPosition = board.GetCoords(entityB);
            Assert.That(entityBPosition, Is.EqualTo(position));

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.Member(entityA));
            Assert.That(entities, Has.Member(entityB));
        }

        [Test]
        public void AddFailsWithIncompatibleEntity()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entityA = new Entity(f.GroundEntityType);
            var entityB = new Entity(f.GroundEntityType);

            var position = new Coords(1, 1);
            board.Add(entityA, position);
            var added = board.Add(entityB, position);
            Assert.That(added, Is.False);

            var entityAPosition = board.GetCoords(entityA);
            Assert.That(entityAPosition, Is.EqualTo(position));

            var entityBPosition = board.GetCoords(entityB);
            Assert.That(entityBPosition, Is.Null);

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.Member(entityA));
            Assert.That(entities, Has.No.Member(entityB));
        }

        [Test]
        public void AddFailsWithIncompatibleTile()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.WallTileType);
            var entity = new Entity(f.GroundEntityType);

            var position = new Coords(1, 1);
            var added = board.Add(entity, position);
            Assert.That(added, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.Null);

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.No.Member(entity));
        }

        [Test]
        [TestCase(0, 1, 2,  0, false)]
        [TestCase(0, 1, 2, -1, true)]
        [TestCase(0, 0, 2,  2, false)]
        [TestCase(0, 0, 2,  2,  true)]
        [TestCase(1, 1, 1,  1, false)]
        [TestCase(1, 1, 0,  0, true)]
        public void MovePasses(int ix, int iy, int fx, int fy, bool relative)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var initialPosition = new Coords(ix, iy);
            board.Add(entity, initialPosition);

            var finalPosition = new Coords(fx, fy);
            var moved = board.Move(entity, finalPosition, relative);
            Assert.That(moved, Is.True);

            if (relative) finalPosition += initialPosition;

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.EqualTo(finalPosition));

            if (initialPosition != finalPosition)
            {
                var initialEntities = board.GetEntities(initialPosition);
                Assert.That(initialEntities, Has.No.Member(entity));
            }

            var finalEntities = board.GetEntities(finalPosition);
            Assert.That(finalEntities, Has.Member(entity));
        }

        [Test]
        [TestCase(0, 0,  0, -1, false)]
        [TestCase(0, 0,  0, -1,  true)]
        [TestCase(1, 1, -1, -1, false)]
        [TestCase(1, 1, -2, -2,  true)]
        [TestCase(1, 2,  3,  0, false)]
        [TestCase(1, 2,  2,  0,  true)]
        public void MoveFailsOutOfBounds(int ix, int iy, int fx, int fy, bool relative)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var initialPosition = new Coords(ix, iy);
            board.Add(entity, initialPosition);

            var finalPosition = new Coords(fx, fy);
            var moved = board.Move(entity, finalPosition, relative);
            Assert.That(moved, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.EqualTo(initialPosition));

            var initialEntities = board.GetEntities(initialPosition);
            Assert.That(initialEntities, Has.Member(entity));

            var finalEntities = board.GetEntities(finalPosition);
            Assert.That(finalEntities, Has.No.Member(entity));
        }

        [Test]
        public void MoveFailsWhenEntityIsNotAdded()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var finalPosition = new Coords(1, 1);
            var moved = board.Move(entity, finalPosition, false);
            Assert.That(moved, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.Null);

            var finalTile = board.GetEntities(finalPosition);
            Assert.That(finalTile, Has.No.Member(entity));
        }

        [Test]
        public void MovePassesWithCompatibleEntity()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entityA = new Entity(f.GroundEntityType);
            var entityB = new Entity(f.GhostAgentType);

            var initialPosition = new Coords(0, 1);
            board.Add(entityA, initialPosition);

            var finalPosition = new Coords(2, 1);
            board.Add(entityB, finalPosition);
            var moved = board.Move(entityA, finalPosition, false);
            Assert.That(moved, Is.True);

            var entityAPosition = board.GetCoords(entityA);
            Assert.That(entityAPosition, Is.EqualTo(finalPosition));

            var entityBPosition = board.GetCoords(entityB);
            Assert.That(entityBPosition, Is.EqualTo(finalPosition));

            var initialEntities = board.GetEntities(initialPosition);
            Assert.That(initialEntities, Has.No.Member(entityA));

            var finalEntities = board.GetEntities(finalPosition);
            Assert.That(finalEntities, Has.Member(entityA));
            Assert.That(finalEntities, Has.Member(entityB));
        }

        [Test]
        public void MoveFailsWithIncompatibleEntity()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entityA = new Entity(f.GroundEntityType);
            var entityB = new Entity(f.GroundEntityType);

            var initialPosition = new Coords(0, 1);
            board.Add(entityA, initialPosition);

            var finalPosition = new Coords(2, 1);
            board.Add(entityB, finalPosition);
            var moved = board.Move(entityA, finalPosition, false);
            Assert.That(moved, Is.False);

            var entityAPosition = board.GetCoords(entityA);
            Assert.That(entityAPosition, Is.EqualTo(initialPosition));

            var entityBPosition = board.GetCoords(entityB);
            Assert.That(entityBPosition, Is.EqualTo(finalPosition));

            var initialEntities = board.GetEntities(initialPosition);
            Assert.That(initialEntities, Has.Member(entityA));

            var finalEntities = board.GetEntities(finalPosition);
            Assert.That(finalEntities, Has.No.Member(entityA));
            Assert.That(finalEntities, Has.Member(entityB));
        }

        [Test]
        public void MoveFailsWithIncompatibleTile()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var initialPosition = new Coords(0, 1);
            board.Add(entity, initialPosition);

            var finalPosition = new Coords(2, 1);
            board.Modify(f.WallTileType, finalPosition);
            var moved = board.Move(entity, finalPosition, false);
            Assert.That(moved, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.EqualTo(initialPosition));

            var initialEntities = board.GetEntities(initialPosition);
            Assert.That(initialEntities, Has.Member(entity));

            var finalEntities = board.GetEntities(finalPosition);
            Assert.That(finalEntities, Has.No.Member(entity));
        }

        [Test]
        public void RemovePasses()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var position = new Coords(1, 1);
            board.Add(entity, position);

            var removed = board.Remove(entity);
            Assert.That(removed, Is.True);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.Null);

            var entities = board.GetEntities(position);
            Assert.That(entities, Has.No.Member(entity));
        }

        [Test]
        public void RemoveFailsWhenEntityIsNotAdded()
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);
            var entity = new Entity(f.GroundEntityType);

            var removed = board.Remove(entity);
            Assert.That(removed, Is.False);

            var entityPosition = board.GetCoords(entity);
            Assert.That(entityPosition, Is.Null);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(2, 2)]
        public void ChangeTileTypePasses(int x, int y)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);

            var position = new Coords(x, y);
            var changed = board.Modify(f.WallTileType, position);
            Assert.That(changed, Is.True);

            var tileType = board.GetTileType(position);
            Assert.That(tileType, Is.SameAs(f.WallTileType));
        }

        [Test]
        [TestCase(0, -1)]
        [TestCase(-1, -1)]
        [TestCase(3, 0)]
        [TestCase(20, 30)]
        public void ChangeTileTypeFailsOutOfBounds(int x, int y)
        {
            var f = new ContentFixture();

            var board = new Board(new Size(3, 3), f.FloorTileType);

            var position = new Coords(x, y);
            var changed = board.Modify(f.WallTileType, position);
            Assert.That(changed, Is.False);

            var tileType = board.GetTileType(position);
            Assert.That(tileType, Is.Null);
        }
    }
}