using CommonUtils.LocalPersistence;
using NUnit.Framework;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocalPersistanceTests {
	[Serializable]
	private class TestEntity : AbstractEntity<float> {
		[SerializeField] private string testString;

		public string TestString { get => testString; set => testString = value; }
	}


    [Test]
    public void LocalPersistance_SaveAndLoadEntity() {
		var entity = new TestEntity() {
			Id = Random.value,
			TestString = "This is a test"
		};

		var saveKey = entity.GetSaveKey<TestEntity, float>();

		entity.SaveLocal<TestEntity, float>();

		var newEntity = PlayerPrefsDb.RetrieveFromLocal<TestEntity, float>(entity.Id);

		PlayerPrefs.DeleteKey(saveKey);

		Assert.AreEqual(entity.TestString, newEntity.TestString);
	}
}
