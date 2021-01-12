using ReadyGamerOne.Script;
using ReadyGamerOne.MemorySystem;
namespace PurificationPioneer.Script
{
	public partial class PurificationPioneerMgr : AbstractGameMgr<PurificationPioneerMgr>
	{
		protected override IResourceLoader ResourceLoader => ResourcesResourceLoader.Instance;
		protected override IAssetConstUtil AssetConstUtil => Utility.AssetConstUtil.Instance;
		partial void OnSafeAwake();
		protected override void OnStateIsNull()
		{
			base.OnStateIsNull();
			OnSafeAwake();
		}
	}
}
