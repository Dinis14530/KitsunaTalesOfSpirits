using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class MusicPuzzleManagerTests
{
    private MusicPuzzleManager puzzle;
    private GameObject gameObject;
    private bool solvedFired;
    private bool failedFired;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("MusicPuzzle");
        puzzle = gameObject.AddComponent<MusicPuzzleManager>();
        puzzle.correctSequence = new List<int> { 0, 2, 1, 3 };
        puzzle.onPuzzleSolved = new UnityEvent();
        puzzle.onPuzzleFailed = new UnityEvent();

        solvedFired = false;
        failedFired = false;
        puzzle.onPuzzleSolved.AddListener(() => solvedFired = true);
        puzzle.onPuzzleFailed.AddListener(() => failedFired = true);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void CorrectSequence_FiresSolvedEvent()
    {
        puzzle.RegisterNote(0);
        puzzle.RegisterNote(2);
        puzzle.RegisterNote(1);
        puzzle.RegisterNote(3);

        Assert.IsTrue(solvedFired);
        Assert.IsFalse(failedFired);
    }

    [Test]
    public void WrongFirstNote_FiresFailedEvent()
    {
        puzzle.RegisterNote(1);

        Assert.IsTrue(failedFired);
        Assert.IsFalse(solvedFired);
    }

    [Test]
    public void WrongSecondNote_FiresFailedEvent()
    {
        puzzle.RegisterNote(0);
        puzzle.RegisterNote(3);

        Assert.IsTrue(failedFired);
        Assert.IsFalse(solvedFired);
    }

    [Test]
    public void PartialCorrectSequence_DoesNotFireAnyEvent()
    {
        puzzle.RegisterNote(0);
        puzzle.RegisterNote(2);

        Assert.IsFalse(solvedFired);
        Assert.IsFalse(failedFired);
    }

    [Test]
    public void AfterSolving_FurtherNotesAreIgnored()
    {
        puzzle.RegisterNote(0);
        puzzle.RegisterNote(2);
        puzzle.RegisterNote(1);
        puzzle.RegisterNote(3);
        Assert.IsTrue(solvedFired);

        solvedFired = false;
        failedFired = false;

        puzzle.RegisterNote(0);

        Assert.IsFalse(solvedFired);
        Assert.IsFalse(failedFired);
    }

    [Test]
    public void AfterFailure_CanRetryAndSolve()
    {
        puzzle.RegisterNote(1);
        Assert.IsTrue(failedFired);
        failedFired = false;

        puzzle.RegisterNote(0);
        puzzle.RegisterNote(2);
        puzzle.RegisterNote(1);
        puzzle.RegisterNote(3);

        Assert.IsTrue(solvedFired);
    }
}
