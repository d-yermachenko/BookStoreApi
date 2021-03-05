using BookStoreApi.Code;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreApiTests.Units {
    [TestClass]
    public class DataToolsTests {

        [TestMethod]
        public void TestCaseInserted() {
            Point[] oldPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(2, 3)
            };

            Point[] newPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(4, 3)
            };

            var differences = DataTools.FindDifferences<Point, Point>(oldPoints, newPoints, (o, n) => o==n);
            Assert.IsTrue(
                differences.InsertedItems.First() == new Point(4, 3)
                && differences.RemovedItems.First() == new Point(2, 3)
                && differences.StayedItems.Count == 2
                );
        }


        [TestMethod]
        public void TestCaseAllInserted() {
            Point[] oldPoints = Array.Empty<Point>();

            Point[] newPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(4, 3)
            };

            var differences = DataTools.FindDifferences<Point, Point>(oldPoints, newPoints, (o, n) => o == n);
            Assert.IsTrue(
                differences.InsertedItems.Count ==3
                && differences.RemovedItems.Count == 0
                && differences.StayedItems.Count == 0
                );
        }

        [TestMethod]
        public void TestCaseAllRemoved() {
            Point[] oldPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(2, 3)
            };

            Point[] newPoints = Array.Empty<Point>();

            var differences = DataTools.FindDifferences<Point, Point>(oldPoints, newPoints, (o, n) => o == n);
            Assert.IsTrue(
                differences.InsertedItems.Count == 0
                && differences.RemovedItems.Count == 3
                && differences.StayedItems.Count == 0
                );
        }

        [TestMethod]
        public void TestCaseNothingChanged() {
            Point[] oldPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(2, 3)
            };

            Point[] newPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(2, 3)
            };

            var differences = DataTools.FindDifferences<Point, Point>(oldPoints, newPoints, (o, n) => o == n);
            Assert.IsTrue(
                 differences.InsertedItems.Count == 0
                 && differences.RemovedItems.Count == 0
                 && differences.StayedItems.Count == 3
                 );
        }

        [TestMethod]
        public void TestPointsWithSize() {
            Point[] oldPoints = new Point[] {
                new Point(1, 4),
                new Point(2, 5),
                new Point(2, 3)
            };

            Size[] newPoints = new Size[] {
                new Size(1, 4),
                new Size(2, 5),
                new Size(4, 3)
            };

            var differences = DataTools.FindDifferences<Point, Size>(oldPoints, newPoints, (o, n) => o.Equals(new Point(n)));
            Assert.IsTrue(
                differences.InsertedItems.First() == new Size(4, 3)
                && differences.RemovedItems.First() == new Point(2, 3)
                && differences.StayedItems.Count == 2
                );
        }
    }
}
