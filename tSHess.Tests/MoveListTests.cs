using System;
using System.Collections;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class MoveListTests
    {
        [Fact]
        public void NewList_StartsEmpty()
        {
            MoveList list = new MoveList();

            Assert.Empty(list);
            Assert.Null(list[0]);
        }

        [Fact]
        public void AddContainsRemove_WorksForSingleMove()
        {
            MoveList list = new MoveList();
            Move move = new Move(8, 24);

            list.Add(move);
            Assert.True(list.Contains(new Move(8, 24)));
            Assert.Single(list);

            list.Remove(new Move(8, 24));
            Assert.Empty(list);
            Assert.False(list.Contains(move));
        }

        [Fact]
        public void Add_DuplicateSuppressedByDefault()
        {
            MoveList list = new MoveList();
            Move move = new Move(8, 24);

            list.Add(move);
            list.Add(new Move(8, 24));

            Assert.Single(list);
        }

        [Fact]
        public void Add_DuplicateAllowed_WhenSuppressDuplicatesIsFalse()
        {
            MoveList list = new MoveList
            {
                SuppressDuplicates = false
            };

            list.Add(new Move(8, 24));
            list.Add(new Move(8, 24));

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void Merge_AppendsMoves()
        {
            MoveList left = new MoveList();
            MoveList right = new MoveList();

            left.Add(new Move(8, 24));
            right.Add(new Move(9, 17));

            left.Merge(right);

            Assert.Equal(2, left.Count);
            Assert.NotNull(left[8, 24]);
            Assert.NotNull(left[9, 17]);
        }

        [Fact]
        public void RemoveAt_InvalidIndex_DoesNotThrowOrChange()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));

            list.RemoveAt(-1);
            list.RemoveAt(99);

            Assert.Single(list);
        }

        [Fact]
        public void Indexers_ReturnExpectedMoves()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 25));

            Assert.Equal("B2-D2", list[1].ToString());
            Assert.NotNull(list[8, 24]);
            Assert.Null(list[24, 8]);
            Assert.Null(list[-1, 8]);
        }

        [Fact]
        public void Clone_CreatesIndependentMoveCopies()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 25));

            MoveList clone = list.Clone();
            clone.RemoveAt(0);

            Assert.Equal(2, list.Count);
            Assert.Single(clone);
            Assert.NotSame(list[0], clone[0]);
        }

        [Fact]
        public void CopyTo_CopiesItemsInOrder()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 25));

            Move[] arr = new Move[2];
            list.CopyTo(arr, 0);

            Assert.Equal("B1-D1", arr[0].ToString());
            Assert.Equal("B2-D2", arr[1].ToString());
        }

        [Fact]
        public void CopyTo_WithSmallTargetArray_Throws()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 25));

            Move[] arr = new Move[1];
            Assert.ThrowsAny<Exception>(() => list.CopyTo(arr, 0));
        }

        [Fact]
        public void Sort_KeepsAllMoves()
        {
            MoveHistoryTable.GetInstance().Clear();

            MoveList list = new MoveList();
            list.SuppressDuplicates = false;
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 17));
            list.Add(new Move(10, 18));

            MoveHistoryTable.GetInstance().AddMove(Color.White, new Move(9, 17));
            MoveHistoryTable.GetInstance().AddMove(Color.White, new Move(9, 17));
            MoveHistoryTable.GetInstance().AddMove(Color.White, new Move(8, 24));

            list.Sort(Color.White);

            Assert.Equal(3, list.Count);
            Assert.True(list.Contains(new Move(8, 24)));
            Assert.True(list.Contains(new Move(9, 17)));
            Assert.True(list.Contains(new Move(10, 18)));
        }

        [Fact]
        public void GetEnumerator_IteratesAllMoves()
        {
            MoveList list = new MoveList();
            list.Add(new Move(8, 24));
            list.Add(new Move(9, 25));

            int count = 0;
            foreach (object item in list)
            {
                Assert.IsType<Move>(item);
                count++;
            }

            Assert.Equal(2, count);
        }

        [Fact]
        public void ICollectionSurface_IsExposed()
        {
            MoveList list = new MoveList();
            ICollection collection = list;

            Assert.False(collection.IsSynchronized);
            Assert.NotNull(collection.SyncRoot);
        }
    }
}