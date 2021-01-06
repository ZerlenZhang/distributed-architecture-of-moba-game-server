using ReadyGamerOne.Data;
using UnityEngine;
using System.Collections;

namespace PurificationPioneer.Data
{
	[System.Serializable]
	public  partial class BulletData : CsvMgr
	{
		public const int BulletDataCount = 1;

		public override string ID => bulletId.ToString();

		public int bulletId;
		public int speed;
		public int maxLife;
		public int damage;
		public string statement;
		public override string ToString()
		{
			var ans="==《	BulletData	》==\n" +
					"bulletId" + "	" + bulletId+"\n" +
					"speed" + "	" + speed+"\n" +
					"maxLife" + "	" + maxLife+"\n" +
					"damage" + "	" + damage+"\n" +
					"statement" + "	" + statement;
			return ans;

		}

	}
}

